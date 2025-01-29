using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class SkillBarManager : Control
{
	[Export] private Control skillBarParent;
	private PackedScene skillBarPrefab = ResourceLoader.Load<PackedScene>("res://scenes/skill_bar.tscn");
	// private List<SkillBar> skillBars;
	private Dictionary<SkillData, SkillBar> skillBars;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{	
		// skillBarParent = this;
		skillBars = new Dictionary<SkillData, SkillBar>();
		// CreateSkillBar(SkillData.Test0);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{	
		// int dir = -1;
		// if(Input.IsActionJustPressed("left")){
		// 	dir = Settings.LEFT;
		// }else if(Input.IsActionJustPressed("right")){
		// 	dir = Settings.RIGHT;
		// }else if(Input.IsActionJustPressed("up")){
		// 	dir = Settings.UP;
		// }else if(Input.IsActionJustPressed("down")){
		// 	dir = Settings.DOWN;
		// }

		// if(dir >= 0){
		// 	EnterDirection(dir);
		// }
	}

	private SkillBar CreateSkillBar(SkillData data){

		var skillBar = skillBarPrefab.Instantiate<SkillBar>();
		skillBarParent.AddChild(skillBar);

		skillBar.Init(data);

		skillBars[data] = skillBar;

		return skillBar;
	}

	public void DeleteSkillBar(SkillData data){
		if(skillBars.ContainsKey(data)){
			skillBars[data].QueueFree();
			skillBars.Remove(data);
		}
	}

	public void ClearSkillBar(){
		foreach(var skillBar in skillBars.Values){
			skillBar.QueueFree();
		}
		skillBars.Clear();
	}

	// public void CreateSkillBars(List<SkillData> datas){
	// 	foreach(var data in datas){
	// 		CreateSkillBar(data);
	// 	}
	// }

	public void UpdateSkillBars(List<SkillData> datas){
		var keys = skillBars.Keys;

		// 当前没有的技能：创建
		var toCreate = datas.Except(keys);
		foreach(var data in toCreate){
			CreateSkillBar(data);
		}

		// 当前已创建，但列表中没有的技能：删除
		var toDelete = keys.Except(keys);
		foreach(var data in toDelete){
			DeleteSkillBar(data);
		}
	}

	public void EnterDirection(int dir){
		if(skillBars != null){
			foreach(var skillBar in skillBars.Values){
				skillBar.EnterDirection(dir);
			}
		}
	}

	public List<SkillData> GetReadySkills(){

		var data = new List<SkillData>();

		foreach(var skillBar in skillBars.Values){
			if(skillBar.IsReady()){
				data.Add(skillBar.Data);
			}
		}

		return data;
	}

	public void ResetSkill(SkillData data){
		if(skillBars.ContainsKey(data)){
			var skillBar = skillBars[data];
			skillBar.Reset();
		}
	}


}
