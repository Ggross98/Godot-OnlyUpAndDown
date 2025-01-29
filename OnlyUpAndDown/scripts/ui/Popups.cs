using Godot;
using System;

public partial class Popups : Control
{
    [Export] private PopupPanel popupPanel;
    [Export] private Label nameLabel;
    [Export] private RichTextLabel infoLabel;
    [Export] private Control stepIconParent;
    private PackedScene stepIconPrefab = ResourceLoader.Load<PackedScene>("res://scenes/step_icon.tscn");

    public static string TYPE_FOOD = string.Format("[color={0}]{1}[/color]", "green", "食物. ");
    public static string TYPE_RESOURCE = string.Format("[color={0}]{1}[/color]", "brown", "资源. ");
    public static string TYPE_TRAP = string.Format("[color={0}]{1}[/color]", "red", "陷阱. ");
    public static string TYPE_ARTIFACT = string.Format("[color={0}]{1}[/color]", "cyan", "遗物. ");
    public static string TYPE_SKILL = string.Format("[color={0}]{1}[/color]", "yellow", "技能. ");
    public static string TYPE_ENEMY = string.Format("[color={0}]{1}[/color]", "red", "敌人. ");

    public static string RACE_HUMAN = string.Format("[color={0}]{1}[/color]", "cyan", "人类. ");
    public static string RACE_YOKAI = string.Format("[color={0}]{1}[/color]", "cyan", "妖怪. ");
    public static string RACE_YOUSEI = string.Format("[color={0}]{1}[/color]", "cyan", "妖精. ");


    public override void _Ready()
    {
        GD.Print("Popups loaded");
    }

    public void PopupItem(Item item){
        popupPanel.Popup();
        popupPanel.Position = (Vector2I)(item.GlobalPosition + new Vector2(1366/2, 768/2));
        GD.Print(item.Data.id);

        nameLabel.Text = item.Data.name;

        string tmp = "";

        switch(item.Data.type){
            case ItemData.Type.Food:
                tmp += TYPE_FOOD;
                break;
            case ItemData.Type.Resource:
                tmp += TYPE_RESOURCE;
                break;
            case ItemData.Type.Trap:
                tmp += TYPE_TRAP;
                break;
            case ItemData.Type.Artifact:
                tmp += TYPE_ARTIFACT;
                break;
            case ItemData.Type.Skill:
                tmp += TYPE_SKILL;
                break;
        }

        tmp += item.Data.description;
        infoLabel.Text = tmp;
    }

    public void PopupEnemy(Character enemy){
        popupPanel.Popup();
        popupPanel.Position = (Vector2I)(enemy.GlobalPosition + new Vector2(1366/2, 768/2));
        // GD.Print(enemy.Data.name);
        
        nameLabel.Text = enemy.Data.name;

        string tmp = TYPE_ENEMY;
        tmp += Race2RichText(enemy.Data.race);
        tmp += "击败后可获得1金钱";
        
        infoLabel.Text = tmp;
    }

    public static string Race2RichText(CharacterData.Race race){
        switch(race){
            case CharacterData.Race.Yousei:
                return RACE_YOUSEI;
            case CharacterData.Race.Human:
                return RACE_HUMAN;
            case CharacterData.Race.Yokai:
                return RACE_YOKAI;
        }
        return "";
    }

    public void ShowPopup(Vector2 gPos, string name, string info, float deltaY = -150){

        popupPanel.Popup();
        popupPanel.Position = (Vector2I)(gPos + new Vector2(0, deltaY));
        nameLabel.Text = name;
        infoLabel.Text = info;

        
    }

    public void PopupSkillBar(SkillBar skillBar){
        popupPanel.Popup();
        popupPanel.Position = (Vector2I)(skillBar.GlobalPosition + new Vector2(1366/2, 768/2 - 150));

        nameLabel.Text = skillBar.Data.name;

        infoLabel.Text = skillBar.Data.description;
    }

    public void PopupArtifact(ArtifactIcon icon){
        popupPanel.Popup();
        popupPanel.Position = (Vector2I)(icon.GlobalPosition + new Vector2(1366/2, 768/2 + 50));

        nameLabel.Text = icon.Data.name;
        infoLabel.Text = icon.Data.description;
    }

    public void CreateStepIcons(int[] cost){

    }

    public void HidePopup(){
        popupPanel.Hide();
    }
}
