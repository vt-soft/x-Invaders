using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace xInvaders;


/// <summary>
/// This class is used to manage the input devices (mouse and keyboard) for the game.
/// </summary>
internal class InputManager
{
    
    private MouseState currentMouseState, previousMouseState;           // Current and previous mouse state
    private KeyboardState currentKeyboardState, previousKeyboardState;  // Current and previous keyboard state



    /// <summary>
    /// Gets the current position of the mouse, relative to the top-left corner of the screen.
    /// </summary>
    public Vector2 MousePosition
    {
        get { return new Vector2(currentMouseState.X, currentMouseState.Y); }
    }

    /// <summary>
    /// Updates InputManager object for one frame of the game loop.
    /// This method retrieves the current mouse and keyboard state, and stores the previous states as a backup.
    /// </summary>
    public void Update()
    {
        previousMouseState = currentMouseState;
        previousKeyboardState = currentKeyboardState;
        currentMouseState = Mouse.GetState();
        currentKeyboardState = Keyboard.GetState();
    }
    

    /// <summary>
    /// Checks and returns whether the player has started pressing the left mouse button in the last frame of the game loop.
    /// </summary>
    /// <returns>true if the left mouse button is now pressed and was not yet pressed in the previous frame; false otherwise.</returns>
    public bool MouseLeftButtonPressed()
    {
        return currentMouseState.LeftButton == ButtonState.Pressed && previousMouseState.LeftButton == ButtonState.Released;
    }


    /// <summary>
    /// Checks and returns whether the left mouse button is currently being held down.
    /// </summary>
    /// <returns>true if the left mouse button is currently being held down; false otherwise.</returns>
    public bool MouseLeftButtonDown()
    {
        return currentMouseState.LeftButton == ButtonState.Pressed;
    }



    /// <summary>
    /// Checks and returns whether the player has started pressing a certain keyboard key in the last frame of the game loop.
    /// </summary>
    /// <param name="k">The key to check.</param>
    /// <returns>true if the given key is now pressed and was not yet pressed in the previous frame; false otherwise.</returns>
    public bool KeyPressed(Keys k)
    {
        return currentKeyboardState.IsKeyDown(k) && previousKeyboardState.IsKeyUp(k);
    }


    /// <summary>
    /// Checks and returns whether a certain keyboard key is currently being held down.
    /// </summary>
    /// <param name="k">The key to check.</param>
    /// <returns>true if the given key is currently being held down; false otherwise.</returns>
    public bool KeyDown(Keys k)
    {
        return currentKeyboardState.IsKeyDown(k);

    }

}

