using UnityEngine;

public class AltarInteraction : MonoBehaviour
{
    public bool ContainsItem { get { return m_item != null; } }
    public Item Item { get { return m_item; } }

    [SerializeField] private LayerMask m_InteractionMask;
    [SerializeField] private ParticlesController m_particlesController;
    [SerializeField] private Item m_item = null;
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent(out Item item))
        {
            m_item = item;
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (m_item == null)
        {
            return;
        }
        if (collision.gameObject == m_item.gameObject)
        {
            m_item = null;
        }
    }
    public void StartInteracting()
    {
        if (m_item != null)
        {
            print(m_item);
            m_particlesController.Play();
        }
    }
    public void StopInteracting()
    {
        m_particlesController.Stop();
    }
}
