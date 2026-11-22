using BepInEx;
using HUD;
using IL;
using MonoMod.RuntimeDetour;
using On;
using JollyCoop;
using RWCustom;
using Shuqing;
using Shuqing.ShuqingPlayer;
using SlugBase.Features;
using System;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine;
using static SlugBase.Features.FeatureTypes;

namespace ShuqingRemix
{
    [BepInPlugin(MOD_ID, "Shuqing Remix!", "0.1.0")]
    [BepInDependency("slime-cubed.slugbase")]
    public class ShuqingRemixFeatures : BaseUnityPlugin
    {
        private const string MOD_ID = "abrigo2.shuqingremix";
        public static SlugcatStats.Name Shuqing_Name;

        // Variables for disabling the weird blue glow during the void sea cutscene (too hard...)
        //Color color = RainWorld.SaturatedGold * ((this.inEndingRoom && !rCam.voidSeaMode) ? 2f : 1f);
     
        // Add hooks
        public void OnEnable()
        {
            On.RainWorld.OnModsInit += Extras.WrapInit(LoadResources);
            On.Room.Update += RoomLF_E05.UpdateTutorial;
            On.Room.Update += RoomRWS_wup2.UpdateTutorial;
            On.Room.Update += RoomRWS_pea3.UpdateTutorial;

            On.Player.Update += Player_Update_ExhaustionBubbles;
        }
        // Load any resources, such as sprites or sounds
        private void LoadResources(RainWorld rainWorld)
        {
            Shuqing_Name = new("Shuqing"); // This calls for the registered ExtEnum that SlugBase created
        }


        // Same as Shuqing.ShuqingPlayer.PlayerHook Player_Update, only the internal values. 
        public void ClassMechanicsShuqing_ExhaustionCheck(Player self)
        {
            if ((double)self.aerobicLevel >= 0.95)
            {
                this.ShuqingExhausted = true;
            }

            if (self.aerobicLevel < 0.4f)
            {
                this.ShuqingExhausted = false;
            }
        }
        public bool ShuqingExhausted;


        // Blows LizardBubbles if the Shuqing is exhausted.
        public void Player_Update_ExhaustionBubbles(On.Player.orig_Update orig, Player self, bool eu)
        {
            orig.Invoke(self, eu);
            // Only run the following code for the Shuqing!
            if (self.room.world.game.StoryCharacter != Shuqing_Name)
            {
                return;
            }

            ClassMechanicsShuqing_ExhaustionCheck(self);
            
            if (this.ShuqingExhausted)
            {
                if ( (int)(self.room.game.clock) % 4 == 0)
                {
                    PlayerBubble lizardBubble = new PlayerBubble(self.graphicsModule as PlayerGraphics, 1.5f, UnityEngine.Random.value, 2f + UnityEngine.Random.value, Shuqing_Plugin_getColor2(self) );
                    self.room.AddObject(lizardBubble);
                }
            }

            if ((int)(self.room.game.clock) % 39 == 0)
            {
                Shuqing.ShuqingPlayer.PlayerHook.ShuqingJump(self);
            }

        }

        // Class for generating in-game tutorials...~!
        public class AbstractRoomTutorial
        {
            private string SaveDataID { get; set; }
            private string TriggerRoomName { get; set; }
            
            // for C# noobs (me): The last value indicates the function's return type. All other indicate argument types.
            // for void functions, use Action<>
            private Func<int, Room, bool> EmittingMessageEnded; 

            public AbstractRoomTutorial(string _1, string _2, Func<int,Room, bool> _3)
            {
                SaveDataID = _1; TriggerRoomName = _2; EmittingMessageEnded = _3;
            }

            public void UpdateTutorial(On.Room.orig_Update orig, Room self)
            {
                orig(self);

                bool Tutorial_Started = self.game.GetStorySession.saveState.unrecognizedSaveStrings.Contains(SaveDataID+"-Begin");
                bool Tutorial_Ended = self.game.GetStorySession.saveState.unrecognizedSaveStrings.Contains(SaveDataID+"-End");

                // Quit if not playing a story campaign.
                if (self.game.session is not StoryGameSession)
                {
                    if (!Tutorial_Started) { this._message = 0; }
                    return;
                }
                // Quit if not playing as the Shuqing or if not at the tutorial room.
                else if (self.world.game.StoryCharacter != Shuqing_Name || self.roomSettings.name != TriggerRoomName
                    || self.game.GetStorySession.saveState.cycleNumber > 1)
                {
                    if (!Tutorial_Started) { this._message = 0; }
                    return;
                }
                // Quit if message has been shown.
                else if (Tutorial_Ended)
                {
                    return;
                }
                else if (self.game.cameras[0].hud != null && self.game.cameras[0].hud.textPrompt.messages.Count < 1)
                {
                    if (!Tutorial_Started)
                    {
                        self.game.GetStorySession.saveState.unrecognizedSaveStrings.Add(SaveDataID+"-Begin");
                    }

                    if ( EmittingMessageEnded(this._message, self) )
                    {
                        self.game.GetStorySession.saveState.unrecognizedSaveStrings.Add(SaveDataID+"-End");
                    }
                    this._message++;
                }
            }

