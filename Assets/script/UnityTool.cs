using System;
using System.Collections.Generic;
using System.Reflection;
using TMPro;

using UnityEngine;

public enum QualityType
{
    White,
    Blue,
    Red,
    Green,
    Purple,
    Orange
}

public class UnityTool : MonoBehaviour
{
    private static UnityTool instance;
    public static UnityTool Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new UnityTool();
            }
            return instance;
        }
    }
    private class ListInfo
    {
        public object list;
        public string name;
        public Type type;
        public MethodInfo AddMethod;
        public MethodInfo ClearMethod;
    }
    private ListInfo GetListInfo(string name, FieldInfo info)
    {
        ListInfo listInfo = new ListInfo();
        listInfo.list = Activator.CreateInstance(typeof(List<>).MakeGenericType(info.FieldType.GenericTypeArguments));
        listInfo.name = name;
        listInfo.type = info.FieldType;
        listInfo.AddMethod = info.FieldType.GetMethod("Add");
        listInfo.ClearMethod = info.FieldType.GetMethod("Clear");
        return listInfo;
    }
    public object ChangeType(string s, Type type)//字符串转指定类型
    {
        if (s == "TRUE")
        {
            return true;
        }
        if (s == "FALSE")
        {
            return false;
        }
        if (typeof(Enum).IsAssignableFrom(type))
        {
            //Debug.Log(type.ToString() + " " + s);

            return Enum.Parse(type, s);
        }
        return Convert.ChangeType(s, type);
    }
    public void WriteDataToListFromTextAssest<T>(List<T> list, TextAsset textAsset) where T : new()
    {
        if (!textAsset)
        {
            return;
        }
        list.Clear();
        Type type = typeof(T);
        Type lastType = null;
        List<ListInfo> listOfListInfo = new List<ListInfo>();
        string[] lineText = textAsset.text.Replace("\r", "").Split('\n');
        string[] fieldName = lineText[0].Split(',');
        foreach (string s in fieldName)
        {
            FieldInfo info = type.GetField(s);
            if (typeof(System.Collections.IList).IsAssignableFrom(info.FieldType))
            {
                if (lastType == null)
                {
                    lastType = info.FieldType;
                    listOfListInfo.Add(GetListInfo(s, info));
                }
                if (info.FieldType != lastType)
                {
                    lastType = info.FieldType;
                    listOfListInfo.Add(GetListInfo(s, info));
                }
            }
        }
        for (int i = 1; i < lineText.Length; i++)
        {
            if (lineText[i] == "") continue;
            string[] rows = lineText[i].Split(',');
            if (rows[0] == "") continue;
            T obj = new T();
            //clear the list in listinfo every line
            foreach (ListInfo listInfo in listOfListInfo)
            {
                listInfo.list = Activator.CreateInstance(typeof(List<>).MakeGenericType(listInfo.type.GenericTypeArguments));
            }
            for (int j = 0; j < rows.Length; j++)
            {
                FieldInfo info = type.GetField(fieldName[j]);
                if (info == null) continue;
                //if the field is type of list ,assign value to the list in listInfo
                if (typeof(System.Collections.IList).IsAssignableFrom(info.FieldType))
                {
                    foreach (ListInfo listInfo in listOfListInfo)
                    {
                        if (info.FieldType == listInfo.type)
                        {
                            listInfo.AddMethod.Invoke(listInfo.list, new object[] { ChangeType(rows[j], info.FieldType.GenericTypeArguments[0]) });
                        }
                    }
                }
                else
                {
                    info.SetValue(obj, ChangeType(rows[j], info.FieldType));
                }

            }
            if (listOfListInfo.Count != 0)
            {
                foreach (ListInfo listInfo in listOfListInfo)
                {

                    type.GetField(listInfo.name).SetValue(obj, listInfo.list);
                }
            }
            list.Add(obj);
        }
    }
    public void SetTextColor(TextMeshProUGUI text, QualityType quality)
    {
        switch (quality)
        {
            case QualityType.White:
                text.color = Color.white; break;
            case QualityType.Green:
                text.color = new Color(61f / 255f, 226f / 255f, 90 / 255); break;
            case QualityType.Blue:
                text.color = new Color(14f / 255f, 160f / 255f, 255f / 255f); break;
            case QualityType.Purple:
                text.color = new Color(190f / 255f, 7f / 255, 201f / 255f); break;
            case QualityType.Orange:
                text.color = new Color(255f / 255f, 143f / 255f, 1f / 255f); break;
            case QualityType.Red:
                text.color = new Color(255f / 255f, 26f / 255f, 26f / 255f); break;
        }
    }
}
