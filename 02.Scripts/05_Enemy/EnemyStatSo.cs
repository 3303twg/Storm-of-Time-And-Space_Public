using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "EnemyData", menuName ="DataSo/EnemyData")]
public class EnemyStatSo : ScriptableObject
{
    public string enemyName;
    public bool isBoss;

    public float hp;
    public float speed;

    public float safeDistance = 5f;

    public GameObject enemyPrefab;
    public List<GameObject> projectile;

    public float attackPower;
    public bool isLongRange;
    public float attackRange;
    public float attackSpeed;

    public List<DropEntry> dropTable;
    public int NoDropWeight;
}

[System.Serializable]
public class DropEntry
{
    public ItemInfo itemInfo;
    [Range(0f, 100f)]
    public float dropChance;
}