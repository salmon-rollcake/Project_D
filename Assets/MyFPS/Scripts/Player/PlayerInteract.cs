using System;
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
        [SerializeField] GameObject gunInteractUI;

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

            // 1. 문 레이어 감지
            if (Physics.Raycast(ray, out hit, interactDistance, doorLayer))
            {
                SetInteractionUIActive(true);
                ActiveGunUI(false); // 다른 UI는 꺼줌

                if (Input.GetKeyDown(KeyCode.E))
                {
                    DoorCellOpen door = hit.collider.GetComponentInParent<DoorCellOpen>();
                    if (door != null)
                    {
                        door.OpenDoor();
                    }
                }
            }
            // 2. 총 레이어 감지 (문이 아닐 때)
            else if (Physics.Raycast(ray, out hit, interactDistance, gunLayer))
            {
                SetInteractionUIActive(false);
                ActiveGunUI(true); // 총 줍기 UI 활성화

                if (Input.GetKeyDown(KeyCode.E))
                {
                    PickupGun gun = hit.collider.GetComponentInParent<PickupGun>();
                    if (gun != null)
                    {
                        gun.GunPickup();
                        // 줍자마자 바로 UI를 끄기 (오브젝트가 파괴되면서 다음 프레임에 어차피 꺼지지만, 명시적으로 꺼줍니다)
                        ActiveGunUI(false);
                    }
                }
            }
            // 3. 아무것도 감지되지 않음
            else
            {
                SetInteractionUIActive(false);
                ActiveGunUI(false);
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

        private void ActiveGunUI(bool isActive)
        {
            if (gunInteractUI != null)
            {
                // 현재 상태와 바꿀 상태가 다를 때만 SetActive를 호출하여 불필요한 연산을 줄입니다.
                if (gunInteractUI.activeSelf != isActive)
                {
                    gunInteractUI.SetActive(isActive);
                }
            }
        }
    }
}