using System;
using System.Drawing;
using GXPEngine;
using TiledMapParser;

public class Level : GameObject
{
    public Player player;

    /*public Level()
    {
        player = new Player(400, 400);
        AddChild(player);

        AddChild(new Platform(0, 400, 10000, 100));
        AddChild(new Platform(1100, 400, 200, 100));
        AddChild(new Platform(1100, 100, 400, 100));
        *//*AddChild(new Platform(900, 400, 100, 100));
        AddChild(new Platform(1100, 400, 400, 100));
        AddChild(new Platform(1600, 400, 200, 100));*//*
        AddChild(new Platform(-300, 0, 100, 1000));
    }
    
    */
    public Level(string filename)
    {
        Map leveldata = MapParser.ReadMap(filename);

        spawnTiles(leveldata);
        spawnObjects(leveldata);
    }

    void spawnTiles(Map leveldata)
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
    void spawnObjects(Map leveldata)
    {
        if (leveldata.ObjectGroups == null || leveldata.ObjectGroups.Length == 0) return;

        ObjectGroup objectGroup = leveldata.ObjectGroups[0];
        if (objectGroup.Objects == null || objectGroup.Objects.Length == 0) return;

        foreach (TiledObject obj in objectGroup.Objects)
        {
            switch (obj.Name)
            {
                case "Player":
                    player = new Player(obj.X, obj.Y);
                    AddChild(player);
                    break;
            }
        }


    }

    void Update()
    {
    }

}
