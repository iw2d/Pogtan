using Pogtan;
using Pogtan.Server;

LoginServer loginServer = new(ServerConfig.LoginPort);
GameServer gameServer = new(ServerConfig.ChannelPort);

Task.WaitAll(loginServer.Start(), gameServer.Start());
