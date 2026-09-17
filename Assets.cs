using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

namespace xInvaders;

/// <summary>
/// Central cache for textures that used to be loaded repeatedly (every time a Ufo, Bullet,
/// BigBossBullet or Explosion object was created) via ContentManager.Load inside their
/// constructors. Loading them once here at startup avoids the runtime overhead/stutter
/// caused by repeated Content.Load calls during gameplay.
/// </summary>
internal static class Assets
{
    // Ufo / mine / big boss textures
    public static Texture2D UfoBlue { get; private set; }
    public static Texture2D UfoGreen { get; private set; }
    public static Texture2D UfoPurple { get; private set; }
    public static Texture2D MineGrey { get; private set; }
    public static Texture2D MineRed { get; private set; }
    public static Texture2D BigBoss { get; private set; }
    public static Texture2D BigBoss2Part { get; private set; }

    // Bullet textures
    public static Texture2D Bullet { get; private set; }
    public static Texture2D GreyBullet { get; private set; }

    // Explosion textures
    public static Texture2D ExplosionBig { get; private set; }
    public static Texture2D ExplosionSmall { get; private set; }

    // Star / UI textures
    public static Texture2D Star { get; private set; }
    public static Texture2D SliderBar { get; private set; }
    public static Texture2D SliderButton { get; private set; }

    // Rocket textures
    public static Texture2D RocketBody { get; private set; }
    public static Texture2D RocketFlames { get; private set; }

    // Fonts
    public static SpriteFont FontTitle { get; private set; }         // For big headline
    public static SpriteFont FontTitle2 { get; private set; }        // For "Game paused" and "Game Over" text
    public static SpriteFont FontText { get; private set; }          // For game score and other texts

    // Sounds and music
    public static SoundEffect SoundNextLevel { get; private set; }
    public static SoundEffect ShootSound { get; private set; }
    public static SoundEffect RedMineSound { get; private set; }
    public static SoundEffect GreyMineSound { get; private set; }
    public static SoundEffect UfoSound { get; private set; }
    public static SoundEffect DeathStarExplosion { get; private set; }
    public static SoundEffect DeathStarComming { get; private set; }
    public static Song BackgroundMusic { get; private set; }

    private static bool loaded;

    /// <summary>
    /// Loads all cached textures once. Safe to call multiple times - subsequent calls are no-ops.
    /// </summary>
    public static void LoadAll(ContentManager content)
    {
        if (loaded)
            return;

        UfoBlue = content.Load<Texture2D>("_pictures/ufo_blue_80x41");
        UfoGreen = content.Load<Texture2D>("_pictures/ufo_green_80x41");
        UfoPurple = content.Load<Texture2D>("_pictures/ufo_purple_80x41");
        MineGrey = content.Load<Texture2D>("_pictures/mine_grey_60x60");
        MineRed = content.Load<Texture2D>("_pictures/mine_red_60x60");
        BigBoss = content.Load<Texture2D>("_pictures/big_boss");
        BigBoss2Part = content.Load<Texture2D>("_pictures/big_boss_2part");

        Bullet = content.Load<Texture2D>("_pictures/bullet10x22");
        GreyBullet = content.Load<Texture2D>("_pictures/grey_bullet");

        ExplosionBig = content.Load<Texture2D>("_pictures/explosion_big_2");
        ExplosionSmall = content.Load<Texture2D>("_pictures/explosion_small");

        Star = content.Load<Texture2D>("_pictures/star2x2");
        SliderBar = content.Load<Texture2D>("_pictures/spr_slider_bar");
        SliderButton = content.Load<Texture2D>("_pictures/spr_slider_button");

        RocketBody = content.Load<Texture2D>("_pictures/rocket_body");
        RocketFlames = content.Load<Texture2D>("_pictures/rocket_flames_2");

        FontTitle = content.Load<SpriteFont>("_fonts/font_rubik_80s_fade");
        FontTitle2 = content.Load<SpriteFont>("_fonts/font_rubik_80s_fade_2");
        FontText = content.Load<SpriteFont>("_fonts/font_open_sans");

        SoundNextLevel = content.Load<SoundEffect>("_sounds/mixkit_winning_a_coin_video_game_2069");
        ShootSound = content.Load<SoundEffect>("_sounds/mixkit_laser_weapon_shot_1681");
        RedMineSound = content.Load<SoundEffect>("_sounds/mixkit_sea_mine_explosion_1184");
        GreyMineSound = content.Load<SoundEffect>("_sounds/mixkit_blast_hit_with_echo_2186");
        UfoSound = content.Load<SoundEffect>("_sounds/mixkit_big_fire_magic_swoosh_1327");
        DeathStarExplosion = content.Load<SoundEffect>("_sounds/death_star_explosion");
        DeathStarComming = content.Load<SoundEffect>("_sounds/death_star_comming");
        BackgroundMusic = content.Load<Song>("_music/space_heroes");

        loaded = true;
    }
}
