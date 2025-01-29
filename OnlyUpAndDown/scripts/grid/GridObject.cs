using Godot;
using System;

public abstract partial class GridObject : Node2D
{
    private Vector2I coord;
    public Vector2I Coord {
        get { return coord; }
        set { coord = value; }
    }
}
