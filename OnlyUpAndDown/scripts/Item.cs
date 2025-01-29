using Godot;
using System;

public partial class Item : GridObject
{   
    [Export] private Sprite2D sprite;

    public int value;

    private ItemData data;
	public ItemData Data{
		get { return data; }
		set { 
            data = value; 
            ShowImage();
        }
	}

    public void ShowImage(){
        if(data.type == ItemData.Type.Skill){
            sprite.Texture = Utils.GetSkillImage(data.skill.iconName);
        }
        else{
            sprite.Texture = Utils.GetItemImage(data.iconName);
        }
        
    }

    public void MouseEnter(){
        GD.Print("Mouse entered " + data.id);
        GetNode<Popups>("/root/Popups").PopupItem(this);
    }

    public void MouseExit(){
        GD.Print("Mouse exited " + data.id);
        GetNode<Popups>("/root/Popups").HidePopup();
    }
}
