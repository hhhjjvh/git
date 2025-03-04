#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using TMPro;

public class FontAssetChanger : MonoBehaviour
{
    // �������ϵ��˴������� Inspector �з���
    public TMP_FontAsset newFontAsset;

    [ContextMenu("Change All TextMesh Pro Fonts in Scene and Prefabs")]
    public void ChangeAllTextMeshProFonts()
    {
        if (newFontAsset == null)
        {
            Debug.LogError("No Font Asset assigned! Please assign a TMP_FontAsset.");
            return;
        }

        // �޸ĳ����е� TextMeshPro ���
        ChangeFontInScene();

        // �޸�����Ԥ�����е� TextMeshPro ���
        ChangeFontInPrefabs();

        Debug.Log("Font asset update completed for both scene objects and prefabs.");
    }

    private void ChangeFontInScene()
    {
        // �޸ĳ����е� TextMeshProUGUI ���
        TextMeshProUGUI[] textUIComponents = FindObjectsOfType<TextMeshProUGUI>();
        foreach (var textUI in textUIComponents)
        {
            textUI.font = newFontAsset;
        }

        // �޸ĳ����е� TextMeshPro ���
        TextMeshPro[] textComponents = FindObjectsOfType<TextMeshPro>();
        foreach (var text in textComponents)
        {
            text.font = newFontAsset;
        }

        Debug.Log($"Updated Font Asset for {textUIComponents.Length + textComponents.Length} TextMesh Pro components in the scene.");
    }

    private void ChangeFontInPrefabs()
    {
        // ��ȡ��Ŀ�����е�Ԥ����·��
        string[] prefabPaths = AssetDatabase.FindAssets("t:Prefab");
        int updatedCount = 0;

        foreach (string guid in prefabPaths)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);

            // ����Ԥ�����е� TextMeshProUGUI �� TextMeshPro ���
            TextMeshProUGUI[] textUIComponents = prefab.GetComponentsInChildren<TextMeshProUGUI>(true);
            TextMeshPro[] textComponents = prefab.GetComponentsInChildren<TextMeshPro>(true);

            bool prefabModified = false;

            foreach (var textUI in textUIComponents)
            {
                if (textUI.font != newFontAsset)
                {
                    textUI.font = newFontAsset;
                    prefabModified = true;
                }
            }

            foreach (var text in textComponents)
            {
                if (text.font != newFontAsset)
                {
                    text.font = newFontAsset;
                    prefabModified = true;
                }
            }

            // ���Ԥ�������޸ģ��򱣴�
            if (prefabModified)
            {
                EditorUtility.SetDirty(prefab);
                updatedCount++;
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"Updated Font Asset for {updatedCount} prefabs.");
    }
}
#endif
