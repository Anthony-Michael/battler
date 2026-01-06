using UnityEngine;
using UnityEngine.UI;

public class OpponentListItem : MonoBehaviour
{
    public Text monsterNameText;
    public Text statsText;
    public Button selectButton;

    private MonsterData monsterData;
    private PlayerConnector connector;

    public void Setup(MonsterData monster, PlayerConnector connector)
    {
        this.monsterData = monster;
        this.connector = connector;

        if (monsterNameText != null)
        {
            monsterNameText.text = monster.archetype.ToString();
        }

        if (statsText != null)
        {
            statsText.text = $"HP:{monster.stats.hp} ATK:{monster.stats.attack} DEF:{monster.stats.defense} SPD:{monster.stats.speed}";
        }

        if (selectButton != null)
        {
            selectButton.onClick.AddListener(OnSelectClicked);
        }
    }

    void OnSelectClicked()
    {
        if (connector != null && monsterData != null)
        {
            connector.SelectOpponent(monsterData);
        }
    }
}
