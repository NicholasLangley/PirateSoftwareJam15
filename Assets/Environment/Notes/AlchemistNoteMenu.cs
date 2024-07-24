using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AlchemistNoteMenu : Menu
{
    [SerializeField]
    TextMeshProUGUI tmp;

    // Start is called before the first frame update
    void Start()
    {
        isActive = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setText(string text)
    {
        tmp.text = text;
    }
}
