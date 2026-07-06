using UnityEngine;

namespace MySample
{
    public class SoundTest : MonoBehaviour
    {
        public AudioClip audioClip;

        [SerializeField] float volume = 1f;
        [SerializeField] float pitch = 1f;
        [SerializeField] bool isLoop = false;
        [SerializeField] bool playOnAwake = false;

        AudioSource audioSource;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();

            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
                audioSource.clip = audioClip;
            audioSource.volume = volume;
            audioSource.pitch = pitch;
            audioSource.loop = isLoop;
            audioSource.playOnAwake = playOnAwake;
        }

        private void Start()
        {
            audioSource.Play();
        }
    }
}
