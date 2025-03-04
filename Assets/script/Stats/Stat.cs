using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Stat
{
    [SerializeField] private float baseValue;

    public List<float> modifers;
    public float GetValue()
    {
        float finalValue = baseValue;
        modifers.ForEach(x => finalValue += x);

        return finalValue;
    }
    public void AddModifier(float modifier)
    {
        modifers.Add(modifier);
    }
    public void RemoveModifier(float modifier)
    {
        modifers.Remove(modifier);
    }
    public void RemoveAllModifiers()
    {
        modifers.Clear();
    }
    public void SetBaseValue(float value)
    {
        baseValue = value;
    }
}
    

