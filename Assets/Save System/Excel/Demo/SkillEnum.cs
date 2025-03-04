using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SkillType
{
    武,
    法,
    神,
    阵,
    护,
    遁,
}

public enum SkillTagType
{
    破,
    伤,
    厉,
    无,
}

public enum SkillTargetRange 
{
    己方任一角色,
    己方所有角色,
    敌方任一角色,
    敌方所有角色,
}

public enum SkillDamageType
{
    法术伤害 = 1,
    物理伤害 = 2,
}

public enum AttributeType
{
    金 = 1,
    木 = 2,
    土 = 4,
    水 = 8,
    火 = 16,
    风 = 32,
    雷 = 64,
    冰 = 128,
    阴 = 256,
    阳 = 512,
}