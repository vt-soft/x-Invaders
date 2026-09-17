using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace xInvaders;

/// <summary>
/// A class representing a field of stars on the screen. The StarField class manages a collection of Star objects, updating their positions and drawing them on the screen. Each star has a random position, speed, and tint color, creating a dynamic star field effect.
/// </summary>
internal class StarField
{
    private List<Star> stars = new List<Star>();
    private int screenWidth, screenHeight;
    private Color tintColor;
    private Vector2 starVelocity;
    private Texture2D starSprite;
    private Random rnd;


    /// <summary>
    /// Creates a star field with a specified number of stars. Each star has a random position, speed, and tint color. The stars are stored in a list and can be updated and drawn on the screen.
    /// </summary>
    /// <param name="Content">The ContentManager to load the star texture.</param>
    /// <param name="starCount">The number of stars to create in the star field.</param>
    public StarField(ContentManager Content, int starCount)
    {
 
        starSprite = Assets.Star;

        screenWidth = XInvaders.XRes;
        screenHeight = XInvaders.YRes;

        rnd = new Random(Guid.NewGuid().GetHashCode());

        for (int x = 0; x < starCount; x++)
        {
            starVelocity = new Vector2(0, 10) * rnd.Next(1, 4);             // Random speed
            tintColor = Color.White * (float)(rnd.Next(30, 80) / 100f);     // Random tint

            // Create one star and add it to the list of stars
            stars.Add(new Star(Content, starSprite, new Vector2(rnd.Next(0, screenWidth), rnd.Next(0, screenHeight)), starVelocity, tintColor));
        }

    }

    public void Update(GameTime gameTime)
    {
        foreach (Star star in stars)
        {
            star.Update(gameTime);

            // If Star is out of screen then change Y to 0 and X to random number
            if (star.Location.Y > screenHeight)
            {
                // Modify a local copy of the Vector2 and write it back to the property
                var loc = star.Location;
                loc.X = rnd.Next(0, screenWidth);
                loc.Y = 0;
                star.Location = loc;
            }
        }
    }


    public void Draw(SpriteBatch spriteBatch)
    {
        foreach (Star star in stars)
        {
            star.Draw(spriteBatch);
        }
    }

}
