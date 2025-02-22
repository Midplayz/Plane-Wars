using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class PursuitManager : MonoBehaviour
{
    [field: Header("------ Pursuit Values ------")]
    [field: SerializeField] int maxTimeToChangeTarget = 5;

    //Private Variables
    float currentMaxTime;
    float timer = 0f;

    private void Start()
    {
        currentMaxTime = Random.Range(0, maxTimeToChangeTarget+1);
        ChangeTargets();       
    }

    private void Update()
    {
        timer += Time.deltaTime;

        if(timer > currentMaxTime)
        {
            ChangeTargets();
            currentMaxTime = Random.Range(0, maxTimeToChangeTarget+1);
            timer = 0f;
        }
    }

    private void ChangeTargets()
    {
        for (int i = 0; i < GameManager.Instance.allPlanesInGame.Count; i++)
        {
            if (TryGetComponent<OpponentAIScript>(out OpponentAIScript AIScript))
            {
                GameManager.Instance.allPlanesInGame[i].GetComponent<OpponentAIScript>().target = GameManager.Instance.allPlanesInGame[GetRandomTarget(i)];
                continue;
            }
            else if (TryGetComponent<OpponentML>(out OpponentML oppoML))
            {
                GameManager.Instance.allPlanesInGame[i].GetComponent<OpponentML>().UpdateTarget(GameManager.Instance.allPlanesInGame[GetRandomTarget(i)]);
                continue;
            }
            else
            {
                print("No Object");
            }
        }
    }

    private int GetRandomTarget(int currentPlanePos)
    {
        int targettedPlane = Random.Range(0, GameManager.Instance.allPlanesInGame.Count);
        while(targettedPlane == currentPlanePos) 
        {
            targettedPlane = Random.Range(0, GameManager.Instance.allPlanesInGame.Count);
        }
        return targettedPlane;
    }
}
