using System;
using System.Drawing;
using System.Runtime.InteropServices;
using GXPEngine;

public class Button : LevelChange
{
    EasyDraw visual;
    Sound pressSound = new Sound("assets/confirm.wav");
    public Button(float pX, float pY, int pWidth, int pHeight, string nextLevel, string visualFilename) : base(pX, pY, pWidth, pHeight, nextLevel)
    {
        if (visualFilename != "" && visualFilename != null)
        {
            //Clear(0, 0, 0, 0);
            visual = new EasyDraw(visualFilename);
            visual.width = (int)(pWidth / 2.5f);
            visual.height = pHeight / 2;
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
            visual.SetColor(0.8f, 0.8f, 0.8f);
            if (Input.GetMouseButtonDown(0) && _nextLevel != "")
            {
                pressSound.Play(false, 0, 0.3f);
                ((MyGame)game).LoadLevel(_nextLevel);
            }
        }
        else
        {
            visual.SetColor(1f, 1f, 1f);
        }
    }
}
