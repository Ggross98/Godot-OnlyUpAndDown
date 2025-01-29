using Godot;
using System;

public partial class IconLabel : Control
{
    [Export] protected TextureRect icon;
    [Export] protected Label label;

    // [Export] Texture2D iconTexture;

    public override void _Ready()
    {
        // ShowIcon(iconTexture);
    }

    public void ShowIcon(Texture2D texture){
        icon.Texture = texture;
    }

    public void ShowValue(int value){
        label.Text = value + "";
    }

    public void ShowText(string text){
        label.Text = text;
    }

    public void Show(Texture2D icon, int value){
        ShowIcon(icon);
        ShowValue(value);

        Show();
    }
}
