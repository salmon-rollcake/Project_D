using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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
            gun.SetActive(false);
            ammoUI.SetActive(false);
            gunShoot = gun.GetComponent<GunShootProjectile>();
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
            arrow.SetActive(false);
            spotLight.SetActive(false);
        }

        public void GunPickup()
        {
            isGunPickedUp = true;
            gun.SetActive(true);
            desk_gun.SetActive(false);
            ammoUI.SetActive(true);
        }

        public void AmmoPickup()
        {
            isAmmoPickedUp = true;
            ammo.SetActive(false);

            // 탄약을 주우면 7발 충전 (최대 탄수를 넘지 않도록 제한하려면 Mathf.Min 사용 가능)
            gunShoot.ammoCount = Mathf.Min(gunShoot.ammoCount + 7, gunShoot.maxAmmo);

            // ★ 중요: 탄약 주웠을 때 애니메이터의 Ammo 파라미터도 같이 갱신!
            gunShoot.UpdateAnimatorAmmo();
        }
    }
}