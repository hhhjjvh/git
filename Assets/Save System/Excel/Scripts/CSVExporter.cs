
#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Unity.VisualScripting;
using Object = UnityEngine.Object;


public class ResourcesToAddressable
{
    [MenuItem("Tools/Migrate Resources to Addressables")]
    public static void MigrateResources()
    {
        string resourcesPath = "Assets/Resources";
        string[] assetGuids = AssetDatabase.FindAssets("", new[] { resourcesPath });

        foreach (string guid in assetGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            Object asset = AssetDatabase.LoadAssetAtPath<Object>(path);

            if (asset != null)
            {
                // 设置资源为 Addressable
                var entry = UnityEditor.AddressableAssets.AddressableAssetSettingsDefaultObject.Settings.CreateOrMoveEntry(guid, null);
                entry.SetAddress(asset.name);
                Debug.Log($"Migrated: {path} to Addressables with address: {asset.name}");
            }
        }

        Debug.Log("Migration completed!");
    }
}



public class CSVExporter : ScriptableWizard
{
	public string _ScriptableObjectClassName = "";

	private string _SaveTo = "";
	public string SaveTo
	{
		get
		{
			return _SaveTo;
		}
		private set
		{
			_SaveTo = value;
		}
	}

	private Type type = null;

    [MenuItem("Tools/Data/Export Excel Template")]
	static void ExportXml()
	{
		ScriptableWizard.DisplayWizard<CSVExporter>("Export Excel Template", "Export", "SelectPath");
	}
   

	void OnWizardUpdate()
	{
		isValid = false;

		if (_ScriptableObjectClassName == "")
		{
			errorString = "Enter ScriptableObject class name";
			return;
		}

		type = Type.GetType(_ScriptableObjectClassName);
		if (type == null)
		{
			errorString = "Invalid class name!";
			return;
		}

		isValid = true;

		if (SaveTo == "")
		{
			errorString = "Select save path!";
			return;
		}

		errorString = " ";
	}

	void OnWizardCreate()
	{
		ExportCSVMethod(_ScriptableObjectClassName, SaveTo, _ScriptableObjectClassName);
	}

	void OnWizardOtherButton()
	{
		SaveTo = EditorUtility.OpenFolderPanel("SaveTo", "", "") + "/";
		if (SaveTo != "")
		{
			helpString = "SavePath: \"" + SaveTo + "\"";
			return;
		}
	}

	public void ExportCSVMethod(string typeName, string path, string fileName)
	{
		Type type = Type.GetType(typeName);
		//read all allow-type variable by reflection and save as .csv;
		string filePath = path + fileName + ".csv";
		string content = string.Empty;
		using (StreamWriter sw = new StreamWriter(filePath))
		{
			FieldInfo[] fieldInfos = type.GetFields();
			int count = 0;
			foreach (FieldInfo fieldInfo in fieldInfos)
			{
				if (!fieldInfo.IsNotSerialized) 
				{
					if (count > 0) 
					{
						sw.Write(',');
					}
					sw.Write(fieldInfo.Name);
					count++;
				}
			}
		}
	}
}
#endif
