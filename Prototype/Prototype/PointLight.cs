using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Prototype
{
    class PointLight
    {
        private Vector4 LightPos;
        private Vector4 DiffuseCol;
        private Vector4 AmbientCol;
        private Vector4 SpecularCol;
        private Vector3 AttenVal;

        public PointLight(Vector4 setPosition)
        {
            LightPos = setPosition;

        }

        public void UpdateLight(EffectParameter LightParameter)
        {
            LightParameter.StructureMembers["gAmbLight"].SetValue(AmbientCol);
            LightParameter.StructureMembers["gDiffuseLight"].SetValue(DiffuseCol);
            LightParameter.StructureMembers["gSpecLight"].SetValue(SpecularCol);
            LightParameter.StructureMembers["gLightPosW"].SetValue(LightPos);
            LightParameter.StructureMembers["gAtten123"].SetValue(AttenVal);
        }


        public Vector4 Position
        {
            set
            {
                LightPos = value;
            }
            get
            {
                return LightPos;
            }
        }

        public Vector4 Diffuse
        {
            set
            {
                DiffuseCol = value;
            }
            get
            {
                return DiffuseCol;
            }
        }

        public Vector4 Ambient
        {
            set
            {
                AmbientCol = value;
            }
            get
            {
                return AmbientCol;
            }
        }

        public Vector4 Specular
        {
            set
            {
                SpecularCol = value;
            }
            get
            {
                return AmbientCol;
            }
        }

        public Vector3 Attenuation
        {
            set
            {
                AttenVal = value;
            }
            get
            {
                return AttenVal;
            }
        }




    }
}

