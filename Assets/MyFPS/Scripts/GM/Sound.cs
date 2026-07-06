using UnityEngine;
using System;

namespace MyFPS
{
    [Serializable]
    public class Sound
    {
        public string name;

        public AudioClip audioClip;

        public float volume;
        public float pitch;

        public bool isLoop;
        public bool playOnAwake;

        [HideInInspector]
        public AudioSource audioSource;
    }
}
