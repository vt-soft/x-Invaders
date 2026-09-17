using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace xInvaders;
/// <summary>
/// This class represents a clickable  button (on start screen) that opens a specified URL when clicked.
/// </summary>
internal class WebButton
{

    private int x, y;
    private SpriteFont font;
    private string urlLink, urlText;
    private Rectangle rect;
    private Texture2D texture;
    private Color rectColor;

    /// <summary>
    /// Initializes a new instance of the WebButton class.
    /// </summary>
    /// <param name="Content">The ContentManager to load the button texture.</param>
    /// <param name="x">The x-coordinate of the button.</param>
    /// <param name="y">The y-coordinate of the button.</param>
    /// <param name="font">The font used for the button text.</param>
    /// <param name="urlLink">The URL link that the button will open when clicked.</param>
    /// <param name="urlText">The text displayed on the button.</param>
    public WebButton(ContentManager Content, int x, int y, SpriteFont font, string urlLink, string urlText)
    {
        this.x = x;
        this.y = y;
        this.font = font;
        this.urlLink = urlLink;
        this.urlText = urlText;

        Vector2 size = font.MeasureString(urlText);

        rect.X = x - 15;
        rect.Y = y - 5;
        rect.Width = (int)size.X + 30;
        rect.Height = (int)size.Y + 10;

        texture = Assets.Star;
        rectColor = Color.LightGray;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(texture, rect, rectColor);
        spriteBatch.DrawString(font, urlText, new Vector2(x, y), Color.Black);
    }

    public void HandleInput(InputManager inputManager)
    {

        Vector2 mousePos = inputManager.MousePosition;
        Rectangle mouseRec = new((int)mousePos.X, (int)mousePos.Y, 1, 1);

        // MouseOver
        if (rect.Intersects(mouseRec))
        {
            rectColor = Color.WhiteSmoke;
        }
        else
        {
            rectColor = Color.LightGray;
        }


        // MouseOver and click the button
        if ((rect.Intersects(mouseRec) && inputManager.MouseLeftButtonPressed()))
        {
            try
            {
                System.Diagnostics.ProcessStartInfo webPage = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = urlLink,
                    UseShellExecute = true

                };
                System.Diagnostics.Process.Start(webPage);
            }
            catch { }
        }
    }


}
