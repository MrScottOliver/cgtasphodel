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
 * Not my code!
******************************************************************************/

namespace GameStateManagement
{
    class QuaternionCamera
    {
        public static Matrix GetViewMatrix(ref Vector3 position, ref Vector3 target, ref Vector3 up, float yaw, float pitch, float roll)
        {
            // The right vector can be inferred
            Vector3 forward = target - position;
            Vector3 right = Vector3.Cross(forward, up);

            // This quaternion is the total of all the
            // specified rotations
            Quaternion yawpitch = CreateFromYawPitchRoll(up, yaw,
                right, pitch, forward, roll);

            // Calculate the new target position, and the
            // new up vector by transforming the quaternion
            target = position + Vector3.Transform(forward, yawpitch);
            up = Vector3.Transform(up, yawpitch);

            return Matrix.CreateLookAt(position, target, up);
        }
        public static Quaternion CreateFromYawPitchRoll(Vector3 up, float yaw, Vector3 right, float pitch, Vector3 forward, float roll)
        {
            // Create a quaternion for each rotation, and multiply them
            // together.  We normalize them to avoid using the conjugate
            Quaternion qyaw = Quaternion.CreateFromAxisAngle(up, (float)yaw);
            qyaw.Normalize();
            Quaternion qtilt = Quaternion.CreateFromAxisAngle(right, (float)pitch);
            qtilt.Normalize();
            Quaternion qroll = Quaternion.CreateFromAxisAngle(forward, (float)roll);
            qroll.Normalize();
            Quaternion yawpitch = qyaw * qtilt * qroll;
            yawpitch.Normalize();

            return yawpitch;
        }
    }
}
