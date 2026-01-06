using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class PlayerConnector : MonoBehaviour
{
    [Header("UI References")]
    public GameObject opponentSelectionPanel;
    public Transform opponentListContent;
    public GameObject opponentListItemPrefab;
    public Button selectOpponentButton;
    public Text opponentStatusText;

    private List<MonsterData> availableOpponents = new List<MonsterData>();
    private MonsterData selectedOpponent;
    private MainMenuController mainMenu;

    void Start()
    {
        LoadAvailableOpponents();
        if (selectOpponentButton != null)
        {
            selectOpponentButton.onClick.AddListener(ToggleOpponentSelection);
        }
    }

    void LoadAvailableOpponents()
    {
        string monstersJson = PlayerPrefs.GetString("PlayerMonsters", "");
        if (!string.IsNullOrEmpty(monstersJson))
        {
            MainMenuController.PlayerMonsterData data = JsonUtility.FromJson<MainMenuController.PlayerMonsterData>(monstersJson);
            if (data != null && data.monsters != null)
            {
                availableOpponents = new List<MonsterData>(data.monsters);
            }
        }

        if (availableOpponents.Count == 0)
        {
            availableOpponents.Add(CreateSampleOpponent("Beast"));
            availableOpponents.Add(CreateSampleOpponent("Robot"));
            availableOpponents.Add(CreateSampleOpponent("Mystic"));
        }
    }

    MonsterData CreateSampleOpponent(string archetypeName)
    {
        string sampleBarcode = Random.Range(100000, 999999).ToString() + GetArchetypeCode(archetypeName);
        return BarcodeGenerator.GenerateMonster(sampleBarcode);
    }

    string GetArchetypeCode(string archetypeName)
    {
        switch (archetypeName)
        {
            case "Beast": return "050";
            case "Robot": return "250";
            case "Undead": return "450";
            case "Alien": return "650";
            case "Mystic": return "850";
            default: return "050";
        }
    }

    void ToggleOpponentSelection()
    {
        if (opponentSelectionPanel != null)
        {
            opponentSelectionPanel.SetActive(!opponentSelectionPanel.activeSelf);
            if (opponentSelectionPanel.activeSelf)
            {
                RefreshOpponentList();
            }
        }
    }

    void RefreshOpponentList()
    {
        if (opponentListContent == null || opponentListItemPrefab == null) return;

        foreach (Transform child in opponentListContent)
        {
            Destroy(child.gameObject);
        }

        foreach (MonsterData monster in availableOpponents)
        {
            GameObject item = Instantiate(opponentListItemPrefab, opponentListContent);
            OpponentListItem itemScript = item.GetComponent<OpponentListItem>();

            if (itemScript != null)
            {
                itemScript.Setup(monster, this);
            }
        }
    }

    public void SelectOpponent(MonsterData opponent)
    {
        selectedOpponent = opponent;
        UpdateStatus($"Selected opponent: {opponent.archetype}");

        if (opponentSelectionPanel != null)
        {
            opponentSelectionPanel.SetActive(false);
        }
    }

    public MonsterData GetOpponentMonster()
    {
        return selectedOpponent;
    }

    public void SetMainMenuController(MainMenuController controller)
    {
        mainMenu = controller;
    }

    void UpdateStatus(string message)
    {
        if (opponentStatusText != null)
        {
            opponentStatusText.text = message;
        }
        Debug.Log("PlayerConnector: " + message);
    }

    public bool HasOpponentSelected()
    {
        return selectedOpponent != null;
    }

    public void ClearSelection()
    {
        selectedOpponent = null;
        UpdateStatus("No opponent selected");
    }
}
