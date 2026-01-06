using UnityEngine;

public class TestBattle : MonoBehaviour
{
    void Start()
    {
        RunTestBattle();
    }

    void RunTestBattle()
    {
        Debug.Log("=== PocketBattler Test Battle ===");

        // Create two test monsters
        string barcode1 = "123456789012"; // Sample barcode
        string barcode2 = "987654321098"; // Different barcode

        MonsterData monster1 = BarcodeGenerator.GenerateMonster(barcode1);
        MonsterData monster2 = BarcodeGenerator.GenerateMonster(barcode2);

        Debug.Log($"Monster 1: {monster1.archetype} - HP:{monster1.stats.hp} ATK:{monster1.stats.attack} DEF:{monster1.stats.defense} SPD:{monster1.stats.speed}");
        Debug.Log($"Monster 2: {monster2.archetype} - HP:{monster2.stats.hp} ATK:{monster2.stats.attack} DEF:{monster2.stats.defense} SPD:{monster2.stats.speed}");

        // Simulate battle
        BattleResult result = BattleEngine.SimulateBattle(monster1, monster2);

        Debug.Log("=== Battle Result ===");
        Debug.Log(BattleEngine.FormatBattleLog(result.battleLog));
        Debug.Log(BattleEngine.GetBattleSummary(result));

        Debug.Log("=== Test Complete ===");
    }
}
