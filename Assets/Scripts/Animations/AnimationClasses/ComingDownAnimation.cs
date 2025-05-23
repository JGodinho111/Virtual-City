/// <summary>
/// Extends class ObjectStartAnimation to set up wanted animation parameters
/// </summary>
public class ComingDownAnimation : ObjectStartAnimation
{
    public override void SetAnimationParameters()
    {
        rotatingSideways = false;
        rotatingVertically = false;
        comingDown = true;
    }
}
