using System;
using System.Drawing;
using GXPEngine;
using GXPEngine.Core; // for Collision
public class Player : EasyDraw
{
    AnimationSprite playerSprite;

    const float playerSpeed = 3;
    const float gravity = 0.1f;

    float xAcceleration = 0;
    const float xAccelerationIncrease = 0.05f;
    const float maxXAcceleration = 5f;

    float yAcceleration = 0;
    const float yAccelerationIncrease = 0.15f;
    const float minYAcceleration = -3.3f; //Max jump acceleration
    const float maxYAcceleration = 5f;

    const int jumpCount = 2;
    int currJumpCount = 0;
    const float jumpImpulse = 2;
    const float jumpAcceleration = 0.05f; //Upward acceleration when holding jump button

    const float reactivityPercent = 0.01f; //Multiplier when moving in opposite direction of acceleration

    int idleFrames = 0;

    bool isJumping = false;

    public Player(int pX, int pY) : base(50, 65)
    {
        SetXY(pX, pY);
        Clear(Color.Green);
        SetOrigin(width / 2, height);

        playerSprite = new AnimationSprite("platformerPack_character.png", 4, 2, -1, false, false);
        playerSprite.SetXY(-playerSprite.width / 2, -playerSprite.height);
        this.AddChild(playerSprite);
    }

    void MovePlayer()
    {
        Console.WriteLine(xAcceleration);

        bool moving = false;

        //Handle gravity and downward acceleration
        Collision col = MoveUntilCollision(0, gravity + yAcceleration);
        if (col != null)
        {
            yAcceleration = 0;
            if (col.normal.y < 0) //If player touches floor reset available jumps
            {
                currJumpCount = 0;
            }
        }
        else if (!isJumping)
        {
            yAcceleration += yAccelerationIncrease;
            yAcceleration = Mathf.Clamp(yAcceleration, minYAcceleration, maxYAcceleration);
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
        xAcceleration = Mathf.Clamp(xAcceleration, -maxXAcceleration, maxXAcceleration);

        //Handle Jumping
        if (Input.GetKeyDown(Key.SPACE))
        {
            if (currJumpCount >= jumpCount) { return; }

            isJumping = true;
            currJumpCount++;

            yAcceleration = 0;
            yAcceleration -= jumpImpulse;
        }
        if (Input.GetKeyUp(Key.SPACE))
        {
            isJumping = false;
        }
        if (Input.GetKey(Key.SPACE))
        {
            if (yAcceleration <= minYAcceleration)
            {
                isJumping = false;
            }
            if (isJumping)
            {
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

            idleFrames++;
            if (idleFrames >= 5)
            {
                idleFrames = 5;
                xAcceleration = 0;
            }
        }

        playerSprite.Animate(0.1f);
    }

    //Called every frame
    void Update()
    {
        MovePlayer();
    }

    //Called every frame
    void OnCollision(GameObject other)
    {
    }
}
