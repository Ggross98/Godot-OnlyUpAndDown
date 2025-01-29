using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Utils
{
    public static Texture2D GetSkillImage(string skillImageName){
        return ResourceLoader.Load<Texture2D>("res://assets/sprites/skills/" + skillImageName + ".png");
    }

    public static Texture2D GetItemImage(string itemName){
        return ResourceLoader.Load<Texture2D>("res://assets/sprites/items/" + itemName + ".png");
    }

    public static Texture2D GetIconImage(string iconName){
        return ResourceLoader.Load<Texture2D>("res://assets/sprites/icons/" + iconName + ".png");
    }

    public static Texture2D GetIconImageFromItem(ItemData data){
        
        string iconName = "";
        switch(data.type){
            case ItemData.Type.Food:

                if(data.iconName == "milk") iconName = "shield";
                else iconName = "heart";
                break;

            case ItemData.Type.Trap:

                iconName = "heart";
                break;

            case ItemData.Type.Artifact:

                iconName = "coin";
                break;

            case ItemData.Type.Resource:

                if(data.iconName == "magic_stone") iconName = "coin";
                break;
                
            case ItemData.Type.Skill:
                iconName = "coin";
                break;
        }

        return GetIconImage(iconName);
    }

    public static Vector2I Direction2Vector(int dir){
        switch(dir){
            case Settings.UP:
                return new Vector2I(0, 1);
            case Settings.DOWN:
                return new Vector2I(1, 0);
            default:
                return new Vector2I();
        }
    }

    public static int Vector2Direction(Vector2I v){
        if(v.Equals(new Vector2I(0, 1))) return Settings.UP;
        else if(v.Equals(new Vector2I(1, 0))) return Settings.DOWN;
        else return -1;
    }

    public static List<T> Shuffle<T>(List<T> list, RandomNumberGenerator rng = null) {
        if(rng == null) rng = new RandomNumberGenerator();

        var newList = new List<T>();
        foreach (var item in list) {
            newList.Add(item);
        }
        return newList.OrderBy(x => rng.Randf()).ToList();
    }


    public static List<T> GetRandomChildren<T>(List<T> list, int count, RandomNumberGenerator rng = null){
        if(count <= 0) return new List<T>();

        if(rng == null) rng = new RandomNumberGenerator();

        var res = Shuffle<T>(list, rng);
        if(count > list.Count) return res;
        else return res.GetRange(0, count);
    }

    public static float[] CumulativeProbability(float[] p){
        var cp = new float[p.Length];
        var last = 0f;
        for(int i = 0; i < p.Length; i++){
            cp[i] = last + p[i];
            last = cp[i];
        }
        return cp;
    }

    public static T Draw<T>(List<T> list, float[] cp, RandomNumberGenerator rng = null){
        if(rng == null) rng = new RandomNumberGenerator();

        // var cp = new float[p.Length];
        // var last = 0f;
        // for(int i = 0; i < p.Length; i++){
        //     cp[i] = last + p[i];
        //     last = cp[i];
        // }

        var r = rng.Randf() * cp[cp.Length - 1];
        for(int i = 0; i < cp.Length; i++){
            if(r <= cp[i]) return list[i];
        }

        return list[0];
    }

    
	private static void PrintItems(List<ItemData> datas){
		foreach(var data in datas) GD.Print(data.name);
	}
}
