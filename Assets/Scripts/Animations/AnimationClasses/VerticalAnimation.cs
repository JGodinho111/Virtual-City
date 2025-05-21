using UnityEngine;

public class VerticalAnimation : ObjectStartAnimation
{
    public override void SetAnimationParameters()
    {
        rotatingSideways = false;
        rotatingVertically = true;
        comingDown = false;
    }
}
