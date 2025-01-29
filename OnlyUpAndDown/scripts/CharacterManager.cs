using Godot;
using System;
using System.Collections.Generic;

public partial class CharacterManager : GridObjectManager<Character>
{
	// private PackedScene characterPrefab = ResourceLoader.Load<PackedScene>("res://scenes/character.tscn");

	private Character player;
	public Character Player {
		get { return player; }
		set { player = value; }
	}

	public Character CreateCharacter(Vector2I pos, CharacterData data, bool isEnemy = false){
		var obj = CreateObjectAt(pos);

		obj.Data = data;
		obj.isEnemy = isEnemy;

		if(obj.isEnemy){
			obj.SetFlip(true);
		}else{
			player = obj;
		}
		

		return obj;
	}

	public List<Character> GetEnemies(){
		var res = new List<Character>(objects);
		res.Remove(player);

		return res;
	}


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// characters = new List<Character>();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

	}

	// public void MovePlayer(Vector2I coord){
	// 	MoveObject(player, coord);
	// }

    public void MovePlayer(Vector2I coord, Tween tween = null){
        if(tween == null){
            MoveObject(player, coord);
        }
		else{
			tween.TweenProperty(player, "position", grid.Coord2LocalPosition(coord), Settings.MOVE_DURATION);
			player.Coord = coord;
			// MoveObject(player, coord);
		}
    }

	public Character GetEnemyAt(Vector2I pos){
		foreach(var obj in objects){
			if(obj.Coord == pos && obj != player){
				return obj;
			}
		}
		return null;
	}

	public List<Character> GetReachableEnemies(){
		var res = new List<Character>(){};
		foreach(var ch in objects){
			if(ch.isEnemy && grid.IsReachable(player.Coord, ch.Coord)){
				res.Add(ch);
			}
		}
		return res;
	}
}
