using Godot;
using System;

/// <summary>
/// 可以唤起Popup界面的图标对象
/// </summary>
public partial class PopupIcon : Control
{
    public int dataID;
    public enum DataType { Enemy, Skill, Item, None };
    private DataType type = DataType.None;

    private StaticData staticData;
    private Popups popups;
    [Export] private TextureRect rect;

    public override void _Ready()
    {
        staticData = GetNode<StaticData>("/root/StaticData");
        popups = GetNode<Popups>("/root/Popups");
    }

    public void SetData(int _id, DataType _type){
        this.dataID = _id;
        this.type = _type;

        switch(type){
            case DataType.Skill:
                var skill = staticData.GetSkillData(dataID);
                rect.Texture = Utils.GetSkillImage(skill.iconName);
                break;
            case DataType.Item:
                var item = staticData.GetItemData(dataID);
                rect.Texture = Utils.GetItemImage(item.iconName);
                break;
        }
    }

    public void MouseEnter(){
        if(type != DataType.None){
            var pos = this.GlobalPosition;
            switch(type){
                case DataType.Skill:
                    var skill = staticData.GetSkillData(dataID);
                    popups.ShowPopup(pos, skill.name, skill.description, -150);
                    break;
                case DataType.Item:
                    var item = staticData.GetItemData(dataID);
                    popups.ShowPopup(pos, item.name, item.description);
                    break;
            }
        }
    }

    public void MouseExit(){
        popups.HidePopup();
    }

}
