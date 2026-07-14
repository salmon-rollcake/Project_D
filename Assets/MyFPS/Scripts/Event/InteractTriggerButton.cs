using System;
using UnityEngine;

namespace MyFPS
{
    public class InteractTriggerButton : MonoBehaviour
    {
        [Header("상호작용 조건 설정")]
        [Tooltip("이 버튼을 작동하기 위해 열쇠가 필요한지 여부입니다.")]
        public bool needsKey = false; // ★ 추가: 열쇠가 필요한 버튼인지 체크하는 옵션

        [Header("마테리얼 설정")]
        [SerializeField] private Renderer buttonRenderer;          
        [SerializeField] private Material defaultMaterial;          
        [SerializeField] private Material activeMaterial;           

        [Header("연동 애니메이션 설정")]
        [SerializeField] private Animator targetAnimator;           
        [SerializeField] private string animatorBoolParam = "IsOpen"; 

        [Header("사운드 설정 (선택사항)")]
        [SerializeField] private AudioSource interactSound;          

        private bool isOn = false;

        // 기존에 안 쓰던 getKey 변수는 PlayerInteract에서 통합 관리하므로 제거해도 무방합니다.

        public bool IsOn => isOn;

        void Awake()
        {
            if (buttonRenderer == null)
            {
                buttonRenderer = GetComponent<Renderer>();
            }

            if (buttonRenderer != null && defaultMaterial != null)
            {
                buttonRenderer.material = defaultMaterial;
            }
        }

        public void Interact()
        {
            isOn = !isOn;

            if (buttonRenderer != null)
            {
                Material targetMat = isOn ? activeMaterial : defaultMaterial;
                if (targetMat != null)
                {
                    buttonRenderer.material = targetMat;
                }
            }

            if (targetAnimator != null && !string.IsNullOrEmpty(animatorBoolParam))
            {
                targetAnimator.SetBool(animatorBoolParam, isOn);
                Debug.Log($"[InteractTriggerButton] {gameObject.name} 상호작용: Param '{animatorBoolParam}' -> {isOn}");
            }

            if (interactSound != null)
            {
                interactSound.Play();
            }
        }
    }
}
