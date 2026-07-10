using System;
using UnityEngine;

namespace MyFPS
{
    /// <summary>
    /// PlayerInteract와 연계하여 작동하는 재사용 가능한 버튼형 트리거 스크립트
    /// 상호작용 시 On/Off 상태를 토글하며 마테리얼과 애니메이터 상태를 연동합니다.
    /// </summary>
    public class InteractTriggerButton : MonoBehaviour
    {
        [Header("마테리얼 설정")]
        [SerializeField] private Renderer buttonRenderer;          // 마테리얼을 바꿀 대상 (버튼 메시 렌더러)
        [SerializeField] private Material defaultMaterial;          // 기본 마테리얼 (Off 상태)
        [SerializeField] private Material activeMaterial;           // 활성화 마테리얼 (On 상태)

        [Header("연동 애니메이션 설정")]
        [SerializeField] private Animator targetAnimator;           // 제어할 대상 애니메이터 (예: 문)
        [SerializeField] private string animatorBoolParam = "IsOpen"; // 제어할 Bool 파라미터 이름

        [Header("사운드 설정 (선택사항)")]
        [SerializeField] private AudioSource interactSound;          // 상호작용 시 재생할 사운드

        private bool isOn = false;

        public bool getKey = false;

        // 외부에서 버튼의 현재 켜짐/꺼짐 상태를 확인하기 위한 프로퍼티
        public bool IsOn => isOn;

        void Awake()
        {
            if (buttonRenderer == null)
            {
                buttonRenderer = GetComponent<Renderer>();
            }

            // 초기 마테리얼 설정
            if (buttonRenderer != null && defaultMaterial != null)
            {
                buttonRenderer.material = defaultMaterial;
            }
        }

        /// <summary>
        /// PlayerInteract에 의해 E키 상호작용 감지 시 호출되는 메서드
        /// </summary>
        public void Interact()
        {
            // On/Off 상태 토글
            isOn = !isOn;

            // 1. 버튼 마테리얼 교체
            if (buttonRenderer != null)
            {
                Material targetMat = isOn ? activeMaterial : defaultMaterial;
                if (targetMat != null)
                {
                    buttonRenderer.material = targetMat;
                }
            }

            // 2. 연동된 애니메이터 상태 업데이트 (문 열림/닫힘 등)
            if (targetAnimator != null && !string.IsNullOrEmpty(animatorBoolParam))
            {
                targetAnimator.SetBool(animatorBoolParam, isOn);
                Debug.Log($"[InteractTriggerButton] {gameObject.name} 상호작용: Param '{animatorBoolParam}' -> {isOn}");
            }

            // 3. 사운드 재생
            if (interactSound != null)
            {
                interactSound.Play();
            }
        }
    }
}
