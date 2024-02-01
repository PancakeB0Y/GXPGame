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

    EasyDraw explosiveVisual;
    Sound placeSound = new Sound("assets/place.wav");
    Sound explodeSound = new Sound("assets/explode.wav");

    float _explosionForce = 6;
    public float ExplosionForce { get => _explosionForce; set => _explosionForce = value; }

    const int closeExplosionRange = 60; //The distance within which the explosion is the strongest
    const int explosionRange = 300; //The distance within which the explosion has any effect
    float distMult; //Explosion multiplier based on the distance from the player to the explosive
    Vector2 explosionDir;

    public Explosive(Player player) : base(10, 10, false)
    {
        this.player = player;

        //Clear(Color.Red);
        SetXY(this.player.x, this.player.y + this.player.height / 2 - width / 2);
        SetOrigin(width / 2, height / 2);

        explosiveVisual = new EasyDraw("assets/explosive.png", false);
        explosiveVisual.width = 15;
        explosiveVisual.height = 15;
        explosiveVisual.SetColor(0.5f, 0.5f, 1f);
        explosiveVisual.SetXY(-7.5f, -7.5f);
        AddChild(explosiveVisual);

        placeSound.Play(false, 0, 0.3f);
    }

    public (float, Vector2) Explode()
    {
        //Remove the explosive visualizer 
        Clear(0, 0, 0, 0);
        explosiveVisual.Clear(0, 0, 0, 0);

        explodeSound.Play(false, 0, 0.4f);

        Vector2 globalXY = GetGlobalXY(); //Global coordinates of the explosive
        Vector2 playerXY = player.GetGlobalXY(); //Global coordinates of the player

        float distToPlayer = GetDistance(globalXY, playerXY);

        //Returns a value between 2 and 0 based on the distance from the explosive to the player
        //If the player is within the closeExplosionRange the value is always 2
        distMult = distToPlayer <= closeExplosionRange ? 2 : 2 - (Mathf.Clamp(distToPlayer - closeExplosionRange, 0, explosionRange) / (explosionRange / 2));

        explosionDir = new Vector2(playerXY.x - globalXY.x, playerXY.y - globalXY.y);
        explosionDir = explosionDir.NormalizeVector();

        return (distMult, explosionDir);
    }

    float GetDistance(Vector2 thisGlobalXY, Vector2 otherGlobalXY)
    {
        float dx = thisGlobalXY.x - otherGlobalXY.x;
        float dy = thisGlobalXY.y - otherGlobalXY.y;
        return Mathf.Sqrt(dx * dx + dy * dy);
    }

    public Vector2 GetGlobalXY()
    {
        return TransformPoint(0, 0);
    }
}
