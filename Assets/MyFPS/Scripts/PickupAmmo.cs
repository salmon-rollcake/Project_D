using UnityEngine;

namespace MyFPS
{
    /// <summary>
    /// 총 오브젝트 상호작용 구현
    /// 근접하여 마우스로 감지하면 액션 UI 팝업
    /// </summary>
    public class PickupAmmo : MonoBehaviour
    {
        [SerializeField] GameObject gun;
        [SerializeField] GameObject desk_gun;
        [SerializeField] GameObject arrow;
        [SerializeField] GameObject spotLight;

        private void Awake()
        {
            gun.SetActive(false);
        }

        public void AmmoPickup()
        {
            gun.SetActive(true);
            arrow.SetActive(false);
            spotLight.SetActive(false);
            desk_gun.SetActive(false);
        }
    }
}