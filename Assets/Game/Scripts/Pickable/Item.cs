using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Outline))]
public class Item : MonoBehaviour, IPickable
{
    [SerializeField] private Color m_highlightColor = Color.blue;
    [SerializeField] private float m_followSpeed = 1f;
    [SerializeField] private float m_rotateToCameraDuration = 1f;
    [SerializeField] private float m_rotateVerticalSpeed;
    [SerializeField] private float m_rotateHorizontalSpeed;
    [SerializeField] private float m_rotateHorizontalLerpRate = 2f;

    [field:SerializeField] public ItemData ItemData { get; private set; }
    public bool RotatingToCamera { get; private set; } = false;
    public bool Interactable { get; set; } = true;

    public bool Picked { get; set; } = false;
    public Rigidbody Rigidbody;
    public ConstantForce ConstantForce;
    private Outline m_outline;
    private Transform m_target;
    private Transform m_camera;
    private CancellationTokenSource m_CTS;
    private Quaternion targetRotation;
    private void Start()
    {
        m_outline = GetComponent<Outline>();
        Rigidbody = GetComponent<Rigidbody>();
        ConstantForce = GetComponent<ConstantForce>();
        m_camera = Camera.main.transform;
        m_outline.enabled = false;
    }
    private void FixedUpdate()
    {
        if (Picked)
        {
            FollowTarget();
        }
        if (targetRotation.x != 0 || targetRotation.y != 0)
        {
            Rigidbody.rotation = Quaternion.Lerp(Rigidbody.rotation, targetRotation, Time.deltaTime * m_rotateHorizontalLerpRate);
        }
    }
    private void FollowTarget()
    {
        Vector3 moveVector = m_target.position - transform.position;
        Rigidbody.velocity = moveVector * m_followSpeed;
    }
    public void Highlight()
    {
        m_outline.OutlineColor = m_highlightColor;
        m_outline.enabled = true;
    }
    public void ClearHighlight()
    {
        m_outline.enabled = false;
    }
    public void Pick(Transform followTarget)
    {
        if (!Interactable)
        {
            return;
        }
        this.enabled = true;
        m_target = followTarget;
        Picked = true;
        Highlight();
    }
    public void Drop()
    {
        this.enabled = false;
        targetRotation = transform.localRotation;
        Picked = false;
        ClearHighlight();
        if (m_CTS != null)
        {
            m_CTS.Cancel();
        }
    }
    public async void RotateToCamera()
    {
        targetRotation = Quaternion.identity;
        m_CTS = new CancellationTokenSource();
        RotatingToCamera = true;
        Rigidbody.angularVelocity = Vector3.zero;
        Quaternion lookRotation = Quaternion.LookRotation(m_camera.position - transform.position);
        await RotateToCameraTask(lookRotation, m_CTS.Token);
    }
    public async Task RotateToCameraTask(Quaternion lookRotation, CancellationToken ct)
    {
        NullifyTargetRotation();
        float progress = 0;
        float expiredTime = 0;
        Quaternion startRotation = Rigidbody.rotation;
        while (progress < 1)
        {
            expiredTime += Time.fixedDeltaTime;
            progress = expiredTime / m_rotateToCameraDuration;
            Rigidbody.rotation = Quaternion.Lerp(startRotation, lookRotation, progress);
            if (ct.IsCancellationRequested)
            {
                RotatingToCamera = false;
                print("RotatingToCameraCanceled");
                return;
            }
            await Task.Yield();
        }
        RotatingToCamera = false;
    }
    public void RotateVertical(float delta)
    {
        NullifyTargetRotation();
        Rigidbody.AddTorque(transform.right * delta * m_rotateVerticalSpeed, ForceMode.VelocityChange);
        Rigidbody.angularVelocity *= 1 - 0.05f;
        /*        m_rigidbody.angularVelocity = Vector3.zero;
                targetRotation = Quaternion.Euler(transform.localEulerAngles.x - delta * m_rotateVerticalSpeed, 0, 0);
                print(targetRotation.eulerAngles);*/
    }
    public void RotateHorizontal(float delta)
    {
        Rigidbody.angularVelocity = Vector3.zero;
        targetRotation = Quaternion.Euler(0, transform.localEulerAngles.y + delta * m_rotateHorizontalSpeed, 0);
    }    
    private void NullifyTargetRotation()
    {
        targetRotation = Quaternion.identity;
    }
    private void OnCollisionStay(Collision collision)
    {
        NullifyTargetRotation();
    }
}
