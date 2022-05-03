using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewModdingAPI;

namespace WeatherTotems
{
	// Inherit from Game1 to have access to the protected multiplayer field
    public class WeatherTotem
		: Game1
    {
        private static IModHelper helper;

		// Get access to the required SMAPI apis
        public static void Initialise( IModHelper helper)
        {
            WeatherTotem.helper = helper;
        }

		public static void UseWeatherTotem(Farmer who, int totemtype)
		{
			// Get location context, (Main area or Ginger Island)
			GameLocation.LocationContext location_context = Game1.currentLocation.GetLocationContext();
			string message = "Nothing";
			
			// Get asset key for animation sprites
			string assetkey = helper.ModContent.GetInternalAssetName("assets/loosesprites.png").Name;

			// Change weather based on totem type
			if (location_context == GameLocation.LocationContext.Default)
			{
				if (Utility.isFestivalDay(Game1.dayOfMonth + 1, Game1.currentSeason) == false)
				{
					switch (totemtype)
					{
						case 932:
							Game1.netWorldState.Value.WeatherForTomorrow = (Game1.weatherForTomorrow = 0);
							message = "Clouds vanish from the horizon...";
							break;
						case 933:
							Game1.netWorldState.Value.WeatherForTomorrow = (Game1.weatherForTomorrow = 2);
							message = "A gentle breeze passes by...";
							break;
						case 934:
							Game1.netWorldState.Value.WeatherForTomorrow = (Game1.weatherForTomorrow = 5);
							message = "The air gets colder around you...";
							break;
						case 935:
							Game1.netWorldState.Value.WeatherForTomorrow = (Game1.weatherForTomorrow = 3);
							message = "The crackle of electricity fills the air...";
							break;
					}

					Game1.pauseThenMessage(2000, message, showProgressBar: false);
				}
			}
			else
			{
				switch (totemtype)
				{
					case 932:
						Game1.netWorldState.Value.GetWeatherForLocation(location_context).weatherForTomorrow.Value = 0;
						message = "Clouds vanish from the horizon...";
						break;
					case 933:
						Game1.netWorldState.Value.GetWeatherForLocation(location_context).weatherForTomorrow.Value = 2;
						message = "A gentle breeze passes by...";
						break;
					case 934:
						Game1.netWorldState.Value.GetWeatherForLocation(location_context).weatherForTomorrow.Value = 5;
						message = "The air gets colder around you...";
						break;
					case 935:
						Game1.netWorldState.Value.GetWeatherForLocation(location_context).weatherForTomorrow.Value = 3;
						message = "The crackle of electricity fills the air...";
						break;
				}

				Game1.pauseThenMessage(2000, message, showProgressBar: false);
			}

			// Totem activation stuff
			Game1.screenGlow = false;
			who.currentLocation.playSound("thunder");
			who.canMove = false;
			switch (totemtype)
			{
				case 932:
					Game1.screenGlowOnce(Color.Gold, hold: false);
					break;
				case 933:
					Game1.screenGlowOnce(Color.Lavender, hold: false);
					break;
				case 934:
					Game1.screenGlowOnce(Color.AliceBlue, hold: false);
					break;
				case 935:
					Game1.screenGlowOnce(Color.DarkSlateBlue, hold: false);
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
				void showsprites(string asset, Rectangle area)
                {
					multiplayer.broadcastSprites(who.currentLocation, new TemporaryAnimatedSprite(asset, area, 9999f, 1, 999, who.Position + new Vector2(0f, -128f), flicker: false, flipped: false, 1f, 0.01f, Color.White * 0.8f, 2f, 0.01f, 0f, 0f)
					{
						motion = new Vector2((float)Game1.random.Next(-10, 11) / 10f, -2f),
						delayBeforeAnimationStart = i * 200
					});
				}

				void showspritefromtotemtype(Rectangle whichareaforthunderanimation)
                {
					switch (totemtype)
					{
						case 932:
							showsprites(assetkey, new Microsoft.Xna.Framework.Rectangle(0, 0, 24, 24));
							break;
						case 933:
							showsprites(assetkey, new Microsoft.Xna.Framework.Rectangle(24, 0, 51, 24));
							break;
						case 934:
							showsprites(assetkey, new Microsoft.Xna.Framework.Rectangle(75, 0, 25, 25));
							break;
						case 935:
							showsprites("LooseSprites\\Cursors", whichareaforthunderanimation);
							break;
					}
				}

				showspritefromtotemtype(new Microsoft.Xna.Framework.Rectangle(645, 1079, 36, 56));
				showspritefromtotemtype(new Microsoft.Xna.Framework.Rectangle(648, 1045, 52, 33));
				showspritefromtotemtype(new Microsoft.Xna.Framework.Rectangle(648, 1045, 52, 33));				
			}

			multiplayer.broadcastSprites(who.currentLocation, new TemporaryAnimatedSprite(totemtype, 9999f, 1, 999, Game1.player.Position + new Vector2(0f, -96f), flicker: false, flipped: false, verticalFlipped: false, 0f)
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
			});

			DelayedAction.playSoundAfterDelay("rainsound", 2000);
		}
	}
}
