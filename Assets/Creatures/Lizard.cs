using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lizard : MonoBehaviour
{   
    [SerializeField]
    HeadController lizardHead;

    [SerializeField]
    float movementSpeed;

    [SerializeField]
    Transform player;

    bool running;

    Vector3 startDirection, goalDirection, currentDirection;
    float lerpTimer;
    [SerializeField]
    float lerpDuration, movementAngleVarience, movementAngleFactor;

    // Start is called before the first frame update
    void Start()
    {
        running = false;
        currentDirection = Random.insideUnitCircle;
        currentDirection.Normalize();
        lerpTimer = 0;
        if (player == null) { player = GameObject.FindGameObjectWithTag("Player").transform; }
    }

    // Update is called once per frame
    void Update()
    {
        lerpTimer += Time.deltaTime;
        //run away
        if (running)
        {
            currentDirection = Vector3.Lerp(startDirection, goalDirection, lerpTimer / lerpDuration);

            Vector3 finalDirection = Quaternion.Euler(0, 0, Mathf.Sin(lerpTimer * movementAngleFactor) * movementAngleVarience) * currentDirection;

            lizardHead.setHeadPosition(lizardHead.headPosition + finalDirection.normalized * Time.deltaTime * movementSpeed );
        }
        //slow circles
        else
        {
            currentDirection = Quaternion.Euler(0, 0, 45 * Time.deltaTime + Mathf.Sin(lerpTimer * 10)) * currentDirection;
            lizardHead.setHeadPosition(lizardHead.headPosition + currentDirection.normalized * Time.deltaTime * movementSpeed / 8.0f);
        }
        
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        Vector3 vectorFromPlayer = lizardHead.headPosition + transform.position - player.position;
        vectorFromPlayer.z = 0;
        if (vectorFromPlayer.magnitude < 7)
        {
            running = true;
            goalDirection = vectorFromPlayer;
            startDirection = currentDirection;
            lerpTimer = 0;
        }
    }

    public void Reset()
    {
        running = false;
        lizardHead.setHeadPosition(Vector3.zero);
        currentDirection = Random.insideUnitCircle;
        currentDirection.Normalize();
    }
}
