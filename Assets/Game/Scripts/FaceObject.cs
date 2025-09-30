using System;
using UnityEngine;

public class FaceObject : MonoBehaviour
{
    [SerializeField] private bool x; 
    [SerializeField] private bool y; 
    [SerializeField] private bool z;
    [SerializeField] private Transform obj;
    private Quaternion lookRotation;
    private void LateUpdate()
    {
        lookRotation = Quaternion.LookRotation(obj.position - transform.position);

        Quaternion newRotation = Quaternion.Euler(lookRotation.eulerAngles.x * Convert.ToInt32(x),
                                                  lookRotation.eulerAngles.y * Convert.ToInt32(y),
                                                  lookRotation.eulerAngles.z * Convert.ToInt32(z));

        transform.rotation = lookRotation;
    }
}