using UnityEngine;

namespace Test.Matrices
{
    public class ScaleTransformation : Transformation
    {
        public Vector3 Scale = Vector3.one;

        public override Vector3 Apply(Vector3 point)
        {
            point.x *= Scale.x;
            point.y *= Scale.y;
            point.z *= Scale.z;
            return point;
        }
    }
}