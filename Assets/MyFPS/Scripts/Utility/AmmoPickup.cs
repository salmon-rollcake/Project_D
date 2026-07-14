using UnityEngine;

namespace MyFPS
{
    public class AmmoPickup : MonoBehaviour
    {
        [Header("탄약 획득 설정")]
        [SerializeField] private int ammoAmount = 7; // 이 탄약 상자를 먹었을 때 추가할 탄약 수

        [Header("사운드 설정 (선택사항)")]
        [SerializeField] private AudioClip pickupSound;
        [SerializeField] private float soundVolume = 1.0f;

        private void OnTriggerEnter(Collider other)
        {
            // 플레이어 태그나 PlayerInteract 컴포넌트가 있는지 확인하여 플레이어인지 판별합니다.
            PlayerInteract player = other.GetComponentInChildren<PlayerInteract>();
            if (player == null)
            {
                player = other.GetComponentInParent<PlayerInteract>();
            }

            // 플레이어가 충돌한 것이 맞다면 탄약 충전 프로세스 진행
            if (player != null)
            {
                // 1. 씬 내의 GunShootProjectile 무기 스크립트를 찾습니다.
                GunShootProjectile gunShoot = FindFirstObjectByType<GunShootProjectile>();
                
                if (gunShoot != null)
                {
                    // 탄환 수 충전 및 애니메이터 갱신
                    gunShoot.ammoCount += ammoAmount;
                    gunShoot.UpdateAnimatorAmmo();

                    // 2. AmmoUI 연동 및 즉시 갱신
                    AmmoUI ammoUI = FindFirstObjectByType<AmmoUI>();
                    if (ammoUI != null)
                    {
                        ammoUI.UpdateAmmo(gunShoot.ammoCount);
                    }

                    Debug.Log($"[AmmoPickupItem] 탄약 {ammoAmount}발을 획득했습니다! (현재 탄환: {gunShoot.ammoCount}발)");

                    // 3. 사운드 재생 및 오브젝트 제거
                    if (pickupSound != null)
                    {
                        AudioSource.PlayClipAtPoint(pickupSound, transform.position, soundVolume);
                    }

                    Destroy(gameObject);
                }
                else
                {
                    Debug.LogWarning("[AmmoPickupItem] 씬에서 GunShootProjectile 스크립트를 찾을 수 없습니다.");
                }
            }
        }
    }
}