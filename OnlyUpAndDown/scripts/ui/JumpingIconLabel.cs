using Godot;
using System;

public partial class JumpingIconLabel : IconLabel
{
    public Vector2 velocity = Vector2.Zero;
    
    private bool jumping = false;
    public Tweener tweener;

    // Visual effects

    /// <summary>
    /// 跳动方向的偏移范围
    /// </summary>
    [Export] public float velocity_x_range = 50f;

    /// <summary>
    /// 向上跳动的初速度
    /// </summary>
    [Export] public float velocity_y0 = -300;

    /// <summary>
    /// 质量，用于计算重力加速度
    /// </summary>
    [Export] public float mass = 1f;

    /// <summary>
    /// 重力大小
    /// </summary>
    [Export] public Vector2 gravity = new Vector2(0, 800);

    /// <summary>
    /// 多长时间后开始动画（变小、变淡）
    /// </summary>
    [Export] public float animStartDelay = 0.8f;


    /// <summary>
    /// 动画开始后多久销毁对象
    /// </summary>
    [Export] public float animDuration = 0.1f;
    

    public override void _Process(double delta)
    {
        if(jumping){
            velocity += (float)delta * gravity * mass;
            GlobalPosition += (float)delta * velocity;
        }
    }

    public void Jump(Vector2 gPos, Texture2D icon, int value){

        if(value >= 0) label.Modulate = Colors.Green;
        else label.Modulate = Colors.Red;

        ShowIcon(icon);
        string text;
        if(value >= 0) text = "+" + value;
        else text = "" + value;
        ShowText(text);

        GlobalPosition = gPos;
        var x = new RandomNumberGenerator().RandfRange(-velocity_x_range, velocity_x_range);
        velocity = new Vector2(x, velocity_y0);

        Tween tween = GetTree().CreateTween();
        tween.TweenInterval(animStartDelay);
        tween.TweenProperty(this, "modulate", Colors.Transparent, animDuration).SetTrans(Tween.TransitionType.Linear);
        tween.Parallel().TweenProperty(this, "scale", Vector2.Zero, animDuration).SetTrans(Tween.TransitionType.Linear);
        tween.TweenCallback(Callable.From(QueueFree));

        jumping = true;
    }
}
