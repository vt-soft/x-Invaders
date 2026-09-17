using System;
using System.Collections.Generic;
using System.Text;

namespace xInvaders;


/// <summary>
/// This class is used to store the configuration data (in x-invaders_config.json) for the game. 
/// </summary>
internal class FileConfigData
{
    public DateOnly Last_played_date { get; set; } = DateOnly.FromDateTime(DateTime.Now);
    public int Daily_max_score { get; set; } = 0;
    public int Ath_score { get; set; } = 0;
    public float Music_volume { get; set; } = 0.125f;
    public float Effects_volume { get; set; } = 0.25f;

  
}
