using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;


public class HelloWorldManager : MonoBehaviour
{
    VisualElement rootVisualElement;
    Button hostButton;
    Button clientButton;

    // Creates the buttons for host and client joining
    void OnEnable()
    {
        var uiDocument = GetComponent<UIDocument>();
        rootVisualElement = uiDocument.rootVisualElement;

        hostButton = CreateButton("HostButton", "Host");
        clientButton = CreateButton("ClientButton", "Client");

        rootVisualElement.Clear();
        rootVisualElement.Add(hostButton);
        rootVisualElement.Add(clientButton);

        hostButton.clicked += OnHostButtonClicked;
        clientButton.clicked += OnClientButtonClicked;
    }

    void Start()
    {
        if (NetworkManager.Singleton == null)
            Debug.LogError("NetworkManager not found!");
        else
        {
            Debug.Log("NetworkManager found!");
        }
    }

    void Update()
    {
        UpdateUI();
    }

    // Gets rid of buttons
    void OnDisable()
    {
        hostButton.clicked -= OnHostButtonClicked;
        clientButton.clicked -= OnClientButtonClicked;
    }

    // Makes single instance of the host and client on button click
    void OnHostButtonClicked() => NetworkManager.Singleton.StartHost();
    void OnClientButtonClicked() => NetworkManager.Singleton.StartClient();

    private Button CreateButton(string name, string text)
    {
        var button = new Button();
        button.name = name;
        button.text = text;
        button.style.width = 240;
        button.style.backgroundColor = Color.white;
        button.style.color = Color.black;
        button.style.unityFontStyleAndWeight = FontStyle.Bold;
        return button;
    }

    void UpdateUI()
    {
        // If there is no client, disables buttons
        if (NetworkManager.Singleton == null)
        {
            SetStartButtons(false);
            return;
        }

        // If there is the host, shows buttons
        if (!NetworkManager.Singleton.IsClient)
        {
            SetStartButtons(true);
        }
        else
        {
            SetStartButtons(false);
        }
    }

    void SetStartButtons(bool state)
    {
        hostButton.style.display = state ? DisplayStyle.Flex : DisplayStyle.None;
        clientButton.style.display = state ? DisplayStyle.Flex : DisplayStyle.None;
    }


    void SubmitNewPosition()
    {
        // Checks if the current instance is a client or host
        if (!NetworkManager.Singleton.IsClient)
        {
            // Loop through each client's ID
            foreach (ulong uid in NetworkManager.Singleton.ConnectedClientsIds)
            {
                // Gets Network object associated with ID and the control script
                var playerObject = NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(uid);
                var player = playerObject.GetComponent<PlayerControls>();
            }
        }
        else if (NetworkManager.Singleton.IsClient)
        {
            // Gets Network object associated with local client and the control script
            var playerObject = NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject();
            var player = playerObject.GetComponent<PlayerControls>();
        }
    }
}

