using UnityEngine;

namespace GHeart {
    public static class Bezier {
        public static Vector3 GetPoint(Vector3 a_point0, Vector3 a_point1, Vector3 a_point2, Vector3 a_point3, float a_tCoeff) {
            a_tCoeff = Mathf.Clamp01(a_tCoeff);
            float oneMinusT = 1f - a_tCoeff;
            return
                oneMinusT * oneMinusT * oneMinusT * a_point0 +
                3f * oneMinusT * oneMinusT * a_tCoeff * a_point1 +
                3f * oneMinusT * a_tCoeff * a_tCoeff * a_point2 +
                a_tCoeff * a_tCoeff * a_tCoeff * a_point3;
        }

        public static Vector3 GetFirstDerivative(Vector3 a_point0, Vector3 a_point1, Vector3 a_point2, Vector3 a_point3, float a_tCoeff) {
            a_tCoeff = Mathf.Clamp01(a_tCoeff);
            float oneMinusT = 1f - a_tCoeff;
            return 3f * oneMinusT * oneMinusT * (a_point1 - a_point0) 
                + 6f * oneMinusT * a_tCoeff * (a_point2 - a_point1) 
                + 3f * a_tCoeff * a_tCoeff * (a_point3 - a_point2);
        }
    }
}