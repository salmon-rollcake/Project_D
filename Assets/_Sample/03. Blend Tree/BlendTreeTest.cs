using UnityEngine;
using UnityEngine.InputSystem;

namespace MySample
{

    public class BlendTreeTest : MonoBehaviour
    {
        Animator anim;

        [SerializeField] float moveSpeed = 5f;

        [SerializeField] InputActionReference moveAction;

        Vector2 inputMove;

        string moveState = "MoveState";
        string moveX = "MoveX";
        string moveY = "MoveY";

        private void Awake()
        {
            anim = GetComponent<Animator>();
        }

        private void OnEnable()
        {
            moveAction.action.Enable();
        }

        private void OnDisable()
        {
            moveAction.action.Disable();
        }

        private void Update()
        {
            inputMove = moveAction.action.ReadValue<Vector2>();

            // StateTest(inputMove);
            BlendAnim(inputMove);

            Vector3 dir = new Vector3(inputMove.x, 0f, inputMove.y);
            transform.Translate(dir * Time.deltaTime * moveSpeed, Space.World);
        }

        void BlendAnim(Vector2 moveDir)
        {
            anim.SetFloat(moveX, moveDir.x);
            anim.SetFloat(moveY, moveDir.y);
        }

        void StateTest(Vector2 moveDir)
        {
            if (moveDir == Vector2.zero)
            {
                anim.SetInteger(moveState, 0); // Idle
            }
            else
            {
                if (moveDir.y > 0f)
                {
                    anim.SetInteger(moveState, 1); // Foward
                }
                if (moveDir.y < 0f)
                {
                    anim.SetInteger(moveState, 2); // Backward
                }
                if (moveDir.x < 0f)
                {
                    anim.SetInteger(moveState, 3); // Right
                }
                if (moveDir.x > 0f)
                {
                    anim.SetInteger(moveState, 4); // Left
                }
            }
        }
    }
}