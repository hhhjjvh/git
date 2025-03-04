using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = System.Object;

public class StringConvert
{
    public static Object ToValue(Type type, string s, bool multi)
    {
        object o = null;
        s = ClearEndEmpty(s);
        if (type.IsEnum)
        {
            if (multi)
            {
                int value = 0;
                if (s == "нч")
                {
                    value = 0;
                }
                else 
                {
                    string[] currentEnumNames = s.Split('+');
                    for (int i = 0; i < currentEnumNames.Length; i++)
                    {
                        value += (int)Enum.Parse(type, currentEnumNames[i]);
                    }
                    o = value;
                }
                o = value;
            }
            else
            {
                o = Enum.Parse(type, s);
            }
        }
        else if (typeof(string).Equals(type))
        {
            o = s;
        }
        else if (typeof(bool).Equals(type))
        {
            o = s.Equals("1");
        }
        else if (typeof(int).Equals(type))
        {
            int value = 0;
            int.TryParse(s, out value);
            o = value;
        }
        else if (typeof(float).Equals(type))
        {
            float value = 0f;
            float.TryParse(s, out value);
            o = value;
        }

        return o;
    }

    public static string ClearEndEmpty(string s) 
    {
        return s.TrimEnd(new char[] {' ', '\r', '\n' });
    }
}
