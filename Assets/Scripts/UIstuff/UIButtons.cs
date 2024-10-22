using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class UIButtons : MonoBehaviour
{
    public bool begged = false;
    public AudioClip[] clips;

    public float CalculateRaiseChance(GameController gc)
    {
        return Mathf.Clamp(0.5f + gc.status * 0.01f - gc.promotionLevel * 0.25f, 0f, 1f);
    }

    public void BegRaise()
    {
        print("clicked button");
        if (begged)
            return;
        begged = true;
        GameController gc = GameObject.FindObjectOfType<GameController>();
        float rand = Random.Range(0f, 1f);
        float chance = CalculateRaiseChance(gc);
        if(chance > rand)
        {
            gc.promotionLevel++;
            gc.income = Mathf.FloorToInt(10 * Mathf.Pow(1.2f, gc.promotionLevel));
            transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = "<color=green>a raise!";
            transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = Mathf.RoundToInt(CalculateRaiseChance(gc) * 100) + "%";
            GetComponent<AudioSource>().clip = clips[0];
            GetComponent<AudioSource>().Play();
            gc.quota++;
        }
        //one third of fails will become a demotion
        else if(rand > 1-((1-chance)/2f) && gc.promotionLevel > 0)
        {
            gc.promotionLevel--;
            gc.income = 10 * Mathf.FloorToInt(Mathf.Pow(1.2f, gc.promotionLevel));
            transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = "<color=red>pay cut";
            transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = Mathf.RoundToInt(CalculateRaiseChance(gc) * 100) + "%";
            GetComponent<AudioSource>().clip = clips[1];
            GetComponent<AudioSource>().Play();
            gc.quota--;
        }
        else
        {
            transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = "<color=white>nothing happens";
            GetComponent<AudioSource>().clip = clips[2];
            GetComponent<AudioSource>().Play();
        }
        GetComponent<Image>().color = Color.black;
    }
    public void NextDay()
    {
        SceneManager.LoadScene("DayScene");
    }

    public void ToNight()
    {
        SceneManager.LoadScene("NightScene");
    }

    public void Restart()
    {
        FindObjectOfType<GameController>().ResetGame();
        SceneManager.LoadScene("MainMenu");
    }

    public void StartGame()
    {
        SceneManager.LoadScene("DayScene");
    }
}
