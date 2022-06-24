using UnityEngine;

[CreateAssetMenu(fileName = "Test")]
public class PlayerCreature : ScriptableObject
{
    public float life;
    public float defense;
    public float evasion;

    public float damage;
    public float attackSpeed;
}
