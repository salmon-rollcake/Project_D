using UnityEngine;
using System.Collections;

namespace MyFPS
{
    public class EventDoorTrigger : MonoBehaviour
    {
        [Header("이벤트 설정")]
        [SerializeField] private float eventDuration = 3.0f;     // 조작이 불가능한 총 시간
        [SerializeField] private float lookSpeed = 2.0f;         // 카메라가 목표를 향해 돌아가는 속도

        [Header("제어할 오브젝트")]
        [SerializeField] private GameObject playerObject;        // 플레이어 오브젝트
        [SerializeField] private Animator doorAnimator;          // 문 애니메이터
        [SerializeField] private string doorOpenTrigger = "DoorTrg"; // 문 애니메이터 Trigger 파라미터 이름
        [SerializeField] private GameObject enemyObject;         // 활성화할 적 오브젝트
        [SerializeField] private Transform lookTarget;           // 강제로 바라볼 대상 (문 중앙이나 적 등)

        private PlayerInteract playerInteraction;
        private PlayerMove playerMove;
        private MouseLook mouseLook;

        private bool hasTriggered = false;

        void Start()
        {
            if (enemyObject != null)
            {
                enemyObject.SetActive(false); // 시작할 때 적은 비활성화 상태여야 함
            }

            if (playerObject != null)
            {
                playerInteraction = playerObject.GetComponent<PlayerInteract>();
                playerMove = playerObject.GetComponent<PlayerMove>();
                mouseLook = playerObject.GetComponent<MouseLook>();
            }
            else
            {
                Debug.LogWarning("EventDoorTrigger에 Player Object가 할당되지 않았습니다!");
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (hasTriggered || !other.CompareTag("Player")) return;

            hasTriggered = true;
            StartCoroutine(EventSequence());
        }

        IEnumerator EventSequence()
        {
            // 1. 조작 불가
            SetPlayerControl(false);

            // 2. 문 열기
            if (doorAnimator != null)
            {
                doorAnimator.SetTrigger(doorOpenTrigger);
            }

            // 3. 적 활성화 및 전투 BGM 재생
            if (enemyObject != null)
            {
                enemyObject.SetActive(true);
                if (BGMManager.Instance != null)
                {
                    BGMManager.Instance.PlayCombatBGM();
                }
            }

            // 4. 강제로 문 쪽 바라보기 (부드럽게 회전)
            if (lookTarget != null && mouseLook != null && mouseLook.cameraTrans != null)
            {
                // 목표 방향 계산
                Vector3 dirToTarget = (lookTarget.position - mouseLook.cameraTrans.position).normalized;
                Quaternion targetRot = Quaternion.LookRotation(dirToTarget);

                // 목표 Yaw (플레이어 몸체 좌우 회전)와 Pitch (카메라 상하 회전) 분리
                float targetYaw = targetRot.eulerAngles.y;
                float targetPitch = targetRot.eulerAngles.x;
                if (targetPitch > 180f) targetPitch -= 360f; // -180 ~ 180 범위로 변환

                // 시작 시점의 회전값
                float startYaw = playerObject.transform.eulerAngles.y;
                float startPitch = mouseLook.cameraTrans.localEulerAngles.x;
                if (startPitch > 180f) startPitch -= 360f;

                float t = 0f;
                // eventDuration 동안 회전과 대기를 동시에 처리하기 위한 시간 체크
                float elapsedTime = 0f;

                while (elapsedTime < eventDuration)
                {
                    elapsedTime += Time.deltaTime;

                    if (t < 1f)
                    {
                        t += Time.deltaTime * lookSpeed;
                        // 보간(Lerp)을 사용하여 부드럽게 회전
                        float currentYaw = Mathf.LerpAngle(startYaw, targetYaw, t);
                        float currentPitch = Mathf.Lerp(startPitch, targetPitch, t);

                        // 플레이어 좌우 회전 적용
                        playerObject.transform.rotation = Quaternion.Euler(0f, currentYaw, 0f);
                        // 카메라 상하 회전 적용 (MouseLook의 내부 변수까지 함께 동기화)
                        mouseLook.SetTargetPitch(currentPitch);
                    }

                    yield return null; // 다음 프레임까지 대기
                }
            }
            else
            {
                // lookTarget이 없는 경우 설정된 시간만큼만 단순 대기
                yield return new WaitForSeconds(eventDuration);
            }

            // 5. 조작 복구
            SetPlayerControl(true);
        }

        private void SetPlayerControl(bool isEnable)
        {
            if (playerInteraction != null) playerInteraction.enabled = isEnable;
            if (playerMove != null) playerMove.enabled = isEnable;
            if (mouseLook != null) mouseLook.enabled = isEnable;
        }
    }
}
