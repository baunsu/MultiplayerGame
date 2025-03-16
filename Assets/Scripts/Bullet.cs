using System;
using Unity.Netcode;
using UnityEngine;

public class Bullet : NetworkBehaviour
{
    public float bulletSpeed = 5;
    private Rigidbody2D rb;
    private Vector2 velocity;
    private ulong ownerId;
    private NetworkVariable<Vector2> Position = new NetworkVariable<Vector2>();
    

    public void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Initialize(Vector2 direction, ulong ownerId)
    {
        velocity = direction.normalized * bulletSpeed;
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
        if (IsServer)
        {
            rb.MovePosition(rb.position + velocity * Time.fixedDeltaTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!IsServer) return;
        
        if (collision.CompareTag("Player"))
        {
            if (collision.TryGetComponent<PlayerControls>(out var player))
            {
                ulong hitPlayerId = collision.GetComponent<NetworkObject>().OwnerClientId;

                if (hitPlayerId == ownerId) return;

                Debug.Log("Hit player!");
                player.SubmitDeathServerRpc();
                DespawnBullet();
            }
        }

        if (collision.CompareTag("Wall"))
        {
            Debug.Log("Hit wall!");
            DespawnBullet();
        }
    }

    private void DespawnBullet()
    {
        GetComponent<NetworkObject>().Despawn();
    }
}
