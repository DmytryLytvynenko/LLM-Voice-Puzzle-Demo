using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;
using TMPro;

[System.Serializable]
public class UnityAndGeminiKey
{
    public string key;
}

[System.Serializable]
public class Response
{
    public Candidate[] candidates;
}

public class ChatRequest
{
    public Content[] contents;
}

[System.Serializable]
public class Candidate
{
    public Content content;
}

[System.Serializable]
public class Content
{
    public string role;
    public Part[] parts;
}

[System.Serializable]
public class Part
{
    public string text;
}

public class GeminiAPI : MonoBehaviour
{
    [Header("JSON API Configuration")]
    public TextAsset jsonApi;
    private string apiKey = "";
    private string apiEndpoint = "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash-lite:generateContent"; // Edit it and choose your prefer model

    [Header("ChatBot Function")]
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private TMP_Text OutputText;
    [SerializeField, TextArea(5,10)] private string systemPrompt = "";
    [SerializeField] private ElevenlabsAPI elevenlabsAPI;
    [SerializeField] private bool gettingResponse = false;

    [Header("Referances")]
    [SerializeField] private PhysicalButton m_sendMessageButton;

    private string geminiResponse = "";
    private Content[] chatHistory;

    void Start()
    {
        UnityAndGeminiKey jsonApiKey = JsonUtility.FromJson<UnityAndGeminiKey>(jsonApi.text);
        apiKey = jsonApiKey.key;
        InitializeChatHistory(); // Инициализируем историю чата с системным промптом
    }
    private void OnEnable()
    {
        m_sendMessageButton.ButtonPressed += OnSendMessageButtonPressed;
    }
    private void OnDisable()
    {
        m_sendMessageButton.ButtonPressed -= OnSendMessageButtonPressed;
    }
    private void InitializeChatHistory()
    {
        Content systemContent = new Content
        {
            role = "user", // Важно: системный промпт также отправляется как "user"
            parts = new Part[]
            {
                new Part { text = systemPrompt }
            }
        };

        chatHistory = new Content[] { systemContent };
    }
    public void SendMessage()
    {
        StartCoroutine(FormResponse());
    }
    public void SendSpecifiedMessage(string message)
    {
        StartCoroutine(FormResponse(message));
    }
    public IEnumerator FormResponse(string message = null)
    {
        gettingResponse = true;
        string userMessage;
        if (string.IsNullOrEmpty(message))
        {
            userMessage = inputField.text;
        }
        else
        {
            userMessage = message;
        }
        yield return SendChatRequestToGemini(userMessage);
        if (geminiResponse == "")
        {
            Debug.LogError("No Gemini responce Generated!");
        }
        yield return elevenlabsAPI.DoRequest(geminiResponse);
        OutputText.text = geminiResponse;
        elevenlabsAPI.PlayAudio();
        geminiResponse = "";
        gettingResponse = false;
    }
    private IEnumerator SendChatRequestToGemini(string newMessage)
    {

        string url = $"{apiEndpoint}?key={apiKey}";
        print(url);
        Content userContent = new Content
        {
            role = "user",
            parts = new Part[]
            {
                new Part { text = newMessage }
            }
        };

        List<Content> contentsList = new List<Content>(chatHistory);
        contentsList.Add(userContent);
        chatHistory = contentsList.ToArray();

        ChatRequest chatRequest = new ChatRequest { contents = chatHistory };

        string jsonData = JsonUtility.ToJson(chatRequest);

        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(jsonData);

        // Create a UnityWebRequest with the JSON data
        using (UnityWebRequest www = new UnityWebRequest(url, "POST"))
        {
            www.uploadHandler = new UploadHandlerRaw(jsonToSend);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(www.error);
            }
            else
            {
                Debug.Log("Request complete!");
                Response response = JsonUtility.FromJson<Response>(www.downloadHandler.text);
                if (response.candidates.Length > 0 && response.candidates[0].content.parts.Length > 0)
                {
                    //This is the response to your request
                    string reply = response.candidates[0].content.parts[0].text;
                    geminiResponse = reply;
                    Content botContent = new Content
                    {
                        role = "model",
                        parts = new Part[]
                        {
                                new Part { text = reply }
                        }
                    };

                    Debug.Log(reply);
                    //This part shows the text in the Canvas
                    //OutputText.text = reply;
                    //This part adds the response to the chat history, for your next message
                    contentsList.Add(botContent);
                    chatHistory = contentsList.ToArray();
                }
                else
                {
                    Debug.Log("No text found.");
                }
            }
        }
    }
    private void OnSendMessageButtonPressed()
    {
        if (gettingResponse)
        {
            print("gettingResponse");
            return;
        }
        SendMessage();
    }
}


