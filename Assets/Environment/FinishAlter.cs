using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishAlter : MonoBehaviour
{
    Transform player;

    bool gameComplete;
    // Start is called before the first frame update
    void Start()
    {
        if (player == null) { player = GameObject.FindGameObjectWithTag("Player").transform; }
        gameComplete = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        Vector3 vectorFromPlayer = transform.position - player.position;
        vectorFromPlayer.z = 0;
        if (vectorFromPlayer.magnitude < 3 && !gameComplete)
        {
            if (collision.gameObject.CompareTag("RedLight"))
            {
                BadEnding();
            }
            else if (collision.gameObject.CompareTag("DivineLight"))
            {
                GoodEnding();
            }
        }
    }

    void BadEnding()
    {
        Debug.Log("BAD END");
        gameComplete = true;
    }

    void GoodEnding()
    {
        Debug.Log("GOOD END");
        gameComplete = true;
    }


}
