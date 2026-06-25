using UnityEngine;

namespace MyFPS
{

    public class CharacterInput : MonoBehaviour
    {
        #region Variables
        // 인풋 시스템 인스턴스
        InputSystem_Actions inputActions;

        // 이동 입력 값 - WASD
        Vector2 move;
        [SerializeField] bool isRun;

        // 마우스 회전
        Vector2 look;

        // 점프
        [SerializeField] bool isJump;
        #endregion

        #region Property
        public Vector2 Move
        {
            get
            {
                return move;
            }
            set
            {
                move = value;
            }
        }

        public Vector2 Look
        {
            get { return look; }
            set { look = value; }
        }

        public bool IsRun
        {
            get { return isRun; }
            set {  isRun = value; }
        }

        public bool IsJump
        {
            get { return isJump; }
            set { isJump = value; }
        }
        #endregion

        #region Unity Event Methods
        private void Awake()
        {
            inputActions = new InputSystem_Actions();
        }

        private void OnEnable()
        {
            // 인풋 시스템 인스턴스 활성화
            inputActions.Enable();
        }

        private void OnDisable()
        {
            // 인풋 시스템 인스턴스 비활성화
            inputActions.Disable();
        }

        private void Update()
        {
            // WASD 입력값 처리 : 인스턴스이름.액션맵이름.액션이름.ReadValue< >();
            Move = inputActions.Player.Move.ReadValue<Vector2>();
            Look = inputActions.Player.Look.ReadValue<Vector2>();

            if (inputActions.Player.Jump.WasPressedThisFrame())
            {
                isJump = true;
            }

            if (inputActions.Player.Sprint.WasPressedThisFrame())
            {
                isRun = true;
            }
            else if (inputActions.Player.Sprint.WasReleasedThisFrame())
            {
                isRun = false;
            }
        }
        #endregion
    }
}