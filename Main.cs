using BepInEx;
using HarmonyLib;

namespace SkillRebalanceExpansionMod
{
    [BepInPlugin(PluginInfo.GUID, PluginInfo.NAME, PluginInfo.VERSION)]
    public class Main : BaseUnityPlugin
    {
        public static BepInEx.Logging.ManualLogSource Log;

        private void Awake()
        {
            Log = Logger;

            Harmony harmony = new Harmony(PluginInfo.GUID);
            harmony.PatchAll();

            Log.LogInfo(PluginInfo.NAME + " Loaded");
        }
    }
}