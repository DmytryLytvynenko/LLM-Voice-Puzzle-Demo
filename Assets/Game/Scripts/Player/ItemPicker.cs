using Unity.VisualScripting;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.InputSystem;

public class ItemPicker : MonoBehaviour
{
    [SerializeField] private Transform m_followPoint;
    [SerializeField] private Transform m_head;
    [SerializeField] private Transform m_rayTarget;
    [SerializeField] private float m_detectDistance = 10;
    [SerializeField] private float m_moveObjectSpeed = 1;
    [SerializeField] private Vector2 m_objectCloseFarLimit;
    [SerializeField] private LayerMask m_pickableMask;

    private IPickable m_currentItem;
    private bool m_EPressed;
    private bool m_QPressed;
    private bool m_MoveObjectPressed;
    private float m_MoveObjectDelta;

    private AI_Talker_Input_Map _Input;
    private void Start()
    {
        m_objectCloseFarLimit.x = m_followPoint.transform.localPosition.z;
        m_objectCloseFarLimit.y = m_detectDistance;
    }
    private void OnEnable() // subscribe to inputs
    {
        _Input = new AI_Talker_Input_Map();
        _Input.Player.Interact.Enable();
        _Input.Player.MMBClick.Enable();
        _Input.Player.Scroll.Enable();
        _Input.Player.RotateObjectE.Enable();
        _Input.Player.RotateObjectQ.Enable();
        _Input.Player.MoveObject.Enable();

        _Input.Player.MoveObject.performed += HandleMoveObjectInput;
        _Input.Player.MoveObject.canceled += HandleMoveObjectInputCanceled;

        _Input.Player.Interact.started += HandleInteract;

        _Input.Player.MMBClick.performed += HandleMMBClick;

        _Input.Player.Scroll.started += HandlePerformedScroll;
        _Input.Player.Scroll.canceled += HandleCanceledScroll;

        _Input.Player.RotateObjectE.performed += HandlePerformedRotateObjectE;
        _Input.Player.RotateObjectE.canceled += HandleCanceledRotateObjectE;

        _Input.Player.RotateObjectQ.performed += HandlePerformedRotateObjectQ;
        _Input.Player.RotateObjectQ.canceled += HandleCanceledRotateObjectQ;
    }
    private void OnDisable() // unsubscribe inputs
    {
        _Input.Player.MoveObject.performed -= HandleMoveObjectInput;
        _Input.Player.MoveObject.canceled -= HandleMoveObjectInputCanceled;

        _Input.Player.Interact.started -= HandleInteract;

        _Input.Player.MMBClick.performed -= HandleMMBClick;

        _Input.Player.Scroll.started -= HandlePerformedScroll;
        _Input.Player.Scroll.canceled -= HandleCanceledScroll;

        _Input.Player.RotateObjectE.performed -= HandlePerformedRotateObjectE;
        _Input.Player.RotateObjectE.canceled -= HandleCanceledRotateObjectE;

        _Input.Player.RotateObjectQ.performed -= HandlePerformedRotateObjectQ;
        _Input.Player.RotateObjectQ.canceled -= HandleCanceledRotateObjectQ;

        _Input.Player.MoveObject.Disable();
        _Input.Player.Interact.Disable();
        _Input.Player.MMBClick.Disable();
        _Input.Player.Scroll.Disable();
        _Input.Player.RotateObjectE.Disable();
        _Input.Player.RotateObjectQ.Disable();
    }
    private void FixedUpdate()
    {
        if (m_currentItem == null)
        {
            return;
        }
        if (m_EPressed)
        {
            m_currentItem.RotateHorizontal(-1);
        }
        if (m_QPressed)
        {
            m_currentItem.RotateHorizontal(1);
        }
        if (m_MoveObjectPressed)
        {
            MoveObjectCloseFar();
        }
    }

