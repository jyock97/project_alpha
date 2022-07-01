using UnityEngine;
using UnityEngine.AI;

public class CreatureGenerator : MonoBehaviour
{
    [SerializeField] private GameObject creature;

    public GameObject previousCreature;

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

        CreateEnemyCreature(previousCreature);

        int creaturesToSpawn = Random.Range(1, 2);
        
        for (int i = 0; i < creaturesToSpawn; i++)
        {
            CreateEnemyCreature();
        }

        return creaturesToSpawn + 1;
    }

    private void CreateEnemyCreature(GameObject prevCreature)
    {
        GameObject go = prevCreature;
        go.SetActive(true);

        CreatureIA creatureIA = go.GetComponent<CreatureIA>();
        creatureIA.enabled = false;
        go.GetComponent<NavMeshAgent>().enabled = false;

        CreatureController creatureController = prevCreature.GetComponent<CreatureController>();
        creatureController.enabled = true;

        _bsController.SetCreature(BattleStageController.BattleStageFields.EnemyField, creatureController);
        _creaturesManager.enemyCreatures.Add(creatureController);
    }

    private void CreateEnemyCreature()
    {
        GameObject go = Instantiate(creature);
        go.SetActive(true);

        CreatureIA creatureIA = go.GetComponent<CreatureIA>();
        creatureIA.enabled = false;
        go.GetComponent<NavMeshAgent>().enabled = false;

        CreatureController creatureController = go.GetComponent<CreatureController>();
        creatureController.enabled = true;

        _bsController.SetCreature(BattleStageController.BattleStageFields.EnemyField, creatureController);
        _creaturesManager.enemyCreatures.Add(creatureController);
    }
}