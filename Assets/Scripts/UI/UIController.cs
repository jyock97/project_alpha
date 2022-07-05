using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [SerializeField] private GameObject mainUIElement;

    [Header("Creature Management")]
    [SerializeField] private GameObject creatureManagementContent;
    [SerializeField] private GameObject creatureManagementStats;

    private GameController gameController;
    private CreaturesManager creaturesManager;
    private PlayerInventoryController playerInventoryController;
    private Animator mainUIElementAnimator;

    private int currentSelectedCreature;
    private int currentCreatureOneItemIndex;
    private int currentCreatureTwoItemIndex;


    private void Awake()
    {
        gameController = FindObjectOfType<GameController>();
        creaturesManager = FindObjectOfType<CreaturesManager>();
        playerInventoryController = FindObjectOfType<PlayerInventoryController>();
        mainUIElementAnimator = (mainUIElement != null)? mainUIElement.GetComponent<Animator>() : null;
    }

    private void Start()
    {
        if (creatureManagementContent)
        {
            LoadItems();
            currentSelectedCreature = 0;
            currentCreatureOneItemIndex = -1;
            currentCreatureTwoItemIndex = -1;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (gameController.gameState == GameController.GameState.GAMEPLAY)
            {
                LoadItems();
                LoadCreatureStats();
                gameController.gameState = GameController.GameState.UI;
            }
            else
            {
                gameController.gameState = GameController.GameState.GAMEPLAY;
            }
            ToogleMainUIElement();
        }
    }

    public void ApplicationQuit()
    {
        Application.Quit();
    }

    public void ToogleMainUIElement()
    {
        mainUIElementAnimator.SetTrigger("flipUI");
    }


    #region Creature Management
    public void SelectCreature(int creatureIndex)
    {
        currentSelectedCreature = creatureIndex;
        LoadCreatureStats();
    }

    public void SelectItem(GameObject button)
    {
        int itemIndex = int.Parse(button.name);

        // PlayerInventory flip equiped items
        int prevItemIndex = currentSelectedCreature == 0 ? currentCreatureOneItemIndex : currentCreatureTwoItemIndex;
        
        ItemStats item;
        if (prevItemIndex > -1)
        {
            item = playerInventoryController.items[prevItemIndex];
            item.equipedCreature = -1;
            playerInventoryController.items[prevItemIndex] = item;
        }

        item = playerInventoryController.items[itemIndex];
        item.equipedCreature = currentSelectedCreature;
        playerInventoryController.items[itemIndex] = item;

        // Creature reset values
        CreatureController creatureController = creaturesManager.playerCreatures[currentSelectedCreature];
        creatureController.statsModifiers = item;
        creatureController.SetStats(creaturesManager.playerCreaturesData[currentSelectedCreature].creatureStats);

        LoadItems();
        LoadCreatureStats();
    }

    private void LoadItems()
    {
        creatureManagementContent.transform.GetChild(0).gameObject.SetActive(false);
        for (int i = 1; i < creatureManagementContent.transform.childCount; i++)
        {
            Destroy(creatureManagementContent.transform.GetChild(i).gameObject);
        }

        int index = 0;
        foreach (ItemStats item in playerInventoryController.items)
        {
            GameObject go = Instantiate(creatureManagementContent.transform.GetChild(0).gameObject, creatureManagementContent.transform);
            go.SetActive(true);
            go.name = index.ToString();

            if(item.equipedCreature == 0)
            {
                currentCreatureOneItemIndex = index;
            }
            if(item.equipedCreature == 1)
            {
                currentCreatureTwoItemIndex = index;
            }

            go.transform.GetChild(0).GetComponent<TMP_Text>().SetText($"Life: +{item.lifeMod}");
            go.transform.GetChild(1).GetComponent<TMP_Text>().SetText($"Defense: +{item.defenseMod}");
            go.transform.GetChild(2).GetComponent<TMP_Text>().SetText($"Evasion: +{item.evasionMod}");
            go.transform.GetChild(3).GetComponent<TMP_Text>().SetText($"Damage: +{item.damageMod}");
            go.transform.GetChild(4).GetComponent<TMP_Text>().SetText($"Attack Speed: +{item.attackSpeedMod}");
            
            index++;
        }
    }

    private void LoadCreatureStats()
    {
        CreatureController creatureController = creaturesManager.playerCreatures[currentSelectedCreature];

        SetUICreatureStat(0, creatureController.life.ToString(),
            creaturesManager.playerCreaturesData[currentSelectedCreature].creatureStats.life.ToString(),
            creatureController.statsModifiers.lifeMod.ToString());

        SetUICreatureStat(1, creatureController.defense.ToString(),
            creaturesManager.playerCreaturesData[currentSelectedCreature].creatureStats.defense.ToString(),
            creatureController.statsModifiers.defenseMod.ToString());

        SetUICreatureStat(2, creatureController.evasion.ToString(),
            creaturesManager.playerCreaturesData[currentSelectedCreature].creatureStats.evasion.ToString(),
            creatureController.statsModifiers.evasionMod.ToString());

        SetUICreatureStat(3, creatureController.damage.ToString(),
            creaturesManager.playerCreaturesData[currentSelectedCreature].creatureStats.damage.ToString(),
            creatureController.statsModifiers.damageMod.ToString());

        SetUICreatureStat(4, creatureController.attackSpeed.ToString(),
            creaturesManager.playerCreaturesData[currentSelectedCreature].creatureStats.attackSpeed.ToString(),
            creatureController.statsModifiers.attackSpeedMod.ToString());

    }
    private void SetUICreatureStat(int index, string totalVal, string defaultVal, string modValue)
    {
        creatureManagementStats.transform.GetChild(index).GetChild(0).GetChild(0) // Total Value
            .GetComponent<TMP_Text>().SetText($" {totalVal}");


        creatureManagementStats.transform.GetChild(index).GetChild(0).GetChild(0).GetChild(0) // Default Value
            .GetComponent<TMP_Text>().SetText($" ( {defaultVal}");

        creatureManagementStats.transform.GetChild(index).GetChild(0).GetChild(0).GetChild(0).GetChild(0) // Mod Value
            .GetComponent<TMP_Text>().SetText($" + {modValue}");
    }
    #endregion
}
