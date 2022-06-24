using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ItemController : MonoBehaviour
{
    public ItemForms items;

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
    
    public void SetItemModifiers()
    {
//        switch (rarity)
//        {
//            case "Normal":
//                InsertModifiers(2);
//                break;
//
//            case "Rare":
//                InsertModifiers(3);
//                break;
//
//            case "Legendary":
//                InsertModifiers(4);
//                break;
//        }
    }

    void InsertModifiers(int numberModifiers)
    {
//        List<int> modIndex = new List<int>();
//
//        for (int i = 0; i < numberModifiers; i++)
//        {
//            int randomIndex = Random.Range(0, increments.Length - 1);
//            while (RepeatedIndex(modIndex, randomIndex))
//            {
//                randomIndex = Random.Range(0, increments.Length - 1);
//            }
//            increments[randomIndex] += Random.Range(5, 15);
//            modIndex.Add(randomIndex);
//        }
    }

    bool RepeatedIndex(List<int> indexList, int newIndex)
    {
        for (int i = 0; i < indexList.Count; i++)
        {
            if (indexList[i] == newIndex)
                return true;
        }
        return false;
    }

    public void SetRarity()
    {
        int random = Random.Range(0, 10);

        if (random > 9)
        {
//            rarity = "Legendary";
            return;
        }

        else if (random > 5)
        {
//            rarity = "Rare";
            return;
        }

//        else
//            rarity = "Normal";
    }
}
