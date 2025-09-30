using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable 
{
    public bool Interactable { get; }
    public void Interact(PlayerComponents playerComponents);
}
