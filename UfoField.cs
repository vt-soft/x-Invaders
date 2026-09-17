using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace xInvaders;

internal class UfoField
{
    public List<Ufo> ufos { get; private set; } = new List<Ufo>(); // Standard list where we are storing all ufos/mines.

    // There are six corridors for ufos/mines - we are storing only the last ufo/mine in such corridor
    private Ufo[] ufoCorridors = new Ufo[6];

    private ContentManager Content;
    private Random rnd;


    public UfoField(ContentManager Content)
    {
        this.Content = Content;
        rnd = new Random(Guid.NewGuid().GetHashCode());

    }


    /// <summary>
    /// This method will go thru all ufo-list and will call ufo.Update() method for each ufo/mine in such list .
    /// </summary>
    /// <param name="gameTime"></param>
    public void Update(GameTime gameTime)
    {
        foreach (Ufo ufo in ufos)
        {
            ufo.Update(gameTime); // Updating position of each ufo/mine

            // If ufo/mine hit the ground then set new corridor, velocity, color for it
            if (ufo.UfoPosition.Y > XInvaders.YRes && ufo.UfoType != 4)
            {
                if (ufo.UfoType != 2)  //  Decreasing the score if ufo or red mine  hit the ground
                    XInvaders.Score -= 10;

                UfoCorridor(ufo); // We have to set proper corridor for new Ufo

                if (ufo.UfoType == 1) 
                    ufo.UfoChangeColorAndVelocity(Content);

                if (ufo.UfoType != 1)
                    ufo.MineChangeVelocity(ufo.UfoType);
            }


        }
    }


    /// <summary>
    /// This method will go thru all ufo-list and will call ufo.Draw() method for each ufo/mine in such list .
    /// </summary>
    /// <param name="spriteBatch"></param>
    public void Draw(SpriteBatch spriteBatch)
    {
        foreach (Ufo ufo in ufos)
        {
            ufo.Draw(spriteBatch);
        }
    }



    /// <summary>
    /// This method will create new ufos/mines and will add them to the ufo list. 
    /// </summary>
    /// <param name="ufoType"></param>
    public void UfoAdd(int ufoType, int number) // Ufo type: 1 - Ufo, 2 - Grey Mine, 3 - Red mine
    {

        for (int i = 1; i <= number; i++)
        {
            Ufo ufo;
            ufo = new Ufo(Content, ufoType);
            ufos.Add(ufo);
            UfoCorridor(ufo);     // We have to set proper corridor for new Ufo
        }
    }



    /// <summary>
    /// This method will add ufo/mine to correct corridor. 
    /// The "corridor" is just some "attempt" how to organize the ufos/mines on the screen nicely.
    /// We have to call this method each time we create new ufo/mine or if ufo/mine leaves the screen or is shot down.
    /// </summary>
    /// <param name="ufo"></param>
    public void UfoCorridor(Ufo ufo)

    {
        int i; // corridor index

        // Ufo is maybe still in the corriddor (especially at the beginning of the game where only few ufos/mines are on the screen)
        // so we have to delete it first - fortunatelly it is the one who is calling this method
        for (int j = 0; j < ufoCorridors.Length; j++)
        {
            if (ufoCorridors[j] == ufo)
            {
                ufoCorridors[j] = null;
            }
        }

        int a = rnd.Next(0, 5 + 1);   // select new random coridor (X-axis)
        int b = rnd.Next(0, 5 + 1);   // select new random coridor (X-axis)

        if ((ufoCorridors[a] != null) && (ufoCorridors[b] != null))
        {
            if (ufoCorridors[a].UfoPosition.Y >= ufoCorridors[b].UfoPosition.Y)
            {
                i = a;
            }
            else
            {
                i = b;
            }
        }
        else if ((ufoCorridors[a] == null) && (ufoCorridors[b] != null))
        {
            i = a;
        }
        else
        {
            i = b;
        }


        int offset = rnd.Next(0, 20); // little random offset (X-axis) within the corridor
        int lastUfoYpos;

        if (ufoCorridors[i] == null)
        {
            lastUfoYpos = 100;        // some fake number if no Ufo is in current corridor yet
        }
        else
        {
            lastUfoYpos = (int)ufoCorridors[i].UfoPosition.Y;
        }

        int y = lastUfoYpos - rnd.Next(200, 400);  // new Ufo Y-coordinate is 200-400 pixels  above the last Ufo


        if (ufo.UfoType == 4) // big boss is not appearing so often
            y = y - (1000 + rnd.Next(500, 1500));
        if (ufo.UfoType == 4 && i == 0) // big boss is will also not appear at the edge of the screen
            i = 1;
        if (ufo.UfoType == 4 && i == 5)
            i = 4;


        if (y > 0)   // this is to prevent that new Ufo will appear in the middle of screen
            y = -50;

        // 70 boundary
        // 90 width of each coridor. 6 corridors

        int x = 70 + (90 * i) + offset;

        ufo.UfoNewPosition(new Vector2(x, y));

        ufoCorridors[i] = ufo;  // add Ufo to corridor (this will replace the old ufo here)


        // We are also activating/deactivating the sinus movement for some random ufos here
        switch (rnd.Next(1, 11))
        {
            case 1:
            case 2:
            case 3:
            case 4:
                ufo.UfoSinActivated = true;
                break;
            default:
                ufo.UfoSinActivated = false;
                break;
        }


    }




    /// <summary>
    /// Run this method when restarting the game.
    /// </summary>
    public void Reset()
    {
        ufos.Clear();
        Array.Clear(ufoCorridors);

    }
}
