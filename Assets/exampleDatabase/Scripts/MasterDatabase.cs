using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RelationsInspector.Backend.AutoBackend;

[CreateAssetMenu]
public class MasterDatabase  : ScriptableObject
{
    //example database 
    public LevelData[] Levels;
    public RewardData[] Rewards;

    //those probably don't need to be there, but maybe get them using AssetDatabase or in the ExampleGameDatabaseBackEnd?
    public GameObject[] RewardPrefabs;

}
