﻿using MiraAPI.GameOptions;
using TownOfSushi.Options.Roles.Neutral;
using TownOfSushi.Utilities;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TownOfSushi.Modules;

public sealed class Ignite
{
    public Transform Transform { get; set; }

    public void Clear()
    {
        Object.Destroy(Transform.gameObject);
    }

    public static Ignite CreateIgnite(Vector3 location)
    {
        var igniteRadius = OptionGroupSingleton<ArsonistOptions>.Instance.IgniteRadius.Value;

        var gameObject = MiscUtils.CreateSpherePrimitive(location, igniteRadius);
        gameObject.GetComponent<MeshRenderer>().material = AuAvengersAnims.IgniteMaterial.LoadAsset();

        var ignite = new Ignite
        {
            Transform = gameObject.transform
        };

        return ignite;
    }
}
