using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class CreatureController : MonoBehaviour
{
    public int currentPosition;
    public BattleStageController.BattleStageFields field;

    public float life;
    public float defense;
    public float evasion;

    public float damage;
    public float attackSpeed;
    
    public string type;

    public float levelStrength = 500;
    public float creatureStrength;

    public GameObject normalProjectilePrefab;
    public LayerMask targetLayerMask;

    private int splitValue = 12;
    private int tmLm = 13;
    private int tMLm = 17;
    private int tmDem = 2;
    private int tMDem = 3;
    private int tmDam = 3;
    private int tMDam = 5;

    private BattleStageController _battleStageController;
    private CreaturesManager _creaturesManager;
    private bool _isBehaving;
    private float _currentAttackTime;

    private void Start()
    {
        _battleStageController = FindObjectOfType<BattleStageController>();
        _creaturesManager = FindObjectOfType<CreaturesManager>();
    }
    
    private void Update()
    {
        if (_isBehaving)
        {
            if (Time.time > _currentAttackTime)
            {
                Attack();
                UpdateAttackTime();
            }
        }
    }

    public void InsertStatisticsValues()
    {
        type = "Dummy";

        float tmpVal1 = levelStrength / splitValue;
        float tmpVal2 = tmpVal1 * (splitValue - 1);
        life = Random.Range(tmpVal2 / tMLm, tmpVal2 / tmLm);

        float tmpVal3 = tmpVal2 / life;
        defense = (int) Random.Range(tmpVal3 / tMDem, tmpVal3 / tmDem);
        evasion = MathF.Round(tmpVal3 / defense, 3);
        damage = (int) Random.Range(tmpVal1 / tMDam, tmpVal1 / tmDam);
        attackSpeed = MathF.Round(tmpVal1 / damage, 3);

        CalculateCreatureStrenght();
    }

    public void CalculateCreatureStrenght()
    {
        creatureStrength = (life * defense * evasion) + (damage * attackSpeed);
        Debug.Log("Creature Strenght: " + creatureStrength);
    }

    
    public void SetStats(PlayerCreature stats)
    {
        life = stats.life;
        defense = stats.defense;
        evasion = stats.evasion;
        damage = stats.damage;
        attackSpeed = stats.attackSpeed;
    }

    public void StartBehaviour()
    {
        _isBehaving = true;
        UpdateAttackTime();
    }
    
    private void UpdateAttackTime()
    {
        _currentAttackTime = Time.time + 2 - attackSpeed / 100;
    }

    private void Attack()
    {
        List<CreatureController> creatureList;
        if (field == BattleStageController.BattleStageFields.PlayerField)
        {
            creatureList = _creaturesManager.enemyCreatures;
        }
        else
        {
            creatureList = _creaturesManager.playerCreatures;
        }

        float currentDistance = float.MaxValue;
        CreatureController currentCreature = creatureList[0];
        foreach (CreatureController creatureController in creatureList)
        {
            if (Vector3.Distance(transform.position, creatureController.transform.position) < currentDistance)
            {
                currentDistance = Vector3.Distance(transform.position, creatureController.transform.position);
                currentCreature = creatureController;
            }
        }
        
        GameObject go = Instantiate(normalProjectilePrefab);
        go.transform.position = transform.position;
        go.GetComponent<Projectile>().InitProjectile(currentCreature.transform.position, targetLayerMask, damage);
        currentCreature.SetTarget();
    }

    public void SetTarget()
    {
        if (Random.value < evasion / 100)
        {
            _battleStageController.MoveCreatureToRandomPosition(field, this);
        }
    }

    public void DealtDamage(float damageAmount)
    {
        life -= Mathf.Clamp(damageAmount - defense, 0, damageAmount);
        
        if (life <= 0)
        {
            //TODO change this destroy to a die anim and dont destroy it if Player Creature
            Destroy(gameObject);
        }
    }
}