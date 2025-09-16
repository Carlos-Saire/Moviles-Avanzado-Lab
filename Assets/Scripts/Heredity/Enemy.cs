using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Components;
using System;

[RequireComponent(typeof(NetworkObject))]
[RequireComponent(typeof(NetworkTransform))]
public class Enemy : NetworkBehaviour
{
    public event Action<int> OnLife;

    [Header("Characteristics")]
    [SerializeField] private float speed = 3f;
    public int life;

    private NetworkVariable<int> health = new NetworkVariable<int>();

    [Header("Target")]
    private Transform currentTarget;

    [Header("OverlapSphere")]
    [SerializeField] private float radius = 5f;
    [SerializeField] private LayerMask layer;

    [Header("Gizmos")]
    [SerializeField] private Color color = Color.red;

    public bool IsDead => health.Value <= 0;

    private void Start()
    {
        if (IsOwner)
        {
            health.Value = life;
        }
    }
    private void Update()
    {
        if (!IsOwner) return; 
        CheckVisionPlayer();
        if (currentTarget == null) return;

        if (Vector3.Distance(transform.position, currentTarget.position) > 0)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                currentTarget.position,
                speed * Time.deltaTime
            );
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsOwner) return;

        if (other.CompareTag("Player"))
        {
            DestroyEnemyRpc();
        }
        else if (other.CompareTag("Bullet"))
        {
            TakeDamageRpc(1);
            OnLife?.Invoke(health.Value);
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = color;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
    [Rpc(SendTo.Server)]
    private void TakeDamageRpc(int damage)
    {
        health.Value -= damage;

        if (health.Value <= 0)
            DestroyEnemyRpc();
    }
    [Rpc(SendTo.Server)]
    private void DestroyEnemyRpc()
    {
        GetComponent<NetworkObject>().Despawn(true);
    }

    private void CheckVisionPlayer()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, radius, layer);

        if (colliders.Length == 0)
        {
            currentTarget = null;
            return;
        }

        Transform nearestTarget = colliders[0].transform;
        float minDistance = Vector3.Distance(transform.position, nearestTarget.position);

        for (int i = 1; i < colliders.Length; i++)
        {
            float distance = Vector3.Distance(transform.position, colliders[i].transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestTarget = colliders[i].transform;
            }
        }

        currentTarget = nearestTarget;
    }


}
