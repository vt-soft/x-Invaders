using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;

namespace xInvaders;

/// <summary>
/// A class representing LIST of bullets from Death Star (Big Boss). 
/// </summary>
class BigBossBulletField
{

    public List<BigBossBullet> bigBossBullets { get; private set; } = new List<BigBossBullet>();
    private ContentManager Content;


    public BigBossBulletField(ContentManager Content)
    {
        this.Content = Content;
    }


    /// <summary>
    /// Run this method when Big Boss reached the ground. It creates 16 bullets in a circle around Big Boss.
    /// </summary>
    /// <param name="bigBossPosition">The position of the Big Boss.</param>
    public void Activate(Vector2 bigBossPosition)
    {
        int x, y, x1, x2, y1, y2;
        int r = 50;
        Vector2 bulletPosition;

        for (float alfa = 360; alfa >= 0; alfa -= 22.5f)
        {
            x = (int)((Math.Cos(Math.PI / 180 * alfa)) * r);
            y = (int)((Math.Sin(Math.PI / 180 * alfa)) * r);

            x1 = (int)bigBossPosition.X;
            y1 = (int)bigBossPosition.Y;
            x2 = (int)bigBossPosition.X + x;
            y2 = (int)bigBossPosition.Y + y;

            bulletPosition.X = bigBossPosition.X + x;
            bulletPosition.Y = bigBossPosition.Y + y;

            bigBossBullets.Add(new BigBossBullet(Content, bulletPosition, new Vector4(x1, y1, x2, y2)));
        }

    }


    // Recalculate position and remove bullet if bullet is out of screen
    public void Update(GameTime gameTime)
    {
        for (int i = bigBossBullets.Count - 1; i >= 0; i--)
        {
            bigBossBullets[i].Update(gameTime); // Recalculate position of each bullet

            // Remove the bullet from the list if bullet leaves the screen
            if (bigBossBullets[i].BulletPosition.Y < -10
                || bigBossBullets[i].BulletPosition.X < -10
                || bigBossBullets[i].BulletPosition.X > XInvaders.XRes + 10
                || bigBossBullets[i].BulletPosition.Y > XInvaders.YRes + 10)
            {
                bigBossBullets.RemoveAt(i);
            }
        }
    }



    public void Draw(SpriteBatch spriteBatch)
    {
        foreach (BigBossBullet bullet in bigBossBullets)
        {
            bullet.Draw(spriteBatch);
        }
    }


    /// <summary>
    /// Run this method when restarting the game.
    /// </summary>
    public void Reset()
    {
        bigBossBullets.Clear();
    }



}