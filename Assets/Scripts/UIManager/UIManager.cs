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
    [field: SerializeField] private GameObject LoadingScreenUICanvas;
    [field: SerializeField] private GameObject MissionsUICanvas;
    [field: SerializeField] private GameObject shopUICanvas;
    [field: SerializeField] private GameObject matchMakingSceneUICanvas;

    [field: SerializeField] private GameObject noInternetPanel;
    
    [field: SerializeField] private List<TextMeshProUGUI> coinsAmount;
    [field: SerializeField] private List<GameObject> mainMenuItems;

    [field: Header("------- Script References -------")]
    private SettingsManager settingsManager;
    private ProfileManager profileManager;
    [field: SerializeField] private ShopManager shopManager;


    [field: HideInInspector] public bool isOpen = false;

    private void OnEnable()
    {
        InternetConnectivityChecker.Instance.IsDisconnectedFromInternet += () => { noInternetPanel.SetActive(true); Time.timeScale = 0; };
    }

    private void OnDisable()
    {
        InternetConnectivityChecker.Instance.IsDisconnectedFromInternet -= () => { noInternetPanel.SetActive(true); Time.timeScale = 0; };
    }

    private void Start()
    {
        UpdateCurrency();
        settingsManager = GetComponent<SettingsManager>();
        profileManager = GetComponent<ProfileManager>();
        Time.timeScale = 1.0f;
        mainMenuCanvas.SetActive(true);
        LoadingScreenUICanvas.SetActive(true);

        shopUICanvas.SetActive(false);
        MissionsUICanvas.SetActive(false);
        matchMakingSceneUICanvas.SetActive(false);
        settingsCanvas.SetActive(false);
        profileCanvas.SetActive(false);
        noInternetPanel.SetActive(false);
    }
    public void SettingsUIControl()
    {
        if (!isOpen)
        {
            settingsCanvas.SetActive(!isOpen);
            settingsManager.UpdateIcons();
            isOpen = true;
        }
        else if (isOpen)
        {
            settingsCanvas.SetActive(!isOpen);
            isOpen = false;
        }
    }

    public void ShopUIControl()
    {
        if (!isOpen)
        {
            shopUICanvas.SetActive(!isOpen);
            mainMenuCanvas.SetActive(isOpen);
            shopManager.UpdateActionButton();
            isOpen = true;        
        }
        else if (isOpen)
        {
            shopUICanvas.SetActive(!isOpen);
            mainMenuCanvas.SetActive(isOpen);
            isOpen = false;        }
    }

    public void ProfileUIControl()
    {
        if (!isOpen)
        {
            profileManager.SetPlayerStats();
            profileCanvas.SetActive(!isOpen);
            isOpen = true;
        }
        else if (isOpen)
        {
            profileCanvas.SetActive(!isOpen);
            isOpen = false;        }
    }

    public void MissionsUIControl()
    {
        if (!isOpen)
        {
            MissionsUICanvas.SetActive(!isOpen);
            isOpen = true;        }
        else if (isOpen)
        {
            MissionsUICanvas.SetActive(!isOpen);
            isOpen = false;        }
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
            Debug.Log("Is Open");
        }
        else if (isOpen)
        {

            usernamePromptCanvas.SetActive(!isOpen);
            isOpen = false;
            Debug.Log("Isnt Open");
        }
    }

    public void MainMenuContentController()
    {
        for (int i = 0; i < mainMenuItems.Count; i++)
        {
            mainMenuItems[i].SetActive(!mainMenuItems[i].activeInHierarchy);
        }
    }
   
    public void StartMatchMaking()
    {
        for (int i = 0; i < mainMenuItems.Count; i++)
        {
            mainMenuItems[i].SetActive(false);
        }
        matchMakingSceneUICanvas.SetActive(true);
    }
    public void UpdateCurrency()
    {
        for (int i = 0; i < coinsAmount.Count; i++)
        {
            coinsAmount[i].text = CurrencyDataHandler.instance.ReturnSavedValues().amountOfCoins.ToString();
        }
    }
}
