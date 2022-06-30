using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureSpawner : MonoBehaviour
{
    [SerializeField] private GameObject creature;
    List<GameObject> creatureList = new List<GameObject>();
    bool canGenerate = true;
    [SerializeField] float timeToGenerate;

    void Start()
    {
        StartCoroutine(waitToGenerate());
    }

    void Update()
    {
        if (creatureList.Count < 1)
            canGenerate = true;
    }

    IEnumerator waitToGenerate()
    {
        if(canGenerate)
        {
            yield return new WaitForSeconds(timeToGenerate);
            generateCreature();
            canGenerate = false;
        }
        else
            yield return new WaitForSeconds(0);
        
        StartCoroutine(waitToGenerate());
    }

    private void generateCreature()
    {
        GameObject go = Instantiate(creature, transform.position, transform.rotation);
        go.SetActive(true);
        //CreatureController creatureController = go.GetComponent<CreatureController>();
        //creatureController.InsertStatisticsValues();
        creatureList.Add(go);
    }

    void deleteCreature()
    {
        Destroy(creatureList[0].gameObject);
        creatureList = new List<GameObject>();
    }
}
