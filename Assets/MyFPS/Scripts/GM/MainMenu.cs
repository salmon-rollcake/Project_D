using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

namespace MyFPS
{

    public class MainMenu : MonoBehaviour
    {
        AudioManager audioManager;

        public SceneFader fader;

        [SerializeField] string LoadToScene;

        private void Start()
        {
            AudioManager.Instance.PlayBGM("MainTheme");
        }

        public void NewGame(string sceneName)
        {
            Debug.Log("새 게임을 시작합니다.");
            SceneManager.LoadScene(sceneName);
        }

        public void LoadGame()
        {
            Debug.Log("저장된 게임을 불러옵니다.");
        }

        public void Options()
        {
            Debug.Log("옵션 메뉴를 엽니다.");
        }

        public void Credits()
        {
            Debug.Log("크레딧을 표시합니다.");
        }

        public void QuitGame()
        {
            Debug.Log("게임을 종료합니다.");
            Application.Quit();
        }
    }
}