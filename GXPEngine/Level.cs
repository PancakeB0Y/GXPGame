using System;
using System.Drawing;
using GXPEngine;

public class Level : GameObject
{
    Game myGame;
    float horizontalCenter;

    Player player;

    public Level(Game myGame)
    {
        this.myGame = myGame;
        horizontalCenter = myGame.width / 2;

        player = new Player(400, 300);
        AddChild(player);

        AddChild(new Platform(1200, 300, 3000, 100));
        AddChild(new Platform(-300, 0, 100, 1000));
        AddChild(new Platform(600, 115, 200, 100));
    }

    void Update()
    {
        HandleScrolling();
    }

    void HandleScrolling()
    {
        //Scroll when player is too far right
        if (player.x + x > horizontalCenter)
        {
            x = horizontalCenter - player.x;
        }
        if (player.x + x < horizontalCenter)
        {
            x = horizontalCenter - player.x;
        }
    }
}
