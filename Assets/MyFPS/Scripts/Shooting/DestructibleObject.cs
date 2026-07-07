using UnityEngine;

namespace MyFPS
{
    /// <summary>
    /// 파괴 가능한 오브젝트 (항아리 등)
    /// 일정 횟수 피격 시 금이 간 상태로 전환, 추가 피격 시 파괴되어 조각 생성
    /// </summary>
    public class DestructibleObject : MonoBehaviour, IDamageable
    {
        [Header("오브젝트 상태")]
        [SerializeField] private GameObject normalObject;           // 정상 상태 메시
        [SerializeField] private GameObject crackedObject;          // 금이 간 상태 메시

        [Header("파괴 조각")]
        [SerializeField] private GameObject destroyedPiecesPrefab;  // 파괴 조각 프리팹

        [Header("적중 임계값")]
        [SerializeField] private int crackThreshold = 3;            // 금이 가기까지 필요한 적중 횟수
        [SerializeField] private int destroyThreshold = 5;          // 파괴까지 필요한 총 적중 횟수

        [Header("파괴 효과")]
        [SerializeField] private float explosionForce = 200f;       // 조각에 가해지는 힘
        [SerializeField] private float explosionRadius = 2f;        // 폭발 반경
        [SerializeField] private float destroyDelay = 5f;           // 조각 자동 제거 시간 (초)

        private int hitCount = 0;       // 현재 적중 횟수
        private bool isCracked = false; // 금이 간 상태 여부
        private bool isDestroyed = false;

        private void Start()
        {
            // 초기 상태: 정상 오브젝트만 활성화
            if (normalObject != null) normalObject.SetActive(true);
            if (crackedObject != null) crackedObject.SetActive(false);
        }

        /// <summary>
        /// IDamageable 구현 - 외부에서 대미지를 받을 때 호출
        /// </summary>
        public void TakeDamage(int amount)
        {
            if (isDestroyed) return;

            hitCount++;
            Debug.Log($"<color=orange>{gameObject.name} 적중! ({hitCount}/{destroyThreshold})</color>");

            // 파괴 임계값 도달
            if (hitCount >= destroyThreshold)
            {
                Destroy();
            }
            // 금이 간 임계값 도달
            else if (!isCracked && hitCount >= crackThreshold)
            {
                Crack();
            }
        }

        /// <summary>
        /// 금이 간 상태로 전환
        /// </summary>
        private void Crack()
        {
            isCracked = true;

            if (normalObject != null) normalObject.SetActive(false);
            if (crackedObject != null) crackedObject.SetActive(true);

            Debug.Log($"<color=yellow>{gameObject.name}에 금이 갔습니다!</color>");
        }

        /// <summary>
        /// 오브젝트 파괴 - 조각 프리팹 생성 및 물리 효과 적용
        /// </summary>
        private void Destroy()
        {
            isDestroyed = true;

            // 조각 프리팹 생성
            if (destroyedPiecesPrefab != null)
            {
                GameObject pieces = Instantiate(destroyedPiecesPrefab, transform.position, transform.rotation);

                // 각 조각에 폭발 물리력 적용
                Rigidbody[] pieceRigidbodies = pieces.GetComponentsInChildren<Rigidbody>();
                foreach (Rigidbody rb in pieceRigidbodies)
                {
                    rb.AddExplosionForce(explosionForce, transform.position, explosionRadius);
                }

                // 일정 시간 후 조각 자동 제거
                UnityEngine.Object.Destroy(pieces, destroyDelay);
            }

            Debug.Log($"<color=red>{gameObject.name}이(가) 파괴되었습니다!</color>");

            // 원본 오브젝트 제거
            UnityEngine.Object.Destroy(gameObject);
        }
    }
}
