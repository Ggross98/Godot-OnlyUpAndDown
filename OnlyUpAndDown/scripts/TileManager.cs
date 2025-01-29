using Godot;
using System;
using System.Collections.Generic;

public partial class TileManager : GridObjectManager<Tile>
{	
	private PackedScene tilePrefab = ResourceLoader.Load<PackedScene>("res://scenes/tile.tscn");
	
	[Export] private int[] textureCount;
	private List<List<Texture2D>> tileTextures;
	// private Texture2D staircaseTexture = ResourceLoader.Load<Texture2D>("res://assets/sprites/tiles/staircase-0.png");

	// private Tile[,] tiles;
	// private Dictionary<Vector2I, Tile> tiles;
	// private TextureRect background;
	
	// public const int HSpace = 0, WSpace = 0;
	// private int row, col;

	

    public override void _Ready()
    {
		tileTextures = new List<List<Texture2D>>();
		for(int i = 0; i < textureCount.Length; i++){
			var tmp = new List<Texture2D>();
			for(int j = 0; j < textureCount[i]; j++){
				var file = i + "-" + j + ".png";
				var texture = ResourceLoader.Load<Texture2D>("res://assets/sprites/tiles/" + file);
				tmp.Add(texture);
			}
			tileTextures.Add(tmp);
		}

        // tileParent = this;

		// CreateGrid(7,7);
    }

    public override void _Process(double delta)
    {
        // if(Input.IsActionJustPressed("skill_0")){
		// 	CreateTiles(grid.cols, grid.cols);
		// }
		// if(Input.IsActionJustPressed("skill_1")){
		// 	ClearTiles();
		// }
    }

    public void CreateAllTiles(){
		CreateAllTiles(grid.range);
	}

	public int MapTerrains(){
		return tileTextures.Count;
	}

    public void CreateAllTiles(List<Vector2I> range){

		ClearObjects();

		foreach(var coord in range){
			CreateTileAt(coord);
		}

		GD.Print("Tiles: " + objects.Count);
	}

	public void ColorAllTiles(int idx){
		if(idx < 0 || idx >= tileTextures.Count) idx = 0;
		var palette = tileTextures[idx];
		var rng = new RandomNumberGenerator();

		foreach(var tile in objects){
			tile.SetTexture(palette[rng.RandiRange(0, palette.Count - 1)]);
		}

		// GetAt(new Vector2I(grid.width-1, grid.width-1)).SetTexture(staircaseTexture);
	}

	public Tile CreateTileAt(Vector2I coord){

		var tile = CreateObjectAt(coord);

		
		// tile.ShowCoord(coord);

		// var flag = coord.X + coord.Y;
		// tile.SetTexture(flag % 2);

		tile.ClearInfo();

		return tile;
	}

	
}
