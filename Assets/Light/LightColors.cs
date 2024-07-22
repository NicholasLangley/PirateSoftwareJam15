using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/LightColors")]
public class LightColors : ScriptableObject
{
    [SerializeField]
    List<Color> colors;

    public Color GetColor(LightSource.LIGHT_TYPE type)
    {
        switch (type)
        {
            case LightSource.LIGHT_TYPE.mundane:
                return colors[0];
            case LightSource.LIGHT_TYPE.magical:
                return colors[1];
            case LightSource.LIGHT_TYPE.silver:
                return colors[2];
            case LightSource.LIGHT_TYPE.red:
                return colors[3];
            case LightSource.LIGHT_TYPE.black:
                return colors[4];
            case LightSource.LIGHT_TYPE.divine:
                return colors[5];
            default:
                return colors[0];
        }
    }
}

