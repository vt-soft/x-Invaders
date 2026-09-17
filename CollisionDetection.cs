using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace xInvaders;

internal class CollisionDetection
{

    // We are checking collisions between rocket and ufos/mines and also between bullets and ufos/mines


    Rocket rocket;
    UfoField ufoField;
    BulletField bulletField;
    ExplosionField explosionField;
    ContentManager content;
    SoundEffect deathStarComming;
    bool bBossSoundPlayed = false;
    SoundEffect redMineSound, greyMineSound, ufoSound, deathStarExplosion;

    // Accumulator for time-based collision checks (seconds). There is no need to run this method so often.
    private double collisionAccumulator = 0.0;
    private const double CollisionInterval = 1.0 / 20.0; // 12 times per second


    public CollisionDetection(ContentManager content,  Rocket rocket, UfoField ufoField, BulletField bulletField, ExplosionField explosionField) 
    { 
        this.content = content; 
        this.rocket = rocket;
        this.ufoField = ufoField;  
        this.bulletField = bulletField;
        this.explosionField = explosionField;

        redMineSound = Assets.RedMineSound;
        greyMineSound = Assets.GreyMineSound;
        deathStarExplosion = Assets.DeathStarExplosion;
        ufoSound = Assets.UfoSound;
        deathStarComming = Assets.DeathStarComming;
    }

    public void Update(GameTime gameTime)
    {
        // Accumulate elapsed time and run collision checks at a lower fixed frequency
        collisionAccumulator += gameTime.ElapsedGameTime.TotalSeconds;
        if (collisionAccumulator >= CollisionInterval)
        {
            // Subtract interval instead of resetting to handle cases where
            // elapsed time is larger than the interval
            collisionAccumulator -= CollisionInterval;

            RocketVsUfo();
            RocketVsBigBossBullet();
            BulletVsUfo();
            BigBoss();
        }
    }



  


    public void RocketVsBigBossBullet()
    {
        if (XInvaders.GameState == 1)
        {
            for (int i = XInvaders.bigBossBulletField.bigBossBullets.Count - 1; i >= 0; i--)
            {
                if (XInvaders.bigBossBulletField.bigBossBullets[i].BoundingBox.Intersects(rocket.BoundingBox))   // collision found
                {
                    explosionField.AddExplosion(content, rocket.RocketPosition, ExplosionType.Big);      // big explosion
                    XInvaders.GameState = 2; // game over
                    redMineSound.Play(XInvaders.EffectsVolume, 0.0f, 0.0f); // same sound as for redMine
                    break;
                }
            }
        }
    }

    /// <summary>
    /// This method is checking if rocket is in collision with ufo/mine
    /// </summary>
    public void RocketVsUfo()
    {
        if (XInvaders.GameState == 1)
        {
            for (int i = ufoField.ufos.Count - 1; i >= 0; i--)
            {
                if (ufoField.ufos[i].BoundingBox.Intersects(rocket.BoundingBox))   // collision found
                {
                    if (ufoField.ufos[i].UfoType == 1)  // rocket was hit by classic ufo
                    {
                        explosionField.AddExplosion(content, rocket.RocketPosition, ExplosionType.Big);         // big explosion  
                        explosionField.AddExplosion(content, ufoField.ufos[i].UfoPosition, ExplosionType.Small);  // small explosion 
                        ufoField.ufos.RemoveAt(i);
                    }
                    else if (ufoField.ufos[i].UfoType == 2) // rocket was hit by grey mine
                    {
                        explosionField.AddExplosion(content, rocket.RocketPosition, ExplosionType.Big);         // big explosion (only rocket)
                    }
                    else if (ufoField.ufos[i].UfoType == 3 || ufoField.ufos[i].UfoType == 4) // rocket was hit by red mine or big boss
                    {
                        explosionField.AddExplosion(content, rocket.RocketPosition, ExplosionType.Big);          // big explosion  
                        explosionField.AddExplosion(content, ufoField.ufos[i].UfoPosition, ExplosionType.Big);   // big explosion
                        ufoField.ufos.RemoveAt(i);     
                    }

                    XInvaders.GameState = 2; // game over
                    redMineSound.Play(XInvaders.EffectsVolume, 0.0f, 0.0f); // same sound as for redMine
                    break;
                }
            }
        }
    }


