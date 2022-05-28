using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using TMPro;
public class CurrencyManager : MonoBehaviour
{
    [Header("Currency Data")]
    [SerializeField] private Currency currency;
    private double amount = 0;
    [SerializeField] private double currencyPerSecond;

    [SerializeField] private List<UpgradeCurrency> allCurrencyUpgrades;

    [SerializeField] private Currency goldCurrency;
    private double goldAmount = 0;
    

    [Header("Multiplier Data")]
    //Keeps track of current multiplier
    [SerializeField] private int maxMultiplierNum;
    private int currentMultiplier = 1;

    //Keeps track of amount of taps, amount of taps needed to increment multiplier & increase amount of taps per mutliplier
    private double amountPerClick = 1;
    [SerializeField] private int tapsPerMultiplier;
    private int currentTaps;
    [SerializeField] private int multiplierIncrement;

    private float timeToWait = 1;
    private float currentTime;

    private float multiplierTimeToDecrease = 3;
    private float currentMultiplierTime;

    private bool bMultiplierActive = false;

    [SerializeField] private TextMeshProUGUI currencyDisplay;
    [SerializeField] private TextMeshProUGUI gpsDisplay;
    [SerializeField] private TextMeshProUGUI multiplierDisplay;

    [SerializeField] private TextMeshProUGUI goldCurrencyDisplay;

    [Header("Time Data")] 
    [SerializeField] private int maxOfflineProduction;
    private DateTime currentDate;
    private DateTime previousDate;

    [SerializeField] private GameObject offlineProdUI;
    [SerializeField] private TextMeshProUGUI descriptionUI;
    private bool bFirstTimeOpening = true;

    [Header("Prestige Data")] 
    [SerializeField] private GameObject prestigeUI;

    private bool bCanPrestige;

    private void Awake()
    {
        CustomEvents.SaveSystem.OnLoadGame?.Invoke();
        CustomEvents.Currency.OnUpdateUI?.Invoke();
    }

    private void Start()
    {
        CustomEvents.Audio.OnPlayThemeSound?.Invoke("Chill Theme");
        currentTime = timeToWait;
        currentMultiplierTime = multiplierTimeToDecrease;

        foreach (UpgradeCurrency upgrade in allCurrencyUpgrades)
        {
            if (upgrade.upgradeName == "Diamond Mine")
            {
                upgrade.bIsUnlocked = true;
            }
            else
            {
                upgrade.bIsUnlocked = false;
            }
        }
        if (bFirstTimeOpening)
        {
            //Adapted from OXMOND Tutorials, 2019
            currentDate = DateTime.UtcNow.ToLocalTime();
            previousDate = DateTime.UtcNow.ToLocalTime();
            bFirstTimeOpening = false;
        }
        if(!bFirstTimeOpening)
        {
            currentDate = DateTime.UtcNow.ToLocalTime();
            //End of adapted code
            CheckTimeBetweenGames();
        }
        CustomEvents.Currency.OnUpdateUI?.Invoke();
    }

    private void Update()
    {
        currentTime -= Time.deltaTime;

        if(currentTime <= 0)
        {
            CustomEvents.Currency.OnAddCurrency?.Invoke(currencyPerSecond);
            currentTime = timeToWait;
        }

        if (currentTaps > 0)
        {
            bMultiplierActive = true;
        }

        if (bMultiplierActive)
        {
            multiplierDisplay.enabled = true;
            currentMultiplierTime -= Time.deltaTime;
            if (currentMultiplierTime <= 0)
            {
                if (currentMultiplier > 1)
                {
                    currentMultiplier--;
                    currentMultiplierTime = multiplierTimeToDecrease;
                    currentTaps = 0;
                    tapsPerMultiplier = (currentMultiplier + multiplierIncrement);
                }
                else
                {
                    currentMultiplier = 1;
                    bMultiplierActive = false;
                    currentTaps = 0;
                    tapsPerMultiplier = (currentMultiplier + multiplierIncrement);
                }
            }

            if (currentTaps >= tapsPerMultiplier)
            {
                if (currentMultiplier < maxMultiplierNum)
                {
                    currentMultiplier++;
                    currentTaps = 0;
                    tapsPerMultiplier = (currentMultiplier + multiplierIncrement);
                }
                else
                {
                    currentMultiplier = maxMultiplierNum;
                    tapsPerMultiplier = Int32.MaxValue;
                }
            }
            
        }
        else
        {
            multiplierDisplay.enabled = false;
        }
    }

