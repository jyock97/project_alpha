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

    [SerializeField] GameObject stepRayUpper;
    [SerializeField] GameObject stepRayLower;
    [SerializeField] float stepHeight = 0.3f;
    [SerializeField] float stepSmooth = 0.1f;

    public GameObject fakePlayer;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();

        stepRayUpper.transform.position = new Vector3(stepRayUpper.transform.position.x, stepHeight, stepRayUpper.transform.position.z);

        fakePlayer = GameObject.FindGameObjectWithTag("FakePlayerPosition");
    }

    void Update()
    {
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");

        direction = new Vector3(horizontal, 0f, vertical).normalized;

        if (!inBattle)
        {
            anim.SetFloat("speed", direction.magnitude);

            if (direction.magnitude > 0.1f)
                playerMovementControler();

            playerRotationControler();

            stepClimb();
        }
    }

    void playerMovementControler()
    {
        rb.velocity = transform.TransformDirection(new Vector3(0, rb.velocity.y, speed * direction.z));
    }

    void playerRotationControler()
    {
        if (horizontal < 0)
            transform.Rotate(-Vector3.up * rotationSpeed * Time.deltaTime);
        else if (horizontal > 0)
            transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
    }

    void stepClimb()
    {
        RaycastHit hitLower;

        if (Physics.Raycast(stepRayLower.transform.position, transform.TransformDirection(Vector3.forward), out hitLower, 0.1f))
        {
            RaycastHit hitUpper;

            if (!Physics.Raycast(stepRayUpper.transform.position, transform.TransformDirection(Vector3.forward), out hitUpper, 0.2f))
            {
                rb.position -= new Vector3(0f, -stepSmooth, 0f);
            }
        }
    }
}
