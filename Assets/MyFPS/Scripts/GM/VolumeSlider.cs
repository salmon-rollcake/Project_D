using UnityEngine;
using UnityEngine.UI;

namespace MyFPS
{
    public class VolumeSlider : MonoBehaviour
    {
        [Header("UI Components")]
        [SerializeField] private Slider bgmSlider;
        [SerializeField] private Slider sfxSlider;

        private void Start()
        {
            // 1. 기존에 저장된 볼륨 값 로드 (없으면 기본값 1f)
            float savedBGM = PlayerPrefs.GetFloat("BGMVolume", 1f);
            float savedSFX = PlayerPrefs.GetFloat("SFXVolume", 1f);

            // 2. 슬라이더 UI에 값 반영
            if (bgmSlider != null)
            {
                bgmSlider.value = savedBGM;
                // 슬라이더 값이 바뀔 때마다 AudioManager의 메서드가 호출되도록 이벤트 연결
                bgmSlider.onValueChanged.AddListener(OnBGMVolumeChanged);
            }

            if (sfxSlider != null)
            {
                sfxSlider.value = savedSFX;
                sfxSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
            }
        }

        // 슬라이더가 조작될 때 호출되는 리스너 메서드들
        private void OnBGMVolumeChanged(float value)
        {
            // 오디오 매니저 싱글톤을 통해 볼륨만 조절하도록 명령
            AudioManager.Instance.SetBGMVolume(value);
        }

        private void OnSFXVolumeChanged(float value)
        {
            AudioManager.Instance.SetSFXVolume(value);
        }

        private void OnDestroy()
        {
            // 메모리 누수 방지를 위해 리스너 해제 (좋은 습관)
            if (bgmSlider != null) bgmSlider.onValueChanged.RemoveListener(OnBGMVolumeChanged);
            if (sfxSlider != null) sfxSlider.onValueChanged.RemoveListener(OnSFXVolumeChanged);
        }
    }
}
