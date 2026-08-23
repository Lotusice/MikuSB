using MikuSB.Data;
using MikuSB.Database;
using MikuSB.GameServer.Game.Player;
using MikuSB.Proto;
using System.Text.Json.Serialization;

namespace MikuSB.GameServer.Server.CallGS.Handlers.Rogue3D;

[CallGSApi("Rogue3D_UpgradeTech")]
public class Rogue3D_UpgradeTech : CallGSHandler<UpgradeTechParam>
{
    protected override Task<CallGSResult> HandleAsync(CallGSContext context, UpgradeTechParam req)
    {
        if (!Rogue3DTechHelper.TryGetScience(req.TechId, out var science) ||
            science.MaxLevel == 0 ||
            science.LevelList.Count < science.MaxLevel)
        {
            return Task.FromResult(CallGSResult.Error("error.BadParam"));
        }

        var player = context.Player;
        var techLevel = player.Attributes.GetOrCreate(AttrIds.Rogue3D.Gid, req.TechId);
        if (techLevel.Val >= science.MaxLevel)
        {
            return Task.FromResult(CallGSResult.Error("tip.girlcard_break_max"));
        }

        if (!Rogue3DTechHelper.IsUnlocked(player, science) ||
            Rogue3DTechHelper.IsRestricted(player, req.TechId))
        {
            return Task.FromResult(CallGSResult.Error("error.condition_limit"));
        }

        var nextLevelId = science.LevelList[(int)techLevel.Val];
        if (!GameData.Rogue3DScienceLevelData.TryGetValue(nextLevelId, out var level))
        {
            return Task.FromResult(CallGSResult.Error("error.BadParam"));
        }

        if (level.Cost.Count < 2 || level.Cost[0] != AttrIds.Rogue3D.TechPointCurrencyType)
        {
            return Task.FromResult(CallGSResult.Error("error.BadParam"));
        }

        var currency = player.Attributes.GetOrCreate(
            AttrIds.Currency.GroupId,
            AttrIds.Currency.GetSid(level.Cost[0]));
        if (currency.Val < level.Cost[1])
        {
            return Task.FromResult(CallGSResult.Error("error.gold_not_enough"));
        }

        currency.Val -= level.Cost[1];
        techLevel.Val++;

        var sync = new NtfSyncPlayer();
        player.Attributes.SyncTo(sync, currency);
        player.Attributes.SyncTo(sync, techLevel);
        DatabaseHelper.SaveDatabaseType(player.Data);

        return Task.FromResult(CallGSResult.Ok("{}", sync));
    }
}

public sealed class UpgradeTechParam
{
    [JsonPropertyName("nTechId")]
    public uint TechId { get; set; }
}
