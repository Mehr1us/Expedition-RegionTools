using BepInEx;
using System.Security.Permissions;
using BepInEx.Logging;
using System.Security;
using UnityEngine;
using System;
using System.Collections.Generic;

#pragma warning disable CS0618 // Type or member is obsolete
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]
[module: UnverifiableCode]
#pragma warning restore CS0618 // Type or member is obsolete

namespace mehr1us.expedition
{
    [BepInPlugin(PLUGIN_GUID, "Expedition RegionTools", "0.1")]
    public class Plugin : BaseUnityPlugin
    {
        public const string PLUGIN_GUID = "mehr1us.expedition";
        public static ManualLogSource logger;
        private static bool initialized = false;
        private static bool initialized2 = false;
        public static CustomRegionStarts customRegionStarts = new();
        public static bool hasCustomRegionStarts = false;

        public void OnEnable()
        {
            logger = base.Logger;
            LogDebug("mehr1us.expedition is now active");
            ModManagerHooks.Apply();
            On.PlayerGraphics.ctor += PlayerGraphics_ctor;
            On.HUD.HUD.InitSinglePlayerHud += HUD_logging;

            On.RainWorld.OnModsInit += RainWorld_OnModsInit;
            On.RainWorld.PostModsInit += RainWorld_PostModsInit;
        }

        private void PlayerGraphics_ctor(On.PlayerGraphics.orig_ctor orig, PlayerGraphics self, PhysicalObject ow)
        {
            try
            {
                Player p = (Player)ow;
                LogDebug("player" + isNull(p));
                if (p != null) LogDebug("player.npcCharacterStats" + isNull(p.npcCharacterStats));
                if (p.npcCharacterStats != null) LogDebug("player.npcCharacterStats.malnourished" + isNull(p.npcCharacterStats.malnourished));
            } catch (Exception e) { LogDebug(e.ToString()); }
            orig(self, ow);
            LogDebug("PlayerGraphics_ctor passed");
        }

        private string isNull(object o)
        {
            if (o == null) return " is null";
            return " isn't null";
        }
        private void HUD_logging(On.HUD.HUD.orig_InitSinglePlayerHud orig, HUD.HUD self, RoomCamera cam)
        {
            try
            {
                LogDebug("cam.room" + isNull(cam.room));
                LogDebug("cam.room.world" + isNull(cam.room.world));
                LogDebug("cam.room.game" + isNull(cam.room.game));
                LogDebug("cam.room.game.rainWorld" + isNull(cam.room.game.rainWorld));
                LogDebug("cam.room.game.session" + isNull(cam.room.game.session));
                LogDebug("cam.room.game.session.Players" + isNull(cam.room.game.session.Players));
                LogDebug("cam.InCutscene" + isNull(cam.InCutscene));
                LogDebug("foreach cam.room.game.session.Players:");
                foreach (AbstractCreature player in cam.room.game.session.Players)
                {
                    LogDebug(isNull(player));

                }
                LogDebug("cam.room.abstractRoom.shelter" + isNull(cam.room.abstractRoom.shelter));
            }
            catch (Exception e) { LogDebug(e.ToString()); }
            orig(self, cam);
        }

        private void RainWorld_PostModsInit(On.RainWorld.orig_PostModsInit orig, RainWorld self)
        {
            orig(self);

            if (!initialized2)
            {
                try
                {
                    ExpeditionGameHooks.Apply();
                    initialized2 = true;
                }
                catch (Exception e)
                {
                    Debug.Log($"[Expedition RegionTools]  Exception at RainWorld.PostModsInit:\n{e}");
                }
            }
        }

        private void RainWorld_OnModsInit(On.RainWorld.orig_OnModsInit orig, RainWorld self)
        {
            orig(self);

            if (!initialized)
            {
                try
                {
                    initialized = true;
                }
                catch (Exception e)
                {
                    Debug.Log($"[Expedition RegionTools]  Exception at RainWorld.OnModsInit:\n{e}");
                }
            }
        }

        public static void LogDebug(string message)
        {
            logger.LogDebug(message);
        }

        public static void LogError(string message)
        {
            logger.LogError(message);
        }
    }
}