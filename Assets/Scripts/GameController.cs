using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameController : MonoBehaviour
{
    public int money = 0;
    public int status = 0;
    public int promotionLevel = 0;
    public int dayNum = 0;
    float timer = 0;
    public int income = 10;
    public float dayLength;
    bool isDay = false;
    int necessities = 8;
    Queue<string> endOfDayAnnouncements = new Queue<string> { };
    public int quota = 10;
    public int tasksComplete = 0;
    public bool[] itemsBought = new bool[12];
    public GameObject[] trinkets;
    public int loanDay = -1;
    public bool lost = false;
    public Vector2[] trinketPositions;
    public AudioClip[] clips;
    AudioSource source;

    void DayStart()
    {
        dayNum++;
        tasksComplete = 0;

        for(int i = 0; i < itemsBought.Length; i++)
        {
            if(itemsBought[i] && trinkets[i] != null)
                Instantiate(trinkets[i], trinketPositions[i], Quaternion.identity);
        }
    }

    public void ClockIn()
    {
        timer = Time.time + dayLength;
        isDay = true;
        FindObjectOfType<clockController>().StartClock(dayLength);
    }

    private void Update()
    {
        if(isDay && timer < Time.time)
        {
            source.clip = clips[1];
            source.Play();
            DayEnd();
        }
    }

    public void ClockOut()
    {
        GameObject.Find("Canvas").transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = "clocked out";
        DayEnd();
    }

    private void Awake()
    {
        SceneManager.activeSceneChanged += OnChangeScene;
        source = GetComponent<AudioSource>();
        if(SceneManager.GetActiveScene().name == "DayScene")
            DayStart();
    }

    void DayEnd()
    {
        isDay = false;
        bool enoughTasks = tasksComplete >= quota;

        if (enoughTasks)
            money += income;
        money -= necessities;

        endOfDayAnnouncements.Enqueue((enoughTasks ? "" : "<color=red>") + "tasks complete: " + tasksComplete + "/" + quota);
        tasksComplete = 0;

        //only get paid if you did enough tasks
        if(enoughTasks)
            endOfDayAnnouncements.Enqueue("<color=green>+$" + income + " (income)</color>");
        endOfDayAnnouncements.Enqueue("<color=red>-$" + necessities + " (necessities)</color>");
        //membership
        if(itemsBought[4])
        {
            endOfDayAnnouncements.Enqueue("<color=red>-$5 (membership)</color>");
            money -= 5;
        }
        //loan
        if (loanDay != -1 && dayNum < loanDay + 11)
        {
            int daysLeft = 10 - dayNum + loanDay;
            string end = daysLeft > 1 ? (daysLeft + " days left") : (daysLeft == 1 ? (daysLeft + " day left") : "last day!");
            endOfDayAnnouncements.Enqueue("<color=red>-$15 (loan) (" + end + ")</color>");
            money -= 15;
        }
        //bonsai tree
        if (itemsBought[8])
        {
            endOfDayAnnouncements.Enqueue("<color=green>+3 status (tree)</color>");
            status += 3;
        }

        GameObject.Find("Canvas").transform.GetChild(0).gameObject.SetActive(true);
        foreach(Draggable drag in FindObjectsOfType<Draggable>())
        {
            Destroy(drag);
        }
        foreach (Rotatable rot in FindObjectsOfType<Rotatable>())
        {
            Destroy(rot);
        }
        FindObjectOfType<clockController>().going = false;
    }

    void LoseState()
    {
        //lose the game haha
    }

    void OnChangeScene(Scene current, Scene next)
    {
        if(next.name == "DayScene")
        {
            DayStart();
        }
        else if(next.name == "NightScene")
        {
            GameObject.FindWithTag("announcements").GetComponent<TextMeshProUGUI>().text = "";
            StartCoroutine("readAnnouncements");
        }
    }

    IEnumerator readAnnouncements()
    {
        TextMeshProUGUI ann = GameObject.FindWithTag("announcements").GetComponent<TextMeshProUGUI>();
        while (endOfDayAnnouncements.Count > 0)
        {
            yield return new WaitForSeconds(0.5f);
            source.clip = clips[0];
            source.Play();
            ann.text += endOfDayAnnouncements.Dequeue() + '\n';
        }
        yield return new WaitForSeconds(0.5f);
        source.clip = clips[0];
        source.Play();
        if (money >= 0)
        {
            GameObject.Find("moneytext").GetComponent<TextMeshProUGUI>().text = "money: " + money + "$";
            TextMeshProUGUI stattext = GameObject.Find("statustext").GetComponent<TextMeshProUGUI>();
            stattext.text = "status: " + status;
            stattext.transform.GetChild(0).gameObject.SetActive(true);
            stattext.transform.GetChild(1).gameObject.SetActive(true);
            GameObject.Find("shoptext").GetComponent<TextMeshProUGUI>().text = "shop";
            GameObject shopbg = GameObject.Find("shopbg");
            shopbg.GetComponent<SpriteRenderer>().enabled = true;
            shopbg.transform.GetChild(0).gameObject.SetActive(true);
            GameObject tooltipbgs = GameObject.Find("ttiphitboxes");
            for(int i = 0; i < tooltipbgs.transform.childCount; i++)
            {
                tooltipbgs.transform.GetChild(i).gameObject.SetActive(true);
            }
            GameObject.Find("raisechance").GetComponent<TextMeshProUGUI>().text = Mathf.RoundToInt(GameObject.FindObjectOfType<UIButtons>().CalculateRaiseChance(this) * 100) + "%";
        }
        else
        {
            ann.text += "<color=red>you have gone too far into debt.  you die.";
            lost = true;
            TextMeshProUGUI stattext = GameObject.Find("statustext").GetComponent<TextMeshProUGUI>();
            stattext.transform.GetChild(2).gameObject.SetActive(true);
        }
    }

    public void ResetGame()
    {
        money = 0;
        status = 0;
        promotionLevel = 0;
        dayNum = 0;
        income = 10;
        isDay = false;
        necessities = 8;
        quota = 3;
        itemsBought = new bool[9];
        loanDay = -1;
        lost = false;
    }   
}
