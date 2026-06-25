using UnityEngine;
using UnityEngine.UI;

namespace MyFPS
{

    public class PlayerInteract : MonoBehaviour
    {
        [Header("Raycast 설정")]
        [SerializeField] private float interactDistance = 3.0f;
        [SerializeField] private LayerMask doorLayer;
        [SerializeField] private LayerMask gunLayer;

        [Header("카메라 참조")]
        [SerializeField] private Transform playerCamera;

        [Header("UI 설정")]
        // 유니티 에디터에서 상호작용 텍스트 오브젝트(또는 Canvas 패널)를 드래그앤드롭할 변수
        [SerializeField] private GameObject interactionUI;

        void Start()
        {
            if (playerCamera == null)
            {
                playerCamera = Camera.main.transform;
            }
        }

        void Update()
        {
            CheckForTrigger();
        }

        private void CheckForTrigger()
        {
            Ray ray = new Ray(playerCamera.position, playerCamera.forward);
            RaycastHit hit;

            Debug.DrawRay(ray.origin, ray.direction * interactDistance, Color.red);

            // 레이캐스트 발사
            if (Physics.Raycast(ray, out hit, interactDistance, doorLayer))
            {
                // [추가] 레이가 오브젝트에 맞았으므로 UI를 활성화합니다.
                SetInteractionUIActive(true);

                // E키 입력 처리
                if (Input.GetKeyDown(KeyCode.E))
                {
                    // 이전 턴에서 해결한 부모 오브젝트 체크 방식 적용
                    DoorCellOpen door = hit.collider.GetComponentInParent<DoorCellOpen>();
                    if (door != null)
                    {
                        door.OpenDoor();
                    }
                }
            }
            else if (Physics.Raycast(ray, out hit, interactDistance, gunLayer))
            {
                // [추가] 레이가 오브젝트에 맞았으므로 UI를 활성화합니다.
                SetInteractionUIActive(true);

                // E키 입력 처리
                if (Input.GetKeyDown(KeyCode.E))
                {
                    // 이전 턴에서 해결한 부모 오브젝트 체크 방식 적용
                    PickupGun gun = hit.collider.GetComponentInParent<PickupGun>();
                    if (gun != null)
                    {
                        gun.GunPickup();
                    }
                }
            }
            else
            {
                // [추가] 레이가 아무것도 맞추지 못했거나 거리가 멀어졌으므로 UI를 비활성화합니다.
                SetInteractionUIActive(false);
            }
        }

        // UI의 활성화 상태를 안전하게 변경하는 헬퍼 함수
        private void SetInteractionUIActive(bool isActive)
        {
            if (interactionUI != null)
            {
                // 현재 상태와 바꿀 상태가 다를 때만 SetActive를 호출하여 불필요한 연산을 줄입니다.
                if (interactionUI.activeSelf != isActive)
                {
                    interactionUI.SetActive(isActive);
                }
            }
        }
    }
}