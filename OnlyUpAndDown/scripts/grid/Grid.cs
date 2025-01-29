using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;

public partial class Grid : Node2D
{   
    public int height, width;
    public List<Vector2I> range;
    public Vector2I startPoint, endPoint;

    public override void _Ready()
    {
        // SetSize(3, 10);
    }

    public void SetSize(int _height, int _width, bool allTiles = true){
        height = _height;
        width = _width;

        if(allTiles){
            range = new List<Vector2I>();

            for(int i = 0; i < width; i++){
                for(int j = 0; j < width; j++){
                    var h = j - i;
                    if(h < -height || h > height) continue;
                    range.Add(new Vector2I(i, j));
                }
            }

            startPoint = new Vector2I(0, 0);
            endPoint = new Vector2I(width-1, width-1);
        }

        Position = new Vector2(- width / 2f * Settings.TILE_WIDTH * Scale.X, - Settings.TILE_HEIGHT / 2f * Scale.Y);
    }

    public bool IsInRange(Vector2I coord){

        // // width
        // if(coord.X < 0 || coord.X >= width) return false;

        // // height
        // int y = coord.Y - coord.X;
        // if(y < -height || y > height) return false;

        // // boundary
        // if(coord.X >= width || coord.Y >= width) return false;
		
		// return true;
		
        if(range == null){
            GD.PrintErr("The range of the grid is not defined!");
            return false;
        }

        return range.Contains(coord);
	}

    public bool IsReachable(Vector2I from, Vector2I to){
        if(!IsInRange(from) || !IsInRange(to)){
            return false;
        }

        return (to.X > from.X && to.Y >= from.Y) || (to.X >= from.X && to.Y > from.Y);
    }

	public Vector2 Coord2LocalPosition(Vector2I coord){

        var x = (coord.X + coord.Y) / 2f * Settings.TILE_WIDTH + 0.5f * Settings.TILE_WIDTH;
        var y = -(coord.Y - coord.X) / 2f * Settings.TILE_HEIGHT;

		return new Vector2(x, y);
	}

	public Vector2I LocalPosition2Coord(Vector2 pos){

        int x = (int)((pos.X - pos.Y) / Settings.TILE_WIDTH);
        int y = (int)(-(pos.X + pos.Y) / Settings.TILE_HEIGHT);

		return new Vector2I(x, y);
	}
}
