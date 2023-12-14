using System;                                   // System contains a lot of default C# libraries 
using GXPEngine;                                // GXPEngine contains the engine
using System.Drawing;
using System.Diagnostics;                           // System.Drawing contains drawing tools such as Color definitions

public class MyGame : Game
{
    EasyDraw background;

    public MyGame() : base(800, 600, false)
    {
        background = new EasyDraw(800, 600, false);
        background.Clear(Color.Black);

        AddChild(background);
        AddChild(new Level(this));

        Console.WriteLine("Project initialized");
    }

    void Update()
    {
    }

    static void Main()
    {
        new MyGame().Start();
    }
}