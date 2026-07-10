using System;
using UnityEngine;
using TMPro;

namespace MyFPS
{
    public class PlayerInteract : MonoBehaviour
    {
        [Header("Raycast 설정")]
        [SerializeField] private float interactDistance = 3.0f;
        [SerializeField] private LayerMask doorLayer;
        [SerializeField] private LayerMask gunLayer;
        [SerializeField] private LayerMask ammoLayer;

        [Header("카메라 참조")]
        [SerializeField] private Transform playerCamera;

        [Header("UI 설정")]
        [SerializeField] private GameObject interactionUI;
        [SerializeField] private GameObject gunInteractUI;
        [SerializeField] private GameObject ammoInteractUI;

        [Header("버튼 트리거 UI 설정 (TMPro)")]
        [SerializeField] private TextMeshProUGUI buttonOffUI; // Off 상태(기본 상태)일 때 보여줄 UI (예: "문 열기 [E]")
        [SerializeField] private TextMeshProUGUI buttonOnUI;  // On 상태(활성화 상태)일 때 보여줄 UI (예: "문 닫기 [E]")

        void Start()
        {
            if (playerCamera == null)
            {
                playerCamera = Camera.main.transform;
            }

            // 시작할 때 모든 버튼 UI를 꺼둡니다.
            DeactivateButtonUI();
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

            // 레이어 마스크 비트 연산으로 단 한 번만 레이캐스트를 발사하도록 최적화
            int combinedMask = doorLayer | gunLayer | ammoLayer;

            if (Physics.Raycast(ray, out hit, interactDistance, combinedMask))
            {
                int hitLayer = hit.collider.gameObject.layer;

                // 1. 문 레이어 감지
                if (((1 << hitLayer) & doorLayer) != 0)
                {
                    ActiveGunUI(false);
                    ActiveAmmoUI(false);

                    InteractTriggerButton button = hit.collider.GetComponent<InteractTriggerButton>();
                    if (button != null)
                    {
                        // 일반 문 UI는 끄고 버튼 상태에 따른 UI 활성화
                        SetInteractionUIActive(true);
                        UpdateButtonUI(button.IsOn);
                    }
                    else
                    {
                        // 일반 문 UI 활성화 및 버튼 UI 비활성화
                        SetInteractionUIActive(false);
                        DeactivateButtonUI();
                    }

                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        DoorCellOpen door = hit.collider.GetComponentInParent<DoorCellOpen>();
                        if (door != null)
                        {
                            door.OpenDoor();
                        }

                        if (button != null)
                        {
                            button.Interact();
                            // 상호작용 후 즉시 상태 갱신
                            UpdateButtonUI(button.IsOn);
                        }
                    }
                }
                // 2. 총 레이어 감지
                else if (((1 << hitLayer) & gunLayer) != 0)
                {
                    SetInteractionUIActive(false);
                    DeactivateButtonUI();
                    ActiveGunUI(true);
                    ActiveAmmoUI(false);

                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        PickupGun gun = hit.collider.GetComponentInParent<PickupGun>();
                        if (gun != null)
                        {
                            gun.GunPickup();
                            ActiveGunUI(false);
                        }
                    }
                }
                // 3. 탄약 레이어 감지
                else if (((1 << hitLayer) & ammoLayer) != 0)
                {
                    SetInteractionUIActive(false);
                    DeactivateButtonUI();
                    ActiveGunUI(false);
                    ActiveAmmoUI(true);

                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        PickupGun ammo = hit.collider.GetComponentInParent<PickupGun>();
                        if (ammo != null)
                        {
                            ammo.AmmoPickup();
                            ActiveAmmoUI(false);
                        }
                    }
                }
            }
            else
            {
                // 아무것도 감지되지 않았을 때 모든 UI 비활성화
                SetInteractionUIActive(false);
                ActiveGunUI(false);
                ActiveAmmoUI(false);
                DeactivateButtonUI();
            }
        }

        // 버튼 상태에 따른 UI 갱신 헬퍼 함수
        private void UpdateButtonUI(bool isOn)
        {
            if (isOn)
            {
                if (buttonOnUI != null) buttonOnUI.gameObject.SetActive(true);
                if (buttonOffUI != null) buttonOffUI.gameObject.SetActive(false);
            }
            else
            {
                if (buttonOnUI != null) buttonOnUI.gameObject.SetActive(false);
                if (buttonOffUI != null) buttonOffUI.gameObject.SetActive(true);
            }
        }

        // 버튼 UI 일괄 비활성화 헬퍼 함수
        private void DeactivateButtonUI()
        {
            if (buttonOnUI != null) buttonOnUI.gameObject.SetActive(false);
            if (buttonOffUI != null) buttonOffUI.gameObject.SetActive(false);
        }

        private void SetInteractionUIActive(bool isActive)
        {
            if (interactionUI != null && interactionUI.activeSelf != isActive)
            {
                interactionUI.SetActive(isActive);
            }
        }

        private void ActiveGunUI(bool isActive)
        {
            if (gunInteractUI != null && gunInteractUI.activeSelf != isActive)
            {
                gunInteractUI.SetActive(isActive);
            }
        }

        private void ActiveAmmoUI(bool isActive)
        {
            if (ammoInteractUI != null && ammoInteractUI.activeSelf != isActive)
            {
                ammoInteractUI.SetActive(isActive);
            }
        }
    }
}