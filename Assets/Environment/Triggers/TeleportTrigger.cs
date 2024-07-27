using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportTrigger : Trigger
{
    [SerializeField]
    Vector3 destination;

    [SerializeField]
    LostWoodsManager lwManager;
    [SerializeField]
    LostWoodsManager.LostWoodsDirection dir;

    public override void TriggerEffects(PlayerController player)
    {
        Debug.Log("TP");
        //offset based on where character is from center of zone
        Vector3 newPlayerPos = destination + (player.transform.position - transform.position);
        newPlayerPos.z = player.transform.position.z;
        player.transform.position = newPlayerPos;

        Vector3 newCameraPos = destination + (Camera.main.transform.position - transform.position);
        newCameraPos.z = Camera.main.transform.position.z;
        Camera.main.transform.position = newCameraPos;

        if(lwManager != null)
        {
            lwManager.EnterDirection(dir);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
