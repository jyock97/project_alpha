using System;
using System.Collections.Generic;
using UnityEngine;

public class CreaturesManager : MonoBehaviour
{
    [Serializable]
    public struct PlayerCreatureStt
    {
        public GameObject prefab;
        public PlayerCreature creatureStats;
    }

    public List<PlayerCreatureStt> playerCreaturesData;

    [HideInInspector] public List<CreatureController> enemyCreatures = new List<CreatureController>();
    [HideInInspector] public List<CreatureController> playerCreatures = new List<CreatureController>();

    private BattleStageController _battleStageController;
    private PlayerInventoryController _playerInventoryController;

    private void Awake()
    {
        _battleStageController = FindObjectOfType<BattleStageController>();
        _playerInventoryController = FindObjectOfType<PlayerInventoryController>();
    }

    public int InitPlayerCreatures()
    {
        foreach (CreatureController creature in playerCreatures)
        {
            Destroy(creature.gameObject);
        }
        playerCreatures.Clear();

        CreatureController creatureController = Instantiate(playerCreaturesData[0].prefab).GetComponent<CreatureController>();
        creatureController.SetStats(playerCreaturesData[0].creatureStats, _playerInventoryController.playerCreature1Item);
        playerCreatures.Add(creatureController);
        _battleStageController.SetCreature(BattleStageController.BattleStageFields.PlayerField, creatureController);

        creatureController = Instantiate(playerCreaturesData[1].prefab).GetComponent<CreatureController>();
        creatureController.SetStats(playerCreaturesData[1].creatureStats, _playerInventoryController.playerCreature2Item);
        playerCreatures.Add(creatureController);
        _battleStageController.SetCreature(BattleStageController.BattleStageFields.PlayerField, creatureController);

        // For the demo only 2 creatures will be used at all moment.
        return 2;
    }

    public void RemovePlayerCreature(CreatureController creatureController)
    {
        playerCreatures.Remove(creatureController);
    }

    public void RemoveEnemyCreature(CreatureController creatureController)
    {
        enemyCreatures.Remove(creatureController);
    }

    public void ClearEnemies()
    {
        foreach (CreatureController creature in enemyCreatures)
        {
            Destroy(creature.gameObject);
        }
        enemyCreatures.Clear();
    }
}
