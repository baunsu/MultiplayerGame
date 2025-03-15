using System;
using Unity.Netcode;
using UnityEngine;

public class NetworkTransformTest : NetworkBehaviour
{
    void Update()
    {
        if (IsServer)
        {
            float theta = Time.time;
            transform.position = new Vector2((float) Math.Cos(theta), (float) Math.Sin(theta));
        }
    }
}
