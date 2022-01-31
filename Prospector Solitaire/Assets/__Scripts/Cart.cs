using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cart : MonoBehaviour
{
    [Header("Set Dynamically")]
    public string suit;// Масть карты (С,D,H,S)
    public int rank;// Достоинство карты(1-14)
    public Color color = Color.black;//Цвет значков
    public string cols = "Black";// Имя цвета 
    //Этот список хранит все игровые объекты Decorator
    public List<GameObject> decoGOs = new List<GameObject>();
    //Этот список хранит все игровые объекты Pip
    public List<GameObject> pipGOs = new List<GameObject>();
    public GameObject back;//Игровой объект рубашки карты
    public CartDefinition def;//Извлекаетса из DeckXML.xml
    
}

[System.Serializable]
public class Decorator
{
    public string type;//Значок определяющий достоинство карты 
    public Vector3 loc;//Местоположение спрайта на карте 
    public bool flip = false;//Признак переворота спрайта по вертикали
    public float scale = 1f;//Масштаб спрайта 
}
[System.Serializable]
public class CartDefinition
{
    public string face;//Спрайт изображающий лицевую сторону 
    public int rank;//Достоиства карты (1-13)
    public List<Decorator> pips = new List<Decorator>();//Значки
}
