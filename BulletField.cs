using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace xInvaders;

/// <summary>
/// A class representing LIST of bullets. 
/// This class is also listening to the rocket fire event and creates/reuses a bullet when the rocket fires.
/// </summary>
internal class BulletField
{

    // Here we store the bullets that are currently active in the game (i.e., bullets that have been fired and are still on the screen).
    public List<Bullet> activeBulletsList { get; private set; } = new List<Bullet>();

    // Here we store all bullets that have been created during the game.
    // The idea is to reuse bullets from this list instead of creating new ones every time, which can be more efficient.
    private Queue<Bullet> allBulletsList = new Queue<Bullet> (); 

    private ContentManager Content;
    private SoundEffect shootSound;
    private Bullet bullet;

 

    public BulletField(ContentManager Content)
    {
        this.Content = Content;
        shootSound = Assets.ShootSound;
    }


    public void Update(GameTime gameTime)
    {
        for (int i = activeBulletsList.Count - 1; i >= 0; i--)
        {
            activeBulletsList[i].Update(gameTime); // Recalculate position of each bullet

            // Move the bullet from the activeBulletsList to the allBulletsList queue if it goes off the top of the screen (Y < 30).
            if (activeBulletsList[i].BulletPosition.Y < 30)
            {
                MoveBulletToQueue(i);
            }
        }
    }



    public void Draw(SpriteBatch spriteBatch)
    {
        foreach (Bullet bullet in activeBulletsList)
        {
            bullet.Draw(spriteBatch);
        }
    }


    /// <summary>
    /// Moves a bullet from the activeBulletsList to the allBulletsList queue. 
    /// This is called when a bullet goes off the top of the screen or if it hits an enemy. 
    /// </summary>
    /// <param name="index">The index of the bullet to move.</param>
    public void MoveBulletToQueue(int index)
    {
        Bullet b = activeBulletsList[index];
        allBulletsList.Enqueue(b);
        activeBulletsList.RemoveAt(index);
    }



    /// <summary>
    /// Called when the rocket fires.
    /// Creates a new bullet at the rocket's posiont or reuses an existing bullet from the allBulletsList if available.
    /// </summary>
    public void OnRocketFire(Vector2 rocketPosition)
    {

        if (allBulletsList.Count > 0)
        {
            // Reuse a bullet from the allBulletsList
            bullet = allBulletsList.Dequeue();
            bullet.Reset(rocketPosition);
            activeBulletsList.Add(bullet);
        }
        else
        {
            // Create a new bullet if there are no bullets to reuse
            activeBulletsList.Add(new Bullet(Content, rocketPosition));
        }

        XInvaders.Score -= 1; // Each shot costs you one point 
        shootSound.Play(XInvaders.EffectsVolume, 0.0f, 0.0f);
    }




    /// <summary>
    /// Restart the game by clearing the list of bullets. This method is called when the game is reset.
    /// </summary>
    public void Reset()
    {
        activeBulletsList.Clear();
    }


}
