

// **************************************************************************************************************************
// *  Project information:                                                                                                  *
// *                                                                                                                        *
// *  Name        :  x-Invaders                                                                                             *
// *  Description :  A simple space invaders game implemented in C# using MonoGame framework.                               *
// *  Language    :  C# 14                                                                                                  *
// *  Framework   :  .NET 10.0                                                                                              *
// *  UI          :  Monogame                                                                                               *
// *  Web         :  https://www.vt-soft.com/x-invaders                                                                     *
// *                                                                                                                        *
// *  Please be aware that I am not professional developer so this code is not perfect                                      *
// *  and it is probably not following all best programming practices.                                                      *
// *  However I still hope that you will find it useful and that it will help you to create your own application.           *
// *  For more projects please check https://www.vt-soft.com/                                                               *
// *  Any link to this site is highly appreciated. Enjoy the code! :)                                                       *
// *                                                                                                                        *
// *  Copyright(c) 2026, vt-soft                                                                                            *
// *  All rights reserved.                                                                                                  *
// *                                                                                                                        *
// *  This source code is licensed under the MIT-style license.                                                             *
// *  More info in the license.txt file in the root directory of this source tree.                                          *
// **************************************************************************************************************************


using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;






namespace xInvaders;


internal class XInvaders : Game
{
    private GraphicsDeviceManager graphics;
    private SpriteBatch spriteBatch;
    private Texture2D overlayTexture; // 1x1 pixel texture reused for semi-transparent overlays (e.g. Game Over screen)
    private InputManager inputManager; // InputManager object to manage the input devices (mouse and keyboard)

 
    private Rocket rocket;
    private StarField starField;
    private BulletField bulletField;
    private ExplosionField explosionField;
    private UfoField ufoField;
    public static BigBossBulletField bigBossBulletField;
    CollisionDetection cDetection; // CollisionDetection object to manage collisions between rocket, bullets and ufos/mines



    private FileManager fileManager;
    private FileConfigData cData;    // config data from x-invaders_config.json file
    private FileGameStrings gsData;  // game strings from game-strings.json file
    private List<string> gcData;    // game credits from game-credits.txt file



    public static int XRes; // Game window resolution  x-dimension
    public static int YRes; // Game window resolution  y-dimension


    // Game data:
    public const int InitialLevelSpeed = 70;    // Initial level speed for Ufos and Mines 
    public static int LevelSpeed;  // LevelSpeed is increased with each level
    public static float RocketSpeed; // Rocket speed is increased with each level
    public static int GameState = 0;  // 0 = intro screen, 1 = game, 2 = game over
    public static int GameLevel = 0;
    public static int Score = 0;
    public static int MaxScore = 0;

    //public static int DailyMaxScore = 0;   use cData.Daily_max_score isntead
    //public static int AthScore = 0;        use cData.Ath_score instead

    public static Boolean GameSinActivated = false;
    private Boolean pause = false;
    private Boolean gameSaved = false;


    // Auxiliary field to prevent adding more ufos/mines in the same level.
    // (Because score can also decrease if ufo/mine hit the ground. So we can reach some level several times.)
    private int[] UfosAlreadyAdded = new int[11];


    // accumulator to run daily-check once per minute
    private double secondsSinceLastCheck = 0.0;
    private static readonly TimeSpan MaxGameplayElapsedTime = TimeSpan.FromSeconds(1.0 / 30.0);


    public static float EffectsVolume;

    public static Texture2D ufoFromFile1 = null, ufoFromFile2 = null, ufoFromFile3 = null;   //user can use his/her own textures  
    public static Texture2D mineFromFile1 = null, mineFromFile2 = null, deathStarFromFile = null;


    private WebButton WebButton;
    private Slider musicSlider, effectsSlider;
    ScrollingText1 scrollingText1; // on intro screen
    ScrollingText2 scrollingText2; // on game over screen
  
   


    public XInvaders()
    {
        graphics = new GraphicsDeviceManager(this);
        graphics.SynchronizeWithVerticalRetrace = true; // Enable VSync to prevent screen tearing
        IsFixedTimeStep = false; // Disable fixed time step to allow the game to run as fast as possible (not limited to 60 FPS)

        // Setting game window resolution to 600x800 pixels
        XRes = graphics.PreferredBackBufferWidth = 600;
        YRes = graphics.PreferredBackBufferHeight = 800;
        graphics.ApplyChanges();

        Content.RootDirectory = "Content";
        IsMouseVisible = true;

        this.Exiting += OnGameExiting;  // Save game (score and music settings) when the user closes the game window
        this.Deactivated += OnWindowDeactivated; // Pause the game when the window loses focus
    }

  

