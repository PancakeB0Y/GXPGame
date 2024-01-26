using System;
using System.Drawing;
using System.Runtime.InteropServices;
using GXPEngine;
using TiledMapParser;

public class TextBox : EasyDraw
{
    String text = "";
    float fontSize = 0;
    public TextBox(float pX, float pY, int pWidth, int pHeight, Text textInfo) : base(pWidth, pHeight, false)
    {
        text = textInfo.text;
        fontSize = textInfo.fontSize;
        TextSize(fontSize);

        TextAlign(CenterMode.Min, CenterMode.Min);
        Text(text);
        SetXY(pX, pY);
    }
}
