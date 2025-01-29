using Godot;
using System;

public partial class MainMenu : Node
{
    [Export] private Control tutorialPanel;

    [Export] private Label tutorial;
    private int tutorialIndex;
    private string[] tutorialTexts;

    public override void _Ready()
    {
        HideTutorial();

        tutorialTexts = new string[]{

            "游戏概述：玩家操纵角色在地图中前进，以到达终点为目标。地图上的陷阱和敌人会阻碍你的探险，而不同的道具、遗物与技能则会祝你一臂之力。\n\n" +
                "基本操作：玩家只能控制角色向右上/右下两个方向移动。使用WS或方向键上下控制\n\n" +
                "关卡与胜利条件：当前版本共有6大关，每一关有5层。到达最后一层的最后一格格子以通关游戏！",
            "生命与护盾：地图中的陷阱和敌人会造成伤害。受到伤害时，若玩家拥有护盾，则优先扣除护盾值，待其归零后则扣除生命。生命归零时，角色被击倒，游戏结束\n\n" +
                "金币：地图中的遗物与技能需要金币购买，金币可通过击倒敌人获得\n\n",
            "技能：屏幕下方是技能栏，显示玩家拥有的技能。每个技能有不同的耗能需求，能量需要通过连续以特定方向移动获得。能量充满后，技能会自动释放并消耗所有能量。"+
                "若走出错误的方向，则技能已获得的能量会清零。每一大关的第五层将会有技能出售，玩家可用金币购买。\n\n" +
                "魔力：角色的属性，会让技能效果增强对应数值",
            "遗物：可以购买的永久道具，具有各种神奇的效果。鼠标悬停在图标上可以查看其具体功用",
        };
        tutorialIndex = 0;

    }

    public void TutorialNext(){
        tutorialIndex += 1;
        if(tutorialIndex >= tutorialTexts.Length){ tutorialIndex = 0;}

        tutorial.Text = tutorialTexts[tutorialIndex];
    }

    public void GotoStageSelect(){
        GetTree().ChangeSceneToFile("res://scenes/stage_select.tscn");
    }

    public void ShowTutorial(){
        tutorial.Text = tutorialTexts[0];
        tutorialPanel.Show();
    }

    public void HideTutorial(){
        tutorialPanel.Hide();
    }

    public void Quit(){
        GetTree().Quit();
    }
}
