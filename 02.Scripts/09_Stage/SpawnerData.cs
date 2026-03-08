using UnityEngine;

[CreateAssetMenu(fileName = "SpawnerData", menuName = "New SpawnerData")]
public class SpawnerData : ScriptableObject
{
    public string spawnerName;
    public GameObject spawnerObject;

    public int spawnerHp;
}
