﻿using Il2CppInterop.Runtime.Attributes;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Reactor.Utilities.Attributes;
using UnityEngine;

namespace AuAvengers.Animations;

[RegisterInIl2Cpp]
[System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1051:Do not declare visible instance fields", Justification = "Unity")]
[System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:Fields should be private", Justification = "Unity")]
[System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.NamingRules", "SA1307:Accessible fields should begin with upper-case letter", Justification = "Unity")]
public sealed class UE_SpriteFlipper : MonoBehaviour
{
    [HideFromIl2Cpp]
    public Il2CppArrayBase<SpriteRenderer> RenderersArray { get; set; }

    public bool UseNegative { get; set; }
    public bool DoOffset { get; set; }
    public float Offset { get; set; } = 0.8f;

    [HideFromIl2Cpp]
    public CosmeticsLayer reference { get; set; }

    public void Start()
    {
        RenderersArray = GetComponentsInChildren<SpriteRenderer>(true);
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S3358:Ternary operators should not be nested", Justification = "Too much work")]
    public void Update()
    {
        if (RenderersArray.Count != 0)
            foreach (var rend in RenderersArray)
            {
                rend.flipX = reference.FlipX;

                if (DoOffset)
                    transform.parent.localPosition = new Vector3(reference.FlipX ? (UseNegative ? -Offset : Offset) : 0, transform.parent.localPosition.y, transform.parent.localPosition.z);
            }
    }
}
