using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class DetectorPlate : MonoBehaviour
{
    [SerializeField] private ItemTransporter itemTransporter;
    [SerializeField] private float waitTime = 2f;
    private Item m_item;
    private CancellationTokenSource m_cancellationTokenSource;
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent(out Item item))
        {
            if (item.Picked)
                return;

            m_item = item;
            m_cancellationTokenSource = new CancellationTokenSource();
            WaitUntilStartTransporting(m_cancellationTokenSource.Token);
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        m_cancellationTokenSource?.Cancel();
        m_item = null;
    }
    private async void WaitUntilStartTransporting(CancellationToken ct)
    {
        Debug.Log("Transporting Timer Started");
        float timer = 0;
        while (timer < waitTime)
        {
            timer += Time.deltaTime;
            await Task.Yield();
            if (ct.IsCancellationRequested)
            { 
                Debug.Log("StartTransportingCanceled");
                return;
            }
        }
        if (ct.IsCancellationRequested)
        {
            Debug.Log("StartTransportingCanceled");
            return;
        }
        Debug.Log("StartTransporting");
        StartTransporting();

    }
    private void StartTransporting()
    {
        itemTransporter.Activate(m_item);
    }
}
