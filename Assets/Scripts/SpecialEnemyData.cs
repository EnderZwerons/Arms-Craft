using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialEnemyData
{
    public static Dictionary<SPECIALENEMY, Color> SpecialColors = new Dictionary<SPECIALENEMY, Color>
    {
        {SPECIALENEMY.FAST, new Color(1f, 0.4f, 1f, 1f)},
        {SPECIALENEMY.TOUGH, new Color(0.3f, 0.4f, 1f, 1f)},
        {SPECIALENEMY.STRONG, new Color(1f, 0.3f, 0.3f, 1f)},
        {SPECIALENEMY.VERYFAST, new Color(0f, 0f, 1f, 1f)},
        {SPECIALENEMY.VERYTOUGH, new Color(0.5f, 0f, 1f, 1f)},
        {SPECIALENEMY.VERYSTRONG, new Color(1f, 0f, 0f, 1f)},
        {SPECIALENEMY.BOSS, new Color(1f, 1f, 0f, 1f)},
        {SPECIALENEMY.MEGABOSS, new Color(1f, 0.5f, 0f, 1f)}
    };

    public static Dictionary<SPECIALENEMY, EnemyData> SpecialStats = new Dictionary<SPECIALENEMY, EnemyData>
    {
        {SPECIALENEMY.FAST, new EnemyData(0.7f, 0.8f, 1.5f, 1.65f, 2f, 0.9f, 1.2f, 1.1f, 0.95f)},
        {SPECIALENEMY.TOUGH, new EnemyData(1.75f, 1.25f, 0.8f, 0.5f, 0.8f, 1.2f, 1.5f, 1.3f, 1.05f)},
        {SPECIALENEMY.STRONG, new EnemyData(1.25f, 1.3f, 1.1f, 1.15f, 1.75f, 1.25f, 1.75f, 1.5f, 1.1f)},
        {SPECIALENEMY.VERYFAST, new EnemyData(0.5f, 0.75f, 3f, 2f, 5f, 1.1f, 1.5f, 1.25f, 0.9f)},
        {SPECIALENEMY.VERYTOUGH, new EnemyData(2.5f, 2f, 1.2f, 0.5f, 1f, 1.2f, 2f, 1.75f, 1.15f)},
        {SPECIALENEMY.VERYSTRONG, new EnemyData(1.75f, 3f, 2f, 1.75f, 3f, 1.5f, 2.5f, 2f, 1.2f)},
        {SPECIALENEMY.BOSS, new EnemyData(5f, 3f, 1.2f, 0.7f, 4f, 5f, 1.5f, 3f, 1.5f)},
        {SPECIALENEMY.MEGABOSS, new EnemyData(10f, 5f, 1.3f, 0.575f, 10f, 2f, 10f, 7.5f, 2f)}
    };

    public static SPECIALENEMY GetRandomSpecialEnemy
    {
        get
        {
            int random = BetterRandom.Range(0, 56);
            if (random < 15)
            {
                return SPECIALENEMY.FAST;
            }
            if (random < 30)
            {
                return SPECIALENEMY.TOUGH;
            }
            if (random < 40)
            {
                return SPECIALENEMY.STRONG;
            }
            if (random < 45)
            {
                return SPECIALENEMY.VERYFAST;
            }
            if (random < 50)
            {
                return SPECIALENEMY.VERYTOUGH;
            }
            if (random < 53)
            {
                return SPECIALENEMY.VERYSTRONG;
            }
            if (random < 55)
            {
                return SPECIALENEMY.BOSS;
            }
            if (random < 56)
            {
                return SPECIALENEMY.MEGABOSS;
            }
            return SPECIALENEMY.FAST;
        }
    }

    public struct EnemyData
    {
        public float health, damage, attackSpeed, moveSpeed, chargeRange, attackRange, goldDrop, itemDrop, scale;

        public EnemyData(float health, float damage, float attackSpeed, float moveSpeed, float chargeRange, float attackRange, float goldDrop, float itemDrop, float scale)
        {
            this.health = health;
            this.damage = damage;
            this.attackSpeed = attackSpeed;
            this.moveSpeed = moveSpeed;
            this.chargeRange = chargeRange;
            this.attackRange = attackRange;
            this.goldDrop = goldDrop;
            this.itemDrop = itemDrop;
            this.scale = scale;
        }
    }
}
