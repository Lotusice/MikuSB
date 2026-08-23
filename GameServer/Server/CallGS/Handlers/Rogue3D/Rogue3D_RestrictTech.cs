using MikuSB.Data;
using MikuSB.Database;
using MikuSB.Proto;
using System.Text.Json.Serialization;

namespace MikuSB.GameServer.Server.CallGS.Handlers.Rogue3D;

[CallGSApi("Rogue3D_RestrictTech")]
public class Rogue3D_RestrictTech : CallGSHandler<RestrictTechParam>
{
    protected override Task<CallGSResult> HandleAsync(CallGSContext context, RestrictTechParam req)
    {
        if (req.Restrict > 1 || !Rogue3DTechHelper.TryGetScience(req.TechId, out _))
        {
            return Task.FromResult(CallGSResult.Error("error.BadParam"));
        }

        var player = context.Player;
        if (player.Attributes.GetValue(AttrIds.Rogue3D.Gid, req.TechId) == 0)
        {
            return Task.FromResult(CallGSResult.Error("error.condition_limit"));
        }

        var restriction = player.Attributes.GetOrCreate(
            AttrIds.Rogue3D.Gid,
            Rogue3DTechHelper.GetRestrictionSid(req.TechId));
        if (restriction.Val == req.Restrict)
        {
            return Task.FromResult(CallGSResult.Ok());
        }

        restriction.Val = req.Restrict;
        var sync = new NtfSyncPlayer();
        player.Attributes.SyncTo(sync, restriction);
        DatabaseHelper.SaveDatabaseType(player.Data);

        return Task.FromResult(CallGSResult.Ok("{}", sync));
    }
}

public sealed class RestrictTechParam
{
    [JsonPropertyName("nTechId")]
    public uint TechId { get; set; }

    [JsonPropertyName("nRestrict")]
    public uint Restrict { get; set; }
}
