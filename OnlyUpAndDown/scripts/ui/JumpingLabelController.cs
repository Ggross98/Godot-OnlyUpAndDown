using Godot;
using System;
using System.Collections.Generic;

public partial class JumpingLabelController : Control
{
    private PackedScene labelPrefab = ResourceLoader.Load<PackedScene>("res://scenes/jumping_icon_label.tscn");
    [Export] private Control labelParent;

    private List<JumpingIconLabel> labels;

    public override void _Ready()
    {
        // labels = new List<JumpingIconLabel>();
        // CreateJumpingLabel(Vector2.Zero, "heart", 99);
    }

    public JumpingIconLabel CreateJumpingLabel(Vector2 gPos, string iconName, int value){
        var label = labelPrefab.Instantiate<JumpingIconLabel>();
		labelParent.AddChild(label);
        if(labels == null) labels = new List<JumpingIconLabel>();
        labels.Add(label);

        var icon = Utils.GetIconImage(iconName);
        label.Jump(gPos, icon, value);

        return label;
    }
}
