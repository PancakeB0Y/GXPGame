using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection.Emit;
using GXPEngine;
using TiledMapParser;

public class Level : GameObject
{
    Player player;
    Chaser chaser;
    readonly EasyDraw background;
    readonly Map leveldata;
    bool isGameOver = false;
    public Level(string filename)
    {
        leveldata = MapParser.ReadMap(filename);

        background = new EasyDraw(800, 600, false);
        AddChild(background);

        SpawnTiles(leveldata);
        SpawnObjects(leveldata);
    }

    void SpawnTiles(Map leveldata)
    {
        if (leveldata.Layers == null || leveldata.Layers.Length == 0) return;

        Layer mainLayer = leveldata.Layers[0];

        short[,] tileNumbers = mainLayer.GetTileArray();

        for (int row = 0; row < mainLayer.Height; row++)
        {
            for (int col = 0; col < mainLayer.Width; col++)
            {
                int tileNumber = tileNumbers[col, row];
                if (tileNumber > 0)
                {
                    Platform tile = new Platform(col * 16, row * 16, 16, 16);
                    AddChild(tile);
                }
            }
        }
    }
    void SpawnObjects(Map leveldata)
    {
        if (leveldata.ObjectGroups == null || leveldata.ObjectGroups.Length == 0) return;

        ObjectGroup objectGroup = leveldata.ObjectGroups[0];
        if (objectGroup.Objects == null || objectGroup.Objects.Length == 0) return;

        foreach (TiledObject obj in objectGroup.Objects)
        {
            switch (obj.Name)
            {
                case "Player":
                    player = new Player(this, obj.X, obj.Y, obj.GetFloatProperty("Speed", 3));
                    AddChild(player);
                    break;
                case "Chaser":
                    chaser = new Chaser(obj.X, obj.Y, (int)obj.Width, (int)obj.Height, obj.GetFloatProperty("Speed", 0.5f), obj.GetBoolProperty("MoveRight", true));
                    AddChild(chaser);
                    break;
                case "MovingPlatform":
                    MovingPlatform movingPlatform = new MovingPlatform(obj.X, obj.Y, (int)obj.Width, (int)obj.Height, obj.GetFloatProperty("StartX", obj.X), obj.GetFloatProperty("StartY", obj.Y), obj.GetFloatProperty("EndX", obj.X + obj.Width * 2), obj.GetFloatProperty("EndY", obj.Y), obj.GetFloatProperty("Speed", 0.5f));
                    AddChild(movingPlatform);
                    break;
            }
        }
    }

    float GetGlobalX(GameObject item)
    {
        float value = item.x;
        if (item.parent != null && item.parent != this)
        {
            value += GetGlobalX(item.parent);
        }
        return value;
    }

    void HandleScrolling()
    {
        //Scroll following the player
        if (player != null)
        {
            if (GetGlobalX(player) + x > 640)
            {
                x = 640 - GetGlobalX(player);
            }
            if (GetGlobalX(player) + x < 640)
            {
                x = 640 - GetGlobalX(player);
            }
        }

        background.x = x * 0.5f;
    }

    void DestroyChildren()
    {
        List<GameObject> children = GetChildren();
        for (int i = children.Count - 1; i >= 0; i--)
        {
            children[i].Destroy();
        }
    }

    void Restart()
    {
        DestroyChildren();
        SpawnTiles(leveldata);
        SpawnObjects(leveldata);
        isGameOver = false;
    }
    void HandleRestart()
    {
        //Restart level when pressing R
        if (Input.GetKeyDown(Key.R))
        {
            Restart();
            return;
        }

        if (player.HitTest(chaser))
        {
            Restart();
            return;
        }

        if (player.y > game.height + player.height * 2)
        {
            Restart();
            return;
        }
    }

    void Update()
    {
        HandleScrolling();
        HandleRestart();
    }

}
