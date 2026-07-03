using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace MyFPS
{

    public class AmmoUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI ammoText;
        [SerializeField] private GunShootProjectile gunShoot;

        private void Awake()
        {
            // 만약 인스펙터에서 텍스트를 할당 안 했다면 컴포넌트에서 찾아옴
            if (ammoText == null)
            {
                ammoText = GetComponent<TextMeshProUGUI>();
            }
        }

        private void Update()
        {
            // gunShoot과 ammoText가 모두 정상적으로 연결되어 있을 때만 UI 갱신
            if (ammoText != null && gunShoot != null)
            {
                UpdateAmmo(gunShoot.ammoCount);
            }
        }
        public void UpdateAmmo(int ammo)
        {
            ammoText.text = $"{ammo} / {gunShoot.maxAmmo}";
        }
    }
}