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

    // Start is called before the first frame update
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
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
                Debug.Log("why");
                triggerToDisable.SetActive(false);
            }
        }
        else { Reset(); }
    }


}
