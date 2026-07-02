using UnityEngine;
using System.Collections;

namespace MyFPS
{
    public class GuideTrigger : MonoBehaviour
    {
        [Header("연출 설정")]
        [SerializeField] private GameObject guideTextUI;       // 보여줄 텍스트 UI
        [SerializeField] private float displayTime = 3.0f;     // 텍스트를 보여줄 시간

        [Header("더빙 오디오 설정")]
        [SerializeField] private AudioSource audioSource;       // 오디오 재생을 위한 소스
        [SerializeField] private AudioClip guideVoice;          // 대사 음원
        [SerializeField] private GameObject arrowObject;       // 연출 후 활성화할 화살표 오브젝트

        [Header("제어할 플레이어 참조")]
        [SerializeField] private GameObject playerObject;      // 조작을 멈출 플레이어 오브젝트

        private PlayerInteract playerInteraction;
        private PlayerMove playerMove;
        private MouseLook mouseLook;

        // 한 번만 실행되도록 체크하는 변수
        private bool hasTriggered = false;

        void Start()
        {
            // 화살표는 시작할 때 꺼져있어야 하므로 혹시 켜져있다면 꺼줍니다.
            if (arrowObject != null)
            {
                arrowObject.SetActive(false);
            }

            if (guideTextUI != null)
            {
                guideTextUI.SetActive(false);
            }

            // 플레이어 안의 조작 스크립트들을 가져옵니다.
            if (playerObject != null)
            {
                playerInteraction = playerObject.GetComponent<PlayerInteract>();
                playerMove = playerObject.GetComponent<PlayerMove>();
                mouseLook = playerObject.GetComponent<MouseLook>();
            }
            else
            {
                Debug.LogWarning("GuideTrigger에 Player Object가 할당되지 않았습니다!");
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            // 이미 한 번 실행되었거나, 트리거에 닿은 대상이 플레이어가 아니면 무시합니다.
            // (플레이어 오브젝트에 "Player" 태그가 설정되어 있어야 합니다!)
            if (hasTriggered || !other.CompareTag("Player")) return;

            hasTriggered = true; // 중복 실행 방지
            StartCoroutine(GuideSequence());
        }

        IEnumerator GuideSequence()
        {
            // 1. 플레이어 조작 정지
            SetPlayerControl(false);

            // 2. 텍스트 UI 띄우기
            if (guideTextUI != null) guideTextUI.SetActive(true);

            // 3. 음원 재생 및 음원 길이에 따른 대기 (음원이 없을 경우 displayTime만큼 대기)
            if (audioSource != null && guideVoice != null)
            {
                audioSource.clip = guideVoice;
                audioSource.Play();
                yield return new WaitForSeconds(guideVoice.length);
            }
            else
            {
                yield return new WaitForSeconds(displayTime);
            }

            // 4. 텍스트 UI 숨기기
            if (guideTextUI != null) guideTextUI.SetActive(false);

            // 5. 화살표 활성화!
            if (arrowObject != null) arrowObject.SetActive(true);

            // 6. 플레이어 조작 재개
            SetPlayerControl(true);
        }

        // 플레이어 조작 스크립트를 켜고 끄는 헬퍼 함수
        private void SetPlayerControl(bool isEnable)
        {
            if (playerInteraction != null) playerInteraction.enabled = isEnable;
            if (playerMove != null) playerMove.enabled = isEnable;
            if (mouseLook != null) mouseLook.enabled = isEnable;
        }
    }
}
