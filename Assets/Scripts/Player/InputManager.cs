using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public Action<InputActionPhase, Vector2> OnMoveAction;
    public Action<InputActionPhase, Vector2> OnLookAction;
    public Action<InputActionPhase> OnJumpAction;
    public Action<InputActionPhase> OnFireAction;
    public Action OnPauseAction;
    
    public void OnMove(InputAction.CallbackContext context)
    {
        OnMoveAction?.Invoke(context.phase, context.ReadValue<Vector2>());
    }
    
    public void OnLook(InputAction.CallbackContext context)
    {
        OnLookAction?.Invoke(context.phase, context.ReadValue<Vector2>());
    }
    
    public void OnJump(InputAction.CallbackContext context)
    {
        OnJumpAction?.Invoke(context.phase);
    }
    
    public void OnFire(InputAction.CallbackContext context)
    {
        OnFireAction?.Invoke(context.phase);
    }
    
    public void OnPause(InputAction.CallbackContext context)
    {
        OnPauseAction?.Invoke();
    }
}