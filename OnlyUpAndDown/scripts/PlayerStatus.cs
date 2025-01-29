using Godot;
using System;
using System.Collections.Generic;

public partial class PlayerStatus
{
    public int HP, maxHP;
    public int shield, mana;
    public int money, keys;
    // public bool alive;
    public List<SkillData> skills;
    public List<ItemData> artifacts;

    public PlayerStatus(int _maxHP, int _shield, int _mana, int _money, int _keys, 
                        List<SkillData> startSkills, List<ItemData> startArtifacts){

        maxHP = _maxHP;
        HP = maxHP;

        shield = _shield;
        mana = _mana;
        money = _money;
        keys = _keys;

        skills = new List<SkillData>(){};
        skills.AddRange(startSkills);

        artifacts = new List<ItemData>(){};
        artifacts.AddRange(startArtifacts);

    }

    public static PlayerStatus Copy(PlayerStatus model){
        var skills = new List<SkillData>(){};
        skills.AddRange(model.skills);

        var artifacts = new List<ItemData>(){};
        artifacts.AddRange(model.artifacts);

        return new PlayerStatus(model.maxHP, model.shield, model.mana, model.money, model.keys, skills, artifacts);
    }

    public void ChangeHP(int delta){
        HP += delta;
        if(HP < 0) HP = 0;
        if(HP > maxHP) HP = maxHP;
    }

    public void ChangeMoney(int delta){
        money += delta;
        if(money < 0) money = 0;
    }

    public void ChangeShield(int delta){
        shield += delta;
        if(shield < 0) shield = 0;
    }

    public void ChangeMana(int delta){
        mana += delta;
    }

    public bool IsAlive(){
        return HP > 0;
    }
}
