using UnityEngine;
using System.Linq;
using System;
using System.Security.Cryptography;
using System.Text;
using PocketBattler.Domain;

public class MonsterData
{
    public string barcode;
    public Archetype archetype;
    public MonsterStats stats;
    public DateTime createdAt;

    public MonsterData(string barcode, Archetype archetype, MonsterStats stats)
    {
        this.barcode = barcode;
        this.archetype = archetype;
        this.stats = stats;
        this.createdAt = DateTime.UtcNow;
    }
}

public static class BarcodeGenerator
{
    private static readonly int[] ARCHETYPE_RANGES = { 199, 399, 599, 799, 999 };

    public static MonsterData GenerateMonster(string barcodeDigits)
    {
        if (string.IsNullOrEmpty(barcodeDigits) || barcodeDigits.Length < 3)
        {
            throw new ArgumentException("Barcode must be at least 3 digits long");
        }

        string digitsOnly = new string(barcodeDigits.Where(char.IsDigit).ToArray());
        if (digitsOnly.Length < 3)
        {
            throw new ArgumentException("Barcode must contain at least 3 digits");
        }

        string lastThreeDigits = digitsOnly.Substring(Math.Max(0, digitsOnly.Length - 3));
        string remainingDigits = digitsOnly.Substring(0, digitsOnly.Length - 3);

        int archetypeCode = int.Parse(lastThreeDigits);
        Archetype archetype = DetermineArchetype(archetypeCode);

        MonsterStats stats = GenerateStats(remainingDigits, archetypeCode);

        return new MonsterData(digitsOnly, archetype, stats);
    }

    private static Archetype DetermineArchetype(int code)
    {
        if (code <= ARCHETYPE_RANGES[0]) return Archetype.Beast;
        if (code <= ARCHETYPE_RANGES[1]) return Archetype.Robot;
        if (code <= ARCHETYPE_RANGES[2]) return Archetype.Undead;
        if (code <= ARCHETYPE_RANGES[3]) return Archetype.Alien;
        return Archetype.Mystic;
    }

    private static MonsterStats GenerateStats(string seedDigits, int archetypeSeed)
    {
        int[] hashValues = GenerateHashValues(seedDigits + archetypeSeed.ToString());

        int hp = Mathf.Clamp(hashValues[0] % 451 + 50, 50, 500);
        int attack = Mathf.Clamp(hashValues[1] % 95 + 5, 5, 99);
        int defense = Mathf.Clamp(hashValues[2] % 95 + 5, 5, 99);
        int speed = Mathf.Clamp(hashValues[3] % 10 + 1, 1, 10);
        float critRate = Mathf.Clamp((hashValues[4] % 25 + 1) / 100f, 0.01f, 0.25f);

        return new MonsterStats(hp, attack, defense, speed, critRate);
    }

    private static int[] GenerateHashValues(string input)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
            int[] values = new int[5];

            for (int i = 0; i < 5; i++)
            {
                values[i] = BitConverter.ToInt32(hash, i * 4);
            }

            return values;
        }
    }
}
