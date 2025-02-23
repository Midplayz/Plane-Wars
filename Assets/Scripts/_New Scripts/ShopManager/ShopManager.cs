using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    [field: Header("Shop Manager")]
    public List<SkinsInfo> infoOnSkins;
    [field: SerializeField] private Button actionButton;
    [field: SerializeField] private AudioSource audioSource;
    [field: SerializeField] private AudioClip clip;
    private int currentSkin = 0;

    [field: Header("Action Button Sprites")]
    [field: SerializeField] private Sprite purchaseSkin;
    [field: SerializeField] private Sprite equipSkin;
    [field: SerializeField] private Sprite equippedSkin;
    [field: SerializeField] private GameObject purchasableText;
    [field: SerializeField] private GameObject purchasedText;

    [field: Header("Confirm Purchase")]
    [field: SerializeField] private GameObject confirmPurchasePanel;
    [field: SerializeField] private TextMeshProUGUI descriptionText;

    [field: Header("Ad Panel")]
    [field: SerializeField] private GameObject watchADsPanel;
    [field: SerializeField] private Button watchADsButton;

    [field: Header("Page Indicator")]
    [field: SerializeField] private List<GameObject> circles;
    [field: SerializeField] private GameObject targetForInstantiating;
    [field: SerializeField] private GameObject circlePrefab;
    [field: SerializeField] private Vector2 smallerCircleScale;
    [field: SerializeField] private Sprite smallerCircleImage;
    [field: SerializeField] private Color smallerCircleColour;
    [field: SerializeField] private Vector2 largerCircleScale;
    [field: SerializeField] private Sprite largerCircleImage;
    [field: SerializeField] private Color largerCircleColour;

    [field: Header("Swipe Input")]
    [field: SerializeField] private GameObject shopPanel;
    [field: SerializeField] private float minSwipeDistance = 50f;
    [field: SerializeField] private float swipeThreshold = 1f;
    private Vector2 fingerDownPosition;
    private Vector2 fingerUpPosition;

    public List<Vector3> scalesOfSkins;

    bool transitioning = false;

    float zPositionOriginal;
    bool canSwipe = true;

    private void Start()
    {
        for (int i = 0; i < infoOnSkins.Count; i++)
        {
            if (SkinsOwnershipDataHandler.instance.ReturnSavedValues().skinsInfo[i].isEquipped == true)
            {
                infoOnSkins[i].isEquipped = true;
                infoOnSkins[i].skinObject.SetActive(true);
                currentSkin = i;
                UpdateActionButton();
            }
            else
            {
                infoOnSkins[i].isEquipped = false;
            }
        }

        for (int i = 0; i < infoOnSkins.Count; i++)
        {
            infoOnSkins[i].isOwned = SkinsOwnershipDataHandler.instance.ReturnSavedValues().skinsInfo[i].isOwned;
        }

        for (int i = 0; i < infoOnSkins.Count; i++)
        {
            scalesOfSkins.Add(infoOnSkins[i].skinObject.transform.localScale);
        }

        InstantiateDots();
        confirmPurchasePanel.SetActive(false);
        watchADsPanel.SetActive(false);
    }

    private void Update()
    {
        if (canSwipe && SceneManager.GetActiveScene().name == "MainMenu" && Input.touchCount > 0 && shopPanel.activeInHierarchy)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                fingerDownPosition = touch.position;
                fingerUpPosition = touch.position;
            }

            if (touch.phase == TouchPhase.Ended)
            {
                fingerUpPosition = touch.position;
                CheckSwipe();
            }
        }
    }

    private void CheckSwipe()
    {
        if (Vector2.Distance(fingerDownPosition, fingerUpPosition) >= minSwipeDistance)
        {
            float deltaX = fingerUpPosition.x - fingerDownPosition.x;
            float deltaY = fingerUpPosition.y - fingerDownPosition.y;

            if (Mathf.Abs(deltaX) > Mathf.Abs(deltaY) * swipeThreshold)
            {
                if (deltaX > 0)
                {
                    //Right
                    ScrollThroughSkins(false);
                }
                else
                {
                    //Left
                    ScrollThroughSkins(true);
                }
            }
        }
    }

    private void ScrollThroughSkins(bool idkman)
    {
        int previousSkin = currentSkin;
        if (idkman == true)
        {
            currentSkin++;
            if (currentSkin > infoOnSkins.Count - 1)
            {
                currentSkin = 0;
            }
        }
        else if (idkman == false)
        {
            currentSkin--;
            if (currentSkin < 0)
            {
                currentSkin = infoOnSkins.Count - 1;
            }
        }
        ScrollingEffect(previousSkin);
        ChangeDotsSprites();
    }

    private void ScrollingEffect(int previousSkin)
    {
        if (!SettingsDataHandler.instance.ReturnSavedValues().soundMuted)
        {
            audioSource.clip = clip;
            audioSource.Play();
        }
        float transitionDuration = 0.1f;
        float targetScaleOfPreviousObject = 0.2f;
        Vector3 initialScaleOfPreviousObject = scalesOfSkins[previousSkin];
        float initialScaleOfCurrentObject = 0f;
        Vector3 targetScaleOfCurrentObject = scalesOfSkins[currentSkin];
        StartCoroutine(ShrinkMenuItem(previousSkin, targetScaleOfPreviousObject, transitionDuration, initialScaleOfPreviousObject, currentSkin, targetScaleOfCurrentObject, initialScaleOfCurrentObject));
    }

    private IEnumerator ShrinkMenuItem(int indexOfShrinkingObject, float targetScale, float transitionDuration, Vector3 startScale, int indexOfGrowingObject, Vector3 growingTargetScale, float growingStartScale)
    {
        transitioning = true;

        float elapsedTime = 0.0f;

        while (elapsedTime < transitionDuration)
        {
            float t = elapsedTime / transitionDuration;
            float scale = Mathf.Lerp(startScale.x, targetScale, t);
            infoOnSkins[indexOfShrinkingObject].skinObject.transform.localScale = new Vector3(scale, scale, scale);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        infoOnSkins[indexOfShrinkingObject].skinObject.transform.localScale = new Vector3(targetScale, targetScale, targetScale);
        transitioning = false;
        infoOnSkins[indexOfShrinkingObject].skinObject.SetActive(false);
        infoOnSkins[indexOfGrowingObject].skinObject.SetActive(true);
        StartCoroutine(GrowMenuItem(indexOfGrowingObject, growingTargetScale, transitionDuration, growingStartScale));
    }

    private IEnumerator GrowMenuItem(int indexOfGrowingObject, Vector3 targetScale, float transitionDuration, float startScale)
    {
        transitioning = true;

        float elapsedTime = 0.0f;

        while (elapsedTime < transitionDuration)
        {
            float t = elapsedTime / transitionDuration;
            float scale = Mathf.Lerp(startScale, targetScale.x, t);
            infoOnSkins[indexOfGrowingObject].skinObject.transform.localScale = new Vector3(scale, scale, scale);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        infoOnSkins[indexOfGrowingObject].skinObject.transform.localScale = targetScale;
        transitioning = false;
        UpdateActionButton();
    }

    public void UpdateActionButton()
    {
        if (infoOnSkins[currentSkin].isOwned == true)
        {
            purchasedText.SetActive(true);
            purchasableText.SetActive(false);
            if (infoOnSkins[currentSkin].isEquipped == true)
            {
                actionButton.gameObject.GetComponent<Image>().sprite = equippedSkin;
                actionButton.enabled = false;
                purchasedText.GetComponentInChildren<TextMeshProUGUI>().text = "Equipped";
            }
            else
            {
                actionButton.gameObject.GetComponent<Image>().sprite = equipSkin;
                actionButton.enabled = true;
                purchasedText.GetComponentInChildren<TextMeshProUGUI>().text = "Equip";
            }
        }

        if (infoOnSkins[currentSkin].isOwned == false)
        {
            purchasableText.SetActive(true);
            purchasedText.SetActive(false);
            actionButton.gameObject.GetComponent<Image>().sprite = purchaseSkin;
            actionButton.enabled = true;
            #region Check If you have enough currency (Disabled For Now)
            //int amountOfCurrencyToConsider;
            //switch (infoOnSkins[currentSkin].isGems)
            //{
            //    case true:
            //        amountOfCurrencyToConsider = 0; //CurrencyDataHandler.instance.ReturnSavedValues().amountOfGems;
            //        break;
            //    case false:
            //        amountOfCurrencyToConsider = CurrencyDataHandler.instance.ReturnSavedValues().amountOfCoins;
            //        break;
            //}
            //if (infoOnSkins[currentSkin].skinPrice <= amountOfCurrencyToConsider)
            //{
            //    actionButton.enabled = true;
            //    actionButton.interactable = true;
            //}
            //else
            //{
            //    actionButton.enabled = true;
            //    actionButton.interactable = false;
            //}
            #endregion
            actionButton.GetComponentInChildren<TextMeshProUGUI>().text = "$" + infoOnSkins[currentSkin].skinPrice;
        }
    }

    private void EquipSkin()
    {
        if (infoOnSkins[currentSkin].isOwned && !infoOnSkins[currentSkin].isEquipped)
        {
            actionButton.gameObject.GetComponent<Image>().sprite = equippedSkin;
            for (int i = 0; i < infoOnSkins.Count; i++)
            {
                infoOnSkins[i].isEquipped = false;
                SkinsOwnershipDataHandler.instance.ReturnSavedValues().skinsInfo[i].isEquipped = false;
                if (i == currentSkin)
                {
                    infoOnSkins[i].isEquipped = true;
                    SkinsOwnershipDataHandler.instance.ReturnSavedValues().skinsInfo[i].isEquipped = true;
                }
            }
            SkinsOwnershipDataHandler.instance.SaveSkinData();
        }
    }

    private void InstantiateDots()
    {
        for (int i = 0; i < infoOnSkins.Count; i++)
        {
            var instantiatedThing = Instantiate(circlePrefab, Vector3.zero, Quaternion.identity);
            instantiatedThing.transform.SetParent(targetForInstantiating.transform, false);
            instantiatedThing.GetComponent<RectTransform>().transform.localScale = largerCircleScale;
            circles.Add(instantiatedThing);
        }
        ChangeDotsSprites();
    }

    private void ChangeDotsSprites()
    {
        for (int i = 0; i < infoOnSkins.Count; i++)
        {
            if (i == currentSkin)
            {
                circles[i].GetComponent<Image>().sprite = largerCircleImage;
                circles[i].GetComponent<Image>().color = largerCircleColour;
                circles[i].GetComponent<RectTransform>().transform.localScale = largerCircleScale;
            }
            else
            {
                circles[i].GetComponent<Image>().sprite = smallerCircleImage;
                circles[i].GetComponent<Image>().color = smallerCircleColour;
                circles[i].GetComponent<RectTransform>().transform.localScale = smallerCircleScale;
            }
        }
    }

    public void ConfirmPurchase()
    {
        infoOnSkins[currentSkin].isOwned = true;
        SkinsOwnershipDataHandler.instance.ReturnSavedValues().skinsInfo[currentSkin].isOwned = true;
        SkinsOwnershipDataHandler.instance.SaveSkinData();

        switch (infoOnSkins[currentSkin].isGems)
        {
            case true:
                CurrencyManager.instance.AdjustCurrency(-infoOnSkins[currentSkin].skinPrice);
                break;
            case false:
                CurrencyManager.instance.AdjustCurrency(-infoOnSkins[currentSkin].skinPrice);
                break;
        }

        UpdateActionButton();
        infoOnSkins[currentSkin].skinObject.transform.position = new Vector3(infoOnSkins[currentSkin].skinObject.transform.position.x, infoOnSkins[currentSkin].skinObject.transform.position.y, zPositionOriginal);
        confirmPurchasePanel.SetActive(false);
        canSwipe = true;
    }

    public void DeclinePurchase()
    {
        infoOnSkins[currentSkin].skinObject.transform.position = new Vector3(infoOnSkins[currentSkin].skinObject.transform.position.x, infoOnSkins[currentSkin].skinObject.transform.position.y, zPositionOriginal);
        confirmPurchasePanel.SetActive(false);
        canSwipe = true;
    }
    public void DeclineADs()
    {
        infoOnSkins[currentSkin].skinObject.transform.position = new Vector3(infoOnSkins[currentSkin].skinObject.transform.position.x, infoOnSkins[currentSkin].skinObject.transform.position.y, zPositionOriginal);
        watchADsPanel.SetActive(false);
        canSwipe = true;
    }
    public void UponPurchase()
    {
        if (infoOnSkins[currentSkin].isOwned == false)
        {
            int amountOfCurrencyToConsider;
            switch (infoOnSkins[currentSkin].isGems)
            {
                case true:
                    amountOfCurrencyToConsider = 0; //CurrencyDataHandler.instance.ReturnSavedValues().amountOfGems;
                    break;
                case false:
                    amountOfCurrencyToConsider = CurrencyDataHandler.instance.ReturnSavedValues().amountOfCoins;
                    break;
            }
            if (infoOnSkins[currentSkin].skinPrice > amountOfCurrencyToConsider)
            {
                //if (!IronSource.Agent.isRewardedVideoAvailable())
                //{
                //    watchADsButton.interactable = false;
                //}
                //else
                //{
                //    watchADsButton.interactable = true;
                //}

                watchADsPanel.SetActive(true);
            }
            else
            {
                descriptionText.text = "Are you sure you want to unlock this for " + infoOnSkins[currentSkin].skinPrice + " coins?";
                confirmPurchasePanel.SetActive(true);
            }


            zPositionOriginal = infoOnSkins[currentSkin].skinObject.transform.position.z;
            infoOnSkins[currentSkin].skinObject.transform.position = new Vector3(infoOnSkins[currentSkin].skinObject.transform.position.x, infoOnSkins[currentSkin].skinObject.transform.position.y, 600);
            canSwipe = false;
        }
        else
        {
            EquipSkin();
        }
        UpdateActionButton();
    }
}
