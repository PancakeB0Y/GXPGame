using System;
using System.Drawing;
using System.Runtime.InteropServices;
using GXPEngine;

public class Collectible : AnimationSprite
{
    bool _isCollected = false;
    public bool IsCollected { get => _isCollected; }
    public Collectible(float pX, float pY, int pWidth = 32, int pHeight = 32) : base("assets/coin_animation.png", 6, 1)
    {
        SetXY(pX, pY);
        width = pWidth;
        height = pHeight;
        collider.isTrigger = true;

        SetCycle(0, 5);
    }

    void Update()
    {
        Animate(0.1f);
    }

    public void Collect()
    {
        _isCollected = true;
        visible = false;
    }
}
