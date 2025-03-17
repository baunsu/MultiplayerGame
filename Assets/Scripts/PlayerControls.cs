using Unity.Netcode;
using UnityEngine;

public class PlayerControls : NetworkBehaviour
{
    // Public variables
    public float speed;
    public GameObject bullet;
    public float shootCooldown = 3.0f;
    
    // Private variables
    private float shootable = 0.0f;
    private float offset = 1.2f;
    private Vector2 y_bounds = new Vector2(-7, 7); 
    private Vector2 x_bounds = new Vector2(-15, 15);
    private Vector2 movementInput = Vector2.zero;
    private Vector2 facing = Vector2.right;
    private Rigidbody2D rb;
    private AudioSource deathSound;
    private AudioSource quack;
    private NetworkVariable<bool> Alive = new NetworkVariable<bool>(true);

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        // Reads audio sources that are attached to player
        var audio = GetComponents<AudioSource>();
        deathSound = audio[0];
        quack = audio[1];
    }

    public void Start()
    {   
        Alive.OnValueChanged += OnAliveChange;
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            quack.Play();

            // Randomizes the player's spawn position when joining the server
            float random_x = Random.Range(x_bounds.x, x_bounds.y);
            float random_y = Random.Range(y_bounds.x, y_bounds.y);
            transform.position = new Vector2(random_x, random_y);
        }
    }

    public override void OnDestroy()
    {
        Alive.OnValueChanged -= OnAliveChange;
    }

    void Update()
    {
        // Skips if no the owner or not alive
        if (!IsOwner || !Alive.Value) return;

        getDirection();

        // Controls player shooting and the cooldown
        if (Input.GetKeyDown(KeyCode.Space) && Time.time >= shootable)
        {
            SubmitShootRequestServerRpc(rb.position, facing);
            shootable = Time.time + shootCooldown;
        }
    }

    void FixedUpdate()
    {
        // Only allows the owner to update
        if (!IsOwner || !Alive.Value) return;


        // If movement is inputted, tells server to move player    
        if (movementInput != Vector2.zero) 
        {
            SubmitPositionRequestServerRpc(movementInput);
            movementInput = Vector2.zero;    
        }
    }

    public void getDirection() 
    {
        // Reads inputs and sets direction
        if (Input.GetKey(KeyCode.W))
            movementInput.y = 1;
        else if (Input.GetKey(KeyCode.S))
            movementInput.y = -1;
        if (Input.GetKey(KeyCode.D))
            movementInput.x = 1;
        else if (Input.GetKey(KeyCode.A))
            movementInput.x = -1;

        // Prevents the bullet from staying still
        if (movementInput != Vector2.zero)
            facing = movementInput;

        // Prevents diagonal speed up
        movementInput = movementInput.normalized;
    }

    public void Die()
    {
        // Death is handled server side
        if (!IsServer) return;

        Debug.Log($"Player {OwnerClientId} died!");
        deathSound.Play();
        Alive.Value = false;

        GetComponent<SpriteRenderer>().enabled = (false);
        rb.simulated = false;
    }

    // Disables the sprite when killed
    private void OnAliveChange(bool prev, bool curr)
    {
        gameObject.GetComponent<SpriteRenderer>().enabled = curr;
    }

    // Server RPCS 

    [Rpc(SendTo.Server)]
    void SubmitPositionRequestServerRpc(Vector2 direction)
    {
        // If no direction, then does not send server new position
        if (direction == Vector2.zero) {
            return;
        }

        // Sets the new direction server side
        Vector2 newPosition = rb.position + direction * speed * Time.fixedDeltaTime;
        rb.MovePosition(newPosition);
    }


    // Spawns the bullet on the network before syncing it with the client
    [Rpc(SendTo.Server)]
    public void SubmitShootRequestServerRpc(Vector2 position, Vector2 direction)
    {
        var bulletObj = Instantiate(bullet, position + (direction * offset), Quaternion.identity);
        bulletObj.GetComponent<NetworkObject>().Spawn();
        bulletObj.GetComponent<Bullet>().Initialize(direction, OwnerClientId);
    }

    // Handles death on the server
    [Rpc(SendTo.Server)]
    public void SubmitDeathServerRpc()
    {
        Die();
    }
}