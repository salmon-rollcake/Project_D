using UnityEngine;

namespace MyFPS
{
    public class EyeKey : MonoBehaviour
    {
        public enum EyeKeyType
        {
            Left,
            Right
        }

        [Header("열쇠 타입 설정")]
        [SerializeField] private EyeKeyType keyType;

        public void Pickup(PlayerInteract player)
        {
            if (keyType == EyeKeyType.Left)
            {
                player.getEyeKey_L = true;
                Debug.Log("[EyeKey] 왼쪽 눈알 열쇠 획득 완료");
            }
            else if (keyType == EyeKeyType.Right)
            {
                player.getEyeKey_R = true;
                Debug.Log("[EyeKey] 오른쪽 눈알 열쇠 획득 완료");
            }

            // 플레이어 인벤토리 상태 UI 최신화 지시
            player.UpdateInventoryUI();

            // 상호작용 후 씬에서 즉시 오브젝트 제거
            Destroy(gameObject);
        }
    }
}
