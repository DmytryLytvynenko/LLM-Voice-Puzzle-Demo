using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class Interact : MonoBehaviour
{
    [SerializeField] private Transform m_head;
    [SerializeField] private Transform m_rayTarget;
    [SerializeField] private float m_detectDistance;
    [SerializeField] private Animator m_animator;
    [SerializeField] private Rigidbody m_rigidbody;
    [SerializeField] private LayerMask m_interactableMask;
    [SerializeField] private PlayerComponents m_playerComponents;

    private AI_Talker_Input_Map _Input;
    private void Awake()
    {
        m_rigidbody = GetComponent<Rigidbody>();
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
            if (hitInfo.transform.TryGetComponent(out IInteractable interactable))
            {
                if (interactable.Interactable)
                {
                    interactable.Interact(m_playerComponents);
                    //m_animator.SetTrigger(CharacterAnimationParameters.PressButton.ToString());
                }
            }
        }
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
