using System.Collections.Generic;

using HarmonyLib;
using SkillRebalanceExpansionMod.Core;

namespace SkillRebalanceExpansionMod.Patches
{
    [HarmonyPatch(typeof(YSJSONHelper), "InitJSONClassData")]
    public static class InitJSONPatch
    {
        [HarmonyPrefix]
        public static void Prefix()
        {
            Loader.Start();
        }
    }
}