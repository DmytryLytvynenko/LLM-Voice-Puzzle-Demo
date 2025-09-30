using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ReferanceContainerInitializer : MonoBehaviour
{
    private void Awake()
    {
        ReferanceContainer.PlayerMovement = FindFirstObjectByType<PlayerMovement>().GetComponent<PlayerMovement>();
    }
}
