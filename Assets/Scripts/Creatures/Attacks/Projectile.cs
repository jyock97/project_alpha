using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed;

    private bool _isMoving;
    private LayerMask _targetLayer;
    private float _damage;


    private Vector3 _targetPosition;
    private AnimationCurve xCurve;
    private AnimationCurve zCurve;
    private AnimationCurve yCurve;

    private float _distanceToTarget;
    private float _timeToTarget;
    private float _currentTime;

    private void Update()
    {
        if (_isMoving)
        {
            if (_currentTime < _timeToTarget)
            {
                _currentTime += Time.deltaTime;
            }
            else
            {
                _currentTime = _timeToTarget;
                Destroy(gameObject);
            }

            transform.position = new Vector3(xCurve.Evaluate(_currentTime), yCurve.Evaluate(_currentTime), zCurve.Evaluate(_currentTime));
        }
    }

    public void InitProjectile(Vector3 newTargetPosition, LayerMask newTargetLayer, float newDamage)
    {
        _targetPosition = newTargetPosition;
        _targetLayer = newTargetLayer;
        _damage = newDamage;
        _isMoving = true;

        _distanceToTarget = Vector3.Distance(transform.position, _targetPosition);
        _timeToTarget = _distanceToTarget / speed;

        xCurve = AnimationCurve.Linear(0, transform.position.x, _timeToTarget, _targetPosition.x);
        zCurve = AnimationCurve.Linear(0, transform.position.z, _timeToTarget, _targetPosition.z);
        yCurve = AnimationCurve.Linear(0, 0, 0, 0);
        yCurve.keys = new Keyframe[]
        {
            new Keyframe(0, transform.position.y, 0, Mathf.PI),
            new Keyframe(_timeToTarget/2, transform.position.y + 1),
            new Keyframe(_timeToTarget, _targetPosition.y, -2 * Mathf.PI, 0)
        };
    }

    private void OnTriggerEnter(Collider other)
    {
        if (1 << other.gameObject.layer == _targetLayer.value)
        {
            if (other.gameObject.activeSelf)
            {
                other.GetComponent<CreatureController>().DealtDamage(_damage);
                AudioClip hurt = other.GetComponent<CreatureController>().hurtSound;
                other.GetComponent<CreatureController>()._source.PlayOneShot(hurt);
            }
        }
    }
}
