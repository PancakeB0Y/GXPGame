using System;
using System.Drawing;
using System.Runtime.InteropServices;
using GXPEngine;

//Object that changes level when touched
public class LevelChange : EasyDraw
{
    protected string _nextLevel;
    public string NextLevel
    {
        get
        {
            if (_nextLevel != null) return _nextLevel;
            return "";
        }
        set => _nextLevel = value;
    }
    public LevelChange(float pX, float pY, int pWidth, int pHeight, string nextLevel) : base("assets/levelChange.png")
    {
        //Clear(Color.Yellow);
        //Clear(255, 0, 0, 255);
        SetXY(pX, pY);
        width = pWidth;
        height = pHeight;
        _nextLevel = nextLevel;
        collider.isTrigger = true;
    }
}
