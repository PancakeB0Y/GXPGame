using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using GXPEngine;
using GXPEngine.Core; // for Collision
public class Player : EasyDraw
{
    readonly Level level;
    readonly AnimationSprite playerSprite;

    float basePlayerSpeed = 3;
    float playerSpeed = 3;
    const float gravity = 0.1f;

    //Acceleration when walking
    float xAcceleration = 0;
    const float xAccelerationIncrease = 0.05f;
    const float maxXAcceleration = 1.5f;
    const float reactivityPercent = 0.01f; //Multiplier when moving in opposite direction of acceleration

    //Acceleration when jumping and falling
    float yAcceleration = 0;
    const float yAccelerationIncrease = 0.15f;
    const float maxYAcceleration = 3.3f;

    const float jumpImpulse = 2;
    const float jumpAcceleration = 0.12f; //Upward acceleration when holding jump button
    int jumpFrames = 0; //Frames while holding jump button
    const int maxJumpFrames = 30;

    bool isJumping = false;
    bool isGrounded = false;

    int idleFrames = 0;
    const int maxIdleFrames = 5;

    bool isExplosivePlaced = false;
    List<Explosive> explosives;
    List<(float distMult, Vector2 explosionDir)> explosionInfo;

    bool isOnPlatform = false;

    public Player(Level level, float pX, float pY) : base(50, 65)
    {
        SetXY(pX, pY);
        Clear(Color.Green);
        SetOrigin(width / 2, height);

        //Create player sprite on top of hitbox
        playerSprite = new AnimationSprite("platformerPack_character.png", 4, 2, -1, false, false);
        playerSprite.SetXY(-playerSprite.width / 2, -playerSprite.height);
        this.AddChild(playerSprite);

        explosives = new List<Explosive>();
        explosionInfo = new List<(float distMult, Vector2 explosionDir)>();
        this.level = level;
    }

    public Player(Level level, float pX, float pY, float speed) : this(level, pX, pY)
    {
        playerSpeed = speed;
        basePlayerSpeed = speed;
    }

    void MovePlayer()
    {
        bool moving = false;

        //Handle gravity and downward acceleration
        Collision col = MoveUntilCollision(0, gravity + yAcceleration);
        if (col != null)
        {
            yAcceleration = 0;
            if (col.normal.y < 0) //If player touches floor -> isGrounded = true
            {
                isGrounded = true;
                if (col.other.GetType() == typeof(MovingPlatform))
                {
                    if (!isOnPlatform)
                    {
                        StickToPlatform(col.other);
                    }
                }
            }
        }
        else
        {
            isGrounded = false;

            //Falling speed gradually increases when not touching ground
            yAcceleration += yAccelerationIncrease;
            yAcceleration = Mathf.Clamp(yAcceleration, -maxYAcceleration, maxYAcceleration);

            if (isOnPlatform)
            {
                Console.WriteLine();
                UnstickToPlatform(parent);
            }
        }

        //Handle directional movement
        if (Input.GetKey(Key.A))
        {
            MoveUntilCollision(-playerSpeed + xAcceleration, 0);

            //If moving in the opposite direction of acceleration increase the acceleration more
            xAcceleration -= xAcceleration <= 0 ? xAccelerationIncrease :
                xAccelerationIncrease + playerSpeed * reactivityPercent;

            moving = true;
            playerSprite.Mirror(true, false);
        }
        if (Input.GetKey(Key.D))
        {
            MoveUntilCollision(playerSpeed + xAcceleration, 0);

            //If moving in the opposite direction of acceleration increase the acceleration more
            xAcceleration += xAcceleration >= 0 ? xAccelerationIncrease :
                xAccelerationIncrease + playerSpeed * reactivityPercent;

            moving = true;
            playerSprite.Mirror(false, false);
        }
        if (!Input.GetKey(Key.A) && !Input.GetKey(Key.D))
        {
            idleFrames++;
            if (idleFrames >= maxIdleFrames)
            {
                idleFrames = maxIdleFrames;
                xAcceleration = 0;
            }
        }
        xAcceleration = Mathf.Clamp(xAcceleration, -maxXAcceleration, maxXAcceleration);

        //Handle Jumping
        if (Input.GetKeyDown(Key.SPACE))
        {
            if (!isGrounded) { return; }

            isJumping = true;

            yAcceleration = 0;
            yAcceleration -= jumpImpulse;
        }
        if (Input.GetKeyUp(Key.SPACE))
        {
            isJumping = false;
        }
        if (Input.GetKey(Key.SPACE))
        {
            if (isJumping)
            {
                jumpFrames++;
                if (jumpFrames >= maxJumpFrames)
                {
                    jumpFrames = 0;
                    isJumping = false;
                }
                yAcceleration -= jumpAcceleration;
            }
        }

        //Handle animation
        if (isJumping)
        {
            playerSprite.SetCycle(1, 1);
            idleFrames = 0;
        }
        else if (moving)
        {
            playerSprite.SetCycle(2, 2);
            idleFrames = 0;
        }
        else
        {
            playerSprite.SetCycle(0, 1);
        }

        playerSprite.Animate(0.1f);
    }