    /// <summary>
    /// This method is checking if bullet is in collision with ufo/mine
    /// </summary>
    public void BulletVsUfo()
    {
        for (int i = ufoField.ufos.Count - 1; i >= 0; i--)
        {
            for (int j = bulletField.activeBulletsList.Count - 1; j >= 0; j--)
            { 
                if (ufoField.ufos[i].BoundingBox.Intersects(bulletField.activeBulletsList[j].BoundingBox)) // collision found
                {
                    if (ufoField.ufos[i].UfoType == 1)  // bullet hit classic ufo
                    {
                        explosionField.AddExplosion(content, ufoField.ufos[i].UfoPosition, ExplosionType.Small);  // small explosion 
                        // this time we can't destroy the object - so we are sending ufo back to the top of the sky
                        ufoField.UfoCorridor(ufoField.ufos[i]);
                        // and changing its color and velocity
                        ufoField.ufos[i].UfoChangeColorAndVelocity(content);
                        // and moving the bullet which hit the ufo (from active bullets list to bullet's queue)
                        bulletField.MoveBulletToQueue(j);

                        XInvaders.Score += 5;
                        ufoSound.Play(XInvaders.EffectsVolume, 0.0f, 0.0f);
                    }
                    else if (ufoField.ufos[i].UfoType == 2) // bullet hit grey mine
                    {
                        // you can`t destroy grey mine with bullet - so just moving the bullet here (from active bullets list to bullet's queue)
                        bulletField.MoveBulletToQueue(j);
                        greyMineSound.Play(XInvaders.EffectsVolume, 0.0f, 0.0f);
                    }
                    else if (ufoField.ufos[i].UfoType == 3) // bullet hit red mine
                    {
                        redMineSound.Play(XInvaders.EffectsVolume, 0.0f, 0.0f);
                        Vector2 redMinePosition = ufoField.ufos[i].UfoPosition;
                        explosionField.AddExplosion(content, ufoField.ufos[i].UfoPosition, ExplosionType.Big);          // big explosion  
                        // this time we can't destroy the object - we are sending ufo back to sky
                        ufoField.UfoCorridor(ufoField.ufos[i]);
                        // and changing its velocity
                        ufoField.ufos[i].MineChangeVelocity(3);
                        // and moving the bullet which hit the ufo (from active bullets list to bullet's queue)
                        bulletField.MoveBulletToQueue(j);
                        XInvaders.Score += 10;
                        RedMineVsEverything(redMinePosition);  
                    }

                    else if (ufoField.ufos[i].UfoType == 4) // bullet hit big boss
                    {
                        ufoField.ufos[i].BigBossLives--;

                        if (ufoField.ufos[i].BigBossLives >= 1)
                        {
                            bulletField.MoveBulletToQueue(j);
                            greyMineSound.Play(XInvaders.EffectsVolume, 0.0f, 0.0f);
                        }
                        else  // this is exactly the same part as for above (for red mine) - except the first line and also you will get 15 points
                        {
                            ufoField.ufos[i].BigBossLives = 5;
                            deathStarExplosion.Play(XInvaders.EffectsVolume*2.5f, 0.0f, 0.0f);
                            Vector2 redMinePosition = ufoField.ufos[i].UfoPosition;
                            explosionField.AddExplosion(content, ufoField.ufos[i].UfoPosition, ExplosionType.Big);          // big explosion  
                            // this time we can't destroy the object - we are sending ufo back to sky
                            ufoField.UfoCorridor(ufoField.ufos[i]);
                            // and changing its velocity
                            ufoField.ufos[i].MineChangeVelocity(3);
                            // and moving the bullet which hit the ufo (from active bullets list to bullet's queue)
                            bulletField.MoveBulletToQueue(j);
                            XInvaders.Score += 15;
                            RedMineVsEverything(redMinePosition);
                        }

                    }

                }
            }
        }
    }


