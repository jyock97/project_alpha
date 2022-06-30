using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    float horizontal, vertical;
    Vector3 direction;
    [SerializeField] float speed;
    [SerializeField] float rotationSpeed;

    public bool inBattle = false;

    Rigidbody rb;
    public Animator anim;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");

        direction = new Vector3(horizontal, 0f, vertical).normalized;

        anim.SetFloat("speed", direction.magnitude);

        if (direction.magnitude > 0.1f)
            playerMovementControler();

        playerRotationControler();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            anim.Play("Victory");
        }
    }

    void playerMovementControler()
    {
        rb.velocity = transform.TransformDirection(new Vector3(0, 0, speed * direction.z));
    }

    void playerRotationControler()
    {
        if (horizontal < 0)
            transform.Rotate(-Vector3.up * rotationSpeed * Time.deltaTime);
        else if (horizontal > 0)
            transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
    }
}
