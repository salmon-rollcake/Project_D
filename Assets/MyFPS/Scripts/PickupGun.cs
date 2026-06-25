using UnityEngine;
using UnityEngine.UI;

namespace MyFPS
{
    /// <summary>
    /// 문 오브젝트 상호작용 구현
    /// 근접하여 마우스로 감지하면 액션 UI 팝업
    /// </summary>
    public class PickupGun : MonoBehaviour
    {
        // 유니티 에디터의 Animator 컴포넌트를 연결할 변수
        private Animator animator;

        // 현재 문이 열려있는지 여부를 체크하는 변수
        private bool isPickup = false;

        // Animator에 설정한 파라미터 이름과 정확히 일치해야 합니다.
        [SerializeField] private string animationBoolName = "isPickup";

        [SerializeField] GameObject gun;
        [SerializeField] GameObject interactGunUI;

        void Awake()
        {
            // 문 오브젝트에 붙어있는 Animator 컴포넌트를 자동으로 가져옵니다.
            animator = GetComponent<Animator>();

            if (gun != null)
            {
                gun.SetActive(false);
            }
            
            if (interactGunUI != null)
            {
                interactGunUI.SetActive(false);
            }

            if (animator == null)
            {
                Debug.LogError($"{gameObject.name} 오브젝트에 Animator 컴포넌트가 없습니다!");
            }
        }

        // 플레이어가 바라보고 E키를 눌렀을 때 호출될 함수
        public void GunPickup()
        {
            if (animator == null) return;

            gun.SetActive(true);

            // Animator 컨트롤러의 Bool 파라미터 값을 변경하여 애니메이션을 재생합니다.
            animator.SetBool(animationBoolName, isPickup);

            if (isPickup)
            {
                Debug.Log("총 획득");

            }
        }
    }
}