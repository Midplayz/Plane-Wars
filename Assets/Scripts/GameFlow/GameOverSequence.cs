using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class GameOverSequence : MonoBehaviour
{
    [field: Header("------ Game Over Sequence ------")]
    [field: SerializeField] GameObject gameOverScreen;
    [field: SerializeField] ResultManager resultManager;

    private void Start()
    {
        gameOverScreen.SetActive(false);

        GameManager.Instance.onGameOver += OnGameOver ;
    }

    private void OnGameOver()
    {
        resultManager.DecideResult();
        gameOverScreen.SetActive(true);
        foreach (GameObject plane in GameManager.Instance.allPlanesInGame)
        {
           Destroy(plane);
        }
    }
}
