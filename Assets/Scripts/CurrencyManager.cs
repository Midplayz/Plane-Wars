using UnityEngine;

public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager instance;

    [field: SerializeField] private UIManager uiManager;

    private void Awake()
    {
        instance = this;
    }

    public void AdjustCurrency(int amountToAdjust)
    {
        HelperClass.DebugMessage("Currency was added! This much amount: " + amountToAdjust);
        CurrencyDataHandler.instance.ReturnSavedValues().amountOfCoins+= amountToAdjust;
        CurrencyDataHandler.instance.SaveCurrencyData();
        if(uiManager != null)
        {
            uiManager.UpdateCurrency();
        }
    }
}
