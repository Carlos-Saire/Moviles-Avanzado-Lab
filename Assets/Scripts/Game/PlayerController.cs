using UnityEngine;
[RequireComponent (typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("Characteristics")]
    private Rigidbody rb;
    [SerializeField] private float speed;
    private Vector2 inputMove;
    private void Reset()
    {
        rb = GetComponent<Rigidbody>();
    }
    private void OnEnable()
    {
        InputReader.OnMovePlayer += InputMove;
    }
    private void OnDisable()
    {
        InputReader.OnMovePlayer -= InputMove;
    }
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    private void FixedUpdate()
    {
        rb.linearVelocity = new Vector3(inputMove.x* speed, rb.linearVelocity.y, inputMove.y*speed);
    }
    private void InputMove(Vector2 value)
    {
        inputMove = value;
    }
}
