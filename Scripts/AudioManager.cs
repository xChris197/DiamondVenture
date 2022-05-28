using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private List<Sound> sfxSounds;
    [SerializeField] private List<Sound> themeSounds;

    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource themeSource;

    private void PlaySFXSound(string _name)
    {
        foreach (Sound s in sfxSounds)
        {
            if (_name == s.audioName)
            {
                sfxSource.clip = s.audio;
                if (sfxSource.clip != null)
                {
                    sfxSource.Play();
                }
            }
        }
    }
    
    private void PlayThemeSound(string _name)
    {
        foreach (Sound s in themeSounds)
        {
            if (_name == s.audioName)
            {
                themeSource.clip = s.audio;
                if (themeSource.clip != null)
                {
                    themeSource.Play();
                }
            }
        }
    }

    private void OnEnable()
    {
        CustomEvents.Audio.OnPlaySfxSound += PlaySFXSound;
        CustomEvents.Audio.OnPlayThemeSound += PlayThemeSound;
    }

    private void OnDisable()
    {
        CustomEvents.Audio.OnPlaySfxSound -= PlaySFXSound;
        CustomEvents.Audio.OnPlayThemeSound -= PlayThemeSound;
    }
}