    protected override void Initialize()
    {
   
        base.Initialize();
    }


    protected override void LoadContent()
    {
        inputManager = new InputManager();   // Create an InputManager object to manage the input devices (mouse and keyboard)
        spriteBatch = new SpriteBatch(base.GraphicsDevice);

        Assets.LoadAll(Content);   // Preload textures/fonts/sounds/music reused across the game to avoid runtime Content.Load stutter

        starField = new StarField(Content, 50);         // Create a star field with 50 stars
        rocket = new Rocket(Content, GraphicsDevice);   // Create a rocket object
        bulletField = new BulletField(Content);         // Create a bullet field object to manage bullets
        
        rocket.FirePressed += bulletField.OnRocketFire; // Subscribe bullet field to rocket fire events so BulletField reacts when the player fires
        
        ufoField = new UfoField(Content);               // Create a ufo field object to manage ufos and mines
        bigBossBulletField = new BigBossBulletField(Content);
        explosionField = new ExplosionField(Content);
        cDetection  = new CollisionDetection(Content, rocket, ufoField, bulletField, explosionField);
        

        // Get data from external files (from x-invaders_config.json, game-strings.json, game-credits.txt)
        fileManager = new FileManager();
        cData = fileManager.GetConfigData();
        // Ensure daily score is up-to-date on startup
        CheckLastPlayedDate();

        gsData = fileManager.GetGameStrings();
        gcData = fileManager.GetGameCredits();
        // Note: the window title (used by Windows/Task Manager to identify the app) is
        // intentionally kept as "xInvaders" regardless of the in-game display string
        // (gsData.s1, e.g. "x-Invaders") so the process/window name stays consistent
        // with the exe name across all build/publish configurations.
        Window.Title = "xInvaders";
        SetWindowIcon();

        // Make sure the max value is bigger then the value in FileConfigData. Otherwise the slider will not work correctly.
        musicSlider = new Slider(Content, 390, 330, 0, 0.25f, cData.Music_volume); 
        effectsSlider = new Slider(Content, 390, 370, 0, 0.5f, cData.Effects_volume);

        // Start playing the background music
        MediaPlayer.IsRepeating = true;
        MediaPlayer.Volume = cData.Music_volume;
        MediaPlayer.Play(Assets.BackgroundMusic);
        EffectsVolume = cData.Effects_volume;

        CheckExternalFiles(base.GraphicsDevice);

        scrollingText1 = new ScrollingText1(Content, Assets.FontText, gsData.s16, gsData.s17, gsData.s18, gsData.s19, gsData.s20, gsData.s21);
        scrollingText2 = new ScrollingText2(48, 290, 18, Assets.FontText, gcData);


        if (gsData.buttonVisible)
            WebButton = new WebButton(Content, 90, 650, Assets.FontText, gsData.s14, gsData.s15);

        ResetGame();
    }

    /// <summary>
    /// Explicitly sets the game window icon from the running executable.
    /// This is needed because when the app is published as a self-contained single-file
    /// (Publish folder), Assembly.Location is empty, which prevents MonoGame/WinForms from
    /// automatically resolving the icon, resulting in a generic icon being shown for the window.
    /// Using the actual process executable path instead works correctly in both cases.
    /// </summary>
    private void SetWindowIcon()
    {
        try
        {
            string exePath = Process.GetCurrentProcess().MainModule?.FileName;
            if (string.IsNullOrEmpty(exePath) || !File.Exists(exePath))
                return;

            using System.Drawing.Icon extractedIcon = System.Drawing.Icon.ExtractAssociatedIcon(exePath);
            if (extractedIcon == null)
                return;

            if (System.Windows.Forms.Control.FromHandle(Window.Handle) is System.Windows.Forms.Form form)
            {
                form.Icon = (System.Drawing.Icon)extractedIcon.Clone();
            }
        }
        catch
        {
            // Icon is a cosmetic detail; failing to set it should never crash the game.
        }
    }




