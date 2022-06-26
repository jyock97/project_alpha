using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ItemController : MonoBehaviour
{
    public ItemForms items;

    public string rarity; 

    public float lifeMod;
    public float defenseMod;
    public float evasionMod;

    public float damageMod;
    public float attackSpeedMod;


    private LevelController _levelController;
    private int splitValue = 12;
    private int tmLm = 13;
    private int tMLm = 17;
    private int tmDem = 2;
    private int tMDem = 3;
    private int tmDam = 3;
    private int tMDam = 5;

    private void Awake()
    {
        _levelController = FindObjectOfType<LevelController>();
    }

    private void Start()
    {
        int index = Random.Range(0, items.itemForm.Count);
        GameObject itemForm = items.itemForm[index];

        Instantiate(itemForm, transform);
        CalculateValues();
        SetRarity();
    }

    private void CalculateValues()
    {
        float tmpVal1 = _levelController.currentItemStrength / splitValue;
        float tmpVal2 = tmpVal1 * (splitValue - 1);
        lifeMod = Random.Range(tmpVal2 / tMLm, tmpVal2 / tmLm);

        float tmpVal3 = tmpVal2 / lifeMod;
        defenseMod = (int) Random.Range(tmpVal3 / tMDem, tmpVal3 / tmDem);
        evasionMod = MathF.Round(tmpVal3 / defenseMod, 3);
        damageMod = (int) Random.Range(tmpVal1 / tMDam, tmpVal1 / tmDam);
        attackSpeedMod = MathF.Round(tmpVal1 / damageMod, 3);
    }

    private void SetRarity()
    {
        List<float> l = new List<float> {lifeMod, defenseMod, evasionMod, damageMod, attackSpeedMod};

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
        lifeMod = l[0];
        defenseMod = l[1];
        evasionMod = l[2];
        damageMod = l[3];
        attackSpeedMod = l[4];
    }
}
