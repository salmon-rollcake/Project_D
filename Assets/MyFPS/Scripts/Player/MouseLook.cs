using UnityEngine;

namespace MyFPS
{
    /// <summary>
    /// 마우스 움직임을 받아 시야를 구현하는 클래스
    /// </summary>
    public class MouseLook : MonoBehaviour
    {
        #region Variables
        public Transform cameraTrans;

        CharacterInput input;

        [SerializeField] float rotateSpeed = 1f;

        [SerializeField] float sensivity = 100f;

        float cameraTargetPitch = 0f; // 카메라 회전 연산값 (위, 아래)
        float rotationVelocity = 0f; // 카메라 회전 속도 (좌, 우)

        [SerializeField] float topClamp = 45f; // 카메라 위 최대값
        [SerializeField] float bottomClamp = -90f; // 카메라 아래 최대값
        #endregion

        #region Unity Event Methods
        private void Awake()
        {
            input = GetComponent<CharacterInput>();
        }

        private void Start()
        {
            // 마우스 커서 초기화
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false; // 마우스 포인터 숨김 처리
        }

        private void LateUpdate()
        {
            // 카메라 회전
            CameraRotate();
        }
        #endregion

        #region Custom Methods
        void CameraRotate()
        {
            if (input.Look.sqrMagnitude < 0.01f)
            {
                return;
            }

            // 좌 / 우 (플레이어의 트랜스폼을 회전)
            rotationVelocity = input.Look.x * rotateSpeed * Time.deltaTime * sensivity;
            transform.Rotate(Vector3.up * rotationVelocity);

            // 위 / 아래 (카메라 회전)
            cameraTargetPitch -= input.Look.y * rotateSpeed * Time.deltaTime * sensivity;
            cameraTargetPitch = ClampAngle(cameraTargetPitch, bottomClamp, topClamp);
            cameraTrans.localRotation = Quaternion.Euler(cameraTargetPitch, 0f, 0f);
        }

        public void SetTargetPitch(float pitch)
        {
            cameraTargetPitch = ClampAngle(pitch, bottomClamp, topClamp);
            cameraTrans.localRotation = Quaternion.Euler(cameraTargetPitch, 0f, 0f);
        }
        #endregion

        #region
        float ClampAngle(float angle, float min, float max)
        {
            if (angle < -360f) angle += 360f;
            if (angle > 360f) angle -= 360f;
            return Mathf.Clamp(angle, min, max);
        }
        #endregion
    }
}