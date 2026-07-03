using UnityEngine;
using System.Collections;
using TMPro;

namespace MyFPS
{

    public class IntroManager : MonoBehaviour
    {
        [Header("시작 연출 설정")]
        [SerializeField] private GameObject introTextUI;       // 3초간 보여줄 인트로 텍스트 UI
        [SerializeField] private float introDelayTime = 3.0f;  // 대기할 시간 (3초)

        [Header("더빙 오디오 설정")]
        [SerializeField] private AudioSource audioSource;       // 오디오 재생을 위한 소스
        [SerializeField] private AudioClip voiceClip1;          // 첫 번째 대사 음원
        [SerializeField] private AudioClip voiceClip2;          // 두 번째 대사 음원

        [Header("자막 설정")]
        [SerializeField] private TextMeshProUGUI introTextMeshPro; // 자막을 표시할 TMPro UI 컴포넌트
        [SerializeField] private string introText1 = "여기는 어디지..."; // 첫 번째 자막 내용
        [SerializeField] private string introText2 = "누가 날 가둔 거야..."; // 두 번째 자막 내용
        [SerializeField] private float delayBetweenTexts = 1.0f; // 두 자막 사이의 대기 시간 (초)

        [Header("제어할 플레이어 참조")]
        // 플레이어 최상위 오브젝트를 연결합니다.
        [SerializeField] private GameObject playerObject;

        [SerializeField] GameObject actionUI;

        [SerializeField] GameObject sceneFader;

        // 내부에 숨겨진 컴포넌트들을 담을 변수
        private PlayerInteract playerInteraction;
        private PlayerMove playerMove;
        MouseLook mouseLook;

        private void Awake()
        {
            if (sceneFader != null)
            {
                sceneFader.SetActive(true);
            }
        }

        void Start()
        {
            // 플레이어 오브젝트가 할당되어 있다면 필요한 컴포넌트들을 가져옵니다.
            if (playerObject != null)
            {
                playerInteraction = playerObject.GetComponent<PlayerInteract>();
                playerMove = playerObject.GetComponent<PlayerMove>();
                mouseLook = playerObject.GetComponent<MouseLook>();
            }
            else
            {
                Debug.LogError("IntroManager에 Player Object가 할당되지 않았습니다!");
                return;
            }

            // 게임 시작 시에는 상호작용 UI를 꺼둡니다.
            if (actionUI != null)
            {
                actionUI.SetActive(false);
            }

            // 인트로 연출 시작
            StartCoroutine(StartIntroSequence());
        }

        IEnumerator StartIntroSequence()
        {
            // 1. 플레이어 조작 비활성화
            SetPlayerControl(false);

            // 2. 인트로 텍스트 UI 활성화
            if (introTextUI != null)
            {
                introTextUI.SetActive(true);
            }

            // 첫 번째 대사 출력 및 음원 재생
            if (introTextMeshPro != null)
            {
                introTextMeshPro.text = introText1;
            }

            if (audioSource != null && voiceClip1 != null)
            {
                audioSource.clip = voiceClip1;
                audioSource.Play();
                yield return new WaitForSeconds(voiceClip1.length);
            }
            else
            {
                // 음원이 없을 시 대기
                yield return new WaitForSeconds(introDelayTime * 0.5f);
            }

            // 대사 사이 딜레이
            yield return new WaitForSeconds(delayBetweenTexts);

            // 두 번째 대사 출력 및 음원 재생
            if (introTextMeshPro != null)
            {
                introTextMeshPro.text = introText2;
            }

            if (audioSource != null && voiceClip2 != null)
            {
                audioSource.clip = voiceClip2;
                audioSource.Play();
                yield return new WaitForSeconds(voiceClip2.length);
            }
            else
            {
                // 음원이 없을 시 대기
                yield return new WaitForSeconds(introDelayTime * 0.5f);
            }

            // 4. 인트로 텍스트 UI 비활성화
            if (introTextUI != null)
            {
                introTextUI.SetActive(false);
            }

            // 5. 플레이어 조작 활성화
            SetPlayerControl(true);
            if (actionUI != null)
            {
                actionUI.SetActive(true);
            }
        }

        public void SetPlayerControl(bool isEnable)
        {
            if (playerInteraction != null)
            {
                playerInteraction.enabled = isEnable;
            }

            if (playerMove != null)
            {
                playerMove.enabled = isEnable;
            }

            if (mouseLook != null)
            {
                mouseLook.enabled = isEnable;
            }

        // 💡 [참고] 만약 플레이어 오브젝트 자체를 완전히 껐다 켜고 싶으시다면 
        // 컴포넌트 제어 대신 아래 한 줄로 대체하셔도 됩니다. 
        // (단, 카메라까지 같이 꺼져서 화면이 안 보일 수 있으니 주의하세요!)
        // playerObject.SetActive(isEnable);
    }
    }
}