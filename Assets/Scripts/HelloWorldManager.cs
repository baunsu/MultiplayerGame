using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;


public class HelloWorldManager : MonoBehaviour
{
    VisualElement rootVisualElement;
    Button hostButton;
    Button clientButton;
    Button serverButton;
    Label statusLabel;

    void OnEnable()
    {
        var uiDocument = GetComponent<UIDocument>();
        rootVisualElement = uiDocument.rootVisualElement;

        hostButton = CreateButton("HostButton", "Host");
        clientButton = CreateButton("ClientButton", "Client");
        serverButton = CreateButton("ServerButton", "Server");
        statusLabel = CreateLabel("StatusLabel", "Not Connected");

        rootVisualElement.Clear();
        rootVisualElement.Add(hostButton);
        rootVisualElement.Add(clientButton);
        rootVisualElement.Add(serverButton);
        rootVisualElement.Add(statusLabel);

        hostButton.clicked += OnHostButtonClicked;
        clientButton.clicked += OnClientButtonClicked;
        serverButton.clicked += OnServerButtonClicked;
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

    void OnDisable()
    {
        hostButton.clicked -= OnHostButtonClicked;
        clientButton.clicked -= OnClientButtonClicked;
        serverButton.clicked -= OnServerButtonClicked;
    }

    // Makes single instance of the host, client, or server
    void OnHostButtonClicked() => NetworkManager.Singleton.StartHost();
    void OnClientButtonClicked() => NetworkManager.Singleton.StartClient();
    void OnServerButtonClicked() => NetworkManager.Singleton.StartServer();

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

    private Label CreateLabel(string name, string content)
    {
        var label = new Label();
        label.name = name;
        label.text = content;
        label.style.color = Color.black;
        label.style.fontSize = 18;
        return label;
    }

    void UpdateUI()
    {
        if (NetworkManager.Singleton == null)
        {
            SetStartButtons(false);
            SetStatusText("NetworkManager not found");
            return;
        }

        if (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer)
        {
            SetStartButtons(true);
            SetStatusText("Not Connected");
        }
        else
        {
            SetStartButtons(false);
            UpdateStatusLabels();
        }
    }

    void SetStartButtons(bool state)
    {
        hostButton.style.display = state ? DisplayStyle.Flex : DisplayStyle.None;
        clientButton.style.display = state ? DisplayStyle.Flex : DisplayStyle.None;
        serverButton.style.display = state ? DisplayStyle.Flex : DisplayStyle.None;
    }

    void SetStatusText(string text) => statusLabel.text = text;

    void UpdateStatusLabels()
    {
        var mode = NetworkManager.Singleton.IsHost ? "Host" : NetworkManager.Singleton.IsServer ? "Server" : "Client";
        string transport = "Transport: " + NetworkManager.Singleton.NetworkConfig.NetworkTransport.GetType().Name;
        string modeText = "Mode: " + mode;
        SetStatusText($"{transport}\n{modeText}");
    }

    void SubmitNewPosition()
    {
        if (NetworkManager.Singleton.IsServer && !NetworkManager.Singleton.IsClient)
        {
            foreach (ulong uid in NetworkManager.Singleton.ConnectedClientsIds)
            {
                var playerObject = NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(uid);
                var player = playerObject.GetComponent<PlayerControls>();
            }
        }
        else if (NetworkManager.Singleton.IsClient)
        {
            var playerObject = NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject();
            var player = playerObject.GetComponent<PlayerControls>();
        }
    }
}

