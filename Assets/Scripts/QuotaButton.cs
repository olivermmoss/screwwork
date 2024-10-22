
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class QuotaButton : MonoBehaviour
{
    //debug
    public int specificBox = 0;

    BoxCollider2D col;
    SpriteRenderer sr;
    flipNumController flipNum;
    public Sprite unpushed;
    public Sprite pushed;

    public rulesSheetController rsc;

    public List<GameObject> items = new();
    bool pressed;
    public bool exiting;
    public bool entering;

    private float timeFalling;
    private float fallingSpeed;
    private bool seen;

    //0: Puzzle
    //1: Wheel
    private int boxTypes;
    int boxNum; //current box type;
    GameObject activeBox; //curreny box object
    public int day = 2;

    public GameObject sheetPrefab;

    public GameObject wheelPrefab;

    public GameObject sortPrefab;

    public GameObject punchCardPrefab;

    private int[] puzzlePool = new int[] { 1, 2, 4, 5, 7, 9, 11, 13}; //day -2
    public Sprite[] cubeSprites;
    public int colorAmounts; //always 2 or 3
    int width = 3;
    int height = 2;
    public GameObject[] gridPrefabs;

    GameController gc;
    public AudioClip[] clips;
    AudioSource source;

    void Start()
    {
        col = GetComponent<BoxCollider2D>();
        sr = GetComponent<SpriteRenderer>();
        flipNum = GetComponent<flipNumController>();
        gc = FindObjectOfType<GameController>();
        source = GetComponent<AudioSource>();

        day = gc.dayNum;
        flipNum.rightValue = gc.quota;
        flipNum.SetVals();

        /*
        pressed = true;

        //Animation button up
        sr.sprite = unpushed;
        pressed = false;
        col.enabled = false;
        timeFalling = 0f;
        exiting = true;
        */

    }

    private void OnMouseOver()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            pressed = true;
            Draggable[] draggables = FindObjectsOfType(typeof(Draggable)) as Draggable[];

            foreach (Draggable thing in draggables)
            {
                if (!thing.transform.CompareTag("Keep On Screen") && !thing.transform.CompareTag("trinket") && thing.transform.parent == null)
                    items.Add(thing.gameObject);
            }
            //animation button down
            sr.sprite = pushed;

        }


        if (pressed && Input.GetKeyUp(KeyCode.Mouse0))
        {
            NewStuff();
        }
    }




    private void OnMouseExit()
    {
        if (pressed)
        {
            NewStuff();
        }
    }

    private void NewStuff()
    {
        //Animation button up
        sr.sprite = unpushed;

        pressed = false;
        col.enabled = false;
        timeFalling = 0f;
        exiting = true;

        if (activeBox != null)
            Check();
        else if(gc.tasksComplete == 0)
        {
            gc.ClockIn();
        }

    }

    private void Check()
    {

        bool ollKorrect = true;

        Transform t;
        for(int i = 0; i < activeBox.transform.childCount; i++)
        {
            t = activeBox.transform.GetChild(i);
            if ((!t.transform.parent.CompareTag("SortBox") && t.GetComponent<Rotatable>() == null && t.childCount == 0) || (t.childCount > 0 && t.GetChild(0).CompareTag("screw") && !t.GetChild(0).GetComponent<Animator>().GetBool("Screwed")))
            {
                ollKorrect = false;
                break;
            }
        }


        switch (boxNum)
        {
            case 0:

                if (rsc.CheckWheel() && ollKorrect)
                {
                    flipNum.leftValue++;
                    source.clip = clips[0];
                    source.Play();
                }
                else
                {
                    if (flipNum.leftValue > 0)
                        flipNum.leftValue--;

                    source.clip = clips[1];
                    source.Play();
                }

                break;
            case 1:

                int[] tempGrid = CreateGrid(width * height, activeBox);



                if (rsc.CheckGrid(width, height, CreateGrid(width * height, activeBox)) && ollKorrect)
                {
                    //Debug.Log("right :) ");
                    flipNum.leftValue++;
                    source.clip = clips[0];
                    source.Play();
                }
                else 
                {
                    if (flipNum.leftValue > 0)
                        flipNum.leftValue--;

                    source.clip = clips[1];
                    source.Play();
                }
                //else
                    //Debug.Log("wrong but no penalty");

                break;

            case 2:

                if (rsc.CheckSort(activeBox) && ollKorrect)
                {
                    flipNum.leftValue++;
                    source.clip = clips[0];
                    source.Play();
                }
                else
                {
                    if (flipNum.leftValue > 0)
                        flipNum.leftValue--;

                    source.clip = clips[0];
                    source.Play();
                }

                break;
            default:
                break;
        }
        flipNum.SetVals();
        gc.tasksComplete = flipNum.leftValue;
    }

    void FixedUpdate()
    {
        if (exiting && items.Count == 0)
        {
            exiting = false;
            SpawnTask();
            entering = true;
        }

        if (exiting && items.Count > 0)
        {
            timeFalling += Time.deltaTime;

            fallingSpeed = 4.9f * Mathf.Pow(timeFalling, 2); 


            seen = false;

            foreach (GameObject obj in items)
            {
                obj.transform.position += new Vector3(fallingSpeed, 0, 0);
                

                if (obj.GetComponent<Renderer>().isVisible)
                {
                    seen = true;
                }
            }


            if (!seen)
            {
                for (int i = 0; i < items.Count; i++)
                {
                    Destroy(items[i]);
                }
                items.Clear();

                exiting = false;
                SpawnTask();
                entering = true;
            } 
        }






        if (entering)
        {
            timeFalling += Time.deltaTime;

            foreach (GameObject obj in items)
            {
                fallingSpeed = (-obj.transform.position.x)/6;

                obj.transform.position += new Vector3(fallingSpeed, 0, 0);


                if (obj.transform.position.x >= -0.06)
                {
                    obj.transform.position = new Vector3(0, obj.transform.position.y, 0);
                    entering = false;
                    col.enabled = true;
                }
            }
        }
    }



    void SpawnTask()
    {
        timeFalling = 0f;

        if (flipNum.leftValue >= flipNum.rightValue)
        {
            GameObject card = Instantiate(punchCardPrefab, new Vector3(-18, -1, 0), Quaternion.identity);
            GameObject[] objs = GameObject.FindGameObjectsWithTag("PunchHolder");

            card.GetComponent<punchCard>().snapPoints = new Transform[2];

            card.GetComponent<punchCard>().snapPoints[0] = objs[0].transform;
            card.GetComponent<punchCard>().snapPoints[1] = objs[1].transform;

            activeBox = null;
            items.Add(card);

            return;
        }



        if (day >= 3)
            boxTypes = 3;
        else if (day >= 2)
            boxTypes = 2;
        else
            boxTypes = 1;

        boxNum = Random.Range(0, boxTypes);

        if (boxTypes >= 2 && Random.Range(0, 3) == 1) //more likely to be puzzlebox because that one is the most fun and interesting 
            boxNum = 1;


        GameObject sheet = Instantiate(sheetPrefab, new Vector3(-18, -2, 0), Quaternion.identity);
        rsc = sheet.GetComponent<rulesSheetController>();
        items.Add(sheet);

        switch (boxNum)
        {
            case 0: //Wheel 

                activeBox = Instantiate(wheelPrefab, new Vector3(-15, -3.5f, 0), Quaternion.identity);
                items.Add(activeBox);
                rsc.ShowWheelRule(WheelRules());

                break;
            case 1: //Puzzle

                if (day >= 2) //shouldnt ever get here without day being 2 but its a precaution. otherwise could try to access index of -1 in PuzzleRule()
                {
                    //insert code for different sizes

                    int maxPuzzleGrid = 1; //in ascending order 1 though 4
                    int size = 6;

                    if (day >= 7)
                        maxPuzzleGrid = 4;
                    else if (day >= 5)
                        maxPuzzleGrid = 3;
                    else if (day >= 3)
                        maxPuzzleGrid = 2;

                    int curbox = Random.Range(0, maxPuzzleGrid);

                    switch (curbox)
                    {
                        case 1:
                            size = 9; width = 3; height = 3; break;
                        case 2:
                            size = 12; width = 4; height = 3; break;
                        case 3:
                            size = 15; width = 5; height = 3; break;
                        default:
                            size = 6; width = 3; height = 2; break;
                    }



                    activeBox = Instantiate(gridPrefabs[curbox], new Vector3(-15, -3.5f, 0), Quaternion.identity);
                    
                    items.Add(activeBox);

                    int[] rules = PuzzleRules(activeBox, size);
                    rsc.ShowGridRules(rules[0], rules[1], rules[2], rules[3]);
                }
                
                break;

            case 2: //sort box

                activeBox = Instantiate(sortPrefab, new Vector3(-15, -3.5f, 0), Quaternion.identity);
                items.Add(activeBox);
                activeBox = activeBox.transform.GetChild(0).GameObject();
                rsc.ShowSortRules(SortRules());

                break;
            default:
                break;
        }
    }

    //gonna be honest I couldnt tell you whats happening here
    //returns [rule1, rule2, color1, color2]
    int[] PuzzleRules(GameObject obj, int size, int rule = -1)
    {

        if (day >= 7)
            colorAmounts = 3;
        else
            colorAmounts = 2;

        List<int> availColors = new List<int>(); //used to get colors not being used for rules

        for (int i = 0; i < colorAmounts; i++)
            availColors.Add(i);

        if (rule == -1)
        {
            if (day >= puzzlePool.Length + 1)
            {
                rule = Random.Range(0, 14);
            }
            else
                rule = Random.Range(0, puzzlePool[day - 2] + 1);
        }
            

        int ruleColor = availColors[Random.Range(0, availColors.Count)];
        availColors.Remove(ruleColor);
        int otherRuleColor = availColors[Random.Range(0, availColors.Count)];


        List<GameObject> cubes = new();
        List<int> palette = new();

        for (int i = 0; i < size; i++)
        {
            cubes.Add(obj.transform.GetChild(i).GetChild(0).gameObject);
        }


        int color = 0; //used in outdated cases to set color

        int amtRule = 0; //The amount of boxes for rule 1
        int amtRuleAlt = 0; //The amount of boxes for rule 2

        switch (rule)
        {
            //dont let same color touch
            //half (rounded up) or less of cubes are ruleColor
            case 0:

                palette.Clear();

                amtRule = Random.Range(Mathf.CeilToInt(size / 4f), Mathf.CeilToInt(size / 2f) + 1);

                for (int i = 0; i < amtRule; i++)
                {
                    palette.Add(ruleColor);
                }

                for (int i = 0; i < (size - amtRule); i++)
                {
                    palette.Add(availColors[Random.Range(0, availColors.Count)]);
                }

                foreach (GameObject cube in cubes)
                {
                    color = palette[Random.Range(0, palette.Count)];
                    palette.Remove(color);

                    cube.GetComponent<SpriteRenderer>().sprite = cubeSprites[color];

                    cube.tag = ColorTag(color);
                }

                return new int[] { 0, -1, ruleColor, -1 };


            //Horizontal symmetry
            //Even amount of all cube colors. One non-paired cube if odd amount of cells
            case 1:

                palette.Clear();


                while (size - palette.Count >= 2)
                {
                    color = Random.Range(0, colorAmounts);
                    palette.Add(color);
                    palette.Add(color);
                }
                if (size - palette.Count == 1)
                {
                    palette.Add(Random.Range(0, colorAmounts));
                }

                foreach (GameObject cube in cubes)
                {
                    color = palette[Random.Range(0, palette.Count)];
                    palette.Remove(color);

                    cube.GetComponent<SpriteRenderer>().sprite = cubeSprites[color];

                    cube.tag = ColorTag(color);
                }


                return new int[] { 1, -1, ruleColor, -1 };



            //Dont let same color touch (0) AND horizonal symmetry (1)
            //4x3 case (even width means max amount of ruleColor boxes is -2)
            case 2:

                palette.Clear();

                amtRule = 0;

                if (size == 6)
                    amtRule = Random.Range(1, 4);
                else if (size == 9)
                    amtRule = Random.Range(2, 6);
                else if (size == 12)
                    amtRule = (Random.Range(1, 3) * 2);
                else //size == 15
                    amtRule = Random.Range(3, 9);


                for (int i = 0; i < amtRule; i++)
                {
                    palette.Add(ruleColor);
                }

                for (int i = 0; i < (size - amtRule); i++)
                {
                    palette.Add(availColors[Random.Range(0, availColors.Count)]);
                }


                foreach (GameObject cube in cubes)
                {
                    color = palette[Random.Range(0, palette.Count)];
                    palette.Remove(color);

                    cube.GetComponent<SpriteRenderer>().sprite = cubeSprites[color];

                    cube.tag = ColorTag(color);
                }

                return new int[] { 0, 1, ruleColor, ruleColor };


            //Connect all of same color
            //no restrictions
            case 3:

                foreach (GameObject cube in cubes)
                {
                    color = Random.Range(0, colorAmounts);
                    cube.GetComponent<SpriteRenderer>().sprite = cubeSprites[color];

                    cube.tag = ColorTag(color);
                }

                return new int[] { 2, -1, ruleColor, -1 };
            //3, -1, ruleColor, -1


            //Connect all of same color (2) AND horizonal symmetry (1)
            //I dont believe this has additional restrictions
            case 4:

                palette.Clear();

                amtRule = 0;

                if (size == 6)
                    amtRule = Random.Range(3, 6);
                else if (size == 9)
                    amtRule = Random.Range(3, 8);
                else if (size == 12)
                    amtRule = (Random.Range(3, 5) * 2) + 1;
                else //size == 15
                    amtRule = Random.Range(5, 12);


                for (int i = 0; i < amtRule; i++)
                {
                    palette.Add(ruleColor);
                }


                while (size - palette.Count >= 2)
                {
                    color = availColors[Random.Range(0, availColors.Count)];
                    palette.Add(color);
                    palette.Add(color);
                }
                if (size - palette.Count == 1)
                {
                    palette.Add(availColors[Random.Range(0, availColors.Count)]);
                }

                foreach (GameObject cube in cubes)
                {
                    color = palette[Random.Range(0, palette.Count)];
                    palette.Remove(color);

                    cube.GetComponent<SpriteRenderer>().sprite = cubeSprites[color];

                    cube.tag = ColorTag(color);
                }

                return new int[] { 2, 1, ruleColor, ruleColor };
            //3, 1, ruleColor, ruleColor

            //Vertical symmetry
            //same restrictions as horizontal (1)
            case 5:

                palette.Clear();

                while (size - palette.Count >= 2)
                {
                    color = Random.Range(0, colorAmounts);
                    palette.Add(color);
                    palette.Add(color);
                }
                if (size - palette.Count == 1)
                {
                    palette.Add(Random.Range(0, colorAmounts));
                }

                foreach (GameObject cube in cubes)
                {
                    color = palette[Random.Range(0, palette.Count)];
                    palette.Remove(color);

                    cube.GetComponent<SpriteRenderer>().sprite = cubeSprites[color];

                    cube.tag = ColorTag(color);
                }

                return new int[] { 3, -1, ruleColor, -1 };
            //5, -1, ruleColor, -1

            //All groups size 3
            //ruleColor blocks must be multiple of 3.

            //3x2: 1 group max
            //3x3: 2 groups max
            //4x3: 2 groups max
            //5x3: 3 groups max
            case 6:
                palette.Clear();

                amtRule = 0;

                if (size == 6)
                    amtRule = 1;
                else if (size == 9)
                    amtRule = Random.Range(1, 3);
                else if (size == 12)
                    amtRule = Random.Range(1, 3);
                else //size == 15
                    amtRule = Random.Range(1, 4);

                amtRule *= 3;

                for (int i = 0; i < amtRule; i++)
                {
                    palette.Add(ruleColor);
                }

                for (int i = 0; i < (size - amtRule); i++)
                {
                    palette.Add(availColors[Random.Range(0, availColors.Count)]);
                }


                foreach (GameObject cube in cubes)
                {
                    color = palette[Random.Range(0, palette.Count)];
                    palette.Remove(color);

                    cube.GetComponent<SpriteRenderer>().sprite = cubeSprites[color];

                    cube.tag = ColorTag(color);
                }

                return new int[] { 4, -1, ruleColor, -1 };
            //6, -1, ruleColor, -1


            //No forming 3-in-a-row of certain color
            //need enough cells of not that color.  3x2: 2, 3x3: 3, 4x3: 4, 5x3: 5

            case 7:

                if (size == 6)
                {
                    amtRule = Random.Range(3, 5);
                }
                else if (size == 9)
                {
                    amtRule = Random.Range(4, 7);
                }
                else if (size == 12)
                {
                    amtRule = Random.Range(5, 9);
                }
                else //if (size == 15)
                {
                    amtRule = Random.Range(6, 11);
                }

                for (int i = 0; i < amtRule; i++)
                {
                    palette.Add(ruleColor);
                }

                for (int i = 0; i < size - amtRule; i++)
                {
                    palette.Add(availColors[Random.Range(0, availColors.Count)]);
                }


                foreach (GameObject cube in cubes)
                {
                    color = palette[Random.Range(0, palette.Count)];
                    palette.Remove(color);

                    cube.GetComponent<SpriteRenderer>().sprite = cubeSprites[color];

                    cube.tag = ColorTag(color);
                }

                return new int[] { 5, -1, ruleColor, -1 };
            //7, -1, ruleColor, -1

            //Dont let the same color touch (0) AND groups size 3 of OTHER color (4)
            //if just red and blue, max independent cells are 3x2:2, 3x3:4, 5x3:5
            //if rby, sufficient non rule color must be added
            case 8:

                availColors.Remove(otherRuleColor);
                palette.Clear();


                amtRule = 0; //otherRuleColor, group size 3
                amtRuleAlt = 3; //ruleColor, dont let touch

                if (size == 6)
                {
                    amtRule = 3;
                    amtRuleAlt = Random.Range(1, 3);
                }
                else if (size == 9)
                {
                    amtRule = Random.Range(1, 3) * 3;

                    if (amtRule == 6)
                        amtRuleAlt = Random.Range(1, 4);
                    else // == 3
                        amtRuleAlt = Random.Range(1, 5);
                }
                else if (size == 12)
                {
                    amtRule = Random.Range(1, 3) * 3;

                    if (amtRule == 6)
                        amtRuleAlt = Random.Range(1, 5);
                    else // == 3
                        amtRuleAlt = Random.Range(1, 6);
                }
                else //size == 15
                {
                    amtRule = Random.Range(1, 4) * 3;

                    if (amtRule == 9)
                        amtRuleAlt = Random.Range(1, 6);
                    else if (amtRule == 6)
                        amtRuleAlt = Random.Range(1, 7);
                    else // == 3
                        amtRuleAlt = Random.Range(1, 8);

                }

                for (int i = 0; i < amtRule; i++)
                {
                    palette.Add(otherRuleColor);
                }

                for (int i = 0; i < amtRuleAlt; i++)
                {
                    palette.Add(ruleColor);
                }

                for (int i = 0; i < size - (amtRule + amtRuleAlt); i++)
                {
                    palette.Add(availColors[0]);
                }


                foreach (GameObject cube in cubes)
                {
                    color = palette[Random.Range(0, palette.Count)];
                    palette.Remove(color);

                    cube.GetComponent<SpriteRenderer>().sprite = cubeSprites[color];

                    cube.tag = ColorTag(color);
                }

                return new int[] { 0, 4, ruleColor, otherRuleColor };


            //Groups size 3 of two colors
            //requires yellow as filler
            case 9:

                palette.Clear();

                if (size == 6)
                {
                    ruleColor = 3;
                    otherRuleColor = 3;
                }

                else if (size == 9)
                {
                    amtRule = Random.Range(1, 3) * 3;

                    amtRuleAlt = 3;
                }

                else if (size == 12)
                {
                    amtRule = Random.Range(1, 3) * 3;

                    amtRuleAlt = Random.Range(1, 3) * 3;
                }

                else //if (size == 15)
                {
                    amtRule = Random.Range(1, 4) * 3;

                    amtRuleAlt = Random.Range(1, 3) * 3;
                }

                for (int i = 0; i < amtRule; i++)
                    palette.Add(ruleColor);

                for (int i = 0; i < amtRuleAlt; i++)
                    palette.Add(otherRuleColor);

                for (int i = 0; i < size - (amtRule + amtRuleAlt); i++)
                    palette.Add(availColors[Random.Range(0, availColors.Count)]);

                foreach (GameObject cube in cubes)
                {
                    color = palette[Random.Range(0, palette.Count)];
                    palette.Remove(color);

                    cube.GetComponent<SpriteRenderer>().sprite = cubeSprites[color];

                    cube.tag = ColorTag(color);
                }

                return new int[] { 4, 4, ruleColor, otherRuleColor };


            //all groups size 3 AND horizontal symmetry
            //same as just groups of 3, make sure leftovers follow rules
            case 10:

                palette.Clear();

                amtRule = 0;

                if (size == 6)
                    amtRule = 1;
                else if (size == 9)
                    amtRule = Random.Range(1, 3);
                else if (size == 12)
                    amtRule = Random.Range(1, 3);
                else //size == 15
                    amtRule = Random.Range(1, 4);

                amtRule *= 3;

                for (int i = 0; i < amtRule; i++)
                {
                    palette.Add(ruleColor);
                }

                while (size - palette.Count >= 2)
                {
                    color = availColors[Random.Range(0, availColors.Count)];
                    palette.Add(color);
                    palette.Add(color);
                }
                if (size - palette.Count == 1)
                    palette.Add(availColors[Random.Range(0, availColors.Count)]);

                foreach (GameObject cube in cubes)
                {
                    color = palette[Random.Range(0, palette.Count)];
                    palette.Remove(color);

                    cube.GetComponent<SpriteRenderer>().sprite = cubeSprites[color];

                    cube.tag = ColorTag(color);
                }

                return new int[] { 4, 1, ruleColor, ruleColor };


            //No forming 2x2 of certain color
            //non rule colors needed: 3x2:1, 3x3:1, 4x3:2, 5x3:2
            case 12:

                palette.Clear();

                if (size == 6)
                    amtRule = Random.Range(4, 6);
                else if (size == 9)
                    amtRule = Random.Range(4, 9);
                else if (size == 12)
                    amtRule = Random.Range(7, 11);
                else //size == 15
                    amtRule = Random.Range(8, 14);

                for (int i = 0; i < amtRule; i++)
                    palette.Add(ruleColor);

                for (int i = 0; i < size - amtRule; i++)
                    palette.Add(availColors[Random.Range(0, availColors.Count)]);

                foreach (GameObject cube in cubes)
                {
                    color = palette[Random.Range(0, palette.Count)];
                    palette.Remove(color);

                    cube.GetComponent<SpriteRenderer>().sprite = cubeSprites[color];

                    cube.tag = ColorTag(color);
                }

                return new int[] { 6, -1, ruleColor, -1 };


            //Connect all of same color (2) AND no 3-in-a-row in that color (5)
            //need enough cells of not that color.  3x2: 2, 3x3: 4, 4x3: 6, 5x3: 9
            case 13:

                palette.Clear();

                if (size == 6)
                {
                    amtRule = Random.Range(3, 5);
                }
                else if (size == 9)
                {
                    amtRule = Random.Range(4, 6);
                }
                else if (size == 12)
                {
                    amtRule = Random.Range(5, 7);
                }
                else //if size == 15
                {
                    amtRule = Random.Range(5, 7);
                }

                for (int i = 0; i < amtRule; i++)
                    palette.Add(ruleColor);

                for (int i = 0; i < size - amtRule; i++)
                    palette.Add(availColors[Random.Range(0, availColors.Count)]);

                foreach (GameObject cube in cubes)
                {
                    color = palette[Random.Range(0, palette.Count)];
                    palette.Remove(color);

                    cube.GetComponent<SpriteRenderer>().sprite = cubeSprites[color];

                    cube.tag = ColorTag(color);
                }

                return new int[] {2, 5, ruleColor, ruleColor };


            default:
                Debug.Log("Rule not working");
                return new int[]{ 1, -1, 1, -1 };
        }

    }

    float WheelRules()
    {
        if (day >= 11)
            return (Random.Range(1, (4 * 11) + 1) / 4f) * Mathf.RoundToInt(Mathf.Lerp(-1, 1, Random.Range(0, 2)));

        else
            return (Random.Range(1, (4 * day) + 1) / 4f) * Mathf.RoundToInt(Mathf.Lerp(-1, 1, Random.Range(0, 2)));
    }

    int[] SortRules() //screwNumRed, springNumRed, screwNumBlue, springNumBlue
    {
        int maxNum = 4;

        maxNum = day >= 10 ? 10 : day;

        int screwNumRed = Random.Range(1, maxNum + 1);
        int springNumRed = 0;

        if (day >= 4)
        {
            springNumRed = Random.Range(0, maxNum + 1);
        }

        int screwNumBlue = 0;
        int springNumBlue = 0;

        if (day >= 5)
        {
            screwNumBlue = Random.Range(1, maxNum + 1);
            springNumBlue = Random.Range(0, maxNum + 1);
        }

        return new int[] { screwNumRed, springNumRed, screwNumBlue, springNumBlue };
    }

    string ColorTag(int n)
    {
        if (n == 2)
            return "Yellow Cube";
        else if (n == 1)
            return "Blue Cube";
        else
            return "Red Cube";
    }

    int ColorToInt(string n)
    {
        if (n == "Red Cube")
            return 0;
        else if (n == "Blue Cube")
            return 1;
        else if (n == "Yellow Cube")
            return 2;

        else return -1;
    }

    int[] CreateGrid(int size, GameObject box)
    {
        int[] gridList = new int[size];

        for (int i = 0; i < size; i++)
        {
            gridList[i] = ColorToInt(box.transform.GetChild(i).GetChild(0).gameObject.tag);
        }

        return gridList;
    }
}
