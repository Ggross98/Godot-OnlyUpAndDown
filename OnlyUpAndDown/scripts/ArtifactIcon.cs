using Godot;
using System;

public partial class ArtifactIcon : Control
{
    [Export] private TextureRect rect;
    private ItemData data;
    public ItemData Data {
        get { return data; }
        set { data = value; }
    }

    public void SetTexture(Texture2D texture){
        rect.Texture = texture;
    }

    public void LoadData(ItemData _data){
        data = _data;
        SetTexture(Utils.GetItemImage(_data.iconName));
    }

    public void MouseEnter(){
        GD.Print("Mouse entered " + data.id);
        GetNode<Popups>("/root/Popups").PopupArtifact(this);
    }

    public void MouseExit(){
        GD.Print("Mouse exited " + data.id);
        GetNode<Popups>("/root/Popups").HidePopup();
    }
}
