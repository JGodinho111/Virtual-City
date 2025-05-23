/// <summary>
/// Extends class ObjectStartAnimation to set up wanted animation parameters
/// </summary>
public class SidewaysAnimation : ObjectStartAnimation
{
    public override void SetAnimationParameters()
    {
        rotatingSideways = true;
        rotatingVertically = false;
        comingDown = false;
    }
}
