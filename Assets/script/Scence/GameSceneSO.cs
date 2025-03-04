using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public enum GameScenceType
{
  MainScence = 0,
  GameScence = 1
}

public enum ScenceBGM
{
    ¶ñÄ§³Ç,
    BGM_a_fateful_encounter, 
    BGM_the_village

}
[CreateAssetMenu(fileName = "GameScenceSo", menuName = "GameScence/GameScenceSo")]
public class GameSceneSO : ScriptableObject
{
  public AssetReference SceneReference;
  public GameScenceType gameScenceType;
  public ScenceBGM scenceBGM;
}
