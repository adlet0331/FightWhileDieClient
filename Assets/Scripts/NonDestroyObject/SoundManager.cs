using System;
using UI;
using UnityEngine;

namespace NonDestroyObject
{
    [Serializable]
    public enum ClipName
    {
        ButtonClick = 0,
        Hit1 = 1,
        Hit2 = 2,
        Hit3 = 3,
        GatchaOpen = 4,
        EnhanceSuccess = 5,
        EnhanceFail = 6,
    }
    [Serializable]
    public class SoundClip
    {
        public ClipName ClipName; 
        public AudioClip AudioClip;
    }
    public class SoundManager : Singleton<SoundManager>
    {
        [SerializeField] private bool bgmOn;
        [SerializeField] private bool clipOn;

        [SerializeField] private int clipVolume;
        [SerializeField] private int bgmVolume;

        [SerializeField] private SliderWithValue clipSlider;
        [SerializeField] private SliderWithValue bgmSlider;

        [SerializeField] private AudioSource clipSource;
        [SerializeField] private AudioSource bgmSource;

        private void Start()
        {
            bgmOn = 1 == PlayerPrefs.GetInt("BgmOn", 1);
            clipOn = 1 == PlayerPrefs.GetInt("ClipOn", 1);
            bgmVolume = PlayerPrefs.GetInt("BgmVolume", 100);
            clipVolume = PlayerPrefs.GetInt("ClipVolume", 100);
            
            bgmSource.volume = (bgmVolume / 100.0f);
            clipSource.volume = (clipVolume / 100.0f);
            
            bgmSlider.InitValue(bgmVolume);
            clipSlider.InitValue(clipVolume);
        }

        public void SetBgmOn(bool value)
        {
            bgmOn = value;
            PlayerPrefs.SetInt("BgmOn", bgmOn ? 1 : 0);
        }

        public void SetClipOn(bool value)
        {
            clipOn = value;
            PlayerPrefs.SetInt("ClipOn", clipOn ? 1 : 0);
        }
        
        public void SetBgmVolume(float value)
        {
            bgmVolume = (int) value;
            bgmSource.volume = (bgmVolume / 100.0f);
            PlayerPrefs.SetInt("BgmVolume", bgmVolume);
        }

        public void SetClipVolume(float value)
        {
            clipVolume = (int) value;
            clipSource.volume = (clipVolume / 100.0f);
            PlayerPrefs.SetInt("ClipVolume", clipVolume);
        }
        
        [SerializeField] private SoundClip[] audioClip;

        public void PlayClip(int clipName)
        {
            if (!clipOn) return;
            
            foreach (var clip in audioClip)
            {
                if ((int)clip.ClipName == clipName)
                {
                    clipSource.clip = clip.AudioClip;
                    clipSource.Play();
                    return;
                }
            }
        }

        public void PlayBGM()
        {
            
        }
    }
}