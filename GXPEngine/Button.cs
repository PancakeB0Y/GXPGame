using System;
using System.Drawing;
using System.Runtime.InteropServices;
using GXPEngine;

public class Button : LevelChange
{
    EasyDraw visual;
    string nextLevel;
    public Button(float pX, float pY, int pWidth, int pHeight, string nextLevel, string visualFilename) : base(pX, pY, pWidth, pHeight, nextLevel)
    {
        this.nextLevel = nextLevel;
        if (visualFilename != "" && visualFilename != null)
        {
            visual = new EasyDraw(visualFilename);
            visual.width = pWidth;
            visual.height = pHeight;
            AddChild(visual);
        }
        else
        {
            visual = new EasyDraw(pWidth, pHeight);
            visual.Clear(Color.Red);
            AddChild(visual);
        }
    }

    void Update()
    {
        if (visual.HitTestPoint(Input.mouseX, Input.mouseY))
        {
            visual.SetColor(1, 1, 1);
            if (Input.GetMouseButtonDown(0) && nextLevel != "")
            {
                ((MyGame)game).LoadLevel(nextLevel);
            }
        }
        else
        {
            visual.SetColor(0.8f, 0.8f, 0.8f);
        }
    }
}