    public void ClickCurrency()
    {
        AddCurrency(amountPerClick * currentMultiplier);
        CustomEvents.Stats.OnAddTotalClicks?.Invoke();
        currentTaps++;
        currentMultiplierTime = multiplierTimeToDecrease;
    }

    private void AddToTapMultiplier(int _amount)
    {
        amountPerClick += _amount;
    }

    private void AddCurrency(double _amount)
    {
        if (currency.GetAmount() + _amount >= 1000000000)
        {
            currency.SetAmount(1000000000);
            RemoveGPS(currencyPerSecond);
            CustomEvents.Stats.OnTogglePrestigeButton?.Invoke(true);
        }
        else
        {
            currency.AddAmount(_amount);
        }
        CustomEvents.Currency.OnUpdateUI?.Invoke();
    }

    public void AddGoldBtn()
    {
        goldCurrency.Amount += 10;
        CustomEvents.Currency.OnUpdateUI?.Invoke();
    }

    public void AddMoneyBtn()
    {
        currency.Amount += 999999999;
        CustomEvents.Currency.OnUpdateUI?.Invoke();
    }

    private void AddGPS(double _amount)
    {
        currencyPerSecond += _amount;
        CustomEvents.Currency.OnUpdateUI?.Invoke();
    }

    private void RemoveGPS(double _amount)
    {
        if (_amount > currencyPerSecond)
        {
            currencyPerSecond = 0;
        }
        else
        {
            currencyPerSecond -= _amount;
        }

        CustomEvents.Currency.OnUpdateUI?.Invoke();
    }

    private void RemoveCurrency(double _amount)
    {
        currency.RemoveAmount(_amount);
        CustomEvents.Currency.OnUpdateUI?.Invoke();
    }

    private bool CheckCurrency(double _amount)
    {
        return currency.CheckAmount(_amount);
    }

    //Updates the total gem and gold amount & updates the UI to show the changes
    private void UpdateAmount()
    {
        amount = currency.Amount;
        goldAmount = goldCurrency.Amount;
        currencyDisplay.text = amount.ToString("n0") + " Gems!";
        gpsDisplay.text = currencyPerSecond.ToString("n0") + " Gems/Sec";
        multiplierDisplay.text = "Multiplier: " + "x" + currentMultiplier.ToString();
        goldCurrencyDisplay.text = goldAmount.ToString("n0");
    }

    private void AddGoldCurrency(double _amount)
    {
        goldCurrency.AddAmount(_amount);
        CustomEvents.Currency.OnUpdateUI?.Invoke();

    }
    private void RemoveGoldCurrency(double _amount)
    {
        goldCurrency.RemoveAmount(_amount);
        CustomEvents.Currency.OnUpdateUI?.Invoke();
    }

    private bool CheckGoldCurrency(double _amount)
    {
        return goldCurrency.CheckAmount(_amount);
    }

    //Called to save each piece of data line by line
    public void SaveData()
    {
        var fullpath = Path.Combine(Application.persistentDataPath, "currencyData.dat");

        using (BinaryWriter writer = new BinaryWriter(File.Open(fullpath, FileMode.OpenOrCreate)))
        {
            writer.Write(currency.Amount);
            writer.Write(currencyPerSecond);
            writer.Write(amountPerClick);
            writer.Write(goldCurrency.Amount);
            writer.Write(bFirstTimeOpening);
            writer.Write(currentDate.ToString("T"));
            writer.Write(previousDate.ToString("T"));

            writer.Close();
        }
    }

    //Loads each data piece line by line
    public void LoadData()
    {
        var fullpath = Path.Combine(Application.persistentDataPath, "currencyData.dat");

        if (File.Exists(fullpath))
        {
            using (BinaryReader reader = new BinaryReader(File.Open(fullpath, FileMode.Open)))
            {
                amount = reader.ReadDouble();
                currencyPerSecond = reader.ReadDouble();
                amountPerClick = reader.ReadDouble();
                goldAmount = reader.ReadDouble();
                bFirstTimeOpening = reader.ReadBoolean();
                string tempCurrent = reader.ReadString();
                currentDate = DateTime.Parse(tempCurrent);
                string tempPrevious = reader.ReadString();
                previousDate = DateTime.Parse(tempPrevious);
                
                reader.Close();
            }
        }
        else
        {
            amount = 0;
            currencyPerSecond = 0;
            amountPerClick = 1;
            goldAmount = 0;
        }
        CustomEvents.Currency.OnUpdateUI?.Invoke();
    }

