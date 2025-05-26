using Godot;
using TheRingGoesSouth.scripts.actors;

public partial class BattleCameraController : Node
{
    private Camera2D _camera2D;

    public void Initialize(Camera2D camera2D)
    {
        _camera2D = camera2D;
    }

    public void FocusOnUnit(PlayerUnit unit, Node reparentTo)
    {
        if (_camera2D.GetParent() != reparentTo)
        {
            _camera2D.Reparent(reparentTo);
        }
        _camera2D.MakeCurrent();

        Tween tween = CreateTween();
        tween.TweenProperty(_camera2D, "global_position", unit.GlobalPosition, 0.5f)
            .SetEase(Tween.EaseType.Out)
            .SetTrans(Tween.TransitionType.Sine);
    }
}