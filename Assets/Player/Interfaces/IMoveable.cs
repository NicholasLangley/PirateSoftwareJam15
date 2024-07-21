using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMoveable
{

    bool _isFacingRight { get; set; }

    void Move();

    void checkDirectionToFace(float accel);
}
