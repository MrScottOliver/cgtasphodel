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
 * Class: ObjectManipulator
 * 
 * Static class designed to access and manage all the level objects created,
 * 
******************************************************************************/

namespace Prototype
{
    static class ObjectManipulator
    {
        public static List<LevelObject> LevelData;                  //A list of LevelObject(s)
        public static List<LevelObject>.Enumerator iterator;                   //An iterator to move through various lists

        public static int VertexBufferSize = 0;      //Total amount of vertices to be drawn
        public static int IndexBufferSize = 0;      //Total amount of indices to be used

        public static VertexBuffer vBuffer;                    //The vertex buffer
        public static IndexBuffer iBuffer;                    //The index buffer

        public static VertexPositionNormalTexture[] Vertices;                   //An array containing all the vertices to be drawn
        public static short[] Indices;                    //An array of all the indices to be used

        public static int vLength = 0;                //The first array element not used in Vertices
        public static int iLength = 0;                //The first array element not used in Indices

        public static VertexDeclaration vertexFormat;               //Analogous to the Flexible Vertex Format

        public static int currentObject = -1;

        static ObjectManipulator()
        {
            LevelData = new List<LevelObject>();
            iterator = new List<LevelObject>.Enumerator();
        }

        public static void Initialise(GraphicsDevice device)
        {
            vertexFormat = new VertexDeclaration(device, VertexPositionNormalTexture.VertexElements);
        }

        public static void NewLevelObject()
        {
            LevelData.Add(new LevelObject());
            currentObject++;
        }

        public static void NewLevelObject(int vSize, int iSize, PrimitiveType prim, short primAmount)
        {
            LevelData.Add(new LevelObject(vSize, iSize, prim, primAmount));

            currentObject++;
        }

        public static void CalculateBufferLengths()
        {
            Vertices = new VertexPositionNormalTexture[VertexBufferSize];
            Indices = new short[IndexBufferSize];
        }

        public static void ConcatanateArrays()
        {

            foreach (LevelObject obj in LevelData)
            {
                //int i = 0;

                obj.vertexData.CopyTo(Vertices, vLength);
                /*
                while( i < obj.indexData.Length )
                {
                    obj.indexData[i] += (short)vLength;
                    i++;
                }
                //*/
                obj.indexData.CopyTo(Indices, iLength);

                vLength += obj.vertexData.Length;
                iLength += obj.indexData.Length;

            }
                        vLength = 0;
            iLength = 0;
        }

        public static void CreateBuffers(GraphicsDevice device)
        {
            vBuffer = new VertexBuffer(device, Vertices.Length * VertexPositionNormalTexture.SizeInBytes, BufferUsage.WriteOnly);
            vBuffer.SetData(Vertices);
            iBuffer = new IndexBuffer(device, typeof(short), Indices.Length, BufferUsage.WriteOnly);
            iBuffer.SetData(Indices);
        }

        public static void Draw(GraphicsDevice device, BasicEffect std, Effect effect, Matrix view, Matrix proj, GrowEvent P, Vector3 CamPos, Lights[] light)
        {
            int vOffset = 0;
            int iOffset = 0;

            Vector4 CamPosition = new Vector4(CamPos.X, CamPos.Y, CamPos.Z, 0);

            foreach (LevelObject obj in LevelData)
            {
                


                if (obj.basicEffect)
                {
                    std.EnableDefaultLighting();
                    std.PreferPerPixelLighting = true;

                    std.World = obj.world;
                    std.Projection = proj;
                    std.View = view;

                    std.Begin();

                    foreach (EffectPass pass in std.CurrentTechnique.Passes)
                    {
                        pass.Begin();

                        device.Indices = iBuffer;
                        device.Vertices[0].SetSource(vBuffer, 0, VertexPositionNormalTexture.SizeInBytes);
                        device.DrawUserIndexedPrimitives(obj.primType, Vertices, vOffset, obj.vertexData.Length, Indices, iOffset, obj.primAmount);

                        pass.End();
                    }

                    std.End();

                    vOffset += obj.vertexData.Length;
                    iOffset += obj.indexData.Length;
                }
                else
                {
                    //Jess: set wv, wvIT and  wvp matrices in here as obj.world is required//
                    Matrix wvp = obj.world * view * proj;
                    Matrix wv = obj.world * view;

                    Matrix wvIT = Matrix.Invert(wv);
                    wvIT = Matrix.Transpose(wvIT);

                    Matrix worldIT = Matrix.Invert(obj.world);
                    worldIT = Matrix.Transpose(worldIT);

                    effect.Parameters["gWVP"].SetValue(wvp);
                    effect.Parameters["gWorldView"].SetValue(wv);
                    effect.Parameters["gWorldViewIT"].SetValue(wvIT);
                    effect.Parameters["gWorld"].SetValue(obj.world);

                    effect.Parameters["gWorldIT"].SetValue(worldIT);
                    effect.Parameters["gCamPosW"].SetValue(CamPosition);


                    //set material params for each object
                    effect.Parameters["gAmbMtrl"].SetValue(obj.ambMtrl);
                    effect.Parameters["gDiffuseMtrl"].SetValue(obj.diffMtrl);
                    effect.Parameters["gSpecMtrl"].SetValue(obj.specMtrl);

                    //set texture
                    effect.Parameters["gTex"].SetValue(obj.tex);


                  

                    effect.Begin();


                    effect.CurrentTechnique.Passes["PointLights"].Begin();

                    device.DrawUserIndexedPrimitives(obj.primType, Vertices, vOffset, obj.vertexData.Length, Indices, iOffset, obj.primAmount);

                    effect.CurrentTechnique.Passes["PointLights"].End();


                    effect.End();

                    vOffset += obj.vertexData.Length;
                    iOffset += obj.indexData.Length;
                }
            }
        }

        public static LevelObject Current()
        {
            return LevelData.ElementAt(currentObject);
        }

        public static void UpdateObjects(GraphicsDevice device)
        {
            vLength = 0;
            iLength = 0;
            CalculateBufferLengths();
            ConcatanateArrays();
            CreateBuffers(device);
        }
    }
}
