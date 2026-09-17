using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace xInvaders;

/// <summary>
/// This class represents a UFO (Unidentified Flying Object) in the game. 
/// It handles the UFO's position, velocity, type, and rendering.
/// </summary>
internal class Ufo
{
    public Vector2 UfoPosition { get; private set; }
    public int UfoType { get; private set; }
    public int BigBossLives { get; set; } = 5;
    public Boolean UfoSinActivated { get; set; }

    /// <summary>
    /// Gets the bounding box of the UFO for collision detection purposes.
    /// </summary>
    public Rectangle BoundingBox
    {
        get
        {
            // Get the sprite's bounds
            Rectangle spriteBounds = ufoSprite.Bounds;
            // Add the object's position to it as an offset
            spriteBounds.Offset(UfoPosition - ufoOrigin);
            return spriteBounds;
        }
    }

    private Vector2 ufoOrigin;
    private Vector2 ufoOrigin_2part;
    private Texture2D ufoSprite;
    private Texture2D ufoSprite_2part;

    private Vector2 ufoVelocity;

    private float elapsed;
    private float angle; // for big boss rotation
    private double time;

    
    private Color bigBossColor;
    private int lastBigBossLivesForColor = -1; // Tracks the BigBossLives value bigBossColor was last computed for
    private float deathStarRotation;

    // for sinus movement:
    private float newX;
    private int sinRadius;
    private Random rnd;




    public Ufo(ContentManager content, int ufoType)
    {
        // Ufo type: 1 - Ufo, 2 - Grey Mine, 3 - Red mine, 4 - Big boss
        this.UfoType = ufoType;
        rnd = new Random(Guid.NewGuid().GetHashCode());
        sinRadius = rnd.Next(70, 370);
        UfoSinActivated = false;


        switch (UfoType)   // Select proper sprite based on the UFO type
        {
            case 1:     // Random Ufo
                UfoChangeColorAndVelocity(content);
                break;

            case 2:     // Grey Mine

                if (XInvaders.mineFromFile1 != null)
                    ufoSprite = XInvaders.mineFromFile1;
                else
                    ufoSprite = Assets.MineGrey;

                ufoVelocity = new Vector2(0, rnd.Next(30, 50)); ;
                break;

            case 3:     // Red Mine

                if (XInvaders.mineFromFile2 != null)
                    ufoSprite = XInvaders.mineFromFile2;
                else
                    ufoSprite = Assets.MineRed;

                ufoVelocity = new Vector2(0, rnd.Next(20, 40)); ;
                break;

            case 4:  // Death Star (originally big boss)


                if (XInvaders.deathStarFromFile != null)
                    ufoSprite = XInvaders.deathStarFromFile;
                else
                {
                    ufoSprite = Assets.BigBoss;
                    ufoSprite_2part = Assets.BigBoss2Part;
                    ufoOrigin_2part = new Vector2(ufoSprite_2part.Width / 2, ufoSprite_2part.Height / 2);
                    ufoOrigin_2part += new Vector2(1, 1);
                }

                break;
        }

        ufoOrigin = new Vector2(ufoSprite.Width / 2, ufoSprite.Height / 2);
    }


    /// <summary>
    /// This method updates the UFO's position based on its velocity and the elapsed game time.
    /// </summary>
    /// <param name="gameTime"></param>
    public void Update(GameTime gameTime)
    {
        // Calculating the sinus movement
        if (XInvaders.GameSinActivated && UfoSinActivated && UfoType == 1)
        {
            newX = (float)(Math.Sin(UfoPosition.Y / 60f) * sinRadius);
        }
        else
        {
            newX = 0;
        }

        elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

        // Avoid creating temporary Vector2 instances every update: modify a local copy and write back
        var pos = UfoPosition;
        pos.X += (newX + ufoVelocity.X) * elapsed;
        pos.Y += (ufoVelocity.Y + XInvaders.LevelSpeed) * elapsed;
        UfoPosition = pos;
       
        if (UfoType == 4) // Big boss part2 movement and color
        {
            time += gameTime.ElapsedGameTime.TotalMilliseconds / 500; ;
            angle = (float)Math.Sin(time);

            if (BigBossLives != lastBigBossLivesForColor)
            {
                bigBossColor = new Color((50 * (6 - BigBossLives)), (BigBossLives * 10) - 10, (BigBossLives * 10) - 10);
                lastBigBossLivesForColor = BigBossLives;
            }
        }
    }


    public void Draw(SpriteBatch spriteBatch)
    {

        if (UfoType == 4) // For big boss we have two sprites - the second one is rotating
        {
            if (XInvaders.deathStarFromFile == null) // No png from file - so we can use the second part
            {
                spriteBatch.Draw(ufoSprite_2part, UfoPosition, null, bigBossColor, (float)time, ufoOrigin_2part, 1.0f, SpriteEffects.None, 0);
                deathStarRotation = (float)-time;
            }
            else
                deathStarRotation = 0.0f;  // Png from file is not rotating and there is also not second part

            spriteBatch.Draw(ufoSprite, UfoPosition, null, Color.White, deathStarRotation, ufoOrigin, 1.0f, SpriteEffects.None, 0);
        }
        else
        {
            spriteBatch.Draw(ufoSprite, UfoPosition, null, Color.White, 0.0f, ufoOrigin, 1.0f, SpriteEffects.None, 0);
        }
    }



    /// <summary>
    /// This method will set random color and velocity for ufo (not for mine).
    /// </summary>
    /// <param name="Content"></param>
    public void UfoChangeColorAndVelocity(ContentManager Content)
    {
        switch (rnd.Next(1, 3 + 1))
        {
            case 1:
                if (XInvaders.ufoFromFile1 != null)
                    ufoSprite = XInvaders.ufoFromFile1;   // In case we have external files then we are using these external files.
                else
                    ufoSprite = Assets.UfoBlue;
                break;
            case 2:
                if (XInvaders.ufoFromFile2 != null)
                    ufoSprite = XInvaders.ufoFromFile2;
                else
                    ufoSprite = Assets.UfoGreen;
                break;
            case 3:
                if (XInvaders.ufoFromFile3 != null)
                    ufoSprite = XInvaders.ufoFromFile3;
                else
                    ufoSprite = Assets.UfoPurple;
                break;
        }


        ufoVelocity.X = 0;
        ufoVelocity.Y = rnd.Next(10, 70);

    }



    /// <summary>
    /// This method will set random velocity for mine.
    /// </summary>
    /// <param name="type"></param>
    public void MineChangeVelocity(int type)
    {
        if (type == 2)
        { 
            ufoVelocity.X = 0;
            ufoVelocity.Y = rnd.Next(30, 50);
        }
        else if (type == 3)
        {
            ufoVelocity.X = 0;
            ufoVelocity.Y = rnd.Next(20, 40);
        }
        else if (type == 4)
        {
            ufoVelocity.X = 0;
            ufoVelocity.Y = rnd.Next(10, 30);
        }
    }



    public void UfoNewPosition(Vector2 newPosition)
    {
        UfoPosition = newPosition;
    }




}
