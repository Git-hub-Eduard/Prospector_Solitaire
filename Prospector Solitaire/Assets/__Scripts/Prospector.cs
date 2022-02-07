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
        Deck.Shuffle(ref deck.cards);//���������� ������ ����
        Cart c;
        for(int cNum = 0; cNum<deck.cards.Count; cNum++)
        {
            c = deck.cards[cNum];
            c.transform.localPosition = new Vector3((cNum % 13) * 3, cNum / 13 * 4, 0);
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
