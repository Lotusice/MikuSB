namespace MikuSB.GameServer.Server.CallGS.Handlers.Achievement;

[CallGSApi("Achievement_GetReward")]
public class Achievement_GetReward : ICallGSHandler
{
    public async Task Handle(Connection connection, string param, ushort seqNo)
    {
        // param: json.encode({nId = nId})
        // TODO: implement reward logic

        await CallGSRouter.SendScript(connection, "Achievement_GetReward", "{}", seqNo);
    }
}
