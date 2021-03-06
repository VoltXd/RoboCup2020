﻿using Constants;
using EventArgsLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace RobotMessageProcessor
{
    public class RobotMsgProcessor
    {
        public RobotMsgProcessor()
        {

        }


        //Input CallBack        
        public void ProcessRobotDecodedMessage(object sender, MessageDecodedArgs e)
        {
            ProcessDecodedMessage((Int16)e.MsgFunction,(Int16) e.MsgPayloadLength, e.MsgPayload);
        }
        //Processeur de message en provenance du robot...
        //Une fois processé, le message sera transformé en event sortant
        public void ProcessDecodedMessage(Int16 command, Int16 payloadLength, byte[] payload)
        {
            switch(command)
            {
                case (short)Commands.IMUData:
                    {
                        uint timeStamp = (uint)(payload[3] | payload[2] << 8 | payload[1] << 16 | payload[0] << 24);
                        byte[] tab = payload.GetRange(4, 4);
                        float accelX = tab.GetFloat();
                        tab = payload.GetRange(8, 4);
                        float accelY = tab.GetFloat();
                        tab = payload.GetRange(12, 4);
                        float accelZ = tab.GetFloat();
                        tab = payload.GetRange(16, 4);
                        float gyroX = tab.GetFloat();
                        tab = payload.GetRange(20, 4);
                        float gyroY = tab.GetFloat();
                        tab = payload.GetRange(24, 4);
                        float gyroZ = tab.GetFloat();

                        Point3D accelXYZ = new Point3D(accelX, accelY, accelZ);
                        Point3D gyroXYZ = new Point3D(gyroX, gyroY, gyroZ);

                        //On envois l'event aux abonnés
                        OnIMUDataFromRobot(timeStamp, accelXYZ, gyroXYZ);
                    }
                    break;

                case (short)Commands.MotorCurrents:
                    {
                        uint time = (uint)(payload[3] | payload[2] << 8 | payload[1] << 16 | payload[0] << 24);
                        byte[] tab = payload.GetRange(4, 4);
                        float motor1Current = tab.GetFloat();
                        tab = payload.GetRange(8, 4);
                        float motor2Current = tab.GetFloat();
                        tab = payload.GetRange(12, 4);
                        float motor3Current = tab.GetFloat();
                        tab = payload.GetRange(16, 4);
                        float motor4Current = tab.GetFloat();
                        tab = payload.GetRange(20, 4);
                        float motor5Current = tab.GetFloat();
                        tab = payload.GetRange(24, 4);
                        float motor6Current = tab.GetFloat();
                        tab = payload.GetRange(28, 4);
                        float motor7Current = tab.GetFloat();
                        //On envois l'event aux abonnés
                        OnMotorsCurrentsFromRobot(time, motor1Current, motor2Current, motor3Current, motor4Current, motor5Current, motor6Current, motor7Current);
                    }
                    break;
                case (short)Commands.EnableDisableMotors:
                    bool value = Convert.ToBoolean(payload[0]);
                    //On envois l'event aux abonnés
                    OnEnableDisableMotorsACKFromRobot(value);
                    break;
                case (short)Commands.EnableDisableTir:
                    value = Convert.ToBoolean(payload[0]);
                    //On envois l'event aux abonnés
                    OnEnableDisableTirACKFromRobot(value);
                    break;
                default: break;
            }
        }

        //Output events
        public delegate void IMUDataEventHandler(object sender, IMUDataEventArgs e);
        public event EventHandler<IMUDataEventArgs> OnIMUDataFromRobotGeneratedEvent;
        public virtual void OnIMUDataFromRobot(uint timeStamp, Point3D accelxyz, Point3D gyroxyz)
        {
            var handler = OnIMUDataFromRobotGeneratedEvent;
            if (handler != null)
            {
                handler(this, new IMUDataEventArgs { timeStampMS = timeStamp, accelX = accelxyz.X, accelY = accelxyz.Y, accelZ= accelxyz.Z , gyrX=gyroxyz.X, gyrY=gyroxyz.Y, gyrZ=gyroxyz.Z });
            }
        }

        public delegate void EnableDisableMotorsEventHandler(object sender, BoolEventArgs e);
        public event EventHandler<BoolEventArgs> OnEnableDisableMotorsACKFromRobotGeneratedEvent;
        public virtual void OnEnableDisableMotorsACKFromRobot(bool isEnabled)
        {
            var handler = OnEnableDisableMotorsACKFromRobotGeneratedEvent;
            if (handler != null)
            {
                handler(this, new BoolEventArgs { value=isEnabled });
            }
        }

        public delegate void EnableDisableTirEventHandler(object sender, BoolEventArgs e);
        public event EventHandler<BoolEventArgs> OnEnableDisableTirACKFromRobotGeneratedEvent;
        public virtual void OnEnableDisableTirACKFromRobot(bool isEnabled)
        {
            var handler = OnEnableDisableTirACKFromRobotGeneratedEvent;
            if (handler != null)
            {
                handler(this, new BoolEventArgs { value = isEnabled });
            }
        }

        public delegate void MotorsCurrentsEventHandler(object sender, MotorsCurrentsEventArgs e);
        public event EventHandler<MotorsCurrentsEventArgs> OnMotorsCurrentsFromRobotGeneratedEvent;
        public virtual void OnMotorsCurrentsFromRobot(uint timeStamp, double m1A, double m2A, double m3A,
                                                                        double m4A, double m5A, double m6A, double m7A)
        {
            var handler = OnMotorsCurrentsFromRobotGeneratedEvent;
            if (handler != null)
            {
                handler(this, new MotorsCurrentsEventArgs { timeStampMS = timeStamp,
                                                                motor1 = m1A,
                                                                motor2 = m2A,
                                                                motor3 = m3A,
                                                                motor4 = m4A,
                                                                motor5 = m5A,
                                                                motor6 = m6A,
                                                                motor7 = m7A});
            }
        }


    }
}
