using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewModdingAPI;
using StardewModdingAPI.Events;


namespace WeatherTotems
{
	public class ModEntry
		: Mod
	{
		public override void Entry(IModHelper helper)
		{
			WeatherTotem.Initialise(this.Helper);

			helper.Events.Input.ButtonPressed += this.ButtonPressed;
			helper.Events.Content.AssetRequested += this.AssetRequested;
		}

		private void AssetRequested(object sender, AssetRequestedEventArgs e)
        {
			// Add object data, indexes are the last four spots on the springobjects map so object can be added with a sprite
			// This means the mod is incompatible with mods that edit this area or replace the asset
			if (e.NameWithoutLocale.IsEquivalentTo("Data/ObjectInformation") == true)
			{
				e.Edit(asset =>
				{
					IDictionary<int, string> data = asset.AsDictionary<int, string>().Data;

					data[932] = "Sun Totem/20/-300/Basic/Sun Totem/Activate to greatly increase the chance for sun tomorrow. Consumed on use.";
					data[933] = "Wind Totem/20/-300/Basic/Wind Totem/Activate to greatly increase the chance for wind tomorrow. Will not work on Ginger Island. Consumed on use.";
					data[934] = "Snow Totem/20/-300/Basic/Snow Totem/Activate to greatly increase the chance for snow tomorrow. Will not work on Ginger Island. Consumed on use.";
					data[935] = "Thunder Totem/20/-300/Basic/Thunder Totem/Activate to greatly increase the chance for a storm tomorrow. Consumed on use.";
				});
				
			}
			// Add totem sprites to springobjects asset
			else if (e.NameWithoutLocale.IsEquivalentTo("Maps/springobjects") == true)
			{
				e.Edit(asset =>
				{
					var editor = asset.AsImage();

					Texture2D sourceImage = this.Helper.ModContent.Load<Texture2D>("assets/totems.png");
					editor.PatchImage(sourceImage, targetArea: new Rectangle(320, 608, 64, 16));
				});				
			}
			// Add crafting recipes for totems
			else if (e.NameWithoutLocale.IsEquivalentTo("Data/CraftingRecipes") == true)
			{
				e.Edit(asset =>
				{
					IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;
					data["Sun Totem"] = "709 1 432 1 768 5/Field/932/false/Foraging 8";
					data["Wind Totem"] = "709 1 432 1 725 5/Field/933/false/Foraging 4";
					data["Snow Totem"] = "709 1 432 1 283 5/Field/934/false/Foraging 8";
					data["Thunder Totem"] = "709 1 432 1 769 5/Field/935/false/Foraging 9";
				});
				
			}
		}

		private void ButtonPressed(object sender, ButtonPressedEventArgs e)
		{
			if (true 
				&& e.Button.IsActionButton() == true 
				&& Context.CanPlayerMove == true 
				&& Game1.player.CurrentItem != null 
				&& Game1.player.CurrentItem.parentSheetIndex.Value > 931 
				&& Game1.player.CurrentItem.parentSheetIndex.Value < 936)
			{
				// Get whether totem can change weather
				bool normal_gameplay = true 
					&& Game1.eventUp == false 
					&& Game1.isFestival() == false 
					&& Game1.fadeToBlack == false 
					&& Game1.player.swimming == false 
					&& Game1.player.bathingClothes == false 
					&& Game1.player.onBridge.Value == false;

				// Is the item used one of the weather totems?
				if (Game1.player.CurrentItem.Name != null && Game1.player.CurrentItem.Name.Contains("Totem") == true)
				{
					// Yes, can the totem update tomorrows weather?

					if (normal_gameplay == true)
					{
						// Yes, which totem is it?
						// Execute method with arguments based on the totem type
						switch (Game1.player.CurrentItem.parentSheetIndex.Value)
						{
							case 932:
								WeatherTotem.UseWeatherTotem(Game1.player, 932);
								this.Monitor.Log("Weather set to sunny tomorrow", LogLevel.Trace);
								break;
							case 933:
								WeatherTotem.UseWeatherTotem(Game1.player, 933);
								this.Monitor.Log("Weather set to windy tomorrow", LogLevel.Trace);
								break;
							case 934:
								WeatherTotem.UseWeatherTotem(Game1.player, 934);
								this.Monitor.Log("Weather set to snowy tomorrow", LogLevel.Trace);
								break;
							case 935:
								WeatherTotem.UseWeatherTotem(Game1.player, 935);
								this.Monitor.Log("Weather set to stormy tomorrow", LogLevel.Trace);
								break;
							default:
								break;
						}

						Game1.player.removeFirstOfThisItemFromInventory(Game1.player.CurrentItem.parentSheetIndex.Value);

					}
				}
			}
		}
	}	
}
