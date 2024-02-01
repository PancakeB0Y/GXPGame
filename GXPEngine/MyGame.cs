using System;                                   // System contains a lot of default C# libraries 
using GXPEngine;                                // GXPEngine contains the engine
using System.Drawing;
using System.Diagnostics;
using System.Collections.Generic;
using GXPEngine.Core;                           // System.Drawing contains drawing tools such as Color definitions

public class MyGame : Game
{
    string nextLevel;
    HUD _HUD;
    readonly Sound backgroundMusic = new Sound("assets/background.wav", true);

    public MyGame() : base(Settings.Width, Settings.Height, Settings.FullScreen, true, Settings.ScreenResolutionX, Settings.ScreenResolutionY, true)
    {
        LoadLevel("startMenu.tmx");
        backgroundMusic.Play(false, 0, 0.03f);
        //game.OnAfterStep += LateUpdate;
        game.OnAfterStep += CheckLoadLevel;
    }

    void DestroyAll()
    {
        List<GameObject> children = GetChildren();
        for (int i = children.Count - 1; i >= 0; i--)
        {
            children[i].LateDestroy();
        }
    }

    public void LoadLevel(string filename)
    {
        nextLevel = filename;
    }

    void CheckLoadLevel()
    {
        if (nextLevel != null)
        {
            //Destroy current level
            DestroyAll();
            //Load next level
            LateAddChild(new Level(nextLevel));
            CreateUI();
            nextLevel = null;
        }
    }

    void CreateUI()
    {
        _HUD = new HUD();
        LateAddChild(_HUD);
    }

    public HUD GetHUD()
    {
        return _HUD;
    }

    static void Main()
    {
        // Load settings before creating and starting the game:
        Settings.Load();
        if (Settings.FullScreen)
        {
            // "Release mode": catch all exceptions and exit:
            try
            {
                new MyGame().Start();
            }
            catch (Exception error)
            {
                // Shortly show the error before terminating:
                Console.WriteLine("Error: {0}\n terminating game.", error.Message);
                // Alternatively (better): write the error data to a log file
            }
        }
        else
        {
            // "Debug mode": let your IDE show the exception, line where it triggered, call stack, etc.:
            new MyGame().Start();
        }
    }
}