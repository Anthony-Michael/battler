namespace PocketBattler.Domain
{
    public enum Archetype
    {
        Beast,
        Robot,
        Undead,
        Alien,
        Mystic
    }

    public class MonsterStats
    {
        public int hp;
        public int attack;
        public int defense;
        public int speed;
        public float critRate;

        public MonsterStats(int hp, int attack, int defense, int speed, float critRate)
        {
            this.hp = hp;
            this.attack = attack;
            this.defense = defense;
            this.speed = speed;
            this.critRate = critRate;
        }
    }
}
