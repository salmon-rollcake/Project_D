using UnityEngine;

namespace MyFPS
{
    public class CCTV_World : MonoBehaviour
    {
        public Camera targetCamera;
        public float targetDepth = 10f; // 카메라로부터의 거리(깊이)
        public float rotationSpeed = 5f;

        void Update()
        {
            if (targetCamera == null) return;

            // 1. 마우스 화면 좌표에 가상 깊이(z)를 입력
            Vector3 mouseScreenPos = Input.mousePosition;
            mouseScreenPos.z = targetDepth;

            // 2. 화면 좌표를 3D 월드 좌표로 변환
            Vector3 targetWorldPos = targetCamera.ScreenToWorldPoint(mouseScreenPos);

            // 3. CCTV가 그 좌표를 바라보도록 회전
            Vector3 direction = targetWorldPos - transform.position;
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }
}
