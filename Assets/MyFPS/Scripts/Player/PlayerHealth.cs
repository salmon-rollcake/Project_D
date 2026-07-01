using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace MyFPS
{
    public class PlayerHealth : MonoBehaviour
    {
        [Header("능력치 설정")]
        public int maxHealth = 20;     // 최대 체력
        public int currentHealth;

        [Header("피격 효과 UI")]
        [SerializeField] private Image hitEffectImage;   // 피격 시 활성화할 이미지 (예: 빨간 테두리)
        [SerializeField] private float fadeOutDuration = 0.5f; // 페이드 아웃 되는 데 걸리는 시간

        [Header("피격 음성")]
        [SerializeField] private AudioClip[] hitVoiceClips; // 준비해 둔 피격 음성 3가지
        [SerializeField] private AudioSource audioSource;   // 음성을 재생할 오디오 소스

        [Header("게임 오버 UI")]
        [SerializeField] private GameObject gameOverUI; // 게임 오버 UI를 연결할 변수
        [SerializeField] private Animator gameOverAnim;

        private Coroutine fadeCoroutine; // 페이드 아웃 코루틴 중복 실행 방지용

        public bool isFainted = false; // 플레이어가 기절 상태인지 여부를 나타내는 변수
        [SerializeField] GameObject player;

        void Start()
        {
            currentHealth = maxHealth;

            // 시작할 때 피격 UI 이미지를 완전히 투명하게 초기화
            if (hitEffectImage != null)
            {
                Color c = hitEffectImage.color;
                c.a = 0f;
                hitEffectImage.color = c;
            }

            if (gameOverUI != null)
            {
                gameOverUI.SetActive(false);
            }

            if (gameOverAnim != null)
            {
                gameOverAnim = gameOverUI.GetComponent<Animator>();
            }
        }

        // 외부(EnemyAI 등)에서 호출할 피격 함수
        public void TakeDamage(int amount)
        {
            currentHealth -= amount;
            currentHealth = Mathf.Max(currentHealth, 0); // 0 미만으로 내려가지 않게 클램프

            Debug.Log($"플레이어가 {amount}의 데미지를 받았습니다. 남은 체력: {currentHealth}/{maxHealth}");

            // 1. 피격 음성 랜덤 재생
            PlayRandomHitVoice();

            if (currentHealth > 0)
            {
                // 2. 피격 UI 효과 실행
                ShowHitEffect();
            }

            // 3. 체력 0 이하 시 사망 처리
            if (currentHealth <= 0)
            {
                Die();
            }
        }

        private void PlayRandomHitVoice()
        {
            if (audioSource != null && hitVoiceClips != null && hitVoiceClips.Length > 0)
            {
                // 배열에 들어있는 클립 중 하나를 랜덤으로 선택
                int randomIndex = Random.Range(0, hitVoiceClips.Length);
                AudioClip selectedClip = hitVoiceClips[randomIndex];

                if (selectedClip != null)
                {
                    // 이전 음성이 재생 중이더라도 덮어쓰지 않고 끊지 않으려면 PlayOneShot을 사용
                    audioSource.PlayOneShot(selectedClip);
                }
            }
        }

        private void ShowHitEffect()
        {
            if (hitEffectImage == null) return;

            // 기존에 페이드 아웃 중이던 코루틴이 있다면 중지하고 다시 시작
            // (빠르게 연속으로 맞을 때 효과가 초기화되어 다시 나타나게 하기 위함)
            if (fadeCoroutine != null)
            {
                StopCoroutine(fadeCoroutine);
            }

            fadeCoroutine = StartCoroutine(FadeOutHitEffect());
        }

        private IEnumerator FadeOutHitEffect()
        {
            // 이미지를 즉시 완전히 불투명하게 만들기
            Color c = hitEffectImage.color;
            c.a = 1f;
            hitEffectImage.color = c;

            // fadeOutDuration 동안 서서히 투명해지기
            float elapsed = 0f;
            while (elapsed < fadeOutDuration)
            {
                elapsed += Time.deltaTime;
                c.a = Mathf.Lerp(1f, 0f, elapsed / fadeOutDuration);
                hitEffectImage.color = c;
                yield return null;
            }

            // 완전히 투명하게 마무리
            c.a = 0f;
            hitEffectImage.color = c;
            fadeCoroutine = null;
        }

        private void Die()
        {
            Debug.Log("<color=red>플레이어가 사망했습니다!</color>");

            isFainted = true;
            player.SetActive(false); // 플레이어 오브젝트 비활성화

            gameOverUI.SetActive(true); // 게임 오버 UI 활성화
            gameOverAnim.Play("GameOver"); // 게임 오버 애니메이션 재생
            
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}
