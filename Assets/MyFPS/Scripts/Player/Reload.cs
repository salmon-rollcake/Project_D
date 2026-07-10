using UnityEngine;

namespace MyFPS
{
    /// <summary>
    /// PlayScene02 진입 시 1회성으로 플레이어의 탄환을 초기화하고 UI를 갱신하는 클래스
    /// 빈 오브젝트(Empty Object)에 부착하여 사용합니다.
    /// </summary>
    public class Reload : MonoBehaviour
    {
        [Header("탄약 초기화 설정")]
        [SerializeField] private int resetAmmoCount = 7;

        void Start()
        {
            // 1. PlayScene01에서 획득하여 사용한 탄환(ammoCount)을 maxAmmo(7)로 초기화
            GunShootProjectile gunShoot = FindFirstObjectByType<GunShootProjectile>();
            if (gunShoot != null)
            {
                gunShoot.ammoCount = resetAmmoCount;
                gunShoot.UpdateAnimatorAmmo();
                Debug.Log($"[Reload] 플레이어의 탄환이 {resetAmmoCount}발로 초기화되었습니다.");
            }
            else
            {
                // GunShootProjectile 대신 GunShoot 스크립트를 사용할 경우를 위해 체크
                GunShoot gunShootRay = FindFirstObjectByType<GunShoot>();
                if (gunShootRay != null)
                {
                    gunShootRay.ammoCount = resetAmmoCount;
                    gunShootRay.UpdateAnimatorAmmo();
                    Debug.Log($"[Reload] 플레이어(레이캐스트)의 탄환이 {resetAmmoCount}발로 초기화되었습니다.");
                }
            }

            // 2. AmmoUI 연동 및 즉시 갱신
            AmmoUI ammoUI = FindFirstObjectByType<AmmoUI>();
            if (ammoUI != null && gunShoot != null)
            {
                ammoUI.UpdateAmmo(gunShoot.ammoCount);
            }
        }
    }
}