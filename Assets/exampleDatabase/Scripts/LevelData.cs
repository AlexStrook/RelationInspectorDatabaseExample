using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RelationsInspector.Backend.AutoBackend;


[CreateAssetMenu]
public class LevelData : ScriptableObject
{
    public MasterDatabase ParentDatabase;
    public RewardData LinkedReward;
    public int ID;
}
