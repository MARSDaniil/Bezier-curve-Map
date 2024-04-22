using System.Collections.Generic;
using System.Drawing;
using UnityEditor;
using UnityEngine;
using UnityEngine.U2D;

namespace GHeart
{
    public class SplineDecorator : MonoBehaviour
    {
        [SerializeField] private BezierSpline m_spline;
        public BezierSpline spline;

        [SerializeField] private int frequency;

        [SerializeField] private bool lookForward;

        [SerializeField] private Transform parent;
        [SerializeField] private GameObject mainItemGO;
        [SerializeField] private GameObject otherItemGO;
        [SerializeField] private List<GameObject> m_mainDots;
        public List<GameObject> mainDots {  get { return m_mainDots; } }

        public void SetObjectsToMap() {
            SetMainDotsToSpline();
            SetDottetItemsToSpline();
        }

        public void SetDottetItemsToSpline() {
            if (frequency <= 0 || otherItemGO == null) {
                return;
            }
            float stepSize = 1f / (frequency * BezierSpline.m_stepsPerCurve);
            for (int p = 0, f = 0; f < frequency; f++) {
                for (int i = 0; i < BezierSpline.m_stepsPerCurve; i++, p++) {
                    Transform item = Instantiate(otherItemGO.transform) as Transform;
                    Vector3 position = m_spline.GetPoint(p * stepSize);
                    item.transform.localPosition = position;
                    if (lookForward) {
                        item.transform.LookAt(position + m_spline.GetDirection(p * stepSize));
                    }
                    item.transform.parent = transform;
                }
            }
        }

        public void SetMainDotsToSpline() {
            if (mainItemGO != null) {
                for (int i = 0; i < m_spline.MainPointCount; i++) {
                    GameObject item = Instantiate(mainItemGO, parent);
                    Vector3 position = m_spline.GetPoint(i * 3);
                    m_mainDots.Add(item);
                    item.transform.localPosition = position;
                }
            }
        }

        public Vector3[] GetDirectionPoints(int index) {
            List<Vector3> directionPoints = new List<Vector3>();
            int steps = BezierSpline.m_stepsPerCurve * m_spline.CurveCount;
            for (int i = index * BezierSpline.m_stepsPerCurve; i <= index * BezierSpline.m_stepsPerCurve + BezierSpline.m_stepsPerCurve; i++) {
                directionPoints.Add(m_spline.GetPoint(i / (float)steps));
            }
            return directionPoints.ToArray();
        }

    }
}
