using UnityEngine;
using System.Collections;

namespace MyFPS
{
    public class ConditionDoorTrigger : MonoBehaviour
    {
        [Header("이벤트 설정")]
        [SerializeField] private float eventDuration = 3.0f;     // 조작이 불가능한 총 시간
        [SerializeField] private float lookSpeed = 2.0f;         // 카메라가 목표를 향해 돌아가는 속도

        [Header("제어할 오브젝트")]
        [SerializeField] private GameObject playerObject;        // 플레이어 오브젝트
        [SerializeField] private Animator doorAnimator;          // 문 애니메이터
        [SerializeField] private string doorOpenTrigger = "DoorTrg"; // 문 애니메이터 Trigger 파라미터 이름
        [SerializeField] private Transform lookTarget;           // 강제로 바라볼 대상

        private PlayerInteract playerInteraction;
        private PlayerMove playerMove;
        private MouseLook mouseLook;
        private bool hasTriggered = false;

        void Start()
        {
            if (playerObject != null)
            {
                playerInteraction = playerObject.GetComponent<PlayerInteract>();
                playerMove = playerObject.GetComponent<PlayerMove>();
                mouseLook = playerObject.GetComponent<MouseLook>();
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (hasTriggered || !other.CompareTag("Player")) return;

            // BGMManager를 참조하여 모든 적이 제거되었는지 사전 확인
            if (BGMManager.Instance != null && !BGMManager.Instance.AllEnemiesDefeated)
            {
                Debug.Log("적이 아직 모두 제거되지 않아 문이 열리지 않습니다.");
                return;
            }

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

            // 3. 강제로 목표 방향 바라보기
            if (lookTarget != null && mouseLook != null && mouseLook.cameraTrans != null)
            {
                Vector3 dirToTarget = (lookTarget.position - mouseLook.cameraTrans.position).normalized;
                Quaternion targetRot = Quaternion.LookRotation(dirToTarget);

                float targetYaw = targetRot.eulerAngles.y;
                float targetPitch = targetRot.eulerAngles.x;
                if (targetPitch > 180f) targetPitch -= 360f;

                float startYaw = playerObject.transform.eulerAngles.y;
                float startPitch = mouseLook.cameraTrans.localEulerAngles.x;
                if (startPitch > 180f) startPitch -= 360f;

                float t = 0f;
                float elapsedTime = 0f;

                while (elapsedTime < eventDuration)
                {
                    elapsedTime += Time.deltaTime;

                    if (t < 1f)
                    {
                        t += Time.deltaTime * lookSpeed;
                        float currentYaw = Mathf.LerpAngle(startYaw, targetYaw, t);
                        float currentPitch = Mathf.Lerp(startPitch, targetPitch, t);

                        playerObject.transform.rotation = Quaternion.Euler(0f, currentYaw, 0f);
                        mouseLook.SetTargetPitch(currentPitch);
                    }
                    yield return null;
                }
            }
            else
            {
                yield return new WaitForSeconds(eventDuration);
            }

            // 4. 조작 복구
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
