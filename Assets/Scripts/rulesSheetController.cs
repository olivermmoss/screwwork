using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rulesSheetController : Draggable
{
    public int rule1 = -1;
    public int rule2 = -1;
    public int color1 = -1;
    public int color2 = -1;
    public Sprite[] puzzleRuleSprites;
    public Sprite[] rotationSprites;
    public Sprite[] sortSprites;
    public Sprite[] otherSprites;
    public SpriteRenderer rule1sr;
    public SpriteRenderer rule2sr;
    public SpriteRenderer clockwiseOrWiddershins;
    public float expectedRots = 0;
    public float rotationTolerance;
    public int[] sortRules;
    public SpriteRenderer[] sortingList;

    protected override void OnGrabbed()
    {
        for(int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).GetComponent<SpriteRenderer>().sortingOrder = sr.sortingOrder + 1;
        }
    }

    public override void SendBack()
    {
        base.SendBack();
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).GetComponent<SpriteRenderer>().sortingOrder = sr.sortingOrder + 1;
        }
    }

    public bool CheckWheel()
    {
        return Mathf.Abs(FindObjectOfType<Rotatable>().CWSpinFromStart() - expectedRots) < rotationTolerance;
    }

    public bool CheckGrid(int width, int height, int[] grid)
    {
        int rule = rule1;
        int color = color1;
        while (true)
        {
            switch(rule)
            {
                //no touchy same color
                case 0:
                    for(int i = 0; i < grid.Length; i++)
                    {
                        if (grid[i] == color)
                        {
                            //up
                            if (InRange(i - width, 0, grid.Length))
                                if (grid[i - width] == color)
                                    return false;
                            //down
                            if (InRange(i + width, 0, grid.Length))
                                if (grid[i + width] == color)
                                    return false;
                            //left
                            if (InRange(i - 1, 0, grid.Length) && (i % width) > 0)
                                if (grid[i - 1] == color)
                                    return false;
                            //right
                            if (InRange(i + 1, 0, grid.Length) && (i % width) < width - 1)
                                if (grid[i + 1] == color)
                                    return false;
                        }
                    }
                    break;
                //horizontal symmetry    
                case 1:
                    for (int i = 0; i < grid.Length; i++)
                    {
                        //trust me lol
                        if (grid[i - (i % width) + width - 1 - (i % width)] != grid[i])
                            return false;
                    }
                    break;
                //all connecting 
                case 2:
                    if (SeparateIntoGroups(width, height, grid, color).Count > 1)
                        return false;
                    break;
                //vertical symmetry    
                case 3:
                    for (int i = 0; i < grid.Length; i++)
                    {
                        //trust me lol
                        if (grid[grid.Length - width - (i - (i % width)) + (i % width)] != grid[i])
                            return false;
                    }
                    break;
                //all groups size 3   
                case 4:
                    foreach(List<int> group in SeparateIntoGroups(width, height, grid, color))
                    {
                        if (group.Count != 3)
                            return false;
                    }
                    break;
                //rotational symmetry    
                case 5:
                    for (int i = 0; i < grid.Length; i++)
                    {
                        //this one works really nicely
                        if (grid[grid.Length - 1 - i] != grid[i])
                            return false;
                    }
                    break;
                //no 2x2s   
                case 6:
                    for (int x = 0; x < width - 1; x++)
                    {
                        for (int y = 0; y < height - 1; y++)
                        {
                            int i = y * width + x;
                            if (grid[i + 1] == grid[i] && grid[i + width] == grid[i] && grid[i + 1 + width] == grid[i] && grid[i] == color)
                                return false;
                        }
                    }
                    break;
                //no 3 in a rows   
                case 7:
                    //horizontal
                    for (int x = 0; x < width - 2; x++)
                    {
                        for (int y = 0; y < height; y++)
                        {
                            int i = y * width + x;
                            if (grid[i + 1] == grid[i] && grid[i + 2] == grid[i] && grid[i] == color)
                                return false;
                        }
                    }
                    //vertical
                    for (int x = 0; x < width; x++)
                    {
                        for (int y = 0; y < height - 2; y++)
                        {
                            int i = y * width + x;
                            if (grid[i + width] == grid[i] && grid[i + 2 * width] == grid[i])
                                return false;
                        }
                    }
                    break;
                default:
                    Debug.LogWarning("Rule not defined in switch statement!!!");
                    break;
            }

            if(rule == rule2 || rule2 == -1)
            {
                return true;
            }
            rule = rule2;
            color = color2;
        }
    }

    public bool CheckSort(GameObject obj)
    {
        SortBox sb = obj.GetComponent<SortBox>();

        if (sb.redSide[0] != sortRules[0])
            return false;
        if (sb.redSide[1] != sortRules[1])
            return false;
        if (sb.blueSide[0] != sortRules[2])
            return false;
        if (sb.blueSide[1] != sortRules[3])
            return false;

        return true;
    }

    public bool InRange(int x, int min, int max)
    {
        return (x >= min) && (x < max);
    }

    //really messy lol.  might work :shrug:
    public List<List<int>> SeparateIntoGroups(int width, int height, int[] grid, int color)
    {
        List<List<int>> groups = new List<List<int>> { };

        for (int i = 0; i < grid.Length; i++)
        {
            if (grid[i] != color)
                continue;
            else
                groups.Add(new List<int> { i });
        }
        for (int i = 0; i < grid.Length; i++)
        {
            if (grid[i] != color)
                continue;

            //up
            if (InRange(i - width, 0, grid.Length))
            {
                if (grid[i - width] == color)
                {
                    int targetList = SearchListList(groups, i - width);
                    if (targetList != -1)
                    {
                        CombineLists(groups, SearchListList(groups, i), targetList);
                    }
                }
            }
            //down
            if (InRange(i + width, 0, grid.Length))
            {
                if (grid[i + width] == color)
                {
                    int targetList = SearchListList(groups, i + width);
                    if (targetList != -1)
                    {
                        CombineLists(groups, SearchListList(groups, i), targetList);
                    }
                }
            }
            //left
            if (InRange(i - 1, 0, grid.Length) && i % width > 0)
            {
                if (grid[i - 1] == color)
                {
                    int targetList = SearchListList(groups, i - 1);
                    if (targetList != -1)
                    {
                        CombineLists(groups, SearchListList(groups, i), targetList);
                    }
                }
            }
            //right
            if (InRange(i + 1, 0, grid.Length) && i % width < width - 1)
            {
                if (grid[i + 1] == color)
                {
                    int targetList = SearchListList(groups, i + 1);
                    if (targetList != -1)
                    {
                        CombineLists(groups, SearchListList(groups, i), targetList);
                    }
                }
            }
        }

        return groups;
    }

    int SearchListList(List<List<int>> ll, int target)
    {
        for(int i = 0; i < ll.Count; i++)
        {
            for(int j = 0; j < ll[i].Count; j++)
            {
                if (ll[i][j] == target)
                    return i;
            }
        }
        return -1;
    }
    void CombineLists(List<List<int>> ll, int l1, int l2)
    {
        if (l1 == l2)
            return;

        ll[l1].AddRange(ll[l2]);
        ll.RemoveAt(l2);
    }

    public void ShowGridRules(int rule1, int rule2, int color1, int color2)
    {
        this.rule1 = rule1;
        this.rule2 = rule2;
        this.color1 = color1;
        this.color2 = color2;

        rule1sr.sprite = puzzleRuleSprites[rule1 * 3 + color1];
        if(rule2 != -1)
            rule2sr.sprite = puzzleRuleSprites[rule2 * 3 + color2];
    }

    //positive is clockwise
    public void ShowWheelRule(float rotations)
    {
        this.expectedRots = rotations;

        rule1sr.sprite = rotationSprites[Mathf.RoundToInt(Mathf.Abs(rotations) * 4 - 1)];
        if(rotations > 0)
        {
            clockwiseOrWiddershins.sprite = otherSprites[0];
        }
        if (rotations < 0)
        {
            //easter egg
            int d100 = Random.Range(0, 100);
            if(d100 == 0)
                clockwiseOrWiddershins.sprite = otherSprites[2];
            else
                clockwiseOrWiddershins.sprite = otherSprites[1];
        }
    }

    public void ShowSortRules(int[] rules)
    {
        this.sortRules = rules;

        rule1sr.sprite = sortSprites[20];
        sortingList[0].sprite = sortSprites[22];
        sortingList[1].sprite = sortSprites[sortRules[0] - 1];

        if (sortRules[1] > 0)
        {
            sortingList[2].sprite = sortSprites[sortRules[1] + 9];
            sortingList[3].sprite = sortSprites[21];
        }

        if (sortRules[2] > 0)
        {
            rule2sr.sprite = sortSprites[20];
            sortingList[4].sprite = sortSprites[23];
            sortingList[5].sprite = sortSprites[sortRules[2] - 1];

            if (sortRules[3] > 0)
            {
                sortingList[6].sprite = sortSprites[sortRules[3] + 9];
                sortingList[7].sprite = sortSprites[21];
            }
        }
    }

    protected override void Update()
    {
        base.Update();

        /*Transform box = GameObject.FindGameObjectWithTag("puzzleBox").transform;
        for(int i = 0; i < 15; i++)
        {
            if (box.GetChild(i).childCount == 0)
                return;
        }

        int[] grid = new int[15];
        for (int i = 0; i < 15; i++)
        {
            if (box.GetChild(i).GetChild(0).CompareTag("Red Cube"))
                grid[i] = 0;
            if (box.GetChild(i).GetChild(0).CompareTag("Blue Cube"))
                grid[i] = 1;
            if (box.GetChild(i).GetChild(0).CompareTag("Yellow Cube"))
                grid[i] = 2;
        }
        print(CheckGrid(5, 3, grid));*/
    }
}
