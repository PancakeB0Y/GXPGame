using System;
using System.Drawing;
using System.Runtime.InteropServices;
using GXPEngine;

public class Chaser : EasyDraw
{
    float speed;
    bool moveRight;
    public Chaser(float pX, float pY, int pWidth, int pHeight, float speed = 0.5f, bool moveRight = true) : base(pWidth, pHeight)
    {
        Clear(Color.Red);
        SetXY(pX, pY);
        this.speed = speed;
        this.moveRight = moveRight;
        collider.isTrigger = true;
    }

    void MoveChaser()
    {
        Move(!moveRight ? -speed : speed, 0);
    }

    void Update()
    {
        MoveChaser();
    }
}
