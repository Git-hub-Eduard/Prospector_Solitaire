using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Globalization;

public class Deck : MonoBehaviour
{
    [Header("set in Inspector")]
    public bool startFaceUp = false;
    //Масти
    public Sprite suitClub;
    public Sprite suitDiamond;
    public Sprite suitHeart;
    public Sprite suitSpade;
    public Sprite[] faceSprites;
    public Sprite[] rankSprites;
    public Sprite cardBack;
    public Sprite cardBackGold;
    public Sprite cardFront;
    public Sprite cardFrontGold;
    //Шаблон
    public GameObject prefabCard;
    public GameObject prefabSprite;
    [Header("Set Dynamically")]
    public PT_XMLReader xmlr;
    public List<Cart> cards;
    public List<string> cardNames;
    public List<Decorator> decorators;
    public List<CartDefinition> cardDef;
    public Transform deckAnchor;
    public Dictionary<string, Sprite> dictSuits;
    public void InitDeck(string deckXMLText)
    {
        //Создать точку привязки для всех игровых Card в иерархии
        if (GameObject.Find("_Deck") == null)
        {
            GameObject anchorGO = new GameObject("_Deck");
            deckAnchor = anchorGO.transform;
        }
        //Инициализировать словарь со спрайтом значков мастей
        dictSuits = new Dictionary<string, Sprite>()
        {
            {"C",suitClub },
            {"D",suitDiamond },
            {"H",suitHeart },
            {"S",suitSpade }
        };

        ReadDeck(deckXMLText);

        MakeCards();
    }

    /// <summary>
    /// Метод читает указаный XML файл и создает масив экземпляров CardDefinion
    /// </summary>
    /// <param name="DeckXMLText"></param>
    public void ReadDeck(string deckXMLText)
    {
        xmlr = new PT_XMLReader();//Создать новый  экземпляр PT_XMLReader
        xmlr.Parse(deckXMLText);//Использовать его для чтения deckXML

        //Вывод проверочной строки, чтобы показать как использовать xmlr
        string s = "xml[0] decorator[0] ";
        s += "type=" + xmlr.xml["xml"][0]["decorator"][0].att("type");
        s += " x=" + xmlr.xml["xml"][0]["decorator"][0].att("x");
        s += " y=" + xmlr.xml["xml"][0]["decorator"][0].att("y");
        s += " scale" + xmlr.xml["xml"][0]["decorator"][0].att("scale");
        //print(s);
        //Прочитать элементы <decorator> для всех карт 
        decorators = new List<Decorator>();//Инициализировать список экземпляров
        //Извлечь список PT_XMLHashList всех элементов <decorator> из XML-файла
        PT_XMLHashList xDecos = xmlr.xml["xml"][0]["decorator"];
        Decorator deco;
        for (int i = 0; i < xDecos.Count; i++)
        {
            //Для каждого элемента <decorator> в XML
            deco = new Decorator();//Создать экземпляр  Decorator
            //Скопировать атрибуты из <decorator> в Decorator
            deco.type = xDecos[i].att("type");
            //deco.flip получит значение  true, если атрибут flip одержит текст "1"
            deco.flip = (xDecos[i].att("flip") == "1");
            //Получитьь значение float из строковых атрибутов
            // Vector3 loc инициализируетса как [0,0,0], поэтому нам остаетса только изменить его
            deco.loc.x = float.Parse(xDecos[i].att("x"), CultureInfo.InvariantCulture);
            deco.loc.y = float.Parse(xDecos[i].att("y"), CultureInfo.InvariantCulture);
            deco.loc.z = float.Parse(xDecos[i].att("z"), CultureInfo.InvariantCulture);
            deco.scale = float.Parse(xDecos[i].att("scale"), CultureInfo.InvariantCulture);

            //добавить deco в список decorators
            decorators.Add(deco);
        }
        //Прочитать координаты для значков, определяющих достоинство карты 
        cardDef = new List<CartDefinition>();//Инициализировать список карт 
        //Извлечь список PT_XMLHashList всех элементов <card> из XML-файла
        PT_XMLHashList xCardDefs = xmlr.xml["xml"][0]["card"];
        for (int i = 0; i < xCardDefs.Count; i++)
        {
            //Для каждого эелемента <card> создать экземпляр CardDefinition
            CartDefinition cDef = new CartDefinition();
            //Получить значение атрибута и добавить их в cDef
            cDef.rank = int.Parse(xCardDefs[i].att("rank"));
            //Извлечь список PT_XMLHashList всех эелементов <pip> внутри этого элемента <card>
            PT_XMLHashList xPips = xCardDefs[i]["pip"];
            if (xPips != null)
            {
                for (int j = 0; j < xPips.Count; j++)
                {
                    //Обойти все эелементы <pip>
                    deco = new Decorator();
                    //Элементы <pip>  в <card> обрабатываютса класом Decorator
                    deco.type = "pip";
                    deco.flip = (xPips[j].att("flip") == "1");
                    deco.loc.x = float.Parse(xPips[j].att("x"), CultureInfo.InvariantCulture);
                    deco.loc.y = float.Parse(xPips[j].att("y"), CultureInfo.InvariantCulture);
                    deco.loc.z = float.Parse(xPips[j].att("z"), CultureInfo.InvariantCulture);
                    if (xPips[j].HasAtt("scale"))
                    {
                        deco.scale = float.Parse(xPips[j].att("scale"), CultureInfo.InvariantCulture);
                    }
                    cDef.pips.Add(deco);
                }
            }
            // Карты с картинками (Валет, Дама и король) имеют атрибут face
            if (xCardDefs[i].HasAtt("face"))
            {
                cDef.face = xCardDefs[i].att("face");
            }
            cardDef.Add(cDef);
        }
    }
    //Получить  CardDefinion на основе значения достоинства (от 1 до 14)
    public CartDefinition GetCartDefinitionByRank(int rnk)
    {
        //Поиск во всех опрежелениях  CardDefinion
        foreach (CartDefinition cd in cardDef)
        {
            //Если достоинство совпадает, вернуть это определение
            if (cd.rank == rnk)
            {
                return cd;
            }
        }
        return null;
    }

