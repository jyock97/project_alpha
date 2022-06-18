using UnityEngine;

public class GameController : MonoBehaviour
{
    private CreaturesManager _creaturesManager;
    private void Start()
    {
        _creaturesManager = FindObjectOfType<CreaturesManager>();
        _creaturesManager.InitPlayerCreatures();
    }

    private void Update()
    {
        //TODO this is just for testing, remove it later when creatures spawn itselfs
        if (Input.GetKeyDown(KeyCode.Space))
        {
            BattleStageController battleStageController = FindObjectOfType<BattleStageController>();
            foreach (CreatureController creatureController in _creaturesManager.playerCreatures)
            {
                creatureController.InsertStatisticsValues();
                battleStageController.SetCreature(BattleStageController.BattleStageFields.PlayerField, creatureController);
                creatureController.StartBehaviour();
            }
            foreach (CreatureController creatureController in _creaturesManager.enemyCreatures)
            {
                creatureController.InsertStatisticsValues();
                battleStageController.SetCreature(BattleStageController.BattleStageFields.EnemyField, creatureController);
                creatureController.StartBehaviour();
            }
        }
    }
}
