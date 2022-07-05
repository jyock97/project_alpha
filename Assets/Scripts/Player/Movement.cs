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
    [SerializeField] float stepSmooth = 0.1f;

    public GameObject fakePlayer;

    [SerializeField] GameObject groundBoxDetectionPosition;
    [SerializeField] Vector3 groundBoxDetectionSize;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();

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

            playerMovementControler();

            playerRotationControler();

            if (direction.z > 0f)
            {
                stepClimb();
            }

            Fall();
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
        RaycastHit hitLower, hitUpper;
        Physics.Raycast(stepRayLower.transform.position, transform.forward, out hitLower, 0.3f, ~LayerMask.GetMask("Player"));
        if (hitLower.collider != null)
        {
            Physics.Raycast(stepRayUpper.transform.position, transform.forward, out hitUpper, 0.3f, ~LayerMask.GetMask("Player"));
            if (hitUpper.collider == null)
            {
                rb.position += new Vector3(0, stepSmooth, 0);
            }
        }
    }

    void Fall()
    {
        var hits = Physics.BoxCastAll(groundBoxDetectionPosition.transform.position, groundBoxDetectionSize, -transform.up, Quaternion.identity, 0.1f, ~LayerMask.GetMask("Player"));
        if (hits.Length == 0)
        {
            rb.position -= new Vector3(0, stepSmooth*2, 0);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawCube(groundBoxDetectionPosition.transform.position, groundBoxDetectionSize*2);
    }
}
