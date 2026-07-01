using Unity.VisualScripting;
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

        bool isGunPickedUp = false;
        bool isAmmoPickedUp = false;


        private void Awake()
        {
            gun.SetActive(false);
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
        }

        public void AmmoPickup()
        {
            isAmmoPickedUp = true;
            ammo.SetActive(false);
        }
    }
}