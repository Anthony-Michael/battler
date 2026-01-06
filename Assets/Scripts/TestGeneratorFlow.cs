using UnityEngine;

public class TestGeneratorFlow : MonoBehaviour
{
    void Start()
    {
        TestBarcodeGeneration();
    }

    void TestBarcodeGeneration()
    {
        string sampleBarcode = "123456789012";

        Debug.Log("Testing barcode generation with: " + sampleBarcode);

        try
        {
            MonsterData monster = BarcodeGenerator.GenerateMonster(sampleBarcode);

            Debug.Log("Monster generated successfully!");
            Debug.Log("Barcode: " + monster.barcode);
            Debug.Log("Archetype: " + monster.archetype);
            Debug.Log("Stats:");
            Debug.Log("  HP: " + monster.stats.hp);
            Debug.Log("  Attack: " + monster.stats.attack);
            Debug.Log("  Defense: " + monster.stats.defense);
            Debug.Log("  Speed: " + monster.stats.speed);
            Debug.Log("  Crit Rate: " + monster.stats.critRate);
            Debug.Log("Created At: " + monster.createdAt);

            // Test determinism - generate the same monster again
            MonsterData monster2 = BarcodeGenerator.GenerateMonster(sampleBarcode);
            bool isDeterministic = (monster.stats.hp == monster2.stats.hp &&
                                   monster.stats.attack == monster2.stats.attack &&
                                   monster.stats.defense == monster2.stats.defense &&
                                   monster.stats.speed == monster2.stats.speed &&
                                   Mathf.Approximately(monster.stats.critRate, monster2.stats.critRate) &&
                                   monster.archetype == monster2.archetype);

            Debug.Log("Deterministic test passed: " + isDeterministic);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error generating monster: " + e.Message);
        }
    }
}
