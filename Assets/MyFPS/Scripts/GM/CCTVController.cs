using UnityEngine;

namespace MyFPS
{

    public class CCTVController : MonoBehaviour
    {
        [Header("회전 민감도")]
        public float sensitivityX = 2.0f;
        public float sensitivityY = 2.0f;

        [Header("추적 속도")]
        public float rotationSpeed = 5.0f;

        private Quaternion centerRotation;

        void Start()
        {
            // 피벗 오브젝트의 처음 회전값(0,0,0)을 기준점으로 잡습니다.
            centerRotation = transform.localRotation;
        }

        void Update()
        {
            float mouseXRatio = (Input.mousePosition.x / Screen.width) * 2f - 1f;
            float mouseYRatio = (Input.mousePosition.y / Screen.height) * 2f - 1f;

            float targetYaw = mouseXRatio * sensitivityX * 30f;
            float targetPitch = -mouseYRatio * sensitivityY * 30f;

            // 부모(피벗)가 회전하므로, 자식인 CCTV 머리는 자체 꺾인 각도를 유지한 채 자연스럽게 회전합니다.
            Quaternion targetRotation = centerRotation * Quaternion.Euler(targetPitch, targetYaw, 0f);
            transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }
}