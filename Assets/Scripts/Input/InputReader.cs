using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
[RequireComponent (typeof(PlayerInput))]
public class InputReader : MonoBehaviour
{
    public static event Action<Vector2> OnMovePlayer;
    public static event Action<bool> OnJump;
    [Rpc(SendTo.Owner)]
    public void InputPlayerRpc(InputAction.CallbackContext context)
    {
        OnMovePlayer?.Invoke(context.ReadValue<Vector2>());
    }
    [Rpc(SendTo.Owner)]
    public void InputJumpRpc(InputAction.CallbackContext context)
    {
        OnJump?.Invoke(context.performed);
    }
}
