using Godot;
using System;
using System.Collections.Generic;

public partial class Settings
{

    // public const int BOARD_OFFSET_X = 200, BOARD_OFFSET_Y = 0;
    // public const int TILE_WIDTH = 100, TILE_HEIGHT = 100;
    public const float TILE_WIDTH = 120 * 1.4f, TILE_HEIGHT = 70 * 1.4f;
    public const int BOARD_MARGIN = 5;

    public const int MAX_DEPTH = 5;

    // Directions
    public const int LEFT = 0, UP = 1, RIGHT = 2, DOWN = 3;
    public const int ANY_DIRECTION = 4;

    public const int STEP_ICON_INTERVAL = 50;
    
    public static float P_ENEMY_NORMAL = 0.4f;
    public static List<ItemData.Type> SPAWN_NORMAL = new List<ItemData.Type>(){
        ItemData.Type.Food, ItemData.Type.Resource, ItemData.Type.Trap, ItemData.Type.Artifact,
    };
    public static float[] SPAWN_NORMAL_CP = Utils.CumulativeProbability(new float[]{
        4f, 1f, 2f, 0.5f
    });

    public static List<ItemData.Type> SPAWN_BONUS = new List<ItemData.Type>(){
        ItemData.Type.Food, ItemData.Type.Resource, 
    };
    public static float[] SPAWN_BONUS_CP = Utils.CumulativeProbability(new float[]{
        5f, 2f,
    });

    // Item and probability
    // ...Food
    public static List<ItemData> FOODS = new List<ItemData>(){
        ItemData.Food_Apple, ItemData.Food_Milk, ItemData.Food_Mushroom
    };
    public static float[] FOODS_CP = Utils.CumulativeProbability(new float[]{
        0.4f, 0.4f, 0.2f
    });
    // ...Resource
    public static List<ItemData> RESOURCES = new List<ItemData>(){
        ItemData.Resource_MagicStone,
    };
    public static float[] RESOURCES_CP = Utils.CumulativeProbability(new float[]{
        1.0f,
    });
    // ...Trap
    public static List<ItemData> TRAPS = new List<ItemData>(){
        ItemData.Trap_BearTrap,
    };
    public static float[] TRAPS_CP = Utils.CumulativeProbability(new float[]{
        1.0f,
    });


    // Animation
    public const float MOVE_DURATION = 0.2f;
    public const float ANIMATION_INTERVAL = 0.5f;

}
