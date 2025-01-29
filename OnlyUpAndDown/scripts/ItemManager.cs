using Godot;
using System;
using System.Collections.Generic;

public partial class ItemManager : GridObjectManager<Item>
{
    public Item CreateItem(Vector2I pos, ItemData data){
        var obj = CreateObjectAt(pos);
        obj.Data = data;
        return obj;
    }


    public List<Item> GetReachableItems(Vector2I coord){
		var res = new List<Item>(){};
		foreach(var ch in objects){
			if(grid.IsReachable(coord, ch.Coord)){
				res.Add(ch);
			}
		}
		return res;
	}

    public List<Item> GetReachableItems(Vector2I coord, ItemData.Type type){
		var res = GetReachableItems(coord);
        for(int i = res.Count - 1; i >= 0; i--){
            var ch = res[i];
            if(ch.Data.type != type){
                res.Remove(ch);
            }
        }
		return res;
	}
}
