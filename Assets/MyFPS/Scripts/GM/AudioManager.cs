using UnityEngine;
using UnityEngine.Audio;
using System;

namespace MyFPS
{
    public class AudioManager : Persistantsingleton<AudioManager>
    {
        public Sound[] sounds;

        [SerializeField] string bgmNow = "";
        [SerializeField] private AudioMixer audioMixer;

        protected override void Awake()
        {
            base.Awake();

            if (Instance != this) return; 

            foreach (var s in sounds)
            {
                s.audioSource = gameObject.AddComponent<AudioSource>();
                s.audioSource.clip = s.audioClip;
                s.audioSource.volume = s.volume;
                s.audioSource.pitch = s.pitch;
                s.audioSource.loop = s.isLoop;
                s.audioSource.playOnAwake = s.playOnAwake;

                if (s.mixerGroup != null)
                {
                    s.audioSource.outputAudioMixerGroup = s.mixerGroup;
                }
            }
        }

        private void Start()
        {
            // 게임이 켜질 때 최초 1회 저장된 볼륨 값으로 오디오 믹서 초기화
            InitializeVolume();
        }

        private void InitializeVolume()
        {
            float savedBGM = PlayerPrefs.GetFloat("BGMVolume", 1f);
            float savedSFX = PlayerPrefs.GetFloat("SFXVolume", 1f);

            SetBGMVolume(savedBGM);
            SetSFXVolume(savedSFX);
        }

        // 외부(VolumeSliderController 등)에서 호출하여 볼륨을 조절하는 public 메서드들
        public void SetBGMVolume(float sliderValue)
        {
            float clampedValue = Mathf.Clamp(sliderValue, 0.0001f, 1f); 
            audioMixer.SetFloat("BGMVol", Mathf.Log10(clampedValue) * 20);
            PlayerPrefs.SetFloat("BGMVolume", sliderValue);
        }

        public void SetSFXVolume(float sliderValue)
        {
            float clampedValue = Mathf.Clamp(sliderValue, 0.0001f, 1f);
            audioMixer.SetFloat("SFXVol", Mathf.Log10(clampedValue) * 20);
            PlayerPrefs.SetFloat("SFXVolume", sliderValue);
        }

        // --- 재생 및 정지 로직 (이전과 동일) ---
        public void Play(string name)
        {
            Sound sound = Array.Find(sounds, s => s.name == name);
            if (sound == null) return;
            sound.audioSource.Play();
        }

        public void Stop(string name)
        {
            Sound sound = Array.Find(sounds, s => s.name == name);
            if (sound == null) return;
            sound.audioSource.Stop();
        }

        public void PlayBGM(string name)
        {
            if (bgmNow == name) return;
            Stop(bgmNow);

            Sound sound = Array.Find(sounds, s => s.name == name);
            if (sound == null) return;

            bgmNow = name; 
            sound.audioSource.Play();
        }
    }
}