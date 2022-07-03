using UnityEngine;

public class WorldCamera : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private Vector3 offset;

    private GameObject player;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void Update()
    {
        Vector3 newPosition = player.transform.position - (player.transform.forward * offset.z);
        newPosition.y += offset.y;
        transform.position = Vector3.Lerp(transform.position, newPosition, Time.deltaTime * speed);

        transform.GetChild(0).transform.rotation = Quaternion.LookRotation((player.transform.position - transform.position).normalized, transform.up);
    }
}
