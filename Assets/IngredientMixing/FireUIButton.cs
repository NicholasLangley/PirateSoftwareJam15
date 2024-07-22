using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FireUIButton : MonoBehaviour
{
    public LightSource.LIGHT_TYPE lightType;

    [SerializeField]
    Sprite mundaneSprite, magicalSprite, silverSprite, redSprite, blackSprite, divineSprite;

    // Start is called before the first frame update
    void OnEnable()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetFireType(LightSource.LIGHT_TYPE type)
    {
        lightType = type; ;
        switch (type)
        {
            case LightSource.LIGHT_TYPE.mundane:
                GetComponent<Image>().sprite = mundaneSprite;
                break;
            case LightSource.LIGHT_TYPE.magical:
                GetComponent<Image>().sprite = magicalSprite;
                break;
            case LightSource.LIGHT_TYPE.silver:
                GetComponent<Image>().sprite = silverSprite;
                break;
            case LightSource.LIGHT_TYPE.red:
                GetComponent<Image>().sprite = redSprite;
                break;
            case LightSource.LIGHT_TYPE.black:
                GetComponent<Image>().sprite = blackSprite;
                break;
            case LightSource.LIGHT_TYPE.divine:
                GetComponent<Image>().sprite = divineSprite;
                break;
        }

       
    }

}

