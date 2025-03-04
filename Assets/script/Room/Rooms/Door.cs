using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Door
{
    public Vector2Int Position { get; private set; } // �ŵ���ʼλ��
    public int Height { get; private set; }          // �ŵĸ߶ȣ����ڴ�ֱ�ţ�
    public int Width { get; private set; }           // �ŵĿ�ȣ�����ˮƽ�ţ�
    public bool IsVertical { get; private set; }     // �Ƿ�Ϊ��ֱ��
    public bool IsOpen { get; private set; }         // ��ǰ�ŵ�״̬����/�أ�
    public GameObject DoorColliderObject { get; private set; } // �ŵ���ײ������
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

        // ������ײ������
        CreateDoorCollider(parent);

        // ������������
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
        // ����һ���յ� GameObject ������ŵĶ���
        GameObject doorObject = new GameObject($"Door_{Position}");
        doorObject.transform.position = new Vector3(Position.x + Width / 2f, Position.y + Height / 2f, 0);
        doorObject.transform.parent = parent;

        // Ϊ�Ŷ������һ�� Animator ���
        doorAnimator = doorObject.AddComponent<Animator>();

        // ���ö�����������������Unity�༭����Ϊ����Ӷ���������
        doorAnimator.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("DoorController");
    }

    public void SetDoorState(bool isOpen)
    {
        IsOpen = isOpen;

        // ����/�����ŵ���ײ��
        DoorColliderObject?.SetActive(!isOpen);

        // �����ŵĶ���
        //doorAnimator.SetBool("IsOpen", isOpen); // �����ŵĶ�������������һ����Ϊ "IsOpen" �� bool ����
    }

}

