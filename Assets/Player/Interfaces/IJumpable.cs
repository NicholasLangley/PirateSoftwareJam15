using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IJumpable
{
    bool _isJumping { get; set; }

    float _coyoteTime { get; set; }

    float _coyoteTimer { get; set; }

    void Jump();
}