    /// <summary>
    /// Explosion of red mine will kill everything in its nearby. Except big boss.
    /// </summary>
    /// <param name="redMinePosition"></param>
    public void RedMineVsEverything(Vector2 redMinePosition)
    {
        int dRange = 200; //  deadly range radius
        int cRange;       // current range radius
        int a, b;

        // checking if explosion of red mine hit ufo, grey mine or another red mine 
        for (int i = ufoField.ufos.Count - 1; i >= 0; i--)
        {
            if (ufoField.ufos[i].UfoType == 4) // ignoring big boss
                continue;

            a = (int)ufoField.ufos[i].UfoPosition.X - (int)redMinePosition.X;
            b = (int)ufoField.ufos[i].UfoPosition.Y - (int)redMinePosition.Y;
            cRange = (int)Math.Sqrt(a * a + b * b);

            if (cRange < dRange) // ufo/mine is in deadly radius of red mine explosion
            {
                if (ufoField.ufos[i].UfoType != 3) // red mine hit ufo or grey mine
                {
                    ufoSound.Play(XInvaders.EffectsVolume, 0.0f, 0.0f);
                    explosionField.AddExplosion(content, ufoField.ufos[i].UfoPosition, ExplosionType.Small);          // small explosion
                    ufoField.UfoCorridor(ufoField.ufos[i]);

                    if (ufoField.ufos[i].UfoType == 1) // ufo 
                    {
                        ufoField.ufos[i].UfoChangeColorAndVelocity(content);
                    }
                    else  // grey mine
                    {
                        ufoField.ufos[i].MineChangeVelocity(2);
                    }                        
                }
                else if (ufoField.ufos[i].UfoType == 3) // red mine killed another red mine
                {
                    redMineSound.Play(XInvaders.EffectsVolume, 0.0f, 0.0f);
                    Vector2 anotherRedMinePosition = ufoField.ufos[i].UfoPosition;
                    explosionField.AddExplosion(content, ufoField.ufos[i].UfoPosition, ExplosionType.Big);          // big explosion 
                    ufoField.UfoCorridor(ufoField.ufos[i]);
                    ufoField.ufos[i].MineChangeVelocity(3);
                    RedMineVsEverything(anotherRedMinePosition); // recursive calling of the same function
                }
            }
        }
        // and finally checking if explosion of red mine hit the rocket
        a = (int)rocket.RocketPosition.X - (int)redMinePosition.X;
        b = (int)rocket.RocketPosition.Y - (int)redMinePosition.Y;
        cRange = (int)Math.Sqrt(a * a + b * b);

        if (cRange < (dRange-10)) // rocket is in deadly radius of red mine explosion
        {
            explosionField.AddExplosion(content, rocket.RocketPosition, ExplosionType.Big);         // big explosion
            XInvaders.GameState = 2; // game over
        }
    }

    // big boss ground explosion + sound when big boss is comming into the scene
    public void BigBoss()
    {
        for (int i = ufoField.ufos.Count - 1; i >= 0; i--)
        {
            if (ufoField.ufos[i].UfoType == 4 ) // big boss detected
            {
                if (ufoField.ufos[i].UfoPosition.Y + 150 > XInvaders.YRes) // big boss hit the ground
                {
                    explosionField.AddExplosion(content, ufoField.ufos[i].UfoPosition - new Vector2(0,20), ExplosionType.BigStatic);   // big static explosion
                    XInvaders.Score -= 10;
                    XInvaders.bigBossBulletField.Activate(ufoField.ufos[i].UfoPosition);
                    ufoField.ufos[i].BigBossLives = 5;
                    ufoField.UfoCorridor(ufoField.ufos[i]);
                    ufoField.ufos[i].MineChangeVelocity(4);
                    deathStarExplosion.Play(XInvaders.EffectsVolume*2.5f, 0.0f, 0.0f);
                }

                // big boss is comming - so let's play the sound
                if ((ufoField.ufos[i].UfoPosition.Y + 150 > 0 && ufoField.ufos[i].UfoPosition.Y + 150 < 50)
                     && !bBossSoundPlayed) 
                {
                    deathStarComming.Play(XInvaders.EffectsVolume*1.4f, 0.0f, 0.0f);
                    bBossSoundPlayed = true;
                }

                if (ufoField.ufos[i].UfoPosition.Y + 150 >= 50 && bBossSoundPlayed)
                {
                    bBossSoundPlayed = false;
                }
                break;
            }
        }
    }




    /// <summary>




}
