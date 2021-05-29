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
			if (asset.AssetNameEquals("Data/ObjectInformation") || asset.AssetNameEquals("Maps/springobjects"))
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
		}

		public static void UseWeatherTotem(Farmer who, int totemtype, IModHelper helper)
		{
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
				Game1.netWorldState.Value.GetWeatherForLocation(location_context).weatherForTomorrow.Value = 1;
				Game1.pauseThenMessage(2000, Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.12822"), showProgressBar: false);
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
			
			DelayedAction.playSoundAfterDelay("rainsound", 2000);
		}
	}		
}
