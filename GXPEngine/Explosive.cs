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

    public float explosionForce = 8;
    const int closeExplosionRange = 60; //The distance within which the explosion is the strongest
    const int explosionRange = 300; //The distance within which the explosion has any effect
    float distMult; //Explosion multiplier based on the distance from the player to the explosive
    Vector2 explosionDir;

    public Explosive(Player player) : base(20, 20, false)
    {
        this.player = player;

        Clear(Color.Red);
        SetXY(this.player.x, this.player.y - width / 2);
        SetOrigin(width / 2, height / 2);
    }

    public (float, Vector2) Explode()
    {
        float distToPlayer = DistanceTo(player);

        //Returns a value between 2 and 0 based on the distance from the explosive to the player
        //If the player is within the closeExplosionRange the value is always 2
        distMult = distToPlayer <= closeExplosionRange ? 2 : 2 - (Mathf.Clamp(distToPlayer - closeExplosionRange, 0, explosionRange) / (explosionRange / 2));

        explosionDir = new Vector2(player.x - x, player.y - y - (player.height / 2));
        explosionDir = explosionDir.NormalizeVector();

        return (distMult, explosionDir);
    }
}
