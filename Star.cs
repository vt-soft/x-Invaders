using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace xInvaders;


/// <summary>
/// A class representing one tiny (white) star on the screen. All stars are stored in StarField class.  
/// </summary>
internal class Star
{
    public Vector2 Location { get; set; }
    private Texture2D texture;
    private Color tintColor;
    private Vector2 velocity;
    private Rectangle initialFrame;


    /// <summary>
    /// Creates a new instance of the Star class with the specified parameters. The star is initialized with a location, texture, velocity, and tint color. The initial frame of the star is set to a 2x2 rectangle.    
    /// </summary>
    /// <param name="Content"></param>
    /// <param name="texture"></param>
    /// <param name="location"></param>
    /// <param name="velocity"></param>
    /// <param name="tintColor"></param>
    public Star(ContentManager Content, Texture2D texture, Vector2 location, Vector2 velocity, Color tintColor)
    {
        Location = location;
        this.texture = texture;
        this.velocity = velocity;
        this.tintColor = tintColor;
        initialFrame = new Rectangle(0, 0, 2, 2);
    }


    public void Update(GameTime gameTime)
    {
        float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
        Location += velocity * elapsed;
    }


    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(texture, Location, initialFrame, tintColor);
    }


}
