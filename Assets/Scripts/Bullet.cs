using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class Bullet : NetworkBehaviour
{
    // Public variables
    public float bulletSpeed = 5;

    // Private variables
    private Rigidbody2D rb;
    private Vector2 velocity;
    private ulong ownerId;
    private AudioSource whoosh;
    private AudioSource hit_wall;
    private NetworkVariable<Vector2> Position = new NetworkVariable<Vector2>();
    
    
    public void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        // Reads audio sources that are attached to bullet
        var audio = GetComponents<AudioSource>();
        whoosh = audio[0];
        hit_wall = audio[1];
    }

    public void Initialize(Vector2 direction, ulong ownerId)
    {
        velocity = direction.normalized * bulletSpeed;

        // Stores the player that shot the bullet's ID
        this.ownerId = ownerId;
    }

    public void OnStateChanged(Vector2 previous, Vector2 current)
    {
        if (Position.Value != previous)
        {
            transform.position = Position.Value;
        }
    }

    public void FixedUpdate()
    {
        if (IsServer) // Bullet only moves on the server
        {
            rb.MovePosition(rb.position + velocity * Time.fixedDeltaTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Collisions should be handled server side
        if (!IsServer) return;
        
        if (collision.CompareTag("Player"))
        {
            // Gets player's controls to check the ID of the player that was hit
            // We don't want to shoot ourselves on accident.
            if (collision.TryGetComponent<PlayerControls>(out var player))
            {
                ulong hitPlayerId = collision.GetComponent<NetworkObject>().OwnerClientId;

                if (hitPlayerId == ownerId) return;

                // Debug.Log("Hit player!");

                // Tells server player was shot
                player.SubmitDeathServerRpc();
                DespawnBullet();
            }
        }

        if (collision.CompareTag("Wall"))
        {
            Debug.Log("Hit wall!");
            StartCoroutine(soundTimeOut());
        }
    }

    // Coroutine to play the sound then despawn the bullet
    IEnumerator soundTimeOut()
    {
        GetComponent<SpriteRenderer>().enabled = (false);
        hit_wall.Play();
        yield return new WaitForSeconds (0.5f);
        DespawnBullet();
    }

    // Despawns the bullet
    private void DespawnBullet()
    {
        GetComponent<NetworkObject>().Despawn();
    }
}
