using System;
using System.Drawing;
using System.Runtime.InteropServices;
using GXPEngine;
using GXPEngine.Core;

public class MovingPlatform : Platform
{
    float speed;
    Vector2 startXY;
    Vector2 endXY;

    float curTime = 0.0f;
    public MovingPlatform(float pX, float pY, int pWidth, int pHeight, float speed = 0.5f) : base(pX, pY, pWidth, pHeight)
    {
        //Clear(Color.Blue);
        startXY.x = pX;
        startXY.y = pX;
        endXY.x = pX + pWidth * 2;
        endXY.y = pY;
        this.speed = speed;
        EasyDraw platformVisual = new EasyDraw("assets/platform.png", false);
        platformVisual.width = pWidth;
        platformVisual.height = pHeight;
        AddChild(platformVisual);
    }

    public MovingPlatform(float pX, float pY, int pWidth, int pHeight, float endX, float endY, float speed = 0.5f) : this(pX, pY, pWidth, pHeight, speed)
    {
        endXY.x = endX;
        endXY.y = endY;
    }

    public MovingPlatform(float pX, float pY, int pWidth, int pHeight, float startX, float startY, float endX, float endY, float speed = 0.5f) : this(pX, pY, pWidth, pHeight, endX, endY, speed)
    {
        startXY.x = startX;
        startXY.y = startY;
    }

    void MovePlatform()
    {
        curTime += Time.deltaTime * 0.001f * speed;
        Vector2 movedPoint = new Vector2(x, y);
        movedPoint.PingPong(curTime, startXY, endXY);
        Move(movedPoint.x - x, movedPoint.y - y);
    }

    void Update()
    {
        MovePlatform();
    }
}
