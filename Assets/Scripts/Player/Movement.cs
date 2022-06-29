using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    float horizontal, vertical;
    Vector3 direction;
    [SerializeField] float speed;
    [SerializeField] float rotationSpeed;
    [SerializeField] float jumpForce;

    [SerializeField] float turnSmoothVelocity;

    Rigidbody rb;
    CharacterController controller;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");

        direction = new Vector3(horizontal, 0f, vertical).normalized;

        if (direction.magnitude > 0.1f)
        {
            playerMovementControler();
        }

        
        //playerRotationControler();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            playerJumpController();
        }
    }

    void playerMovementControler()
    {
        rb.velocity = transform.TransformDirection(new Vector3(0, 0, speed * vertical));
        //controller.Move(direction * speed * Time.deltaTime);

        float turnSmoothVelocity;

        float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
        //float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref 0.1f, turnSmoothVelocity);
    }

    void playerRotationControler()
    {
        if (horizontal < 0)
            transform.Rotate(-Vector3.up * rotationSpeed * Time.deltaTime);
        else if (horizontal > 0)
            transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
    }

    void playerJumpController()
    {
        rb.AddForce(new Vector3(0, jumpForce, 0), ForceMode.Impulse);
    }
}
