using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Objects;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley.GameData.Objects;

namespace WeatherTotems
{
	public class ModEntry
		: Mod
	{
		private static bool success = false;

		private static List<string> totems = new List<string>() { "TheMightyAmondee.WeatherTotemsCP_SunTotem", "TheMightyAmondee.WeatherTotemsCP_WindTotem", "TheMightyAmondee.WeatherTotemsCP_SnowTotem", "TheMightyAmondee.WeatherTotemsCP_ThunderTotem", "TheMightyAmondee.WeatherTotemsCP_GreenRainTotem" };
		public override void Entry(IModHelper helper)
		{
			WeatherTotem.Initialise(this.Helper);
			i18n.gethelpers(helper.Translation);

			helper.Events.Input.ButtonPressed += this.ButtonPressed;
			helper.Events.Content.AssetRequested += this.AssetRequested;
		}

		private void AssetRequested(object sender, AssetRequestedEventArgs e)
        {

			// Add crafting recipes for totems
			if (e.NameWithoutLocale.IsEquivalentTo("Data/CraftingRecipes") == true)
			{
				e.Edit(asset =>
				{
					IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;
					data["Sun Totem"] = "709 1 432 1 768 5/Field/TheMightyAmondee.WeatherTotemsCP_SunTotem/false/Foraging 8";
					data["Wind Totem"] = "709 1 432 1 725 5/Field/TheMightyAmondee.WeatherTotemsCP_WindTotem/false/Foraging 4";
					data["Snow Totem"] = "709 1 432 1 283 5/Field/TheMightyAmondee.WeatherTotemsCP_SnowTotem/false/Foraging 8";
					data["Thunder Totem"] = "709 1 432 1 769 5/Field/TheMightyAmondee.WeatherTotemsCP_ThunderTotem/false/Foraging 9";
                    data["Green Rain Totem"] = "709 1 432 1 Moss 50/Field/TheMightyAmondee.WeatherTotemsCP_GreenRainTotem/false/Foraging 9";
                });
				
			}
		}

		private void ButtonPressed(object sender, ButtonPressedEventArgs e)
		{
			if (true 
				&& e.Button.IsActionButton() == true 
				&& Context.CanPlayerMove == true 
				&& Game1.player.CurrentItem != null 
				&& totems.Contains(Game1.player.CurrentItem.ItemId) == true)
			{
				// Get whether totem can change weather
				bool normal_gameplay = true 
					&& Game1.eventUp == false 
					&& Game1.isFestival() == false 
					&& Game1.fadeToBlack == false 
					&& Game1.player.swimming.Value == false 
					&& Game1.player.bathingClothes.Value == false 
					&& Game1.player.onBridge.Value == false;

				// Is the item used one of the weather totems?
				if (Game1.player.CurrentItem.Name != null)
				{
					// Yes, can the totem update tomorrows weather?

					if (normal_gameplay == true)
					{
						success = false;
						// Yes, which totem is it?
						// Execute method with arguments based on the totem type
						switch (Game1.player.CurrentItem.ItemId)
						{
							case "TheMightyAmondee.WeatherTotemsCP_SunTotem":
								success = WeatherTotem.UseWeatherTotem(Game1.player, 0);
								this.Monitor.Log("Try set to sunny weather tomorrow", LogLevel.Trace);
								break;
							case "TheMightyAmondee.WeatherTotemsCP_WindTotem":
                                success = WeatherTotem.UseWeatherTotem(Game1.player, 1);
								this.Monitor.Log("Try set to windy weather tomorrow", LogLevel.Trace);
								break;
							case "TheMightyAmondee.WeatherTotemsCP_SnowTotem":
								success = WeatherTotem.UseWeatherTotem(Game1.player, 2);
								this.Monitor.Log("Try set to snowy weather tomorrow", LogLevel.Trace);
								break;
							case "TheMightyAmondee.WeatherTotemsCP_ThunderTotem":
								success = WeatherTotem.UseWeatherTotem(Game1.player, 3);
								this.Monitor.Log("Try set to stormy weather tomorrow", LogLevel.Trace);
								break;
                            case "TheMightyAmondee.WeatherTotemsCP_GreenRainTotem":
                                success = WeatherTotem.UseWeatherTotem(Game1.player, 4);
                                this.Monitor.Log("Try set to green rain weather tomorrow", LogLevel.Trace);
                                break;
                            default:
								break;
						}

						if (success == true)
						{
                            Game1.player.removeFirstOfThisItemFromInventory(Game1.player.CurrentItem.ItemId);
                            this.Monitor.Log("Weather set successfully", LogLevel.Trace);
                        }
						else
						{
							var hudmessage = new HUDMessage(i18n.string_Error(), 3);
                            this.Monitor.Log("Failed to set weather", LogLevel.Trace);
                            Game1.addHUDMessage(hudmessage);
						}

					}
				}
			}
		}
	}	
}
