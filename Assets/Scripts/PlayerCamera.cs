using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCamera : MonoBehaviour {
    // Using SerializeField to access the variables in the inspector
    [Header("Sensitivity")]
    [SerializeField] private float sensX;
    [SerializeField] private float sensY;

    [Header("References")]
    [SerializeField] private Transform orientation;

    [Header("Input")]
    [SerializeField] private InputAction look;

    private float xRotation;
    private float yRotation;

    private void OnEnable()
    {
        look.Enable();
    }

    private void OnDisable()
    {
        look.Disable();
    }
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        Vector2 lookInput = look.ReadValue<Vector2>();

        float mouseX = lookInput.x * sensX;
        float mouseY = lookInput.y * sensY;

        yRotation += mouseX;

        xRotation -= mouseY;

        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        orientation.rotation = Quaternion.Euler(0, yRotation, 0);
    }
}
