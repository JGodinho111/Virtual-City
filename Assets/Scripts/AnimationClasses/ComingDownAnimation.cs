using UnityEngine;

public class ComingDownAnimation : ObjectStartAnimation
{
    public override void SetAnimationParameters()
    {
        rotatingSideways = false;
        rotatingVertically = false;
        comingDown = true;
    }
}