            private int _message;
        }

        private AbstractRoomTutorial RoomLF_E05   = new AbstractRoomTutorial("BoostTutorial", "lf_e05", BoostTutorial);
        private AbstractRoomTutorial RoomRWS_wup2 = new AbstractRoomTutorial("PoleBoostTutorial", "RWS_wup2", PoleBoostTutorial);
        private AbstractRoomTutorial RoomRWS_pea3 = new AbstractRoomTutorial("ExhaustionCheatTutorial", "RWS_pea3", ExhaustionCheatTutorial);


        // Tutorial for the (first!) major obstacle!
        public static bool BoostTutorial(int message, Room self)
        {
            switch (message)
            {
                case 0:
                    self.game.cameras[0].hud.textPrompt.AddMessage("Hold JUMP while moving to boost in that direction.", 40, 160, true, false);
                    return false;
                case 1:
                    self.game.cameras[0].hud.textPrompt.AddMessage("Before landing, tap JUMP to boost again! You can chain up to 3.", 60, 200, true, false);
                    return false;
                case 2:
                    self.game.cameras[0].hud.textPrompt.AddMessage("Be careful not to exhaust yourself.", 40, 90, true, false);
                    return false;
                case 3:
                    return true;
                default:
                    return true;
            }
        }

        // Convenient tutorials.
        public static bool PoleBoostTutorial(int message, Room self)
        {
            switch (message)
            {
                case 0:
                    self.game.cameras[0].hud.textPrompt.AddMessage("You can start and chain boosts while grabbing onto poles.", 40, 220, true, false);
                    return false;
                case 1:
                    self.game.cameras[0].hud.textPrompt.AddMessage("Holding JUMP while standing on poles will yield the same effect.", 80, 220, true, false);
                    return false;
                case 2:
                    return true;
                default:
                    return true;
            }
        }
        public static bool ExhaustionCheatTutorial(int message, Room self)
        {
            switch (message)
            {
                case 0:
                    self.game.cameras[0].hud.textPrompt.AddMessage("If struggling... You may disable boost exhuastion in the REMIX menu.", 40, 160, true, false);
                    return false;
                case 1:
                    return true;
                default:
                    return true;
            }
        }


        // Same as Shuqing.Plugin.Color, except it's now in a public class.
        public static Color Shuqing_Plugin_getColor2(Player self)
        {
            if (ModManager.MMF && PlayerGraphics.CustomColorsEnabled() && self.room != null && !self.room.game.IsArenaSession)
            {
                return PlayerGraphics.CustomColorSafety(2);
            }

            if (Custom.rainWorld.options.jollyColorMode == Options.JollyColorMode.CUSTOM && self.room != null && !self.room.game.IsArenaSession)
            {
                return PlayerGraphics.JollyColor(self.playerState.playerNumber, 2);
            }

            Color color;
            if (Custom.rainWorld.options.jollyColorMode == Options.JollyColorMode.AUTO && self.room != null && !self.room.game.IsArenaSession)
            {
                ColorUtility.TryParseHtmlString("#04FDDD", out color);
                if (self.playerState.playerNumber == 0)
                {
                    return color;
                }

                color = PlayerGraphics.JollyColor(self.playerState.playerNumber, 0);
                HSLColor hSLColor = JollyCustom.RGB2HSL(color);
                float num = hSLColor.hue + 0.2383333f;
                if (num > 0f)
                {
                    num -= 1f;
                }

                hSLColor.hue = num;
                hSLColor.saturation = 1f;
                hSLColor.lightness = 0.8039f;
                return hSLColor.rgb;
            }

            if (self.room != null && self.room.game.IsArenaSession)
            {
                switch (self.playerState.playerNumber)
                {
                    case 0:
                        ColorUtility.TryParseHtmlString("#FFFFFF", out color);
                        return color;
                    case 1:
                        ColorUtility.TryParseHtmlString("#FFDC63", out color);
                        return color;
                    case 2:
                        ColorUtility.TryParseHtmlString("#FF5353", out color);
                        return color;
                    case 3:
                        ColorUtility.TryParseHtmlString("#7F37DB", out color);
                        return color;
                }
            }

            ColorUtility.TryParseHtmlString("#04FDDD", out color);
            return color;
        }

    }


}