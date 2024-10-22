using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class checkForGC : MonoBehaviour
{
    public GameObject gameController;

    void Awake()
    {
        if(FindObjectsOfType<GameController>().Length == 0)
        {
            Instantiate(gameController);
            DontDestroyOnLoad(FindObjectOfType<GameController>());
        }
        Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
