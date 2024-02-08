using System;
using System.Collections;
using System.Drawing;
using System.Runtime.InteropServices;
using GXPEngine;
using GXPEngine.Core;
using TiledMapParser;

public class HUD : GameObject
{
    readonly EasyDraw explosiveCooldown;
    readonly EasyDraw levelProgress;
    readonly EasyDraw coinCount;
    readonly EasyDraw coinVisual;
    float levelProgressStartX = 0;
    float levelProgressEndX = 0;

    public HUD()
    {
        explosiveCooldown = new EasyDraw(100, 20, false);
        explosiveCooldown.NoStroke();
        AddChild(explosiveCooldown);

        levelProgress = new EasyDraw(Settings.Width / 2, 10, false);
        levelProgress.SetXY(Settings.Width / 4, 10);
        levelProgress.NoStroke();
        AddChild(levelProgress);

        coinVisual = new EasyDraw("assets/coin.png", false);
        coinVisual.SetXY(20, 10);
        coinVisual.width = 50;
        coinVisual.height = 50;
        AddChild(coinVisual);

        coinCount = new EasyDraw(250, 60, false);
        coinCount.SetXY(70, -5);
        coinCount.Text(": " + 0, true);
        coinCount.TextSize(25);
        AddChild(coinCount);
    }

    public void SetExplosiveCooldown(float curCooldown, float maxCooldown, Vector2 playerXY)
    {
        float barX = playerXY.x - (explosiveCooldown.width / 4);
        float barY = playerXY.y - (explosiveCooldown.height * 2.5f) + 20;

        explosiveCooldown.SetXY(barX, barY);

        //When the cooldown is over hide cooldown bar
        if (curCooldown >= maxCooldown)
        {
            explosiveCooldown.Clear(255, 255, 255, 0);
            return;
        }
        /*explosiveCooldown.Fill(Color.Red);
        explosiveCooldown.Rect(0, 0, 120, 20);*/
        explosiveCooldown.Fill(Color.White);
        explosiveCooldown.Rect(0, 0, explosiveCooldown.width * (curCooldown / maxCooldown), explosiveCooldown.height / 2);

    }

    public void SetLevelProgress(float startX, float endX)
    {
        levelProgressStartX = startX;
        levelProgressEndX = endX;
    }

    public void UpdateLevelProgress(float playerX)
    {
        levelProgress.Clear(Color.White);
        levelProgress.Fill(Color.Blue);

        float barX = levelProgress.width * (playerX - levelProgressStartX) / (levelProgressEndX - levelProgressStartX);
        levelProgress.Rect(barX, 2.5f, 5, 19);

        levelProgress.Fill(Color.Yellow);
        levelProgress.Rect(levelProgress.width - 5, 2.5f, 5, 10);
    }

    public void UpdateCoinCount(int count)
    {
        coinCount.Text(": " + (((MyGame)game).CoinCount + count), true);
    }
}
