using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;

public partial class JsonLoader : Node
{
    public static Godot.Collections.Array LoadArrayFromJsonFile(string file){
        var str = ResourceLoader.Load<Json>(file);
        return str.Data.AsGodotArray();
    }

    public static SkillData ToSkillData(Godot.Collections.Dictionary data){
        var id = (int)data["id"];
        var iconName = (string)data["icon"];
        var name = (string)data["name"];
        var description = (string)data["description"];
        var price = (int)data["price"];
        var cost = (int[])data["cost"];

        return new SkillData(id, iconName, name, description, cost, price);
    }

    public static ItemData ToArtifactData(Godot.Collections.Dictionary data){
        var id = (int)data["id"];
        var iconName = (string)data["icon"];
        var name = (string)data["name"];
        var description = (string)data["description"];
        var price = (int)data["price"];

        return new ItemData(id, iconName, name, description, ItemData.Type.Artifact, price);
    }

    public static CharacterData ToCharacterData(Godot.Collections.Dictionary data){
        var id = (int)data["id"];
        var spriteName = (string)data["spriteName"];
        var name = (string)data["name"];
        var description = (string)data["description"];

        CharacterData.Race race = default;
        var raceIdx = (int)data["race"];
        switch (raceIdx){
            case 0:
                race = CharacterData.Race.Human;
                break;
            case 1:
                race = CharacterData.Race.Yousei;
                break;
            case 2:
                race = CharacterData.Race.Yokai;
                break;
        }

        var baseHP = (int)data["baseHP"];

        return new CharacterData(id, spriteName, name, description, baseHP, race);

    }
}
