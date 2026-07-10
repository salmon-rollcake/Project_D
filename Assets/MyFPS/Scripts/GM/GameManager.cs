using UnityEngine;

namespace MyFPS
{
    
public class GameManager : MonoBehaviour
{
    private static GameManager instance;

    void Awake()
    {
        // 싱글톤 패턴을 활용해 중복 생성을 방지합니다.
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // 씬이 바뀌어도 파괴되지 않음!
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
}