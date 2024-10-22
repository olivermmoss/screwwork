using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class flipNumController : MonoBehaviour
{
    public Sprite[] sprites;
    public SpriteRenderer[] flipNums;
    public int leftValue;
    public int rightValue;
    public int[] fourValues;
    public int[] fourIndices = new int[] { 0, 0, 0, 0 };

    private void Start()
    {
        fourValues = new int[] { leftValue / 10, leftValue % 10, rightValue / 10, rightValue % 10 };
    }

    public void SetVals()
    {
        fourValues[0] = leftValue / 10;
        fourValues[1] = leftValue % 10;
        fourValues[2] = rightValue / 10;
        fourValues[3] = rightValue % 10;
    }

    private void FixedUpdate()
    {
        for(int i = 0; i < 4; i++)
        {
            if(fourIndices[i] != fourValues[i] * 4)
            {
                fourIndices[i] = (fourIndices[i] + 1) % 40;
                flipNums[i].sprite = sprites[fourIndices[i]];
            }
        }
    }
}
