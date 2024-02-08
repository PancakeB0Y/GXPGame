using System;
using System.Drawing;
using System.Runtime.InteropServices;
using GXPEngine;

public class Checkpoint : EasyDraw
{
    bool _isPassed = false;
    public bool IsPassed { get => _isPassed; }

    int _collected = 0;
    public int Collected { get => _collected; }

    public Checkpoint(float pX, float pY, int pWidth = 40, int pHeight = 80) : base("assets/flag.png")
    {
        SetXY(pX, pY);
        width = pWidth;
        height = pHeight;
        collider.isTrigger = true;
        SetColor(0.5f, 0.5f, 0.5f);
    }

    public void Pass(Player player)
    {
        _isPassed = true;
        SetColor(1f, 1f, 1f);
        _collected = player.Collected;
        ((Level)parent).UpdateCollectedCollectibles();
    }
}
