using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace xInvaders;

/// <summary>
/// Represents the player's rocket in the game. 
/// This class handles the rocket's position, movement, and rendering, including the rocket's flames animation. 
/// It also provides a bounding box for collision detection.
/// </summary>
internal class Rocket
{

    public event Action<Vector2> FirePressed; // Event raised when the rocket fires. Subscribers receive the rocket position.
    
    private GraphicsDevice graphicsDevice;

    // For rocket:
    public Vector2 RocketPosition; 
    private Vector2 rocketOrigin;
    private Texture2D rocketSprite;
    
    // Auxiliary variables to control the rocket's movement speed based on how long a key is pressed.
    private float timeRight, timeLeft, timeUp, timeDown;

    // For flames:
    private Texture2D flameSprite;
    private Vector2 flamePosition;
  
    
    private int frameIndex;
    private double flameAnimationAccumulator;
  
    private const int ssNumberOfFrames = 8; // Total number of frames in srpite sheet for rocket flames animation.
    private const int ssNumberOfRows = 1; // Total number of rows in sprite sheet for rocket flames animation.
    private const int ssNumberOfColumns = 8; // Total number of columns in sprite sheet for rocket flames animation.

    private int frameWidth;
    private int frameHeight;
    private int currentFrameXPosition;  // X position of the current frame in the sprite sheet.
    private int currentFrameYPosition;  // Y position of the current frame in the sprite sheet.

    // Cached rectangles for flame animation, recalculated only when needed (in Update), not on every Draw call.
    private Rectangle flameSourceRectangle;
    private Rectangle flameDestinationRectangle;
    
    // Offsets:
    private const int yOffset = 9;  // The flames are drawn yOffset pixels below the rocket.
    private const int animationSpeed = 11; // Speed of rocket flame animation.
    private const double flameFrameIntervalSeconds = animationSpeed / 60.0;
    private const float initialMovementSpeed = 120f;
    private const float acceleratedMovementSpeed = 300f;
    private const float accelerationDelaySeconds = 8f / 60f;

 



    /// <summary>
    /// Gets the bounding box of the rocket for collision detection.
    /// </summary>
    public Rectangle BoundingBox
    {
        get
        {
            // Get the sprite's bounds
            Rectangle spriteBounds = rocketSprite.Bounds;
            // Add the object's position to it as an offset
            spriteBounds.Offset(RocketPosition - rocketOrigin);
            return spriteBounds;
        }
    }

    public Rocket(ContentManager Content, GraphicsDevice graphicsDevice)
    {
        rocketSprite = Assets.RocketBody;
        flameSprite = Assets.RocketFlames;

        // Set the origin to the center of the rocket sprite
        // (This is because the rocket sprite is drawn from its center, not from its top-left corner)
        rocketOrigin = new Vector2(rocketSprite.Width / 2, rocketSprite.Height / 2);

        // Set the initial position of the rocket to the center of the screen horizontally and near the bottom vertically
        RocketPosition = new Vector2(XInvaders.XRes / 2, XInvaders.YRes - 100);

        // Preparation for the flames animation
        frameWidth = flameSprite.Width / ssNumberOfColumns;
        frameHeight = flameSprite.Height / ssNumberOfRows;

        this.graphicsDevice = graphicsDevice;
    }

   

    public void Update(GameTime gameTime)
    {
        flameAnimationAccumulator += gameTime.ElapsedGameTime.TotalSeconds;

        while (flameAnimationAccumulator >= flameFrameIntervalSeconds)
        {
            flameAnimationAccumulator -= flameFrameIntervalSeconds;
            frameIndex++;

            if (frameIndex == ssNumberOfFrames)
                frameIndex = 0;
        }

        // Calculate the current frame's position in the sprite sheet
        currentFrameXPosition = (frameIndex  %  ssNumberOfColumns) * frameWidth;
        currentFrameYPosition = (frameIndex  /  ssNumberOfColumns) * frameHeight;

        // Recalculate the flame rectangles here (only when the position/frame actually changes)
        // instead of doing it every Draw call, avoiding unnecessary struct construction each frame.
        flameSourceRectangle = new Rectangle(currentFrameXPosition, currentFrameYPosition, frameWidth, frameHeight);
        flameDestinationRectangle = new Rectangle((int)flamePosition.X - frameWidth / 2, ((int)flamePosition.Y - frameHeight / 2) + yOffset, frameWidth, frameHeight);
    }


