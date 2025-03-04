using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class CSVImporter
{
#if UNITY_EDITOR
    [MenuItem("Tools/Data/Import Excel Json")]
	public static void ImportXmlTemplateMethod()
	{
		DirectoryInfo info = new DirectoryInfo("Assets");
		if (!info.Exists) info.Create();
		//choose csv
		string path = EditorUtility.OpenFilePanel("Import Excel Data", "", "csv");
		if (string.IsNullOrEmpty(path))
			return;
		string[] split = path.Split('/');
		string fileName = split[split.Length - 1].Split('.')[0];
		string copyPath = "Assets/" + fileName + ".csv";
		TextAsset text = AssetDatabase.LoadMainAssetAtPath(copyPath) as TextAsset;
		if (text != null)
		{
			AssetDatabase.DeleteAsset(copyPath);
		}
		FileUtil.CopyFileOrDirectory(path, Application.dataPath + "/" + fileName + ".csv");
		AssetDatabase.ImportAsset(copyPath);
		text = AssetDatabase.LoadMainAssetAtPath(copyPath) as TextAsset;
		if (text == null) 
		{
			return;
		}

		//get scriptable object 
		Type dataType = Type.GetType(fileName);
		if (dataType == null)
		{
			AssetDatabase.DeleteAsset(copyPath);
			Debug.Log("Don't hava this name of ScriptableObject£º" + fileName);
			return;
		}

		//choose data path
		string importPath = EditorUtility.SaveFilePanel("Save Data Path", Application.dataPath, "data", ".asset");
		if (string.IsNullOrEmpty(importPath))
			return;
		string relativePath = importPath.Split(new string[] { "Assets" }, StringSplitOptions.None)[1];
		string[] divides = relativePath.Split('/');
		string saveFolder = "Assets";
		for (int i = 0; i < divides.Length - 1; i++) 
		{
			saveFolder += divides[i] + "/";
		}

		//get field
		string[] rows = text.text.Split(new string[] { "\n" }, System.StringSplitOptions.RemoveEmptyEntries);
		for (int i = 0; i < rows.Length; i++) 
		{
			rows[i] = rows[i].TrimEnd('\r');
		}
		string[] columns = rows[0].Split(',');
		List<FieldInfo> fieldInfos = new List<FieldInfo>();
		List<bool> multis = new List<bool>();
		foreach (string column in columns)
		{
			string col = column.TrimEnd();
			string fieldName = col.Split('+')[0];
			FieldInfo field = dataType.GetField(fieldName);
			if (field != null)
			{
				fieldInfos.Add(field);

				bool multi = col.Contains("+");
				multis.Add(multi);
			}
			else 
			{
				Debug.Log(fieldName);
			}
		}


		//create data
		for (int i = 1; i < rows.Length; i++)
        {
			if (rows[i] == null)
				continue;
			columns = rows[i].Split(',');
            string assetPath = saveFolder + columns[0] + ".asset";
            ScriptableObject asset = ScriptableObject.CreateInstance(fileName);
            for (int j = 0; j < fieldInfos.Count; j++)
			{
				//Debug.Log(fieldInfos[j].FieldType + ":" + columns[j]);
				object value = StringConvert.ToValue(fieldInfos[j].FieldType, columns[j], multis[j]);
                fieldInfos[j].SetValue(asset, value);
            }
            AssetDatabase.CreateAsset(asset, assetPath);
            AssetDatabase.ImportAsset(assetPath);
			AssetDatabase.Refresh();
        }
        AssetDatabase.DeleteAsset(copyPath);
	}
    #endif
}
