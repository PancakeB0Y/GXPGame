using System;                                   // System contains a lot of default C# libraries 
using GXPEngine;                                // GXPEngine contains the engine
using System.Drawing;
using System.Diagnostics;                           // System.Drawing contains drawing tools such as Color definitions

public class MyGame : Game
{
    EasyDraw background;
    Level level;

    public MyGame() : base(1280, 720, false)
    {
        background = new EasyDraw(800, 600, false);
        FillBackground();

        AddChild(background);

        level = new Level(this);
        AddChild(level);

        Console.WriteLine("Project initialized");
    }
    void FillBackground()
    {
        for (int i = 0; i < 100; i++)
        {
            // Set the fill color of the canvas to a random color:
            background.Fill(Utils.Random(100, 255), Utils.Random(100, 255), Utils.Random(100, 255));
            // Don't draw an outline for shapes:
            background.NoStroke();
            // Choose a random position and size:
            float px = Utils.Random(0, width);
            float py = Utils.Random(0, height);
            float size = Utils.Random(2, 5);
            // Draw a small circle shape on the canvas:
            background.Ellipse(px, py, size, size);
        }
    }

    void HandleScrolling()
    {
        //Scroll when player is too far right
        if (level.player.x + level.x > level.horizontalCenter)
        {
            level.x = level.horizontalCenter - level.player.x;
        }
        if (level.player.x + level.x < level.horizontalCenter)
        {
            level.x = level.horizontalCenter - level.player.x;
        }

        if (Input.GetKey(Key.A))
        {
            background.x += 1;
        }
        if (Input.GetKey(Key.D))
        {
            background.x -= 1;
        }
    }

    void Update()
    {
        HandleScrolling();
    }

    static void Main()
    {
        new MyGame().Start();
    }
}