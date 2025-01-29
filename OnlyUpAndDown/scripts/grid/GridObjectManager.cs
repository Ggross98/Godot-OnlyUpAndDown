using Godot;
using System;
using System.Collections.Generic;

public abstract partial class GridObjectManager<T> : Node2D where T: GridObject
{
    [Export] protected Node2D parent;
    [Export] protected PackedScene prefab;
    protected Grid grid;
    protected List<T> objects;

    public void LoadGrid(Grid _grid){
        grid = _grid;
    }
    
    public T CreateObject(){
        var obj = prefab.Instantiate<T>();
		parent.AddChild(obj);

		if(objects == null) objects = new List<T>();
		objects.Add(obj);

		return obj;
    }

    public T CreateObjectAt(Vector2I coord){
        var obj = CreateObject();

        MoveObject(obj, coord);

        return obj;
    }

    public void ClearObjects(){
        if(objects == null) return;

        // for(int i = 0; i <  objects.Count; i++){
        //     // var obj = objects[0];
        //     // DeleteObject(obj);

        //     var obj = objects[0];
        //     objects.RemoveAt(0);
        //     obj.QueueFree();
        // }

        foreach(var obj in objects){
            obj.QueueFree();
        }
        objects = new List<T>();
    }

    public void DeleteObject(T obj){
        if(objects != null && objects.Contains(obj)){
            objects.Remove(obj);
        }

        obj.QueueFree();
    }

    public T GetAt(Vector2I coord){
        if(objects == null) return null;

        foreach(var obj in objects){
            if(obj.Coord.Equals(coord)) return obj;
        }
        return null;
    }

    public void DeleteAt(Vector2I coord){
        var obj = GetAt(coord);
        if(obj != null) DeleteObject(obj);
    }

    public void MoveObject(T obj, Vector2I coord){
        obj.Coord = coord;
        obj.Position = grid.Coord2LocalPosition(coord);
    }


}
