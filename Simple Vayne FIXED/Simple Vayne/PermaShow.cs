using System;
using System.Drawing;
using EloBuddy;
using EloBuddy.SDK.Rendering;
using SharpDX;
using Simple_Vayne.Properties;

namespace Simple_Vayne
{
    public class PermaShow
    {
        public static readonly TextureLoader TextureLoader = new TextureLoader();

        public static Vector2 Position;
        private static Sprite Sprite { get; set; }
        private static Text Text { get; set; }

        public static void Initalize()
        {
            TextureLoader.Load("sprite", Resources.sprite);

            Sprite = new Sprite(TextureLoader["sprite"]);

            Text = new Text("", new Font("Tahoma", 9, FontStyle.Regular));

            Position = new Vector2(Config.Misc.PermashowX, Config.Misc.PermashowY);

            Drawing.OnEndScene += Drawing_OnEndScene;
        }

        private static void Drawing_OnEndScene(EventArgs args)
        {
            if (!Config.Drawings.DrawPermashow)
                return;


            Sprite.Draw(Position);
            Text.Draw("Simple Vayne : Permashow", System.Drawing.Color.GreenYellow, (int)Position.X + 15, (int)Position.Y + 5);
            Text.Draw("Disable safety checks : ", System.Drawing.Color.White, (int)Position.X + 15, (int)Position.Y + 33);
            Text.Draw(Config.TumbleMenu.IgnoreAllChecks ? "Enabled" : "Disabled", Config.TumbleMenu.IgnoreAllChecks ? System.Drawing.Color.GreenYellow : System.Drawing.Color.Red, (int)Position.X + 20 + 149, (int)Position.Y + 33);
            Text.Draw("\nNo AA while stealth : ", System.Drawing.Color.White, (int)Position.X + 15, (int)Position.Y + 33);
            Text.Draw(Config.Misc.NoAAWhileStealth ? "\nEnabled" : "\nDisabled", Config.Misc.NoAAWhileStealth ? System.Drawing.Color.GreenYellow : System.Drawing.Color.Red, (int)Position.X + 20 + 149, (int)Position.Y + 33);
            Text.Draw("\n\nTumble mode : ", System.Drawing.Color.White, (int)Position.X + 15, (int)Position.Y + 33);
            Text.Draw(Config.TumbleMenu.Mode == 0 ? "\n\nCursorPos" : "\n\nAuto", System.Drawing.Color.GreenYellow, (int)Position.X + 20 + 149, (int)Position.Y + 33);

        }
    }
}