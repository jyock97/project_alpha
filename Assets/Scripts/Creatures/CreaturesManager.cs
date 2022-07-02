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

    private void Awake()
    {
        _battleStageController = FindObjectOfType<BattleStageController>();
    }

    public void InitPlayerCreatures()
    {
        if (playerCreatures.Count == 0)
        {
            CreatureController creatureController = Instantiate(playerCreaturesData[0].prefab).GetComponent<CreatureController>();
            creatureController.SetStats(playerCreaturesData[0].creatureStats);
            playerCreatures.Add(creatureController);
            _battleStageController.SetCreature(BattleStageController.BattleStageFields.PlayerField, creatureController);

            creatureController = Instantiate(playerCreaturesData[1].prefab).GetComponent<CreatureController>();
            creatureController.SetStats(playerCreaturesData[1].creatureStats);
            playerCreatures.Add(creatureController);
            _battleStageController.SetCreature(BattleStageController.BattleStageFields.PlayerField, creatureController);
        }
        else
        {
            playerCreatures[0].SetStats(playerCreaturesData[0].creatureStats);
            playerCreatures[1].SetStats(playerCreaturesData[1].creatureStats);
            _battleStageController.MoveCreatureToRandomPosition(BattleStageController.BattleStageFields.PlayerField, playerCreatures[0]);
            _battleStageController.MoveCreatureToRandomPosition(BattleStageController.BattleStageFields.PlayerField, playerCreatures[1]);
        }
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
