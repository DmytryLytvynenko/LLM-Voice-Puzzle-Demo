using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using TMPro;

public class InteractWorldUI : MonoBehaviour
{
    [SerializeField] private Transform m_head;
    [SerializeField] private Transform m_rayTarget;
    [SerializeField] private float m_detectDistance = 10;
    [SerializeField] private LayerMask m_interactableMask;

    private EventSystem m_eventSystem;
    private List<RaycastResult> results = new List<RaycastResult>();
    private AI_Talker_Input_Map _Input;
    private void Awake()
    {
        m_eventSystem = EventSystem.current;
    }
    private void OnEnable() // subscribe to inputs
    {
        _Input = new AI_Talker_Input_Map();
        _Input.Player.Interact.Enable();

        _Input.Player.Interact.started += HandleInteract;

    }
    private void OnDisable() // unsubscribe inputs
    {
        _Input.Player.Interact.started -= HandleInteract;
        _Input.Player.Interact.Disable();
    }
    private void HandleInteract(InputAction.CallbackContext ctx)
    {
        Vector3 rayDirection = m_rayTarget.position - m_head.position;
        Debug.DrawRay(m_head.position, rayDirection.normalized * m_detectDistance, Color.red, 5);

        if (Physics.Raycast(m_head.position, rayDirection, out RaycastHit hitInfo, m_detectDistance, m_interactableMask))
        {
            if (hitInfo.transform.TryGetComponent(out TMP_InputField inputField))
            {
                PointerEventData pointerEventData = new PointerEventData(m_eventSystem);
                pointerEventData.position = Input.mousePosition;
                //ExecuteEvents.Execute(inputFieldController.m_inputField.gameObject, new PointerEventData(EventSystem.current), ExecuteEvents.selectHandler);
                inputField.Select();
            }
        }
        results.Clear();
    }
    public void Enable()
    {
        _Input.Player.Interact.Enable();
    }
    public void Disable()
    {
        _Input.Player.Interact.Disable();
    }
}
