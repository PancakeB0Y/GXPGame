using System;
using System.Drawing;
using System.Runtime.InteropServices;
using GXPEngine;

//Object that changes level when touched
public class LevelChange : EasyDraw
{
    string _nextLevel;
    public string NextLevel
    {
        get
        {
            if (_nextLevel != null) return _nextLevel;
            return "";
        }
        set => _nextLevel = value;
    }
    public LevelChange(float pX, float pY, int pWidth, int pHeight, string nextLevel) : base(pWidth, pHeight)
    {
        Clear(Color.Yellow);
        SetXY(pX, pY);
        _nextLevel = nextLevel;
        collider.isTrigger = true;
    }
}
