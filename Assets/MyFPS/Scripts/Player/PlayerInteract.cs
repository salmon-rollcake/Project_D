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
        [SerializeField] private LayerMask eyeKeyLayer; 

        [Header("카메라 참조")]
        [SerializeField] private Transform playerCamera;

        [Header("UI 설정")]
        [SerializeField] private GameObject interactionUI;
        [SerializeField] private GameObject gunInteractUI;
        [SerializeField] private GameObject ammoInteractUI;

        [Header("버튼 트리거 UI 설정 (TMPro)")]
        [SerializeField] private TextMeshProUGUI buttonOffUI; 
        [SerializeField] private TextMeshProUGUI buttonOnUI;  
        [SerializeField] private TextMeshProUGUI lockedUI;    
        [SerializeField] private TextMeshProUGUI eyekeyUI;    // 조준했을 때 뜨는 상호작용 UI (예: "눈알 열쇠 획득 [E]")

        [Header("지속 인벤토리 UI 설정")]
        // ★ 추가: 플레이어가 획득 상태를 계속 볼 수 있는 UI 요소 (활성화/비활성화로 표시)
        [SerializeField] private GameObject eyeKeyL_InventoryUI; // 왼쪽 눈알 인벤토리 UI 오브젝트 (아이콘 혹은 텍스트)
        [SerializeField] private GameObject eyeKeyR_InventoryUI; // 오른쪽 눈알 인벤토리 UI 오브젝트 (아이콘 혹은 텍스트)

        [Header("상태 변수")]
        public bool getKey = false; 
        public bool getEyeKey_L = false;
        public bool getEyeKey_R = false;

        void Start()
        {
            if (playerCamera == null)
            {
                playerCamera = Camera.main.transform;
            }

            DeactivateButtonUI();
            if (lockedUI != null) lockedUI.gameObject.SetActive(false);
            if (eyekeyUI != null) eyekeyUI.gameObject.SetActive(false);

            // 게임 시작 시 인벤토리 UI는 꺼둡니다.
            UpdateInventoryUI();
        }

        void Update()
        {
            CheckForTrigger();
        }

        // ★ 추가: 인벤토리 상태에 따라 UI를 지속적으로 업데이트하는 함수
        public void UpdateInventoryUI()
        {
            if (eyeKeyL_InventoryUI != null)
            {
                eyeKeyL_InventoryUI.SetActive(getEyeKey_L);
            }
            if (eyeKeyR_InventoryUI != null)
            {
                eyeKeyR_InventoryUI.SetActive(getEyeKey_R);
            }
        }

        private void CheckForTrigger()
        {
            Ray ray = new Ray(playerCamera.position, playerCamera.forward);
            RaycastHit hit;

            Debug.DrawRay(ray.origin, ray.direction * interactDistance, Color.red);

            int combinedMask = doorLayer | gunLayer | ammoLayer | eyeKeyLayer;

            if (Physics.Raycast(ray, out hit, interactDistance, combinedMask))
            {
                int hitLayer = hit.collider.gameObject.layer;

                // 1. 문 레이어 감지
                if (((1 << hitLayer) & doorLayer) != 0)
                {
                    ActiveGunUI(false);
                    ActiveAmmoUI(false);
                    ActiveEyeKeyUI(false);

                    InteractTriggerButton button = hit.collider.GetComponent<InteractTriggerButton>();
                    if (button != null)
                    {
                        if (button.needsKey && !getKey)
                        {
                            SetInteractionUIActive(true); 
                            DeactivateButtonUI(); 
                            
                            if (lockedUI != null && !lockedUI.gameObject.activeSelf) 
                                lockedUI.gameObject.SetActive(true);
                        }
                        else
                        {
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
                    ActiveEyeKeyUI(false);
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
                    ActiveEyeKeyUI(false);
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
                // 4. 눈알 열쇠 레이어 감지
                else if (((1 << hitLayer) & eyeKeyLayer) != 0)
                {
                    SetInteractionUIActive(false);
                    DeactivateButtonUI();
                    if (lockedUI != null) lockedUI.gameObject.SetActive(false);
                    ActiveGunUI(false);
                    ActiveAmmoUI(false);
                    
                    ActiveEyeKeyUI(true); 

                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        EyeKey eyeKeyObj = hit.collider.GetComponent<EyeKey>();
                        if (eyeKeyObj != null)
                        {
                            eyeKeyObj.Pickup(this); 
                            ActiveEyeKeyUI(false);
                        }
                    }
                }
            }
            else
            {
                SetInteractionUIActive(false);
                ActiveGunUI(false);
                ActiveAmmoUI(false);
                ActiveEyeKeyUI(false); 
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

        private void ActiveEyeKeyUI(bool isActive)
        {
            if (eyekeyUI != null && eyekeyUI.gameObject.activeSelf != isActive)
            {
                eyekeyUI.gameObject.SetActive(isActive);
            }
        }
    }
}