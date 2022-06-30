using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIController : MonoBehaviour
{
    [SerializeField] private GameObject mainUIElement;

    [Header("Creature Management")]
    [SerializeField] private GameObject creatureManagementContent;

    private GameController gameController;
    private PlayerInventoryController playerInventoryController;
    private Animator mainUIElementAnimator;
    

    private void Awake()
    {
        gameController = FindObjectOfType<GameController>();
        playerInventoryController = FindObjectOfType<PlayerInventoryController>();
        mainUIElementAnimator = mainUIElement.GetComponent<Animator>();
    }

    private void Start()
    {
        LoadItems();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToogleMainUIElement();
            gameController.gameState = gameController.gameState == GameController.GameState.UI ?
                GameController.GameState.GAMEPLAY : GameController.GameState.UI;
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
    private void LoadItems()
    {
        creatureManagementContent.transform.GetChild(0).gameObject.SetActive(false);
        for (int i = 1; i < creatureManagementContent.transform.childCount; i++)
        {
            Destroy(creatureManagementContent.transform.GetChild(i).gameObject);
        }

        foreach(ItemStats item in playerInventoryController.items)
        {
            GameObject go = Instantiate(creatureManagementContent.transform.GetChild(0).gameObject, creatureManagementContent.transform);
            go.SetActive(true);

            go.transform.GetChild(0).GetComponent<TMP_Text>().SetText($"Life: +{item.lifeMod}");
            go.transform.GetChild(1).GetComponent<TMP_Text>().SetText($"Defense: +{item.defenseMod}");
            go.transform.GetChild(2).GetComponent<TMP_Text>().SetText($"Evasion: +{item.evasionMod}");
            go.transform.GetChild(3).GetComponent<TMP_Text>().SetText($"Damage: +{item.damageMod}");
            go.transform.GetChild(4).GetComponent<TMP_Text>().SetText($"Attack Speed: +{item.attackSpeedMod}");
        }
        //creatureManagementContent.transform.childCount
        //playerInventoryController.items;
    }
    #endregion
}
