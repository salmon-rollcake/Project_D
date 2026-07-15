using UnityEngine;
using Unity.Cinemachine;

namespace MyFPS
{
    /// <summary>
    /// 화면 흔들림 효과 구현
    /// </summary>
public class CinemachineShake : MonoBehaviour
{
    // 컴포넌트를 참조할 변수
    private CinemachineImpulseSource _impulseSource;

    void Start()
    {
        // 2. 오브젝트에 붙은 임펄스 소스를 가져옵니다.
        _impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    void Update()
    {
        // 마우스 좌클릭(사격) 시 실행
        if (Input.GetButtonDown("Fire1"))
        {
            PerformShot();
        }
    }

    void PerformShot()
    {
        // [여기에 실제 레이캐스트나 탄환 생성 로직 작성]
        Debug.Log("사격! 탕!");

        // 3. 씨네머신 카메라를 향해 흔들림 신호 방출
        if (_impulseSource != null)
        {
            _impulseSource.GenerateImpulse();
        }
    }
}
}