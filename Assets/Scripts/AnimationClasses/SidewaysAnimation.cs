using UnityEngine;

public class SidewaysAnimation : ObjectStartAnimation
{
    public override void SetAnimationParameters()
    {
        rotatingSideways = true;
        rotatingVertically = false;
        comingDown = false;
    }
}
