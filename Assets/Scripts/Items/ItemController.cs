using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public struct ItemStats
{
    public float lifeMod;
    public float defenseMod;
    public float evasionMod;

    public float damageMod;
    public float attackSpeedMod;
}
public class ItemController : MonoBehaviour
{
    public float flightToPlayerTime;
    
    public ItemForms items;

    public string rarity; 

    public ItemStats stats;

    private LevelController _levelController;
    private GameObject _player;
    private PlayerInventoryController _playerInventoryController;
    private int splitValue = 12;
    private int tmLm = 13;
    private int tMLm = 17;
    private int tmDem = 2;
    private int tMDem = 3;
    private int tmDam = 3;
    private int tMDam = 5;

    private float _flightToPlayerTimeCurrent;

    private void Awake()
    {
        _player = GameObject.FindWithTag("FakePlayerPosition");
        _levelController = FindObjectOfType<LevelController>();
        _playerInventoryController = FindObjectOfType<PlayerInventoryController>();
    }

    private void Start()
    {
        int index = Random.Range(0, items.itemForm.Count);
        GameObject itemForm = items.itemForm[index];

        Instantiate(itemForm, transform);
        CalculateValues();
        SetRarity();

        _flightToPlayerTimeCurrent = Time.time + flightToPlayerTime;
    }

    private void Update()
    {
        if (Time.time > _flightToPlayerTimeCurrent)
        {
            transform.position = Vector3.MoveTowards(transform.position, _player.transform.position, Time.deltaTime * 4);
        }

        if (Vector3.Distance(transform.position, _player.transform.position) < 0.01)
        {
            _playerInventoryController.AddItem(stats);
            Destroy(gameObject);
        }
    }


    private void CalculateValues()
    {
        float tmpVal1 = _levelController.currentItemStrength / splitValue;
        float tmpVal2 = tmpVal1 * (splitValue - 1);
        stats.lifeMod = Random.Range(tmpVal2 / tMLm, tmpVal2 / tmLm);

        float tmpVal3 = tmpVal2 / stats.lifeMod;
        stats.defenseMod = (int) Random.Range(tmpVal3 / tMDem, tmpVal3 / tmDem);
        stats.evasionMod = MathF.Round(tmpVal3 / stats.defenseMod, 3);
        stats.damageMod = (int) Random.Range(tmpVal1 / tMDam, tmpVal1 / tmDam);
        stats.attackSpeedMod = MathF.Round(tmpVal1 / stats.damageMod, 3);
    }

    private void SetRarity()
    {
        List<float> l = new List<float> {stats.lifeMod, stats.defenseMod, stats.evasionMod, stats.damageMod, stats.attackSpeedMod};

        // calculate rarity
        int valuesToClear;
        float randomValue = Random.value;
        if (randomValue < 0.1)
        {
            rarity = "Legendary";
            valuesToClear = 1;
        } else if (randomValue < 0.25)
        {
            rarity = "Rare";
            valuesToClear = 2;
        }
        else
        {
            rarity = "Normal";
            valuesToClear = 3;
        }

        // Clear values
        while (valuesToClear != 0)
        {
            int randomIndex = Random.Range(0, l.Count);
            if (l[randomIndex] <= 0.01)
            {
                continue;
            }

            l[randomIndex] = 0;
            
            valuesToClear--;
        }
        
        // Recover values
        stats.lifeMod = l[0];
        stats.defenseMod = l[1];
        stats.evasionMod = l[2];
        stats.damageMod = l[3];
        stats.attackSpeedMod = l[4];
        stats.attackSpeedMod = l[4];
        stats.attackSpeedMod = l[4];
        stats.attackSpeedMod = l[4];
        stats.attackSpeedMod = l[4];
        stats.attackSpeedMod = l[4];
        stats.attackSpeedMod = l[4];
    }
}
