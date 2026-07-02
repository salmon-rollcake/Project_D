using UnityEngine;

namespace MyFPS
{
    public class BGMManager : MonoBehaviour
    {
        public static BGMManager Instance { get; private set; }

        [Header("Audio Source")]
        [SerializeField] private AudioSource audioSource;

        [Header("Audio Clips")]
        [SerializeField] private AudioClip defaultBGM;
        [SerializeField] private AudioClip combatBGM;

        private bool isCombat = false;
        private bool enemiesDetected = false;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            if (audioSource == null)
            {
                audioSource = GetComponent<AudioSource>();
                if (audioSource == null)
                {
                    audioSource = gameObject.AddComponent<AudioSource>();
                }
            }

            audioSource.loop = true;
            audioSource.playOnAwake = false;
        }

        private void Start()
        {
            PlayDefaultBGM();
        }

        private void Update()
        {
            if (isCombat)
            {
                // 씬 내의 모든 EnemyAI를 탐색 (FindObjectsByType 사용으로 경고 해결 및 성능 향상)
                EnemyAI[] enemies = FindObjectsByType<EnemyAI>(FindObjectsSortMode.None);
                
                bool anyEnemyAlive = false;
                foreach (var enemy in enemies)
                {
                    if (enemy != null && enemy.gameObject.activeInHierarchy && !enemy.IsDead)
                    {
                        anyEnemyAlive = true;
                        break;
                    }
                }

                if (anyEnemyAlive)
                {
                    enemiesDetected = true;
                }

                // 적이 감지되었었고, 현재 살아있는 적이 없다면 기본 BGM으로 전환
                if (enemiesDetected && !anyEnemyAlive)
                {
                    PlayDefaultBGM();
                }
            }
        }

        public void PlayCombatBGM()
        {
            if (combatBGM != null)
            {
                if (audioSource.clip != combatBGM)
                {
                    audioSource.clip = combatBGM;
                    audioSource.Play();
                }
                isCombat = true;
                enemiesDetected = false; // 적 감지 플래그 초기화
            }
            else
            {
                Debug.LogWarning("BGMManager: 전투 BGM 클립이 지정되지 않았습니다!");
            }
        }

        public void PlayDefaultBGM()
        {
            if (defaultBGM != null)
            {
                if (audioSource.clip != defaultBGM)
                {
                    audioSource.clip = defaultBGM;
                    audioSource.Play();
                }
                isCombat = false;
                enemiesDetected = false;
            }
            else
            {
                Debug.LogWarning("BGMManager: 기본 BGM 클립이 지정되지 않았습니다!");
            }
        }
    }
}
