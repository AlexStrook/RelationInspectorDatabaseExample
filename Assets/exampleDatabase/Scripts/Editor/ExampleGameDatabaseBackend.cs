using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using RelationsInspector;
using RelationsInspector.Backend;
using UnityEditor;
public class ExampleGameDatabaseBackend : MinimalBackend<ScriptableObject, string>
{
    private List<TempPrefabSO> prefabsData = new List<TempPrefabSO>();

    public override GUIContent GetContent(ScriptableObject entity)
    {

        if (entity is MasterDatabase)
        {
            Texture2D icon = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/Editor Default Resources/Icons/database.png", typeof(Texture2D));
            string label = "MASTER DB";
            var content = new GUIContent(HasWrongConnection(entity) ? SetErrorColorText(label) : label, icon);
            return content;
        }
        if (entity is LevelData)
        {
            LevelData level = (LevelData)entity;
            Texture2D icon = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/Editor Default Resources/Icons/level.png", typeof(Texture2D));
            string rwd = level.LinkedReward != null ? level.LinkedReward.RewardName : "MISSING";
            string label = $"Level[{level.ID}]_{rwd}";
            var content = new GUIContent(HasWrongConnection(entity) ? SetErrorColorText(label) : label, icon);
            return content;
        }
        if (entity is RewardData)
        {
            RewardData reward = (RewardData)entity;
            Texture2D icon = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/Editor Default Resources/Icons/reward.png", typeof(Texture2D));
            string label = $"{reward.RewardName}";
            var content = new GUIContent(HasWrongConnection(entity) ? SetErrorColorText(label) : label, icon);
            return content;
        }
        if (entity is TempPrefabSO)
        {
            TempPrefabSO prefabData = (TempPrefabSO)entity;
            Texture2D icon = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/Editor Default Resources/Icons/prefab.png", typeof(Texture2D));
            string label = $"{prefabData.Prefab.name}";
            var content = new GUIContent(prefabData.RewardData==null ? SetErrorColorText(label) : label, icon);
            return content;
        }

        return base.GetContent(entity);
    }

