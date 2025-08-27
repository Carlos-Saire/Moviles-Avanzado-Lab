using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Components;
[RequireComponent(typeof(NetworkObject))]
public class PlayerController : NetworkBehaviour
{
    [Header("Characteristics")]
    [SerializeField] private float speed;

    [Header("Input")]
    private float horizontal;
    private float vertical;
    private void Update()
    {
        horizontal = Input.GetAxis("Horizontal") * speed * Time.deltaTime;
        vertical = Input.GetAxis("Vertical") * speed * Time.deltaTime;
        transform.position += new Vector3(horizontal, 0, vertical);
    }
}
