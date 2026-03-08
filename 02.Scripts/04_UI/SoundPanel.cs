using UnityEngine;
using UnityEngine.UI;

public class SoundPanel : MonoBehaviour
{
    public Slider bgmVolumeSlider;
    public Slider sfxVolumeSlider;

    void Awake() 
    {
        gameObject.SetActive(false); 
    }
    
    void OnEnable() 
    {
        float savedBGMVolume = PlayerPrefs.GetFloat("BGMVolume", 0.5f);
        bgmVolumeSlider.value = savedBGMVolume;
        SoundManager.Instance.SetBGMVolume(savedBGMVolume); 
        bgmVolumeSlider.onValueChanged.AddListener(OnBGMVolumeChanged);

        float savedSFXVolume = PlayerPrefs.GetFloat("SFXVolume", 0.5f);
        sfxVolumeSlider.value = savedSFXVolume;
        SoundManager.Instance.SetSFXVolume(savedSFXVolume); 
        sfxVolumeSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
    }

    private void OnBGMVolumeChanged(float volume)
    {
        SoundManager.Instance.SetBGMVolume(volume); 
        PlayerPrefs.SetFloat("BGMVolume", volume);
        PlayerPrefs.Save();
    }

    private void OnSFXVolumeChanged(float volume)
    {
        SoundManager.Instance.SetSFXVolume(volume); 
        PlayerPrefs.SetFloat("SFXVolume", volume);
        PlayerPrefs.Save();
    }
    
    void OnDisable()
    {
        bgmVolumeSlider.onValueChanged.RemoveListener(OnBGMVolumeChanged);
        sfxVolumeSlider.onValueChanged.RemoveListener(OnSFXVolumeChanged);
        gameObject.SetActive(false);
    }
}