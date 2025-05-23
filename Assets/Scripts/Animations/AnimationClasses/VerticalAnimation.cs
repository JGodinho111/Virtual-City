/// <summary>
/// Extends class ObjectStartAnimation to set up wanted animation parameters
/// </summary>
public class VerticalAnimation : ObjectStartAnimation
{
    public override void SetAnimationParameters()
    {
        rotatingSideways = false;
        rotatingVertically = true;
        comingDown = false;
    }
}
