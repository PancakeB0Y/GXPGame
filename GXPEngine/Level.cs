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

        player = new Player(400, 400);
        AddChild(player);

        AddChild(new Platform(0, 400, 10000, 100));
        AddChild(new Platform(1100, 400, 200, 100));
        AddChild(new Platform(1100, 100, 400, 100));
        /*AddChild(new Platform(900, 400, 100, 100));
        AddChild(new Platform(1100, 400, 400, 100));
        AddChild(new Platform(1600, 400, 200, 100));*/
        AddChild(new Platform(-300, 0, 100, 1000));
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
