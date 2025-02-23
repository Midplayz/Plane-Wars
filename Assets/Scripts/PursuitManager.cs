using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class PursuitManager : MonoBehaviour
{
    [field: Header("------ Pursuit Values ------")]
    [field: SerializeField] int maxTimeToChangeTarget = 5;
    [field: SerializeField, Range(1, 100)] int probabilityOfGainingPursuitTarget = 50;
    [field: SerializeField, Range(1, 100)] int probabilityOfPursuingPlayer = 50;

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
            if (GameManager.Instance.allPlanesInGame[i] == null)
            {
                break;
            }
            if (GameManager.Instance.allPlanesInGame[i].TryGetComponent<PlaneMovement>(out PlaneMovement planemovement))
            {
                continue;
            }
            if(Random.Range(0,101) >= (100 - probabilityOfGainingPursuitTarget))
            {
                    GameManager.Instance.allPlanesInGame[i].GetComponent<OpponentAIScript>().target = GameManager.Instance.allPlanesInGame[GetRandomTarget(i)];
            }
            else
            {
                GameManager.Instance.allPlanesInGame[i].GetComponent<OpponentAIScript>().target = null;
            }
        }
    }

    private int GetRandomTarget(int currentPlanePos)
    {
        int targettedPlane = Random.Range(0, GameManager.Instance.allPlanesInGame.Count);
        while(targettedPlane == currentPlanePos) 
        {
            targettedPlane = Random.Range(0, GameManager.Instance.allPlanesInGame.Count);
            if (GameManager.Instance.allPlanesInGame[targettedPlane].TryGetComponent<PlaneMovement>(out PlaneMovement planeMovement))
            {
                if (Random.Range(0, 101) >= (100 - probabilityOfPursuingPlayer))
                {
                    targettedPlane = Random.Range(0, GameManager.Instance.allPlanesInGame.Count);
                }
            }
        }
        return targettedPlane;
    }
}
