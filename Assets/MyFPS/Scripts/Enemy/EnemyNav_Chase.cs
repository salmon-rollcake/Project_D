using UnityEngine;
using UnityEngine.AI; // 1. NavMesh 사용을 위해 추가

namespace MyFPS
{
    // NavMeshAgent 컴포넌트가 오브젝트에 없을 때 자동으로 추가되도록 강제합니다.
    [RequireComponent(typeof(NavMeshAgent))]
    public class EnemyNav_Chase : MonoBehaviour, IDamageable
    {
        [Header("능력치 설정")]
        [SerializeField] private int health = 20;
        //[SerializeField] private float moveSpeed = 0.5f; // NavMeshAgent의 Speed를 사용하므로 제거하거나 백업용으로만 둡니다.
        [SerializeField] private int damage = 5;
        [SerializeField] private float attackRange = 1.5f;
        [SerializeField] private float attackInterval = 2.0f;

        [Header("컴포넌트 및 참조")]
        [SerializeField] private Animator animator;
        [SerializeField] private string stateParamName = "Enemy State";

        private Transform playerTarget;
        private PlayerHealth playerHealth;
        private NavMeshAgent agent; // 2. 에이전트 변수 추가
        private float attackTimer = 0f;
        private bool isDead = false;
        public bool IsDead => isDead;

        [SerializeField] GameObject player;

        void Start()
        {
            if (animator == null) animator = GetComponent<Animator>();

            // NavMeshAgent 컴포넌트 참조 및 설정
            agent = GetComponent<NavMeshAgent>();
            
            // 기존 스크립트의 공격 범위 값을 NavMeshAgent의 정지 거리(Stopping Distance)에 연동하면 편리합니다.
            agent.stoppingDistance = attackRange; 

            if (player == null) player = GameObject.FindGameObjectWithTag("Player");

            if (player != null)
            {
                playerTarget = player.transform;
                playerHealth = player.GetComponent<PlayerHealth>();

                if (playerHealth == null)
                {
                    enabled = false;
                    Debug.LogWarning("EnemyAI: required Player reference is missing.");
                    return;
                }
            }
            else
            {
                enabled = false;
                Debug.LogWarning("EnemyAI: required Player reference is missing.");
                return;
            }

            SetState(0);
        }

        private bool isAttacking = false;

        void Update()
        {
            if (isDead || playerTarget == null)
            {
                if(agent.enabled) agent.isStopped = true; // 사망 시 이동 정지
                return;
            }

            // 공격 중일 때는 이동을 멈춥니다.
            if (isAttacking)
            {
                agent.isStopped = true;
                return;
            }

            if (playerHealth != null && playerHealth.isFainted)
            {
                agent.isStopped = true;
                SetState(0);
                return;
            }

            // 플레이어와의 거리를 agent의 남은 거리 계산식으로 대체하거나 기존 방식을 유지합니다.
            float distanceToPlayer = Vector3.Distance(transform.position, playerTarget.position);

            if (distanceToPlayer > attackRange)
            {
                MoveTowardsPlayer();
            }
            else
            {
                AttemptAttack();
            }
        }

        private void MoveTowardsPlayer()
        {
            SetState(1);

            // NavMesh Agent 가동 및 목적지 설정
            agent.isStopped = false;
            agent.SetDestination(playerTarget.position);

            // 💡 팁: NavMeshAgent가 자체적으로 회전(Angular Speed)을 처리하므로, 
            // 기존의 transform.position 이동 및 Slerp 회전 코드는 제거했습니다.

            if (attackTimer < attackInterval)
            {
                attackTimer += Time.deltaTime;
            }
        }

        private void AttemptAttack()
        {
            // 공격 범위 내에 들어오면 우선 이동을 멈춥니다.
            agent.isStopped = true;

            // 공격 시에도 플레이어를 바라보게 하는 수동 회전은 유지하는 것이 자연스럽습니다.
            Vector3 direction = (playerTarget.position - transform.position).normalized;
            direction.y = 0;
            if (direction != Vector3.zero)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * 5f);
            }

            attackTimer += Time.deltaTime;

            if (attackTimer >= attackInterval)
            {
                StartCoroutine(AttackRoutine());
            }
            else
            {
                SetState(0);
            }
        }

        private System.Collections.IEnumerator AttackRoutine()
        {
            isAttacking = true;
            attackTimer = 0f;

            SetState(2);

            yield return new WaitForSeconds(0.5f);

            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }

            yield return new WaitForSeconds(1.0f);

            isAttacking = false;
        }

        public void TakeDamage(int amount)
        {
            if (isDead) return;

            health -= amount;
            if (health <= 0) Die();
        }

        private void Die()
        {
            isDead = true;
            agent.enabled = false; // 사망 시 컴포넌트를 꺼서 길찾기 연산을 완전히 중단합니다.
            SetState(3);
        }

        private void SetState(int stateValue)
        {
            if (animator != null && animator.GetInteger(stateParamName) != stateValue)
            {
                animator.SetInteger(stateParamName, stateValue);
            }
        }
    }
}
