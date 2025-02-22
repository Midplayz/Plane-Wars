using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UsernamePromptManager : MonoBehaviour
{
    [field: Header("Username Pop Up Manager")]
    [field: SerializeField] private UIManager mainMenuUIManager;
    [field: SerializeField] private Button confirmButton;
    [field: SerializeField] private TMP_InputField inputField;
    void Start()
    {
        mainMenuUIManager = GetComponent<UIManager>();
        confirmButton.interactable = false;
        if (string.IsNullOrEmpty(UserDataHandler.instance.ReturnSavedValues().username))
        {
            mainMenuUIManager.UserNamePromptUIControl();
        }
    }

    public void EnableConfirmButton()
    {
        if(!string.IsNullOrWhiteSpace(inputField.text) && inputField.text.Length >= 5)
        {
            confirmButton.interactable = true;
        }
        else
        {
            confirmButton.interactable = false;
        }
    }
    public void OnConfirm()
    {
        UserDataHandler.instance.ReturnSavedValues().username = inputField.text;
        UserDataHandler.instance.SaveUserData();
        mainMenuUIManager.UserNamePromptUIControl();
    }
}
