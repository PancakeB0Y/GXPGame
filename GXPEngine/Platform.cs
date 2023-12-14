using System;
using System.Drawing;
using System.Runtime.InteropServices;
using GXPEngine;

public class Platform : EasyDraw
{
    public Platform(int pX, int pY, int pWidth, int pHeight) : base(pWidth, pHeight)
    {
        Clear(Color.White);
        SetOrigin(pWidth / 2, 0);
        SetXY(pX, pY);
    }
}
