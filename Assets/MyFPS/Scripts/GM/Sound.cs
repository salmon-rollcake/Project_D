using UnityEngine;
using System;
using UnityEngine.Audio; // 추가

namespace MyFPS
{
    [Serializable]
    public class Sound
    {
        public string name;
        public AudioClip audioClip;

        // 추가: 이 사운드가 속할 오디오 믹서 그룹 (인스펙터에서 BGM 혹은 SFX 그룹 지정)
        public AudioMixerGroup mixerGroup; 

        [Range(0f, 1f)] public float volume = 1f; // 기본값 세팅 및 슬라이더 제한
        public float pitch = 1f;

        public bool isLoop;
        public bool playOnAwake;

        [HideInInspector]
        public AudioSource audioSource;
    }
}
