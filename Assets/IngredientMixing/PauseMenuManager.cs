using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenuManager : MonoBehaviour
{
    [SerializeField]
    public MenuTile mixingMenuTile, alchemistsNoteMenuTile;
    MenuTile currentTile;

    [SerializeField]
    float tileTransitionDuration;

    bool paused;

    float delayTimer;

    // Start is called before the first frame update
    void Start()
    {
        mixingMenuTile.transitionDuration = tileTransitionDuration;
        alchemistsNoteMenuTile.transitionDuration = tileTransitionDuration;

        paused = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(paused)
        {
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Tab) || Input.GetKeyDown(KeyCode.I) || Input.GetKeyDown(KeyCode.F))
            {
                if (delayTimer > 0.1f)
                { 
                    CloseMenu(); 
                }
            }
            delayTimer += Time.unscaledDeltaTime;
        }

        else
        {
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Tab) || Input.GetKeyDown(KeyCode.I))
            {
                OpenMenu(mixingMenuTile);
            }
            if (Input.GetKeyDown(KeyCode.R))
            {
                OpenMenu(alchemistsNoteMenuTile);
            }
        }
    }

    public void OpenMenu(MenuTile menu)
    {
        Time.timeScale = 0f;
        menu.ArriveFromBottom();
        paused = true;
        currentTile = menu;
        delayTimer = 0f;
    }

    public void CloseMenu()
    {
        Time.timeScale = 1.0f;
        currentTile.DepartToBottom();
        paused = false;
    }


}
