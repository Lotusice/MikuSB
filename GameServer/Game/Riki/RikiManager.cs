using MikuSB.Data;
using MikuSB.Enums.Item;
using MikuSB.GameServer.Game.Player;
using MikuSB.Proto;

namespace MikuSB.GameServer.Game.Riki;

public sealed class RikiManager(PlayerInstance player) : BasePlayerManager(player)
{
    public void UnlockItem(
        ItemTypeEnum genre,
        uint detail,
        uint particular,
        uint level,
        NtfSyncPlayer? sync = null)
    {
        if (genre is not (ItemTypeEnum.TYPE_CARD or ItemTypeEnum.TYPE_WEAPON))
        {
            return;
        }

        var type = (uint)genre;
        foreach (var riki in GameData.RikiData.Values)
        {
            if (riki.Type != type ||
                riki.Condition[0] != type ||
                riki.Condition[1] != detail ||
                riki.Condition[2] != particular ||
                riki.Condition[3] != level)
            {
                continue;
            }

            SetBit(riki.Id, sync);
        }
    }

    public void EnsureOwnedItemsUnlocked(NtfSyncPlayer? sync = null)
    {
        foreach (var character in Player.CharacterManager.CharacterData.Characters)
        {
            var card = GameData.CardData.Values.FirstOrDefault(x =>
                GameResourceTemplateId.FromGdpl(x.Genre, x.Detail, x.Particular, x.Level) == character.TemplateId);
            if (card != null)
            {
                UnlockItem((ItemTypeEnum)card.Genre, card.Detail, card.Particular, card.Level, sync);
            }
        }

        foreach (var weapon in Player.InventoryManager.InventoryData.Weapons.Values)
        {
            var config = GameData.WeaponData.Values.FirstOrDefault(x =>
                GameResourceTemplateId.FromGdpl(x.Genre, x.Detail, x.Particular, x.Level) == weapon.TemplateId);
            if (config != null)
            {
                UnlockItem((ItemTypeEnum)config.Genre, config.Detail, config.Particular, config.Level, sync);
            }
        }
    }

    private void SetBit(uint rikiId, NtfSyncPlayer? sync)
    {
        var (sid, bit) = GetTaskPosition(rikiId);
        var mask = 1u << bit;

        SetBit(sid, mask, sync);
        SetBit(sid + 500, mask, sync);
    }

    private void SetBit(uint sid, uint mask, NtfSyncPlayer? sync)
    {
        var attr = Player.Attributes.GetOrCreate(AttrIds.Riki.TaskGroupId, sid);
        if ((attr.Val & mask) == mask)
        {
            return;
        }

        attr.Val |= mask;
        if (sync != null)
        {
            Player.Attributes.SyncTo(sync, attr);
        }
    }

    private static (uint Sid, int Bit) GetTaskPosition(uint rikiId)
    {
        var index = rikiId / 1000;
        var value = rikiId % 1000;
        if (index == 0 || value == 0)
        {
            throw new ArgumentOutOfRangeException(nameof(rikiId), rikiId, "Riki ID must use the type-prefixed format.");
        }

        var sid = ((index - 1) * 30) + ((value + 29) / 30);
        return (sid, (int)(value % 30));
    }
}
