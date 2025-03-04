using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu_Text_Load_DataBase : MonoBehaviour
{
    private List<Menu_Text_Set_DataBase> menuTextSet = new List<Menu_Text_Set_DataBase>();

    // Token: 0x0400078C RID: 1932
    public TextAsset menuTextFile;

    // Token: 0x0400078D RID: 1933
    private int[] id;

    // Token: 0x0400078E RID: 1934
    private int[] menuType;

    // Token: 0x0400078F RID: 1935
    private int[] menuSubType;

    // Token: 0x04000790 RID: 1936
    private string[] textENG;

    // Token: 0x04000791 RID: 1937
    private string[] textCHT;

    // Token: 0x04000792 RID: 1938
    private string[] textCHS;

    // Token: 0x04000793 RID: 1939
    private string[] textJP;

    // Token: 0x04000794 RID: 1940
    private string[] textKR;
    private void Awake()
    {
        string[] array = this.menuTextFile.text.Split(new char[]
        {
            '\n'
        });
        this.FileNewList(array.Length - 1);
        for (int i = 1; i < array.Length - 1; i++)
        {
            string[] array2 = array[i].Split(new char[]
            {
                ','
            });
            Menu_Text_Set_DataBase menu_Text_Set_DataBase = new Menu_Text_Set_DataBase();
            int.TryParse(array2[0], out menu_Text_Set_DataBase.id);
            int.TryParse(array2[1], out menu_Text_Set_DataBase.menuType);
            int.TryParse(array2[2], out menu_Text_Set_DataBase.menuSubType);
            menu_Text_Set_DataBase.textENG = array2[3];
            menu_Text_Set_DataBase.textCHT = array2[4];
            menu_Text_Set_DataBase.textCHS = array2[5];
            menu_Text_Set_DataBase.textJP = array2[6];
            menu_Text_Set_DataBase.textKR = array2[7];
            this.FileRowConfirm_Attributes(i - 1, menu_Text_Set_DataBase);
            this.menuTextSet.Add(menu_Text_Set_DataBase);
        }
    }

    // Token: 0x060000FA RID: 250 RVA: 0x00018E34 File Offset: 0x00017034
    private void FileNewList(int maxLength)
    {
        this.id = new int[maxLength];
        this.menuType = new int[maxLength];
        this.menuSubType = new int[maxLength];
        this.textENG = new string[maxLength];
        this.textCHT = new string[maxLength];
        this.textCHS = new string[maxLength];
        this.textJP = new string[maxLength];
        this.textKR = new string[maxLength];
    }

    // Token: 0x060000FB RID: 251 RVA: 0x00018EA4 File Offset: 0x000170A4
    private void FileRowConfirm_Attributes(int rowNumber, Menu_Text_Set_DataBase value)
    {
        this.id[rowNumber] = value.id;
        this.menuType[rowNumber] = value.menuType;
        this.menuSubType[rowNumber] = value.menuSubType;
        this.textENG[rowNumber] = value.textENG;
        this.textCHT[rowNumber] = value.textCHT;
        this.textCHS[rowNumber] = value.textCHS;
        this.textJP[rowNumber] = value.textJP;
        this.textKR[rowNumber] = value.textKR;
    }

    // Token: 0x060000FC RID: 252 RVA: 0x00018F24 File Offset: 0x00017124
    public string GetMenuTextString(int dataID, int language)
    {
        string result = "";
        switch (language)
        {
            case 0:
                result = this.textENG[dataID];
                break;
            case 1:
                result = this.textCHT[dataID];
                break;
            case 2:
                result = this.textCHS[dataID];
                break;
            case 3:
                result = this.textJP[dataID];
                break;
            case 4:
                result = this.textKR[dataID];
                break;
        }
        return result;
    }

    // Token: 0x0400078B RID: 1931
   

}
