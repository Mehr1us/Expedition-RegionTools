using Mono.Cecil;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using On.Expedition;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace mehr1us.expedition
{
    public static class ExpeditionGameHooks
    {
        private static bool init1 = false;
        private static bool init2 = false;
        public static void Apply()
        {
            if (Plugin.customRegionStarts != null)
            {
                Plugin.LogDebug("hooking Expedition.ExpeditionGame");
                if (!init1)
                {
                    On.Expedition.ExpeditionGame.GetRegionWeight += GetCustomRegionWeight;
                    init1 = true;
                }
                if (!init2) { 
                    IL.Expedition.ExpeditionGame.ExpeditionRandomStarts += AddCustomRandomStarts;
                    On.Expedition.ExpeditionGame.ExpeditionRandomStarts += AddLoggingToRandomStarts; 
                }
            }
            else
            {
                Plugin.LogError("mehr1us.expedition.CustomRegionStarts regionStarts is null in Plugin when initilising ExpeditionGameHooks");
            }
        }

        private static string AddLoggingToRandomStarts(ExpeditionGame.orig_ExpeditionRandomStarts orig, RainWorld rainWorld, SlugcatStats.Name slug)
        {
            string regionNames = "";
            foreach (string region in rainWorld.progression.regionNames)
            {
                regionNames += region + "\t";
            }
            Plugin.LogDebug("ExpeditionRandomStarts stated running");
            //"\nregionNames[" + rainWorld.progression.regionNames.Length + "] = " + regionNames);
            if (rainWorld.progression.regionNames.Length < 1)
            {
                rainWorld.progression.ReloadRegionsList();
            }
            string ret = orig(rainWorld, slug);
            Plugin.LogDebug("ExpeditionRandomStarts returned \"" + ret + "\"");
            return ret;
        }

        private static void AddCustomRandomStarts(ILContext il)
        {
            ILCursor c = new(il);
            ILCursor c2 = new(il);
            ILCursor c3 = new(il);
            ILCursor c4 = new(il);
            ILCursor c5 = new(il); 
            //125   0191    ldloc.1
            //126   0192    ldloc.s V_11(11)
            //127   0194    callvirt instance !1 class [netstandard] System.Collections.Generic.Dictionary`2<string, class [netstandard] System.Collections.Generic.List`1<string>>::get_Item(!0)
            //128	0199	ldloc.s V_4(4)
            //129	019B    ldloc.s V_10 (10)
            //130	019D	ldelem.ref
            //131	019E	callvirt instance bool class [netstandard] System.Collections.Generic.List`1<string>::Contains(!0)
            //132	01A3    brfalse.s   142 (01BE) ldloc.s V_10(10)
            if (!(c.TryGotoNext(
                    x => x.Match(OpCodes.Br),
                    x => x.MatchLdsfld(Retrieve.GetStaticField(typeof(ModManager), "MSC")),
                    x => x.Match(OpCodes.Brfalse)) &&
                c2.TryGotoNext(
                    x => x.MatchLdloc(2),
                    x => x.MatchLdloc(11),
                    x => x.Match(OpCodes.Call),
                    x => x.Match(OpCodes.Brfalse_S)) && 
                c3.TryGotoNext(
                    x => x.MatchLdsfld(Retrieve.GetStaticField(typeof(Expedition.ExpeditionGame), "lastRandomRegion")),
                    x => x.MatchLdloc(11),
                    x => x.MatchCall(typeof(string).GetMethods().Single(x => x.Name == "op_Equality")),
                    x => x.Match(OpCodes.Brtrue) &&
                c4.TryGotoNext(
                    x => x.MatchLdloc(10),
                    x => x.MatchLdcI4(1),
                    x => x.MatchAdd(),
                    x => x.MatchStloc(10)) &&
                c5.TryGotoNext(
                    x => x.MatchLdloc(1),
                    x => x.MatchLdloc(11),
                    x => x.Match(OpCodes.Callvirt),
                    x => x.MatchLdloc(4),
                    x => x.MatchLdloc(10),
                    x => x.MatchLdelemRef(),
                    x => x.Match(OpCodes.Callvirt),
                    x => x.Match(OpCodes.Brfalse_S)
                ))))
            {
                Plugin.LogDebug("!FAILED! to IL Hook Expedition.ExpeditionGame.ExpeditionRandomStarts");
                return;
            }
            else
            {
                try
                {
                    c.Index++;
                    c2.Index += 3;
                    c3.Index += 4;
                    c3.Emit(OpCodes.Ldarg_0);
                    c3.Emit(OpCodes.Ldfld, Retrieve.GetField(typeof(RainWorld), "progression"));
                    c3.Emit(OpCodes.Ldfld, Retrieve.GetField(typeof(PlayerProgression), "regionNames"));
                    c3.Emit(OpCodes.Ldloc_S, (byte)11);
                    c3.Emit(OpCodes.Call, typeof(Enumerable).GetMethods().Single(x => x.Name == "Contains" && x.GetParameters().Length == 2).MakeGenericMethod(typeof(string)));
                    ILLabel labelLoopEnd = c4.DefineLabel();
                    labelLoopEnd.Target = c4.Next;
                    c3.Emit(OpCodes.Brfalse, labelLoopEnd);

                    ILLabel labelBack = c.DefineLabel();
                    ILLabel labelDest = c.DefineLabel();
                    ILLabel labelElseDone = c5.DefineLabel();
                    labelBack.Target = c.Prev;
                    labelDest.Target = c.Next;
                    labelElseDone.Target = c5.Next;
                    c.Emit(OpCodes.Ldsfld, Retrieve.GetStaticField(typeof(Plugin), "hasCustomRegionStarts"));
                    c.Emit(OpCodes.Brfalse_S, labelDest);

                    c.Emit(OpCodes.Ldsfld, Retrieve.GetStaticField(typeof(Plugin), "customRegionStarts"));
                    c.Emit(OpCodes.Ldloc_S, (byte)11);
                    c.Emit(OpCodes.Ldarg_1);
                    c.Emit(OpCodes.Callvirt, Retrieve.GetMethod(typeof(CustomRegionStarts), "Exec"));
                    c.Emit(OpCodes.Brfalse_S, labelDest);

                    c.Emit(OpCodes.Ldloc_1);
                    c.Emit(OpCodes.Ldloc_S, (byte)11);
                    c.Emit(OpCodes.Callvirt, Retrieve.GetStaticMethod(typeof(Dictionary<string, List<string>>), "get_Item"));
                    c.Emit(OpCodes.Ldloc_S, (byte)4);
                    c.Emit(OpCodes.Ldloc_S, (byte)10);
                    c.Emit(OpCodes.Ldelem_Ref);
                    c.Emit(OpCodes.Callvirt, Retrieve.GetStaticMethod(typeof(List<string>), "Add"));
                    c.Emit(OpCodes.Br, labelElseDone);

                    c.GotoLabel(labelBack);
                    ILLabel labelStart = c.DefineLabel();
                    labelStart.Target = c.Next.Next;
                    c2.Next.Operand = labelStart;

                    if (c.TryGotoNext(
                        x => x.MatchLdloc(9),
                        x => x.Match(OpCodes.Ret)))
                    {
                        c.Emit(OpCodes.Ldloc_0);
                        c.Emit(OpCodes.Callvirt, Retrieve.GetStaticMethod(typeof(Dictionary<string, int>), "get_Values"));
                        c.Emit(OpCodes.Ldloc_1);
                        c.Emit(OpCodes.Callvirt, Retrieve.GetStaticMethod(typeof(Dictionary<string, List<string>>), "get_Values")); 
                        c.Emit(OpCodes.Call, Retrieve.GetStaticMethod(typeof(ExpeditionGameHooks), "LogStuff"));
                    }

                    Plugin.LogDebug(il.ToString());
                    Plugin.LogDebug("Expedition.ExpeditionGame.ExpeditionRandomStarts hook applied");
                } catch (Exception e) { Plugin.LogError(e.ToString()); }
            }
        }

        private static int GetCustomRegionWeight(On.Expedition.ExpeditionGame.orig_GetRegionWeight orig, string region)
        {
            int weight = orig(region);
            if (Plugin.customRegionStarts.customRegionWeight.ContainsKey(region))
            {
                weight = Plugin.customRegionStarts.customRegionWeight[region];
            }
            return weight;
        }

        public static void LogStuff(Dictionary<string, int> regions, Dictionary<string, List<string>> shelters)
        {
            Plugin.LogDebug("regions = " + regions.ToString());
            Plugin.LogDebug("shelters = " + shelters.ToString());
            try
            {
                List<string> regionsKeys = regions.Keys.ToList();
                for (int i = 0; i < regionsKeys.Count; i++)
                {
                    Plugin.LogDebug(regionsKeys[i] + " has a weight of " + regions[regionsKeys[i]]);
                }
                List<string> shelterKeys = shelters.Keys.ToList();
                for (int i = 0; i < shelterKeys.Count; i++)
                {
                    Plugin.LogDebug(shelterKeys[i] + " has the following shelters availible:");
                    foreach(string shltr in shelters[shelterKeys[i]])
                    {
                        Plugin.LogDebug(shltr);
                    }
                }
            } catch (Exception e) { Plugin.LogError(e.ToString()); }
        }
    }
}