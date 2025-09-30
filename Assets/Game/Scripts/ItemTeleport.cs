using UnityEngine;

public class ItemTeleport : MonoBehaviour
{
    [SerializeField] private LayerMask m_interactionMask;
    [SerializeField] private Transform m_destination;
    [SerializeField] private GameObject m_teleportationEffect;
    [SerializeField] private ItemTransporter m_itemTransporter;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out Item item))
        {
            item.transform.position = m_destination.position;
            m_itemTransporter.Deactivate();
        }
    }
}
