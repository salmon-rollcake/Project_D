using UnityEngine;

namespace MyFPS
{
    public class KeyItem : MonoBehaviour
    {
        [Header("사운드 설정 (선택사항)")]
        [SerializeField] private AudioClip pickupSound;
        [SerializeField] private float soundVolume = 1.0f;

        private void OnTriggerEnter(Collider other)
        {
            // 1. 충돌한 오브젝트(플레이어) 또는 그 자식(카메라 등)에서 PlayerInteract를 찾습니다.
            PlayerInteract playerInteract = other.GetComponentInChildren<PlayerInteract>();

            // 2. 만약 자식에게서 못 찾았다면, 부모나 자기 자신에게 있는지 한 번 더 포괄적으로 확인
            if (playerInteract == null)
            {
                playerInteract = other.GetComponentInParent<PlayerInteract>();
            }

            if (playerInteract != null)
            {
                // 열쇠 획득 상태를 true로 변경
                playerInteract.getKey = true;
                Debug.Log("[KeyItem] 카메라에 부착된 PlayerInteract를 찾아 열쇠를 획득 처리했습니다!");

                if (pickupSound != null)
                {
                    AudioSource.PlayClipAtPoint(pickupSound, transform.position, soundVolume);
                }

                Destroy(gameObject);
            }
        }
    }
}
