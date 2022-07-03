using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CreatureSpawner : MonoBehaviour
{
    [SerializeField] private GameObject creature;
    public List<GameObject> creatureList = new List<GameObject>();
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
        if (canGenerate)
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
        creatureList.Add(go);
        go.transform.SetParent(this.gameObject.transform);
        go.GetComponent<NavMeshAgent>().enabled = true;
    }

    public void deleteCreature()
    {
        Destroy(creatureList[0].gameObject);
        creatureList = new List<GameObject>();
    }
}
