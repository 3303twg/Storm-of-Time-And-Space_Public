using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class EnemyStat
{
    public string enemyName;
    public bool isBoss;

    public float maxHp;
    public float curHp;
    public float speed;

    public float safeDistance;

    public GameObject enemyPrefab;
    public List<GameObject> projectile;

    public float attackPower;
    public bool isLongRange;
    public float attackRange;
    public float attackSpeed;

    public List<DropEntry> dropTable;
    public int NoDropWeight;

}
