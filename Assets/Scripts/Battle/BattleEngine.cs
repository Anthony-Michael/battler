using UnityEngine;
using System.Collections.Generic;
using System.Text;

public class BattleParticipant
{
    public MonsterData monster;
    public int currentHp;
    public bool isDefeated;

    public BattleParticipant(MonsterData monster)
    {
        this.monster = monster;
        this.currentHp = monster.stats.hp;
        this.isDefeated = false;
    }

    public void TakeDamage(int damage)
    {
        currentHp = Mathf.Max(0, currentHp - damage);
        if (currentHp <= 0)
        {
            isDefeated = true;
        }
    }

    public bool IsAlive()
    {
        return currentHp > 0 && !isDefeated;
    }

    public override string ToString()
    {
        return $"{monster.archetype} ({currentHp}/{monster.stats.hp} HP)";
    }
}

public class BattleResult
{
    public string winnerBarcode;
    public string loserBarcode;
    public string log;

    public BattleResult(string winnerBarcode, string loserBarcode, string log)
    {
        this.winnerBarcode = winnerBarcode;
        this.loserBarcode = loserBarcode;
        this.log = log;
    }
}

public static class BattleEngine
{
    private static System.Random random = new System.Random();

    public static BattleResult SimulateBattle(MonsterData monsterA, MonsterData monsterB)
    {
        BattleParticipant participantA = new BattleParticipant(monsterA);
        BattleParticipant participantB = new BattleParticipant(monsterB);

        List<string> battleLog = new List<string>();
        battleLog.Add($"Battle begins: {participantA} vs {participantB}");

        BattleParticipant firstAttacker = participantA.monster.stats.speed >= participantB.monster.stats.speed ? participantA : participantB;
        BattleParticipant secondAttacker = firstAttacker == participantA ? participantB : participantA;

        battleLog.Add($"{firstAttacker.monster.archetype} attacks first due to higher speed");

        int turnCount = 0;
        const int MAX_TURNS = 50;

        while (participantA.IsAlive() && participantB.IsAlive() && turnCount < MAX_TURNS)
        {
            turnCount++;

            ExecuteAttack(firstAttacker, secondAttacker, battleLog);
            if (!secondAttacker.IsAlive()) break;

            ExecuteAttack(secondAttacker, firstAttacker, battleLog);
            if (!firstAttacker.IsAlive()) break;
        }

        BattleParticipant winner = participantA.IsAlive() ? participantA : participantB;
        BattleParticipant loser = winner == participantA ? participantB : participantA;

        battleLog.Add($"Battle ended! {winner.monster.archetype} wins with {winner.currentHp} HP remaining");

        string battleLogString = FormatBattleLog(battleLog);
        return new BattleResult(winner.monster.barcode, loser.monster.barcode, battleLogString);
    }

    private static int CalculateDamage(int attack, int defense)
    {
        int baseDamage = attack;
        int defenseReduction = Mathf.FloorToInt(defense * 0.5f);
        return Mathf.Max(1, baseDamage - defenseReduction);
    }

    private static float ApplyVariance(float damage)
    {
        return damage * Random.Range(0.8f, 1.2f);
    }

    private static void ExecuteAttack(BattleParticipant attacker, BattleParticipant defender, List<string> battleLog)
    {
        int baseDamage = CalculateDamage(attacker.monster.stats.attack, defender.monster.stats.defense);
        float damageWithVariance = ApplyVariance(baseDamage);
        int finalDamage = Mathf.FloorToInt(damageWithVariance);

        bool isCritical = Random.value < attacker.monster.stats.critRate;
        if (isCritical)
        {
            finalDamage = Mathf.FloorToInt(finalDamage * 1.5f);
        }

        defender.TakeDamage(finalDamage);

        string critText = isCritical ? " CRITICAL!" : "";
        battleLog.Add($"{attacker.monster.archetype} attacks {defender.monster.archetype} for {finalDamage} damage{critText}. {defender} remaining");

        if (!defender.IsAlive())
        {
            battleLog.Add($"{defender.monster.archetype} has been defeated!");
        }
    }

    public static string FormatBattleLog(List<string> log)
    {
        StringBuilder sb = new StringBuilder();
        foreach (string entry in log)
        {
            sb.AppendLine(entry);
        }
        return sb.ToString();
    }

    public static string GetBattleSummary(BattleResult result)
    {
        return $"Winner: {result.winnerBarcode}\n" +
               $"Loser: {result.loserBarcode}\n" +
               $"Battle Log: {result.log}";
    }
}
