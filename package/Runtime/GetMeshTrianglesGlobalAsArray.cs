using UnityEngine;

namespace GaussianSplatting.Runtime
{
    public class GetMeshTrianglesGlobalAsArray : MonoBehaviour
    {
        public GameObject targetGameObject; // Assuming you have a reference to the target GameObject with a Mesh
        public System.Single[] GetTriangleCenterCoords()
        {
            if (targetGameObject == null)
            {
                Debug.LogError("GetMeshTrianglesGlobalAsArray.GetTriangleCenterCoords: No target GameObject assigned!");
                return null;
            }

            // Get the Mesh component from the target GameObject
            Mesh mesh = targetGameObject.GetComponent<MeshFilter>()?.sharedMesh;
            if (mesh == null)
            {
                Debug.LogError("GetMeshTrianglesGlobalAsArray.GetTriangleCenterCoords: Mesh not found on target GameObject!");
                return null;
            }

            // Get triangle indices
            int[] triangleIndices = mesh.triangles;
            // Debug.Log("GetMeshTrianglesGlobalAsArray.GetTriangleCenterCoords: no. triangles: " + triangleIndices.Length);

            // Convert indices to triangle center coordinates in local space
            System.Single[] triangleCenterCoordsLocal = GetTriangleCoordsFromIndices(triangleIndices, mesh.vertices);
            // Debug.Log("GetMeshTrianglesGlobalAsArray.GetTriangleCenterCoords: got " + triangleCenterCoordsLocal.Length + " triangle center coordinates in local space");

            // Transform center coordinates to global space using target GameObject's transform
            System.Single[] triangleCenterCoordsGlobal = TransformTriangleCoordsToGlobal(triangleCenterCoordsLocal, targetGameObject.transform.localToWorldMatrix);
            // Debug.Log("GetMeshTrianglesGlobalAsArray.GetTriangleCenterCoords: got " + triangleCenterCoordsGlobal.Length + " triangle center coordinates in global space");

            return triangleCenterCoordsGlobal;
        }
        private System.Single[] TransformTriangleCoordsToGlobal(System.Single[] triangleCoordsLocal, Matrix4x4 localToWorldMatrix)
        {
            if (triangleCoordsLocal == null || triangleCoordsLocal.Length % 3 != 0)
            {
                Debug.LogError("GetMeshTrianglesGlobalAsArray.GetTriangleCenterCoords: Invalid triangle center coordinates array!");
                return null;
            }

            int numTriangles = triangleCoordsLocal.Length / 3;
            System.Single[] triangleCenterCoordsGlobal = new System.Single[numTriangles * 3];

            for (int i = 0; i < numTriangles; i++)
            {
                int centerCoordsIndex = i * 3;

                // Create a Vector3 from local coordinates
                Vector3 centerLocal = new Vector3(triangleCoordsLocal[centerCoordsIndex], triangleCoordsLocal[centerCoordsIndex + 1], triangleCoordsLocal[centerCoordsIndex + 2]);

                // Transform to global space using localToWorldMatrix
                Vector3 centerGlobal = localToWorldMatrix * centerLocal;

                // Store transformed coordinates in the new array
                triangleCenterCoordsGlobal[centerCoordsIndex] = centerGlobal.x;
                triangleCenterCoordsGlobal[centerCoordsIndex + 1] = centerGlobal.y;
                triangleCenterCoordsGlobal[centerCoordsIndex + 2] = centerGlobal.z;
            }
            return triangleCenterCoordsGlobal;
        }
        private System.Single[] GetTriangleCoordsFromIndices(int[] triangleIndices, Vector3[] vertices)
        {
            if (triangleIndices == null || vertices == null)
            {
                Debug.LogError("GetMeshTrianglesGlobalAsArray.GetTriangleCenterCoords: Invalid triangle indices or vertices array!");
                return null;
            }

            int numTriangles = triangleIndices.Length / 3;
            System.Single[] triangleCenterCoords = new System.Single[numTriangles * 3]; // 3 floats per triangle center

            for (int i = 0; i < numTriangles; i++)
            {
                int triangleStartIndex = i * 3;

                int vertexIndex1 = triangleIndices[triangleStartIndex];
                int vertexIndex2 = triangleIndices[triangleStartIndex + 1];
                int vertexIndex3 = triangleIndices[triangleStartIndex + 2];

                // Calculate center coordinates of the triangle
                float centerX = (vertices[vertexIndex1].x + vertices[vertexIndex2].x + vertices[vertexIndex3].x) / 3f;
                float centerY = (vertices[vertexIndex1].y + vertices[vertexIndex2].y + vertices[vertexIndex3].y) / 3f;
                float centerZ = (vertices[vertexIndex1].z + vertices[vertexIndex2].z + vertices[vertexIndex3].z) / 3f;

                // Store center coordinates in the new array
                int centerCoordsIndex = i * 3;
                triangleCenterCoords[centerCoordsIndex] = centerX;
                triangleCenterCoords[centerCoordsIndex + 1] = centerY;
                triangleCenterCoords[centerCoordsIndex + 2] = centerZ;
            }
            return triangleCenterCoords;
        }
    }
}