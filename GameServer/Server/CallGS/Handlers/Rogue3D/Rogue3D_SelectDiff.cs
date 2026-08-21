using MikuSB.Data;
using MikuSB.GameServer.Game.Player;
using MikuSB.Proto;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MikuSB.GameServer.Server.CallGS.Handlers.Rogue3D;

// Selects the Rogue3D difficulty.
// Persists CurDiff (sid=5) and GameplayId (sid=6) as player attributes (GroupId=124).
// param: {"nDiffId": int}
// Response: {} on success, {"sErr": "key"} on failure
[CallGSApi("Rogue3D_SelectDiff")]
public class Rogue3D_SelectDiff : CallGSHandler<SelectDiffParam>
{
    private const uint GroupId = AttrIds.Rogue3D.Gid;
    private const uint CurDiffSid = AttrIds.Rogue3D.CurDiffSid;
    private const uint GameplayIdSid = AttrIds.Rogue3D.GameplayIdSid;

    protected override Task<CallGSResult> HandleAsync(CallGSContext context, SelectDiffParam req)
    {

        if (req == null)
        {
            return Task.FromResult(CallGSResult.Ok("{}"));
        }

        if (!GameData.Rogue3DDifficultData.TryGetValue(req.DiffId, out var cfg) || cfg.GameplayGroup.Count == 0)
        {
            return Task.FromResult(CallGSResult.Error("rogue3.massage_gameProcessError"));
        }

        var player = context.Connection.Player!;
        var sync = new NtfSyncPlayer();

        SetAttr(player, CurDiffSid, req.DiffId, sync);
        SetAttr(player, GameplayIdSid, cfg.GameplayGroup[0], sync);

        return Task.FromResult(CallGSResult.Ok("{}", sync));
    }

    private static void SetAttr(PlayerInstance player, uint sid, uint val, NtfSyncPlayer sync)
    {
        var attr = player.Attributes.GetOrCreate(GroupId, sid);
        attr.Val = val;
        player.Attributes.SyncTo(sync, attr);
    }
}

public sealed class SelectDiffParam
{
    [JsonPropertyName("nDiffId")]
    public uint DiffId { get; set; }
}
