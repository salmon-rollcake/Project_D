using UnityEngine;

namespace MyFPS
{
    /// <summary>
    /// 플레이어의 이동을 관리하는 클래스
    /// </summary>
    public class PlayerMove : MonoBehaviour
    {
        #region Variables
        // 참조
        CharacterInput input;
        CharacterController controller;

        [Header ("Player")]
        [SerializeField] float walkSpeed = 4f;
        [SerializeField] float runSpeed = 8f;
        float moveSpeed;
        #endregion

        #region Unity Event Methods
        private void Awake()
        {
            input = GetComponent<CharacterInput>();
            controller = GetComponent<CharacterController>();
        }

        private void Update()
        {
            // 이동 처리
            Move();
        }
        #endregion

        #region Custom Methods
        void Move()
        {
            moveSpeed = walkSpeed;

            // 인풋 체크
            if (input.Move == Vector2.zero)
            {
                moveSpeed = 0f;
            }

            // 방향 체크
            Vector3 inputDir = Vector3.zero;

            if (input.Move != Vector2.zero)
            {
                inputDir = transform.right * input.Move.x + transform.forward * input.Move.y;
            }

            // 이동 : 방향 * Time.deltaTime * 이동 속도
            controller.Move(inputDir.normalized * Time.deltaTime * moveSpeed);
        }
        #endregion
    }
}