using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewModdingAPI;
using Harmony;


namespace WeatherTotems
{
	public class ModEntry
		: Mod, IAssetEditor
	{
		public override void Entry(IModHelper helper)
		{
			Patches.Initialise(this.Monitor, this.Helper);
			var harmony = HarmonyInstance.Create(this.ModManifest.UniqueID);

			harmony.Patch(
				original: AccessTools.Method(typeof(StardewValley.Object), nameof(StardewValley.Object.performUseAction)),
				postfix: new HarmonyMethod(typeof(Patches), nameof(Patches.performUseAction_Postfix))
				);
			harmony.Patch(
				original: AccessTools.Method(typeof(StardewValley.Object), nameof(StardewValley.Object.isPlaceable)),
				postfix: new HarmonyMethod(typeof(Patches), nameof(Patches.isPlaceable_Postfix))
				);
		}


		public bool CanEdit<T>(IAssetInfo asset)
		{
			if (asset.AssetNameEquals("Data/ObjectInformation") || asset.AssetNameEquals("Maps/springobjects") || asset.AssetNameEquals("Data/CraftingRecipes"))
			{
				return true;
			}

			return false;
		}

		public void Edit<T>(IAssetData asset)
		{

			if (asset.AssetNameEquals("Data/ObjectInformation"))
			{
				IDictionary<int, string> data = asset.AsDictionary<int, string>().Data;

				data[932] = "Sun Totem/20/-300/Crafting/Sun Totem/Activate to greatly increase the chance for sun tomorrow. Consumed on use.";
				data[933] = "Wind Totem/20/-300/Crafting/Wind Totem/Activate to greatly increase the chance for wind tomorrow. Consumed on use.";
				data[934] = "Snow Totem/20/-300/Crafting/Snow Totem/Activate to greatly increase the chance for snow tomorrow. Consumed on use.";
				data[935] = "Thunder Totem/20/-300/Crafting/Thunder Totem/Activate to greatly increase the chance for a storm tomorrow. Consumed on use.";
			}
			else if (asset.AssetNameEquals("Maps/springobjects"))
			{
				var editor = asset.AsImage();

				Texture2D sourceImage = this.Helper.Content.Load<Texture2D>("assets/totems.png", ContentSource.ModFolder);
				editor.PatchImage(sourceImage, targetArea: new Rectangle(320, 608, 64, 16));
			}
			else if (asset.AssetNameEquals("Data/CraftingRecipes"))
            {
				IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;
				data["Sun Totem"] = "709 1 432 1 768 5/Field/932/false/Foraging 4";
				data["Wind Totem"] = "709 1 432 1 725 5/Field/933/false/Foraging 4";
				data["Snow Totem"] = "709 1 432 1 283 5/Field/934/false/Foraging 7";
				data["Thunder Totem"] = "709 1 432 1 769 5/Field/935/false/Foraging 9";
            }
		}

		public static void UseWeatherTotem(Farmer who, int totemtype, IModHelper helper)
		{
			var multiplayer = helper.Reflection.GetField<Multiplayer>(typeof(Game1), "multiplayer").GetValue();

			GameLocation.LocationContext location_context = Game1.currentLocation.GetLocationContext();
			string message = "Nothing";

			if (location_context == GameLocation.LocationContext.Default)
			{
				if (!Utility.isFestivalDay(Game1.dayOfMonth + 1, Game1.currentSeason))
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
					Game1.screenGlowOnce(Color.DarkGray, hold: false);
					break;
			}
			Game1.player.faceDirection(2);
			Game1.player.FarmerSprite.animateOnce(new FarmerSprite.AnimationFrame[1]
			{
				new FarmerSprite.AnimationFrame(57, 2000, secondaryArm: false, flip: false, Farmer.canMoveNow, behaviorAtEndOfFrame: true)
			});
			for (int i = 0; i < 6; i++)
			{
				multiplayer.broadcastSprites(who.currentLocation, new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(648, 1045, 52, 33), 9999f, 1, 999, who.Position + new Vector2(0f, -128f), flicker: false, flipped: false, 1f, 0.01f, Color.White * 0.8f, 2f, 0.01f, 0f, 0f)
				{
					motion = new Vector2((float)Game1.random.Next(-10, 11) / 10f, -2f),
					delayBeforeAnimationStart = i * 200
				});
				multiplayer.broadcastSprites(who.currentLocation, new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(648, 1045, 52, 33), 9999f, 1, 999, who.Position + new Vector2(0f, -128f), flicker: false, flipped: false, 1f, 0.01f, Color.White * 0.8f, 1f, 0.01f, 0f, 0f)
				{
					motion = new Vector2((float)Game1.random.Next(-30, -10) / 10f, -1f),
					delayBeforeAnimationStart = 100 + i * 200
				});
				multiplayer.broadcastSprites(who.currentLocation, new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(648, 1045, 52, 33), 9999f, 1, 999, who.Position + new Vector2(0f, -128f), flicker: false, flipped: false, 1f, 0.01f, Color.White * 0.8f, 1f, 0.01f, 0f, 0f)
				{
					motion = new Vector2((float)Game1.random.Next(10, 30) / 10f, -1f),
					delayBeforeAnimationStart = 200 + i * 200
				});
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
