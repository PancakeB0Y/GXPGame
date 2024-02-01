using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection.Emit;
using GXPEngine;
using GXPEngine.Core;
using TiledMapParser;

public class Level : GameObject
{
    Player player;
    Chaser chaser;
    LevelChange levelChange;
    EasyDraw imageBackground;
    SpriteBatch spriteBackground;
    readonly Map leveldata;

    readonly Sound nextLevel = new Sound("assets/complete.wav");
    public Level(string filename)
    {
        leveldata = MapParser.ReadMap(filename);

        //Infinite map crashes the game
        if (leveldata.Infinite != 0)
        {
            Console.WriteLine("Tiled map is set to infinite. \nChange infinite value to 0 in order to load the map");
        }
        else
        {
            SpawnBackgroundImage(leveldata);
            SpawnTiles(leveldata);
            SpawnObjects(leveldata);

        }
    }

    void SpawnTiles(Map leveldata)
    {
        if (leveldata.Layers == null || leveldata.Layers.Length == 0) return;

        foreach (Layer curLayer in leveldata.Layers)
        {
            short[,] tileNumbers = curLayer.GetTileArray();
            if (curLayer.Name == "Background")
            {
                spriteBackground = new SpriteBatch();
                for (int row = 0; row < curLayer.Height; row++)
                {
                    for (int col = 0; col < curLayer.Width; col++)
                    {
                        int tileNumber = tileNumbers[col, row];
                        if (tileNumber > 0)
                        {
                            AnimationSprite tile = new AnimationSprite("assets/tileset.png", 12, 2, addCollider: false);
                            spriteBackground.AddChild(tile);
                            tile.SetFrame(tileNumber - 1);
                            tile.SetXY(tile.width * col, tile.height * row);
                        }
                    }
                }
                spriteBackground.Freeze();
                //spriteBackground.SetColor(0.5f, 0.5f, 0.5f);
                AddChild(spriteBackground);
            }
            else
            {
                for (int row = 0; row < curLayer.Height; row++)
                {
                    for (int col = 0; col < curLayer.Width; col++)
                    {
                        int tileNumber = tileNumbers[col, row];
                        if (tileNumber > 0)
                        {
                            AnimationSprite tile = new AnimationSprite("assets/tileset.png", 12, 2);
                            tile.SetFrame(tileNumber - 1);
                            tile.SetXY(tile.width * col, tile.height * row);
                            AddChild(tile);
                        }
                    }
                }
            }
        }
    }

    void SpawnBackgroundImage(Map leveldata)
    {
        if (leveldata.ImageLayers == null || leveldata.ImageLayers.Length == 0) return;
        foreach (ImageLayer curLayer in leveldata.ImageLayers)
        {
            string backgroundFilename = curLayer.Image.ToString();
            backgroundFilename = backgroundFilename.Split(' ')[0];

            imageBackground = new EasyDraw(backgroundFilename, false);
            imageBackground.width = leveldata.Width * leveldata.TileWidth;
            imageBackground.height = leveldata.Height * leveldata.TileHeight;
            AddChild(imageBackground);
        }
    }

    void SpawnObjects(Map leveldata)
    {
        if (leveldata.ObjectGroups == null || leveldata.ObjectGroups.Length == 0) return;

        foreach (ObjectGroup objectGroup in leveldata.ObjectGroups)
        {
            if (objectGroup.Objects == null || objectGroup.Objects.Length == 0) return;

            foreach (TiledObject obj in objectGroup.Objects)
            {
                switch (obj.Name)
                {
                    case "Text":
                        TextBox text = new TextBox(obj.X, obj.Y, (int)obj.Width, (int)obj.Height, obj.textField);
                        AddChild(text);
                        break;
                    case "MovingPlatform":
                        MovingPlatform movingPlatform = new MovingPlatform(obj.X, obj.Y, (int)obj.Width, (int)obj.Height, obj.GetFloatProperty("StartX", obj.X), obj.GetFloatProperty("StartY", obj.Y), obj.GetFloatProperty("EndX", obj.X + obj.Width * 2), obj.GetFloatProperty("EndY", obj.Y), obj.GetFloatProperty("Speed", 0.5f));
                        AddChild(movingPlatform);
                        break;
                    case "LevelChange":
                        levelChange = new LevelChange(obj.X, obj.Y, (int)obj.Width, (int)obj.Height, obj.GetStringProperty("NextLevel"));
                        AddChild(levelChange);
                        break;
                    case "Player":
                        //player = new Player(this, obj.X, obj.Y, obj.GetFloatProperty("Speed", 3));
                        player = new Player(this, obj.X, obj.Y);
                        AddChild(player);
                        break;
                    case "Chaser":
                        chaser = new Chaser(obj.X, obj.Y, (int)obj.Width, (int)obj.Height, obj.GetFloatProperty("Speed", 0.5f), obj.GetBoolProperty("MoveRight", true));
                        AddChild(chaser);
                        break;
                    case "Button":
                        Button button = new Button(obj.X, obj.Y, (int)obj.Width, (int)obj.Height, obj.GetStringProperty("NextLevel", ""), obj.GetStringProperty("Image", ""));
                        AddChild(button);
                        break;
                }
            }
        }
    }

    Vector2 GetGlobalXY(GameObject item)
    {
        if (item == null) return new Vector2(0, 0);
        float valueX = item.x;
        float valueY = item.y;
        if (item.parent != null && item.parent != this)
        {
            valueX += GetGlobalXY(item.parent).x;
            valueY += GetGlobalXY(item.parent).y;
        }
        return new Vector2(valueX, valueY);
    }

    void HandleScrolling()
    {
        Vector2 playerXY = GetGlobalXY(player);
        //Scroll following the player
        if (player != null)
        {
            if (playerXY.x + x > 640)
            {
                x = 640 - playerXY.x;
            }
            if (playerXY.x + x < 640)
            {
                x = 640 - playerXY.x;
            }
        }

        /*if (spriteBackground != null)
        {
            spriteBackground.x = x * 0.5f;
        }*/
    }

    void DestroyChildren()
    {
        List<GameObject> children = GetChildren();
        for (int i = children.Count - 1; i >= 0; i--)
        {
            children[i].LateDestroy();
        }
    }

    void Restart()
    {
        DestroyChildren();
        SpawnBackgroundImage(leveldata);
        SpawnTiles(leveldata);
        SpawnObjects(leveldata);
    }
    void HandleRestart()
    {
        //Restart level when pressing R
        if (Input.GetKeyDown(Key.R))
        {
            Restart();
            return;
        }

        if (chaser != null && player.HitTest(chaser))
        {
            Restart();
            return;
        }

        if (player != null && player.y > game.height + player.height * 2)
        {
            Restart();
            return;
        }
    }


    void SwitchLevel()
    {
        if (levelChange != null && player != null && player.HitTest(levelChange) && levelChange.NextLevel != "")
        {
            nextLevel.Play();
            ((MyGame)game).LoadLevel(levelChange.NextLevel);
        }
    }

    void Update()
    {
        HandleScrolling();
        HandleRestart();
        SwitchLevel();
    }

}
