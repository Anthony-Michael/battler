using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class MainMenuController : MonoBehaviour
{
    [Header("UI References")]
    public Button scanBarcodeButton;
    public Button viewMonstersButton;
    public Button startBattleButton;
    public Button viewLeaderboardButton;
    public Text statusText;
    public GameObject monsterListPanel;
    public Transform monsterListContent;
    public GameObject monsterListItemPrefab;

    [Header("Dependencies")]
    public BarcodeScanner barcodeScanner;
    public FirebaseService firebaseService;

    private List<MonsterData> playerMonsters = new List<MonsterData>();
    private MonsterData selectedMonster;

    void Start()
    {
        SetupUI();
        LoadPlayerMonsters();
    }

    void SetupUI()
    {
        if (scanBarcodeButton != null)
        {
            scanBarcodeButton.onClick.AddListener(OnScanBarcodeClicked);
        }

        if (viewMonstersButton != null)
        {
            viewMonstersButton.onClick.AddListener(ToggleMonsterList);
        }

        if (startBattleButton != null)
        {
            startBattleButton.onClick.AddListener(OnStartBattleClicked);
        }

        if (viewLeaderboardButton != null)
        {
            viewLeaderboardButton.onClick.AddListener(OnViewLeaderboardClicked);
        }

        if (barcodeScanner != null)
        {
            barcodeScanner.OnBarcodeScanned += OnBarcodeScanned;
        }

        UpdateStatus("Welcome to PocketBattler!");
    }

    void OnScanBarcodeClicked()
    {
        if (barcodeScanner != null)
        {
            barcodeScanner.StartScanning();
            UpdateStatus("Scanning barcode...");
        }
        else
        {
            UpdateStatus("Barcode scanner not available");
        }
    }

    void OnBarcodeScanned(string barcode)
    {
        try
        {
            MonsterData monster = BarcodeGenerator.GenerateMonster(barcode);

            if (firebaseService != null && firebaseService.IsInitialized())
            {
                firebaseService.SaveMonster(monster);
            }

            playerMonsters.Add(monster);
            SavePlayerMonsters();

            UpdateStatus($"Monster created! {monster.archetype} - HP:{monster.stats.hp} ATK:{monster.stats.attack} DEF:{monster.stats.defense}");
            RefreshMonsterList();
        }
        catch (System.Exception e)
        {
            UpdateStatus("Invalid barcode: " + e.Message);
        }
    }

    void ToggleMonsterList()
    {
        if (monsterListPanel != null)
        {
            monsterListPanel.SetActive(!monsterListPanel.activeSelf);
            if (monsterListPanel.activeSelf)
            {
                RefreshMonsterList();
            }
        }
    }

    void RefreshMonsterList()
    {
        if (monsterListContent == null || monsterListItemPrefab == null) return;

        foreach (Transform child in monsterListContent)
        {
            Destroy(child.gameObject);
        }

        foreach (MonsterData monster in playerMonsters)
        {
            GameObject item = Instantiate(monsterListItemPrefab, monsterListContent);
            MonsterListItem itemScript = item.GetComponent<MonsterListItem>();

            if (itemScript != null)
            {
                itemScript.Setup(monster, this);
            }
        }
    }

    public void SelectMonster(MonsterData monster)
    {
        selectedMonster = monster;
        UpdateStatus($"Selected: {monster.archetype} - HP:{monster.stats.hp} ATK:{monster.stats.attack}");
    }

    void OnStartBattleClicked()
    {
        if (playerMonsters.Count < 2)
        {
            UpdateStatus("Need at least 2 monsters to battle!");
            return;
        }

        if (selectedMonster == null)
        {
            UpdateStatus("Please select a monster first!");
            return;
        }

        PlayerPrefs.SetString("SelectedMonster", selectedMonster.barcode);
        SceneManager.LoadScene("Battle");
    }

    void OnViewLeaderboardClicked()
    {
        UpdateStatus("Loading leaderboard...");
        StartCoroutine(LoadLeaderboard());
    }

    System.Collections.IEnumerator LoadLeaderboard()
    {
        if (firebaseService != null && firebaseService.IsInitialized())
        {
            var leaderboardTask = firebaseService.GetLeaderboard("alltime", 10);
            yield return new WaitUntil(() => leaderboardTask.IsCompleted);

            if (leaderboardTask.Result.Count > 0)
            {
                string leaderboardText = "Top 10 Players:\n";
                for (int i = 0; i < Mathf.Min(10, leaderboardTask.Result.Count); i++)
                {
                    var entry = leaderboardTask.Result[i];
                    leaderboardText += $"{i+1}. {entry.playerId} - {entry.score} pts\n";
                }
                UpdateStatus(leaderboardText);
            }
            else
            {
                UpdateStatus("No leaderboard data available");
            }
        }
        else
        {
            UpdateStatus("Firebase not initialized");
        }
    }

    void LoadPlayerMonsters()
    {
        string monstersJson = PlayerPrefs.GetString("PlayerMonsters", "");
        if (!string.IsNullOrEmpty(monstersJson))
        {
            PlayerMonsterData data = JsonUtility.FromJson<PlayerMonsterData>(monstersJson);
            if (data != null && data.monsters != null)
            {
                playerMonsters = data.monsters;
            }
        }
    }

    void SavePlayerMonsters()
    {
        PlayerMonsterData data = new PlayerMonsterData { monsters = playerMonsters };
        string monstersJson = JsonUtility.ToJson(data);
        PlayerPrefs.SetString("PlayerMonsters", monstersJson);
        PlayerPrefs.Save();
    }

    void UpdateStatus(string message)
    {
        if (statusText != null)
        {
            statusText.text = message;
        }
        Debug.Log("MainMenu: " + message);
    }

    [System.Serializable]
    private class PlayerMonsterData
    {
        public List<MonsterData> monsters;
    }
}
