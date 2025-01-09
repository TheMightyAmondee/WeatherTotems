using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley.Objects;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewModdingAPI;
using StardewValley.GameData.LocationContexts;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Utilities;
using xTile.Dimensions;
using static StardewValley.Minigames.CraneGame;
using System.Collections.Generic;

namespace WeatherTotems
{
    public class WeatherTotem
    {
        private static IModHelper helper;
        private static IManifest manifest;

        // Get access to the required SMAPI apis
        public static void Initialise( IModHelper helper, IManifest manifest)
        {
            WeatherTotem.helper = helper;
            WeatherTotem.manifest = manifest;
        }

		private static void SetWeather(string weather)
		{
            if (Game1.currentLocation.GetLocationContextId() == "Default")
            {
                if (Utility.isFestivalDay(Game1.dayOfMonth + 1, Game1.season) == false)
                {
                    Game1.netWorldState.Value.WeatherForTomorrow = (Game1.weatherForTomorrow = weather);
                }
            }
            else
            {
                Game1.currentLocation.GetWeather().WeatherForTomorrow = weather;
            }
            if (weather == "GreenRain")
            {
                Game1.player.modData[$"{manifest.UniqueID}/GreenTotemUse"] = "true";
            }
            else
            {
                Game1.player.modData[$"{manifest.UniqueID}/GreenTotemUse"] = "false";
            }
        }

        public static bool UseWeatherTotem(Farmer who, int totemtype)
		{
			bool changedweather = false;

            // Get location context
            var location_context = Game1.currentLocation.GetLocationContextId();		
			string message = "Nothing";
            var totemname = "";
			
			// Get asset key for animation sprites
			string assetkey = helper.ModContent.GetInternalAssetName("assets/loosesprites.png").Name;

            switch (totemtype)
            {
                case 0:
                    SetWeather("Sun");
                    message = i18n.string_SunTotemUse();
                    totemname = "TheMightyAmondee.WeatherTotemsCP_SunTotem";
                    changedweather = true;
                    break;
                case 1:
                    foreach (var weather in Game1.currentLocation.GetLocationContext().WeatherConditions)
                    {
                        if (weather.Weather == "Wind")
                        {
                            SetWeather("Wind");
                            message = i18n.string_WindTotemUse();
                            totemname = "TheMightyAmondee.WeatherTotemsCP_WindTotem";
                            changedweather = true;
                            break;
                        }
                    }
                    break;
                case 2:
                    foreach (var weather in Game1.currentLocation.GetLocationContext().WeatherConditions)
                    {
                        if (weather.Weather == "Snow")
                        {
                            SetWeather("Snow");
                            message = i18n.string_SnowTotemUse();
                            totemname = "TheMightyAmondee.WeatherTotemsCP_SnowTotem";
                            changedweather = true;
                            break;
                        }
                    }

                    break;
                case 3:
                    foreach (var weather in Game1.currentLocation.GetLocationContext().WeatherConditions)
                    {
                        if (weather.Weather == "Storm")
                        {
                            SetWeather("Storm");
                            message = i18n.string_ThunderTotemUse();
                            totemname = "TheMightyAmondee.WeatherTotemsCP_ThunderTotem";
                            changedweather = true;
                            break;
                        }
                    }
                    break;
                case 4:
                    if (Game1.season == Season.Summer && Game1.dayOfMonth != 28 && location_context == "Default")
                    {
                        SetWeather("GreenRain");
                        message = i18n.string_GreenRainTotemUse();
                        totemname = "TheMightyAmondee.WeatherTotemsCP_GreenRainTotem";
                        changedweather = true;
                    }
                    break;
            }
            if (changedweather == false)
            {
                return changedweather;
            }

            Game1.pauseThenMessage(2000, message);

            // Totem activation stuff
            Game1.screenGlow = false;
			who.currentLocation.playSound("thunder");
			who.canMove = false;
			switch (totemtype)
			{
				case 0:
					Game1.screenGlowOnce(Color.Gold, hold: false);
					break;
				case 1:
					Game1.screenGlowOnce(Color.Lavender, hold: false);
					break;
				case 2:
					Game1.screenGlowOnce(Color.AliceBlue, hold: false);
					break;
				case 3:
					Game1.screenGlowOnce(Color.DarkSlateBlue, hold: false);
					break;
                case 4:
                    Game1.screenGlowOnce(Color.OliveDrab, hold: false);
                    break;
            }
			Game1.player.faceDirection(2);
			Game1.player.FarmerSprite.animateOnce(new FarmerSprite.AnimationFrame[1]
			{
				new FarmerSprite.AnimationFrame(57, 2000, secondaryArm: false, flip: false, Farmer.canMoveNow, behaviorAtEndOfFrame: true)
			});

			// Play totem animation 
			for (int i = 0; i < 6; i++)
			{
				void showsprites(string asset, Microsoft.Xna.Framework.Rectangle area)
                {
					Game1.Multiplayer.broadcastSprites(who.currentLocation, new TemporaryAnimatedSprite(asset, area, 9999f, 1, 999, who.Position + new Vector2(0f, -128f), flicker: false, flipped: false, 1f, 0.01f, Color.White * 0.8f, 2f, 0.01f, 0f, 0f)
					{
						motion = new Vector2((float)Game1.random.Next(-10, 11) / 10f, -2f),
						delayBeforeAnimationStart = i * 200
					});
				}

				void showspritefromtotemtype(Microsoft.Xna.Framework.Rectangle whichareaforthunderanimation)
                {
					switch (totemtype)
					{
						case 0:
							showsprites(assetkey, new Microsoft.Xna.Framework.Rectangle(0, 0, 24, 24));
							break;
						case 1:
							showsprites(assetkey, new Microsoft.Xna.Framework.Rectangle(24, 0, 51, 24));
							break;
						case 2:
							showsprites(assetkey, new Microsoft.Xna.Framework.Rectangle(75, 0, 25, 25));
							break;
						case 3:
							showsprites("LooseSprites\\Cursors", whichareaforthunderanimation);
                            break;
                        case 4:
                            showsprites(assetkey, new Microsoft.Xna.Framework.Rectangle(100, 0, 20, 24));
                            break;
                    }
				}

				showspritefromtotemtype(new Microsoft.Xna.Framework.Rectangle(645, 1079, 36, 56));
				showspritefromtotemtype(new Microsoft.Xna.Framework.Rectangle(648, 1045, 52, 33));
				showspritefromtotemtype(new Microsoft.Xna.Framework.Rectangle(648, 1045, 52, 33));				
			}

            TemporaryAnimatedSprite totemanimsprite = new TemporaryAnimatedSprite(0, 9999f, 1, 999, Game1.player.Position + new Vector2(0f, -96f), false, false, false, 0f)
            {
                motion = new Vector2(0f, -7f),
                acceleration = new Vector2(0f, 0.1f),
                scaleChange = 0.015f,
                alpha = 1f,
                alphaFade = 0.0075f,
                shakeIntensity = 1f,
                initialPosition = Game1.player.Position + new Vector2(0f, -96f),
                xPeriodic = true,
                xPeriodicLoopTime = 1000f,
                xPeriodicRange = 4f,
                layerDepth = 1f
            };
            totemanimsprite.CopyAppearanceFromItemId(totemname, 0);

            Game1.Multiplayer.broadcastSprites(who.currentLocation, new TemporaryAnimatedSprite[]
            {
                totemanimsprite
            });

            DelayedAction.playSoundAfterDelay("rainsound", 2000);
			return changedweather;
		}
	}
}
