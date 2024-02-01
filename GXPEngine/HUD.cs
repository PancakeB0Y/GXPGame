using System;
using System.Collections;
using System.Drawing;
using System.Runtime.InteropServices;
using GXPEngine;
using GXPEngine.Core;

public class HUD : GameObject
{
    readonly EasyDraw explosiveCooldown;

    public HUD()
    {
        explosiveCooldown = new EasyDraw(120, 20, false);
        explosiveCooldown.NoStroke();
        AddChild(explosiveCooldown);
    }

    public void SetExplosiveCooldown(float curCooldown, float maxCooldown, Vector2 playerXY)
    {
        float barX = playerXY.x - (explosiveCooldown.width / 4);
        float barY = playerXY.y - (explosiveCooldown.height * 2.5f);

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
        explosiveCooldown.Rect(0, 20, explosiveCooldown.width * (curCooldown / maxCooldown), explosiveCooldown.height / 2 - 5);

    }
}
