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
    [Header("Set Dynamically")]
    public Deck deck;
    // Start is called before the first frame update
    void Awake()
    {
        S = this;//������ ��������
    }

    void Start()
    {
        deck = GetComponent<Deck>();// �������� ���������  Deck
        deck.InitDeck(deckXML.text);// �������� ��� DeckXML
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
