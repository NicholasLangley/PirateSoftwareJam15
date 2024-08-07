using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishAlter : MonoBehaviour
{
    Transform player;

    [SerializeField]
    GameOverManager gameOverManager;
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
        if (vectorFromPlayer.magnitude < 5 && !gameComplete)
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
        gameOverManager.PlayBadEnd();
        gameComplete = true;
    }

    void GoodEnding()
    {
        gameOverManager.PlayGoodEnd();
        gameComplete = true;
    }


}
