using UnityEngine;

namespace MyFPS
{

    public class AudioManager : Persistantsingleton<AudioManager>
    {
        public Sound[] sounds;

        // 현재 재생중인 BGM 이름을 저장하는 변수
        [SerializeField] string bgmNow = "";

        protected override void Awake()
        {
            base.Awake();

            foreach (var s in sounds)
            {
                s.audioSource = gameObject.AddComponent<AudioSource>();

                s.audioSource.clip = s.audioClip;
                s.audioSource.volume = s.volume;
                s.audioSource.pitch = s.pitch;
                s.audioSource.loop = s.isLoop;
                s.audioSource.playOnAwake = s.playOnAwake;
            }
        }

        public void Play(string name)
        {
            // 이름으로 재생할 사운드를 찾습니다.
            Sound sound = null;
            foreach (var s in sounds)
            {
                if (s.name == name)
                {
                    sound = s;
                    break;
                }
            }

            if (sound == null)
            {
                Debug.LogWarning($"Sound: {name} not found!");
                return;
            }

            sound.audioSource.Play();
        }

        public void Stop(string name)
        {
            // 이름으로 중지시킬 사운드를 찾습니다.
            Sound sound = null;
            foreach (var s in sounds)
            {
                if (s.name == name)
                {
                    sound = s;
                    break;
                }
            }

            if (sound == null)
            {
                Debug.LogWarning($"Sound: {name} not found!");
                return;
            }

            sound.audioSource.Stop();
        }

        // BGM 재생 메서드
        public void PlayBGM(string name)
        {
            // 현재 재생중인 BGM이 있다면 중지
            if (bgmNow == name)
            {
                return; // 이미 재생중인 BGM이면 아무 작업도 하지 않음
            }

            Stop(bgmNow);

            // 이름으로 재생할 사운드를 찾습니다.
            Sound sound = null;
            foreach (var s in sounds)
            {
                if (s.name == name)
                {
                    sound = s;
                    bgmNow = name; // 현재 재생중인 BGM 이름을 업데이트
                    break;
                }
            }

            if (sound == null)
            {
                Debug.LogWarning($"Sound: {name} not found!");
                return;
            }

            sound.audioSource.Play();
        }
    }
}