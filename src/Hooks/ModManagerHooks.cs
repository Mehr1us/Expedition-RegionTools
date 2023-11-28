using MonoMod.Cil;
using Mono.Cecil.Cil;
using System.IO;
using System.Reflection;
using System.Text;
using RWCustom;
using UnityEngine;
using MonoMod.RuntimeDetour;
using mehr1us;
using BepInEx.Logging;
using System.Collections.Generic;
using System.Linq;
using System;
using MonoMod.Utils;

namespace mehr1us.expedition
{
    public static class ModManagerHooks
    {
        public static void Apply()
        {
            Plugin.LogDebug("ModManager hooking started");
            IL.ModManager.ModMerger.PendingApply.ApplyMerges += AddRandomStartsMerge;
        }


        private static void AddRandomStartsMerge(ILContext il)
        {
            ILCursor c = new(il);
            if (!c.TryGotoNext(
                    x => x.MatchLdloc(0),
                    x => x.MatchLdstr("strings.txt"),
                    x => x.MatchCallvirt<String>("Contains")
                )
            )
            {
                Plugin.LogDebug("!FAILED! to IL Hook ModManager.ModMerger.PendingApply.ApplyMerges");
                return;
            }
            else
            {
                c.Index++;
                c.Emit(OpCodes.Ldstr, "randomstarts.txt");
                c.Emit(OpCodes.Callvirt, Retrieve.GetMethod(typeof(String), "Contains"));
                ILLabel ifLabel = c.MarkLabel();
                c.Emit(OpCodes.Ldarg_1);
                c.Emit(OpCodes.Ldarg_3);
                c.Emit(OpCodes.Ldarg_0);
                c.Emit(OpCodes.Ldfld, Retrieve.GetField(typeof(ModManager.ModMerger.PendingApply), "mergeLines"));
                c.Emit(OpCodes.Call, Retrieve.GetStaticMethod(typeof(ModManagerHooks), "MergeRandomStarts"));
                c.Emit(OpCodes.Ret);
                ILLabel endLabel = c.MarkLabel();
                c.Emit(OpCodes.Ldloc_0);
                c.GotoLabel(ifLabel);
                c.Emit(OpCodes.Brfalse_S, endLabel);

                Plugin.LogDebug("ModManager.ModMerger.PendingApply.ApplyMerges hook applied");
            }
        }

        #pragma warning disable IDE0051 //it is being used via GetMethod retieval
        private static void MergeRandomStarts(ModManager.Mod mod, string sourcePath, List<string> mergeLines)
        {
            Plugin.LogDebug("merging randomstarts.txt");
            string[] randomStartsFile = File.ReadAllLines(sourcePath);
            for (; mergeLines.Contains("REGIONS") && mergeLines.Contains("END REGIONS");)
            {
                int regionsStart = -1;
                int regionsEnd = -1;
                for (int i = 0; i < mergeLines.Count; i++)
                {
                    if (mergeLines[i].Trim().Equals("REGIONS")) regionsStart = i;
                    if (mergeLines[i].Trim().Equals("END REGIONS")) regionsEnd = i;
                }
                if (regionsStart == -1 || regionsEnd == -1)
                {
                    Plugin.LogError("found no REGIONS or END REGIONS in modify/randomstarts.txt");
                }
                else if (regionsStart >= regionsEnd)
                {
                    Plugin.LogError("found END REGIONS before or at REGIONS in modify/randomstarts.txt");
                }
                else
                {
                    List<string> regions = mergeLines.GetRange(regionsStart, regionsEnd);
                    mergeLines.RemoveRange(regionsStart, regionsEnd);
                    Plugin.customRegionStarts.MergeRegionLines(regions);
                    Plugin.hasCustomRegionStarts = true;
                }
            }
            List<string> input = randomStartsFile.ToList();
            input.AddRange(mergeLines);
            ModManager.ModMerger.WriteMergedFile(mod, sourcePath, input.ToArray());
        }
    }
}