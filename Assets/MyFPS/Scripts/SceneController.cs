using UnityEngine;
using UnityEngine.SceneManagement;

namespace MyFPS
{

    public class SceneController : MonoBehaviour
    {
        public void LoadScene(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }
    }
}