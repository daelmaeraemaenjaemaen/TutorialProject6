using UnityEngine;
using UnityEngine.Audio;

public class Music : MonoBehaviour
{
    [Header("Mixer Routing")]
    [SerializeField] private AudioMixerGroup bgmGroup;
    [SerializeField] private AudioMixerGroup sfxGroup;
    
    [SerializeField] private AudioSource audioSource;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (bgmGroup != null) audioSource.outputAudioMixerGroup = bgmGroup;
        audioSource.loop = true;
        audioSource.Play();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
