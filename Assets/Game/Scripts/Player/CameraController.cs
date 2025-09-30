using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraControl : MonoBehaviour
{
    [SerializeField] private Transform anchor;
    [SerializeField] private float m_rotationLerpRate = 2;
    [SerializeField] private Transform target;
    [SerializeField] Camera cam;

    private Transform defaultAnchor;

    private void Awake()
    {
        defaultAnchor = anchor;
    }
    private void LateUpdate()
    {
        UpdatePosition();
        UpdateRotation();
    }
    public void Teleport(Vector3 teleportPosition)
    {
        cam.transform.position = teleportPosition;
    }
    public void SetAnchor(Transform newAnchor)
    {
        anchor = newAnchor;
    }
    public void SetDefalutAnchor()
    {
        anchor = defaultAnchor;
    }
    private void UpdatePosition()
    {
        transform.position = anchor.position;
    }
    private void UpdateRotation()
    {
        Quaternion newRotation = Quaternion.LookRotation(target.position - transform.position);
        transform.rotation = Quaternion.Lerp(transform.rotation, newRotation, Time.deltaTime * m_rotationLerpRate);
    }
}
