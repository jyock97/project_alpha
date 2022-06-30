using System.Collections.Generic;
using UnityEngine;

public class BattleStageController : MonoBehaviour
{
    public enum BattleStageFields
    {
        PlayerField,
        EnemyField
    }

    private readonly int FieldSize = 3;

    [SerializeField] private GameObject square;
    [SerializeField] float xPadding;
    [SerializeField] float zPadding;

    private GameObject _playerField;
    private GameObject[] _playerFieldPositions;
    private List<int> _playerFieldFreePositions;
    private GameObject _enemyField;
    private GameObject[] _enemyFieldPositions;
    private List<int> _enemyFieldFreePositions;

    public void InitializeBattleStage()
    {
        _playerFieldPositions = new GameObject[FieldSize * FieldSize];
        _playerFieldFreePositions = new List<int>();
        _enemyFieldPositions = new GameObject[FieldSize * FieldSize];
        _enemyFieldFreePositions = new List<int>();

        _playerField = new GameObject("PlayerField");
        _playerField.transform.SetParent(transform);
        _playerField.transform.localPosition = Vector3.zero;
        _enemyField = new GameObject("EnemyField");
        _enemyField.transform.SetParent(transform);
        _enemyField.transform.localPosition = new Vector3(-2f, 0, FieldSize - 1 + (FieldSize - 1) * zPadding);
        _enemyField.transform.rotation = Quaternion.Euler(0, 180, 0);

        for (int i = 0; i < FieldSize; i++)
        {
            for (int j = 0; j < FieldSize; j++)
            {
                _playerFieldPositions[i * FieldSize + j] = Instantiate(square, _playerField.transform);
                _playerFieldPositions[i * FieldSize + j].SetActive(true);
                _playerFieldPositions[i * FieldSize + j].transform.localPosition = new Vector3(i + i * xPadding, 0, j + j * zPadding);
                _playerFieldFreePositions.Add(i * FieldSize + j);

                _enemyFieldPositions[i * FieldSize + j] = Instantiate(square, _enemyField.transform);
                _enemyFieldPositions[i * FieldSize + j].SetActive(true);
                _enemyFieldPositions[i * FieldSize + j].transform.localPosition = new Vector3(i + i * xPadding, 0, j + j * zPadding);
                _enemyFieldFreePositions.Add(i * FieldSize + j);
            }
        }
    }

    public void SetCreature(BattleStageFields field, CreatureController creature)
    {
        MoveCreatureToRandomPosition(field, creature, false);
    }

    public void MoveCreatureToRandomPosition(BattleStageFields field, CreatureController creature, bool saveCreatureLastPosition = true)
    {
        List<int> selectedFieldFreePositions;
        GameObject[] selectedFieldPositions;
        switch (field)
        {
            case BattleStageFields.PlayerField:
                selectedFieldFreePositions = _playerFieldFreePositions;
                selectedFieldPositions = _playerFieldPositions;
                break;
            case BattleStageFields.EnemyField:
            default:
                selectedFieldFreePositions = _enemyFieldFreePositions;
                selectedFieldPositions = _enemyFieldPositions;
                break;
        }

        creature.field = field;
        MoveCreatureToRandomPosition(selectedFieldFreePositions, selectedFieldPositions, creature, saveCreatureLastPosition);
    }

    private void MoveCreatureToRandomPosition(
        List<int> selectedFieldFreePositions, GameObject[] selectedFieldPositions,
        CreatureController creature, bool saveCreatureLastPosition = true)
    {
        if (selectedFieldFreePositions.Count > 0)
        {
            int index = Random.Range(0, selectedFieldFreePositions.Count);
            int freePosition = selectedFieldFreePositions[index];
            selectedFieldFreePositions.RemoveAt(index);
            if (saveCreatureLastPosition) selectedFieldFreePositions.Add(creature.currentPosition);

            creature.currentPosition = freePosition;
            Debug.Log("Real position: " + selectedFieldPositions[freePosition].transform.position);
            creature.transform.position = selectedFieldPositions[freePosition].transform.position;
        }
    }
}
