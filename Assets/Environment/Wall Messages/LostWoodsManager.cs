using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LostWoodsManager : MonoBehaviour
{
    public enum LostWoodsDirection {LEFT, RIGHT, DOWN }

    [SerializeField]
    Sprite leftArrow, DownArrow, RightArrow;
    SpriteRenderer sr;

    [SerializeField]
    List<LostWoodsDirection> solution;

    [SerializeField]
    GameObject triggerToDisable;

    int currentDirectionIndex;
    int resetCount;

    [SerializeField]
    GameObject note, ingredient;

    // Start is called before the first frame update
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        resetCount = -1;
        Reset();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Reset()
    {
        triggerToDisable.SetActive(true);
        currentDirectionIndex = 0;
        DisplayArrow(solution[currentDirectionIndex]);
        resetCount++;
        if(resetCount == 3)
        {
            note.SetActive(true);
            ingredient.SetActive(true);
        }
    }

    void DisplayArrow(LostWoodsDirection dir)
    {
        switch (dir)
        {
            case LostWoodsDirection.LEFT:
                sr.sprite = leftArrow;
                break;
            case LostWoodsDirection.RIGHT:
                sr.sprite = RightArrow;
                break;
            case LostWoodsDirection.DOWN:
                sr.sprite = DownArrow;
                break;
        }
    }

    public void EnterDirection(LostWoodsDirection dir)
    {
        if (dir == solution[currentDirectionIndex])
        {
            currentDirectionIndex++;
            DisplayArrow(solution[currentDirectionIndex]);
            if (currentDirectionIndex == solution.Count - 1)
            {
                triggerToDisable.SetActive(false);
            }
        }
        else { Reset(); }
    }


}
