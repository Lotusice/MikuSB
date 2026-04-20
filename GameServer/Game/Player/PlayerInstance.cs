using MikuSB.Database;
using MikuSB.Database.Account;
using MikuSB.Database.Player;
using MikuSB.GameServer.Server;
using MikuSB.TcpSharp;
using MikuSB.Util.Extensions;

namespace MikuSB.GameServer.Game.Player;

public class PlayerInstance(PlayerGameData data)
{
    #region Property
    public Connection? Connection { get; set; }

    public static readonly List<PlayerInstance> _playerInstances = [];
    public int Uid { get; set; }
    public bool Initialized { get; set; }
    public bool IsNewPlayer { get; set; }

    #endregion

    #region Data & Manager

    public PlayerGameData Data { get; set; } = data;

    #endregion

    #region Initializers
    public PlayerInstance(int uid) : this(new PlayerGameData { Uid = uid })
    {
        // new player
        IsNewPlayer = true;
        Data.Name = AccountData.GetAccountByUid(uid)?.Username;

        DatabaseHelper.CreateInstance(Data);

        var t = Task.Run(async () =>
        {
            await InitialPlayerManager();
        });
        t.Wait();

        Initialized = true;

    }
    private async ValueTask InitialPlayerManager()
    {
        Uid = Data.Uid;
        Data.LastActiveTime = Extensions.GetUnixSec();

        await Task.CompletedTask;
    }
    public T InitializeDatabase<T>() where T : BaseDatabaseDataHelper, new()
    {
        var instance = DatabaseHelper.GetInstanceOrCreateNew<T>(Uid);
        return instance!;
    }

    #endregion

    #region Network
    public async ValueTask OnEnterGame()
    {
        if (!Initialized) await InitialPlayerManager();
    }

    public async ValueTask OnLogin()
    {
        _playerInstances.Add(this);
        await Task.CompletedTask;
    }

    public static PlayerInstance? GetPlayerInstanceByUid(long uid)
        => _playerInstances.FirstOrDefault(player => player.Uid == uid);
    public void OnLogoutAsync()
    {
        _playerInstances.Remove(this);
    }
    public async ValueTask SendPacket(BasePacket packet)
    {
        if (Connection?.IsOnline == true) await Connection.SendPacket(packet);
    }

    #endregion

    #region Actions
    public async ValueTask OnHeartBeat()
    {
        DatabaseHelper.ToSaveUidList.SafeAdd(Uid);
        await Task.CompletedTask;
    }

    #endregion

    #region Serialization

    #endregion
}