using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTransition : MonoBehaviour
{
    [SerializeField] List<Camera> camList = new List<Camera>();
    [SerializeField] GameObject canvas;
    int contIndex = 0;
    [SerializeField] int timeToStop;

    void Start()
    {
        camList[contIndex].enabled = true;

        for (int i = 1; i < camList.Count; i++)
        {
            camList[i].enabled = false;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(StartCameraTransition());
        }
    }


    IEnumerator StartCameraTransition()
    {
        Animator animator = canvas.GetComponent<Animator>();
        animator.SetTrigger("ChangeTransitionState");

        float timeOfAnimation = animator.runtimeAnimatorController.animationClips[1].length;

        yield return new WaitForSeconds(timeToStop/2 + timeOfAnimation);
        camList[contIndex].enabled = false;
        MoveCameraIndex();
        camList[contIndex].enabled = true;
        yield return new WaitForSeconds(timeToStop / 2 + timeOfAnimation);

        canvas.GetComponent<Animator>().SetTrigger("ChangeTransitionState");
    }

    void MoveCameraIndex()
    {
        if (contIndex >= camList.Count - 1)
            contIndex = 0;
        else
            contIndex++;
    }
}
