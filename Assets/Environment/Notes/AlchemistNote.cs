using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlchemistNote : MonoBehaviour
{
    [SerializeField]
    PauseMenuManager menuManager;
    [SerializeField]
    AlchemistNoteMenu noteMenu;
    [SerializeField]
    [TextArea]
    string text;

    public bool isActive;
    public float activeSizeModifier;
    float timer;
    [SerializeField]
    float growthTime;
    // Start is called before the first frame update
    void Start()
    {
        isActive = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(timer < growthTime)
        {
            timer += Time.deltaTime;
            float scale;

            if (isActive) { scale = Mathf.Lerp(1.0f, activeSizeModifier, timer / growthTime); }
            else { scale = Mathf.Lerp(activeSizeModifier, 1.0f, timer / growthTime); }

            transform.localScale = new Vector3(scale, scale, 1);
        }

        if (Input.GetKeyDown(KeyCode.F) && isActive && Time.timeScale > 0)
        {
            Read();
        }
    }

    public void Read()
    {
        noteMenu.setText(text);
        menuManager.OpenMenu(menuManager.alchemistsNoteMenuTile);
    }

    public void Activate()
    {
        isActive = true;
        timer = 0;
    }

    public void Deactivate()
    {
        isActive = false;
        timer = 0;
    }
}