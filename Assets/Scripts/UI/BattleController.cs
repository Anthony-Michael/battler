using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class BattleController : MonoBehaviour
{
    [Header("UI References")]
    public Text playerMonsterText;
    public Text opponentMonsterText;
    public Button battleButton;
    public Button backButton;
    public Text battleLogText;
    public ScrollRect battleLogScroll;

    [Header("Dependencies")]
    public FirebaseService firebaseService;
    public PlayerConnector playerConnector;

    private MonsterData playerMonster;
    private MonsterData opponentMonster;
    private BattleResult battleResult;
    private bool battleInProgress = false;

    void Start()
    {
        SetupUI();
        LoadBattleData();
    }

    void SetupUI()
    {
        if (battleButton != null)
        {
            battleButton.onClick.AddListener(StartBattle);
        }

        if (backButton != null)
        {
            backButton.onClick.AddListener(BackToMain);
        }

        UpdateBattleLog("Battle ready! Press Battle to start.");
    }

    void LoadBattleData()
    {
        string selectedBarcode = PlayerPrefs.GetString("SelectedMonster", "");
        if (string.IsNullOrEmpty(selectedBarcode))
        {
            UpdateBattleLog("No monster selected! Returning to main menu.");
            Invoke("BackToMain", 2f);
            return;
        }

        try
        {
            playerMonster = BarcodeGenerator.GenerateMonster(selectedBarcode);

            if (playerConnector != null)
            {
                opponentMonster = playerConnector.GetOpponentMonster();
                if (opponentMonster == null)
                {
                    UpdateBattleLog("No opponent found! Please select an opponent.");
                    return;
                }
            }
            else
            {
                opponentMonster = GenerateRandomOpponent();
            }

            UpdateMonsterDisplays();
        }
        catch (System.Exception e)
        {
            UpdateBattleLog("Error loading battle data: " + e.Message);
        }
    }

    MonsterData GenerateRandomOpponent()
    {
        string randomBarcode = Random.Range(100000, 999999).ToString() + Random.Range(100, 999).ToString();
        return BarcodeGenerator.GenerateMonster(randomBarcode);
    }

    void UpdateMonsterDisplays()
    {
        if (playerMonsterText != null)
        {
            playerMonsterText.text = FormatMonsterDisplay(playerMonster, "Player");
        }

        if (opponentMonsterText != null)
        {
            opponentMonsterText.text = FormatMonsterDisplay(opponentMonster, "Opponent");
        }
    }

    string FormatMonsterDisplay(MonsterData monster, string label)
    {
        return $"{label}: {monster.archetype}\n" +
               $"HP: {monster.stats.hp}\n" +
               $"ATK: {monster.stats.attack}\n" +
               $"DEF: {monster.stats.defense}\n" +
               $"SPD: {monster.stats.speed}\n" +
               $"CRIT: {(monster.stats.critRate * 100):F1}%";
    }

    void StartBattle()
    {
        if (battleInProgress || playerMonster == null || opponentMonster == null)
        {
            return;
        }

        battleInProgress = true;
        battleResult = BattleEngine.SimulateBattle(playerMonster, opponentMonster);

        UpdateBattleLog(BattleEngine.FormatBattleLog(battleResult.battleLog));
        UpdateBattleLog("\n" + BattleEngine.GetBattleSummary(battleResult));

        if (firebaseService != null && firebaseService.IsInitialized())
        {
            firebaseService.SubmitBattleResult(
                battleResult.winner.monster.barcode,
                battleResult.loser.monster.barcode,
                battleResult.score
            );
        }

        battleInProgress = false;
    }

    void UpdateBattleLog(string message)
    {
        if (battleLogText != null)
        {
            battleLogText.text += message + "\n";
        }

        if (battleLogScroll != null)
        {
            Canvas.ForceUpdateCanvases();
            battleLogScroll.verticalNormalizedPosition = 0f;
        }

        Debug.Log("Battle: " + message);
    }

    void BackToMain()
    {
        SceneManager.LoadScene("Main");
    }

    public void SetOpponent(MonsterData opponent)
    {
        opponentMonster = opponent;
        UpdateMonsterDisplays();
        UpdateBattleLog("Opponent selected! Ready to battle.");
    }

    public bool IsBattleReady()
    {
        return playerMonster != null && opponentMonster != null && !battleInProgress;
    }
}
