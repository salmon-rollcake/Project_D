using UnityEngine;
using UnityEngine.InputSystem;

namespace MySample
{
    /// <summary>
    /// (예제) 캐릭터 애니메이션 제어
    /// New Input System
    /// </summary>
    public class CharAnimTest : MonoBehaviour
    {
        #region Variables
        Animator anim;

        [SerializeField] bool isMove;
        [SerializeField] bool isRun;

        string isMoving = "isMove";
        string isRunning = "isRun";
        string moveVelocity = "velocity";

        [SerializeField] float walkSpeed = 4f;
        [SerializeField] float runSpeed = 7f;
        float moveSpeed = 0f;

        [SerializeField] float accelSpeed = 0.1f;

        public InputActionReference moveAction;
        public InputActionReference runAction;
        #endregion

        #region Property
        public bool IsMove
        {
            get { return isMove; }
            set
            {
                isMove = value;
                anim.SetBool(isMoving, value);
            }
        }

        public bool IsRun
        {
            get { return isRun; }
            set
            {
                isRun = value;
                anim.SetBool(isRunning, value);
            }
        }

        public float MoveSpeed
        {
            get { return moveSpeed; }
            set
            {
                moveSpeed = value;
                anim.SetFloat(moveVelocity, value);
            }
        }
        #endregion

        #region Unity Event Methods
        private void Awake()
        {
            anim = GetComponent<Animator>();
        }

        private void OnEnable()
        {
            //인풋 액션 활성화
            moveAction.action.Enable();
            runAction.action.Enable();
        }

        private void OnDisable()
        {
            //인풋 액션 비활성화
            moveAction.action.Disable();
            runAction.action.Disable();
        }

        private void Update()
        {
            Vector2 inputMove = moveAction.action.ReadValue<Vector2>();

            IsMove = inputMove != Vector2.zero;

            if (runAction.action.WasPressedThisFrame())
            {
                IsRun = true;
            }
            else if (runAction.action.WasReleasedThisFrame())
            {
                IsRun = false;
            }

            // 걷기
            if (isMove && !isRun)
            {
               if (moveSpeed > walkSpeed)
                {

                }
                
                if (runSpeed > walkSpeed)
                {
                    moveSpeed += accelSpeed;
                    if (moveSpeed >= walkSpeed)
                    {
                        moveSpeed = walkSpeed;
                    }
                }
            }
            else if (isMove && isRun) // 뛰기
            {
                moveSpeed += accelSpeed;
                if (moveSpeed >= runSpeed)
                {
                    moveSpeed = runSpeed;
                }
            }
        }
        #endregion
    }
}