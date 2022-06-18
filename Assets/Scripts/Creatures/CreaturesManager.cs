using System;
using System.Collections.Generic;
using UnityEngine;

public class CreaturesManager : MonoBehaviour
{
    [Serializable]
    struct PlayerCreatureStt
    {
        public GameObject prefab;
        public PlayerCreature creatureStats;
    }

    [SerializeField] private List<PlayerCreatureStt> playerCreaturesData;
    
    public List<CreatureController> enemyCreatures = new List<CreatureController>();
    public List<CreatureController> playerCreatures = new List<CreatureController>();

    //test
    public GameObject enemyPrefab;
    //test
    private void Start()
    {
        //test
        enemyCreatures.Add(Instantiate(enemyPrefab).GetComponent<CreatureController>());
        enemyCreatures.Add(Instantiate(enemyPrefab).GetComponent<CreatureController>());
        //test
    }

    public void InitPlayerCreatures()
    {
        if (playerCreatures.Count == 0)
        {
            CreatureController creatureController = Instantiate(playerCreaturesData[0].prefab).GetComponent<CreatureController>();
            creatureController.SetStats(playerCreaturesData[0].creatureStats);
            playerCreatures.Add(creatureController);
            creatureController = Instantiate(playerCreaturesData[1].prefab).GetComponent<CreatureController>();
            creatureController.SetStats(playerCreaturesData[1].creatureStats);
            playerCreatures.Add(creatureController);
        }
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
