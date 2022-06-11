using UnityEngine;

public class CreatureController : MonoBehaviour
{
    public int currentPosition;

    public float life;
    public float defense;
    public float evasion;

    public float damage;
    public float attackSpeed;
    
    public string type;

    public float levelStrength = 500;
    public float creatureStrength;

    int splitValue = 12;
    int tmLm = 13;
    int tMLm = 17;
    int tmDem = 2;
    int tMDem = 3;
    int tmDam = 5;
    int tMDam = 7;

    public void InsertStatisticsValues()
    {
        type = "Dummy";

        float tmpVal1 = levelStrength / splitValue;
        float tmpVal2 = tmpVal1 * (splitValue - 1);
        life = Random.Range(tmpVal2 / tMLm, tmpVal2 / tmLm);

        float tmpVal3 = tmpVal2 / life;
        defense = (int) Random.Range(tmpVal3 / tMDem, tmpVal3 / tmDem);
        evasion = System.MathF.Round(tmpVal3 / defense, 3);
        damage = (int) Random.Range(tmpVal1 / tMDam, tmpVal1 / tmDam);
        attackSpeed = System.MathF.Round(tmpVal1 / damage, 3);

        CalculateCreatureStrenght();
    }

    public void CalculateCreatureStrenght()
    {
        creatureStrength = (life * defense * evasion) + (damage * attackSpeed);
        Debug.Log("Creature Strenght: " + creatureStrength);
    }
}