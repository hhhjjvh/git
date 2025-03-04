using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomTransfer : MonoBehaviour, IInteractable
{
    
    public Vector2 TeleportPosition;

    public bool IsInteractable()
    {
        return true;
    }

    public void TiggerAction()
    {
        RandomMapGenerator.Instance.LevelUpRoom();
    }

    
}
