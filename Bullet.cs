using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace xInvaders;

/// <summary>
/// This class represents a bullet fired by the rocket in the game. It handles the bullet's position, velocity, and rendering.
/// All bullets are stored in BulletField class.  
/// </summary>
internal class Bullet
{
    public Vector2 BulletPosition { get; set; }
    private Vector2 bulletOrigin;
    private Texture2D bulletSprite;
    private Vector2 bulletVelocity;
    private float elapsed; // Time elapsed since the last update, used for movement calculations


    /// <summary>
    /// Gets the bounding box of the bullet for collision detection purposes.
    /// </summary>
    public Rectangle BoundingBox
    {
        get
        {
            // Get the sprite's bounds
            Rectangle spriteBounds = bulletSprite.Bounds;
            // Add the object's position to it as an offset
            spriteBounds.Offset(BulletPosition - bulletOrigin);
            return spriteBounds;
        }
    }

    public Bullet(ContentManager Content, Vector2 rocketPosition)
    {
        bulletSprite = Assets.Bullet;
        bulletOrigin = new Vector2(bulletSprite.Width / 2, bulletSprite.Height / 2);
        BulletPosition = rocketPosition - new Vector2(0, 40);   // Adjusting bullet position a bit
        bulletVelocity = new Vector2(0, 330);
    }


    public void Update(GameTime gameTime)
    {
        elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
        BulletPosition -= bulletVelocity * elapsed;
    }


    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(bulletSprite, BulletPosition, null, Color.White, 0.0f, bulletOrigin, 1.0f, SpriteEffects.None, 0);
    }


    /// <summary>
    /// Resets the bullet's position to the rocket's position. 
    /// This is used when reusing bullets from the BulletField's allBulletsList.
    /// </summary>
    /// <param name="rocketPosition"></param>
    public void Reset(Vector2 rocketPosition)
    {
        // Set the whole Vector2 (Vector2 is a value type, so this does not allocate on the heap)
        BulletPosition = new Vector2(rocketPosition.X, rocketPosition.Y - 40f);
    } 


}
