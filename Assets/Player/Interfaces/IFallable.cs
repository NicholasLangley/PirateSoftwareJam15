using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IFallable
{
    LayerMask _groundLayers { get; set; }
    bool _isGrounded { get; set; }
    void CheckIfGrounded();

    void Fall();
}
