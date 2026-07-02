using UnityEngine;

namespace MyFPS
{
    public class Bullet : MonoBehaviour
    {
        private float speed;
        private int damage;
        private ParticleSystem hitParticlePrefab;

        // 생성 직후 총 스크립트로부터 데이터를 전달받는 초기화 메서드
        public void Setup(float bulletSpeed, int bulletDamage, ParticleSystem particlePrefab, Vector3 direction)
        {
            speed = bulletSpeed;
            damage = bulletDamage;
            hitParticlePrefab = particlePrefab;

            // Rigidbody를 사용해 정해진 방향과 속도로 물리 이동
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = direction * speed;
            }

            // 탄환이 날아가는 방향을 바라보도록 회전 설정
            if (direction != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(direction);
            }

            // 5초 뒤에 자동으로 총알 삭제 (벽에 안 부딪히고 무한히 날아가는 것 방지)
            Destroy(gameObject, 5f);
        }

        private void OnTriggerEnter(Collider other)
        {
            // 1. 적 오브젝트 감지 및 대미지 적용
            EnemyAI enemy = other.GetComponent<EnemyAI>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
                Debug.Log($"<color=cyan>적 '{other.name}'에 {damage}의 대미지를 입혔습니다.</color>");
            }

            // 2. 피격 파티클 프리팹 생성 (Instantiate)
            if (hitParticlePrefab != null)
            {
                // 충돌 지점(가장 가까운 표면)을 대략 계산하거나 현재 탄환 위치에 생성
                Vector3 spawnPos = other.transform.position;

                // 좀 더 정확한 충돌 지점 유추를 위해 접촉점 방향 활용 가능
                ParticleSystem fx = Instantiate(hitParticlePrefab, transform.position, Quaternion.LookRotation(-transform.forward));
                fx.Play();
                Destroy(fx.gameObject, 1.5f); // 이펙트 재생 후 삭제
            }

            // 3. 무언가에 부딪혔으므로 탄환 제거
            Destroy(gameObject);
        }
    }
}