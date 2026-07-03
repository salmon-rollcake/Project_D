using System.Collections.Generic;
using UnityEngine;

namespace MyFPS
{
    /// <summary>
    /// 게임을 일시 정지하고 일시 정지 UI를 활성화하는 클래스.
    /// P 키를 토글 키로 사용하여 일시 정지 및 재개를 처리합니다.
    /// </summary>
    public class PauseMenu : MonoBehaviour
    {
        // 전역에서 일시 정지 상태를 참조할 수 있도록 static 변수 제공
        public static bool IsGamePaused = false;

        [Header("UI 설정")]
        [Tooltip("일시 정지 시 활성화할 Canvas 내부의 UI 패널 오브젝트")]
        [SerializeField] private GameObject pauseMenuUI;

        [Header("플레이어 설정")]
        [Tooltip("플레이어 오브젝트를 찾기 위한 태그")]
        [SerializeField] private string playerTag = "Player";

        // 자동으로 수집된 비활성화할 플레이어 컴포넌트들
        private List<MonoBehaviour> componentsToDisable = new List<MonoBehaviour>();

        private void Start()
        {
            // 게임 시작 시에는 일시 정지 UI를 비활성화하고 게임을 실행 상태로 시작
            if (pauseMenuUI != null)
            {
                pauseMenuUI.SetActive(false);
            }
            
            // 정적 변수 초기화
            IsGamePaused = false;

            // 'Player' 태그를 가진 오브젝트 자동 탐색
            GameObject playerObj = GameObject.FindWithTag(playerTag);
            if (playerObj != null)
            {
                // 조작을 차단할 플레이어의 컴포넌트들을 자동으로 검색 및 등록
                AddScriptToDisable<CharacterInput>(playerObj);
                AddScriptToDisable<MouseLook>(playerObj);
                AddScriptToDisable<PlayerMove>(playerObj);
                AddScriptToDisable<GunShoot>(playerObj);
                AddScriptToDisable<GunShootProjectile>(playerObj);
            }
            else
            {
                Debug.LogWarning($"PauseMenu: '{playerTag}' 태그를 가진 플레이어 오브젝트를 찾을 수 없어 자동 컴포넌트 감지가 생략되었습니다.");
            }
            /*
            // UI 내 모든 Animator를 Unscaled Time 모드로 변경 (일시 정지 중에도 애니메이션이 멈추지 않게 함)
            if (pauseMenuUI != null)
            {
                Animator[] uiAnimators = pauseMenuUI.GetComponentsInChildren<Animator>(true);
                foreach (var anim in uiAnimators)
                {
                    anim.updateMode = AnimatorUpdateMode.UnscaledTime;
                }
            }
            */
        }

        /// <summary>
        /// 플레이어 오브젝트 및 하위 오브젝트에서 특정 컴포넌트를 찾아 비활성화 목록에 추가합니다.
        /// </summary>
        private void AddScriptToDisable<T>(GameObject playerObj) where T : MonoBehaviour
        {
            T comp = playerObj.GetComponentInChildren<T>();
            if (comp != null)
            {
                componentsToDisable.Add(comp);
            }
        }

        private void Update()
        {
            // 키보드 P 키 입력 시 토글 처리
            if (Input.GetKeyDown(KeyCode.P))
            {
                if (IsGamePaused)
                {
                    Resume();
                }
                else
                {
                    Pause();
                }
            }
        }

        /// <summary>
        /// 게임을 재개합니다.
        /// </summary>
        public void Resume()
        {
            if (pauseMenuUI != null)
            {
                pauseMenuUI.SetActive(false);
            }

            Time.timeScale = 1f;
            IsGamePaused = false;

            // 마우스 커서를 다시 화면에 가두고 감춤 (FPS 조작 상태로 복구)
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            // 일시 정지 동안 비활성화했던 플레이어 컴포넌트들을 다시 활성화
            TogglePlayerComponents(true);
        }

        /// <summary>
        /// 게임을 일시 정지합니다.
        /// </summary>
        public void Pause()
        {
            if (pauseMenuUI != null)
            {
                pauseMenuUI.SetActive(true);
            }

            Time.timeScale = 0f;
            IsGamePaused = true;

            // 마우스 커서 잠금을 해제하고 화면에 표시 (UI 조작용)
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            // 일시 정지 동안 플레이어가 조작하거나 움직이지 못하도록 컴포넌트들을 비활성화
            TogglePlayerComponents(false);
        }

        /// <summary>
        /// 플레이어 입력 및 물리 조작 관련 컴포넌트들의 활성화 상태를 토글합니다.
        /// </summary>
        private void TogglePlayerComponents(bool state)
        {
            foreach (var comp in componentsToDisable)
            {
                if (comp != null)
                {
                    comp.enabled = state;
                }
            }
        }
    }
}
