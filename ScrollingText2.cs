using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace xInvaders;

/// <summary>
/// This class is displaying scrolling credits.txt file on the Game Over screen.
/// You have to adjust width of the strings in txt file yourself manually !!!
/// </summary>
internal class ScrollingText2
{

    private int x, y, numberOfLines;
    private SpriteFont font;

    private bool isDoneDrawing, isDoneDrawingLine;
    private int delayInMilliseconds;
    private double time;

    private String typedText;
    private String lastRow;
    private String updatedLastRow;

    private int i, j, p;
    private List<String> gcData = new List<String>();

    public ScrollingText2(int x, int y, int numberOfLines, SpriteFont font, List<string> gcData)
    {
        this.x = x;                             // start position of the scrolling text
        this.y = y;
        this.numberOfLines = numberOfLines;     // number of lines which are scrolling on the screen
        this.font = font;

        this.gcData = gcData;
        Reset();   
    }


    public void Reset()
    {
        isDoneDrawing = false;
        delayInMilliseconds = 30;
        time = 0;
        typedText = "";
        i = 0;  // index for the LIST of strings
        p = 1;  // offset for this index
        j = 0;
        isDoneDrawingLine = true;
        updatedLastRow = "";
    }


    public void Update(GameTime gameTime)
    {

        if (!isDoneDrawing)
        {
            time += gameTime.ElapsedGameTime.TotalMilliseconds / delayInMilliseconds;

            if (time > 60) // counting the scrolling of lines
            {
                typedText = "";

                for (j = i; j < i + p - 1; j++)  // -1 is here because of the lastRow which we are handling differently
                {
                    typedText += gcData[j] + "\n";
                }

                p++;
                if (p > numberOfLines)
                {
                    // All lines are already drawn on the screen.
                    // In this point we are stopping increasing offset "p" and instead we are increasing index "i" 
                    p = numberOfLines;
                    i++;
                }

                if (gcData.Count - p + 1 == i) // we reached end of credits.txt file. Starting again.
                {
                    i = 0;
                    p = 1;
                }

                time = 0;
                isDoneDrawingLine = false;  // we are done with the lines so lets start counting the "moving" characters

            }

            if (!isDoneDrawingLine) // counting the scrolling of standalone characters
            {
                lastRow = gcData[j];

                if ((int)time > lastRow.Length)
                {
                    isDoneDrawingLine = true;
                    time = 0;
                }
                else
                {
                    updatedLastRow = lastRow.Substring(0, (int)time);
                }
            }

        }

    }


    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.DrawString(font, typedText + updatedLastRow, new Vector2(x, y), Color.White);
    }


}
