using Godot;
using System;

public partial class OverPanel : Panel
{
    [Export] private Label title;

    public void SetTitle(string text){
        title.Text = text;
    }
}
