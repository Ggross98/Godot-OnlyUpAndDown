using Godot;
using System;

public partial class PlayerStatusUI : Control
{
    [Export] private IconLabel hp, shield, money, mana, keys;

    public override void _Ready()
    {
        hp.ShowIcon(Utils.GetIconImage("heart"));
        shield.ShowIcon(Utils.GetIconImage("shield"));
        money.ShowIcon(Utils.GetIconImage("coin"));
        mana.ShowIcon(Utils.GetIconImage("mana"));
        keys.ShowIcon(Utils.GetIconImage("key"));
    }

    public void ShowPlayerStatus(PlayerStatus status){
        hp.ShowValue(status.HP);
        shield.ShowValue(status.shield);
        money.ShowValue(status.money);
        mana.ShowValue(status.mana);
        keys.ShowValue(status.keys);
    }
}
