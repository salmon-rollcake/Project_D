using UnityEngine;
using System.Collections;

namespace MyFPS
{

    public class GameOpening : MonoBehaviour
    {
        [Header("시작 연출 설정")]
        [SerializeField] private GameObject introTextUI;       // 3초간 보여줄 인트로 텍스트 UI
        [SerializeField] private float introDelayTime = 3.0f;  // 대기할 시간 (3초)

        [Header("제어할 컴포넌트들")]
        // 기존에 만든 상호작용 스크립트
        private PlayerInteract playerInteraction;
        MouseLook mouseLook;

        // 플레이어 이동을 담당하는 컴포넌트 (본인이 사용하는 것에 맞춰 제어)
        // 예: CharacterController, Rigidbody, 혹은 별도의 PlayerMovement 스크립트
        private PlayerMove playerMove;

        void Awake()
        {
            // 컴포넌트들을 미리 가져옵니다.
            playerInteraction = GetComponent<PlayerInteract>();
            mouseLook = GetComponent<MouseLook>();
            playerMove = GetComponent<PlayerMove>();
        }

        void Start()
        {
            // 게임이 시작되자마자 인트로 연출 코루틴을 실행합니다.
            StartCoroutine(StartIntroSequence());
        }

        IEnumerator StartIntroSequence()
        {
            // 1. 플레이어 조작 및 상호작용 비활성화
            SetPlayerControl(false);

            // 2. 인트로 텍스트 UI 활성화
            if (introTextUI != null)
            {
                introTextUI.SetActive(true);
            }

            // 3. 지정된 시간(3초)만큼 대기
            // SceneFader가 페이드 인 되는 시간(약 1초)과 텍스트 유지 시간을 고려하여 대기합니다.
            yield return new WaitForSeconds(introDelayTime);

            // 4. 인트로 텍스트 UI 비활성화
            if (introTextUI != null)
            {
                introTextUI.SetActive(false);
            }

            // 5. 플레이어 조작 및 상호작용 활성화! 게임 시작!
            SetPlayerControl(true);

            Debug.Log("게임 시작! 플레이어 조작이 활성화되었습니다.");
        }

        // 플레이어의 모든 조작 상태를 한 번에 켜고 끄는 헬퍼 함수
        private void SetPlayerControl(bool isEnable)
        {
            // 상호작용(Raycast) 스크립트 켜고 끄기
            if (playerInteraction != null)
            {
                playerInteraction.enabled = isEnable;
            }

            // 이동 컴포넌트 켜고 끄기 (CharacterController를 쓰는 경우)
            if (playerMove != null)
            {
                playerMove.enabled = isEnable;
            }

            // 💡 [팁] 만약 별도의 마우스 시선 회전 스크립트(MouseLook 등)나 
            // PlayerMovement 스크립트가 있다면 여기에 함께 켜고 끄도록 추가해 주셔야 완전히 멈춥니다!
            
            if (mouseLook != null) mouseLook.enabled = isEnable;
        }
    }
}