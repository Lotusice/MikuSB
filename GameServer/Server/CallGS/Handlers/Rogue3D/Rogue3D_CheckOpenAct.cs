namespace MikuSB.GameServer.Server.CallGS.Handlers.Rogue3D;

// Returns which Rogue3D acts (server_04_timelist) are currently open.
// param: [] (empty)
// Response: {"listActId":[...]}
[CallGSApi("Rogue3D_CheckOpenAct")]
public class Rogue3D_CheckOpenAct : CallGSHandler
{
    protected override Task<CallGSResult> HandleAsync(CallGSContext context, string param)
    {
        var sync = Rogue3DStateHelper.EnsureUnlockState(context.Connection.Player!);
        return Task.FromResult(CallGSResult.Ok("{\"bOpen\":true}", sync));
    }
}