    protected override void Update(GameTime gameTime)
    {
        // run daily check once per minute (accumulate elapsed time)
        secondsSinceLastCheck += gameTime.ElapsedGameTime.TotalSeconds;
        if (secondsSinceLastCheck >= 60.0)
        {
            secondsSinceLastCheck -= 60.0;
            try { CheckLastPlayedDate(); } catch { }
        }

        GameTime gameplayTime = ClampGameplayTime(gameTime);

        inputManager.Update();        // Update the input manager to check for user input (keyboard and mouse).
        starField.Update(gameplayTime);   // Stars are always moving, even in the intro screen and game over screen.

        switch (GameState)
        {
            case 0: // Intro screen
                if (inputManager.KeyPressed(Keys.Enter))
                {
                    GameState = 1;
                }
                
                musicSlider.HandleInput(inputManager);
                effectsSlider.HandleInput(inputManager);

                if (musicSlider.ValueChanged)
                    MediaPlayer.Volume = musicSlider.GetValue();

                if (effectsSlider.ValueChanged)
                    EffectsVolume = effectsSlider.GetValue();

                if (gsData.buttonVisible)
                    WebButton.HandleInput(inputManager);

                scrollingText1.Update(gameplayTime);
            
                break;

            case 1: // Game
                {
                    // Game pause:
                    if (inputManager.KeyPressed(Keys.P) && pause == false)
                    {
                        pause = true;
                    }
                    else if (inputManager.KeyPressed(Keys.P) && pause == true)
                    {
                        pause = false;
                    }

                    if (!pause)
                    {
                        rocket.HandleInput(inputManager, gameplayTime); // Updating rocket position based on user input
                        rocket.Update(gameplayTime);                  // Updating rocket flames
                       
                        bulletField.Update(gameplayTime);    
                        bigBossBulletField.Update(gameplayTime);
                        ufoField.Update(gameplayTime);
                        explosionField.Update(gameplayTime);
                        cDetection.Update(gameplayTime);
                        UpdateLevelsAndScore();
                    }
                    break;
                }

            case 2:  // Game over
                {
                    bulletField.Update(gameplayTime);  // Give bullets time to disappear from the screen after the game is over
                    bigBossBulletField.Update(gameplayTime);
                    // ufoField.Update(gameTime) - Ufos are frozen - for this reason no ufoField.Update() is here
                    explosionField.Update(gameplayTime);
                    cDetection.Update(gameplayTime);

                    scrollingText2.Update(gameplayTime);
                    UpdateLevelsAndScore();

                    if (!gameSaved) // Saving score and volume settings to json file
                    {
                        SaveGameToFile();
                    }

                    if (inputManager.KeyPressed(Keys.Enter))
                    {
                        ResetGame();
                        GameState = 0;
                    }

                    break;
                }
        }
      
        base.Update(gameTime);
    }

