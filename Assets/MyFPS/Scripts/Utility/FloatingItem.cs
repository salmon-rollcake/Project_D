using UnityEngine;

namespace MyFPS
{
    /// <summary>
    /// 아이템이 제자리에서 회전하며 위아래로 둥둥 떠다니는 연출을 담당합니다.
    /// </summary>
    public class FloatingItem : MonoBehaviour
    {
        [Header("회전 설정")]
        [SerializeField] private float rotateSpeed = 50f;       // 회전 속도

        [Header("위아래 흔들림(부유) 설정")]
        [SerializeField] private float amplitude = 0.15f;       // 움직임 폭 (위아래로 얼마나 크게 움직일지)
        [SerializeField] private float frequency = 2f;          // 움직임 속도 (얼마나 빠르게 출렁일지)

        private Vector3 startPosition;

        private void Start()
        {
            // 시작할 때의 기본 위치를 기억합니다.
            startPosition = transform.position;
        }

        private void Update()
        {
            // 1. Y축 기준 회전 효과
            transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime, Space.World);

            // 2. 삼각함수(Sin)를 이용한 위아래 둥둥 연출
            Vector3 tempPosition = startPosition;
            tempPosition.y += Mathf.Sin(Time.time * frequency) * amplitude;
            
            transform.position = tempPosition;
        }
    }
}
