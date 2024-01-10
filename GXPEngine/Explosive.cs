using System;
using System.Drawing;
using System.Runtime.InteropServices;
using GXPEngine;

public class Explosive : EasyDraw
{
    Player curPlayer;

    float explosionForce = 12;
    const float maxExplosion = 500;
    float totalExplosion = 0;

    public bool hasExploded = false;
    public Explosive(float pX, float pY, Player curPlayer) : base(20, 20, false)
    {
        this.curPlayer = curPlayer;

        Clear(Color.Red);
        SetXY(pX, pY);
    }

    public void Explode()
    {
        hasExploded = true;
    }

    void Exploding()
    {
        if (hasExploded)
        {
            Clear(0, 0, 0, 0);
            curPlayer.MoveUntilCollision(curPlayer.xAcceleration + explosionForce, -explosionForce);
            explosionForce *= 0.98f;


            if (explosionForce <= 0.01f)
            {
                LateDestroy();
            }
        }
    }

    void Update()
    {
        Exploding();
    }
}
