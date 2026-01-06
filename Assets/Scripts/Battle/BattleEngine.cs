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
    public BattleParticipant winner;
    public BattleParticipant loser;
    public int score;
    public List<string> battleLog;

    public BattleResult(BattleParticipant winner, BattleParticipant loser, List<string> battleLog)
    {
        this.winner = winner;
        this.loser = loser;
        this.score = CalculateScore(winner, loser);
        this.battleLog = battleLog;
    }

    private int CalculateScore(BattleParticipant winner, BattleParticipant loser)
    {
        int baseScore = 100;
        int hpBonus = Mathf.FloorToInt((float)winner.currentHp / winner.monster.stats.hp * 50);
        int statBonus = Mathf.FloorToInt((winner.monster.stats.attack + winner.monster.stats.defense) * 0.5f);

        return baseScore + hpBonus + statBonus;
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

        return new BattleResult(winner, loser, battleLog);
    }

    private static void ExecuteAttack(BattleParticipant attacker, BattleParticipant defender, List<string> battleLog)
    {
        float damageMultiplier = Random.Range(0.8f, 1.2f);
        int baseDamage = Mathf.FloorToInt(attacker.monster.stats.attack * damageMultiplier);
        int defenseReduction = Mathf.FloorToInt(defender.monster.stats.defense * 0.5f);
        int finalDamage = Mathf.Max(1, baseDamage - defenseReduction);

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
        return $"Winner: {result.winner.monster.archetype} (Score: {result.score})\n" +
               $"Loser: {result.loser.monster.archetype}\n" +
               $"Winner HP remaining: {result.winner.currentHp}/{result.winner.monster.stats.hp}";
    }
}
