using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Door
{
    public Vector2Int Position { get; private set; } // 门的起始位置
    public int Height { get; private set; }          // 门的高度（用于垂直门）
    public int Width { get; private set; }           // 门的宽度（用于水平门）
    public bool IsVertical { get; private set; }     // 是否为垂直门
    public bool IsOpen { get; private set; }         // 当前门的状态（开/关）
    public GameObject DoorColliderObject { get; private set; } // 门的碰撞器对象
    private Animator doorAnimator;
    public Door(Vector2Int position, int size, bool isVertical, Transform parent)
    {
        Position = position;
        IsVertical = isVertical;
        IsOpen = false;

        if (isVertical)
        {
            Height = size;
            Width = 1;
        }
        else
        {
            Width = size;
            Height = 1;
        }

        // 创建碰撞器对象
        CreateDoorCollider(parent);

        // 创建动画对象
      //  CreateAnimator(parent);
    }

    private void CreateDoorCollider(Transform parent)
    {
        DoorColliderObject = new GameObject($"DoorCollider_{Position}");
        DoorColliderObject.transform.position = new Vector3(Position.x + Width / 2f, Position.y + Height / 2f, 0);
        DoorColliderObject.transform.parent = parent;

        var collider = DoorColliderObject.AddComponent<BoxCollider2D>();
        collider.size = new Vector2(Width, Height);
        DoorColliderObject.SetActive(false);
       // collider.isTrigger = true;
    }
    private void CreateAnimator(Transform parent)
    {
        // 创建一个空的 GameObject 来存放门的动画
        GameObject doorObject = new GameObject($"Door_{Position}");
        doorObject.transform.position = new Vector3(Position.x + Width / 2f, Position.y + Height / 2f, 0);
        doorObject.transform.parent = parent;

        // 为门对象添加一个 Animator 组件
        doorAnimator = doorObject.AddComponent<Animator>();

        // 设置动画控制器，可以在Unity编辑器中为门添加动画控制器
        doorAnimator.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("DoorController");
    }

    public void SetDoorState(bool isOpen)
    {
        IsOpen = isOpen;

        // 激活/禁用门的碰撞器
        DoorColliderObject?.SetActive(!isOpen);

        // 控制门的动画
        //doorAnimator.SetBool("IsOpen", isOpen); // 假设门的动画控制器中有一个名为 "IsOpen" 的 bool 参数
    }

}

