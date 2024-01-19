using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using GXPEngine;
using GXPEngine.Core;

public class Explosive : EasyDraw
{
    readonly Player player;
    readonly Level level;

    public float explosionForce = 8;
    const int closeExplosionRange = 60; //The distance within which the explosion is the strongest
    const int explosionRange = 300; //The distance within which the explosion has any effect
    float distMult; //Explosion multiplier based on the distance from the player to the explosive
    Vector2 explosionDir;

    public Explosive(Player player, Level level) : base(20, 20, false)
    {
        this.player = player;
        this.level = level;

        Clear(Color.Red);
        SetXY(this.player.x, this.player.y - width / 2);
        SetOrigin(width / 2, height / 2);
    }

    public (float, Vector2) Explode()
    {
        Vector2 globalXY = level.GetGlobalXY(this); //Global coordinates of the explosive
        Vector2 playerXY = level.GetGlobalXY(player); //Global coordinates of the player
        float distToPlayer = GlobalDistanceTo(player);

        //Returns a value between 2 and 0 based on the distance from the explosive to the player
        //If the player is within the closeExplosionRange the value is always 2
        distMult = distToPlayer <= closeExplosionRange ? 2 : 2 - (Mathf.Clamp(distToPlayer - closeExplosionRange, 0, explosionRange) / (explosionRange / 2));

        explosionDir = new Vector2(playerXY.x - globalXY.x, playerXY.y - globalXY.y - (player.height / 2));
        explosionDir = explosionDir.NormalizeVector();

        return (distMult, explosionDir);
    }

    float GlobalDistanceTo(GameObject other)
    {
        Vector2 globalXY = level.GetGlobalXY(this);
        Vector2 otherGlobalXY = level.GetGlobalXY(other);
        float dx = otherGlobalXY.x - globalXY.x;
        float dy = otherGlobalXY.y - globalXY.y;
        return Mathf.Sqrt(dx * dx + dy * dy);
    }
}
