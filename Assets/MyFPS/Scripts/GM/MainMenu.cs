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

        [SerializeField] GameObject MainUI;
        [SerializeField] GameObject OptionUI;
        [SerializeField] GameObject CreditsUI_Eng;
        [SerializeField] GameObject CreditsUI_Kor;

        bool activeCredit = false;

        int sceneNumber;

        private void Start()
        {
            AudioManager.Instance.PlayBGM("MainTheme");
            ExitOptions();
            CreditsUI_Eng.SetActive(false);
            CreditsUI_Kor.SetActive(false);

            sceneNumber = PlayerPrefs.GetInt("SceneNumber", -1);
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
            MainUI.SetActive(false);
            OptionUI.SetActive(true);
        }

        public void ExitOptions()
        {
            OptionUI.SetActive(false);
            MainUI.SetActive(true);
        }

        public void Credits()
        {
            Debug.Log("크레딧을 표시합니다.");
            MainUI.SetActive(false);
            CreditsUI_Kor.SetActive(true);
            activeCredit = true;

            if (activeCredit && Input.GetKeyDown(KeyCode.Escape))
            {
                ExitCredits();
            }
        }

        public void ExitCredits()
        {
            activeCredit = false;
            CreditsUI_Kor.SetActive(false);
            MainUI.SetActive(false);
        }

        public void QuitGame()
        {
            Debug.Log("게임을 종료합니다.");
            Application.Quit();
        }
    }
}