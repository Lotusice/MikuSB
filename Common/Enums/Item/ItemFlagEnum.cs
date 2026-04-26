namespace MikuSB.Enums.Item;

public enum ItemFlagEnum
{
    FLAG_USE = 1,// 使用中
    FLAG_LOCK = 2,// 锁定中
    FLAG_READED = 4,// 道具已查看
    FLAG_LEAVE = 8,// 角色大招后离场
    FLAG_WEAPON_DEFAULT = 16,// 武器显示原始样式
    FLAG_WEAPON_AUDIO = 32,// 武器消音器音效
    FLAG_ROLE_LIKE = 64,// 心选角色
}

