using Godot;
using System;
using System.Collections.Generic;

public partial class UIManager : Control
{
    
    // Other controllers
    [Export] private ArtifactsUI artifactsUI;
    [Export] private PlayerStatusUI playerStatusUI;
    [Export] private SkillBarManager skillBarManager;
    [Export] private JumpingLabelController jumpingLabelController;

    // Other panels
    [Export] private OverPanel overPanel;
    [Export] private PausePanel pausePanel;

    // Controlled by this
    [Export] private Label levelLabel;

    public override void _Ready()
    {
        HidePausePanel();
        HideOverPanel();
    }

    public void ShowPlayerStatus(PlayerStatus status){
        artifactsUI.ShowArtifacts(status.artifacts);
        playerStatusUI.ShowPlayerStatus(status);
        skillBarManager.UpdateSkillBars(status.skills);
    }

    public void ShowGameStatus(GameStatus status){
        ShowLevel(status.currentLevel);
    }

    public void EnterDirection(int dir){
        skillBarManager.EnterDirection(dir);
    }

    public List<SkillData> GetReadySkills(){
        return skillBarManager.GetReadySkills();
    }

    public void ResetSkill(SkillData data){
        skillBarManager.ResetSkill(data);
    }

    

    public void ShowLevel(int level){
        var depth = level / 5;
        levelLabel.Text = "Level " + (depth + 1) + "-" + (level % 5 + 1);
    }

    public void ShowJumpingLabel(Vector2 gPos, string iconName, int value){
        jumpingLabelController.CreateJumpingLabel(gPos, iconName, value);
    }

    public void ShowOverPanel(bool win){
        overPanel.SetTitle(win ? "胜利通关!" : "游戏结束");
        overPanel.Show();
    }

    public void HideOverPanel(){
        overPanel.Hide();
    }

    public void ShowPausePanel(){
        pausePanel.Show();
    }

    public void HidePausePanel(){
        pausePanel.Hide();
    }

}
