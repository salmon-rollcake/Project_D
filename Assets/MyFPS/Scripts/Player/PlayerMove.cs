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

        [Header ("Move")]
        [SerializeField] float walkSpeed = 4f;
        [SerializeField] float runSpeed = 8f;
        float moveSpeed;

        [Header ("Ground")]
        [SerializeField] bool isGrounded = false;

        [SerializeField] float groundOffset = -0.14f; // 체크 지점 조정값
        [SerializeField] float groundRadius = 0.5f; // 체크 범위
        public LayerMask groundLayers; // 지형 레이어

        [Header ("Jump")]
        [SerializeField] float gravity = -9.81f;
        [SerializeField] float verticalVelocity = 0f;
        [SerializeField] float jumpHigh = 1.2f;
        [SerializeField] float jumpCD = 0.1f;
        #endregion

        #region Unity Event Methods
        private void Awake()
        {
            input = GetComponent<CharacterInput>();
            controller = GetComponent<CharacterController>();
        }

        private void Update()
        {
            // 바닥 체크
            CheckGrounded();

            // 이동 처리
            Move();

            // 중력
            GravitySetup();
        }
        #endregion

        #region Custom Methods
        void CheckGrounded()
        {
            Vector3 checkPos = new Vector3(transform.position.x, transform.position.y - groundOffset, transform.position.z);
            Physics.CheckSphere(checkPos, groundRadius, groundLayers, QueryTriggerInteraction.Ignore);
        }
        
        void GravitySetup()
        {
            verticalVelocity += gravity * Time.deltaTime;

            if (isGrounded)
            {
                // 지면에 있을 때 벨로시티 값을 고정
                if (verticalVelocity < 0f)
                {
                    verticalVelocity = -2f;
                }

                // 점프 입력 체크
                if (input.IsJump && jumpCD <= 0f)
                {
                    // 점프 높이 만큼 속도를 지정
                    verticalVelocity = Mathf.Sqrt(jumpHigh * -2f * gravity);
                }

                if (jumpCD >= 0f)
                {
                    jumpCD -= Time.deltaTime;
                }
            }
            else
            {
                input.IsJump = false;
                jumpCD = 0.1f;
            }
        }

        void Move()
        {
            moveSpeed = input.IsRun ? runSpeed : walkSpeed;

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

            // 이동 : 방향(앞뒤좌우) * Time.deltaTime * 이동 속도 + (상하) * Time.deltaTime * 중력
            controller.Move(inputDir * Time.deltaTime * moveSpeed
                            + Vector3.up * Time.deltaTime * verticalVelocity);
        }
        #endregion
    }
}