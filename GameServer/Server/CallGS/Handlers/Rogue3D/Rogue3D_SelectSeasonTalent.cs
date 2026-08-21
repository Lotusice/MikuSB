using MikuSB.Proto;
using MikuSB.GameServer.Game.Player;
using System.Text.Json;
using System.Text.Json.Serialization;

using MikuSB.Data;

namespace MikuSB.GameServer.Server.CallGS.Handlers.Rogue3D;

// Selects the Rogue3D season talent and persists it as player attribute (GroupId=124, TalentId=1007).
// param: {"nTalentId": int}
// Response: {} on success, {"sErr": "key"} on failure
[CallGSApi("Rogue3D_SelectSeasonTalent")]
public class Rogue3D_SelectSeasonTalent : CallGSHandler<SelectSeasonTalentParam>
{
    private const uint GroupId = AttrIds.Rogue3D.Gid;
    private const uint SeasonTalentIdSid = AttrIds.Rogue3D.SeasonTalentIdSid;

    protected override Task<CallGSResult> HandleAsync(CallGSContext context, SelectSeasonTalentParam req)
    {

        if (req == null)
        {
            return Task.FromResult(CallGSResult.Ok("{}"));
        }

        var player = context.Connection.Player!;
        var attr = player.Attributes.GetOrCreate(GroupId, SeasonTalentIdSid);
        attr.Val = req.TalentId;

        var sync = new NtfSyncPlayer();
        player.Attributes.SyncTo(sync, attr);

        return Task.FromResult(CallGSResult.Ok("{}", sync));
    }
}

public sealed class SelectSeasonTalentParam
{
    [JsonPropertyName("nTalentId")]
    public uint TalentId { get; set; }
}
