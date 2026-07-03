using UnityEngine;
using System.Collections;

namespace MyFPS
{
    public class SceneTransitionTrigger : MonoBehaviour
    {
        [Header("씬 전환 설정")]
        [SerializeField] private string nextSceneName;              // 전환할 다음 씬 이름
        [SerializeField] private SceneFader sceneFader;             // SceneFader 컴포넌트

        [Header("플레이어 제어")]
        [SerializeField] private GameObject playerObject;           // 플레이어 오브젝트

        [SerializeField] GameObject gameManager;

        private PlayerInteract playerInteraction;
        private PlayerMove playerMove;
        private MouseLook mouseLook;
        private BGMManager bgm;
        private bool hasTriggered = false;

        void Start()
        {
            if (playerObject != null)
            {
                playerInteraction = playerObject.GetComponent<PlayerInteract>();
                playerMove = playerObject.GetComponent<PlayerMove>();
                mouseLook = playerObject.GetComponent<MouseLook>();
            }

            if (gameManager != null)
            {
                bgm = gameManager.GetComponent<BGMManager>();
            }

            // SceneFader가 수동으로 할당되지 않은 경우, 씬에서 자동으로 검색
            if (sceneFader == null)
            {
                sceneFader = FindFirstObjectByType<SceneFader>();
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (hasTriggered || !other.CompareTag("Player")) return;

            hasTriggered = true;
            StartCoroutine(TransitionSequence());
        }

        IEnumerator TransitionSequence()
        {
            // 1. 플레이어 조작 비활성화
            SetPlayerControl(false);

            // 2. 페이더가 있으면 페이드아웃 후 전환, 없으면 즉시 전환
            if (sceneFader != null)
            {
                bgm.audioSource.Pause(); // BGM 일시정지
                sceneFader.FadeTo(nextSceneName);
            }
            else
            {
                Debug.LogWarning("씬에서 SceneFader를 찾을 수 없습니다! 직접 씬을 로드합니다.");
                UnityEngine.SceneManagement.SceneManager.LoadScene(nextSceneName);
            }

            yield return null;
        }

        private void SetPlayerControl(bool isEnable)
        {
            if (playerInteraction != null) playerInteraction.enabled = isEnable;
            if (playerMove != null) playerMove.enabled = isEnable;
            if (mouseLook != null) mouseLook.enabled = isEnable;
        }
    }
}
