using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemController : MonoBehaviour
{
    public string rarity;

    public int[] increments = new int[] {0, 0, 0, 0, 0};

    //Convert to dictionary:
    //index 0 -> lifeIncrement
    //index 1 -> damageIncrement
    //index 2 -> speedIncrement
    //index 3 -> defenseIncrement
    //index 4 -> evasionIncrement

    public void SetItemModifiers()
    {
        switch (rarity)
        {
            case "Normal":
                InsertModifiers(2);
                break;

            case "Rare":
                InsertModifiers(3);
                break;

            case "Legendary":
                InsertModifiers(4);
                break;
        }
    }

    void InsertModifiers(int numberModifiers)
    {
        List<int> modIndex = new List<int>();

        for (int i = 0; i < numberModifiers; i++)
        {
            int randomIndex = Random.Range(0, increments.Length - 1);
            while (RepeatedIndex(modIndex, randomIndex))
            {
                randomIndex = Random.Range(0, increments.Length - 1);
            }
            increments[randomIndex] += Random.Range(5, 15);
            modIndex.Add(randomIndex);
        }
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
            rarity = "Legendary";
            return;
        }

        else if (random > 5)
        {
            rarity = "Rare";
            return;
        }

        else
            rarity = "Normal";
    }
}
