using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Globalization;


[System.Serializable]
public class SlotDef
{
    public float x;
    public float y;
    public bool faceUp = false;
    public string layerName = "Default";
    public int layerID = 0;
    public int id;
    public List<int> hiddenBy = new List<int>();
    public string type = "slot";
    public Vector2 stagger;
}

public class Layout : MonoBehaviour
{
    public PT_XMLReader xmlr;
    public PT_XMLHashtable xml;
    public Vector2 multiplier;//�������� �� ������ ���������
    //������ SlotDef
    public List<SlotDef> slotDefs;
    public SlotDef drawPile;
    public SlotDef discardPile;
    //������ ����� ���� �����
    public string[] SortingLayerNames = new string[] { "Row0", "Row1", "Row2", "Row3", "Discard", "Draw" };
    
    //��� ������� ���������� ��� ������ ����� LayoutXML.xml
    public void ReadLayout(string xmlText)
    {
        xmlr = new PT_XMLReader();
        xmlr.Parse(xmlText);// ��������� xml
        xml = xmlr.xml["xml"][0];//��������� xml
        //��������� ���������, ������������ ��������� ����� �������
        multiplier.x = float.Parse(xml["multiplier"][0].att("x"), CultureInfo.InvariantCulture);
        multiplier.y = float.Parse(xml["multiplier"][0].att("y"), CultureInfo.InvariantCulture);
        //��������� �����
        SlotDef tSD;
        //slotsX ������������ ��� ��������� ������� � ��������� <slot>
        PT_XMLHashList slotsX = xml["slot"];
        for(int i = 0;i<slotsX.Count;i++)
        {
            tSD = new SlotDef();
            if(slotsX[i].HasAtt("type"))
            {
                // ���� <slot> ����� ������� type, ��������� ���
                tSD.type = slotsX[i].att("type");
            }
            else
            {
                // ����� ���������� ��� ��� "slot"; ��� ��������� ����� � ����
                tSD.type = "slot";
            }
            //������������� �������� � �������� �������� 
            tSD.x = float.Parse(slotsX[i].att("x"), CultureInfo.InvariantCulture);
            tSD.y = float.Parse (slotsX[i].att("y"), CultureInfo.InvariantCulture);
            tSD.layerID = int.Parse(slotsX[i].att("layer"));
            //������������� ����� ����  � ����� LayerName
            tSD.layerName = SortingLayerNames[tSD.layerID];
            switch (tSD.type)
            {
                //��������� �������������� ��������, �������� �� ��� �����
                case "slot":
                    tSD.faceUp = (slotsX[i].att("faceup") == "1");
                    tSD.id = int.Parse (slotsX[i].att("id"));
                    if(slotsX[i].HasAtt("hiddenby"))
                    {
                        string[] hiding = slotsX[i].att("hiddenby").Split(',');
                        foreach(string s in hiding)
                        {
                            tSD.hiddenBy.Add(int.Parse(s));
                        }
                    }
                    slotDefs.Add(tSD);
                    break;
                case "drawpile":
                    tSD.stagger.x = float.Parse(slotsX[i].att("xstagger"), CultureInfo.InvariantCulture);
                    drawPile = tSD;
                    break;
                case "discardpile":
                    discardPile = tSD;
                    break;
            }
        }
    }

}
