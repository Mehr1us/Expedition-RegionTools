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
            IL.ModManager.ModMerger.PendingApply.ApplyMerges += addRandomStartsMerge;
        }


        private static void addRandomStartsMerge(ILContext il)
        {
            Plugin.LogDebug("hooking started");
            ILCursor c = new(il);
            // finds last if block in method
            if (!c.TryGotoNext(
                    x => x.MatchLdloc(0),
                    x => x.MatchLdstr("strings.txt"),
                    x => x.MatchCallvirt<String>("Contains")
                )
            )
            {
                Plugin.LogDebug("!FAILED! to IL Hook ModManager.ModMerger.PendingApply.ApplyMerges");
                Plugin.LogDebug(il.ToString());
                return;
            }
            else
            {
                c.Index++;
                c.Emit(OpCodes.Ldstr, "randomstarts.txt");
                c.Emit(OpCodes.Callvirt, Retrieve.getMethod(typeof(String), "Contains"));
                ILLabel ifLabel = c.MarkLabel();
                c.Emit(OpCodes.Ldarg_1);
                c.Emit(OpCodes.Ldarg_3);
                c.Emit(OpCodes.Ldarg_0);
                c.Emit(OpCodes.Ldfld, Retrieve.getField(typeof(ModManager.ModMerger.PendingApply), "mergeLines"));
                c.Emit(OpCodes.Call, Retrieve.getStaticMethod(typeof(ModManagerHooks), "MergeRandomStarts"));
                c.Emit(OpCodes.Ret);
                ILLabel endLabel = c.MarkLabel();
                c.Emit(OpCodes.Ldloc_0);
                c.GotoLabel(ifLabel);
                c.Emit(OpCodes.Brfalse_S, endLabel);

                Plugin.LogDebug("ModManager.ModMerger.PendingApply.ApplyMerges hook applied");
            }
        }

        private static void MergeRandomStarts(ModManager.Mod mod, string sourcePath, List<string> mergeLines)
        {
            Plugin.LogDebug("merging randomstarts.txt");
            string[] randomStartsFile = File.ReadAllLines(sourcePath);
            List<string> input = randomStartsFile.ToList();
            input.AddRange(mergeLines);
            ModManager.ModMerger.WriteMergedFile(mod, sourcePath, input.ToArray());
        }
    }
}