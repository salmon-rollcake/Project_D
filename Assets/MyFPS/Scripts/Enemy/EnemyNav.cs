using UnityEngine;
using UnityEngine.AI; // 1. NavMesh 사용을 위해 추가
using System.Collections.Generic; // List 사용을 위해 추가

namespace MyFPS
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class EnemyNav : MonoBehaviour, IDamageable
    {
        private enum AIState { Idle, Patrol, Chase, Attack }
        private AIState currentState = AIState.Patrol;

        [Header("능력치 설정")]
        [SerializeField] private int health = 20;
        [SerializeField] private int damage = 5;
        [SerializeField] private float attackRange = 1.5f;
        [SerializeField] private float attackInterval = 2.0f;

        [Header("순찰 및 감지 설정")]
        [SerializeField] private float detectionRange = 10f;     // 플레이어 감지 범위
        [SerializeField] private float patrolSpeed = 1.5f;       // 순찰 속도
        [SerializeField] private float chaseSpeed = 3.5f;        // 추격 속도
        [SerializeField] private float patrolWaitTime = 2f;      // 지점 도착 후 대기 시간

        [Header("지정 경로 순찰(Waypoints) 설정")]
        [SerializeField] private List<Transform> waypoints = new List<Transform>(); // 순찰할 지점들의 Transform 목록
        private int currentWaypointIndex = 0; // 현재 이동 중인 웨이포인트 번호

        [Header("컴포넌트 및 참조")]
        [SerializeField] private Animator animator;
        [SerializeField] private string stateParamName = "State";

        private Transform playerTarget;
        private PlayerHealth playerHealth;
        private NavMeshAgent agent;
        private float attackTimer = 0f;
        private float patrolTimer = 0f;
        private bool isDead = false;
        private bool isAttacking = false;

        public bool IsDead => isDead;

        [SerializeField] GameObject player;

        void Start()
        {
            if (animator == null) animator = GetComponent<Animator>();

            agent = GetComponent<NavMeshAgent>();
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

            // 첫 순찰 목적지 이동 시작
            currentState = AIState.Patrol;
            MoveToCurrentWaypoint();
        }

        void Update()
        {
            if (isDead || playerTarget == null)
            {
                if (agent.enabled) agent.isStopped = true;
                return;
            }

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

            float distanceToPlayer = Vector3.Distance(transform.position, playerTarget.position);

            switch (currentState)
            {
                case AIState.Patrol:
                    PatrolBehavior(distanceToPlayer);
                    break;

                case AIState.Chase:
                    ChaseBehavior(distanceToPlayer);
                    break;

                case AIState.Attack:
                    AttackBehavior(distanceToPlayer);
                    break;
            }
        }

        #region 순찰(Patrol) 로직 (지정 경로 순회)
        private void PatrolBehavior(float distanceToPlayer)
        {
            if (distanceToPlayer <= detectionRange)
            {
                ToChaseState();
                return;
            }

            // 순찰 경로(Waypoints)가 등록되어 있지 않으면 제자리에 대기
            if (waypoints.Count == 0)
            {
                SetState(0);
                agent.isStopped = true;
                return;
            }

            agent.speed = patrolSpeed;
            agent.isStopped = false;
            SetState(1); // 걷기/달리기 애니메이션

            // 현재 웨이포인트 목적지에 도달했는지 확인
            // (순찰 목적지에서는 attackRange 대신 아주 가까이(예: 0.2m) 붙었을 때 도착한 것으로 판정하기 위해 0.2f 사용)
            if (!agent.pathPending && agent.remainingDistance <= 0.2f)
            {
                SetState(0); // 대기 애니메이션
                agent.isStopped = true;

                patrolTimer += Time.deltaTime;
                if (patrolTimer >= patrolWaitTime)
                {
                    // 다음 웨이포인트 인덱스로 변경 (마지막 지점에 도달하면 다시 0번으로 순환)
                    currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Count;
                    MoveToCurrentWaypoint();
                    patrolTimer = 0f;
                }
            }
        }

        // 현재 인덱스의 웨이포인트로 목적지 설정하는 함수
        private void MoveToCurrentWaypoint()
        {
            if (waypoints.Count > 0 && waypoints[currentWaypointIndex] != null)
            {
                // 순찰 목적지로 갈 때는 stoppingDistance를 일시적으로 아주 작게 세팅합니다.
                // (공격 범위 stoppingDistance가 적용되어 목적지 멀찍이서 멈추는 것을 방지)
                agent.stoppingDistance = 0.2f;
                agent.SetDestination(waypoints[currentWaypointIndex].position);
            }
        }
        #endregion

        #region 추격(Chase) 로직
        private void ToChaseState()
        {
            currentState = AIState.Chase;
            agent.speed = chaseSpeed;
            // 추격할 때는 다시 정지 거리를 공격 범위(attackRange)로 원복합니다.
            agent.stoppingDistance = attackRange;
            patrolTimer = 0f;
        }

        private void ChaseBehavior(float distanceToPlayer)
        {
            if (distanceToPlayer > detectionRange)
            {
                currentState = AIState.Patrol;
                MoveToCurrentWaypoint(); // 다시 순찰 경로로 복귀
                return;
            }

            if (distanceToPlayer <= attackRange)
            {
                currentState = AIState.Attack;
                return;
            }

            SetState(1);
            agent.isStopped = false;
            agent.SetDestination(playerTarget.position);

            if (attackTimer < attackInterval)
            {
                attackTimer += Time.deltaTime;
            }
        }
        #endregion

        #region 공격(Attack) 로직
        private void AttackBehavior(float distanceToPlayer)
        {
            if (distanceToPlayer > attackRange)
            {
                ToChaseState();
                return;
            }

            agent.isStopped = true;

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
        #endregion

        public void TakeDamage(int amount)
        {
            if (isDead) return;

            health -= amount;
            
            if (currentState == AIState.Patrol && !isDead)
            {
                ToChaseState();
            }

            if (health <= 0) Die();
        }

        private void Die()
        {
            isDead = true;
            agent.enabled = false;
            SetState(3);
        }

        private void SetState(int stateValue)
        {
            if (animator != null && animator.GetInteger(stateParamName) != stateValue)
            {
                animator.SetInteger(stateParamName, stateValue);
            }
        }

        // 에디터 씬 뷰에서 순찰 지점들을 선으로 연결해 시각화해주는 기즈모
        private void OnDrawGizmos()
        {
            if (waypoints == null || waypoints.Count == 0) return;

            Gizmos.color = Color.cyan;
            for (int i = 0; i < waypoints.Count; i++)
            {
                if (waypoints[i] == null) continue;

                // 순찰 지점 기즈모 구체 그리기
                Gizmos.DrawSphere(waypoints[i].position, 0.4f);

                // 다음 지점 연결 선 그리기
                int nextIndex = (i + 1) % waypoints.Count;
                if (waypoints[nextIndex] != null)
                {
                    Gizmos.DrawLine(waypoints[i].position, waypoints[nextIndex].position);
                }
            }

            // 플레이어 감지 범위 그리기
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, detectionRange);
        }
    }
}
