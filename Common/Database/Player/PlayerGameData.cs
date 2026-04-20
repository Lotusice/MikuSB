using MikuSB.Common.Util;
using MikuSB.Proto;
using MikuSB.Util.Extensions;
using SqlSugar;

namespace MikuSB.Database.Player;

[SugarTable("Player")]
public class PlayerGameData : BaseDatabaseDataHelper
{
    public string? Name { get; set; } = "";
    public string? Signature { get; set; } = "MikuPS";
    public uint Level { get; set; } = 1;
    public int Exp { get; set; } = 0;
    public long RegisterTime { get; set; } = Extensions.GetUnixSec();
    public long LastActiveTime { get; set; }

    public static PlayerGameData? GetPlayerByUid(long uid)
    {
        var result = DatabaseHelper.GetInstance<PlayerGameData>((int)uid);
        return result;
    }
}