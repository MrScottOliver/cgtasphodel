﻿using System;
using System.Collections.Generic;
using JigLibX.Collision;
using JigLibX.Geometry;
using JigLibX.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Prototype
{
    // A physics object that simulates all the individual polygons in a model
    public class TriangleMeshObject : IPhysicsObject
    {
        // The generated mesh
        TriangleMesh triangleMesh;
        string modelName;

        // Constructors

        public TriangleMeshObject()
            : base()
        {
            Setup(Vector3.Zero, Vector3.Zero);
        }

        public TriangleMeshObject(Vector3 Position, Vector3 Rotation)
            : base()
        {
            
            Setup(Position, Rotation);
        }



        // Sets up the object
        void Setup(Vector3 Position, Vector3 Rotation)
        {
            InitializeBody();
            Body = new Body();
            CollisionSkin = new CollisionSkin();
            PhysicsSystem.CurrentPhysicsSystem.CollisionSystem.AddCollisionSkin(CollisionSkin);

            Body.CollisionSkin = CollisionSkin;

            Matrix mRotation = Matrix.CreateFromYawPitchRoll(Rotation.X, Rotation.Y, Rotation.Z);

            this.Position = Position;
            this.Rotation = mRotation;
        }

        // Sets the model being simulated, extracts vertices, etc.
        public void SetModel(string ModelName, Model _theModel)
        {


            Model Model = _theModel;
            this.modelName = ModelName;

            CollisionSkin.RemoveAllPrimitives();
            triangleMesh = new TriangleMesh();

            List<Vector3> vertexList = new List<Vector3>();
            List<TriangleVertexIndices> indexList =
                new List<TriangleVertexIndices>();

            ExtractData(vertexList, indexList, Model);

            triangleMesh.CreateMesh(vertexList, indexList, 4, 1.0f);
            CollisionSkin.AddPrimitive(triangleMesh, 
                new MaterialProperties(0.8f, 0.7f, 0.6f));

            Mass = Mass;
        }

        // Extracts the neccesary information from a model to 
        // simulate physics on it
        void ExtractData(List<Vector3> vertices,
            List<TriangleVertexIndices> indices, Model model)
        {
            Matrix[] bones_ = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(bones_);

            foreach (ModelMesh mm in model.Meshes)
            {
                Matrix xform = bones_[mm.ParentBone.Index];
                foreach (ModelMeshPart mmp in mm.MeshParts)
                {
                    int offset = vertices.Count;
                    Vector3[] a = new Vector3[mmp.NumVertices];
                    mm.VertexBuffer.GetData<Vector3>(
                        mmp.StreamOffset + mmp.BaseVertex
                            * mmp.VertexStride,
                        a, 0, mmp.NumVertices, mmp.VertexStride);
                    for (int i = 0; i != a.Length; ++i)
                        Vector3.Transform(ref a[i], ref xform,
                            out a[i]);
                    vertices.AddRange(a);

                    if (mm.IndexBuffer.IndexElementSize
                        != IndexElementSize.SixteenBits)
                        throw new Exception(
                            String.Format("Model uses 32-bit "
                            + "indices, which are not supported."));
                    short[] s = new short[mmp.PrimitiveCount * 3];
                    mm.IndexBuffer.GetData<short>(mmp.StartIndex * 2,
                        s, 0, mmp.PrimitiveCount * 3);
                    JigLibX.Geometry.TriangleVertexIndices[] tvi =
                        new JigLibX.Geometry.TriangleVertexIndices[
                            mmp.PrimitiveCount];
                    for (int i = 0; i != tvi.Length; ++i)
                    {
                        tvi[i].I0 = s[i * 3 + 2] + offset;
                        tvi[i].I1 = s[i * 3 + 1] + offset;
                        tvi[i].I2 = s[i * 3 + 0] + offset;
                    }
                    indices.AddRange(tvi);
                }
            }
        }
    }
}
