using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace MikuSB.GameServer.Server.CallGS.Handlers.Girl;

[CallGSApi("GirlSkin_ChangeSkinType")]
public class GirlSkin_ChangeSkinType : ICallGSHandler
{
    public async Task Handle(Connection connection, string param, ushort seqNo)
    {
        var req = JsonSerializer.Deserialize<ChangeSkinTypeParam>(param);
        var response = new JsonObject
        {
            ["nType"] = req?.Type ?? 1,
            ["nSkinId"] = req?.SkinId
        };
        // TODO change type in proto Item ??
        await CallGSRouter.SendScript(connection, "GirlSkin_ChangeSkinType", response.ToJsonString());
    }
}

internal sealed class ChangeSkinTypeParam
{
    [JsonPropertyName("nType")]
    public int? Type { get; set; }

    [JsonPropertyName("nSkinId")]
    public uint? SkinId { get; set; }
}
