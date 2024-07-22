using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HiddenObjectManager : MonoBehaviour
{
    [SerializeField]
    List<GameObject> hiddenObjects;

    // Start is called before the first frame update
    void Start()
    {
        DisableAll();
    }


    public void EnableAll()
    {
        foreach(GameObject go in hiddenObjects)
        {
            go.SetActive(true);
        }
    }

    public void DisableAll()
    {
        foreach (GameObject go in hiddenObjects)
        {
            go.SetActive(false);
        }
    }
}
