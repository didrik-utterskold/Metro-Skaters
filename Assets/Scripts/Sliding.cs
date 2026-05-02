//LEGACY SLIDING SCRIPT, DELETE ONCE SLIDING FROM MOVEMENT FEELS GOOD

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sliding : MonoBehaviour
{
    [Header("References")]
    public Transform orientation;
    public Transform playerObj;
    private Rigidbody rb;
    private PlayerMovement pm;

    [Header("Sliding")]
    public float slideForce;

    public float slideFriction;

    public float minSlideSpeed;

    public float slideYScale;
    private float StartYScale;

    [Header("Input")]
    public KeyCode slideKey = KeyCode.LeftControl;
    private float horizontalInput;
    private float verticalInput;

    public bool sliding;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        pm = GetComponent<PlayerMovement>();

        StartYScale = playerObj.localScale.y;
    }

    private void Update()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");


        if(Input.GetKeyDown(slideKey) && (horizontalInput != 0 || verticalInput != 0))
            StartSlide();

        if (Input.GetKeyUp(slideKey) && sliding)
            StopSlide();    

    }

    private void FixedUpdate()
    {
        if(sliding)
            SlidingMovement();
    }
    private void StartSlide()
    {
        sliding = true;
        playerObj.localScale = new Vector3(playerObj.localScale.x, slideYScale, playerObj.localScale.z);
        rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
        Vector3 inputDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
         rb.AddForce(inputDirection.normalized * slideForce, ForceMode.Impulse);

    }

    private void SlidingMovement()
    {
        Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        if (flatVel.magnitude < minSlideSpeed)
            StopSlide();
        else if (flatVel.magnitude > 0.1f)
            rb.AddForce(-flatVel.normalized * slideFriction, ForceMode.Force);
    }


    private void StopSlide()
    {
        sliding = false;
        playerObj.localScale = new Vector3(playerObj.localScale.x, StartYScale, playerObj.localScale.z);
    }

}

