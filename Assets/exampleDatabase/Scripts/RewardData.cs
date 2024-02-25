using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RelationsInspector.Backend.AutoBackend;


[CreateAssetMenu]
public class RewardData : ScriptableObject
{
    public string RewardName;
    public GameObject RewardPrefab;
    public LevelData LinkedLevel;
}
