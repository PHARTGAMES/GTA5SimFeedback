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
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using GTA;
using GTA.Native;
using GTA.Math;

namespace GTAVTelemetry
{
    class GTA5TelemetryClient : Script
    {
        GTAVAPI dataPacket = new GTAVAPI(); 
        private Vector3 lastLocalVelocity = new Vector3(0, 0, 0);
        private Vector3 lastAngularVelocity = new Vector3(0, 0, 0);
        private TelemetrySender telemetrySender;
        private Int32 port = 20777;

        NoiseFilter pitchFilter = new NoiseFilter(20); //higher number, more smoothing
        NoiseFilter yawFilter = new NoiseFilter(1);
        NoiseFilter rollFilter = new NoiseFilter(20);

        NoiseFilter surgeFilter = new NoiseFilter(10);

        //KalmanFilter pitchFilter = new KalmanFilter(1, 1, 0.125f, 1, 0.1f, 0.0f);
        //KalmanFilter yawFilter = new KalmanFilter(1, 1, 0.125f, 1, 0.1f, 0.0f);
        //KalmanFilter rollFilter = new KalmanFilter(1, 1, 0.125f, 1, 0.1f, 0.0f);
        public GTA5TelemetryClient()
        {
            dataPacket.Initialize();
            telemetrySender = new TelemetrySender(port);
            Tick += OnTick;
            Interval = 0;// 10;
        }


        void OnTick(object sender, EventArgs e)
        {
            Ped player = Game.Player.Character;

            if (player.IsInVehicle())
            {
                // Player in vehicle
                Vehicle vehicle = player.CurrentVehicle;

                Matrix ltw = vehicle.Matrix;

                ltw.M41 = 0;
                ltw.M42 = 0;
                ltw.M43 = 0;

                Vector3 localVelocity = ltw.InverseTransformPoint(vehicle.Velocity);

                localVelocity.X = float.IsNaN(localVelocity.X) ? 0.0f : localVelocity.X;
                localVelocity.Y = float.IsNaN(localVelocity.Y) ? 0.0f : localVelocity.Y;
                localVelocity.Z = float.IsNaN(localVelocity.Z) ? 0.0f : localVelocity.Z;

                Vector3 acceleration = localVelocity - lastLocalVelocity;

                dataPacket.AccG[0] = float.IsNaN(acceleration.X) ? 0.0f : acceleration.X;
                dataPacket.AccG[1] = surgeFilter.Filter(float.IsNaN(acceleration.Y) ? 0.0f : acceleration.Y);
                dataPacket.AccG[2] = float.IsNaN(acceleration.Z) ? 0.0f : acceleration.Z;

                lastLocalVelocity = localVelocity;

                dataPacket.SpeedKmh = vehicle.Speed;
                dataPacket.Rpm = vehicle.CurrentRPM;

                dataPacket.LocalAngularVelocity[0] = float.IsNaN(vehicle.RotationVelocity.X) ? 0.0f : vehicle.RotationVelocity.X;
                dataPacket.LocalAngularVelocity[1] = float.IsNaN(vehicle.RotationVelocity.Y) ? 0.0f : vehicle.RotationVelocity.Y;
                dataPacket.LocalAngularVelocity[2] = float.IsNaN(vehicle.RotationVelocity.Z) ? 0.0f : vehicle.RotationVelocity.Z; 

                dataPacket.LocalVelocity[0] = localVelocity.X;
                dataPacket.LocalVelocity[1] = localVelocity.Y;
                dataPacket.LocalVelocity[2] = localVelocity.Z;

                Vector3 angularAcceleration = (vehicle.RotationVelocity - lastAngularVelocity) * (180.0f / 3.14f);

                float unfilteredPitch = float.IsNaN(vehicle.Rotation.X + angularAcceleration.X) ? 0.0f : vehicle.Rotation.X + angularAcceleration.X;
                float unfilteredRoll = float.IsNaN(vehicle.Rotation.Y + angularAcceleration.Y) ? 0.0f : vehicle.Rotation.Y + angularAcceleration.Y;
                float unfilteredYaw = float.IsNaN(vehicle.Rotation.Z + angularAcceleration.Z) ? 0.0f : vehicle.Rotation.Z + angularAcceleration.Z;

                dataPacket.Pitch = pitchFilter.Filter(unfilteredPitch);
                dataPacket.Roll = rollFilter.Filter(unfilteredRoll);
                dataPacket.Yaw = yawFilter.Filter(unfilteredYaw);

                lastAngularVelocity = vehicle.RotationVelocity;

            }
            else //walkies
            {

                Matrix ltw = player.Matrix;

                ltw.M41 = 0;
                ltw.M42 = 0;
                ltw.M43 = 0;

                Vector3 localVelocity = ltw.InverseTransformPoint(player.Velocity);

                localVelocity.X = float.IsNaN(localVelocity.X) ? 0.0f : localVelocity.X;
                localVelocity.Y = float.IsNaN(localVelocity.Y) ? 0.0f : localVelocity.Y;
                localVelocity.Z = float.IsNaN(localVelocity.Z) ? 0.0f : localVelocity.Z;

                Vector3 acceleration = localVelocity - lastLocalVelocity;

                acceleration *= 0.125f; // need to scale this a bit otherwise it's super jerky

                dataPacket.AccG[0] = float.IsNaN(acceleration.X) ? 0.0f : acceleration.X;
                dataPacket.AccG[1] = surgeFilter.Filter(float.IsNaN(acceleration.Y) ? 0.0f : acceleration.Y);
                dataPacket.AccG[2] = float.IsNaN(acceleration.Z) ? 0.0f : acceleration.Z;

                lastLocalVelocity = localVelocity;

                dataPacket.LocalAngularVelocity[0] = 0.0f;
                dataPacket.LocalAngularVelocity[1] = 0.0f;
                dataPacket.LocalAngularVelocity[2] = 0.0f;

                dataPacket.Pitch = float.IsNaN(player.Rotation.X) ? 0.0f : player.Rotation.X;
                dataPacket.Roll = float.IsNaN(player.Rotation.Y) ? 0.0f : player.Rotation.Y;
                dataPacket.Yaw = float.IsNaN(player.Rotation.Z) ? 0.0f : player.Rotation.Z;

                lastAngularVelocity = Vector3.Zero;
            }

            //loop packet id
            if (dataPacket.PacketId == int.MaxValue)
                dataPacket.PacketId = 0;

            dataPacket.PacketId++;

            byte[] bytes = dataPacket.ToByteArray();
            telemetrySender.SendAsync(bytes);
        }
    }
}
