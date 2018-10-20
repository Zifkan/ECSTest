using UnityEngine;

namespace Test.Matrices
{
    public class PositionTransformation : Transformation
    {
        public Vector3 Position;

        public override Vector3 Apply(Vector3 point)
        {
            return point + Position;
        }
    }
}