using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Prospector : MonoBehaviour
{
    static public Prospector S;
    [Header("Set in Inspector")]
    public TextAsset deckXML;
    public TextAsset layoutXML;
    [Header("Set Dynamically")]
    public Deck deck;
    public Layout layout;
    // Start is called before the first frame update
    void Awake()
    {
        S = this;//объект одиночка
    }

    void Start()
    {
        deck = GetComponent<Deck>();// Получить компонент  Deck
        deck.InitDeck(deckXML.text);// Передать ему DeckXML
        Deck.Shuffle(ref deck.cards);//Перемешать колоду карт
       // Cart c;
       // for(int cNum = 0; cNum<deck.cards.Count; cNum++)
        //{
        //    c = deck.cards[cNum];
         //   c.transform.localPosition = new Vector3((cNum % 13) * 3, cNum / 13 * 4, 0);
        //}
        layout = GetComponent<Layout>();//Получить компонент Layout
        layout.ReadLayout(layoutXML.text);
    }
    
   
}
