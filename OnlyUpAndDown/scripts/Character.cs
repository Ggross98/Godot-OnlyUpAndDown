using Godot;
using System;

public partial class Character : GridObject
{
	
	[Export] private AnimatedSprite2D sprite;

	public int HP;

	private CharacterData data;
	public CharacterData Data{
		get { return data; }
		set { 
			data = value;
			HP = data.baseHP;
			ShowAnimation("idle");
			sprite.SetFrameAndProgress(0, new RandomNumberGenerator().Randf());
		}
	}

	public bool isEnemy = false;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void SetFlip(bool flip){
		sprite.FlipH = flip;
	}

	public void ShowAnimation(string anim){
		sprite.Play(data.spriteName + "_" + anim);
	}

	public void ChangeHP(int delta){
		HP += delta;
		if(HP < 0) HP = 0;
	}

	public void MouseEnter(){
		if(isEnemy){
			GetNode<Popups>("/root/Popups").PopupEnemy(this);
		}
        
    }

    public void MouseExit(){
		if(isEnemy){
			GetNode<Popups>("/root/Popups").HidePopup();
		}
        
    }

	public void AnimationFinished(){
		ShowAnimation("idle");
		sprite.SetFrameAndProgress(0, new RandomNumberGenerator().Randf());
	}
}
