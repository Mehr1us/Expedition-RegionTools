using BepInEx;
using System.Security.Permissions;
using BepInEx.Logging;
using System.Security;
using UnityEngine;
using System;

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

        public void OnEnable()
        {
            logger = base.Logger;
            LogDebug("mehr1us.expedition is now active");
            ModManagerHooks.Apply();

            On.RainWorld.OnModsInit += RainWorld_OnModsInit;
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