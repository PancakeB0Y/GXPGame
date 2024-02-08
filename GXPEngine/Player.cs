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

    readonly float basePlayerSpeed = 4f;
    float playerSpeed = 4f;
    const float gravity = 0.5f;

    //Acceleration when walking
    float xVelocity = 0;
    const float xAcceleration = 0.12f;
    const float maxXVelocity = 2f;
    const float reactivityPercent = 0.1f; //Multiplier when moving in opposite direction of acceleration

    //Acceleration when jumping and falling
    float yVelocity = 0;
    const float yAcceleration = 0.5f;
    const float maxYVelocity = 12f;

    const float jumpImpulse = 5f; //Initial jump height
    const float jumpAcceleration = 0.4f; //Upward acceleration when holding jump button
    int jumpFrames = 0; //Frames while holding jump button
    const int maxJumpFrames = 13; //Max frames for extended jump
    int framesSinceGrounded = 0; //Frames since leaving platform
    const int maxFramesSinceGrounded = 5; //Max frames allowed to jump after leaving platform
    /*int framesSinceJump = 0; //Frames since jump button is pressed
    const int maxFramesSinceJump = 10; //Max frames to register jump before touching ground
    bool tolerateJump = false;*/

    bool isJumping = false;
    bool isGrounded = false;

    int idleFrames = 0;
    const int maxIdleFrames = 3;

    bool isExplosivePlaced = false;
    readonly List<Explosive> explosives;
    readonly List<(float distMult, Vector2 explosionDir)> explosionInfo;
    float curExplosiveCooldown;
    const float explosiveCooldown = 2000f; //In milliseconds

    bool isOnMovingPlatform = false;

    readonly Sound jumpSound = new Sound("assets/jump.wav");
    readonly Sound collectSound = new Sound("assets/collect.wav");

    int _collected = 0;
    public int Collected { get => _collected; set => _collected = value; }

    public Player(Level level, float pX, float pY) : base(32, 40)
    {
        this.level = level;

        SetXY(pX, pY);
        //Clear(Color.Green); //Show hitbox
        SetOrigin(width / 2, height / 2);

        //Create player sprite on top of hitbox
        playerSprite = new AnimationSprite("assets/character.png", 21, 1, -1, false, false);
        playerSprite.SetXY(-playerSprite.width / 2, -playerSprite.height / 2 - 4);
        this.AddChild(playerSprite);

        explosives = new List<Explosive>();
        explosionInfo = new List<(float distMult, Vector2 explosionDir)>();
        curExplosiveCooldown = explosiveCooldown;
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
        Collision col = MoveUntilCollision(0, gravity + yVelocity);
        if (col != null)
        {
            yVelocity = 0;
            //If player touches ground
            if (col.normal.y < 0)
            {
                isGrounded = true;
                framesSinceGrounded = 0;
                if (col.other.GetType() == typeof(MovingPlatform))
                {
                    if (!isOnMovingPlatform)
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
            yVelocity += yAcceleration;
            yVelocity = Mathf.Clamp(yVelocity, -maxYVelocity, maxYVelocity);

            framesSinceGrounded++;

            if (isOnMovingPlatform)
            {
                UnstickToPlatform(parent);
            }
        }

        //Handle directional movement
        if (Input.GetKey(Settings.P1Left))
        {
            MoveUntilCollision(-playerSpeed + xVelocity, 0);

            //If moving in the opposite direction of acceleration increase the acceleration more
            xVelocity -= xVelocity <= 0 ? xAcceleration :
                xAcceleration + playerSpeed * reactivityPercent;

            moving = true;
            playerSprite.Mirror(true, false);
        }
        if (Input.GetKey(Settings.P1Right))
        {
            MoveUntilCollision(playerSpeed + xVelocity, 0);

            //If moving in the opposite direction of acceleration increase the acceleration more
            xVelocity += xVelocity >= 0 ? xAcceleration :
                xAcceleration + playerSpeed * reactivityPercent;

            moving = true;
            playerSprite.Mirror(false, false);
        }
        if (!Input.GetKey(Settings.P1Left) && !Input.GetKey(Settings.P1Right))
        {
            idleFrames++;
            if (idleFrames >= maxIdleFrames)
            {
                idleFrames = maxIdleFrames;
                xVelocity = 0;
            }
        }
        xVelocity = Mathf.Clamp(xVelocity, -maxXVelocity, maxXVelocity);

        //Handle Jumping
        if (Input.GetKeyDown(Settings.Jump))
        {
            if (framesSinceGrounded > maxFramesSinceGrounded)
            {
                return;
            }

            isJumping = true;

            yVelocity = 0;
            yVelocity -= jumpImpulse;

            jumpSound.Play(false, 0, 0.3f);
        }
        if (Input.GetKeyUp(Settings.Jump))
        {
            jumpFrames = 0;
            isJumping = false;
        }
        if (Input.GetKey(Settings.Jump))
        {
            if (isJumping)
            {
                jumpFrames++;
                if (jumpFrames >= maxJumpFrames)
                {
                    jumpFrames = 0;
                    isJumping = false;
                }
                yVelocity -= jumpAcceleration;
            }
        }

        //Handle animation
        if (isJumping)
        {
            playerSprite.SetCycle(8, 1);
            idleFrames = 0;
        }
        else if (moving)
        {
            playerSprite.SetCycle(1, 4);
            idleFrames = 0;
        }
        else
        {
            playerSprite.SetCycle(0, 1);
        }

        playerSprite.Animate(0.05f);
    }

    void HandleExplosives()
    {
        if (Input.GetKeyDown(Settings.Explosive))
        {
            if (!isExplosivePlaced && curExplosiveCooldown >= explosiveCooldown)
            {
                explosives.Add(new Explosive(this));
                parent.AddChild(explosives[explosives.Count - 1]);
                isExplosivePlaced = true;
                curExplosiveCooldown = 0;
            }
            else if (isExplosivePlaced)
            {
                explosionInfo.Add(explosives[explosives.Count - 1].Explode());
                isExplosivePlaced = false;
            }
        }

        curExplosiveCooldown += Time.deltaTime;
        curExplosiveCooldown = Mathf.Clamp(curExplosiveCooldown, 0, explosiveCooldown);
    }

    void ApplyExplosionForce()
    {
        if (explosionInfo.Count == 0 || explosives.Count == 0)
        {
            playerSpeed = basePlayerSpeed;
            return;
        }

        //Limits control while in the air
        if (!isGrounded)
        {
            playerSpeed = 4f;
        }
        else
        {
            playerSpeed = basePlayerSpeed;
        }

        for (int i = explosionInfo.Count - 1; i >= 0; i--)
        {
            if (explosionInfo[i].distMult == 0)
            {
                explosives[i].LateDestroy();
                explosives.RemoveAt(i);
                explosionInfo.RemoveAt(i);
                return;
            }

            //If the player touches the ground after the explosion is stops pushing him upwards to avoid bouncing
            if (explosives[i].ExplosionForce < 6 && isGrounded)
            {
                //MoveUntilCollision(explosionInfo[i].explosionDir.x * explosives[i].ExplosionForce * explosionInfo[i].distMult, 0);
                explosives[i].ExplosionForce = 0.05f;
            }
            else
            {
                Collision col = MoveUntilCollision(explosionInfo[i].explosionDir.x * explosives[i].ExplosionForce * explosionInfo[i].distMult, explosionInfo[i].explosionDir.y * explosives[i].ExplosionForce * explosionInfo[i].distMult);
                if (col != null) //If a player collides with a wall it stops the force of the explosion
                {
                    explosives[i].ExplosionForce = 0.05f;
                }
            }

            explosives[i].ExplosionForce *= 0.98f;

            if (explosives[i].ExplosionForce <= 0.10f) //0.10f minimum explosionForce
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
        isOnMovingPlatform = true;
    }

    void UnstickToPlatform(GameObject movingPlatform)
    {
        level.AddChild(this);
        x += movingPlatform.x;
        y += movingPlatform.y;
        isOnMovingPlatform = false;
    }

    void UpdateHUD()
    {
        ((MyGame)game).GetHUD().SetExplosiveCooldown(curExplosiveCooldown, explosiveCooldown, GetGlobalXY());
        ((MyGame)game).GetHUD().UpdateCoinCount(_collected);
        if (isOnMovingPlatform)
        {
            ((MyGame)game).GetHUD().UpdateLevelProgress(x + parent.x);
        }
        else
        {
            ((MyGame)game).GetHUD().UpdateLevelProgress(x);
        }
    }

    public Vector2 GetGlobalXY()
    {
        return TransformPoint(0, 0);
    }

    //Called every frame
    void Update()
    {
        MovePlayer();
        HandleExplosives();
        ApplyExplosionForce();
        UpdateHUD();
    }

    void OnCollision(GameObject other)
    {
        if (other.GetType() == typeof(Collectible) && !((Collectible)other).IsCollected)
        {
            _collected++;
            collectSound.Play(false, 0, 0.3f);
            ((Collectible)other).Collect();
        }

        if (other.GetType() == typeof(Checkpoint) && !((Checkpoint)other).IsPassed)
        {
            ((Checkpoint)other).Pass(this);
        }
    }
}
