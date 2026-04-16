
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovementJump : MonoBehaviour
{
    [SerializeField]
    InputAction jump;

    [SerializeField]
    float jumpForce = 5f;

    Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        jump.Enable();
    }

    private void FixedUpdate(){
        if (jump.IsPressed())
        {
            if (gameObject.transform.position.y < 2.1)
            {
                rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            }
        }
    }

}