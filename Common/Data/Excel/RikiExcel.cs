using Newtonsoft.Json;

namespace MikuSB.Data.Excel;

[ResourceEntity("riki/Riki.json")]
public class RikiExcel : ExcelResource
{
    public uint Type { get; set; }
    public uint Id { get; set; }
    [JsonProperty("Condition")] public List<uint> Condition { get; set; } = [];

    public override uint GetId() => Id;

    public override void Loaded()
    {
        if (Type is 1 or 2 && Condition.Count >= 4)
        {
            GameData.RikiData[Id] = this;
        }
    }
}
