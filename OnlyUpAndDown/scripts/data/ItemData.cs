using Godot;
using System;
using System.Collections.Generic;

public partial class ItemData
{
    public int id;
    public string iconName;
    public string name, description;
    // public string effect;
    public SkillData skill;
    public int baseValue;

    public enum Type { Artifact, Food, Trap, Resource, Skill }; // Item: 拾取后立刻生效；Artifact: 拾取后进入遗物栏；Weapon：拾取后成为可用技能
    public Type type;

    

    public ItemData(int _id, string _iconName, string _name, string _description, Type _type, int _baseValue){
        id = _id;
        iconName = _iconName;
        name = _name;
        description = _description;
        type = _type;
        baseValue = _baseValue;
    }

    public ItemData(SkillData _skill){
        id = -1;
        iconName = "";
        name = _skill.name;
        description = _skill.description;
        type = Type.Skill;
        baseValue = _skill.price;

        skill = _skill;
    }

    public static ItemData DEBUG_ITEM = new ItemData(0, "skull", "debug name", "debug description", Type.Food, 999);

    // Food data
    public static ItemData Food_Apple = new ItemData(1, "apple", "苹果", "美味多汁的苹果, 食用后恢复生命值", Type.Food, 4);
    public static ItemData Food_Milk = new ItemData(2, "milk", "牛奶", "营养丰富的牛奶, 饮用后增加护盾值", Type.Food, 4);
    public static ItemData Food_Mushroom = new ItemData(3, "mushroom", "蘑菇", "分辨不出种类的蘑菇, 食用后也许会中毒", Type.Food, 8);
    

    public static ItemData Resource_MagicStone = new ItemData(4, "magic_stone", "魔法石", "蕴含魔法力量的矿石, 可以卖钱", Type.Resource, 2);

    // public static ItemData Artifact_WizardHat = new ItemData(5, "wizard_hat", "巫师帽", "战斗前先对敌人造成3点伤害", Type.Artifact, 6);

    public static ItemData Trap_BearTrap = new ItemData(6, "bear_trap", "捕兽夹", "踩上去会受伤", Type.Trap, 6);
    public static ItemData Trap_ToxicTrap = new ItemData(7, "bear_trap", "荆棘丛", "踩上去会中毒", Type.Trap, 3);

    public static ItemData Artifact_Default = new ItemData(0, "sunflower", "向日葵", "生命值上限提高1点, 可重复获得", Type.Artifact, 6);

}
