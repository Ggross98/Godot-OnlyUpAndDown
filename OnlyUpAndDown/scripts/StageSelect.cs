using Godot;
using System;
using System.Collections.Generic;

public partial class StageSelect : Node
{   
    [Export] private Label nameLabel;
    [Export] private PlayerStatusUI playerStatusUI;
    [Export] private PopupIcon[] popupIcons;
    private StaticData staticData;

    // private SkillData[] skillDatas;
    // private ItemData[] artifactDatas;
    
    private int currentCharacter;


    private PlayerStatus[] players;
    // private string[] playerNames;


    public override void _Ready()
    {
        staticData = GetNode<StaticData>("/root/StaticData");

        players = new PlayerStatus[2];
        players[0] = new PlayerStatus(
            50, 10, 0, 10, 1,
            new List<SkillData>(){ staticData.GetSkillData(5) },
            new List<ItemData>(){ staticData.GetItemData(0) } 
        );

        players[1] = new PlayerStatus(
            40, 20, 3, 10, 1,
            new List<SkillData>(){ staticData.GetSkillData(8) },
            new List<ItemData>(){ staticData.GetItemData(5) } 
        );

        // skillDatas = new SkillData[]{
        //     staticData.GetSkillData(5),
        //     staticData.GetSkillData(8)
        // };

        // artifactDatas = new ItemData[]{
        //     staticData.GetItemData(0),
        //     staticData.GetItemData(5)
        // };


        ShowCharacter(0);
    }

    public void ShowCharacter(int idx){

        currentCharacter = idx;

        var characterData = staticData.GetCharacterData(100 + idx);

        // name
        nameLabel.Text = characterData.name;

        // status
        var status = players[idx];
        playerStatusUI.ShowPlayerStatus(status);

        // artifacts and skills
        var artifactData = status.artifacts[0];
        popupIcons[0].SetData(artifactData.id, PopupIcon.DataType.Item);

        var skillData = status.skills[0];
        popupIcons[1].SetData(skillData.id, PopupIcon.DataType.Skill);
                
    }

    public void GotoGame(){

        staticData.startPlayerStatus = players[currentCharacter];
        staticData.playerCharacterData = staticData.GetCharacterData(currentCharacter + 100);

        GetTree().ChangeSceneToFile("res://scenes/game.tscn");
    }

    public void GotoMenu(){
        GetTree().ChangeSceneToFile("res://scenes/main_menu.tscn");
    }
}
