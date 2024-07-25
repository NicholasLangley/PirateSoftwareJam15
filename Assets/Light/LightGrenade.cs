using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightGrenade : MonoBehaviour
{
    [SerializeField]
    LayerMask collisionMask;

    [SerializeField]
    float throwStrength;

    LightSource.LIGHT_TYPE lightType;

    [SerializeField]
    LightSource lightSourcePrefab;

    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    public void Initialize(Vector2 direction, LightSource.LIGHT_TYPE type)
    {
        lightType = type;
        GetComponent<Rigidbody2D>().velocity = direction.normalized * throwStrength;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if ((collisionMask & (1 << collision.gameObject.layer)) != 0)
        {
            Break();
        }
    }

    void Break()
    {
        lightSourcePrefab.currentLightType = lightType;
        LightSource explosion = Instantiate(lightSourcePrefab);
        explosion.transform.position = transform.position;
        Destroy(gameObject);
    }
}
