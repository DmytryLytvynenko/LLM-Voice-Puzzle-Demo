using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float m_moveSpeed = 1;
    [SerializeField] private float m_rotationSpeed = 1;
    [SerializeField] private float m_jumpVelocity = 1;
    [SerializeField] private Rigidbody m_rigidbody;
    [SerializeField] private Transform m_head;
    [SerializeField] private int m_vertRotationLimit;
    [SerializeField] private Animator m_animator;
    private float m_vertRotInput;
    private bool m_isGrounded;

    private AI_Talker_Input_Map _Input;
    private Vector2 m_moveInput;
    private Vector3 m_defaultHeadPosition;
    private Transform m_cam;
    private CapsuleCollider m_collider;

    private void Start()
    {
        m_defaultHeadPosition = new Vector3((float)Math.Round(m_head.position.x, 3),
                                            (float)Math.Round(m_head.position.y, 3),
                                            (float)Math.Round(m_head.position.z, 3));
        m_vertRotInput = 0;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        m_cam = Camera.main.transform;
        m_collider = GetComponent<CapsuleCollider>();
    }
    private void OnEnable() // subscribe to inputs
    {
        _Input = new AI_Talker_Input_Map();

        _Input.Player.Look.Enable();
        _Input.Player.Move.Enable();
        _Input.Player.Jump.Enable();

        _Input.Player.Jump.performed += HandleJumpInput;

        _Input.Player.Look.performed += HandleLookInput;
        _Input.Player.Look.canceled += HandleLookInput;

        _Input.Player.CameraOnlyLook.performed += HandleCameraLookInput;

        _Input.Player.Move.performed += HandleMoveInputPerformed;
        _Input.Player.Move.canceled += HandleMoveInputCanceled;
    }
    private void OnDisable() // unsubscribe inputs
    {
        _Input.Player.Jump.performed -= HandleJumpInput;

        _Input.Player.Look.performed -= HandleLookInput;
        _Input.Player.Look.canceled -= HandleLookInput;

        _Input.Player.CameraOnlyLook.performed -= HandleCameraLookInput;

        _Input.Player.Move.performed -= HandleMoveInputPerformed;
        _Input.Player.Move.canceled -= HandleMoveInputCanceled;

        _Input.Player.Jump.Disable();
        _Input.Player.Look.Disable();
        _Input.Player.CameraOnlyLook.Disable();
        _Input.Player.Move.Disable();
    }
    private void FixedUpdate()
    {
        Move();
        ChangeFrictionOnStop();
    }

    private void Move()
    {
        //camera dir
        Vector3 camFoward = m_cam.forward;
        Vector3 camRight = m_cam.right;

        camFoward.y = 0;
        camRight.y = 0;

        //creating relate cam direction
        Vector3 forwardRelative = m_moveInput.y * camFoward;
        Vector3 rightRelative = m_moveInput.x * camRight;

        Vector3 MoveVector = (forwardRelative + rightRelative) * m_moveSpeed;
        MoveVector.y = m_rigidbody.velocity.y;

        m_rigidbody.velocity = MoveVector;
    }
    private void ChangeFrictionOnStop()
    {
        if (m_moveInput == Vector2.zero)
        {
            m_collider.material.staticFriction = 1;
        }
        else
        {
            m_collider.material.staticFriction = 0;
        }
    }
    private void HandleMoveInputPerformed(InputAction.CallbackContext ctx)
    {
        m_moveInput = ctx.ReadValue<Vector2>();
        if (m_moveInput.y > 0)
        {
            m_animator.SetInteger(CharacterAnimationParameters.State.ToString(), (int)CharacterAnimationStates.RunningForward);
        }
        if (m_moveInput.x > 0)
        {
            m_animator.SetInteger(CharacterAnimationParameters.State.ToString(), (int)CharacterAnimationStates.StrafeR);
        }
        if (m_moveInput.x < 0)
        {
            m_animator.SetInteger(CharacterAnimationParameters.State.ToString(), (int)CharacterAnimationStates.StrafeL);
        }
        if (m_moveInput.y < 0)
        {
            m_animator.SetInteger(CharacterAnimationParameters.State.ToString(), (int)CharacterAnimationStates.RunningBackwards);
        }
    }
    private void HandleMoveInputCanceled(InputAction.CallbackContext ctx)
    {
        if (m_moveInput.y > 0)
        {
            m_animator.SetInteger("State", (int)CharacterAnimationStates.RunningForward);
        }
        if (m_moveInput.x > 0)
        {
            m_animator.SetInteger("State", (int)CharacterAnimationStates.StrafeR);
        }
        if (m_moveInput.x < 0)
        {
            m_animator.SetInteger("State", (int)CharacterAnimationStates.StrafeL);
        }
        if (m_moveInput.y < 0)
        {
            m_animator.SetInteger("State", (int)CharacterAnimationStates.RunningBackwards);
        }
        m_animator.SetInteger("State", (int)CharacterAnimationStates.Idle);
        m_moveInput = ctx.ReadValue<Vector2>();
    }
    private void HandleJumpInput(InputAction.CallbackContext ctx)
    {
        PerformJump();
    }

    private void PerformJump()
    {
        float jumpInput = m_jumpVelocity;
        m_rigidbody.velocity = new Vector3(m_rigidbody.velocity.x, jumpInput, m_rigidbody.velocity.z);
    }

    private void HandleLookInput(InputAction.CallbackContext ctx)
    {
        Vector2 input = ctx.ReadValue<Vector2>();
        m_rigidbody.angularVelocity = new Vector3(0,input.x * m_rotationSpeed, 0);
        float baseRotAngle = AngleClamp.ClampAngle(m_head.localEulerAngles.x - input.y * m_rotationSpeed,
                                                     -m_vertRotationLimit,
                                                     m_vertRotationLimit);
        m_head.localRotation = Quaternion.Euler(baseRotAngle, 0, 0);
        m_vertRotInput += input.y * 0.001f;
        m_vertRotInput = Mathf.Clamp(m_vertRotInput, -0.5f, 0.5f);
        m_animator.SetFloat(CharacterAnimationParameters.HeadAngle.ToString(), 0.5f + m_vertRotInput);
    }       
    private void HandleCameraLookInput(InputAction.CallbackContext ctx)
    {
        Vector2 input = ctx.ReadValue<Vector2>();
        float baseYRotAngle = m_head.localEulerAngles.y + input.x * m_rotationSpeed;
        float baseXRotAngle = AngleClamp.ClampAngle(m_head.localEulerAngles.x - input.y * m_rotationSpeed,
                                                     -m_vertRotationLimit,
                                                     m_vertRotationLimit);
        m_head.localRotation = Quaternion.Euler(baseXRotAngle, baseYRotAngle, 0);
    }    
    public void EnableMovement()
    {
        _Input.Player.Move.Enable();
        _Input.Player.Jump.Enable();
    }
    public void DisableMovement()
    {
        _Input.Player.Move.Disable();
        _Input.Player.Jump.Disable();
    }    
    public void EnableCameraMovement()
    {
        _Input.Player.Look.Enable();
    }    
    public void DisableCameraMovement()
    {
        _Input.Player.Look.Disable();
    }
    public void SetHeadPosition(Vector3 position)
    {
        m_defaultHeadPosition = m_head.position;
        _Input.Player.Look.Disable();
        _Input.Player.CameraOnlyLook.Enable();
        m_head.position = position;
    }
    public void SetHeadDefaultPosition()
    {
        _Input.Player.Look.Enable();
        _Input.Player.CameraOnlyLook.Disable();
        m_head.position = m_defaultHeadPosition;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (!m_isGrounded)
        {
            m_isGrounded = true;
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.contactCount == 0)
        {
            m_isGrounded = false;
        }
    }
    private void OnCollisionStay(Collision collision)
    {
            m_isGrounded = true;
    }
}