    bool HasWrongConnection(ScriptableObject obj)
    {
        if (obj is MasterDatabase)
        {
            var db = (MasterDatabase)obj;
            foreach (var lv in db.Levels)
            {
                if (lv == null) return true;
            }
        }
        if (obj is LevelData)
        {
            var lvl = (LevelData)obj;
            if (lvl.LinkedReward == null) return true;
            if (lvl.ParentDatabase == null) return true;
            if (lvl.LinkedReward.LinkedLevel != lvl) return true;
        }
        if (obj is RewardData)
        {
            var reward = (RewardData)obj;
            if (reward.LinkedLevel == null) return true;
            if (reward.RewardPrefab == null) return true;
            
            if (reward.LinkedLevel.LinkedReward != reward) return true;

        }
        if(obj is TempPrefabSO)
        {
            var prefabData = (TempPrefabSO)obj;
            if (prefabData.RewardData == null) return true;
        }

        return false;
    }
    string SetErrorColorText(string text)
    {
        return "<color=red>" + text + "</color>";
    }
    public override Color GetRelationColor(string relationTagValue)
    {
        if (relationTagValue == "master_level")
        {
            return Color.yellow;
        }
        if (relationTagValue == "level_reward" || relationTagValue == "level_master")
        {
            return Color.green;
        }
        if (relationTagValue == "reward_level" || relationTagValue == "reward_prefab")
        {
            return Color.magenta;
        }
        if(relationTagValue == "prefab_reward")
        {
            return Color.cyan;
        }

        return Color.white;
    }
    public override void OnRelationContextClick(Relation<ScriptableObject, string> relation, GenericMenu menu)
    {
        menu.AddItem(new GUIContent("Delete Link"), false, () => DeleteRelation(relation.Source, relation.Target, relation.Tag));
    }
    void DeleteRelation(ScriptableObject source, ScriptableObject target, string tag)
    {
        if (source is RewardData)
        {
            if (target is LevelData)
            {
                var reward = (RewardData)source;
                var level = (LevelData)target;

                reward.LinkedLevel = null;
                EditorUtility.SetDirty(reward);
                EditorUtility.SetDirty(level);
                api.RemoveRelation(source, target, tag);
            }
        }

        if (source is LevelData)
        {
            if (target is RewardData)
            {
                var level = (LevelData)source;
                var reward = (RewardData)target;

                level.LinkedReward= null;
                EditorUtility.SetDirty(reward);
                EditorUtility.SetDirty(level);
                api.RemoveRelation(source, target, tag,true);
            }
        }

        if (source is RewardData)
        {
            if (target is TempPrefabSO)
            {
                var reward = (RewardData)source;
                var prefabData = (TempPrefabSO)target;

                reward.RewardPrefab= null;
                EditorUtility.SetDirty(reward);
                EditorUtility.SetDirty(prefabData);
                api.RemoveRelation(source, target, tag, true);
            }
        }

    }
    public override void OnEntityContextClick(IEnumerable<ScriptableObject> entities, GenericMenu menu)
    {
        menu.AddItem(new GUIContent("Connect"), false, () => api.InitRelation(entities.ToArray()));
    }
    public override void CreateRelation(ScriptableObject source, ScriptableObject target)
    {
        if (source is RewardData)
        {
            if (target is LevelData)
            {
                var reward = (RewardData)source;
                var level = (LevelData)target;
                if(reward.LinkedLevel != null) api.RemoveRelation(reward, reward.LinkedLevel, "reward_level"); //clear prev relation
                
                reward.LinkedLevel = level;
                EditorUtility.SetDirty(reward);
                EditorUtility.SetDirty(level);
                api.AddRelation(source, target, "reward_level");
            }
        }

        if (source is LevelData)
        {
            if (target is RewardData)
            {
                var level = (LevelData)source;
                var reward = (RewardData)target;
                if (level.LinkedReward != null) api.RemoveRelation(level, level.LinkedReward, "level_reward"); //clear prev relation

                level.LinkedReward= reward;
                EditorUtility.SetDirty(reward);
                EditorUtility.SetDirty(level);
                api.AddRelation(source, target, "reward_level");
            }
        }

        if (source is RewardData)
        {
            if (target is TempPrefabSO)
            {
                var reward = (RewardData)source;
                var prefabData = (TempPrefabSO)target;
                if (reward.RewardPrefab != null) api.RemoveRelation(reward, GetScriptableFromPrefab(reward.RewardPrefab), "reward_prefab"); //clear prev relation
                
                reward.RewardPrefab = prefabData.Prefab;
                prefabData.RewardData = reward;
                EditorUtility.SetDirty(reward);
                api.AddRelation(source, target, "reward_prefab");
            }
        }
    }
    public override IEnumerable<Relation<ScriptableObject, string>> GetRelations(ScriptableObject entity)
    {
        if (entity == null)
        {
            yield break;
        }
        // parent -> entity
        if (entity is MasterDatabase)
        {
            var db = (MasterDatabase)entity;
            foreach (LevelData linkedLevel in db.Levels)
                yield return new Relation<ScriptableObject, string>(db, linkedLevel, "master_level");

            foreach ( RewardData reward in db.Rewards)
            {
                yield return new Relation<ScriptableObject, string>(db, reward, "master_reward");
                api.RemoveRelation(db, reward, "master_reward",true);
            }

            foreach (GameObject prefab in db.RewardPrefabs)
            {
                var prefabData = CreatePrefabSO(prefab);
                yield return new Relation<ScriptableObject, string>(db, prefabData, "master_prefabs");
                api.RemoveRelation(db, prefabData, "master_prefabs", true);
            }
        }
        if (entity is LevelData)
        {
            var level = (LevelData)entity;
            if (level.LinkedReward != null)
                yield return new Relation<ScriptableObject, string>(level, level.LinkedReward, "level_reward");
            if (level.ParentDatabase != null)
                yield return new Relation<ScriptableObject, string>(level, level.ParentDatabase, "level_master");

        }
        if (entity is RewardData)
        {
            var reward = (RewardData)entity;
            if (reward.LinkedLevel != null)
                yield return new Relation<ScriptableObject, string>(reward, reward.LinkedLevel, "reward_level");

            if (reward.RewardPrefab != null)
            {
                var prefabData = GetScriptableFromPrefab(reward.RewardPrefab);
                if(prefabData != null)
                {
                    prefabData.RewardData = reward;
                    yield return new Relation<ScriptableObject, string>(reward, prefabData, "reward_prefab");

                }
            }

        }

        /* if (entity.transform.parent != null)
             yield return new Relation<GameObject, string>(entity.transform.parent.gameObject, entity, string.Empty);

         // entity -> children
         foreach (Transform t in entity.transform)
             yield return new Relation<GameObject, string>(entity, t.gameObject, string.Empty);*/

    }

    //hack to get prefab to show in this graph: Create temporary Scriptable object to hold reference
    public TempPrefabSO CreatePrefabSO(GameObject prefab)
    {
        TempPrefabSO tempPrefabSO = (TempPrefabSO)ScriptableObject.CreateInstance(typeof(TempPrefabSO));
        tempPrefabSO.Prefab = prefab;

        prefabsData.Add(tempPrefabSO);
        return tempPrefabSO;
    }
    public class TempPrefabSO : ScriptableObject
    {
        public RewardData RewardData;
        public GameObject Prefab;
    }
    private TempPrefabSO GetScriptableFromPrefab(GameObject prefabRef)
    {
        foreach (var item in prefabsData)
        {
            if (item.Prefab == prefabRef) return item;
        }
        return null;
    }

}
