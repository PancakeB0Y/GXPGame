using System;                                   // System contains a lot of default C# libraries 
using GXPEngine;                                // GXPEngine contains the engine
using System.Drawing;
using System.Diagnostics;
using System.Collections.Generic;                           // System.Drawing contains drawing tools such as Color definitions

public class MyGame : Game
{
    EasyDraw background;
    Level level;

    public MyGame() : base(1280, 720, false)
    {
        background = new EasyDraw(800, 600, false);
        AddChild(background);

        LoadLevel("level1.tmx");

        game.OnAfterStep += LateUpdate;
    }

    public void LoadLevel(String filename)
    {
        //Destroy old level
        List<GameObject> children = GetChildren();
        for (int i = children.Count - 1; i >= 0; i--)
        {
            children[i].Destroy();
        }

        //Load next level
        level = new Level(filename);
        AddChild(level);
    }

    void HandleScrolling()
    {
        //Scroll when player is too far right
        if (level.player.x + level.x > width / 2)
        {
            level.x = width / 2 - level.player.x;
        }
        if (level.player.x + level.x < width / 2)
        {
            level.x = width / 2 - level.player.x;
        }

        background.x = level.x * 0.5f;
    }

    void LateUpdate()
    {
        HandleScrolling();
    }

    static void Main()
    {
        new MyGame().Start();
    }
}