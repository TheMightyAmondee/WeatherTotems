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
	// Inherit from Game1 to have access to the protected multiplayer field
    public class Patches
		: Game1
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

                if (__instance.name != null && __instance.name.Contains("Totem") && __instance.parentSheetIndex.Value > 931)
                {
                    if (normal_gameplay)
                    {
                        switch (__instance.parentSheetIndex.Value)
                        {
                            case 932:
                                UseWeatherTotem(Game1.player, 932, helper);
                                break;
                            case 933:
                                UseWeatherTotem(Game1.player, 933, helper);
								break;
                            case 934:
                                UseWeatherTotem(Game1.player, 934, helper);
								break;
                            case 935:
                                UseWeatherTotem(Game1.player, 935, helper);
                                break;
                            default:
                                break;
                        }
                        __result = true;
                        Game1.player.removeFirstOfThisItemFromInventory(__instance.parentSheetIndex.Value);
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