    //Создает игровые объекты карт 
    public void MakeCards()
    {
        //cardNames будет содержать имена сконструированых карт 
        //Каждая масть имеет 14 значений достоинства 
        cardNames = new List<string>();
        string[] Letters = new string[] { "C", "D", "H", "S" };
        foreach(string s in Letters)
        {
            for(int i = 0; i<13; i++)
            {
                cardNames.Add(s + (i + 1));
            }
        }
        //Создать список со всеми картами
        cards = new List<Cart>();
        //Обойти все только что созданные имена карт
        for(int i = 0; i<cardNames.Count; i++)
        {
            //Создать карту и добавить ее в колоду 
            cards.Add(MakeCard(i));
        }

    }
    private Cart MakeCard(int cNum)
    {
        //Создать новый игровой объект с картой 
        GameObject cgo = Instantiate(prefabCard) as GameObject;
        //Настроить transform.parent новой карты в соотвецтвии точки привязки
        cgo.transform.parent = deckAnchor;
        Cart card = cgo.GetComponent<Cart>();//Получить компонент Cart
        //Эта строка выкладывает карты в окуратный ряд 
        cgo.transform.localPosition = new Vector3((cNum%13)*3, cNum/13*4, 0);
        //Настроить основные параметры карты 
        card.name = cardNames[cNum];
        card.suit = card.name[0].ToString();
        card.rank = int.Parse(card.name.Substring(1));
        if(card.suit =="D" || card.suit =="H")
        {
            card.cols = "Red";
            card.color = Color.red;
        }
        //Получить CartDefinition для этой карты 
        card.def = GetCartDefinitionByRank(card.rank);
        AddDecorators(card);
        AddPips(card);
        AddFace(card);
        AddBack(card);
        return card;
    }

