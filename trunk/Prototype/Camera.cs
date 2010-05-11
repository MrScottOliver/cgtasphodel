using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

/******************************************************************************
 * Namespace: Graphics_Code_SO
 * Class: Camera
 * 
 * Class which contains all the data required to orient and positon a camera, 
 * as well as methods to alter the aim and positon of the camera.
******************************************************************************/

namespace Prototype
{
    class Camera
    {
        public Matrix       view { get; set; }
        public Matrix       projection { get; set; }

        public Vector3      position { get; set; }
        public Vector3      lookAt { get; set; }
        public Vector3      up { get; set; }
        public Vector3      right { get; set; }
        
        private float       aspectRatio;
        private float       FOV;
        private float       nearClip;
        private float       farClip;

        private float       yaw;
        private float       pitch;
        private float       roll;

        private Quaternion  qYaw;
        private Quaternion  qTilt;
        private Quaternion  qRoll;

        private static bool invertY { get; set; }

        public Camera( Viewport port )
        {
            view = projection       = Matrix.Identity;
            position = lookAt       = Vector3.Zero;
            up                      = Vector3.Up;
            aspectRatio             = port.AspectRatio;
            FOV                     = MathHelper.ToRadians(60.0f);
            nearClip                = 1.0f;
            farClip                 = 1000.0f;

            yaw                     = 0;
            pitch                   = 0;
            roll                    = 0;

            right = Vector3.Cross( lookAt, up );
        }

        public Camera( Viewport port, float _fov, float near, float far )
        {
            view = projection   = Matrix.Identity;
            position = lookAt   = Vector3.Zero;
            up                  = Vector3.Up;
            aspectRatio         = port.AspectRatio;
            FOV                 = MathHelper.ToRadians(_fov);
            nearClip            = near;
            farClip             = far;

            yaw = 0;
            pitch = 0;
            roll = 0;

            right = Vector3.Cross(lookAt, up);
        }

        private void CalculateProjection()
        {
            projection = Matrix.CreatePerspectiveFieldOfView(FOV, aspectRatio, nearClip, farClip);
        }

        private void CalculateView()
        {
            view = Matrix.CreateLookAt(position, lookAt, up);
        }

        public void Update()
        {
            lookAt = Vector3.Transform(lookAt, CreateFromYawPitchRoll());
            up = Vector3.Transform(up, CreateFromYawPitchRoll());
            CalculateProjection();
            CalculateView();
        }

        public void MoveForward()
        {
            lookAt.Normalize();
            position = Vector3.Add(position, lookAt);
            //position.Normalize();
            //lookAt = Vector3.Add(lookAt, position);
        }

        public void MoveBackward()
        {
            lookAt.Normalize();
            position = Vector3.Subtract(position, lookAt);
        }

        public void MoveLeft()
        {
            right.Normalize();
            position = Vector3.Subtract(position, right);
            lookAt = Vector3.Subtract(lookAt, right);
        }

        public void MoveRight()
        {
            right.Normalize();
            position = Vector3.Add(position, right);
            lookAt = Vector3.Add(lookAt, right);
        }

        public void MoveUp()
        {
            up.Normalize();
            position = Vector3.Add(position, up);
            lookAt = Vector3.Add(lookAt, up);
        }

        public void MoveDown()
        {
            up.Normalize();
            position = Vector3.Subtract(position, up);
            lookAt = Vector3.Subtract(lookAt, up);
        }

        public void RotateAimX( float angle )
        {
            //if ( invert ) { angle *= -1; }
            yaw += angle;
            qYaw = Quaternion.CreateFromAxisAngle(up, (float)yaw);
            qYaw.Normalize();
        }

        public void RotateAimY( float angle )
        {
            if ( invertY ) { angle *= -1; }
            pitch += angle;
            qTilt = Quaternion.CreateFromAxisAngle(right, (float)pitch);
            qTilt.Normalize();
        }

        public void RotateAimZ( float angle )
        {
            roll += angle;
            qRoll = Quaternion.CreateFromAxisAngle(lookAt, (float)roll);
            qRoll.Normalize();
        }

        private Quaternion CreateFromYawPitchRoll()
        {
            Quaternion yawpitch = qYaw * qTilt * qRoll;
            yawpitch.Normalize();

            return yawpitch;
        }
    }
}
