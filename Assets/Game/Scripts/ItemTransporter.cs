using UnityEngine;
using static UnityEditor.Progress;

public class ItemTransporter : MonoBehaviour
{
    [SerializeField] private float m_followSpeed = 1f;
    [SerializeField] private float m_spinModifier = .1f;
    [SerializeField] private Transform m_target;
    private Rigidbody m_rigidbody;
    private Item m_item;
    private bool m_active = false;
    private void Start()
    {
        m_rigidbody = GetComponent<Rigidbody>();
    }
    private void FixedUpdate()
    {
        if (m_active)
        {
            FollowTarget();
        }
    }

    private void FollowTarget()
    {
        Vector3 moveVector = m_target.position - m_rigidbody.position;
        m_rigidbody.velocity = moveVector.normalized * m_followSpeed;
    }
    public void Activate(Item item)
    {
        float rand1 = Random.Range(.5f, 1f);
        float rand2 = Random.Range(.5f, 1f);
        float rand3 = Random.Range(.5f, 1f);
        item.Interactable = false;
        item.ConstantForce.enabled = true;
        item.ConstantForce.torque = new Vector3(rand1, rand2, rand3) * m_spinModifier;
        m_active = true;
        m_rigidbody = item.Rigidbody;
        m_item = item;
    }
    public void Deactivate()
    {
        m_active = false;
        m_rigidbody = null;
        m_item.Interactable = true;
        m_item.ConstantForce.enabled = false;
        m_item.ConstantForce.torque = Vector3.zero;
    }
}
