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

        bool isMove;
        bool isRun;

        string isMoving = "isMove";
        string isRunning = "isRun";

        Vector3 move;

        InputActionReference moveInput;
        InputActionReference runInput;
        
        #endregion

        #region Property
        public Vector3 Move
        {
            get { return move; }
            set { move = value; }
        }

        public bool IsMove
        {
            get { return isMove; }
            set { isMove = value; }
        }

        public bool IsRun
        {
            get { return isRun; }
            set { isRun = value; }
        }
        #endregion

        #region Unity Event Methods
        private void Awake()
        {
            anim = GetComponent<Animator>();
            input = new InputSystem_Actions();
        }

        private void OnEnable()
        {
            input.Enable();
        }

        private void OnDisable()
        {
            input.Disable();
        }

        private void Update()
        {
            Vector2 inputMove = input.Player.Move.ReadValue<Vector2>();

            if (input.Player.Sprint.WasPressedThisFrame())
            {
                isRun = true;
            }
            else isRun = false;

        }
        #endregion
    }
}