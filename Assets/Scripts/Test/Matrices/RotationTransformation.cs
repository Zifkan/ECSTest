using UnityEngine;

namespace Test.Matrices
{
    public class RotationTransformation : Transformation
    {
        public Vector3 Rotation;

        public override Vector3 Apply(Vector3 point)
        {
            float radZ = Rotation.z * Mathf.Deg2Rad;
            float sinZ = Mathf.Sin(radZ);
            float cosZ = Mathf.Cos(radZ);

            return new Vector3(
                point.x * cosZ - point.y * sinZ,
                point.x * sinZ + point.y * cosZ,
                point.z);
        }
    }
}