    public void Draw(SpriteBatch spriteBatch)
    {
        // Draw the rocket:
        spriteBatch.Draw(rocketSprite, RocketPosition, null, Color.White, 0.0f, rocketOrigin, 1.0f, SpriteEffects.None, 0);

        // Draw the rocket flames:
        spriteBatch.Draw(flameSprite, flameDestinationRectangle, flameSourceRectangle, Color.White);
    }


    /// <summary>
    /// Handles the rocket's movement based on user input. 
    /// </summary>
    /// <param name="inputManager"></param>
    public void HandleInput(InputManager inputManager, GameTime gameTime)
    {
        float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
        float levelSpeed = XInvaders.RocketSpeed * 60f;
        float slowSpeed = initialMovementSpeed + levelSpeed;
        float fastSpeed = acceleratedMovementSpeed + levelSpeed;

        // If you press the button for short time then the rocket is moving about +2 pixels.
        // If you are holding the button then the rocket is moving about +5 pixels.

        if (inputManager.KeyDown(Keys.Up))
        {
            timeUp += elapsed;
            RocketPosition.Y -= (timeUp < accelerationDelaySeconds ? slowSpeed : fastSpeed) * elapsed;
            if (RocketPosition.Y < 100)
                RocketPosition.Y = 100;
        }
        else
        {
            timeUp = 0;
        }


        if (inputManager.KeyDown(Keys.Down))
        {
            timeDown += elapsed;
            RocketPosition.Y += (timeDown < accelerationDelaySeconds ? slowSpeed : fastSpeed) * elapsed;
            if (RocketPosition.Y > (XInvaders.YRes - 100))
                RocketPosition.Y = XInvaders.YRes - 100;
        }
        else
        {
            timeDown = 0;
        }


        if (inputManager.KeyDown(Keys.Left))
        {
            timeLeft += elapsed;
            RocketPosition.X -= (timeLeft < accelerationDelaySeconds ? slowSpeed : fastSpeed) * elapsed;
            if (RocketPosition.X < 50)
                RocketPosition.X = 50;
        }
        else
        {
            timeLeft = 0;
        }



        if (inputManager.KeyDown(Keys.Right))
        {
            timeRight += elapsed;
            RocketPosition.X += (timeRight < accelerationDelaySeconds ? slowSpeed : fastSpeed) * elapsed;
            if (RocketPosition.X > (XInvaders.XRes - 50))
                RocketPosition.X = XInvaders.XRes - 50;
        }
        else
        {
            timeRight = 0;
        }

        // Fire event: raise when X key is pressed. Subscribers (e.g. BulletField) will react.
        if (inputManager.KeyPressed(Keys.X))
        {
            FirePressed?.Invoke(RocketPosition);
        }


        // Update the flame position to be just below the rocket's current position
        flamePosition.X = RocketPosition.X;
        flamePosition.Y = RocketPosition.Y + 33;    

        }


    /// <summary>
    /// Run this method when restarting the game
    /// </summary>
    public void Reset()
    {
        RocketPosition = new Vector2(XInvaders.XRes / 2, XInvaders.YRes - 100);
        flamePosition = RocketPosition + new Vector2(0, 33);
        flameAnimationAccumulator = 0;


        // Recalculate the cached flame rectangles right away, since rocket.Update()
        // is not called while GameState is 0 (intro) or 2 (game over), and Draw()
        // could otherwise use the stale rectangle from the position where the rocket died.
        flameDestinationRectangle = new Rectangle(
            (int)flamePosition.X - frameWidth / 2,
            ((int)flamePosition.Y - frameHeight / 2) + yOffset,
            frameWidth, frameHeight);
    }



}