    private Sprite _tSp = null;
    private GameObject _tGO = null;
    private SpriteRenderer _tSR = null;
    private void AddDecorators(Cart card)
    {
        //Добавить оформление 
        foreach(Decorator deco in decorators)
        {
            if(deco.type == "suit")
            {
                //Создать экземпляр  игрового объекта спрайта
                _tGO = Instantiate(prefabSprite) as GameObject;
                //Получить ссылку на компонент SpriteRenderer
                _tSR = _tGO.GetComponent<SpriteRenderer>();

                //Установить спрайт масти
                _tSR.sprite = dictSuits[card.suit];
            }
            else
            {
                _tGO = Instantiate(prefabSprite) as GameObject;
                _tSR = _tGO.GetComponent<SpriteRenderer>();
                //Получить спрайт для отображения достоинства 
                _tSp = rankSprites[card.rank];
                //Установить спрайт достоинства 
                _tSR.sprite = _tSp;
                //Установить цвет, соотвецтвующий масти 
                _tSR.color = card.color;
            }
            //Поместить спрайты над картой 
            _tSR.sortingOrder = 1;
            //Сделать спрайт дочерним по отношению к карте
            _tGO.transform.SetParent(card.transform);
            //Установить localPosition
            _tGO.transform.localPosition = deco.loc;
            //Проверить значок, если необходимо 
            if(deco.flip)
            {
                //Поворот относительно оси z 
                _tGO.transform.rotation = Quaternion.Euler(0, 0, 180);
            }
            //Установить масштаб чтобы уменьшить размер спрайта 
            if(deco.scale !=1)
            {
                _tGO.transform.localScale = Vector3.one * deco.scale;
            }
            //Дать имя этому игровому объекту 
            _tGO.name = deco.type;
            //Добавить этот игровой объект с оформлением в список 
            card.decoGOs.Add(_tGO);
        }
    }
    private void AddPips(Cart card)
    {
        //Для каждого значка в определении...
        foreach(Decorator pip in card.def.pips)
        {
            //Создать игровой объект спрата 
            _tGO = Instantiate(prefabSprite) as GameObject;
            //Газначить радителем игровой объект карты 
            _tGO.transform.SetParent(card.transform);
            //Установить localPosition, как определено в XML- файла
            _tGO.transform.localPosition = pip.loc;
            //Перевернуть, если необходимо
            if(pip.flip)
            {
                _tGO.transform.rotation = Quaternion.Euler(0, 0, 180);
            }
            //Масштабировать, если необходимо
            if(pip.scale !=1)
            {
                _tGO.transform.localScale = Vector3.one * pip.scale;
            }
            //Дать имя игровому объекту
            _tGO.name = "pip";
            //Получить ссылку на компонент SpriteRenderer
            _tSR = _tGO.GetComponent<SpriteRenderer>();
            //установить спрайт масти 
            _tSR.sprite = dictSuits[card.suit];
            //Установить sortingOrder, чтобы значок отображался на Card-Fron
            _tSR.sortingOrder = 1;
            //Добавить этот игровой объект в список значков 
            card.pipGOs.Add(_tGO);
        }
    }
    private void AddFace(Cart card)
    {
        if(card.def.face =="")
        {
            return;
        }
        _tGO = Instantiate(prefabSprite) as GameObject;
        _tSR = _tGO.GetComponent<SpriteRenderer>();
        //Сгенерировать имя и передать его GetFace
        _tSp = GetFace(card.def.face + card.suit);
        _tSR.sprite = _tSp;//Установить спрайт
        _tSR.sortingOrder = 1;
        _tGO.transform.SetParent(card.transform);
        _tGO.transform.localPosition = Vector3.zero;
        _tGO.name = "face";
    }
    //Находит спрайт с картинкой для карты 
    private Sprite GetFace(string faceS)
    {
        foreach(Sprite _tSP in faceSprites)
        {
            if(_tSP.name == faceS)
            {
                return (_tSP);
            }
        }
        return null;
    }

    private void AddBack(Cart card)
    {
        //Добваить рубашку
        //Card_Back будет покрывать все остальное на карте
        _tGO= Instantiate(prefabSprite) as GameObject;
        _tSR= _tGO.GetComponent<SpriteRenderer>();
        _tSR.sprite = cardBack;
        _tGO.transform.SetParent(card.transform);
        _tGO.transform.localPosition= Vector3.zero;
        _tSR.sortingOrder = 2;
        _tGO.name = "back";
        card.back = _tGO;
        card.faceUp = startFaceUp;

    }

    //Перемешивает карты в Deck.cards
    static public void Shuffle(ref List<Cart> oCards)
    {
        //Создать временный список для хранения карт в перемешанном порядке
        List<Cart> tCarts = new List<Cart>();
        int ndx;//Булет хранить индекс перемещаемой карты 
        tCarts = new List<Cart>();//Инициализировать ременный список
        //Повторять пока не будет перемешаны все карты в иходном списке
        while(oCards.Count > 0)
        {
            //Выбрать случайный индекс карты 
            ndx = Random.Range(0, oCards.Count);
            //Добавить эту карту во временный список 
            tCarts.Add(oCards[ndx]);
            //и удалить карту из исходного списка
            oCards.RemoveAt(ndx);
        }
        //Заменить исходный список временным
        oCards = tCarts;
        //Так как  oCards - это параметр  - ссылка (ref)
        //оригинальный аргумент, переданный в метод, тоже изменитса

    }

}