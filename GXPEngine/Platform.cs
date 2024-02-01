using System;
using System.Drawing;
using System.Runtime.InteropServices;
using GXPEngine;

public class Platform : EasyDraw
{
    public Platform(float pX, float pY, int pWidth, int pHeight) : base(pWidth, pHeight)
    {
        //Clear(Color.White);
        SetXY(pX, pY);
    }
    public Platform(float pX, float pY, int pWidth, int pHeight, bool addCollision) : base(pWidth, pHeight, addCollision)
    {
        //Clear(Color.White);
        SetXY(pX, pY);
    }
}
