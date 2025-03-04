using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SkillData", menuName = "Data/SkillData")]
public class SkillDataSO : ScriptableObject
{
    public int Id { get { return GetInstanceID(); } }

    [NonSerialized]
    public Sprite icon;

    //bool int float string enum
    public string chineseName;
    public bool isInnate;
    public int useBlue;
    public int usePoint;
    public int useYellow;
    public float rate = 1f;
    public SkillTargetRange targetRange;
    public SkillTagType tag;
    public AttributeType attribute;
    public SkillType type;
    public SkillDamageType damageType;
}