    public void DeleteData()
    {
        var fullpath = Path.Combine(Application.persistentDataPath, "currencyData.dat");
        if (File.Exists(fullpath))
        {
            File.Delete(fullpath);
            currency.ResetAmount();
            goldCurrency.ResetAmount();
        }
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            previousDate = DateTime.UtcNow.ToLocalTime();
            CustomEvents.SaveSystem.OnSaveGame?.Invoke();
        }
        else
        {
            previousDate = DateTime.UtcNow.ToLocalTime();
            CustomEvents.SaveSystem.OnLoadGame?.Invoke();
        }
    }

    private void OnApplicationQuit()
    {
        previousDate = DateTime.UtcNow.ToLocalTime();
        CustomEvents.SaveSystem.OnSaveGame?.Invoke();
        Debug.Log("SAVING");
    }

    private void CheckTimeBetweenGames()
    {
        TimeSpan tempTime = (currentDate - previousDate);
        previousDate = currentDate;
        if (tempTime.TotalHours >= maxOfflineProduction)
        {
            CustomEvents.Currency.OnAddCurrency?.Invoke(currencyPerSecond * 43200);
            ToggleOfflineProdUI(true, currencyPerSecond * 43200);
        }
        else
        {
            CustomEvents.Currency.OnAddCurrency?.Invoke(currencyPerSecond * tempTime.TotalSeconds);
            ToggleOfflineProdUI(true, currencyPerSecond * tempTime.TotalSeconds);
        }
    }

    public void ToggleOfflineProdUI(bool state, double productionAmount)
    {
        if (state)
        {
            offlineProdUI.SetActive(true);
            descriptionUI.text = "You have received " + productionAmount.ToString("n0") + " gems!";
        }
        else
        {
            offlineProdUI.SetActive(false);
            descriptionUI.text = "";
        }
    }

    public void TogglePrestigeUI(bool state)
    {
        if (state)
        {
            RemoveGPS(currencyPerSecond);
            prestigeUI.SetActive(true);
        }
        else
        {
            prestigeUI.SetActive(false);
            CustomEvents.Audio.OnPlaySfxSound?.Invoke("Chicken Bawk");
        }
    }

    private void OnEnable()
    {
        CustomEvents.Currency.OnAddCurrency += AddCurrency;
        CustomEvents.Currency.OnRemoveCurrency += RemoveCurrency;
        CustomEvents.Currency.OnCheckCurrency += CheckCurrency;
        CustomEvents.Currency.OnAddCPS += AddGPS;
        CustomEvents.Currency.OnUpdateUI += UpdateAmount;
        CustomEvents.Stats.OnTogglePrestigeUI += TogglePrestigeUI;
        CustomEvents.Currency.OnAddToTapMultiplier += AddToTapMultiplier;

        CustomEvents.PremiumCurrency.OnAddCurrency += AddGoldCurrency;
        CustomEvents.PremiumCurrency.OnRemoveCurrency += RemoveGoldCurrency;
        CustomEvents.PremiumCurrency.OnCheckCurrency += CheckGoldCurrency;

        CustomEvents.SaveSystem.OnLoadGame += LoadData;
        CustomEvents.SaveSystem.OnSaveGame += SaveData;
        CustomEvents.SaveSystem.OnDeleteGame += DeleteData;
    }

    private void OnDisable()
    {
        CustomEvents.Currency.OnAddCurrency -= AddCurrency;
        CustomEvents.Currency.OnRemoveCurrency -= RemoveCurrency;
        CustomEvents.Currency.OnCheckCurrency -= CheckCurrency;
        CustomEvents.Currency.OnAddCPS -= AddGPS;
        CustomEvents.Currency.OnUpdateUI -= UpdateAmount;
        CustomEvents.Stats.OnTogglePrestigeUI -= TogglePrestigeUI;
        CustomEvents.Currency.OnAddToTapMultiplier -= AddToTapMultiplier;

        CustomEvents.PremiumCurrency.OnAddCurrency -= AddGoldCurrency;
        CustomEvents.PremiumCurrency.OnRemoveCurrency -= RemoveGoldCurrency;
        CustomEvents.PremiumCurrency.OnCheckCurrency -= CheckGoldCurrency;
        
        CustomEvents.SaveSystem.OnLoadGame -= LoadData;
        CustomEvents.SaveSystem.OnSaveGame -= SaveData;
        CustomEvents.SaveSystem.OnDeleteGame -= DeleteData;
    }
}
