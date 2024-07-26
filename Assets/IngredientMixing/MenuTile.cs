using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuTile : MonoBehaviour
{
    [SerializeField]
    Menu menu;

    [SerializeField]
    Vector3 bottomOffScreenPos, onScreenPos;

    Vector3 destPos, startPos;

    public float transitionDuration;
    float transitionTimer;

    // Start is called before the first frame update
    void Start()
    {
        transform.localPosition = bottomOffScreenPos;
        transitionTimer = 100f;
    }

    // Update is called once per frame
    void Update()
    {
        if (transitionTimer < transitionDuration)
        {
            transitionTimer += Time.unscaledDeltaTime;

            transform.localPosition = Vector2.Lerp(startPos, destPos, transitionTimer / transitionDuration);

            if (transitionTimer >= transitionDuration && transform.position == onScreenPos) { menu.isActive = true; }
        }
    }

    public void ArriveFromBottom()
    {
        startTransition(bottomOffScreenPos, onScreenPos);
    }

    public void DepartToBottom()
    {
        menu.isActive = false;
        startTransition(onScreenPos, bottomOffScreenPos);
        menu.Clear();
    }

    void startTransition(Vector3 start, Vector3 dest)
    {
        startPos = start;
        destPos = dest;
        transitionTimer = 0f;
    }
}
