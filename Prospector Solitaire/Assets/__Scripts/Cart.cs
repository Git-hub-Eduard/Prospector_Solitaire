using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cart : MonoBehaviour
{
    [Header("Set Dynamically")]
    public string suit;// ����� ����� (�,D,H,S)
    public int rank;// ����������� �����(1-14)
    public Color color = Color.black;//���� �������
    public string cols = "Black";// ��� ����� 
    //���� ������ ������ ��� ������� ������� Decorator
    public List<GameObject> decoGOs = new List<GameObject>();
    //���� ������ ������ ��� ������� ������� Pip
    public List<GameObject> pipGOs = new List<GameObject>();
    public GameObject back;//������� ������ ������� �����
    public CartDefinition def;//����������� �� DeckXML.xml
    
}

[System.Serializable]
public class Decorator
{
    public string type;//������ ������������ ����������� ����� 
    public Vector3 loc;//�������������� ������� �� ����� 
    public bool flip = false;//������� ���������� ������� �� ���������
    public float scale = 1f;//������� ������� 
}
[System.Serializable]
public class CartDefinition
{
    public string face;//������ ������������ ������� ������� 
    public int rank;//���������� ����� (1-13)
    public List<Decorator> pips = new List<Decorator>();//������
}
