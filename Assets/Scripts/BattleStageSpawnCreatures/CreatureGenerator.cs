using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureGenerator : MonoBehaviour
{
    BattleStageController bsController;
    [SerializeField] private GameObject Creature;
    [SerializeField] private GameObject Escort;
    private List<CreatureController> testList = new List<CreatureController>();
    void Start()
    {
        InicializeCriatures();
    }

    void InicializeCriatures()
    {
        bsController = GameObject.Find("BattleStage").GetComponent<BattleStageController>();
        bsController.InicializeBattleStage();
        InicializeCreature();
    }

    public void InicializeCreature()
    {
        GameObject go = Instantiate(Creature);
        go.SetActive(true);
        CreatureController creatureController = go.AddComponent<CreatureController>();

        creatureController.InsertLevelStrength(GameObject.Find("LevelController").GetComponent<LevelController>().levelPoint);
        creatureController.InsertStatisticsValues();
        testList.Add(creatureController);
        bsController.SetCreature(BattleStageController.BattleStageFields.PlayerField, creatureController);

        for (int i = 0; i <= Random.Range(0, 2); i++)
        {
            InicializeEscortCreature();
        }
    }

    public void InicializeEscortCreature()
    {
        GameObject go = Instantiate(Creature);
        go.SetActive(true);
        CreatureController creatureController = go.AddComponent<CreatureController>();

        creatureController.InsertLevelStrength(GameObject.Find("LevelController").GetComponent<LevelController>().levelPoint);
        creatureController.InsertStatisticsValues();
        testList.Add(creatureController);
        bsController.SetCreature(BattleStageController.BattleStageFields.PlayerField, creatureController);
    }
}