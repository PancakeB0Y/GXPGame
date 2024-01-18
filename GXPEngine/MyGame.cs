using System;                                   // System contains a lot of default C# libraries 
using GXPEngine;                                // GXPEngine contains the engine
using System.Drawing;
using System.Diagnostics;
using System.Collections.Generic;                           // System.Drawing contains drawing tools such as Color definitions

public class MyGame : Game
{
    public MyGame() : base(1280, 720, false)
    {
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
        AddChild(new Level(filename));
    }

    void LateUpdate() { }

    static void Main()
    {
        new MyGame().Start();
    }
}