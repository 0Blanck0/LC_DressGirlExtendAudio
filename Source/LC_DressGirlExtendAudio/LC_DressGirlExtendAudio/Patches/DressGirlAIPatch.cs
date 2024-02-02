using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;

namespace LC_DressGirlExtendAudio.Patches
{
    [HarmonyPatch(typeof(DressGirlAI))]
    internal class DressGirlAIPatch
    {
        public static AudioClip[] audioList;

        internal static float currentTimeInSeconds;
        internal static ManualLogSource mls = DressGirlExtendAudioBase.mls;

        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        static void OverrideAudio(ref DressGirlAI __instance)
        {
            AudioClip[] newSoundSFX = getDressGirlAudioList(__instance);
            audioList = newSoundSFX;

            __instance.appearStaringSFX = audioList;
            Debug.Log($"Start ok - Audio: ${audioList}");
        }

        [HarmonyPatch("Update")]
        [HarmonyPostfix]
        static void appearStaringSFXPatch(ref DressGirlAI __instance)
        {
            if (audioList == null)
            {
                audioList = getDressGirlAudioList(__instance);
                __instance.appearStaringSFX = audioList;
            } else
            {
                if (Time.time - currentTimeInSeconds > 60)
                {
                    __instance.skipWalkSFX = getRandomAudioClip(audioList);
                    currentTimeInSeconds = Time.time;
                }
            }
        }

        public static AudioClip getRandomAudioClip(AudioClip[] audioList)
        {
            System.Random rnd = new System.Random((int)Time.time);
            int random = rnd.Next(0, audioList.Length);
            return audioList[random];
        }

        public static AudioClip[] getDressGirlAudioList(DressGirlAI __instance)
        {
            AudioClip[] newSoundSFX = new AudioClip[DressGirlExtendAudioBase.SoundSFX["DressGirlAI"].Count];

            Debug.Log("Loading sondSFX");

            for (int i = 0; i < DressGirlExtendAudioBase.SoundSFX["DressGirlAI"].Count; i++)
            {
                Debug.Log($"Current i: {i} \n {DressGirlExtendAudioBase.SoundSFX["DressGirlAI"][i]}");
                newSoundSFX[i] = DressGirlExtendAudioBase.SoundSFX["DressGirlAI"][i];
            }

            audioList = newSoundSFX;
            return newSoundSFX;
        }
    }
}