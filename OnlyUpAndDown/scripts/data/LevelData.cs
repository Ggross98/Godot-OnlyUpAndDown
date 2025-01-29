using Godot;
using System;
using System.Collections.Generic;

public partial class LevelData
{
    public int mapWidth, mapHeight;
    public int mapTerrain;
    
    public enum LevelType { Normal, Bonus };
    public LevelType type = LevelType.Normal;


    /// <summary>
    /// 出现敌人的概率。如果类型为奖励关则不生效。
    /// </summary>
    public float pEnemy, pArtifact;

    /// <summary>
    /// 生成遗物的数量。如果类型为奖励关则不生效。
    /// </summary>
    // public int artifactCount;

    public List<CharacterData> enemyDataList;


    // public LevelData(int _width, int _height){
    //     mapWidth = _width;
    //     mapHeight = _height;
    // }
}
