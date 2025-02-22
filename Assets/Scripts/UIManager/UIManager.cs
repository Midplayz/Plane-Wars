using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [field: Header("------- Main Menu UI Manager -------")]
    [field: SerializeField] private GameObject mainMenuCanvas;
    [field: SerializeField] private GameObject settingsCanvas;
    [field: SerializeField] private GameObject profileCanvas;
    [field: SerializeField] private GameObject usernamePromptCanvas;
    [field: SerializeField] private GameObject matchMakingSceneUI;
    [field: SerializeField] private GameObject removeADsButton;
    [field: SerializeField] private TextMeshProUGUI coinsAmount;
    [field: SerializeField] private List<GameObject> mainMenuItems;

    [field: Header("------- Script References -------")]
    private SettingsManager settingsManager;
    private ProfileManager profileManager;


    [field: HideInInspector] public bool isOpen = false;

    private void Start()
    {
        coinsAmount.text = UserDataHandler.instance.ReturnSavedValues().currency.ToString();
        settingsManager = GetComponent<SettingsManager>();
        profileManager = GetComponent<ProfileManager>();
        Time.timeScale = 1.0f;
        mainMenuCanvas.SetActive(true);
        settingsCanvas.SetActive(false);
        profileCanvas.SetActive(false);
        if(PurchaseTrackerDataHandler.instance.ReturnSavedValues().hasPurchasedAdBlock)
        {
            removeADsButton.SetActive(false);
        }
        else
        {
            removeADsButton.SetActive(true);
        }
    }
    public void SettingsUIControl()
    {
        if (!isOpen)
        {
            settingsCanvas.SetActive(!isOpen);
            settingsManager.UpdateIcons();
            isOpen = true;
            MainMenuContentController();
        }
        else if (isOpen)
        {
            settingsCanvas.SetActive(!isOpen);
            isOpen = false;
            MainMenuContentController();
        }
    }

    public void ProfileUIControl()
    {
        if (!isOpen)
        {
            profileManager.SetPlayerStats();
            profileCanvas.SetActive(!isOpen);
            isOpen = true;
            MainMenuContentController();
        }
        else if (isOpen)
        {
            profileCanvas.SetActive(!isOpen);
            isOpen = false;
            MainMenuContentController();
        }
    }

    public void OpenLink(string urlLink)
    {
        Application.OpenURL(urlLink);
    }

    public void UserNamePromptUIControl()
    {
        if (!isOpen)
        {
            usernamePromptCanvas.SetActive(!isOpen);
            isOpen = true;
            MainMenuContentController();
            Debug.Log("Is Open");
        }
        else if (isOpen)
        {

            usernamePromptCanvas.SetActive(!isOpen);
            isOpen = false;
            MainMenuContentController();
            Debug.Log("Isnt Open");
        }
    }

    public void MainMenuContentController()
    {
        for(int i = 0; i < mainMenuItems.Count; i ++) 
        {
            mainMenuItems[i].SetActive(!mainMenuItems[i].activeInHierarchy);
        }
    }

    public void OnPurchaseOfRemoveADs()
    {
        PurchaseTrackerDataHandler.instance.ReturnSavedValues().hasPurchasedAdBlock = true;
        PurchaseTrackerDataHandler.instance.SaveUserData();
        removeADsButton.SetActive(false);
    }

    public void StartMatchMaking()
    {
        for (int i = 0; i < mainMenuItems.Count; i++)
        {
            mainMenuItems[i].SetActive(false);
        }
        matchMakingSceneUI.SetActive(true);
    }
}
