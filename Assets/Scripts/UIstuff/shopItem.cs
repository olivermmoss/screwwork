using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class shopItem : tooltipController
{
    public int itemID;
    GameController gc;
    bool mousedOver = false;
    Transform selector;
    public int cost;
    TextMeshProUGUI moneytext;
    TextMeshProUGUI statustext;
    TextMeshProUGUI raisechance;
    public int statusBonus;
    UIButtons uibutt;
    public AudioClip[] clips;
    AudioSource source;

    // Start is called before the first frame update
    void Start()
    {
        text.Replace("<br>", "\n");
        gc = GameObject.FindObjectOfType<GameController>();
        selector = GameObject.Find("selector").transform;
        moneytext = GameObject.Find("moneytext").GetComponent<TextMeshProUGUI>();
        statustext = GameObject.Find("statustext").GetComponent<TextMeshProUGUI>();
        raisechance = GameObject.Find("raisechance").GetComponent<TextMeshProUGUI>();
        uibutt = GameObject.FindObjectOfType<UIButtons>();
        source = GetComponent<AudioSource>();

        if (gc.itemsBought[itemID])
        {
            GetComponent<SpriteRenderer>().color = Color.black;
            Destroy(this);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(mousedOver && Input.GetKeyDown(KeyCode.Mouse0))
        {
            if(cost > gc.money)
            {
                source.clip = clips[1];
                source.Play();
            }
            else
            {
                gc.money -= cost;
                gc.itemsBought[itemID] = true;
                gc.status += statusBonus;
                source.clip = clips[0];
                source.Play();
                if(cost < 0)
                    gc.loanDay = gc.dayNum;
                GetComponent<SpriteRenderer>().color = Color.black;
                OnMouseExit();
                Destroy(this);
            }
        }
    }

    override protected void ExtraOnMouseEnter()
    {
        mousedOver = true;
        selector.position = transform.position;
        selector.gameObject.SetActive(true);
        if(cost > 0)
            moneytext.text = "money: $" + gc.money + "<color=red> - $" + cost;
        else
            moneytext.text = "money: " + gc.money + "<color=green> + $" + -cost;
        if (statusBonus != 0)
        {
            statustext.text = "status: " + gc.status + "<color=green> + " + statusBonus;
            raisechance.text = Mathf.RoundToInt(uibutt.CalculateRaiseChance(gc) * 100) + "%<color=green> + " + statusBonus + "%";
        }
    }

    override protected void ExtraOnMouseExit()
    {
        mousedOver = false;
        selector.gameObject.SetActive(false);
        moneytext.text = "money: $" + gc.money;
        statustext.text = "status: " + gc.status;
        raisechance.text = Mathf.RoundToInt(uibutt.CalculateRaiseChance(gc) * 100) + "%";
    }
}
