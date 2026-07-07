using UnityEngine;

namespace MyFPS
{
    public class EnemyAI : MonoBehaviour, IDamageable
    {
        [Header("능력치 설정")]
        [SerializeField] private int health = 20;             // 체력
        [SerializeField] private float moveSpeed = 0.5f;      // 이동 속도
        [SerializeField] private int damage = 5;              // 공격력
        [SerializeField] private float attackRange = 1.5f;    // 공격 범위
        [SerializeField] private float attackInterval = 2.0f; // 공격 간격 (초)

        [Header("컴포넌트 및 참조")]
        [SerializeField] private Animator animator;
        [SerializeField] private string stateParamName = "State"; // 애니메이터 int 파라미터 이름

        private Transform playerTarget;
        private PlayerHealth playerHealth;  // 플레이어 체력 스크립트 참조
        private float attackTimer = 0f;
        private bool isDead = false;
        public bool IsDead => isDead;

        [SerializeField] GameObject player; // 플레이어 오브젝트를 에디터에서 연결
        bool playerFainted; 

        int playerHP;

        void Start()
        {
            if (animator == null)
            {
                animator = GetComponent<Animator>();
            }

            // 씬에서 Player 태그를 가진 오브젝트를 찾습니다.
            player = GameObject.FindGameObjectWithTag("Player");
            playerFainted = player.GetComponent<PlayerHealth>().isFainted;

            if (player != null)
            {
                playerTarget = player.transform;
                playerHealth = player.GetComponent<PlayerHealth>();

                playerHP = player.GetComponent<PlayerHealth>().currentHealth;

                if (playerHealth == null)
                {
                    Debug.LogWarning("EnemyAI: Player 오브젝트에 PlayerHealth 스크립트가 없습니다.");
                }
            }
            else
            {
                Debug.LogWarning("EnemyAI: Player 태그를 가진 오브젝트를 찾을 수 없습니다.");
            }

            // 초기 상태 설정 (대기)
            SetState(0);
        }

        private bool isAttacking = false;

        void Update()
        {
            if (isDead || playerTarget == null) return;

            // 공격 중일 때는 이동이나 다른 판단을 잠시 멈춤
            if (isAttacking) return;

            if (playerFainted == true)
            {
                // 플레이어가 기절 상태이면 공격하지 않음
                return;
            }
            else
            {
                // 플레이어와의 거리 계산
                float distanceToPlayer = Vector3.Distance(transform.position, playerTarget.position);

                if (distanceToPlayer > attackRange)
                {
                    // 공격 범위 밖이면 이동
                    MoveTowardsPlayer();
                }
                else
                {
                    // 공격 범위 안이면 공격 시도
                    AttemptAttack();
                }
            }
        }

        private void MoveTowardsPlayer()
        {
            // 이동 상태 애니메이션 (1)
            SetState(1);

            // 플레이어 방향 바라보기 (y축 회전만 적용하여 기울어짐 방지)
            Vector3 direction = (playerTarget.position - transform.position).normalized;
            direction.y = 0; // 상하 회전 방지
            
            if (direction != Vector3.zero)
            {
                Quaternion lookRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
            }

            // 플레이어 방향으로 이동
            transform.position += direction * moveSpeed * Time.deltaTime;

            // 이동 중일 때는 공격 타이머를 유지 (접근하자마자 바로 때릴 수 있게)
            if (attackTimer < attackInterval)
            {
                attackTimer += Time.deltaTime;
            }
        }

        private void AttemptAttack()
        {
            // 공격 시에도 플레이어를 바라보게 합니다.
            Vector3 direction = (playerTarget.position - transform.position).normalized;
            direction.y = 0;
            if (direction != Vector3.zero)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * 5f);
            }

            attackTimer += Time.deltaTime;

            // 공격 쿨타임이 찼을 경우
            if (attackTimer >= attackInterval)
            {
                StartCoroutine(AttackRoutine());
            }
            else
            {
                // 쿨타임이 덜 찼으면 대기 상태 애니메이션(0) 재생
                SetState(0);
            }
        }

        private System.Collections.IEnumerator AttackRoutine()
        {
            isAttacking = true;
            attackTimer = 0f;

            // 1. 공격 애니메이션 시작
            SetState(2);

            // 2. 모션에서 실제로 주먹/무기를 휘두르는 '타격 시점'까지 잠깐 대기 (예: 0.5초)
            // (이 수치를 조절해서 모션과 데미지가 들어가는 타이밍을 맞출 수 있습니다)
            yield return new WaitForSeconds(0.5f);

            // 3. 데미지 적용
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }
            else
            {
                Debug.Log($"<color=red>적이 플레이어에게 {damage}의 데미지를 입혔습니다! (PlayerHealth 없음)</color>");
            }

            // 4. 애니메이션이 끝날 때까지 남은 시간 대기 (예: 1초 더 대기)
            // (애니메이션 길이에 따라 이 수치를 조절하세요)
            yield return new WaitForSeconds(1.0f);

            // 공격 종료. 다음 프레임부터 다시 거리 계산 및 쿨타임 계산 시작
            isAttacking = false;
        }

        // 외부에서 적에게 데미지를 입힐 때 호출하는 함수
        public void TakeDamage(int amount)
        {
            if (isDead) return;

            health -= amount;
            Debug.Log($"적이 {amount}의 데미지를 입었습니다. 남은 체력: {health}");

            if (health <= 0)
            {
                Die();
            }
        }

        private void Die()
        {
            isDead = true;
            SetState(3); // 사망 애니메이션 (3)
            Debug.Log("적이 사망했습니다.");
            
            // 콜라이더를 끄거나 일정 시간 후 오브젝트를 파괴하는 로직을 추가할 수 있습니다.
            // GetComponent<Collider>().enabled = false;
            // Destroy(gameObject, 3f);
        }

        // 애니메이터 State 파라미터 변경을 위한 헬퍼 함수
        private void SetState(int stateValue)
        {
            if (animator != null && animator.GetInteger(stateParamName) != stateValue)
            {
                animator.SetInteger(stateParamName, stateValue);
            }
        }
    }
}
