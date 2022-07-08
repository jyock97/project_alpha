using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public enum GameState
    {
        UI,
        GAMEPLAY
    }

    public GameState gameState;
    public bool isBattleEnd;

    [SerializeField] private float blackoutWaitTime;
    [SerializeField] private GameObject bossPrefab;

    private CreaturesManager _creaturesManager;
    private CameraTransition _cameraTransition;
    private BattleStageController _battleStageController;
    private CreatureGenerator _creatureGenerator;
    private LevelController _levelController;
    private int _battleCurrentPlayerCreatures;
    private int _battleCurrentEnemyCreatures;

    GameObject player;
    GameObject fakePlayer;
    private bool isBossDefeated;

    public GameObject world;
    AudioClip victoryMusic;
    AudioClip defeatMusic;
    AudioClip levelMusic;

    private void Awake()
    {
        world = GameObject.Find("World");
        victoryMusic = world.GetComponent<MusicController>().victory;
        defeatMusic = world.GetComponent<MusicController>().defeat;
        levelMusic = world.GetComponent<MusicController>().theme;

        _creaturesManager = FindObjectOfType<CreaturesManager>();
        _cameraTransition = FindObjectOfType<CameraTransition>();
        _battleStageController = FindObjectOfType<BattleStageController>();
        _creatureGenerator = FindObjectOfType<CreatureGenerator>();
        _levelController = FindObjectOfType<LevelController>();

        player = GameObject.Find("Player");
        fakePlayer = GameObject.Find("FakePlayer");
    }

    private void Start()
    {
        gameState = GameState.GAMEPLAY;
        _battleStageController.InitializeBattleStage();
        _creaturesManager.InitPlayerCreatures();

        isBattleEnd = true;
    }

    public void StartBattle(GameObject preCreature)
    {
        _creatureGenerator.previousCreature = preCreature;
        StartCoroutine(IE_StartBattle());
    }

    private IEnumerator IE_StartBattle()
    {
        if (isBattleEnd)
        {
            isBossDefeated = false;
            isBattleEnd = false;

            // turn off player input
            // camera transition black
            _cameraTransition.FipBlackout();
            yield return new WaitForSeconds(blackoutWaitTime);

            // reset BattleStage free spaces
            _battleStageController.ResetFreeSpace();
            // set player creatures
            _battleCurrentPlayerCreatures = _creaturesManager.InitPlayerCreatures();
            // spawn enemies
            _battleCurrentEnemyCreatures = _creatureGenerator.GenerateCreatures();
            // camera transition 
            _cameraTransition.FlipCameras();
            _cameraTransition.FipBlackout();
            // simulate battle
            foreach (CreatureController creatureController in _creaturesManager.playerCreatures)
            {
                creatureController.StartBehaviour();
            }
            foreach (CreatureController creatureController in _creaturesManager.enemyCreatures)
            {
                creatureController.StartBehaviour();
            }
        }
    }

    public void PlayerCreatureDefeated()
    {
        _battleCurrentPlayerCreatures--;
        if (_battleCurrentPlayerCreatures <= 0)
        {
            StartCoroutine(EndBattle());
            world.GetComponent<AudioSource>().Stop();
            world.GetComponent<AudioSource>().PlayOneShot(defeatMusic);
            player.GetComponent<Movement>().anim.SetInteger("battleWon", 0);
            player.GetComponent<Movement>().fakePlayer.GetComponent<Animator>().SetInteger("battleWon", 0);
        }
    }

    public void EnemyCreatureDefeated(CreatureController creature)
    {
        _battleCurrentEnemyCreatures--;
        _levelController.UpdateItemStrength();


        if (creature.isBoss)
        {
            isBossDefeated = true;
        }
        if (_battleCurrentEnemyCreatures <= 0)
        {
            StartCoroutine(EndBattle());
            world.GetComponent<AudioSource>().Stop();
            world.GetComponent<AudioSource>().PlayOneShot(victoryMusic);
            player.GetComponent<Movement>().anim.SetInteger("battleWon", 1);
            player.GetComponent<Movement>().fakePlayer.GetComponent<Animator>().SetInteger("battleWon", 1);
        }
    }


    private IEnumerator EndBattle()
    {
        if (!isBattleEnd)
        {
            isBattleEnd = true;
            // stop creature behaviour
            foreach (CreatureController creatureController in _creaturesManager.playerCreatures)
            {
                creatureController.EndBehaviour();
            }
            foreach (CreatureController creatureController in _creaturesManager.enemyCreatures)
            {
                creatureController.EndBehaviour();
            }

            // collect Items
            yield return new WaitForSeconds(4); // TODO make this a variable value

            //take off player of the battle
            player.GetComponent<Movement>().inBattle = false;
            player.GetComponent<Movement>().anim.SetInteger("battleWon", -1);
            player.GetComponent<Movement>().fakePlayer.GetComponent<Animator>().SetInteger("battleWon", -1);

            player.GetComponent<AudioListener>().enabled = false;
            fakePlayer.GetComponent<AudioListener>().enabled = true;

            world.GetComponent<AudioSource>().clip = levelMusic;
            world.GetComponent<AudioSource>().Play();

            // camera transition black
            _cameraTransition.FipBlackout();
            yield return new WaitForSeconds(blackoutWaitTime);

            if (isBossDefeated)
            {
                SceneManager.LoadScene("EndScene");
                StopAllCoroutines();
            }
            else
            {
                // clear remaining creatures;
                _creaturesManager.ClearEnemies();
                _creaturesManager.InitPlayerCreatures();

                // camera transition 
                _cameraTransition.FlipCameras();
                _cameraTransition.FipBlackout();

                // Reset boss
                StartCoroutine(ReSpawnBoss());

                if (!_creatureGenerator.previousCreature.GetComponent<CreatureController>().isBoss)
                {
                    GameObject previousSpawner = _creatureGenerator.previousCreature.GetComponent<CreatureIA>().parent;
                    if (previousSpawner.GetComponent<CreatureSpawner>())
                        previousSpawner.GetComponent<CreatureSpawner>().deleteCreature();
                }
            }
        }
    }

    private IEnumerator ReSpawnBoss()
    {
        if(GameObject.FindGameObjectWithTag("Boss") == null)
        {
            yield return new WaitForSeconds(3);
            GameObject go = Instantiate(bossPrefab);
            go.transform.position = new Vector3(-80, 8, 364);
            go.GetComponent<CreatureIA>().enabled = true;
            go.GetComponent<NavMeshAgent>().enabled = true;
        }
    }
}
