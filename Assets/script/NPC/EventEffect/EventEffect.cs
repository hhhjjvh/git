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
        return "δ֪����";
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

        // ����Ĭ������
        DrawPropertiesExcluding(serializedObject, "scriptableEffects");

        // �ű����¼�����
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("�ű����¼�", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(scriptableEffectsProp);

        // ������Ӱ�ť
        if (GUILayout.Button("��ӽ��Ч��"))
        {
            AddEffect<AddGoldEffect>();
        }

        serializedObject.ApplyModifiedProperties();
    }

    void AddEffect<T>() where T : EventEffect
    {
        var path = EditorUtility.SaveFilePanelInProject(
            "������Ч��",
            "NewEffect",
            "asset",
            "ѡ�񱣴�λ��");

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
