using System;
using System.Drawing;
using System.Runtime.InteropServices;
using GXPEngine;

public class Platform : EasyDraw
{
    public Platform(float pX, float pY, int pWidth, int pHeight) : base(pWidth, pHeight)
    {
        Clear(Color.White);
        SetXY(pX, pY);
    }
}
