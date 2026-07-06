using UnityEngine;
using UnityEngine.InputSystem;

namespace MySample
{
    public class PlayerMove : MonoBehaviour
    {
        Rigidbody rb;

        [SerializeField] float forwardForce = 5;
        [SerializeField] float sideForce = 5;

        public InputActionReference moveAction;
        Vector2 move;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
        }

        private void Update()
        {
            move = moveAction.action.ReadValue<Vector2>();
        }

        private void FixedUpdate()
        {
            rb.AddForce(0f, 0f, forwardForce, ForceMode.Acceleration);

            if (move.x < 0f)
            {
                rb.AddForce(-sideForce, 0f, 0f, ForceMode.Acceleration);

            }
            else if (move.x > 0f)
            {
                rb.AddForce(sideForce, 0f, 0f, ForceMode.Acceleration);
            }
        }
    }
}

/*

ForceMode.Force : 연속적인 힘, 무게(o)

*/