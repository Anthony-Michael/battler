using Firebase;
using Firebase.Firestore;
using Firebase.Extensions;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class FirebaseService : MonoBehaviour
{
    private FirebaseApp app;
    private FirebaseFirestore firestore;
    private bool isInitialized = false;

    [System.Serializable]
    public class LeaderboardEntry
    {
        public string playerId;
        public string monsterId;
        public int score;
        public DateTime timestamp;
    }

    [System.Serializable]
    public class BattleResult
    {
        public string winnerId;
        public string loserId;
        public int score;
        public DateTime timestamp;
    }

    async void Awake()
    {
        await InitializeFirebase();
    }

    private async Task InitializeFirebase()
    {
        try
        {
            var dependencyStatus = await FirebaseApp.CheckAndFixDependenciesAsync();

            if (dependencyStatus == DependencyStatus.Available)
            {
                app = FirebaseApp.DefaultInstance;
                firestore = FirebaseFirestore.DefaultInstance;
                isInitialized = true;
                Debug.Log("Firebase initialized successfully");
            }
            else
            {
                Debug.LogError("Could not resolve all Firebase dependencies: " + dependencyStatus);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Firebase initialization failed: " + e.Message);
        }
    }

    public bool IsInitialized()
    {
        return isInitialized;
    }

    public async Task SaveMonster(MonsterData monsterData)
    {
        if (!isInitialized) return;

        try
        {
            DocumentReference docRef = firestore.Collection("monsters").Document(monsterData.barcode);

            Dictionary<string, object> monster = new Dictionary<string, object>
            {
                { "barcode", monsterData.barcode },
                { "archetype", monsterData.archetype.ToString() },
                { "hp", monsterData.stats.hp },
                { "attack", monsterData.stats.attack },
                { "defense", monsterData.stats.defense },
                { "speed", monsterData.stats.speed },
                { "critRate", monsterData.stats.critRate },
                { "createdAt", Timestamp.FromDateTime(monsterData.createdAt) }
            };

            await docRef.SetAsync(monster);
            Debug.Log("Monster saved successfully: " + monsterData.barcode);
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to save monster: " + e.Message);
        }
    }

    public async Task SubmitBattleResult(string winnerId, string loserId, int score)
    {
        if (!isInitialized) return;

        try
        {
            DocumentReference docRef = firestore.Collection("battles").Document();

            Dictionary<string, object> battle = new Dictionary<string, object>
            {
                { "winnerId", winnerId },
                { "loserId", loserId },
                { "score", score },
                { "timestamp", Timestamp.FromDateTime(DateTime.UtcNow) }
            };

            await docRef.SetAsync(battle);
            Debug.Log("Battle result submitted successfully");
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to submit battle result: " + e.Message);
        }
    }

    public async Task<List<MonsterData>> GetTopMonsters(int limit = 50)
    {
        List<MonsterData> monsters = new List<MonsterData>();

        if (!isInitialized) return monsters;

        try
        {
            Query query = firestore.Collection("monsters")
                .OrderByDescending("attack")
                .Limit(limit);

            QuerySnapshot snapshot = await query.GetSnapshotAsync();

            foreach (DocumentSnapshot document in snapshot.Documents)
            {
                if (document.Exists)
                {
                    Dictionary<string, object> data = document.ToDictionary();
                    MonsterData monster = DocumentToMonsterData(data);
                    if (monster != null)
                    {
                        monsters.Add(monster);
                    }
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to get top monsters: " + e.Message);
        }

        return monsters;
    }

    public async Task<List<LeaderboardEntry>> GetLeaderboard(string type = "alltime", int limit = 50)
    {
        List<LeaderboardEntry> entries = new List<LeaderboardEntry>();

        if (!isInitialized) return entries;

        try
        {
            DateTime cutoff = DateTime.UtcNow;

            switch (type.ToLower())
            {
                case "daily":
                    cutoff = DateTime.UtcNow.AddDays(-1);
                    break;
                case "weekly":
                    cutoff = DateTime.UtcNow.AddDays(-7);
                    break;
                case "alltime":
                default:
                    cutoff = DateTime.MinValue;
                    break;
            }

            Query query = firestore.Collection("battles")
                .WhereGreaterThan("timestamp", Timestamp.FromDateTime(cutoff))
                .OrderByDescending("score")
                .Limit(limit);

            QuerySnapshot snapshot = await query.GetSnapshotAsync();

            foreach (DocumentSnapshot document in snapshot.Documents)
            {
                if (document.Exists)
                {
                    Dictionary<string, object> data = document.ToDictionary();
                    LeaderboardEntry entry = new LeaderboardEntry
                    {
                        playerId = data.ContainsKey("winnerId") ? data["winnerId"].ToString() : "",
                        monsterId = data.ContainsKey("winnerId") ? data["winnerId"].ToString() : "",
                        score = data.ContainsKey("score") ? Convert.ToInt32(data["score"]) : 0,
                        timestamp = ((Timestamp)data["timestamp"]).ToDateTime()
                    };
                    entries.Add(entry);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to get leaderboard: " + e.Message);
        }

        return entries;
    }

    private MonsterData DocumentToMonsterData(Dictionary<string, object> data)
    {
        try
        {
            Archetype archetype = (Archetype)Enum.Parse(typeof(Archetype), data["archetype"].ToString());

            MonsterStats stats = new MonsterStats(
                Convert.ToInt32(data["hp"]),
                Convert.ToInt32(data["attack"]),
                Convert.ToInt32(data["defense"]),
                Convert.ToInt32(data["speed"]),
                Convert.ToSingle(data["critRate"])
            );

            DateTime createdAt = ((Timestamp)data["createdAt"]).ToDateTime();

            MonsterData monster = new MonsterData(data["barcode"].ToString(), archetype, stats);
            monster.createdAt = createdAt;

            return monster;
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to convert document to MonsterData: " + e.Message);
            return null;
        }
    }
}
