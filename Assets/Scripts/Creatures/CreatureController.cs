using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class CreatureController : MonoBehaviour
{
    public enum TypeField
    {
        Water,
        Plant, 
        Earth,
        Fire
    }

    public int currentPosition;
    public BattleStageController.BattleStageFields field;

    public float life;
    public float defense;
    public float evasion;

    public float damage;
    public float attackSpeed;

    //public string type;
    public TypeField type;

    public float creatureStrength;

    public ItemStats statsModifiers;

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
    private Animator _animator;
    private bool _isDeath;
    private bool _isBehaving;
    private float _currentAttackTime;

    //string[] creaturesTypes = { "Water", "Plant", "Earth" };

    public bool isBoss;

    private void Awake()
    {
        _gameController = FindObjectOfType<GameController>();
        _battleStageController = FindObjectOfType<BattleStageController>();
        _creaturesManager = FindObjectOfType<CreaturesManager>();
        _levelController = FindObjectOfType<LevelController>();
        _animator = GetComponent<Animator>();

        InsertStatisticsValues();
    }

    private void Update()
    {
        if (_isBehaving && !_isDeath)
        {
            if (Time.time > _currentAttackTime)
            {
                _animator.SetTrigger("Attack");
                UpdateAttackTime();
            }
        }
    }

    public void InsertStatisticsValues()
    {
        float tmpVal1;

        if (isBoss)
        {
            type = TypeField.Fire;

            tmpVal1 = _levelController.globalBossStrength / splitValue;
        }
        else
        {
            insertTypeByNum(Random.Range(0, 3));

            tmpVal1 = _levelController.globalCreatureStrength / splitValue;
        }
        
        
        float tmpVal2 = tmpVal1 * (splitValue - 1);
        life = Random.Range(tmpVal2 / tMLm, tmpVal2 / tmLm);

        float tmpVal3 = tmpVal2 / life;
        defense = (int)Random.Range(tmpVal3 / tMDem, tmpVal3 / tmDem);
        evasion = MathF.Round(tmpVal3 / defense, 3);
        damage = (int)Random.Range(tmpVal1 / tMDam, tmpVal1 / tmDam);
        attackSpeed = MathF.Round(tmpVal1 / damage, 3);

        CalculateCreatureStrength();
    }

    void insertTypeByNum(int i)
    {
        switch (i)
        {
            case 0:
                type = TypeField.Water;
                break;
            case 1:
                type = TypeField.Plant;
                break;
            case 2:
                type = TypeField.Earth;
                break;
        }
    }

    public void CalculateCreatureStrength()
    {
        creatureStrength = (life * defense * evasion) + (damage * attackSpeed);
    }


    public void SetStats(PlayerCreature stats)
    {
        life = stats.life + statsModifiers.lifeMod;
        defense = stats.defense + statsModifiers.defenseMod;
        evasion = stats.evasion + statsModifiers.evasionMod;
        damage = stats.damage + statsModifiers.damageMod;
        attackSpeed = stats.attackSpeed + statsModifiers.attackSpeedMod;
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

    public void Attack()
    {
        List<CreatureController> creatureList = field == BattleStageController.BattleStageFields.PlayerField ? _creaturesManager.enemyCreatures : _creaturesManager.playerCreatures;

        if(creatureList.Count > 0)
        {
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
            go.transform.position = transform.position + Vector3.up;
            go.GetComponent<Projectile>().InitProjectile(currentCreature.transform.position, targetLayerMask, damage);
            currentCreature.SetTarget();
        }
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

            if (!transform.parent)
                Destroy(gameObject);
            else
                gameObject.SetActive(false);
        }
    }
}