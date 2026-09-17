using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace xInvaders;



/// <summary>
/// Slider on the intro screen to adjust music and effects volume
/// </summary>
internal class Slider
{
    private float minValue, maxValue, previousValue, currentValue;
    private Vector2 barOrigin, barPosition;
    private Vector2 buttonOrigin, buttonPosition;
    private readonly int minX, maxX; // SliderButton screen position values
    private int newX;
    private Texture2D sliderBar, sliderButton;


    public bool ValueChanged
    {
        get { return currentValue != previousValue; }
    }

    /// <summary>
    /// Gets the bounding box of the slider button, which is used for detecting mouse interactions.
    /// </summary>
    public Rectangle BoundingBoxButton
    {
        get
        {
            // Get the sprite's bounds
            Rectangle spriteBounds = sliderButton.Bounds;
            // And increasing them a bit
            spriteBounds = new Rectangle(spriteBounds.X - 30, spriteBounds.Y, spriteBounds.Width + 60, spriteBounds.Height);
            // Add the object's position to it as an offset
            spriteBounds.Offset(buttonPosition - buttonOrigin);
            return spriteBounds;
        }
    }

    /// <summary>
    /// Gets the bounding box of the slider bar, which is used for detecting mouse interactions.
    /// </summary>
    public Rectangle BoundingBoxBar
    {
        get
        {
            // Get the sprite's bounds
            Rectangle spriteBounds = sliderBar.Bounds;
            // And increasing them a bit
            spriteBounds = new Rectangle(spriteBounds.X, spriteBounds.Y - 10, spriteBounds.Width, spriteBounds.Height + 20);
            // Add the object's position to it as an offset
            spriteBounds.Offset(barPosition - barOrigin);
            return spriteBounds;
        }
    }

    /// <summary>
    /// Initializes a new instance of the Slider class.
    /// </summary>
    /// <param name="Content">The ContentManager to load textures.</param>
    /// <param name="x">The x-coordinate of the slider's position.</param>
    /// <param name="y">The y-coordinate of the slider's position.</param>
    /// <param name="minValue">The minimum value of the slider.</param>
    /// <param name="maxValue">The maximum value of the slider.</param>
    /// <param name="currentValue">The current value of the slider.</param>
    public Slider(ContentManager Content, int x, int y, float minValue, float maxValue, float currentValue)
    {

        this.minValue = minValue;
        this.maxValue = maxValue;
        this.currentValue = this.previousValue = currentValue; ;

        barPosition = new Vector2(x, y);

        sliderBar = Assets.SliderBar;
        sliderButton = Assets.SliderButton;

        buttonOrigin = new Vector2(sliderButton.Width / 2, sliderButton.Height / 2);
        barOrigin = new Vector2(sliderBar.Width / 2, sliderBar.Height / 2);

        // SliderButton min and max position values:
        minX = (int)(barPosition.X - (sliderBar.Width / 2) + (sliderButton.Width / 2)); 
        maxX = (int)(barPosition.X + (sliderBar.Width / 2) - (sliderButton.Width / 2));

        CurrentValueToNewX();
    }

    public void HandleInput(InputManager inputManager)
    {
        previousValue = currentValue;
        Vector2 mousePos = inputManager.MousePosition;

        // If you click (or hold the mouse) on the bar or the slider button
        if ((inputManager.MouseLeftButtonDown() || inputManager.MouseLeftButtonPressed())
            &&
            (BoundingBoxBar.Contains(mousePos) || BoundingBoxButton.Contains(mousePos)))
        {
            MouseToCurrentValue(mousePos);
            CurrentValueToNewX();
        }
    }

    /// <summary>
    /// Transforms the mouse position to the current value of the slider, which is between minValue and maxValue.
    /// </summary>
    /// <param name="mousePos">The position of the mouse.</param>
    private void MouseToCurrentValue(Vector2 mousePos)
    {
        // Transform the mouse position to currentValue (which is between minValue and maxValue):
        float correctedX = mousePos.X - minX;

        if (correctedX <= 0)
            correctedX = 0;

        if (correctedX >= (maxX - minX))
            correctedX = (maxX - minX);

        float newFraction = correctedX / (maxX - minX);

        // Convert that to a new slider value
        currentValue = minValue + newFraction * (maxValue - minValue);
    }

    /// <summary>
    /// Transforms the current value of the slider to the new X position of the slider button.
    /// </summary>
    private void CurrentValueToNewX()
    {
        // Will transform currentValue to newX (new X possition of the slide button) 
        float fraction = (currentValue - minValue) / (maxValue - minValue);
        newX = minX + (int)(fraction * (maxX - minX));
        buttonPosition = new Vector2(newX, (int)barPosition.Y);
    }

    public float GetValue()
    {
        return currentValue;
    }


    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(sliderBar, barPosition, null, Color.White, 0.0f, barOrigin, 1.00f, SpriteEffects.None, 0);
        spriteBatch.Draw(sliderButton, buttonPosition, null, Color.White, 0.0f, buttonOrigin, 1.0f, SpriteEffects.None, 0);
    }





}
