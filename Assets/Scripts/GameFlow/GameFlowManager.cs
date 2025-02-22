using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class GameFlowManager : MonoBehaviour
{
    [field: Header("------ Main Game Values ------")]
    [field: SerializeField] float gameDuration = 45f;
    [field: SerializeField] TextMeshProUGUI timerText;
    bool startTimer = false;
    float timer = 0;

    private void Start()
    {
        startTimer = true;
        timer = gameDuration;
        timerText.text = Mathf.RoundToInt(timer).ToString();
        OnGameStart();
    }

    //private void Update()
    //{
    //    if(startTimer)
    //    {
    //        timer -= Time.deltaTime;
    //        timerText.text = Mathf.RoundToInt(timer).ToString();
    //        if(timer <= 0)
    //        {
    //            timerText.text = "0";
    //            GameManager.Instance.onGameOver?.Invoke();
    //            startTimer = false;
    //        }
    //    }
    //}
    private async void OnGameStart()
    {
        for(int i = 0; i < GameManager.Instance.allPlanesInGame.Count; i++)
        {
            GameManager.Instance.allPlanesInGame[i].GetComponent<Collider>().enabled = false;
            if (GameManager.Instance.allPlanesInGame[i].TryGetComponent<PlaneMovement>(out PlaneMovement plane))
            {
                GameManager.Instance.allPlanesInGame[i].GetComponent<PlaneMovement>().shootBullets = false;
            }
            else
            {
                //GameManager.Instance.allPlanesInGame[i].GetComponent<OpponentAIScript>().shootBullets = false;
            }
        }

        await Task.Delay(2000);

        for (int i = 0; i < GameManager.Instance.allPlanesInGame.Count; i++)
        {
            GameManager.Instance.allPlanesInGame[i].GetComponent<Collider>().enabled = true;
            if (GameManager.Instance.allPlanesInGame[i].TryGetComponent<PlaneMovement>(out PlaneMovement plane))
            {
                GameManager.Instance.allPlanesInGame[i].GetComponent<PlaneMovement>().shootBullets = true;
            }
            else
            {
               // GameManager.Instance.allPlanesInGame[i].GetComponent<OpponentAIScript>().shootBullets = true;
            }
        }
    }
}
