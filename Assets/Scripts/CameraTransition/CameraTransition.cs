using UnityEngine;

public class CameraTransition : MonoBehaviour
{
    public GameObject battleCam;
    [SerializeField] GameObject worldCam;

    [SerializeField] GameObject canvas;

    private void Start()
    {
        worldCam.SetActive(true);
        battleCam.SetActive(false);
    }

    public void FipBlackout()
    {
        canvas.GetComponent<Animator>().SetTrigger("ChangeTransitionState");
    }

    public void FlipCameras()
    {
        worldCam.SetActive(!worldCam.activeSelf);
        battleCam.SetActive(!worldCam.activeSelf);
    }
}
