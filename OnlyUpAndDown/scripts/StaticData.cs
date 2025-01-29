using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;

public partial class StaticData : Node
{
    public System.Collections.Generic.Dictionary<int, SkillData> allSkills;
    public System.Collections.Generic.Dictionary<int, ItemData> allArtifacts;
    public System.Collections.Generic.Dictionary<int, CharacterData> allCharacters;
    

    // public List<SkillData> startSkills;
    // public List<ItemData> startArtifacts;

    public CharacterData playerCharacterData;

    public PlayerStatus startPlayerStatus;


    public override void _Ready()
    {
        GD.Print("Static data loading...");
        LoadSkills();
        LoadArtifacts();
        LoadCharacters();
        GD.Print("Static data loaded successfully. ");
    }

    public void LoadSkills(){
        allSkills = new System.Collections.Generic.Dictionary<int, SkillData>(){};
        var res = JsonLoader.LoadArrayFromJsonFile("res://profiles/skills.json");
        foreach(var x in res){
            var data = JsonLoader.ToSkillData(x.AsGodotDictionary());
            allSkills[data.id] = data;
        }

        // Debug
        // foreach(var skill in allSkills.Values){
        //     GD.Print(skill.name);
        // }
    }

    public void LoadArtifacts(){
        allArtifacts = new System.Collections.Generic.Dictionary<int, ItemData>(){};
        var res = JsonLoader.LoadArrayFromJsonFile("res://profiles/artifacts.json");
        foreach(var x in res){
            var data = JsonLoader.ToArtifactData(x.AsGodotDictionary());
            allArtifacts[data.id] = data;
        }
        
        // Debug
        // foreach(var artifact in allArtifacts){
        //     GD.Print(artifact.name);
        // }
        
    }

    public void LoadCharacters(){
        allCharacters = new System.Collections.Generic.Dictionary<int, CharacterData>(){};
        var res = JsonLoader.LoadArrayFromJsonFile("res://profiles/characters.json");
        foreach(var x in res){
            var data = JsonLoader.ToCharacterData(x.AsGodotDictionary());
            allCharacters[data.id] = data;
        }
    }

    public SkillData GetSkillData(int id){
        if(allSkills.ContainsKey(id)){
            return allSkills[id];
        }
        else{
            return null;
        }
    }

    public ItemData GetItemData(int id){
        if(allArtifacts.ContainsKey(id)){
            return allArtifacts[id];
        }
        else{
            return null;
        }
    }

    public CharacterData GetCharacterData(int id){
        if(allCharacters.ContainsKey(id)){
            return allCharacters[id];
        }
        else{
            return null;
        }
    }
}
