using System;
using System.Collections.Generic;
using System.Text;

namespace xInvaders;

/// <summary>
/// Default game strings for the game. 
/// This class is used to store the default game strings (in game-strings.json) for the game.
/// </summary>
internal class FileGameStrings
{
    public string s1 { get; set; }
    public string s2 { get; set; }
    public string s3 { get; set; }
    public string s4 { get; set; }
    public string s5 { get; set; }
    public string s6 { get; set; }
    public string s7 { get; set; }
    public string s8 { get; set; }
    public string s9 { get; set; }
    public string s10 { get; set; }
    public string s11 { get; set; }
    public string s12 { get; set; }
    public string s13 { get; set; }
    public string s14 { get; set; }
    public string s15 { get; set; }
    public bool buttonVisible { get; set; }
    public string s16 { get; set; }
    public string s17 { get; set; }
    public string s18 { get; set; }
    public string s19 { get; set; }

    public string s20 { get; set; }

    public string s21 { get; set; }

    public string s22 { get; set; }

    public FileGameStrings()
    {

        s1 = "x-Invaders";
        s2 = "Space shooter game";
        s3 = "Arrow keys:  Left - Right - Up - Down  |  X  Fire   |   P  Pause  ";
        s4 = "Press ENTER to start";
        s5 = "Music volume:";
        s6 = "Sound effects volume:";

        s7 = "score";
        s8 = "max score";
        s9 = "daily max score";
        s10 = "ATH score";
        
        s11 = "Game Over";
        s12 = "Your max (highest) score is";
        s13 = "Press ENTER to play again";

        s14 = "https://www.vt-soft.com/x-invaders";
        s15 = "More info ...";
        buttonVisible = true; 

        s16 = "UFO 1:\nYou earn 5 points for shooting down this UFO. " +
               "If the UFO reaches the ground you lose 10 points. Each shot costs one point.";

        s17 = "UFO 2:\nYou earn 5 points for shooting down this UFO. " +
               "If the UFO reaches the ground you lose 10 points. Each shot costs one point.";

        s18 = "UFO 3:\nYou earn 5 points for shooting down this UFO. " +
              "If the UFO reaches the ground you lose 10 points. Each shot costs one point.";

        s19 = "Iron mine:\nYou cannot destroy iron mines by shooting. " +
              "Only an explosion of the Death Star or an explosive mine can destroy an iron mine.";

        s20 = "Explosive mine:\nYou earn 10 points for destroying an explosive mine. " +
              "However, be careful when firing at explosive mines: their explosion will destroy everything nearby (except the Death Star). " +
               "You lose 10 points if an explosive mine reaches the ground.";

        s21 = "Death Star:\nThe Death Star requires five shots to destroy and awards 15 points when destroyed. " +
               "You lose 10 points if the Death Star reaches the ground and explodes.";

        s22 = "Game Paused";
    }
}
