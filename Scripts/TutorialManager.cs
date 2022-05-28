using System.IO;
using UnityEngine;
using TMPro;

public class TutorialManager : MonoBehaviour
{
    [TextArea]
    [SerializeField] private string startTutorial;
    [TextArea]
    [SerializeField] private string upgradesTutorial;
    [TextArea]
    [SerializeField] private string powerupsTutorial;
    [TextArea]
    [SerializeField] private string statsTutorial;
    [TextArea] 
    [SerializeField] private string prizeWheelTutorial;

    [SerializeField] private GameObject tutorialUI;
    [SerializeField] private TextMeshProUGUI tutorialText;
    [SerializeField] private TextMeshProUGUI titleText;

    private bool bStartGame;
    private bool bOpenedUpgrades;
    private bool bOpenedPowerups;
    private bool bOpenedStats;
    private bool bOpenedPrizeWheel;

    private void Start()
    {
        SetupUI("Start");
    }

    public void SetupUI(string _area)
    {
        switch (_area)
        {
            case "Start":
                if (!bStartGame)
                {
                    bStartGame = true;
                    tutorialText.text = startTutorial;
                    titleText.text = "WELCOME!";
                    ToggleUI(true);
                }

                break;
            case "Upgrades":
                if (!bOpenedUpgrades)
                {
                    bOpenedUpgrades = true;
                    tutorialText.text = upgradesTutorial;
                    titleText.text = "UPGRADES";
                    ToggleUI(true);
                }

                break;
            case "Powerups":
                if (!bOpenedPowerups)
                {
                    bOpenedPowerups = true;
                    tutorialText.text = powerupsTutorial;
                    titleText.text = "POWERUPS";
                    ToggleUI(true);
                }

                break;
            case "Stats":
                if (!bOpenedStats)
                {
                    bOpenedStats = true;
                    tutorialText.text = statsTutorial;
                    titleText.text = "STATS";
                    ToggleUI(true);
                }
                break;
            case "Prize Wheel":
                if (!bOpenedPrizeWheel)
                {
                    bOpenedPrizeWheel = true;
                    tutorialText.text = prizeWheelTutorial;
                    titleText.text = "PRIZE WHEEL";
                    ToggleUI(true);
                }
                break;
        }
    }

    public void ToggleUI(bool _state)
    {
        if (_state)
        {
            tutorialUI.SetActive(true);
        }
        else
        {
            tutorialUI.SetActive(false);
            tutorialText.text = "";
        }
    }

    private void SaveData()
    {
        var fullpath = Path.Combine(Application.persistentDataPath, "tutorialData.dat");

        using (BinaryWriter writer = new BinaryWriter(File.Open(fullpath, FileMode.OpenOrCreate)))
        {
            writer.Write(bStartGame);
            writer.Write(bOpenedPowerups);
            writer.Write(bOpenedUpgrades);
            writer.Write(bOpenedStats);
            writer.Write(bOpenedPrizeWheel);
            
            writer.Close();
        }
    }

    private void LoadData()
    {
        var fullpath = Path.Combine(Application.persistentDataPath, "tutorialData.dat");

        if (File.Exists(fullpath))
        {
            using (BinaryReader reader = new BinaryReader(File.Open(fullpath, FileMode.Open)))
            {
                bStartGame = reader.ReadBoolean();
                bOpenedPowerups = reader.ReadBoolean();
                bOpenedUpgrades = reader.ReadBoolean();
                bOpenedStats = reader.ReadBoolean();
                bOpenedPrizeWheel = reader.ReadBoolean();

                reader.Close();
            }
        }
    }

    private void DeleteData()
    {
        var fullpath = Path.Combine(Application.persistentDataPath, "tutorialData.dat");

        if (File.Exists(fullpath))
        {
            File.Delete(fullpath);
        }
    }

    private void OnEnable()
    {
        CustomEvents.SaveSystem.OnSaveGame += SaveData;
        CustomEvents.SaveSystem.OnLoadGame += LoadData;
        CustomEvents.SaveSystem.OnDeleteGame += DeleteData;
        CustomEvents.Tutorial.OnSetupUI += SetupUI;
    }

    private void OnDisable()
    {
        CustomEvents.SaveSystem.OnSaveGame -= SaveData;
        CustomEvents.SaveSystem.OnLoadGame -= LoadData;
        CustomEvents.SaveSystem.OnDeleteGame -= DeleteData;
        CustomEvents.Tutorial.OnSetupUI -= SetupUI;
    }
}
