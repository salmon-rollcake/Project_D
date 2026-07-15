using UnityEngine;

namespace MyFPS
{
    /// <summary>
    /// 문 오브젝트 상호작용 구현
    /// 근접하여 마우스로 감지하면 액션 UI 팝업
    /// </summary>
    public class DoorCellOpen : MonoBehaviour
    {
        // 유니티 에디터의 Animator 컴포넌트를 연결할 변수
        private Animator animator;

        [SerializeField] AudioSource doorOpenSound;

        // 현재 문이 열려있는지 여부를 체크하는 변수
        private bool isOpen = false;

        // Animator에 설정한 파라미터 이름과 정확히 일치해야 합니다.
        [SerializeField] private string animationBoolName = "IsOpen";

        void Awake()
        {
            // 문 오브젝트에 붙어있는 Animator 컴포넌트를 자동으로 가져옵니다.
            animator = GetComponent<Animator>();

            if (animator == null)
            {
                Debug.LogError($"{gameObject.name} 오브젝트에 Animator 컴포넌트가 없습니다!");
            }
        }

        // 플레이어가 바라보고 E키를 눌렀을 때 호출될 함수
        public void OpenDoor()
        {
            if (animator == null) return;

            // 상태를 반전시킵니다 (열려있으면 false, 닫혀있으면 true)
            isOpen = !isOpen;

            // Animator 컨트롤러의 Bool 파라미터 값을 변경하여 애니메이션을 재생합니다.
            animator.SetBool(animationBoolName, isOpen);
            
            if (doorOpenSound != null && doorOpenSound.clip != null)
            {
                if (isOpen)
                {
                    // 문이 열릴 때: 정방향 재생
                    doorOpenSound.pitch = 1f; // 원래 속도, 정방향
                    doorOpenSound.time = 0f;  // 맨 앞에서부터 시작
                    doorOpenSound.Play();
                }
                else
                {
                    // 문이 닫힐 때: 역방향 재생
                    doorOpenSound.pitch = -1f; // 역방향
                    doorOpenSound.time = doorOpenSound.clip.length - 0.01f; // 맨 끝에서 시작
                    doorOpenSound.Play();
                }
            }
            /*
            // 디버그 로그 출력
            if (isOpen)
            {
                Debug.Log("문이 열리는 애니메이션을 재생합니다.");
            }
            else
            {
                Debug.Log("문이 닫히는 애니메이션을 재생합니다.");
            }
            */
        }
    }
}