using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class ParticlesController : MonoBehaviour
{
    [SerializeField] private List<ParticleSystem> m_psList = new List<ParticleSystem>();
    [SerializeField] private Light m_psLight;
    [SerializeField] private float m_stopDuration = 1.0f;

    [SerializeField, ColorUsage(true, true)] private List<Color> m_defaultColors = new List<Color>();
    private List<ParticleSystemRenderer> m_renderers = new List<ParticleSystemRenderer>();
    private ParticleSystem m_mainPS;
    [SerializeField] private float m_startIntensity;
    private Task m_task;


    private void Awake()
    {
        m_mainPS = GetComponent<ParticleSystem>();
        if (m_psList.Count == 0) 
        {
            Debug.LogWarning("No Particle Systems Added");
        }
        if (!m_psLight)
        {
            Debug.LogWarning("No Particle System Light Added");
        }
        m_startIntensity = m_psLight.intensity;
        for (int i = 0; i < m_psList.Count; i++)
        {
            m_renderers.Add(m_psList[i].GetComponent<ParticleSystemRenderer>());
            m_renderers[i].material.color = new Color(m_renderers[i].material.color.r,
                                                      m_renderers[i].material.color.g,
                                                      m_renderers[i].material.color.b,
                                                      1);
            m_defaultColors.Add(m_renderers[i].material.color);
        }
    }
    private void OnDisable()
    {
        SetDefaultMaterialsValues();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            Stop();
        }        
        if (Input.GetKeyDown(KeyCode.P))
        {
            Play();
        }
    }
    public void Play()
    {
        for (int i = 0; i < m_renderers.Count; i++)
        {
            m_renderers[i].material.color = m_defaultColors[i];
        }
        m_psLight.intensity = m_startIntensity;
        m_mainPS.Simulate(0,true);
        m_mainPS.Play();
    }
    public void Stop()
    {
        if (m_mainPS.isPlaying)
        {
            m_task = StopPS();
        }
    }
    private async Task StopPS()
    {
        m_mainPS.Stop();
        float counter = m_stopDuration;
        while (counter > 0)
        {
            counter -= Time.deltaTime;
            float progress = counter / m_stopDuration;
            Mathf.Clamp(progress, 0f, 1f);
            if (m_psLight)
            {
                m_psLight.intensity = m_startIntensity * progress;
            }
            for (int i = 0;i < m_renderers.Count;i++)
            {
                m_renderers[i].material.color = new Color(m_defaultColors[i].r,
                                                        m_defaultColors[i].g,
                                                        m_defaultColors[i].b,
                                                        progress);
            }
            await Task.Yield();
        }
        m_mainPS.Clear();
    }
    private void SetDefaultMaterialsValues()
    {
        if (m_psLight)
        {
            m_psLight.intensity = m_startIntensity;
        }
        for (int i = 0; i < m_renderers.Count; i++)
        {
            m_renderers[i].material.color = new Color(m_defaultColors[i].r,
                                                    m_defaultColors[i].g,
                                                    m_defaultColors[i].b,
                                                    1);
        }
    }
}
