using System.IO;
using UnityEngine;
using TMPro;

public class StatsManager : MonoBehaviour
{
    [Header("Stats Data")]
    private int totalClicks = 0;
    private int totalUpgradesBought = 0;
    private int totalPowerupsBought = 0;

    [SerializeField] private TextMeshProUGUI clicksText;
    [SerializeField] private TextMeshProUGUI upgradesText;
    [SerializeField] private TextMeshProUGUI powerupsText;

    private void AddToTotalClicks()
    {
        totalClicks += 1;
        UpdateStats();
    }

    private void AddToTotalUpgradesBought()
    {
        totalUpgradesBought += 1;
        UpdateStats();
    }

    private void AddToTotalPowerupsBought()
    {
        totalPowerupsBought += 1;
        UpdateStats();
    }

    private void UpdateStats()
    {
        clicksText.text = "Total Clicks: " + totalClicks.ToString("n0");
        upgradesText.text = "Total Upgrades Bought: " + totalUpgradesBought.ToString("n0");
        powerupsText.text = "Total Powerups Bought: " + totalPowerupsBought.ToString("n0");
    }

    private void SaveData()
    {
        var fullpath = Path.Combine(Application.persistentDataPath, "statsData.dat");

        using (BinaryWriter writer = new BinaryWriter(File.Open(fullpath, FileMode.OpenOrCreate)))
        {
            writer.Write(totalClicks);
            writer.Write(totalPowerupsBought);
            writer.Write(totalUpgradesBought);

            writer.Close();
        }
    }

    private void LoadData()
    {
        var fullpath = Path.Combine(Application.persistentDataPath, "statsData.dat");

        if (File.Exists(fullpath))
        {
            using (BinaryReader reader = new BinaryReader(File.Open(fullpath, FileMode.Open)))
            {
                totalClicks = reader.ReadInt32();
                totalPowerupsBought = reader.ReadInt32();
                totalUpgradesBought = reader.ReadInt32();

                reader.Close();
            }
        }
        else
        {
            totalClicks = 0;
            totalPowerupsBought = 0;
            totalUpgradesBought = 0;
        }
    }

    private void DeleteData()
    {
        var fullpath = Path.Combine(Application.persistentDataPath, "statsData.dat");
        if (File.Exists(fullpath))
        {
            File.Delete(fullpath);
        }
    }

    private void OnEnable()
    {
        CustomEvents.Stats.OnAddTotalClicks += AddToTotalClicks;
        CustomEvents.Stats.OnAddTotalUpgrades += AddToTotalUpgradesBought;
        CustomEvents.Stats.OnAddTotalPowerups += AddToTotalPowerupsBought;
        CustomEvents.Currency.OnUpdateUI += UpdateStats;
        CustomEvents.SaveSystem.OnLoadGame += LoadData;
        CustomEvents.SaveSystem.OnSaveGame += SaveData;
        CustomEvents.SaveSystem.OnDeleteGame += DeleteData;
    }

    private void OnDisable()
    {
        CustomEvents.Stats.OnAddTotalClicks -= AddToTotalClicks;
        CustomEvents.Stats.OnAddTotalUpgrades -= AddToTotalUpgradesBought;
        CustomEvents.Stats.OnAddTotalPowerups -= AddToTotalPowerupsBought;
        CustomEvents.Currency.OnUpdateUI -= UpdateStats;
        CustomEvents.SaveSystem.OnLoadGame -= LoadData;
        CustomEvents.SaveSystem.OnSaveGame -= SaveData;
        CustomEvents.SaveSystem.OnDeleteGame -= DeleteData;
    }
}
