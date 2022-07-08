using UnityEngine;
using UnityEngine.UI;

public class LifeBarController : MonoBehaviour
{
    [SerializeField] private Image image;

    private GameObject battleCamera;

    private float maxLife;
    private float currentLife;
    private void Update()
    {
        if(battleCamera != null)
        {
            Vector3 direction = (battleCamera.transform.position - transform.position).normalized;
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }

    public void Init(float maxLife)
    {
        this.maxLife = maxLife;
        image.fillAmount = 1;

        battleCamera = FindObjectOfType<CameraTransition>().battleCam;
    }

    public void SetLife(float newLife)
    {
        image.fillAmount = Mathf.Clamp(newLife / maxLife, 0, maxLife);
    }

}
