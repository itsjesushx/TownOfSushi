using System.Collections;

namespace TownOfSushi.Roles.Impostors.Power.BomberRole
{
    public class Bomb
    {
        public Transform transform;
    }

    [HarmonyPatch]
    public static class BombExtentions
    {
        public static void ClearBomb(this Bomb b)
        {
            Object.Destroy(b.transform.gameObject);
            b = null;
        }

        public static Bomb CreateBomb(this Vector3 location)
        {
            var BombPref = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            BombPref.name = "Bomb";
            BombPref.transform.localScale = new Vector3(CustomGameOptions.DetonateRadius * ShipStatus.Instance.MaxLightRadius * 2f, 
                CustomGameOptions.DetonateRadius * ShipStatus.Instance.MaxLightRadius * 2f, CustomGameOptions.DetonateRadius * ShipStatus.Instance.MaxLightRadius * 2f);
            GameObject.Destroy(BombPref.GetComponent<SphereCollider>());
            BombPref.GetComponent<MeshRenderer>().material = Bomber.bombMaterial;
            BombPref.transform.position = location;
            var BombScript = new Bomb();
            BombScript.transform = BombPref.transform;
            return BombScript;
        }
        public class BombTeammate
        {
            public static Bomb TempBomb = null;

            public static IEnumerator BombShowTeammate(Vector3 location)
            {
                TempBomb = CreateBomb(location);

                yield return (object)new WaitForSeconds(CustomGameOptions.DetonateDelay);

                try { ClearBomb(TempBomb); }
                catch { }
            }
        }
    }
}