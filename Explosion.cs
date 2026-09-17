using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;

namespace xInvaders;

/// <summary>
/// This class is used to create an explosion animation when a UFO, mine or death star  is destroyed.
/// </summary>
internal class Explosion
{
    public Boolean AnimationFinished { get; private set; }
    private Texture2D sprite;

    private int counter;
    public ExplosionType Type { get; private set; }
    private Vector2 explosionPosition;
  

    private int ssNumberOfFrames; // Total number of frames in srpite sheet for rocket flames animation.
    private int ssNumberOfRows; // Total number of rows in sprite sheet for rocket flames animation.
    private int ssNumberOfColumns; // Total number of columns in sprite sheet for rocket flames animation.


    private int frameIndex;
    private int frameWidth;
    private int frameHeight;
    private int currentFrameXPosition;  // X position of the current frame in the sprite sheet.
    private int currentFrameYPosition;  // Y position of the current frame in the sprite sheet.

    // Cached rectangles, recalculated only when the animation frame/position actually changes (in Update),
    // instead of being reconstructed on every Draw call.
    private Rectangle sourceRectangle;
    private Rectangle destinationRectangle;

    private const int animationSpeed = 11; // Speed of the explosion animation. The higher the number, the slower the animation.

    private const int yExplosionOffset =0;  // 20 

    public Explosion(ContentManager content, Vector2 position, int rows, int columns, int totalFrames, ExplosionType type)
    {
        this.explosionPosition = position;
        this.explosionPosition.Y += 30;
        this.Type = type;    

        if (Type == ExplosionType.Big || Type == ExplosionType.BigStatic)
        {
            sprite = Assets.ExplosionBig;
        }
        else
        {
            this.explosionPosition.Y -= yExplosionOffset;
            sprite = Assets.ExplosionSmall;
        }

        this.ssNumberOfRows = rows;
        this.ssNumberOfColumns = columns;
        this.ssNumberOfFrames = totalFrames;

        frameIndex = 0;
        counter = 0;
        AnimationFinished = false;

        frameWidth = sprite.Width / ssNumberOfColumns;
        frameHeight = sprite.Height / ssNumberOfRows;


        sourceRectangle = new Rectangle(currentFrameXPosition, currentFrameYPosition, frameWidth, frameHeight);
        destinationRectangle = new Rectangle((int)this.explosionPosition.X - frameWidth / 2, (int)this.explosionPosition.Y - frameHeight / 2, frameWidth, frameHeight);
    }


    public void Update(GameTime gameTime)
    {
        // This code will slow down the animation:

        if (!AnimationFinished)
        {

            counter++;
            if (counter == animationSpeed)  // Speed of explosion animation
            {
                counter = 0;
                frameIndex++;
                
                if (Type != ExplosionType.BigStatic)
                    explosionPosition.Y -= 30;  // Moving the explosion sprite up during the animation

                if (frameIndex == ssNumberOfFrames)
                    AnimationFinished = true;

                // Calculate the current frame's position in the sprite sheet
                currentFrameXPosition = (frameIndex % ssNumberOfColumns) * frameWidth;
                currentFrameYPosition = (frameIndex / ssNumberOfColumns) * frameHeight;

                // Recalculate cached rectangles here, only when the frame/position actually changes,
                // instead of doing it on every Draw call.
                sourceRectangle = new Rectangle(currentFrameXPosition, currentFrameYPosition, frameWidth, frameHeight);
                destinationRectangle = new Rectangle((int)explosionPosition.X - frameWidth / 2, (int)explosionPosition.Y - frameHeight / 2, frameWidth, frameHeight);
            }
        }
    }


    public void Draw(SpriteBatch spriteBatch)
    {
        if (!AnimationFinished)
        {
            spriteBatch.Draw(sprite, destinationRectangle, sourceRectangle, Color.White);
        }
    }


    /// <summary>
    /// Resets the explosion's position to the new position. 
    /// This is used when reusing explosions from the ExplosionField's allExplosionsList (big or small).
    /// </summary>
    /// <param name="position"></param>
    /// <param name="type"></param>
    public void Reset(Vector2 position, ExplosionType type)
    {
      
        AnimationFinished = false;
        frameIndex = 0;
        counter = 0;
        Type = type;

        this.explosionPosition = position;
        this.explosionPosition.Y += 30;

        if (type == ExplosionType.Small)
        {
            this.explosionPosition.Y -= yExplosionOffset;
        }


        // Recalculate cached rectangles immediately using the NEW position/frame,
        // otherwise Draw() will use the stale rectangle from this object's previous use
        // for up to `animationSpeed` frames, potentially rendering it at the wrong (or off-screen) location.
        sourceRectangle = new Rectangle(currentFrameXPosition, currentFrameYPosition, frameWidth, frameHeight);
        destinationRectangle = new Rectangle(
            (int)explosionPosition.X - frameWidth / 2,
            (int)explosionPosition.Y - frameHeight / 2,
            frameWidth, frameHeight);


    }



}
