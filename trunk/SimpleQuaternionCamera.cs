#region Using Statements
using System;
using Microsoft.Xna.Framework;
#endregion


namespace QuaternionCamera
{
    class SimpleQuaternionCamera
    {
        public Vector3 Pos;
        public Vector3 Target;
        public Vector3 Up;
        public Vector3 Right;

        public Matrix View;

        public SimpleQuaternionCamera(Vector3 Pos, Vector3 Target, Vector3 Up)
        {
            this.Pos = Pos; this.Target = Target;
            View = Matrix.CreateLookAt(Pos, Target, Up);
            
            // Even if the up vector is off, we can get
            // a reliable right vector
            //Right = Vector3.Cross(Target - Pos, Up);
            //Right.Normalize();
            // Given a correct right vector, can recalculate up
            //this.Up = Vector3.Cross(Right, Target - Pos);
            //this.Up.Normalize();   

            // Shortcut to get the Up and Right vectors
            Right.X = View.M11; Right.Y = View.M21;
            Right.Z = View.M31; this.Up.X = View.M12;
            this.Up.Y = View.M22; this.Up.Z = View.M32;
        }
        public SimpleQuaternionCamera(Vector3 Pos, Vector3 Target) : this (Pos, Target, Vector3.Up)
        {
        }
        public void Rotate(float pan, float tilt, float roll)
        {
            View = GetViewMatrix(ref Pos, ref Target, ref Up, pan, tilt, roll);
            
            Right = Vector3.Cross(Target - Pos, Up);
            Right.Normalize();            
            Up = Vector3.Cross(Right, Target - Pos);
            Up.Normalize();
        }
        public void Translate(float forward, float right, float up)
        {
            // Move the camera position, and calculate a 
            // new target

            //Vector3 direction = Target - Pos;
            //direction.Normalize();
            // Shortcut to pull the above vector from the view matrix
            Vector3 direction = new Vector3(-View.M13, -View.M23, -View.M33);
            
            Pos += direction * forward;
            Pos += this.Right * right;
            Pos += this.Up * up;
            Target = Pos + direction;

            // Calculate the new view matrix
            View = Matrix.CreateLookAt(Pos, Target, Up);
        }
        public static Matrix GetViewMatrix(ref Vector3 position, ref Vector3 target, 
            ref Vector3 up, float yaw, float pitch, float roll)
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