    /// <summary>
    /// Clamps the elapsed game time to a maximum value to prevent large jumps in game logic due to lag or pauses.  
    /// </summary>
    /// <param name="gameTime"></param>
    /// <returns></returns>
    private static GameTime ClampGameplayTime(GameTime gameTime)
    {
        if (gameTime.ElapsedGameTime <= MaxGameplayElapsedTime)
            return gameTime;

        return new GameTime(gameTime.TotalGameTime, MaxGameplayElapsedTime, gameTime.IsRunningSlowly); 
    }



    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);
        spriteBatch.Begin();

        starField.Draw(spriteBatch); // Star field is always drawn, even in the intro screen and game over screen.

        if (GameState == 0) // Intro screen
        {
             StartScreen();
        }

        if (GameState == 1) // Game screen
        {
            bulletField.Draw(spriteBatch);
            explosionField.Draw(spriteBatch);
            ufoField.Draw(spriteBatch);
            bigBossBulletField.Draw(spriteBatch);
            rocket.Draw(spriteBatch);

            if (pause)
            {
                string s = gsData.s22; // "Game paused" string
                Vector2 size = Assets.FontTitle2.MeasureString(s);
                spriteBatch.DrawString(Assets.FontTitle2, s, new Vector2((XRes - size.X) / 2, 150), Color.White);
            }
        }

        if (GameState == 2) // Game over screen
        {
            bulletField.Draw(spriteBatch);
            explosionField.Draw(spriteBatch);
            ufoField.Draw(spriteBatch);    // Ufos are frozen, so only Draw but no Update method for Ufos
            bigBossBulletField.Draw(spriteBatch);
            GameOverScreen();
            scrollingText2.Draw(spriteBatch);
        }

        DrawScore();


        spriteBatch.End();
        base.Draw(gameTime);
    }


    /// <summary>
    /// Checks if the last played date is different from today's date. 
    /// If it is, resets the daily max score to 0 and updates the last played date to today.
    /// </summary>
    private void CheckLastPlayedDate()
    {
        if (cData.Last_played_date != DateOnly.FromDateTime(DateTime.Now))
        {
            cData.Daily_max_score = MaxScore;
            cData.Last_played_date = DateOnly.FromDateTime(DateTime.Now);
        }
    }


    /// <summary>
    /// Draws the intro screen with game title, instructions, sliders for music and effects volume, and a web button if visible.
    /// </summary>
    private void StartScreen()
    {
        starField.Draw(spriteBatch);

        string s = gsData.s1;
        Vector2 size = Assets.FontTitle.MeasureString(s);  // Measuring the text to center it on the screen correctly
        spriteBatch.DrawString(Assets.FontTitle, s, new Vector2((XRes - size.X) / 2, 45), Color.White);

        s = gsData.s2; //  "space shooter game";
        size = Assets.FontText.MeasureString(s);
        spriteBatch.DrawString(Assets.FontText, s, new Vector2((XRes - size.X) / 2, 155), Color.Gray);

        s =  gsData.s3; //  "?  left   |   ?  right   |   ?  up   |   ?  down   |   X  fire   |   P  pause  ";
        size = Assets.FontText.MeasureString(s);
        spriteBatch.DrawString(Assets.FontText, s, new Vector2((XRes - size.X) / 2, 225), Color.WhiteSmoke);

        s = gsData.s4; // Press ENTER to start the game
        size = Assets.FontText.MeasureString(s);
        spriteBatch.DrawString(Assets.FontText, s, new Vector2((XRes - size.X) / 2, 260), Color.WhiteSmoke);

        s = gsData.s5; // Music volume
        size = Assets.FontText.MeasureString(s);
        spriteBatch.DrawString(Assets.FontText, s, new Vector2((XRes / 2) - size.X - 10, 320), Color.WhiteSmoke);

        s = gsData.s6; // Effects volume
        size = Assets.FontText.MeasureString(s);
        spriteBatch.DrawString(Assets.FontText, s, new Vector2((XRes / 2) - size.X - 10, 360), Color.WhiteSmoke);

        musicSlider.Draw(spriteBatch);
        effectsSlider.Draw(spriteBatch);
       
        if (gsData.buttonVisible)
            WebButton.Draw(spriteBatch);
       
        scrollingText1.Draw(spriteBatch);
    }

    private void GameOverScreen()
    {

        // transparent rectangle:
        if (overlayTexture == null)
        {
            overlayTexture = new Texture2D(GraphicsDevice, 1, 1);
            overlayTexture.SetData(new Color[] { Color.Black });
        }
        spriteBatch.Draw(overlayTexture, new Rectangle(40, 40, 520, 260), new Color(50, 50, 50, 0.8f));  // background for Game Over

        spriteBatch.Draw(overlayTexture, new Rectangle(40, 280, 540, 470), new Color(50, 50, 50, 0.8f)); // background for scrolling text

        string s = gsData.s11; // Game Over
        Vector2 size = Assets.FontTitle2.MeasureString(s);  // measuring the text to center it on the screen correctly
        spriteBatch.DrawString(Assets.FontTitle2, s, new Vector2((XRes - size.X) / 2, 80), Color.White);

        s = $"{gsData.s12} {MaxScore}."; //Your max score is
        size = Assets.FontText.MeasureString(s);
        spriteBatch.DrawString(Assets.FontText, s, new Vector2((XRes - size.X) / 2, 190), Color.White);

        s = gsData.s13; // Press ENTER to play again.
        size = Assets.FontText.MeasureString(s);
        spriteBatch.DrawString(Assets.FontText, s, new Vector2((XRes - size.X) / 2, 220), Color.White);

    }

    /// <summary>
    /// Draws the score, max score, daily max score and ATH score on the screen.
    /// </summary>
    private void DrawScore()
    {
        string s = $"{gsData.s7}: {Score}   {gsData.s8}: {MaxScore}   {gsData.s9}: {cData.Daily_max_score}    {gsData.s10}: {cData.Ath_score}";
        spriteBatch.DrawString(Assets.FontText, s, new Vector2(8, YRes - 25), Color.WhiteSmoke);
    }




    /// <summary>
    /// Resets the game state to its initial values.
    private void ResetGame() 
    {
        GameLevel = 0;
        LevelSpeed = 0;

        Score = 0;
        MaxScore = 0;
        
        rocket.Reset();
        bulletField.Reset();
        ufoField.Reset();
        explosionField.Reset();
        bigBossBulletField.Reset();

        GameSinActivated = false;
        Array.Clear(UfosAlreadyAdded);

        ufoField.UfoAdd(1, 4); // Add 4 ufos at the beginning

        // ufoField.UfoAdd(4, 1); // big boss test
        // ufoField.UfoAdd(2, 1); // test
        // ufoField.UfoAdd(3, 1); // test
        // ufoField.UfoAdd(3, 1); // test
        // ufoField.UfoAdd(2, 1); // test

        gameSaved = false;
        scrollingText1.Reset();
        scrollingText2.Reset();

    }


    /// <summary>
    /// Updates the game level and score and also manages the addition of new UFOs for each level.
    /// </summary>
    private void UpdateLevelsAndScore()
    {
        if (Score < 0)
            Score = 0;

        if (Score > MaxScore)
            MaxScore = Score;

        if (Score > cData.Daily_max_score)
            cData.Daily_max_score = Score;

        if (Score > cData.Ath_score)
            cData.Ath_score = Score;


        GameLevel = (int)Math.Floor((double)Score / 100);
        LevelSpeed = InitialLevelSpeed + GameLevel * 8;

        RocketSpeed = (float)(GameLevel * 2) / 12;

        // Adding more ufos each level.

        if ((GameLevel == 1) && UfosAlreadyAdded[1] == 0)
        {
            // It means that we already added ufos for this level and we will not add them again if we reach this level again
            // (because score can also decrease if ufo/mine hit the ground. So we can reach some level several times.)
            UfosAlreadyAdded[1] = 1;
            
            ufoField.UfoAdd(1, 2);
            Assets.SoundNextLevel.Play(XInvaders.EffectsVolume, 0.0f, 0.0f);
        }
        if ((GameLevel == 2) && UfosAlreadyAdded[2] == 0)
        {
            UfosAlreadyAdded[2] = 1;
            ufoField.UfoAdd(1, 3);
            Assets.SoundNextLevel.Play(XInvaders.EffectsVolume, 0.0f, 0.0f);
        }
        if ((GameLevel == 3) && UfosAlreadyAdded[3] == 0)
        {
            UfosAlreadyAdded[3] = 1;
            ufoField.UfoAdd(1, 2);
            ufoField.UfoAdd(2, 2);
            Assets.SoundNextLevel.Play(XInvaders.EffectsVolume, 0.0f, 0.0f);
        }
        if ((GameLevel == 4) && UfosAlreadyAdded[4] == 0)
        {
            UfosAlreadyAdded[4] = 1;
            ufoField.UfoAdd(2, 2);
            ufoField.UfoAdd(3, 1);
            Assets.SoundNextLevel.Play(XInvaders.EffectsVolume, 0.0f, 0.0f);
        }
        if ((GameLevel == 5) && UfosAlreadyAdded[5] == 0)
        {
            UfosAlreadyAdded[5] = 1;
            ufoField.UfoAdd(1, 1);
            ufoField.UfoAdd(2, 1);
            ufoField.UfoAdd(3, 1);
            ufoField.UfoAdd(4, 1); // Big boss
            Assets.SoundNextLevel.Play(XInvaders.EffectsVolume, 0.0f, 0.0f);
        }

        if ((GameLevel == 6) && UfosAlreadyAdded[6] == 0)
        {
            UfosAlreadyAdded[6] = 1;
            ufoField.UfoAdd(1, 1);
            ufoField.UfoAdd(3, 1);
            Assets.SoundNextLevel.Play(XInvaders.EffectsVolume, 0.0f, 0.0f);
        }

        if ((GameLevel == 7) && UfosAlreadyAdded[7] == 0)
        {
            UfosAlreadyAdded[7] = 1;
            ufoField.UfoAdd(1, 2);
            ufoField.UfoAdd(2, 1);
            ufoField.UfoAdd(3, 1);
            GameSinActivated = true; // Activating sinus movement for some random ufos
            Assets.SoundNextLevel.Play(XInvaders.EffectsVolume, 0.0f, 0.0f);
        }

        if ((GameLevel == 8) && UfosAlreadyAdded[8] == 0)
        {
            UfosAlreadyAdded[8] = 1;
            ufoField.UfoAdd(1, 2);
            ufoField.UfoAdd(2, 1);
            Assets.SoundNextLevel.Play(XInvaders.EffectsVolume, 0.0f, 0.0f);
        }

        if ((GameLevel == 9) && UfosAlreadyAdded[9] == 0)
        {
            UfosAlreadyAdded[9] = 1;
            ufoField.UfoAdd(1, 2);
            ufoField.UfoAdd(2, 1);
            Assets.SoundNextLevel.Play(XInvaders.EffectsVolume, 0.0f, 0.0f);
        }

        if ((GameLevel == 10) && UfosAlreadyAdded[10] == 0)
        {
            UfosAlreadyAdded[10] = 1;
            ufoField.UfoAdd(1, 2);
            Assets.SoundNextLevel.Play(XInvaders.EffectsVolume, 0.0f, 0.0f);
        }
    }


    /// <summary>
    /// User can use his/her own pictures. 
    /// </summary>
    /// <param name="graphicsDevice"></param>
    private void CheckExternalFiles(GraphicsDevice graphicsDevice)
    {
        string fileName;
        string folderName = AppContext.BaseDirectory;

        fileName = Path.Combine(folderName,"PNG", "ufo1.png");
        if (System.IO.File.Exists(fileName))
        {
            FileStream fileStream = new FileStream(fileName, FileMode.Open);
            ufoFromFile1 = Texture2D.FromStream(graphicsDevice, fileStream);
            fileStream.Dispose();
        }

        fileName = Path.Combine(folderName, "PNG", "ufo2.png");
        if (System.IO.File.Exists(fileName))
        {
            FileStream fileStream = new FileStream(fileName, FileMode.Open);
            ufoFromFile2 = Texture2D.FromStream(graphicsDevice, fileStream);
            fileStream.Dispose();
        }

        fileName = Path.Combine(folderName, "PNG", "ufo3.png");
        if (System.IO.File.Exists(fileName))
        {
            FileStream fileStream = new FileStream(fileName, FileMode.Open);
            ufoFromFile3 = Texture2D.FromStream(graphicsDevice, fileStream);
            fileStream.Dispose();
        }

        fileName = Path.Combine(folderName, "PNG", "mine1.png");
        if (System.IO.File.Exists(fileName))
        {
            FileStream fileStream = new FileStream(fileName, FileMode.Open);
            mineFromFile1 = Texture2D.FromStream(graphicsDevice, fileStream);
            fileStream.Dispose();
        }

        fileName = Path.Combine(folderName, "PNG", "mine2.png");
        if (System.IO.File.Exists(fileName))
        {
            FileStream fileStream = new FileStream(fileName, FileMode.Open);
            mineFromFile2 = Texture2D.FromStream(graphicsDevice, fileStream);
            fileStream.Dispose();
        }

        fileName = Path.Combine(folderName, "PNG", "deathstar.png");
        if (System.IO.File.Exists(fileName))
        {
            FileStream fileStream = new FileStream(fileName, FileMode.Open);
            deathStarFromFile = Texture2D.FromStream(graphicsDevice, fileStream);
            fileStream.Dispose();
        }

    }





    // ******************************************************************************************************************************
    // ******************   Some tiny methods for proper game behaviour  ************************************************************




    /// <summary>
    /// Saves the game data (daily max score, ATH score, music volume and effects volume) to the x-invaders_config.json file when the game is exiting.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnGameExiting(object sender, EventArgs e)
    {
        try
        {
            // Only attempt to save if we have loaded file manager and haven't saved yet
            if (!gameSaved && fileManager != null && cData != null)
            {
                SaveGameToFile();
            }
        }
        catch
        {
            // If saving fails during shutdown, ignore to avoid blocking exit
        }
    }

    /// <summary>
    /// Pauses the game when the window loses focus (is deactivated).
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnWindowDeactivated(object sender, EventArgs e)
    {
        try
        { 
            if (GameState == 1 && !pause) // Only pause if the game is running and not already paused
                pause = true;
        }
        catch
        {
            // ignore
        }
    }

    /// <summary>
    /// Saves the game data (daily max score, ATH score, music volume and effects volume) to the x-invaders_config.json file.
    /// </summary>
    private void SaveGameToFile()
    {
        cData.Last_played_date = DateOnly.FromDateTime(DateTime.Now);
        cData.Music_volume = musicSlider.GetValue();
        cData.Effects_volume = effectsSlider.GetValue();

        fileManager.SaveConfigData(cData);

        gameSaved = true;
    }




}
