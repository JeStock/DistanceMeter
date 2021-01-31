namespace CTeleport.DistanceMeter.IntegrationTests
{
    using System;
    using WireMock.Server;

    public static class ServerMock
    {
        private static readonly Lazy<WireMockServer> Lazy =
            new(() => WireMockServer.Start(Url));

        public static readonly string Url = "http://localhost:357";

        public static WireMockServer Instance => Lazy.Value;
    }
}