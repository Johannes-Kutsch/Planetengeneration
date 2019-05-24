using PlanetGeneration.Planet;
using UnityEngine;

namespace PlanetGeneration
{
    public class TerrainFace
    {
        private readonly ShapeGenerator shapeGenerator;
        private readonly Mesh mesh;
        private readonly int resolution;
        private int NumVertices => resolution * resolution;
        private readonly Vector3 up;
        private readonly Vector3 forward;
        private readonly Vector3 right;

        private const int TrianglesPerFace = 2;
        private const int VerticesPerPolygon = 3;
        private const int FaceScale = 2;
        private const float FaceOffset = -0.5f;

        public TerrainFace(ShapeGenerator shapeGenerator, Mesh mesh, int resolution, Vector3 up)
        {
            this.shapeGenerator = shapeGenerator;
            this.mesh = mesh;
            this.resolution = resolution;
            this.up = up;
            
            forward = new Vector3(up.y, up.z, up.x);
            right = Vector3.Cross(up, forward);
        }

        public void ConstructMesh()
        {
            Vector2[] uv = (mesh.uv.Length == NumVertices)?mesh.uv:new Vector2[NumVertices];
            mesh.Clear();
            mesh.vertices = CreateVertices(uv);
            mesh.triangles = CreateTriangles();
            mesh.RecalculateNormals();
            mesh.uv = uv;
        }

        private Vector3[] CreateVertices(Vector2[] uv)
        {
            Vector3[] vertices = new Vector3[NumVertices];
            
            for (int y = 0; y < resolution; y++) {
                for (int x = 0; x < resolution; x++) {
                    Vector3 pointOnUnitSphere = GetPointOnSphere(x, y);
                    int index = GetIndex(x, y);
                    float unscaledElevation = shapeGenerator.CalculateUnscaledElevation(pointOnUnitSphere);
                    vertices[index] = shapeGenerator.GetPointOnUnitSphere(pointOnUnitSphere, unscaledElevation);
                    uv[index].y = unscaledElevation;
                }
            }

            return vertices;
        }

        private int[] CreateTriangles()
        {
            int[] triangles = new int[NumberTriangles()];
            int triangleIndex = 0;
            
            for (int y = 0; y < resolution; y++) {
                for (int x = 0; x < resolution; x++) {
                    if (!CanCreateTriangle(x, y)) {
                        continue;
                    }

                    AddTriangles(triangles, triangleIndex, GetIndex(x, y));
                    triangleIndex += 6;
                }
            }

            return triangles;
        }
        
        private int GetIndex(int x, int y)
        {
            return x + y * resolution;
        }

        private Vector3 GetPointOnCube(int x, int y)
        {
            return up + GetPointOnAxis(x) * forward + GetPointOnAxis(y) * right;
        }

        private Vector3 GetPointOnSphere(int x, int y)
        {
            return GetPointOnCube(x, y).normalized;
        }

        private float GetPointOnAxis(int index)
        {
            return ((float)index / (resolution - 1) + FaceOffset) * FaceScale;
        }

        private int NumberTriangles()
        {
            return (resolution - 1) * (resolution - 1) * TrianglesPerFace * VerticesPerPolygon;
        }

        private bool CanCreateTriangle(int x, int y)
        {
            return x != resolution - 1 && y != resolution - 1;
        }

        private void AddTriangles(int[] triangles, int triangleIndex, int index)
        {
            triangles[triangleIndex] = index;
            triangles[triangleIndex + 1] = index + resolution + 1;
            triangles[triangleIndex + 2] = index + resolution;
                        
            triangles[triangleIndex + 3] = index;
            triangles[triangleIndex + 4] = index + 1;
            triangles[triangleIndex + 5] = index + resolution + 1;
        }
        
        public void UpdateUVs(ColorGenerator colourGenerator)
        {
            Vector2[] uv = mesh.uv;

            for (int y = 0; y < resolution; y++)
            {
                for (int x = 0; x < resolution; x++)
                {
                    int i = x + y * resolution;
                    Vector2 percent = new Vector2(x, y) / (resolution - 1);
                    Vector3 pointOnUnitCube = up + (percent.x - .5f) * 2 * forward + (percent.y - .5f) * 2 * right;
                    Vector3 pointOnUnitSphere = pointOnUnitCube.normalized;

                    uv[i].x = colourGenerator.BiomePercentFromPoint(pointOnUnitSphere);
                }
            }
            mesh.uv = uv;
        }
    }
}