using UnityEngine;

public class GameController : MonoBehaviour
{
    private CreaturesManager _creaturesManager;
    private int _battleCurrentPlayerCreatures;
    private int _battleCurrentEnemyCreatures;
    
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
            _battleCurrentPlayerCreatures = 2;
            _battleCurrentEnemyCreatures = 2;
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

    public void PlayerCreatureDefeated()
    {
        _battleCurrentPlayerCreatures--;
        if (_battleCurrentPlayerCreatures <= 0)
        {
            BattleEnds();
        }
    }
    
    public void EnemyCreatureDefeated()
    {
        _battleCurrentEnemyCreatures--;
        if (_battleCurrentEnemyCreatures <= 0)
        {
            BattleEnds();
        }
    }


    private void BattleEnds()
    {
        foreach (CreatureController creatureController in _creaturesManager.playerCreatures)
        {
            creatureController.EndBehaviour();
        }
        foreach (CreatureController creatureController in _creaturesManager.enemyCreatures)
        {
            creatureController.EndBehaviour();
        }
        Debug.Log("Battle Ends");
    }
}
