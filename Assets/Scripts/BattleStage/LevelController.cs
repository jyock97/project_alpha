using UnityEngine;

public class LevelController : MonoBehaviour
{
    public float globalCreatureStrength;
    public float globalBossStrength;
    public float currentItemStrength;
    public float minItemStrength;
    public float maxItemStrength;
    public float itemLevelIncreaseRatio;

    private void Start()
    {
        currentItemStrength = minItemStrength;
    }
}
