using System;
using System.Collections;
using System.Text;
using Newtonsoft.Json;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

[System.Serializable]
public class ElevenlabsConfig
{
    public string key;
    public string apiUrl;
    public string voiceId;
}
public class ElevenlabsAPI : MonoBehaviour
{
    [SerializeField] private TextAsset jsonApi;
    private string m_voiceId;
    private string m_apiKey;
    private string m_apiUrl = "https://api.elevenlabs.io";

    [SerializeField] private AudioSource m_audioSource;
    [SerializeField] private AudioClip m_audioClip;

    // If true, the audio will be streamed instead of downloaded
    // Unfortunately, Unity has some problems with streaming audio
    // but I left this option here in case you want to try it.
    public bool Streaming = false;

    [Range(0, 4)]
    public int LatencyOptimization;

    // This event is used to broadcast the received AudioClip
    public UnityEvent<AudioClip> AudioReceived;

    private void Start()
    {
        FillDataFromConfig();
    }

    private void FillDataFromConfig()
    {
        ElevenlabsConfig elevenlabsConfig = JsonUtility.FromJson<ElevenlabsConfig>(jsonApi.text);
        m_voiceId = elevenlabsConfig.voiceId;
        m_apiKey = elevenlabsConfig.key;
        m_apiUrl = elevenlabsConfig.apiUrl;
    }

    public void GetAudio(string text)
    {
        StartCoroutine(DoRequest(text));
    }    
    public void PlayAudio()
    {
        StopAudio();
        m_audioSource.clip = m_audioClip;
        if (m_audioSource.clip == null)
        {
            Debug.LogWarning("No Audio Clip Generated!");
        }
        m_audioSource.Play();
        m_audioClip = null;
    }
    public void StopAudio()
    {
        m_audioSource.Stop();
        //m_audioClip = null;
    }
    public IEnumerator DoRequest(string message)
    {
        var postData = new TextToSpeechRequest
        {
            text = message,
            //model_id = "eleven_monolingual_v1"
            model_id = "eleven_turbo_v2"
        };

        // TODO: This could be easily exposed in the Unity inspector,
        // but I had no use for it in my work demo.
        var voiceSetting = new VoiceSettings
        {
            speed = 0.9f,
            stability = 0,
            similarity_boost = 0,
            style = 0.5f,
            use_speaker_boost = true
        };
        postData.voice_settings = voiceSetting;
        var json = JsonConvert.SerializeObject(postData);
        var uH = new UploadHandlerRaw(Encoding.ASCII.GetBytes(json));
        var stream = (Streaming) ? "/stream" : "";
        var url = $"{m_apiUrl}/v1/text-to-speech/{m_voiceId}{stream}?optimize_streaming_latency={LatencyOptimization}";
        var request = UnityWebRequest.PostWwwForm(url, json);
        var downloadHandler = new DownloadHandlerAudioClip(url, AudioType.MPEG);
        if (Streaming)
        {
            downloadHandler.streamAudio = true;
        }
        request.uploadHandler = uH;
        request.downloadHandler = downloadHandler;
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("xi-api-key", m_apiKey);
        request.SetRequestHeader("Accept", "audio/mpeg");
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error downloading audio: " + request.error);
            yield break;
        }
        m_audioClip = downloadHandler.audioClip;
        AudioReceived.Invoke(m_audioClip);
        request.Dispose();
    }

    [Serializable]
    public class TextToSpeechRequest
    {
        public string text;
        public string model_id; // eleven_monolingual_v1
        public VoiceSettings voice_settings;
    }

    [Serializable]
    public class VoiceSettings
    {
        public float speed; // 0.7
        public int stability; // 0
        public int similarity_boost; // 0
        public float style; // 0.5
        public bool use_speaker_boost; // true
    }
}
