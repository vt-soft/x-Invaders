using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Diagnostics;



namespace xInvaders;

/// <summary>
/// A class representing one Death Star's (Big Boss) Bullet on the screen. All bullets are stored in BigBossBulletField class.  
/// </summary>

internal class BigBossBullet
{

    /// <summary>
    /// Get the bounding box of the bullet for collision detection purposes.
    /// </summary>
    public Rectangle BoundingBox
    {
        get
        {
            // get the sprite's bounds
            Rectangle spriteBounds = bulletSprite.Bounds;
            // add the object's position to it as an offset
            spriteBounds.Offset(BulletPosition - bulletOrigin);
            return spriteBounds;
        }
    }
    public Vector2 BulletPosition;
    private Vector4 bulletVector;
    private Vector2 bulletOrigin;
    private Texture2D bulletSprite;

    private int dX, dY;
    private float rotation=0f;


    public BigBossBullet(ContentManager Content, Vector2 bigBossPosition, Vector4 bulletVector)
    {
        //bulletSprite = Content.Load<Texture2D>("_pictures/red_bullet");
        bulletSprite = Assets.GreyBullet;
        bulletOrigin = new Vector2(bulletSprite.Width / 2, bulletSprite.Height / 2);
        this.BulletPosition = bigBossPosition;
        this.bulletVector = bulletVector;

        dX = (int)(bulletVector.X - bulletVector.Z);
        dY = (int)(bulletVector.Y - bulletVector.W);
    }


    public void Update(GameTime gameTime)
    {
        float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

        BulletPosition.X -= dX * (elapsed * 1.2f);
        BulletPosition.Y -= dY * (elapsed * 1.2f);

        rotation += elapsed * 6.0f;
        if (rotation > MathHelper.Pi)
        {
            rotation = -MathHelper.Pi;
        }
    }



    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(bulletSprite, BulletPosition, null, Color.White, rotation, bulletOrigin, 1.0f, SpriteEffects.None, 0);
    }




 


}