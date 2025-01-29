using Godot;
using System;

public partial class SkillBar : Control
{
	
	private PackedScene stepIconPrefab = ResourceLoader.Load<PackedScene>("res://scenes/step_icon.tscn");

	// Textures
	[Export]
	public Texture2D[] texturesActive, texturesInactive;

	// UI components
	[Export] private Label buttonLabel;
	[Export] private Control stepIconParent;
	[Export] private TextureRect skillIcon;
	private StepIcon[] icons;

	private SkillData data;
	public SkillData Data {
		get { return data; }
		set { data = value; }
	}
	
	private int stepIndex, stepCount;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// buttonLabel = GetNode<Label>("ButtonLabel");
		// stepIconParent = GetNode<Node2D>("StepIconParent");
		// skillIcon = GetNode<Sprite2D>("SkillIcon");

		// Debug
		// Init(SkillData.Test0);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private StepIcon CreateStepIcon(int idx){
		var icon = stepIconPrefab.Instantiate<StepIcon>();
		stepIconParent.AddChild(icon);
		icons[idx] = icon;

		icon.SetTexture(texturesInactive[data.cost[idx]]);
		icon.Position = new Vector2(idx * Settings.STEP_ICON_INTERVAL, 0);

		return icon;
	}

	public void Init(SkillData _data){

		//TODO: Clear current objects

		// Create new objects
		data = _data;

		// Create step indicators
		var count = data.cost.Length;
		icons = new StepIcon[count];
		for(int i = 0; i<count; i++){
			CreateStepIcon(i);
		}

		// Load skill icon
		skillIcon.Texture = Utils.GetSkillImage(data.iconName);

		// Set
		stepIndex = 0;
		stepCount = data.cost.Length;
	}

	public void EnterDirection(int dir){

		// GD.Print("Direction: " + dir);

		// Is ready: do nothing
		if(stepIndex >= stepCount){
			return;
		}
		// Not ready: check the input direction
		else{
			var check = data.cost[stepIndex];
			if(check == Settings.ANY_DIRECTION || check == dir){
				icons[stepIndex].SetTexture(texturesActive[check]);
				stepIndex++;
			}else{
				Reset();
				check = data.cost[0];
				// 是否符合第一个方向要求
				if(check == Settings.ANY_DIRECTION || check == dir){
					icons[stepIndex].SetTexture(texturesActive[check]);
					stepIndex++;
				}
			}
		}
	} 

	public void Reset(){
		for(int i = 0; i<stepCount; i++){
			icons[i].SetTexture(texturesInactive[data.cost[i]]);
		}
		stepIndex = 0;
	}

	public bool IsReady(){
		return stepIndex >= stepCount;
	}

	public void MouseEnter(){
		GetNode<Popups>("/root/Popups").PopupSkillBar(this);
        
    }

    public void MouseExit(){
		GetNode<Popups>("/root/Popups").HidePopup();
    }
}
