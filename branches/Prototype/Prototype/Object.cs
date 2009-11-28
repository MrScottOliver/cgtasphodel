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

/******************************************************************************
 * Namespace: Graphics_Code_SO
 * Class: LevelObject
 * 
 * Contains all the information required to draw an object to screen, as well
 * as functions to define the information contained in the class.
******************************************************************************/

namespace Prototype
{
    class LevelObject
    {
        public VertexPositionNormalTexture[]    vertexData;         //An array containing vertex information
        public short[]                          indexData;          //An array containing indices into the vertex array

        public short                            primAmount;         //Amount of primitives in object
        
        public static int                       Instances = 0;      //A variable representing the total amount of Object(s)
        public bool                             basicEffect { set; get; }        //A flag which represents whether the object uses its own effect file
        
        private int                             currentVertex;      //An index into the vertexData array
        private int                             currentIndex;       //An index into the indexData array
        
        public PrimitiveType                    primType;           //Describes how to draw the vertices in the array, e.g. triangle list
        
        public Matrix                           translations;       //Matrix describing the movement of the object
        public Matrix                           rotations;          //Matrix describing the rotation of the object
        public Matrix                           world;              //World matrix for this object i.e. identity * rotations * translations

        public Texture2D                        tex { get; set; }   //Texture to be drawn on primitive

        public Vector4 ambMtrl = new Vector4(0.4f, 0.4f, 0.4f, 1.0f);//Jess: default material vals
        public Vector4 diffMtrl = new Vector4(0.5f, 0.5f, 0.5f, 1.0f);
        public Vector4 specMtrl = new Vector4(0.3f, 0.3f, 0.3f, 1.0f);

        public LevelObject()
        {
            vertexData = new VertexPositionNormalTexture[3];
            indexData = new short[3];
            ObjectManipulator.VertexBufferSize += 3;
            ObjectManipulator.IndexBufferSize += 3;
            Instances++;
            basicEffect = true;
            currentVertex = 0;
            currentIndex = 0;
            primType = PrimitiveType.TriangleList;
            translations = Matrix.Identity;
            rotations = Matrix.Identity;
            primAmount = 1;
        }

        public LevelObject(int vArraySize, int iArraySize, PrimitiveType primitive, short noPrim)
        {
            vertexData = new VertexPositionNormalTexture[vArraySize];
            indexData = new short[iArraySize];
            ObjectManipulator.VertexBufferSize += vArraySize;
            ObjectManipulator.IndexBufferSize += iArraySize;
            Instances++;
            basicEffect = true;
            currentVertex = 0;
            currentIndex = 0;
            primType = primitive;
            translations = Matrix.Identity;
            rotations = Matrix.Identity;
            primAmount = noPrim;
        }

        public void AddVertexPNT( float _x, float _y, float _z, float _nx, float _ny, float _nz, float _u, float _v )
        {
            if ( currentVertex < vertexData.Length )
            {
                vertexData[currentVertex].Position = new Vector3( _x, _y, _z );
                vertexData[currentVertex].Normal = new Vector3( _nx, _ny, _nz );
                vertexData[currentVertex].TextureCoordinate = new Vector2( _u, _v );
                currentVertex++;
            }
        }

        public void AddIndex( short index )
        {
            if ( currentIndex < indexData.Length )
            {
                indexData[currentIndex] = index;
                currentIndex++;
            }
        }
        public void AddIndex(short I1, short I2, short I3)
        {
            AddIndex(I1);
            AddIndex(I2);
            AddIndex(I3);
        }

        public void AddTranslation( float _x, float _y, float _z )
        {
            translations *= Matrix.CreateTranslation( _x, _y, _z );
            CalculateWorld();
        }

        public void AddRotationX( float angle )
        {
            rotations *= Matrix.CreateRotationX( MathHelper.ToRadians(angle) );
            CalculateWorld();
        }
        public void AddRotationY( float angle )
        {
            rotations *= Matrix.CreateRotationY( MathHelper.ToRadians(angle) );
            CalculateWorld();
        }
        public void AddRotationZ( float angle )
        {
            rotations *= Matrix.CreateRotationZ( MathHelper.ToRadians(angle) );
            CalculateWorld();
        }

        public void CalculateWorld()
        {
            world = rotations * translations;
        }

        //Jess:set object materials
        public void SetMaterials(Vector4 amb, Vector4 diff, Vector4 spec)
        {
            ambMtrl = amb;
            diffMtrl = diff;
            specMtrl = spec;
        }
        

    }
}
