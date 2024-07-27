using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sentinel : MonoBehaviour
{
    [SerializeField]
    HeadController sentinalHead, bodyHead;

    [SerializeField]
    float movementSpeed;

    [SerializeField]
    Transform player;

    [SerializeField]
    Vector3 tailAnchorPos;

    [SerializeField]
    LightSource lantern;

    bool isAwake;
    // Start is called before the first frame update
    void Start()
    {
        isAwake = true;
        if (player == null) { player = GameObject.FindGameObjectWithTag("Player").transform; }
        bodyHead.anchorPos = tailAnchorPos;
    }

    // Update is called once per frame
    void Update()
    {
        if(isAwake)
        {
            bool isPlayerOnAnchorRight = player.transform.position.x >= tailAnchorPos.x;
            bool isPlayerRightUnderAnchor = Mathf.Abs(player.transform.position.x - tailAnchorPos.x) < 6;

            float posModifier = isPlayerRightUnderAnchor ? 6 : 1;

            Vector3 goalPos = player.transform.position + Vector3.up * posModifier + (isPlayerOnAnchorRight ? Vector3.left * 4 / posModifier : Vector3.right * 4 / posModifier);
            goalPos.z = bodyHead.transform.position.z;
            goalPos.x = PlayerController.expDecay(bodyHead.head.transform.position.x, goalPos.x, 8, Time.deltaTime);
            goalPos.y = PlayerController.expDecay(bodyHead.head.transform.position.y, goalPos.y, 8, Time.deltaTime);
            bodyHead.setHeadPosition(goalPos);

            sentinalHead.anchorPos = bodyHead.head.transform.position;
            goalPos = player.transform.position + Vector3.up * 2.5f + (isPlayerOnAnchorRight ? Vector3.left * 2.5f / posModifier : Vector3.right * 2.5f / posModifier);
            goalPos.z = sentinalHead.transform.position.z;
            goalPos.x = PlayerController.expDecay(sentinalHead.head.transform.position.x, goalPos.x, 12, Time.deltaTime);
            goalPos.y = PlayerController.expDecay(sentinalHead.head.transform.position.y, goalPos.y, 12, Time.deltaTime);
            sentinalHead.setHeadPosition(goalPos);

            if (player.transform.position.x - tailAnchorPos.x > -15)
            {
                lantern.changeLightType(LightSource.LIGHT_TYPE.red);
            }
            else { lantern.changeLightType(LightSource.LIGHT_TYPE.silver); }

            Vector3 aimDirection = player.transform.position - sentinalHead.head.transform.position;
            aimDirection.z = 0;
            lantern.aimDirection = aimDirection.normalized;
        }

        if(Input.GetKeyDown(KeyCode.G))
        {
            isAwake = true;
        }
    }
}
