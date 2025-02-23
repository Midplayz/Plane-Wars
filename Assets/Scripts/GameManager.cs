using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [field: Header("------ Actions ------")]
    public Action onGameOver, onMatchFinished;

    [field: Header("------ Planes Pursuit ------")]
    public List<GameObject> allPlanesInGame;
    [field: SerializeField] GameObject planesHolder;

    [field: Header("------ Planes Pursuit ------")]
    public bool ChangingScenes = false;

    private void Awake()
    {
        Instance = this;
        GetPlaneCount();
        Application.targetFrameRate = 60;
    }

    public void GetPlaneCount()
    {
        for (int i = 0; i < planesHolder.transform.childCount; i++)
        {
            allPlanesInGame.Add(planesHolder.transform.GetChild(i).gameObject);
        }
    }
}
