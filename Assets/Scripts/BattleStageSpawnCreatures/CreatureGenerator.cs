using UnityEngine;

public class CreatureGenerator : MonoBehaviour
{
    [SerializeField] private GameObject creature;
    
    private BattleStageController _bsController;
    private CreaturesManager _creaturesManager;

    private void Awake()
    {
        _bsController = FindObjectOfType<BattleStageController>();
        _creaturesManager = FindObjectOfType<CreaturesManager>();
    }

    public int GenerateCreatures()
    {
        _creaturesManager.ClearEnemies();
        int creaturesToSpawn = Random.Range(2, 3);
        
        for (int i = 0; i < creaturesToSpawn; i++)
        {
            CreateEnemyCreature();
        }

        return creaturesToSpawn;
    }

    private void CreateEnemyCreature()
    {
        GameObject go = Instantiate(creature);
        go.SetActive(true);
        CreatureController creatureController = go.GetComponent<CreatureController>();
        creatureController.InsertStatisticsValues();
        _bsController.SetCreature(BattleStageController.BattleStageFields.EnemyField, creatureController);
        _creaturesManager.enemyCreatures.Add(creatureController);
    }
}