using System;
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace MSFSGPS
{
    internal class GpsBroadcaster : IDisposable
    {
        private readonly UdpClient _udpClient = new UdpClient();
        private readonly IPEndPoint _broadcastAddress = new IPEndPoint(IPAddress.Broadcast, 49002);

        public void BroadcastPosition(Position position)
        {
            var altitude = Math.Round(position.Altitude, 3).ToString(CultureInfo.InvariantCulture);
            var longitude = Math.Round(position.Longitude, 3).ToString(CultureInfo.InvariantCulture);
            var latitude = Math.Round(position.Latitude, 3).ToString(CultureInfo.InvariantCulture);
            var groundTrack = Math.Round(position.GroundTrack, 3).ToString(CultureInfo.InvariantCulture);
            var speed = Math.Round(position.GroundSpeed, 3).ToString(CultureInfo.InvariantCulture);
            byte[] sendBytes = Encoding.ASCII.GetBytes(string.Format("XGPSMSFSGPS,{0},{1},{2},{3},{4}", longitude, latitude, altitude, groundTrack, speed));
            _udpClient.Send(sendBytes, sendBytes.Length, _broadcastAddress);
        }

        public void BroadcastAttitudeData(Attitude attitude)
        {
            var heading = Math.Round(attitude.Heading, 3).ToString(CultureInfo.InvariantCulture);
            var pitch = Math.Round(attitude.Pitch, 3).ToString(CultureInfo.InvariantCulture);
            var roll = Math.Round(attitude.Bank, 3).ToString(CultureInfo.InvariantCulture);
            byte[] sendBytes = Encoding.ASCII.GetBytes(string.Format("XATTMSFSGPS,{0},{1},{2}", heading, pitch, roll));
            _udpClient.Send(sendBytes, sendBytes.Length, _broadcastAddress);
        }

        public void Dispose() => _udpClient.Dispose();
    }
}
