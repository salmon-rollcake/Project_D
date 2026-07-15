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
        [SerializeField] private TextMeshProUGUI lockedUI;    // 열쇠가 없을 때 보여줄 UI (예: "열쇠가 필요합니다")
        [SerializeField] private TextMeshProUGUI eyekeyUI;    // 눈알 열쇠 상호작용 시 보여줄 UI

        [Header("상태 변수")]
        public bool getKey = false; // 열쇠 획득 여부
        public bool getEyeKey = false;

        void Start()
        {
            if (playerCamera == null)
            {
                playerCamera = Camera.main.transform;
            }

            // 시작할 때 모든 버튼 UI를 꺼둡니다.
            DeactivateButtonUI();
            if (lockedUI != null) lockedUI.gameObject.SetActive(false);
            if (eyekeyUI != null) eyekeyUI.gameObject.SetActive(false);
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
                        // ★ 수정: 버튼이 잠겨있고 열쇠가 없는 경우
                        if (button.needsKey && !getKey)
                        {
                            // 기본 상호작용 부모 UI(interactionUI)가 lockedUI의 부모라면 활성화(true)되어야 내부 자식인 lockedUI가 보입니다.
                            SetInteractionUIActive(true); 
                            DeactivateButtonUI(); // 일반 문 열기/닫기 텍스트는 끄기
                            
                            if (lockedUI != null && !lockedUI.gameObject.activeSelf) 
                                lockedUI.gameObject.SetActive(true);
                        }
                        else
                        {
                            // 열쇠가 있거나, 열쇠가 필요 없는 버튼인 경우
                            if (lockedUI != null) lockedUI.gameObject.SetActive(false);
                            SetInteractionUIActive(true);
                            UpdateButtonUI(button.IsOn);
                        }
                    }
                    else
                    {
                        SetInteractionUIActive(false);
                        DeactivateButtonUI();
                        if (lockedUI != null) lockedUI.gameObject.SetActive(false);
                    }

                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        if (button != null && button.needsKey && !getKey)
                        {
                            Debug.LogWarning("[PlayerInteract] 열쇠가 없어 이 버튼을 작동할 수 없습니다!");
                            return; 
                        }

                        DoorCellOpen door = hit.collider.GetComponentInParent<DoorCellOpen>();
                        if (door != null)
                        {
                            door.OpenDoor();
                        }

                        if (button != null)
                        {
                            button.Interact();
                            UpdateButtonUI(button.IsOn);
                        }
                    }
                }
                // 2. 총 레이어 감지
                else if (((1 << hitLayer) & gunLayer) != 0)
                {
                    SetInteractionUIActive(false);
                    DeactivateButtonUI();
                    if (lockedUI != null) lockedUI.gameObject.SetActive(false);
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
                    if (lockedUI != null) lockedUI.gameObject.SetActive(false);
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
                SetInteractionUIActive(false);
                ActiveGunUI(false);
                ActiveAmmoUI(false);
                DeactivateButtonUI();
                if (lockedUI != null) lockedUI.gameObject.SetActive(false); 
            }
        }

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