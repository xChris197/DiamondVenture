using System.IO;
using UnityEngine;

public class UpgradesManager : MonoBehaviour
{
    [SerializeField] private GameObject[] upgradeButtons;
    [SerializeField] private GameObject[] powerupButtons;

    private void SaveData()
    {
        var fullpath = Path.Combine(Application.persistentDataPath, "upgradeData.dat");

        using (BinaryWriter writer = new BinaryWriter(File.Open(fullpath, FileMode.OpenOrCreate)))
        {
            foreach (GameObject button in upgradeButtons)
            {
                UpgradeButton upgradeTemp = button.GetComponent<UpgradeButton>();
                writer.Write(upgradeTemp.bIsStartingUpgrade);
                writer.Write(upgradeTemp.bIsEndingUpgrade);
                writer.Write(upgradeTemp.bIsUnlocked);
                writer.Write(upgradeTemp.upgrade.bIsUnlocked);

                writer.Write(upgradeTemp.upgradeCost);
                writer.Write(upgradeTemp.amountBought);

            }

            foreach (GameObject powerup in powerupButtons)
            {
                PowerupButton temp = powerup.GetComponent<PowerupButton>();
                writer.Write(temp.powerupCost);
                writer.Write(temp.amountBought);
                writer.Write(temp.amountPerTapsMultiplier);
            }

            writer.Close();
        }
    }

    private void LoadData()
    {
        var fullpath = Path.Combine(Application.persistentDataPath, "upgradeData.dat");

        if (File.Exists(fullpath))
        {
            using (BinaryReader reader = new BinaryReader(File.Open(fullpath, FileMode.Open)))
            {
                foreach (GameObject button in upgradeButtons)
                {
                    UpgradeButton temp = button.GetComponent<UpgradeButton>();
                    temp.bIsStartingUpgrade = reader.ReadBoolean();
                    temp.bIsEndingUpgrade = reader.ReadBoolean();
                    temp.bIsUnlocked = reader.ReadBoolean();
                    temp.upgrade.bIsUnlocked = reader.ReadBoolean();
                    temp.upgradeCost = reader.ReadDouble();
                    temp.amountBought = reader.ReadInt32();
                }
                
                foreach (GameObject powerup in powerupButtons)
                {
                    PowerupButton temp = powerup.GetComponent<PowerupButton>();
                    temp.powerupCost = reader.ReadDouble();
                    temp.amountBought = reader.ReadInt32();
                    temp.amountPerTapsMultiplier = reader.ReadDouble();
                }
                CustomEvents.Currency.OnUpdateUI?.Invoke();
                reader.Close();
            }
        }
    }

    private void DeleteData()
    {
        var fullpath = Path.Combine(Application.persistentDataPath, "upgradeData.dat");

        if (File.Exists(fullpath))
        {
            File.Delete(fullpath);
        }
    }

    private void OnEnable()
    {
        CustomEvents.SaveSystem.OnLoadGame += LoadData;
        CustomEvents.SaveSystem.OnSaveGame += SaveData;
        CustomEvents.SaveSystem.OnDeleteGame += DeleteData;
    }

    private void OnDisable()
    {
        CustomEvents.SaveSystem.OnLoadGame -= LoadData;
        CustomEvents.SaveSystem.OnSaveGame -= SaveData;
        CustomEvents.SaveSystem.OnDeleteGame -= DeleteData;
    }
}
