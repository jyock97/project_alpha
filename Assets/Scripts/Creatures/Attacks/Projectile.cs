using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed;

    private bool _isMoving;
    private Vector3 _targetPosition;
    private LayerMask _targetLayer;
    private float _damage;

    private void Update()
    {
        if (_isMoving)
        {
            Vector3 newPosition = transform.position;
            newPosition = Vector3.MoveTowards(newPosition, _targetPosition, Time.deltaTime * speed);
            transform.position = newPosition;

            if (Vector3.Distance(transform.position, _targetPosition) < 0.01f)
            {
                Destroy(gameObject);
            }
        }
    }

    public void InitProjectile(Vector3 newTargetPosition, LayerMask newTargetLayer, float newDamage)
    {
        _targetPosition = newTargetPosition;
        _targetLayer = newTargetLayer;
        _damage = newDamage;
        _isMoving = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (1 << other.gameObject.layer - 1 == _targetLayer.value)
        {
            other.GetComponent<CreatureController>().DealtDamage(_damage);
        }
    }
}
