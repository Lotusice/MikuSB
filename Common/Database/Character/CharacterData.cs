using MikuSB.Proto;
using SqlSugar;

namespace MikuSB.Database.Character;

[SugarTable("character_data")]
public class CharacterData : BaseDatabaseDataHelper
{
    [SugarColumn(IsJson = true)] public List<CharacterInfo> Characters { get; set; } = [];
    public uint NextCharacterGuid { get; set; } = 1;
}

public class CharacterInfo
{
    public uint Guid { get; set; }
    public ulong TemplateId { get; set; }
    public uint Level { get; set; }
    public int Exp { get; set; }
    public uint Break { get; set; }
    public int Evolue { get; set; }
    public int Trust { get; set; }
    public uint WeaponUniqueId { get; set; }
    public uint SkinId { get; set; }
    [SugarColumn(IsJson = true)] public List<uint> UnlockedSkin { get; set; } = [];
    [SugarColumn(IsJson = true)] public List<uint> Spines { get; set; } = [];
    [SugarColumn(IsJson = true)] public List<uint> Affixs { get; set; } = [];
    public long Timestamp { get; set; }
    public uint Count { get; set; } = 1;

    public Item ToProto()
    {
        var proto = new Item
        {
            Id = Guid,
            Template = TemplateId,
            Count = Count,
            Enhance = new Enhance
            {
                Level = Level,
                Break = Break
            }
        };

        proto.Slots[4] = WeaponUniqueId;
        proto.Slots[5] = SkinId;

        return proto;
    }

}