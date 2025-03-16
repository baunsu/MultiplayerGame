using Unity.Netcode;
using UnityEngine;

public class PlayerControls : NetworkBehaviour
{
    // Creates a network vector2 that stores the position of the plaeyr
    public NetworkVariable<Vector2> Position = new NetworkVariable<Vector2>();
    public float speed;
    private Vector2 movementInput = Vector2.zero;

    public override void OnNetworkSpawn()
    {
        Position.OnValueChanged += OnStateChanged;
    }

    public override void OnNetworkDespawn()
    {
        Position.OnValueChanged -= OnStateChanged;
    }

    public void Update()
    {
        // Only allows the owner to update
        if (!IsOwner) return;

        getDirection();
        
        if (movementInput != Vector2.zero) 
            SubmitPositionRequestServerRpc();
    }

    public void getDirection() 
    {
        if (Input.GetKey(KeyCode.W))
            movementInput.y = 1;
        else if (Input.GetKey(KeyCode.S))
            movementInput.y = -1;
        if (Input.GetKey(KeyCode.D))
            movementInput.x = 1.0f;
        else if (Input.GetKey(KeyCode.A))
            movementInput.x = -1;

        movementInput = movementInput.normalized;
    }

    public void OnStateChanged(Vector2 previous, Vector2 current)
    {
        transform.position = current;
    }

    public void Move()
    {
        SubmitPositionRequestServerRpc();
    }

    [Rpc(SendTo.Server)]
    void SubmitPositionRequestServerRpc(RpcParams rpcParams = default)
    {
        Vector2 newPosition = Position.Value + movementInput * speed * Time.deltaTime;
        movementInput = Vector2.zero;
        Position.Value = newPosition;
    }
}