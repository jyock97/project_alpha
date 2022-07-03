using UnityEngine;

public class BattleCamera : MonoBehaviour
{
    private GameObject battleStage;

    private void Awake()
    {
        battleStage = GameObject.FindGameObjectWithTag("BattleStage");
    }

    private void Update()
    {
        transform.GetChild(0).transform.rotation = Quaternion.LookRotation((battleStage.transform.position - transform.position).normalized, transform.up);
    }
}