    private void HandleMoveObjectInput(InputAction.CallbackContext ctx)
    {
        if (m_currentItem != null)
        {
            if (m_currentItem.RotatingToCamera)
            {
                print("already rotating");
                return;
            }
            m_MoveObjectDelta = ctx.ReadValue<float>();
            print(m_MoveObjectDelta);
            m_MoveObjectPressed = true;
        }
    }
    private void MoveObjectCloseFar()
    {
        float zValue = Mathf.Clamp(m_followPoint.localPosition.z + m_MoveObjectDelta * m_moveObjectSpeed,
                                   m_objectCloseFarLimit.x,
                                   m_objectCloseFarLimit.y);
        Vector3 nextPos = new Vector3(m_followPoint.localPosition.x,
                                      m_followPoint.localPosition.y,
                                      zValue);

        m_followPoint.localPosition = nextPos;
    }
    private void HandleMoveObjectInputCanceled(InputAction.CallbackContext ctx)
    {
        m_MoveObjectPressed = false;
    }
    private void HandleInteract(InputAction.CallbackContext ctx)
    {
        if (m_currentItem != null)
        {
            m_currentItem.Drop();
            m_currentItem = null;
            return;
        }
        Vector3 rayDirection = m_rayTarget.position - m_head.position;
        Debug.DrawRay(m_head.position, rayDirection.normalized * m_detectDistance, Color.red, 5);

        if (Physics.Raycast(m_head.position, rayDirection, out RaycastHit hitInfo, m_detectDistance, m_pickableMask))
        {
            if (hitInfo.transform.TryGetComponent(out IPickable pickable))
            {
                m_currentItem = pickable;
                m_currentItem.Pick(m_followPoint);
            }
        }
    }  
    private void HandleMMBClick(InputAction.CallbackContext ctx)
    {
        if (m_currentItem != null)
        {
            if (m_currentItem.RotatingToCamera)
            {
                print("already rotating");
                return;
            }
            m_currentItem.RotateToCamera();
        }
    }    
    private void HandlePerformedScroll(InputAction.CallbackContext ctx)
    {
        if (m_currentItem != null)
        {
            if (m_currentItem.RotatingToCamera)
            {
                print("already rotating");
                return;
            }
            Vector2 delta = ctx.ReadValue<Vector2>();
            delta.Normalize();
            m_currentItem.RotateVertical(delta.y);
        }
    }   
    private void HandleCanceledScroll(InputAction.CallbackContext ctx)
    {
        if (m_currentItem != null)
        {
            if (m_currentItem.RotatingToCamera)
            {
                print("already rotating");
                return;
            }
        }
    }
    private void HandlePerformedRotateObjectE(InputAction.CallbackContext ctx)
    {
        if (m_currentItem != null)
        {
            if (m_currentItem.RotatingToCamera)
            {
                print("already rotating");
                return;
            }
            m_EPressed = true;
        }
    }    
    private void HandlePerformedRotateObjectQ(InputAction.CallbackContext ctx)
    {
        if (m_currentItem != null)
        {
            if (m_currentItem.RotatingToCamera)
            {
                print("already rotating");
                return;
            }
            m_QPressed = true;
        }
    }    
    private void HandleCanceledRotateObjectE(InputAction.CallbackContext ctx)
    {
        m_EPressed = false;
        if (m_currentItem != null)
        {
            if (m_currentItem.RotatingToCamera)
            {
                print("already rotating");
                return;
            }
        }
    }    
    private void HandleCanceledRotateObjectQ(InputAction.CallbackContext ctx)
    {
        m_QPressed = false;
        if (m_currentItem != null)
        {
            if (m_currentItem.RotatingToCamera)
            {
                print("already rotating");
                return;
            }
        }
    }

    public void Enable()
    {
        _Input.Player.Interact.Enable();
        _Input.Player.MMBClick.Enable();
        _Input.Player.Scroll.Enable();
        _Input.Player.RotateObjectE.Enable();
        _Input.Player.RotateObjectQ.Enable();
        _Input.Player.MoveObject.Enable();
    }
    public void Disable()
    {
        _Input.Player.MoveObject.Disable();
        _Input.Player.Interact.Disable();
        _Input.Player.MMBClick.Disable();
        _Input.Player.Scroll.Disable();
        _Input.Player.RotateObjectE.Disable();
        _Input.Player.RotateObjectQ.Disable();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(m_followPoint.position, 0.1f);
        Gizmos.color = Color.blue;
        Vector3 lookDirection = m_rayTarget.position - m_head.position;
        Gizmos.DrawSphere(m_head.position + lookDirection.normalized * m_detectDistance, 0.1f);
    }
}
