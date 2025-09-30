using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerComponents : MonoBehaviour
{
    [field: SerializeField] public PlayerMovement PlayerMovement { get; private set; }
    [field: SerializeField] public Interact PlayerInteract { get; private set; }
    [field: SerializeField] public InteractWorldUI PlayerWorldUIInteract { get; private set; }
    [field: SerializeField] public ItemPicker PlayerItemPicker { get; private set; }
    [field: SerializeField] public Rigidbody PlayerRigidbody { get; private set; }
    [field: SerializeField] public Animator PlayerAnimator { get; private set; }
    private void Start()
    {
        PlayerMovement = GetComponent<PlayerMovement>();
        PlayerInteract = GetComponent<Interact>();
        PlayerWorldUIInteract = GetComponent<InteractWorldUI>();
        PlayerRigidbody = GetComponent<Rigidbody>();
        PlayerItemPicker = GetComponent<ItemPicker>();
        PlayerAnimator = GetComponent<Animator>();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            PlayerInteract.Enable();
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            PlayerInteract.Disable();
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            PlayerWorldUIInteract.Enable();
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            PlayerWorldUIInteract.Disable();
        }
    }
    public void DisablePlayerMovement()
    {
        PlayerMovement.DisableMovement();
    }    
    public void EnablePlayerMovement()
    {
        PlayerMovement.EnableMovement();
    }        
    public void DisableCameraMovement()
    {
        PlayerMovement.DisableCameraMovement();
    }    
    public void EnableCameraMovement()
    {
        PlayerMovement.EnableCameraMovement();
    }    
    public void DisableInteract()
    {
        PlayerInteract.Disable();
    }    
    public void EnableInteract()
    {
        PlayerInteract.Enable();
    }    
    public void DisableInteractWorldUI()
    {
        PlayerWorldUIInteract.Disable();
    }
    public void EnableInteractWorldUI()
    {
        PlayerWorldUIInteract.Enable();
    }
    public void DisableItemPicker()
    {
        PlayerItemPicker.Disable();
    }
    public void EnableItemPicker()
    {
        PlayerItemPicker.Enable();
    }
    public void SetPlayerCameraPosition(Vector3 position)
    {
        PlayerMovement.SetHeadPosition(position);
    }    
    public void SetPlayerCameraDefaultPosition()
    {
        PlayerMovement.SetHeadDefaultPosition();
    }


    //Animator
    public void AnimatorSetInt(CharacterAnimationParameters parameter , int value)
    {
        PlayerAnimator.SetInteger(parameter.ToString(), value);
    }
    public void AnimatorSetTrigger(CharacterAnimationParameters trigger)
    {
        PlayerAnimator.SetTrigger(trigger.ToString());
    }
}
