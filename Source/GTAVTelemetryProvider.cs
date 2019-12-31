//MIT License
//
//Copyright(c) 2019 PHARTGAMES
//
//Permission is hereby granted, free of charge, to any person obtaining a copy
//of this software and associated documentation files (the "Software"), to deal
//in the Software without restriction, including without limitation the rights
//to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//copies of the Software, and to permit persons to whom the Software is
//furnished to do so, subject to the following conditions:
//
//The above copyright notice and this permission notice shall be included in all
//copies or substantial portions of the Software.
//
//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//SOFTWARE.
//
using SimFeedback.log;
using SimFeedback.telemetry;
using System;
using System.Diagnostics;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

namespace GTAVTelemetry
{
    /// <summary>
    /// Assetto Corsa Telemetry Provider
    /// </summary>
    public sealed class GTAVTelemetryProvider : AbstractTelemetryProvider
    {
        private bool isStopped = true;                                  // flag to control the polling thread
        private Thread t;                                               // the polling thread, reads telemetry data and sends TelemetryUpdated events
        int PORTNUM = 20777;
        private IPEndPoint _senderIP;                   // IP address of the sender for the udp connection used by the worker thread

        /// <summary>
        /// Default constructor.
        /// Every TelemetryProvider needs a default constructor for dynamic loading.
        /// Make sure to call the underlying abstract class in the constructor.
        /// </summary>
        public GTAVTelemetryProvider() : base()
        {
            Author = "PEZZALUCIFER";
            Version = "v1.0";
            BannerImage = @"img\banner_gtav.png"; // Image shown on top of the profiles tab
            IconImage = @"img\gtav.jpg";          // Icon used in the tree view for the profile
            TelemetryUpdateFrequency = 100;     // the update frequency in samples per second
        }

        /// <summary>
        /// Name of this TelemetryProvider.
        /// Used for dynamic loading and linking to the profile configuration.
        /// </summary>
        public override string Name { get { return "gtav"; } }

        public override void Init(ILogger logger)
        {
            base.Init(logger);
            Log("Initializing GTAVTelemetryProvider");
        }

        /// <summary>
        /// A list of all telemetry names of this provider.
        /// </summary>
        /// <returns>List of all telemetry names</returns>
        public override string[] GetValueList()
        {
            return GetValueListByReflection(typeof(GTAVAPI));
        }

        /// <summary>
        /// Start the polling thread
        /// </summary>
        public override void Start()
        {
            if (isStopped)
            {
                LogDebug("Starting GTAVTelemetryProvider");
                isStopped = false;
                t = new Thread(Run);
                t.Start();
            }
        }

        /// <summary>
        /// Stop the polling thread
        /// </summary>
        public override void Stop()
        {
            LogDebug("Stopping GTAVTelemetryProvider");
            isStopped = true;
            if (t != null) t.Join();
        }

        /// <summary>
        /// The thread funktion to poll the telemetry data and send TelemetryUpdated events.
        /// </summary>
        private void Run()
        {
            GTAVAPI lastTelemetryData = new GTAVAPI();
            Session session = new Session();
            Stopwatch sw = new Stopwatch();
            sw.Start();

            UdpClient socket = new UdpClient();
            socket.ExclusiveAddressUse = false;
            socket.Client.Bind(new IPEndPoint(IPAddress.Any, PORTNUM));

            Log("Listener started (port: " + PORTNUM.ToString() + ") GTAVTelemetryProvider.Thread");
            while (!isStopped)
            {
                try
                {
                    // get data from game, 
                    if (socket.Available == 0)
                    {
                        if (sw.ElapsedMilliseconds > 500)
                        {
                            IsRunning = false;
                            IsConnected = false;
                            Thread.Sleep(1000);
                        }
                        continue;
                    }
                    else
                    {
                        IsConnected = true;
                    }

                    Byte[] received = socket.Receive(ref _senderIP);


                    var alloc = GCHandle.Alloc(received, GCHandleType.Pinned);
                    GTAVAPI telemetryData = (GTAVAPI)Marshal.PtrToStructure(alloc.AddrOfPinnedObject(), typeof(GTAVAPI));

                    // otherwise we are connected
                    IsConnected = true;

                    if (telemetryData.PacketId != lastTelemetryData.PacketId)
                    {
                        IsRunning = true;

                        sw.Restart();

                        TelemetryEventArgs args = new TelemetryEventArgs(
                            new GTAVTelemetryInfo(telemetryData, lastTelemetryData));
                        RaiseEvent(OnTelemetryUpdate, args);

                        lastTelemetryData = telemetryData;
                    }
                    else if (sw.ElapsedMilliseconds > 500)
                    {
                        IsRunning = false;
                    }
                }
                catch (Exception e)
                {
                    LogError("GTAVTelemetryProvider Exception while processing data", e);
                    IsConnected = false;
                    IsRunning = false;
                    Thread.Sleep(1000);
                }
            }

            socket.Close();
            IsConnected = false;
            IsRunning = false;
        }

    }
}
