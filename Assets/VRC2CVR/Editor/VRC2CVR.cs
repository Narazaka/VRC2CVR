using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEditor;
using VRCSDK2;
using ABI.CCK.Components;

public class VRC2CVR {
    public static Dictionary<string, string> OverrideMap = new Dictionary<string, string> {
        { "Emote1", "EMOTE1" },
        { "Emote2", "EMOTE2" },
        { "Emote3", "EMOTE3" },
        { "Emote4", "EMOTE4" },
        { "Emote5", "EMOTE5" },
        { "Emote6", "EMOTE6" },
        { "Emote7", "EMOTE7" },
        { "Emote8", "EMOTE8" },
        { "HandLeftFist", "FIST" },
        { "HandLeftGun", "HANDGUN" },
        { "HandLeftOpen", "HANDOPEN" },
        { "HandLeftPeace", "VICTORY" },
        { "HandLeftPoint", "FINGERPOINT" },
        { "HandLeftRelaxed", "" },
        { "HandLeftRocknroll", "ROCKNROLL" },
        { "HandLeftThumbsup", "THUMBSUP" },
        { "HandRightFist", "FIST" },
        { "HandRightGun", "HANDGUN" },
        { "HandRightOpen", "HANDOPEN" },
        { "HandRightPeace", "VICTORY" },
        { "HandRightPoint", "FINGERPOINT" },
        { "HandRightRelaxed", "" },
        { "HandRightRocknroll", "ROCKNROLL" },
        { "HandRightThumbsup", "THUMBSUP" },
        { "LocCrouchForward", "CROUCHWALKFWD" },
        { "LocCrouchIdle", "CROUCHIDLE" },
        { "LocCrouchRight", "CROUCHWALKRT" },
        { "LocFlying", "FALL" },
        { "LocIdle", "IDLE" },
        { "LocJumpAir", "" },
        { "LocJumpLand", "" },
        { "LocJumpStart", "" },
        { "LocProneBackward", "" },
        { "LocProneForward", "PRONEFWD" },
        { "LocProneIdle", "PRONEIDLE" },
        { "LocProneRight", "" },
        { "LocRunningBackward", "RUNBACK" },
        { "LocRunningForward", "RUNFWD" },
        { "LocRunningStrafeRight", "RUNSTRAFERT45" },
        { "LocRunningStrafeRightBackwards", "" },
        { "LocRunningStrafeRightForwards", "" },
        { "LocRunning", "SPRINTFWD" },
        { "LocSitting", "" },
        { "LocWalkingBackwards", "WALKBACK" },
        { "LocWalkingForward", "WALKFWD" },
        { "LocWalkingStrafeRight", "STRAFERT" },
        { "LocWalkingStrafeRightBackwards", "" },
        { "LocWalkingStrafeRightForwards", "" },
        { "ToggleDefault", "" },
        { "ToggleState1", "" },
        { "ToggleState2", "" },
        { "ToggleState3", "" },
        { "ToggleState4", "" },
        { "ToggleState5", "" },
        { "ToggleState6", "" },
        { "ToggleState7", "" },
    };

    [MenuItem("CONTEXT/VRC_AvatarDescriptor/Copy To CVR")]
    public static void CopyToCVR(MenuCommand menuCommand) {
        var vrcAvatar = menuCommand.context as VRC_AvatarDescriptor;
        var obj = vrcAvatar.gameObject;
        var cvrAvatar = obj.GetOrAddComponent<CVRAvatar>();
        cvrAvatar.viewPosition = vrcAvatar.ViewPosition;
        cvrAvatar.voicePosition = vrcAvatar.ViewPosition - new Vector3(0, 0.04f, 0);
        cvrAvatar.voiceParent = CVRAvatar.CVRAvatarVoiceParent.Head;
        cvrAvatar.bodyMesh = vrcAvatar.VisemeSkinnedMesh;
        if (vrcAvatar.lipSync == VRC.SDKBase.VRC_AvatarDescriptor.LipSyncStyle.VisemeBlendShape) {
            cvrAvatar.useVisemeLipsync = true;
            cvrAvatar.visemeBlendshapes = vrcAvatar.VisemeBlendShapes.Clone() as string[];
        }
        var vrcAnims = vrcAvatar.CustomStandingAnims ?? vrcAvatar.CustomSittingAnims;
        if (vrcAnims != null) {
            var vrcOverrides = new List<KeyValuePair<AnimationClip, AnimationClip>>(vrcAnims.overridesCount);
            vrcAnims.GetOverrides(vrcOverrides);
            var vrcOverrideMap = vrcOverrides.ToDictionary(kv => kv.Key.name, kv => kv.Value);
            var cvrAnims = cvrAvatar.overrides;
            if (cvrAnims == null) {
                var avatarAnimator = AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>("Assets/ABI.CCK/Animations/AvatarAnimator.controller");
                cvrAnims = new AnimatorOverrideController(avatarAnimator);
                var vrcAnimsPath = AssetDatabase.GetAssetPath(vrcAnims);
                var cvrAnimsPath = Regex.Replace(vrcAnimsPath, @"\.overrideController$", "-cvr.overrideController");
                AssetDatabase.CreateAsset(cvrAnims, cvrAnimsPath);
                cvrAvatar.overrides = cvrAnims;
            }
            var cvrOverrides = new List<KeyValuePair<AnimationClip, AnimationClip>>(vrcAnims.overridesCount);
            cvrAnims.GetOverrides(cvrOverrides);
            for (var i = 0; i < cvrOverrides.Count; ++i) {
                if (OverrideMap.TryGetValue(cvrOverrides[i].Key.name, out var vrcClipName)) {
                    if (vrcClipName == "")
                        continue;
                    var vrcClip = vrcOverrideMap[vrcClipName];
                    cvrOverrides[i] = new KeyValuePair<AnimationClip, AnimationClip>(cvrOverrides[i].Key, vrcClip);
                } else {
                    Debug.LogWarning($"{cvrOverrides[i].Key.name} is unknown. please report this to VRC2CVR-YA author.");
                }
            }
            cvrAnims.ApplyOverrides(cvrOverrides);
        }
    }
}

