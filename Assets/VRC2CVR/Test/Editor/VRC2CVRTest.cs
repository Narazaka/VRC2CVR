using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using NUnit.Framework;
using VRCSDK2;
using ABI.CCK.Components;

public class VRC2CVRTest {
    [OneTimeSetUp]
    public void OneTimeSetup() {
        EditorSceneManager.OpenScene("Assets/Shapell/shapellscene.unity", OpenSceneMode.Single);
    }

    [Test]
    public void Basic() {
        var obj = GameObject.Find("shapell");
        var vrcAvatar = obj.GetComponent<VRC_AvatarDescriptor>();
        VRC2CVR.CopyToCVR(new MenuCommand(vrcAvatar));
        // EditorApplication.ExecuteMenuItem("CONTEXT/VRC_AvatarDescriptor/Copy To CVR");
        var cvrAvatar = obj.GetComponent<CVRAvatar>();

        Assert.IsInstanceOf<VRC_AvatarDescriptor>(vrcAvatar);
        Assert.IsInstanceOf<CVRAvatar>(cvrAvatar);
        Assert.AreEqual(vrcAvatar.ViewPosition, cvrAvatar.viewPosition);
        Assert.AreNotEqual(cvrAvatar.viewPosition, cvrAvatar.voicePosition);
        Assert.IsTrue(cvrAvatar.useVisemeLipsync);
        Assert.AreEqual(vrcAvatar.VisemeBlendShapes, cvrAvatar.visemeBlendshapes);
        Assert.AreEqual(vrcAvatar.VisemeSkinnedMesh, cvrAvatar.bodyMesh);

        var vrcAnims = vrcAvatar.CustomStandingAnims;
        var vrcOverrides = new List<KeyValuePair<AnimationClip, AnimationClip>>(vrcAnims.overridesCount);
        vrcAnims.GetOverrides(vrcOverrides);
        var vrcOverrideMap = vrcOverrides.ToDictionary(kv => kv.Key.name, kv => kv.Value);
        var cvrAnims = cvrAvatar.overrides;
        var cvrOverrides = new List<KeyValuePair<AnimationClip, AnimationClip>>(vrcAnims.overridesCount);
        cvrAnims.GetOverrides(cvrOverrides);
        var cvrOverrideMap = cvrOverrides.ToDictionary(kv => kv.Key.name, kv => kv.Value);
        Assert.AreEqual(vrcOverrideMap["FINGERPOINT"], cvrOverrideMap["HandLeftPoint"]);
        Assert.AreEqual(vrcOverrideMap["FINGERPOINT"], cvrOverrideMap["HandRightPoint"]);
    }
}
