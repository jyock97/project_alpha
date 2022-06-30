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
    
    public float creatureStrength;
    
    public GameObject normalProjectilePrefab;
    public GameObject itemPrefab;
    public LayerMask targetLayerMask;

    private int splitValue = 12;
    private int tmLm = 13;
    private int tMLm = 17;
    private int tmDem = 2;
    private int tMDem = 3;
    private int tmDam = 3;
    private int tMDam = 5;

    private GameController _gameController;
    private BattleStageController _battleStageController;
    private CreaturesManager _creaturesManager;
    private LevelController _levelController;
    private bool _isDeath;
    private bool _isBehaving;
    private float _currentAttackTime;

    string[] creaturesTypes = { "Water", "Plant", "Earth" };

    public bool isBoss;

     private void Awake()
    {
        _gameController = FindObjectOfType<GameController>();
        _battleStageController = FindObjectOfType<BattleStageController>();
        _creaturesManager = FindObjectOfType<CreaturesManager>();
        _levelController = FindObjectOfType<LevelController>();

        InsertStatisticsValues();
    }
    
    private void Update()
    {
        if (_isBehaving && !_isDeath)
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
        float tmpVal1;

        if (isBoss)
        {
            type = "Fire";

            tmpVal1 = _levelController.globalBossStrength / splitValue;
        }
        else
        {
            int randomType = Random.Range(0, 2);
            type = creaturesTypes[randomType];

            tmpVal1 = _levelController.globalCreatureStrength / splitValue;
        }
        
        
        float tmpVal2 = tmpVal1 * (splitValue - 1);
        life = Random.Range(tmpVal2 / tMLm, tmpVal2 / tmLm);

        float tmpVal3 = tmpVal2 / life;
        defense = (int) Random.Range(tmpVal3 / tMDem, tmpVal3 / tmDem);
        evasion = MathF.Round(tmpVal3 / defense, 3);
        damage = (int) Random.Range(tmpVal1 / tMDam, tmpVal1 / tmDam);
        attackSpeed = MathF.Round(tmpVal1 / damage, 3);

        CalculateCreatureStrength();
    }

    public void CalculateCreatureStrength()
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

    public void EndBehaviour()
    {
        _isBehaving = false;
    }
    
    private void UpdateAttackTime()
    {
        _currentAttackTime = Time.time + 2 - attackSpeed / 100;
    }

    private void Attack()
    {
        List<CreatureController> creatureList = field == BattleStageController.BattleStageFields.PlayerField ? _creaturesManager.enemyCreatures : _creaturesManager.playerCreatures;

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
            Die();
        }
    }

    private void Die()
    {
        _isDeath = true;
        GameObject go = Instantiate(itemPrefab);
        go.transform.position = transform.position;
        
        if (field == BattleStageController.BattleStageFields.PlayerField)
        {
            _gameController.PlayerCreatureDefeated();
        }
        else
        {
            _gameController.EnemyCreatureDefeated();
            _creaturesManager.RemoveEnemyCreature(this);
            Destroy(gameObject);
        }
    }
}