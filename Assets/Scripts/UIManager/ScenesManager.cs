using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine;
using UnityEditor;

public class ScenesManager : MonoBehaviour
{
    public void ButtonClicked(Button button)
    {
        ChangeScene(button.gameObject.GetComponent<ButtonConfig>().scene);
    }

    public void ChangeScene(string scene)
    {
        if(SceneManager.GetActiveScene().name == "MainGame")
        {
            GameManager.Instance.ChangingScenes = true;
        }
        SceneManager.LoadScene(scene);
    }
}
