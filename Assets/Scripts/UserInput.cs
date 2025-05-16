using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;




public class InputManager : MonoBehaviour
{
    public static InputManager instance;

    public Vector2 MoveInput { get; private set; }


    public bool JumpJustPressed { get; private set; }

    public bool JumpBeingHeld { get; private set; }

    public bool JumpReleased { get; private set; }

    public bool FlashLightOnOff { get; private set; }

    public bool CrouchPressed { get; private set; }

    public bool CrouchBeingHeld { get; private set; }

    public bool CrouchReleased { get; private set; }

    public bool Reload { get; private set; }

    public bool Interact { get; private set; }
    
    public bool Sprint { get; private set; }

    private PlayerInput _playerInput;


    private InputAction _moveAction;
    private InputAction _jumpAction;
    private InputAction _flashlightAction;
    private InputAction _crouchAction;
    private InputAction _sprintAction;
    private InputAction _reloadAction;
    private InputAction _interactAction;


    private void SetUpInputActions()
    {
        _moveAction = _playerInput.actions["Move"];
        _jumpAction = _playerInput.actions["Jump"];
        _flashlightAction = _playerInput.actions["FlashLight"];
        _sprintAction = _playerInput.actions["Sprint"];
        _crouchAction = _playerInput.actions["Crouch"];
        _reloadAction = _playerInput.actions["Reload"];
        _interactAction = _playerInput.actions["Interact"];
    }

    private void UpdateInput()
    {
        MoveInput = _moveAction.ReadValue<Vector2>();
        JumpJustPressed = _jumpAction.WasPressedThisFrame();
        JumpBeingHeld = _jumpAction.IsPressed();
        JumpReleased = _jumpAction.WasReleasedThisFrame();
        FlashLightOnOff = _flashlightAction.WasPressedThisFrame();
        CrouchPressed = _crouchAction.WasPressedThisFrame();
        CrouchBeingHeld = _crouchAction.IsPressed();
        CrouchReleased = _crouchAction.WasReleasedThisFrame();
        Reload = _reloadAction.WasPressedThisFrame();
        Interact = _interactAction.WasPressedThisFrame();
        Sprint = _sprintAction.WasPressedThisFrame();
    }


}
