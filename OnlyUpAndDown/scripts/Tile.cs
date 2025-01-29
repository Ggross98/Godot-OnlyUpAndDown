using Godot;
using System;

public partial class Tile : GridObject
{
	[Export] private Label label;
	[Export] private Sprite2D sprite;
	[Export] private IconLabel iconLabel;

	// Texture2D[] textures = new Texture2D[]{
	// 	ResourceLoader.Load<Texture2D>("res://assets/sprites/tiles/isometric_block (12).png"),
	// 	ResourceLoader.Load<Texture2D>("res://assets/sprites/tiles/isometric_block (13).png"),
	// };

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

	}

	public void ShowCoord(Vector2I coord){
		label.Text = coord + "";
	}

	// public void SetTexture(int i){
	// 	if(i < 0 || i >= textures.Length){
	// 		GD.PrintErr("Texture index out of range!");
	// 	}
	// 	else{
	// 		sprite.Texture = textures[i];
	// 	}
	// }

	public void SetTexture(Texture2D texture){
		sprite.Texture = texture;
	}


	public void ShowItem(Item item){
		
		Texture2D icon = null;
		int value = item.value;

		var data = item.Data;
		icon = Utils.GetIconImageFromItem(data);
		// GD.Print(icon);
		if(icon != null) iconLabel.Show(icon, value);
	}

	public void ShowEnemy(Character character){
		
		var value = character.HP;
		var icon = Utils.GetIconImage("heart");

		iconLabel.Show(icon, value);
	}

	public void ClearInfo(){
		iconLabel.Hide();
	}

	public void SetModule(Color color){
		sprite.Modulate = color;
	}
}
