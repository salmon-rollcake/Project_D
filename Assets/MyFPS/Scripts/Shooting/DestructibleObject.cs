using UnityEngine;

namespace MyFPS
{
    /// <summary>
    /// 파괴 가능한 오브젝트 (항아리 등)
    /// 지정된 횟수 피격 시 즉시 파괴되며, 설정에 따라 아이템을 특정 위치와 회전값으로 생성합니다.
    /// </summary>
    public class DestructibleObject : MonoBehaviour, IDamageable
    {
        [Header("오브젝트 메쉬")]
        [SerializeField] private GameObject normalObject;           // 평상시 오브젝트 메시

        [Header("파괴 조건")]
        [SerializeField] private int crackThreshold = 3;            // 파괴되기까지 필요한 적중 횟수

        [Header("아이템 드롭 설정")]
        [Tooltip("체크하면 파괴될 때 아이템을 생성합니다.")]
        [SerializeField] private bool dropItemOnDestroy = true;     // 아이템 드롭 여부
        [SerializeField] private GameObject itemPrefab;             // 생성할 아이템 프리팹
        [SerializeField] private float itemSpawnOffsetY = 0.5f;    // 아이템 생성 Y축 오프셋
        
        [Tooltip("생성되는 아이템의 초기 회전값(각도)을 조정합니다. (예: X를 90으로 주면 눕혀서 생성 가능)")]
        [SerializeField] private Vector3 itemSpawnRotationOffset = Vector3.zero; // 로테이션 오프셋 추가

        private int hitCount = 0;         // 현재 적중 횟수
        private bool isDestroyed = false; // 중복 파괴 방지 플래그

        private void Start()
        {
            // 초기 상태: 정상 오브젝트 활성화
            if (normalObject != null) normalObject.SetActive(true);
        }

        /// <summary>
        /// IDamageable 구현 - 외부에서 대미지를 받을 때 호출
        /// </summary>
        public void TakeDamage(int amount)
        {
            if (isDestroyed) return;

            hitCount++;
            if (hitCount >= crackThreshold)
            {
                DestroyObject();
            }
        }

        /// <summary>
        /// 오브젝트 완전히 파괴 및 아이템 생성
        /// </summary>
        private void DestroyObject()
        {
            isDestroyed = true;

            Debug.Log($"<color=red>{gameObject.name}이(가) 완전히 파괴되었습니다!</color>");

            if (dropItemOnDestroy)
            {
                SpawnFloatingItem();
            }

            Destroy(gameObject);
        }

        /// <summary>
        /// 오브젝트 위치에 아이템 스폰 (회전 오프셋 반영)
        /// </summary>
        private void SpawnFloatingItem()
        {
            if (itemPrefab == null) return;

            // 1. 생성 위치 계산 (Y축 높이 조정)
            Vector3 spawnPosition = transform.position + Vector3.up * itemSpawnOffsetY;

            // 2. 인스펙터에서 입력한 Vector3(각도)를 유니티 회전값(Quaternion)으로 변환
            Quaternion spawnRotation = Quaternion.Euler(itemSpawnRotationOffset);

            // 3. 지정한 위치와 회전값으로 아이템 생성
            Instantiate(itemPrefab, spawnPosition, spawnRotation);
        }
    }
}