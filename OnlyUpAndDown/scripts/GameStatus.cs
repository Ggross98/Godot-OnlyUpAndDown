using Godot;
using System;

public partial class GameStatus
{
    public int currentLevel;
    public int currentTerrain;

    public GameStatus(){
        currentLevel = 0;
        currentTerrain = -1;
    }
}
