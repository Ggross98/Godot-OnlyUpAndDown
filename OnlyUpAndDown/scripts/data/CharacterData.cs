using Godot;
using System;
using System.Collections.Generic;

public partial class CharacterData
{
    public int id;
    public string spriteName;
    public string name, description;
    public enum Race { Human, Yousei, Yokai};
    public Race race;
    public int baseHP;

    

    public CharacterData(int _id, string _spriteName, string _name, string _description, int _baseHP, Race _race){
        id = _id;
        spriteName = _spriteName;
        name = _name;
        description = _description;
        baseHP = _baseHP;
        race = _race;
    }

    // // Player
    // public static CharacterData Player_Reimu = new CharacterData("reimu", 99);
    // public static CharacterData Player_Marisa = new CharacterData("reimu", 99);

    // // Enemy
    // public static CharacterData Enemy_Cirno = new CharacterData("cirno", 10);


    // public static Dictionary<string, CharacterData> allPlayers = new Dictionary<string, CharacterData>(){
    //     {"reimu", Player_Reimu},
    //     {"marisa", Player_Marisa},
    // };
}
