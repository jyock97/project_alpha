using System;
using System.Collections;
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

    public TypeField type;

    public float creatureStrength;

    public GameObject normalProjectilePrefab;
    public GameObject itemPrefab;
    public LayerMask targetLayerMask;

    [SerializeField] private GameObject lifeBar;
    private LifeBarController lifeBarController;

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
    [SerializeField] private Renderer _renderer;
    private bool _isDeath;
    private bool _isBehaving;
    private float _currentAttackTime;

    [SerializeField] Material[] creaturesTypeMaterial;
    [SerializeField] GameObject[] creatureProjectile;

    public bool isBoss;

    public AudioSource _source;
    [SerializeField] AudioClip attackSound;
    [SerializeField] AudioClip defeatSound;
    [SerializeField] public AudioClip hurtSound;

    private void Awake()
    {
        _gameController = FindObjectOfType<GameController>();
        _battleStageController = FindObjectOfType<BattleStageController>();
        _creaturesManager = FindObjectOfType<CreaturesManager>();
        _levelController = FindObjectOfType<LevelController>();
        lifeBarController = lifeBar.GetComponent<LifeBarController>();

        _animator = GetComponent<Animator>();
        _source = GetComponent<AudioSource>();

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
            _renderer.sharedMaterials[0] = creaturesTypeMaterial[3];

            normalProjectilePrefab = creatureProjectile[3];

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
        if (lifeBarController != null) lifeBarController.Init(life);
    }

    void insertTypeByNum(int i)
    {
        switch (i)
        {
            case 0:
                type = TypeField.Water;
                _renderer.material = creaturesTypeMaterial[i];
                normalProjectilePrefab = creatureProjectile[i];
                break;
            case 1:
                type = TypeField.Plant;
                _renderer.material = creaturesTypeMaterial[i];
                normalProjectilePrefab = creatureProjectile[i];
                break;
            case 2:
                type = TypeField.Earth;
                _renderer.material = creaturesTypeMaterial[i];
                normalProjectilePrefab = creatureProjectile[i];
                break;
        }
    }

    public void CalculateCreatureStrength()
    {
        creatureStrength = (life * defense * evasion) + (damage * attackSpeed);
    }


    public void SetStats(PlayerCreature stats, ItemStats statsModifiers)
    {
        life = stats.life + statsModifiers.lifeMod;
        defense = stats.defense + statsModifiers.defenseMod;
        evasion = stats.evasion + statsModifiers.evasionMod;
        damage = stats.damage + statsModifiers.damageMod;
        attackSpeed = stats.attackSpeed + statsModifiers.attackSpeedMod;

        if (lifeBarController != null) lifeBarController.Init(life);
    }

    public void StartBehaviour()
    {
        _isBehaving = true;
        UpdateAttackTime();
        lifeBar.SetActive(true);
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

            _source.spatialBlend = 0;
            _source.loop = false;
            _source.clip = attackSound;
            _source.Play();
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
        if (_gameController.isBattleEnd)
        {
            return;
        }

        life -= Mathf.Clamp(damageAmount - defense, 0, damageAmount);
        if (lifeBarController != null) lifeBarController.SetLife(life);

        if (life <= 0)
        {
            _source.spatialBlend = 0;
            _source.loop = false;
            _source.clip = defeatSound;
            _source.Play();

            Die();
        }
    }

    private void Die()
    {
        if (_isDeath)
        {
            return;
        }

        _isDeath = true;
        lifeBar.SetActive(false);
        if(itemPrefab != null)
        {
            GameObject go = Instantiate(itemPrefab);
            go.transform.position = transform.position;
        }

        if (field == BattleStageController.BattleStageFields.PlayerField)
        {
            _gameController.PlayerCreatureDefeated();
            _creaturesManager.RemovePlayerCreature(this);
            //Destroy(gameObject);
            StartCoroutine(DestroyGameObject());
        }
        else
        {
            _gameController.EnemyCreatureDefeated(this);
            _creaturesManager.RemoveEnemyCreature(this);

            if (!transform.parent)
                //Destroy(gameObject);
                StartCoroutine(DestroyGameObject());

            else
            {
                StartCoroutine(DesapearGameObject());
            }
        }
    }

    IEnumerator DestroyGameObject()
    {
        _renderer.enabled = false;

        _source.spatialBlend = 0;
        _source.loop = false;
        _source.clip = defeatSound;
        _source.Play();

        yield return new WaitForSeconds(3);

        Destroy(gameObject);
    }

    IEnumerator DesapearGameObject()
    {
        _renderer.enabled = false;

        _source.spatialBlend = 0;
        _source.loop = false;
        _source.clip = defeatSound;
        _source.Play();

        yield return new WaitForSeconds(3);

        gameObject.SetActive(false);
    }
}