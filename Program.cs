﻿using FlightSimApi;
using System;
using System.Threading.Tasks;

namespace MSFSGPS
{
    internal enum Definitions
    {
        Attitude,
        Position,
    }

#pragma warning disable CS0649
    [SimConnectType(Definitions.Attitude, samplingFrequency: 100)]
    internal struct Attitude
    {
        [SimConnectValue("PLANE HEADING DEGREES TRUE", "Degrees")]
        public double Heading;

        [SimConnectValue("PLANE PITCH DEGREES", "Degrees")]
        public double Pitch;

        [SimConnectValue("PLANE BANK DEGREES", "Degrees")]
        public double Bank;
    }

    [SimConnectType(Definitions.Position, samplingFrequency: 1000)]
    internal struct Position
    {
        [SimConnectValue("PLANE LONGITUDE", "Degrees")]
        public double Longitude;

        [SimConnectValue("PLANE LATITUDE", "Degrees")]
        public double Latitude;

        [SimConnectValue("PLANE ALTITUDE", "Meters")]
        public double Altitude;

        [SimConnectValue("GPS GROUND TRUE TRACK", "Degrees")]
        public double GroundTrack;

        [SimConnectValue("GPS GROUND SPEED")]
        public double GroundSpeed;
    }
#pragma warning restore CS0649

    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Waiting for flight simulator...");
            var api = new SimConnectApi();
            using var gpsBroadcaster = new GpsBroadcaster();

            api.RegisterType<Attitude>();
            api.RegisterType<Position>();

            api.RegisterHandler<Attitude>(gpsBroadcaster.BroadcastAttitudeData);
            api.RegisterHandler<Position>(gpsBroadcaster.BroadcastPosition);

            api.Start();

            var lastConnectedState = false;

            while (true)
            {
                if (lastConnectedState != api.IsConnected)
                {
                    lastConnectedState = api.IsConnected;
                    Console.WriteLine($"{(api.IsConnected ? "Connected" : "Disconnected")} at {DateTime.UtcNow}");
                    if (!api.IsConnected)
                    {
                        Console.WriteLine("MSFSGPS will automatically reconnect...");
                    }
                }

                await Task.Delay(1000);
            }
        }
    }
}
