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

		
	}		
}
