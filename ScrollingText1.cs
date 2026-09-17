using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace xInvaders;

/// <summary>
/// This class is displaying scrolling text (from game-string.json file) on the intro screen.
/// </summary>
internal class ScrollingText1
{
    private SpriteFont font;
    private String[] data;
    private String typedText;
    private int x = 300;
    private int y = 430;
    private int delayInMilliseconds = 100;
    private double time;
    private int offset;
    private int lb;
    private int item;
    private List<int>[] lineBreaks;                    // Here we are storing linebreak positions
    private Texture2D[] ufoList = new Texture2D[6];


    /// <summary>
    /// Initializes a new instance of the ScrollingText1 class.
    /// </summary>
    /// <param name="Content">The ContentManager to load textures.</param>
    /// <param name="font">The SpriteFont used to draw the text.</param>
    /// <param name="data">The strings to be displayed as scrolling text.</param>
    public ScrollingText1(ContentManager Content, SpriteFont font, params string[] data)
    {
        this.font = font;
        this.data = data;

        lineBreaks = new List<int>[data.Length];
        for (int i = 0; i < data.Length; i++)
        {
            lineBreaks[i] = new List<int>();
        }

        Reset();
        ProcessStrings();

        ufoList[0] = Assets.UfoBlue;
        ufoList[1] = Assets.UfoGreen;
        ufoList[2] = Assets.UfoPurple;
        ufoList[3] = Assets.MineGrey;
        ufoList[4] = Assets.MineRed;
        ufoList[5] = Assets.BigBoss;


        // In case we have pictures in external files then we are using these external files

        if (XInvaders.ufoFromFile1 != null)
            ufoList[0] = XInvaders.ufoFromFile1;

        if (XInvaders.ufoFromFile2 != null)
            ufoList[1] = XInvaders.ufoFromFile2;

        if (XInvaders.ufoFromFile3 != null)
            ufoList[2] = XInvaders.ufoFromFile3;

        if (XInvaders.mineFromFile1 != null)
            ufoList[3] = XInvaders.mineFromFile1;

        if (XInvaders.mineFromFile2 != null)
            ufoList[4] = XInvaders.mineFromFile2;

        if (XInvaders.deathStarFromFile != null)
            ufoList[5] = XInvaders.deathStarFromFile;
    }


    public void Reset()
    {
        time = 0;
        offset = 0;
        typedText = "";
        lb = 0;
        item = 0;
    }

    /// <summary>
    /// Processes the input strings to prepare them for scrolling display. It splits the strings into lines based on the font's measurement and counts line breaks for proper scrolling behavior.
    /// </summary>
    private void ProcessStrings()
    {
        for (int i = 0; i < data.Length; i++)   // ufo 1-3 + mine 1-2
        {
            String[] wordArray = data[i].Split(' ');
            String line = String.Empty;
            String returnString = String.Empty;

            foreach (String word in wordArray)
            {
                if (font.MeasureString(line + word).Length() > 280)
                {
                    returnString = returnString + line + '\n';
                    line = String.Empty;
                }

                line = line + word + ' ';
            }
           
            data[i] = returnString + line;
            // Adding some space characters (to the end of the text) to stop the text on the screen for a while
            data[i] = data[i] + "                                                                                                ";


            // Counting linebreaks - we need to know their position to properly scroll the text
            for (int j = 0; j < data[i].Length; j++)
            {
                char c = (char)data[i][j];
                if ((c == '\n'))
                {
                    lineBreaks[i].Add(j);
                }
            }

        }
    }



    public void Update(GameTime gameTime)
    {

        time += gameTime.ElapsedGameTime.TotalMilliseconds / delayInMilliseconds;

        // Go to next ufo/mine
        if (Math.Ceiling(time) >= data[item].Length)
        {
            time = 0;
            offset = 0;
            typedText = "";
            lb = 0;

            item++;  // item 0 = Ufo1, ...
            if (item == data.Length) // Go back to ufo 1
                item = 0;
        }

        // Count offset only for items with at least 14 linebreaks
        if (lineBreaks[item].Count > 14)
        {
            if (time >= lineBreaks[item][lb + 14]) // 14 is number of lines for the text
            {
                offset = lineBreaks[item][lb] + 1;
                lb++;
            }
        }

        typedText = data[item].Substring(0 + offset, (int)time - offset);

    }





    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.DrawString(font, typedText, new Vector2(x, y), Color.WhiteSmoke);
        spriteBatch.Draw(ufoList[item], new Vector2(180, 435), null, Color.White, 0.0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0);
    }


}
