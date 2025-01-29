using Godot;
using System;
using System.Collections.Generic;

public partial class SkillData
{   
    public string name, description;
    public int id;
    public string iconName;
    public int[] cost;
    // public bool autoRelease;
    public int price;


    public SkillData(int _id, string _iconName, string _name = "new skill", string _description = "no description",
                     int[] _cost = null, int _price = 5)
    {
        id = _id;
        iconName = _iconName;
        name = _name;
        description = _description;
        cost = _cost;
        price = _price;
    }
}
