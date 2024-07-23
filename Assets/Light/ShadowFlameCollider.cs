using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowFlameCollider : MonoBehaviour
{
    [SerializeField]
    float shrinkDuration;
    float shrinkTimer;
    float initialHeight;
    [SerializeField]
    float yDriftVelocity, xScaleGrowthRate;

    // Start is called before the first frame update
    void Start()
    {
        shrinkTimer = 0;
        initialHeight = transform.localScale.y;
    }

    // Update is called once per frame
    void Update()
    {
        Shrink();
    }

    void Shrink()
    {
        shrinkTimer += Time.deltaTime;
        Vector3 newScale = transform.localScale;
        newScale.y = Mathf.Lerp(initialHeight, 0, shrinkTimer / shrinkDuration);
        newScale.x += xScaleGrowthRate * Time.deltaTime;

        if (newScale.y <= 0.1)
        {
            Destroy(gameObject);
            return;
        }

        transform.localScale = newScale;

        Vector3 pos = transform.position;
        pos.y += yDriftVelocity * Time.deltaTime;
        transform.position = pos;
    }
}
