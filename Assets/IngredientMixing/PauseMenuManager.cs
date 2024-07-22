using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenuManager : MonoBehaviour
{
    [SerializeField]
    MenuTile menuTile;

    [SerializeField]
    float tileTransitionDuration;

    bool paused;

    // Start is called before the first frame update
    void Start()
    {
        menuTile.transitionDuration = tileTransitionDuration;

        paused = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Tab) || Input.GetKeyDown(KeyCode.I))
        {
            if (paused) { Unpause(); }
            else { Pause(); }
        }
    }

    public void Pause()
    {
        Time.timeScale = 0f;
        menuTile.ArriveFromBottom();
        paused = true;
    }

    public void Unpause()
    {
        Time.timeScale = 1.0f;
        menuTile.DepartToBottom();
        paused = false;
    }


}
