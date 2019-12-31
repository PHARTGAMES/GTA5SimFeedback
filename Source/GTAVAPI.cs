﻿//MIT License
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
using System.Runtime.InteropServices;

namespace GTAVTelemetry
{
    /// <summary>
    /// Helper struct to name the array positions
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Coordinates
    {
        public float X;
        public float Y;
        public float Z;
    }

    /// <summary>
    /// The data packet for sending over udp + some named properties 
    /// for human friendly mapping and stateless calculations
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 4, CharSet = CharSet.Unicode)]
    public struct GTAVAPI
    {
        #region Fields

        public int PacketId;
        public float Gas;
        public float Brake;
        public float Fuel;
        public int Gear;
        public float Rpm;
        public float SteerAngle;
        public float SpeedKmh;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public float[] Velocity;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public float[] AccG;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public float[] WheelSlip;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public float[] WheelLoad;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public float[] WheelsPressure;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public float[] WheelAngularSpeed;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public float[] TyreWear;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public float[] TyreDirtyLevel;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public float[] TyreCoreTemperature;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public float[] CamberRad;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public float[] SuspensionTravel;

        public float Drs;
        public float TC;
        public float Yaw;
        public float Pitch;
        public float Roll;
        public float CgHeight;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
        public float[] CarDamage;

        public int NumberOfTyresOut;
        public int PitLimiterOn;
        public float Abs;

        public float KersCharge;
        public float KersInput;
        public int AutoShifterOn;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public float[] RideHeight;

        public float TurboBoost;
        public float Ballast;
        public float AirDensity;

        public float AirTemp;
        public float RoadTemp;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public float[] LocalAngularVelocity;
        public float FinalFF;

        public float PerformanceMeter;
        public int EngineBrake;
        public int ErsRecoveryLevel;
        public int ErsPowerLevel;
        public int ErsHeatCharging;
        public int ErsisCharging;
        public float KersCurrentKJ;
        public int DrsAvailable;
        public int DrsEnabled;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public float[] BrakeTemp;

        public float Clutch;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public float[] TyreTempI;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public float[] TyreTempM;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public float[] TyreTempO;

        public int IsAIControlled;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public Coordinates[] TyreContactPoint;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public Coordinates[] TyreContactNormal;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public Coordinates[] TyreContactHeading;
        public float BrakeBias;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public float[] LocalVelocity;

        #endregion

        public void Initialize()
        {

            Velocity = new float[3];
            AccG = new float[3];
            WheelSlip = new float[4];
            WheelLoad = new float[4];
            WheelsPressure = new float[4];
            WheelAngularSpeed = new float[4];
            TyreWear = new float[4];
            TyreDirtyLevel = new float[4];
            TyreCoreTemperature = new float[4];
            CamberRad = new float[4];
            SuspensionTravel = new float[4];
            CarDamage = new float[5];

            RideHeight = new float[2];

            LocalAngularVelocity = new float[3];
            BrakeTemp = new float[4];

            TyreTempI = new float[4];
            TyreTempM = new float[4];
            TyreTempO = new float[4];

            TyreContactPoint = new Coordinates[4];
            TyreContactNormal = new Coordinates[4];
            TyreContactHeading = new Coordinates[4];

            LocalVelocity = new float[3];
        }

        #region Properties

        public float Heave
        {
            get
            {
                return AccG[2];
            }
        }

        public float Sway
        {
            get
            {
                return AccG[0];
            }
        }

        public float Surge
        {
            get
            {
                return AccG[1];
            }
        }

        public float Speed
        {
            get
            {
                return SpeedKmh;
            }
        }

        public float RPM
        {
            get
            {
                return Rpm;
            }
        }

        public float SlipAngle
        {
            get
            {
                float slipAngle = 0.0f;
                if (SpeedKmh > 5)
                {
                    // Porsche GT3 Cup
                    // Fahrzeug Länge: 4.564
                    // Radstand: 1.980 x 2.456
                    slipAngle = (float)(Math.Atan(
                        (LocalVelocity[0] - LocalAngularVelocityZ * (1.980f / 2))
                        /
                        (LocalVelocity[1] - LocalAngularVelocityZ * (2.456f / 2))
                        ));
                }
                return slipAngle;
            }
        }


        public float SlipAngle2
        {
            get
            {
                float slipAngle = 0.0f;
                double Vx = AccG[2] + LocalAngularVelocity[2] * LocalVelocity[0];
                double Vy = AccG[0] + LocalAngularVelocity[2] * LocalVelocity[2];
                slipAngle = (float)(Math.Atan(Vy / Vx) * 180 / Math.PI);
                return slipAngle;
            }
        }

        /// <summary>
        /// Placeholder for the stateful property.
        /// This will propergate the Name list for available Telemetry Keys.
        /// </summary>
        public float Rumble
        {
            get { return 0; }
        }



        public float SuspensionTravelAll
        {
            get
            {
                return
                    Math.Abs(SuspensionTravel[0]) +
                    Math.Abs(SuspensionTravel[1]) +
                    Math.Abs(SuspensionTravel[2]) +
                    Math.Abs(SuspensionTravel[3]) / 4;
            }
        }


        public float SuspensionTravelFL
        {
            get
            {
                return SuspensionTravel[0];
            }
        }

        public float SuspensionTravelFR
        {
            get
            {
                return SuspensionTravel[1];
            }
        }

        public float SuspensionTravelRL
        {
            get
            {
                return SuspensionTravel[2];
            }
        }

        public float SuspensionTravelRR
        {
            get
            {
                return SuspensionTravel[3];
            }
        }

        public float VelocityX
        {
            get
            {
                return LocalVelocity[0];
            }
        }
        public float VelocityY
        {
            get
            {
                return LocalVelocity[1];
            }
        }
        public float VelocityZ
        {
            get
            {
                return LocalVelocity[2];
            }
        }

        public float LocalAngularVelocityX
        {
            get
            {
                return LocalAngularVelocity[0];
            }
        }

        public float LocalAngularVelocityY
        {
            get
            {
                return LocalAngularVelocity[1];
            }
        }

        public float LocalAngularVelocityZ
        {
            get
            {
                return LocalAngularVelocity[2];
            }
        }

        #endregion
        public byte[] ToByteArray()
        {
            GTAVAPI packet = this;
            int num = Marshal.SizeOf<GTAVAPI>(packet);
            byte[] array = new byte[num];
            IntPtr intPtr = Marshal.AllocHGlobal(num);
            Marshal.StructureToPtr<GTAVAPI>(packet, intPtr, false);
            Marshal.Copy(intPtr, array, 0, num);
            Marshal.FreeHGlobal(intPtr);
            return array;
        }
    }


}
