using UnityEngine;
using System.Collections.Generic;
using System.Linq;



public class ComboSystem : MonoBehaviour
{
    // private static List<string> pressedKeys = new List<string>(); // ��ΪList�������¼�����ͬ�İ���


    private static List<List<string>> comboSkills = new List<List<string>>()
  {
      new List<string> { "Move_Up", "Move_Down", "Move_Left", "Move_Right", "Attack" },
      new List<string> { "Move_Up", "Move_Down", "Move_Left", "Move_Right", "DistantAttack" },
      new List<string> { "Move_Up", "Move_Down", "DistantAttack" },
      new List<string> { "Move_Up","Move_Down",  "Attack" },
      new List<string> { "Move_Left", "Move_Left" },
      new List<string> { "Move_Right", "Move_Right" },
  };

    public static void RegisterKey(string actionName)
    {
        CheckCombo();
    }


    private static void CheckCombo()
    {
        List<string> currentKeys = InputManager.Instance.GetPressedKeys();
        foreach (var combo in comboSkills)
        {
            if (IsComboMatched(currentKeys, combo))
            {
                InputManager.Instance.RestKeyPressEntries();
               // Debug.Log("���ܴ�����" + string.Join(" + ", combo));
                TriggerSkill(combo);
                return;
            }
        }
    }
    // �ж��Ƿ�����������������ǰ�����˳���������

    private static bool IsComboMatched(List<string> currentKeys, List<string> combo)
    {
        if (currentKeys.Count < combo.Count) return false;

        // ����Ƿ���������İ�������ƥ������
        for (int i = 0; i <= currentKeys.Count - combo.Count; i++)
        {
            bool isMatch = true;
            for (int j = 0; j < combo.Count; j++)
            {
                if (currentKeys[i + j] != combo[j])
                {
                    isMatch = false;
                    break;
                }
            }
            if (isMatch) return true;
        }
        return false;
    }

    private static void TriggerSkill(List<string> combo)
    {
       // Debug.Log("�������У�" + string.Join(" + ", combo));
        int index = comboSkills.FindIndex(c => c.SequenceEqual(combo));
        switch (index)
        {
            case 0:
                ChangeStateWithUseSkill2(); break;
            case 1:
                ChangeStateWithUseSkill1() ;break;
            case 2: UseShillWithFlameBarrier();break;
            case 3: ChangeStateWithUseSkill13(); break;
            case 4:
            case 5: ChangeStateWithRun(); break;

            default:
                Debug.Log("δ֪����");
                break;
        }
      //  pressedKeys.Clear();
    }

    private static void UseShillWithFlameBarrier()
    {
        if (PlayerManager.instance?.player == null) return;
        player1 player = PlayerManager.instance.player;
        if (player.GetComponent<PlayerStats>().mana < 20) return;
        AudioManager.instance.PlaySFX(37, null);
        player.stateMachine.ChangeState(player.useSkillState);
        // ��� PoolMgr ʵ��
        for (int i = 1; i < 4; i++)
        {
            Vector3 position1 = player.transform.position + new Vector3(i * 1.8f, 1f, 0);
            Vector3 position2 = player.transform.position + new Vector3(-i * 1.8f, 1f, 0);
            PoolMgr.Instance?.GetObj("FlameBarrier", position1, player.transform.rotation);
            PoolMgr.Instance?.GetObj("FlameBarrier", position2, player.transform.rotation);
        }
        
        player.GetComponent<PlayerStats>().mana -= 20;
    }
    private static void ChangeStateWithRun()
    {
        if (PlayerManager.instance?.player == null) return;
        player1 player = PlayerManager.instance.player;
        player.stateMachine.ChangeState(player.runState);
    }
    
    private static void ChangeStateWithUseSkill2()
    {
        if (PlayerManager.instance?.player == null) return;
        player1 player = PlayerManager.instance.player;
        if (player.GetComponent<PlayerStats>().mana < 50) return;
        player.stateMachine.ChangeState(player.useSkillWithBigState);
        player.GetComponent<PlayerStats>().mana -= 50;
    }
    private static void ChangeStateWithUseSkill1()
    {
        if (PlayerManager.instance?.player == null) return;
        player1 player = PlayerManager.instance.player;
        if (player.GetComponent<PlayerStats>().mana < 50) return;
        
        player.stateMachine.ChangeState(player.useSkillState);
        AudioManager.instance.PlaySFX(37,null);
        player.GetComponent<PlayerStats>().mana -= 50;
        GameObject bullet = PoolMgr.Instance.GetObj("MagicArray", player.transform.position, player.transform.rotation);
    }
   
    private static void ChangeStateWithUseSkill13()
    {
        if (PlayerManager.instance?.player == null) return;
        player1 player = PlayerManager.instance.player;
        if (player.GetComponent<PlayerStats>().mana < 25) return;

        player.stateMachine.ChangeState(player.useSkillState);
        AudioManager.instance.PlaySFX(37, null);
        player.GetComponent<PlayerStats>().mana -= 25;
        Vector3 position = player.transform.position + new Vector3(0, 1.5f, 0);
        Vector3 rotation1 = new Vector3(0, -180, 0);
        GameObject bullet = PoolMgr.Instance.GetObj("SkillSpecialEffect1", position, Quaternion.Euler(rotation1));
        GameObject bullet2 = PoolMgr.Instance.GetObj("SkillSpecialEffect1", position, Quaternion.identity);
    }
}
