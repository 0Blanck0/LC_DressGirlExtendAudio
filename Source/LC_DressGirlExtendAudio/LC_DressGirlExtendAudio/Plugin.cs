using System.Collections.Generic;
using UnityEngine;
using System.IO;
using HarmonyLib;
using BepInEx;
using BepInEx.Logging;
using LC_DressGirlExtendAudio.Patches;

namespace LC_DressGirlExtendAudio
{
    [BepInPlugin(modGUID, modName, modVersion)]
    public class DressGirlExtendAudioBase : BaseUnityPlugin
    {
        private const string modGUID = "Elia.LC_DressGirlExtendAudio";
        private const string modName = "LC_DressGirlExtendAudio";
        private const string modVersion = "1.0.0";

        private readonly Harmony harmony = new Harmony(modGUID);

        private static DressGirlExtendAudioBase Instance;

        internal static ManualLogSource mls;
        internal static Dictionary<string, List<AudioClip>> SoundSFX;
        internal static Dictionary<string, List<AssetBundle>> Bundle;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }

            SoundSFX = new Dictionary<string, List<AudioClip>>();
            Bundle = new Dictionary<string, List<AssetBundle>>();

            string FolderLocation = Instance.Info.Location.TrimEnd("LC_DressGirlExtendAudio.dll".ToCharArray());

            mls = BepInEx.Logging.Logger.CreateLogSource(modGUID);
            mls.LogInfo("DressGirlExtendAudio mod is awaken.");

            harmony.PatchAll(typeof(DressGirlAIPatch));

            mls.LogInfo("Patching start...");
            mls.LogInfo($"Audio Path:  {FolderLocation}");

            SoundSFX.Add("DressGirlAI", new List<AudioClip>());
            Bundle.Add("DressGirlAI", new List<AssetBundle>());

            try
            {
                if (Directory.Exists(FolderLocation))
                {
                    string[] girlAudioFiles = Directory.GetFiles(FolderLocation, "*_song");

                    foreach (string audioFile in girlAudioFiles)
                    {
                        Bundle["DressGirlAI"].Add(AssetBundle.LoadFromFile(audioFile));
                    }
                }
            }
            catch
            {
                Debug.Log("Error loading Bundle...");
            }

            mls.LogInfo(Bundle["DressGirlAI"].ToString());
            mls.LogInfo("DressGirlExtendAudio loading asset...");
            mls.LogInfo("Asset target: " + FolderLocation);

            if (Bundle.Count > 0)
            {
                try
                {
                    foreach (AssetBundle dressGirlExtendBundle in Bundle["DressGirlAI"])
                    {
                        SoundSFX["DressGirlAI"].AddRange(dressGirlExtendBundle.LoadAllAssets<AudioClip>());
                    }
                }
                catch
                {
                    Debug.Log("Error loading SoundSFX DressGirlAI...");
                }

                mls.LogInfo($"Successfully loaded asset bundle: \n   - Count: {SoundSFX["DressGirlAI"].Count}");
            }
            else
            {
                mls.LogError("Failed to load asset bundle...");
            }
        }
    }
}
