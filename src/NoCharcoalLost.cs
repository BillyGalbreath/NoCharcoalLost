using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using Vintagestory.API.Common;
using Vintagestory.API.Server;
using Vintagestory.GameContent;

namespace NoCharcoalLost;

[HarmonyPatch]
public class NoCharcoalLost : ModSystem {
    private Harmony harmony;

    public override bool ShouldLoad(EnumAppSide side) {
        return side.IsServer();
    }

    public override void StartServerSide(ICoreServerAPI capi) {
        harmony = new Harmony(Mod.Info.ModID);
        harmony.PatchAll(Assembly.GetExecutingAssembly());
    }

    public override void Dispose() {
        harmony?.UnpatchAll(Mod.Info.ModID);
    }

    [HarmonyTranspiler]
    [HarmonyPatch(typeof(BlockEntityCharcoalPit), "ConvertPit")]
    public static IEnumerable<CodeInstruction> ConvertPit(IEnumerable<CodeInstruction> instructions, ILGenerator generator) {
        var codes = new List<CodeInstruction>(instructions);

        for (int i = 0; i < codes.Count; i++) {
            if (!codes[i]?.operand?.ToString()?.Equals("0.125") ?? true) {
                continue;
            }

            codes.RemoveRange(i - 1, 13);
            break;
        }

        return codes.AsEnumerable();
    }
}
