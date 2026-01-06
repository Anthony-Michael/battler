using UnityEngine;
using UnityEngine.UI;

public class MonsterListItem : MonoBehaviour
{
    public Text monsterNameText;
    public Text statsText;
    public Button selectButton;

    private MonsterData monsterData;
    private MainMenuController controller;

    public void Setup(MonsterData monster, MainMenuController controller)
    {
        this.monsterData = monster;
        this.controller = controller;

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
        if (controller != null && monsterData != null)
        {
            controller.SelectMonster(monsterData);
        }
    }
}
