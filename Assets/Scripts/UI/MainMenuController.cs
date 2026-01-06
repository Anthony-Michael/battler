using UnityEngine;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [Header("UI References")]
    public Button scanBarcodeButton;
    public Text monsterStatsOutput;
    public InputField manualBarcodeInput;

    [Header("Dependencies")]
    public BarcodeScanner barcodeScanner;

    void Start()
    {
        SetupUI();
    }

    void SetupUI()
    {
        if (scanBarcodeButton != null)
        {
            scanBarcodeButton.onClick.AddListener(OnScanBarcodeClicked);
        }

        if (manualBarcodeInput != null)
        {
            manualBarcodeInput.onEndEdit.AddListener(OnManualBarcodeEntered);
        }

        if (barcodeScanner != null)
        {
            barcodeScanner.OnBarcodeScanned += OnBarcodeScanned;
        }

        UpdateMonsterStats("Ready to scan or enter barcode");
    }

    void OnScanBarcodeClicked()
    {
        if (barcodeScanner != null)
        {
            barcodeScanner.StartScanning();
            UpdateMonsterStats("Scanning barcode...");
        }
        else
        {
            UpdateMonsterStats("Barcode scanner not available");
        }
    }

    void OnManualBarcodeEntered(string barcode)
    {
        if (!string.IsNullOrEmpty(barcode))
        {
            ProcessBarcode(barcode);
        }
    }

    void OnBarcodeScanned(string barcode)
    {
        ProcessBarcode(barcode);
    }

    void ProcessBarcode(string barcode)
    {
        try
        {
            MonsterData monster = BarcodeGenerator.GenerateMonster(barcode);

            string stats = $"Monster: {monster.archetype}\n" +
                          $"HP: {monster.stats.hp}\n" +
                          $"Attack: {monster.stats.attack}\n" +
                          $"Defense: {monster.stats.defense}\n" +
                          $"Speed: {monster.stats.speed}\n" +
                          $"Crit Rate: {monster.stats.critRate:P1}\n" +
                          $"Barcode: {monster.barcode}";

            UpdateMonsterStats(stats);
        }
        catch (System.Exception e)
        {
            UpdateMonsterStats("Error: " + e.Message);
        }
    }

    void UpdateMonsterStats(string message)
    {
        if (monsterStatsOutput != null)
        {
            monsterStatsOutput.text = message;
        }
        Debug.Log("MainMenu: " + message);
    }
}
