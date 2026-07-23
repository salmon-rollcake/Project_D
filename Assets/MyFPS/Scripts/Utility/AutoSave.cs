using UnityEngine;
using UnityEngine.SceneManagement;

namespace MyFPS
{
    
public class AutoSave : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // 씬 번호 저장
        SaveSceneNum();
    }

    void SaveSceneNum()
        {
            int sceneNumber = SceneManager.GetActiveScene().buildIndex;
            Debug.Log($"SceneNumber {sceneNumber} Saved.");

            //PlayerPrefs
            PlayerPrefs.SetInt("SceneNumber", sceneNumber);
        }
}
}