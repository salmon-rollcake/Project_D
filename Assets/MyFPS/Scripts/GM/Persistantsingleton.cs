using UnityEngine;

public class Persistantsingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                // 씬에 이미 배치되어 있는지 확인
                _instance = FindFirstObjectByType<T>();

                // 씬에 없다면 새로 생성
                if (_instance == null)
                    _instance = CreateDefaultInstance();
            }
            return _instance;
        }
    }

    protected virtual void Awake()
    {
        if (_instance == null)
        {
            _instance = this as T;
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            // 이미 인스턴스가 존재하는데 다른 객체가 가리키고 있다면 파괴
            Destroy(gameObject);
        }
    }

    private static T CreateDefaultInstance()
    {
        GameObject obj = new GameObject(typeof(T).Name + " (Singleton)");
        return obj.AddComponent<T>();
    }
}