    void HandleExplosives()
    {
        if (Input.GetKeyDown(Key.K))
        {
            if (!isExplosivePlaced)
            {
                explosives.Add(new Explosive(this));
                level.AddChild(explosives[explosives.Count - 1]);
                isExplosivePlaced = true;
            }
            else
            {
                explosionInfo.Add(explosives[explosives.Count - 1].Explode());
                isExplosivePlaced = false;
            }
        }
    }

    void ApplyExplosionForce()
    {
        if (explosionInfo.Count == 0 || explosives.Count == 0)
        {
            playerSpeed = basePlayerSpeed;
            return;
        }
        if (!isGrounded) { playerSpeed = 1; }

        for (int i = explosionInfo.Count - 1; i >= 0; i--)
        {
            if (explosionInfo[i].distMult == 0)
            {
                explosives[i].LateDestroy();
                explosives.RemoveAt(i);
                explosionInfo.RemoveAt(i);
                return;
            }

            //Remove the explosive visualizer 
            explosives[i].Clear(0, 0, 0, 0);

            //If the player touches the ground after the explosion is stops pushing him upwards to avoid bouncing
            if (explosives[i].explosionForce < 8 && isGrounded)
            {
                MoveUntilCollision(explosionInfo[i].explosionDir.x * explosives[i].explosionForce * explosionInfo[i].distMult, 0);
            }
            else
            {
                Collision col = MoveUntilCollision(explosionInfo[i].explosionDir.x * explosives[i].explosionForce * explosionInfo[i].distMult, explosionInfo[i].explosionDir.y * explosives[i].explosionForce * explosionInfo[i].distMult);
                if (col != null) //If a player collides with a wall it stops the force of the explosion
                {
                    explosives[i].explosionForce = 0.05f;
                }
            }

            explosives[i].explosionForce *= 0.98f;

            if (explosives[i].explosionForce <= 0.05f)
            {
                explosives[i].LateDestroy();
                explosives.RemoveAt(i);
                explosionInfo.RemoveAt(i);
            }

        }
    }

    void StickToPlatform(GameObject movingPlatform)
    {
        movingPlatform.AddChild(this);
        x -= movingPlatform.x;
        y -= movingPlatform.y;
        isOnPlatform = true;
    }

    void UnstickToPlatform(GameObject movingPlatform)
    {
        level.AddChild(this);
        x += movingPlatform.x;
        y += movingPlatform.y;
        isOnPlatform = false;
    }

    //Called every frame
    void Update()
    {
        MovePlayer();
        HandleExplosives();
        ApplyExplosionForce();
    }

    //Called every frame
    void OnCollision(GameObject other)
    {
    }
}
