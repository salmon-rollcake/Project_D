using UnityEngine;

namespace MyFPS
{
    /// <summary>
    /// 총 오브젝트 상호작용 구현
    /// 근접하여 마우스로 감지하면 액션 UI 팝업
    /// </summary>
    public class PickupGun : MonoBehaviour
    {
        [SerializeField] GameObject gun;
        [SerializeField] GameObject desk_gun;
        [SerializeField] GameObject arrow;
        [SerializeField] GameObject spotLight;

        [SerializeField] GameObject ammo;

        GunShootProjectile gunShoot;

        [SerializeField] private GameObject ammoUI;                     // 장탄 수 UI 오브젝트

        bool isGunPickedUp = false;
        bool isAmmoPickedUp = false;


        private void Awake()
        {
            if (gun != null)
            {
                gun.SetActive(false);
                gunShoot = gun.GetComponent<GunShootProjectile>();

                if (gunShoot == null)
                {
                    Debug.LogWarning("PickupGun: GunShootProjectile is missing on gun.");
                }
            }
            else
            {
                Debug.LogWarning("PickupGun: gun reference is missing.");
            }

            if (ammoUI != null)
            {
                ammoUI.SetActive(false);
            }
            else
            {
                Debug.LogWarning("PickupGun: ammoUI reference is missing.");
            }
        }

        void Update()
        {
            if (isGunPickedUp && isAmmoPickedUp)
            {
                AllPickup();
            }
        }

        void AllPickup()
        {
            if (arrow != null) arrow.SetActive(false);
            if (spotLight != null) spotLight.SetActive(false);
        }

        public void GunPickup()
        {
            isGunPickedUp = true;
            if (gun != null) gun.SetActive(true);
            if (desk_gun != null) desk_gun.SetActive(false);
            if (ammoUI != null) ammoUI.SetActive(true);
        }

        public void AmmoPickup()
        {
            isAmmoPickedUp = true;
            if (ammo != null) ammo.SetActive(false);

            if (gunShoot == null)
            {
                Debug.LogWarning("PickupGun: GunShootProjectile is missing, ammo cannot be added.");
                return;
            }

            // 탄약을 주우면 7발 충전 (최대 탄수를 넘지 않도록 제한하려면 Mathf.Min 사용 가능)
            gunShoot.ammoCount = Mathf.Min(gunShoot.ammoCount + 7, gunShoot.maxAmmo);

            // ★ 중요: 탄약 주웠을 때 애니메이터의 Ammo 파라미터도 같이 갱신!
            gunShoot.UpdateAnimatorAmmo();
        }
    }
}
