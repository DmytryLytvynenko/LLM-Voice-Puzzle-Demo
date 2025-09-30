using System;
using System.Threading.Tasks;
using UnityEngine;

public class PhysicalButton : MonoBehaviour, IInteractable
{
    public bool Interactable { get; private set; }

    public event Action ButtonPressed;
    [SerializeField] private Animation m_buttonAnimation;
    [SerializeField] private Transform m_playerPlace;
    [SerializeField] private float m_moveDuration = 0.5f;
    private Quaternion m_playerLookRotation;
    private Task m_movePlayerToButton;
    private void Start()
    {
        m_playerLookRotation = Quaternion.LookRotation(m_playerPlace.forward,m_playerPlace.up);
    }
    public void Interact(PlayerComponents playerComponents) 
    {
        if (m_movePlayerToButton == null)
        {
            m_movePlayerToButton = MoveToPlace(playerComponents);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        Interactable = true;
    }
    private void OnTriggerExit(Collider other)
    {
        Interactable = false;
    }
    private async Task MoveToPlace(PlayerComponents playerComponents)
    {
        playerComponents.AnimatorSetTrigger(CharacterAnimationParameters.PressButton);
        Rigidbody player = playerComponents.PlayerRigidbody;
        Interactable = false;
        playerComponents.DisableCameraMovement();
        playerComponents.DisablePlayerMovement();
        Vector3 moveVector = m_playerPlace.position - player.position;
        Vector3 startVector = player.position;
        player.useGravity = false;
        player.drag = 100000;
        float progress = 0;
        float expiredTime = 0;
        Quaternion startRotation = player.rotation;
        while (progress < 1)
        {
            expiredTime += Time.fixedDeltaTime;
            progress = expiredTime / m_moveDuration;
            player.MoveRotation(Quaternion.Lerp(startRotation, m_playerLookRotation, progress));
            player.MovePosition(startVector + moveVector * progress);
            await Task.Yield();
        }
        m_movePlayerToButton = null;
        player.useGravity = true;
        player.drag = 0;
        playerComponents.EnableCameraMovement();
        playerComponents.EnablePlayerMovement();
        Invoke(nameof(StartButtonAnimation), 0.85f);
        Invoke(nameof(SetInteractableAfterDelay), 3f);
        ButtonPressed?.Invoke();
        Debug.Log("Button pressed");
    }
    private void StartButtonAnimation()
    {
        m_buttonAnimation.Play();
    }    
    private void SetInteractableAfterDelay()
    {
        Interactable = true;
    }
}
