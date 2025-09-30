using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class ItemAnalizer : MonoBehaviour
{
    [SerializeField] private GeminiAPI m_geminiAPI;
    [SerializeField] private AltarInteraction m_altar;
    [SerializeField] private float m_analizeTime;
    [SerializeField] private PhysicalButton m_sendMessageButton;
    [field: SerializeField] public bool Busy { get; private set; } = false;
    private string m_message = "";
    [SerializeField] private float timer;
    private void OnEnable()
    {
        m_sendMessageButton.ButtonPressed += OnSendMessageButtonPressed;
    }
    private void OnDisable()
    {
        m_sendMessageButton.ButtonPressed -= OnSendMessageButtonPressed;
    }
    public void AnalizeItem()
    {
        if (Busy) return;
        if (m_altar.Item == null) return;


        StringBuilder sb = new StringBuilder();
        sb.AppendLine("Say the name of the object and analize it.");
        sb.AppendLine($"Name: {m_altar.Item.ItemData.Name}");
        sb.AppendLine($"Item description: { m_altar.Item.ItemData.Description}");

        m_message = sb.ToString();

        Busy = true;
        StartAnalizeItemProcess();
    }
    private async void StartAnalizeItemProcess()
    {
        await Task.Delay(1000);
        timer = 0;
        m_altar.StartInteracting();
        while (timer < m_analizeTime)
        {
            timer += Time.deltaTime;
            await Task.Yield();
        }
        m_altar.StopInteracting();
        m_geminiAPI.SendSpecifiedMessage(m_message);
    }
    private void OnSendMessageButtonPressed()
    {
        AnalizeItem();
    }
}
