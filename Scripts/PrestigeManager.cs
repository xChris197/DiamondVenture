using System.IO;
using TMPro;
using UnityEngine;

public class PrestigeManager : MonoBehaviour
{
    private int prestigeRank;
    [SerializeField] private TextMeshProUGUI prestigeTextUI;
    [SerializeField] private GameObject prestigeButton;
    [SerializeField] private GameObject prizeWheelButton;
    [SerializeField] private GameObject upgradeButton;
    private void AddToPrestigeRank()
    {
        prestigeRank += 1;
        UpdateUI();
    }
    
    private void UpdateUI()
    {
        prestigeTextUI.text = prestigeRank.ToString("n0");
    }
    
    private void SaveData()
    {
        var fullpath = Path.Combine(Application.persistentDataPath, "prestigeData.dat");

        using (BinaryWriter writer = new BinaryWriter(File.Open(fullpath, FileMode.OpenOrCreate)))
        {
            writer.Write(prestigeRank);
            
            writer.Close();
        }
    }
    
    private void LoadData()
    {
        var fullpath = Path.Combine(Application.persistentDataPath, "prestigeData.dat");

        if (File.Exists(fullpath))
        {
            using (BinaryReader reader = new BinaryReader(File.Open(fullpath, FileMode.Open)))
            {
                prestigeRank = reader.ReadInt32();
                reader.Close();
            }
        }
    }

    public void ToggleUI(bool state)
    {
        if (state)
        {
            CustomEvents.Stats.OnTogglePrestigeUI?.Invoke(true);
        }
        else
        {
            CustomEvents.Stats.OnTogglePrestigeUI?.Invoke(false);
        }
    }

    private void TogglePrestigeButton(bool state)
    {
        if (state)
        {
            prestigeButton.SetActive(true);
            prizeWheelButton.SetActive(false);
            upgradeButton.SetActive(false);
        }
        else
        {
            prestigeButton.SetActive(false);
            prizeWheelButton.SetActive(true);
            upgradeButton.SetActive(true);
        }
    }

    private void OnEnable()
    {
        CustomEvents.Stats.OnAddToPrestige += AddToPrestigeRank;
        CustomEvents.Stats.OnTogglePrestigeButton += TogglePrestigeButton;
        CustomEvents.SaveSystem.OnSaveGame += SaveData;
        CustomEvents.SaveSystem.OnLoadGame += LoadData;
        CustomEvents.Currency.OnUpdateUI += UpdateUI;
    }

    private void OnDisable()
    {
        CustomEvents.Stats.OnAddToPrestige -= AddToPrestigeRank;
        CustomEvents.Stats.OnTogglePrestigeButton -= TogglePrestigeButton;
        CustomEvents.SaveSystem.OnSaveGame -= SaveData;
        CustomEvents.SaveSystem.OnLoadGame -= LoadData;
        CustomEvents.Currency.OnUpdateUI -= UpdateUI;
    }
}
