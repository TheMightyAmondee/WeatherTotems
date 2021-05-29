using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using StardewValley.Objects;
using StardewValley;
using StardewModdingAPI;
using Harmony;

namespace WeatherTotems
{
    public class Patches
    {
        private static IModHelper helper;
        private static IMonitor monitor;
        public static void Initialise(IMonitor monitor, IModHelper helper)
        {
            Patches.monitor = monitor;
            Patches.helper = helper;
        }
        public static void performUseAction_Postfix(StardewValley.Object __instance, GameLocation location, ref bool __result)
        {
            try
            {
                string[] totemname = __instance.name.Split(' ');
                
                bool normal_gameplay = !Game1.eventUp && !Game1.isFestival() && !Game1.fadeToBlack && !Game1.player.swimming && !Game1.player.bathingClothes && !Game1.player.onBridge.Value;

                if (__instance.name != null && __instance.name.Contains("Totem") && __instance.parentSheetIndex > 931)
                {
                    if (normal_gameplay)
                    {
                        switch (totemname[0])
                        {
                            case "Sun":
                                ModEntry.UseWeatherTotem(Game1.player, 932, helper);
                                break;
                            case "Wind":
                                ModEntry.UseWeatherTotem(Game1.player, 933, helper);
								break;
                            case "Snow":
                                ModEntry.UseWeatherTotem(Game1.player, 934, helper);
								break;
                            case "Thunder":
                                ModEntry.UseWeatherTotem(Game1.player, 935, helper);
                                break;
                            default:
                                break;
                        }
                        __result = true;
                        Game1.player.removeFirstOfThisItemFromInventory(__instance.parentSheetIndex);
                    }
                }             
            }
            catch (Exception ex)
            {
                monitor.Log($"Failed in {nameof(performUseAction_Postfix)}:\n{ex}", LogLevel.Error);
            }
        }

        public static void isPlaceable_Postfix(StardewValley.Object __instance, ref bool __result)
        {
            
            if(__result == true && ((Utility.IsNormalObjectAtParentSheetIndex(__instance, 932) || Utility.IsNormalObjectAtParentSheetIndex(__instance, 933) || Utility.IsNormalObjectAtParentSheetIndex(__instance, 934) || Utility.IsNormalObjectAtParentSheetIndex(__instance, 935))))
            {
                __result = false;
            }
        }
    }
}
