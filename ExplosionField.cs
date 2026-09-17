using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SharpDX.Direct2D1.Effects;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace xInvaders;

enum ExplosionType
{
    Big = 1,
    Small = 2,
    BigStatic = 3
}

internal class ExplosionField
{

    // Here we store the explosions that are currently active in the game
    // (i.e., explosions that have been triggered and are still visible on the screen).
    public List<Explosion> activeExplosionList { get; private set; } = new List<Explosion>(); // For both big and small explosions

    // Here we store all explosions that have been created during the game.
    // The idea is to reuse explosions from this list instead of creating new ones every time, which can be more efficient.
    private Queue<Explosion> allExplosionsListBig = new Queue<Explosion>();  // Queue for big explosions

    private Queue<Explosion> allExplosionsListSmall = new Queue<Explosion>(); // Queue for small explosions

    private Explosion explosion;


    private ContentManager Content;


    public ExplosionField(ContentManager Content)
    {
        this.Content = Content;
    }

    public void Update(GameTime gameTime)
    {
        for (int i = activeExplosionList.Count - 1; i >= 0; i--)
        {
            activeExplosionList[i].Update(gameTime); // Recalculate position of each explosion

            // Move the explosion from the activeExplosionList to the allExplosionsList queue if the animation is finished.
            if (activeExplosionList[i].AnimationFinished==true)
            {
                MoveExplosionToQueue(i);
            }
        }
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        foreach (Explosion  explosion in activeExplosionList)
        {
            explosion.Draw(spriteBatch);
        }
    }



    /// <summary>
    /// Adds an explosion to the activeExplosionList. 
    /// If there are any available explosions in the allExplosionsListBig or allExplosionsListSmall queue, it reuses one of them. 
    /// Otherwise, it creates a new explosion.
    /// </summary>
    /// <param name="content"></param>
    /// <param name="position"></param>
    /// <param name="type"></param>
    public void AddExplosion(ContentManager content, Vector2 position, ExplosionType type)
    {

        if (type == ExplosionType.Small)
        {
            if (allExplosionsListSmall.Count > 0)
            {
                // Reuse an existing small explosion from the queue
                explosion = allExplosionsListSmall.Dequeue();
                explosion.Reset(position, type);
                activeExplosionList.Add(explosion);
            }
            else
            {
                // Create a new small explosion if none are available in the queue
                activeExplosionList.Add(new Explosion(content, position, 1, 5, 5, type));
            }
        }
        else // Big or BigStatic
        {
            if (allExplosionsListBig.Count > 0)
            {
                // Reuse an existing big explosion from the queue
                explosion = allExplosionsListBig.Dequeue();
                explosion.Reset(position, type);
                activeExplosionList.Add(explosion);
            }
            else
            {
                // Create a new big explosion if none are available in the queue
                activeExplosionList.Add(new Explosion(content, position, 2, 4, 8, type));
            }
        }

    }



    /// <summary>
    /// Moves an explosion from the activeExplosionList to the allExplosionsListSmall or allExplosionsListBig queue.
    /// </summary>
    /// <param name="index">The index of the explosion to move.</param>
    public void MoveExplosionToQueue(int index)
    {
        Explosion b = activeExplosionList[index];
        if (b.Type==ExplosionType.Small) // Type 2 is small explosion
        {
            allExplosionsListSmall.Enqueue(b);
        }
        else // Type 1 and 3 are big explosions
        {
            allExplosionsListBig.Enqueue(b);
        }
        activeExplosionList.RemoveAt(index);
    }


    /// Run this method when restarting the game.
    /// </summary>
    public void Reset()
    {
        activeExplosionList.Clear();
    }

}
