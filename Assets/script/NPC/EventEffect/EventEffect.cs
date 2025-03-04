// EventEffect.cs
using UnityEditor;
using UnityEngine;

public abstract class EventEffect : ScriptableObject
{
    public abstract void ApplyEffect(GameObject target = null);
    public virtual bool ApplyTrigger()
    {
        Debug.Log("ApplyTrigger");
        return true;
    }
    public virtual string GetEffectDescription()
    {
        return "未知奖励";
    }

}


#if UNITY_EDITOR
[CustomEditor(typeof(DialogueEvent))]
public class DialogueEventEditor : Editor
{
    private SerializedProperty scriptableEffectsProp;

    void OnEnable()
    {
        scriptableEffectsProp = serializedObject.FindProperty("scriptableEffects");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // 绘制默认属性
        DrawPropertiesExcluding(serializedObject, "scriptableEffects");

        // 脚本化事件区域
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("脚本化事件", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(scriptableEffectsProp);

        // 快速添加按钮
        if (GUILayout.Button("添加金币效果"))
        {
            AddEffect<AddGoldEffect>();
        }

        serializedObject.ApplyModifiedProperties();
    }

    void AddEffect<T>() where T : EventEffect
    {
        var path = EditorUtility.SaveFilePanelInProject(
            "创建新效果",
            "NewEffect",
            "asset",
            "选择保存位置");

        if (!string.IsNullOrEmpty(path))
        {
            var effect = CreateInstance<T>();
            AssetDatabase.CreateAsset(effect, path);
            scriptableEffectsProp.arraySize++;
            scriptableEffectsProp.GetArrayElementAtIndex(
                scriptableEffectsProp.arraySize - 1).objectReferenceValue = effect;
        }
    }
}
#endif
