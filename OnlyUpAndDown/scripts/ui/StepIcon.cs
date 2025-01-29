using Godot;
using System;

public partial class StepIcon : Control
{
	[Export] TextureRect rect;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void SetTexture(Texture2D texture){
		rect.Texture = texture;
	}

}
