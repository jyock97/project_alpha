using System.Collections.Generic;
using UnityEngine;

public class PlayerInventoryController : MonoBehaviour
{
    public List<ItemStats> _items;

    public void AddItem(ItemStats item)
    {
        _items.Add(item);
    }

    private void SortItems()
    {
        _items.Sort((item1, item2) =>
        {
            float a = 1;
            if (item1.lifeMod > 0) a *= item1.lifeMod;
            if (item1.defenseMod > 0) a *= item1.defenseMod;
            if (item1.evasionMod > 0) a *= item1.evasionMod;
            float b = 1;
            if (item1.damageMod > 0) b *= item1.damageMod;
            if (item1.attackSpeedMod > 0) b *= item1.attackSpeedMod;
            float item1TotalValue = a + b;
            
            a = 1;
            if (item2.lifeMod > 0) a *= item2.lifeMod;
            if (item2.defenseMod > 0) a *= item2.defenseMod;
            if (item2.evasionMod > 0) a *= item2.evasionMod;
            b = 1;
            if (item2.damageMod > 0) b *= item2.damageMod;
            if (item2.attackSpeedMod > 0) b *= item2.attackSpeedMod;
            float item2TotalValue = a + b;
            
            if (item1TotalValue < item2TotalValue)
                return 1;
            if (item1TotalValue > item2TotalValue)
                return -1;
            
            return 0;
        });
    }
}
