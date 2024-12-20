using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using Vintagestory.API.Common;
using Vintagestory.API.Server;
using Vintagestory.GameContent;

namespace NoCharcoalLost;

[HarmonyPatch]
public class NoCharcoalLostMod : ModSystem {
    private Harmony? _harmony;

    public override bool ShouldLoad(EnumAppSide side) {
        return side.IsServer();
    }

    public override void StartServerSide(ICoreServerAPI capi) {
        _harmony = new Harmony(Mod.Info.ModID);
        _harmony.PatchAll(Assembly.GetExecutingAssembly());
    }

    public override void Dispose() {
        _harmony?.UnpatchAll(Mod.Info.ModID);
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(BlockEntityCharcoalPit), "ConvertPit")]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    private static void Postfix(BlockEntityCharcoalPit __instance) {
        if (__instance.Api.World.BlockAccessor.GetBlock(__instance.Pos).Code.PathStartsWith("charcoalpile")) {
            __instance.Api.World.BlockAccessor.SetBlock(0, __instance.Pos);
        }
    }

    [HarmonyTranspiler]
    [HarmonyPatch(typeof(BlockEntityCharcoalPit), "ConvertPit")]
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) {
        List<CodeInstruction> codes = new(instructions);

        for (int i = 0; i < codes.Count; i++) {
            if (!codes[i].operand?.ToString()?.Equals("0.125") ?? true) {
                continue;
            }

            codes.RemoveRange(i - 1, 13);
            break;
        }

        return codes.AsEnumerable();
    }
}
