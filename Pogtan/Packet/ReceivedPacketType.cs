namespace Pogtan.Packet;

public enum ReceivedPacketType : byte
{
    LoginRequest = 0x00,
    ChannelsRequest = 0x01,
    ConfirmIdRequest = 0x02,
    NewUserRequest = 0x03,
    ConnectToServerRequest = 0x04,
    ConnectToChannelRequest = 0x05,
    CrashInfo = 0x06,

    DisconnectFromChannelRequest = 0x08,
    EstimateRttRequest = 0x09,
    ChatMessage = 0x0A,
    CommandMessage = 0x0B,
    WhisperMessage = 0x0C,
    UserSimpleInfoRequest = 0x0D,
    MyInfoRequest = 0x0E,
    ModifyUserInfoRequest = 0x0F,
    UserDetailInfoRequest = 0x10,
    JoinSessionRequest = 0x11,
    UserLocationRequest = 0x12,
    GuildInfoRequest = 0x13,
    InviteUserRequest = 0x14,
    CreateSession = 0x15,
    JoinSession = 0x16,
    AutoJoinSession = 0x17,
    LeaveSession = 0x18,
    CreateSessionGuild = 0x19,

    ReadyStateRequest = 0x1B,
    SetSessionMapRequest = 0x1C,
    SetSlotStateRequest = 0x1D,
    BanPlayerRequest = 0x1E,
    AskBanPlayerAnswer = 0x1F,
    StartGameRequest = 0x20,
    ChangeSessionName = 0x21,
    BombIgnite = 0x22,
    BombKickThrow = 0x23,

    ThrowDart = 0x24,

    MovableBoxMove = 0x26,
    ItemEat = 0x27,
    ObstacleEat = 0x28,
    SetBomberEvent = 0x29,
    ThrowDartEffect = 0x2A,
    DropObstacle = 0x2B,
    ProlongLife = 0x2C,
    GameStageCheckIn = 0x2D,
    SetGameTypeNormal = 0x2E,
    SetGameTypeMod = 0x2F,

    GuildMemberSimpleInfoRequest = 0x32,
    Unknown51 = 0x33, // CDataI16Dyn::SetLoad
    OnlinePoll = 0x34,
    Unknown53 = 0x35, // CImageLib::ReportRevisionToGameServer

    No = 0x36
}
