using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAnimateable
{
    #region animations
    public virtual void changeAnimation() { }

    public virtual void PauseAnimation() { }

    public virtual void UnpauseAnimation() { }

    #endregion
}
