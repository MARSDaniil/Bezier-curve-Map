using System;
using UnityEngine;

namespace GHeart {
    public class BezierSpline : MonoBehaviour {

        #region Fields

        [SerializeField] private Vector3[] m_points;
        [SerializeField] private BezierControlPointMode[] m_modes;
        public const int m_stepsPerCurve = 10;

        [SerializeField]
        private bool loop;
        public bool Loop {
            get {
                return loop;
            }
            set {
                loop = value;
                if (value == true) {
                    m_modes[m_modes.Length - 1] = m_modes[0];
                    SetControlPoint(0, m_points[0]);
                }
            }
        }

        public enum BezierControlPointMode {
            Free,
            Aligned,
            Mirrored
        }
        public void Reset() {
            m_points = new Vector3[] {
            new Vector3(1f, 0f, 0f),
            new Vector3(2f, 0f, 0f),
            new Vector3(3f, 0f, 0f),
            new Vector3(4f, 0f, 0f)};

            m_modes = new BezierControlPointMode[] { BezierControlPointMode.Free, BezierControlPointMode.Free };
        }
        public int ControlPointCount {
            get {
                return m_points.Length;
            }
        }
        public int MainPointCount {
            get { return (m_points.Length - 1)/3 + 1; }
        }
        public int OtherPointCount {
            get { return 2 * (m_points.Length - 1) / 3;}
        }
        #endregion

        #region Public

        public Vector3 GetControlPoint(int index) {
            return m_points[index];
            EnforceMode(index);
        }

        public void SetControlPoint(int index, Vector3 point) {
            if (index % 3 == 0) {
                Vector3 delta = point - m_points[index];
                if (loop) {
                    if (index == 0) {
                        m_points[1] += delta;
                        m_points[m_points.Length - 2] += delta;
                        m_points[m_points.Length - 1] = point;
                    } else if (index == m_points.Length - 1) {
                        m_points[0] = point;
                        m_points[1] += delta;
                        m_points[index - 1] += delta;
                    } else {
                        m_points[index - 1] += delta;
                        m_points[index + 1] += delta;
                    }
                } else {
                    if (index > 0) {
                        m_points[index - 1] += delta;
                    }
                    if (index + 1 < m_points.Length) {
                        m_points[index + 1] += delta;
                    }
                }
            }
            m_points[index] = point;
            EnforceMode(index);
        }

        public Vector3 GetPoint(float a_t) {
            int i;
            if (a_t >= 1f) {
                a_t = 1f;
                i = m_points.Length - 4;
            } else {
                a_t = Mathf.Clamp01(a_t) * CurveCount;
                i = (int)a_t;
                a_t -= i;
                i *= 3;
            }
            return transform.TransformPoint(Bezier.GetPoint(
                m_points[i], m_points[i + 1], m_points[i + 2], m_points[i + 3], a_t));
        }
        public Vector3 GetPoint(int a_index) {
            if (a_index >= 0) {
                return m_points[a_index];
            }
            return m_points[0];
        }
        public Vector3 GetDirection(float t) {
            return GetVelocity(t).normalized;
        }
        public void AddCurve() {
            Vector3 point = m_points[m_points.Length - 1];
            Array.Resize(ref m_points, m_points.Length + 3);
            point.x += 1f;
            m_points[m_points.Length - 3] = point;
            point.x += 1f;
            m_points[m_points.Length - 2] = point;
            point.x += 1f;
            m_points[m_points.Length - 1] = point;

            Array.Resize(ref m_modes, m_modes.Length + 1);
            m_modes[m_modes.Length - 1] = m_modes[m_modes.Length - 2];
            EnforceMode(m_points.Length - 4);

            if (loop) {
                m_points[m_points.Length - 1] = m_points[0];
                m_modes[m_modes.Length - 1] = m_modes[0];
                EnforceMode(0);
            }
        }
        public int CurveCount {
            get {
                return (m_points.Length - 1) / 3;
            }
        }
        public BezierControlPointMode GetControlPointMode(int index) {
            return m_modes[(index + 1) / 3];
        }

        public void SetControlPointMode(int index, BezierControlPointMode mode) {
            int modeIndex = (index + 1) / 3;
            m_modes[modeIndex] = mode;
            if (loop) {
                if (modeIndex == 0) {
                    m_modes[m_modes.Length - 1] = mode;
                } else if (modeIndex == m_modes.Length - 1) {
                    m_modes[0] = mode;
                }
            }
            EnforceMode(index);
        }

        #endregion

        #region Helpers 
        private void EnforceMode(int index) {
            int modeIndex = (index + 1) / 3;
            BezierControlPointMode mode = m_modes[modeIndex];
            if (mode == BezierControlPointMode.Free || !loop && (modeIndex == 0 || modeIndex == m_modes.Length - 1)) {
                return;
            }
            int middleIndex = modeIndex * 3;
            int fixedIndex, enforcedIndex;
            if (index <= middleIndex) {
                fixedIndex = middleIndex - 1;
                if (fixedIndex < 0) {
                    fixedIndex = m_points.Length - 2;
                }
                enforcedIndex = middleIndex + 1;
                if (enforcedIndex >= m_points.Length) {
                    enforcedIndex = 1;
                }
            } else {
                fixedIndex = middleIndex + 1;
                if (fixedIndex >= m_points.Length) {
                    fixedIndex = 1;
                }
                enforcedIndex = middleIndex - 1;
                if (enforcedIndex < 0) {
                    enforcedIndex = m_points.Length - 2;
                }
            }
            Vector3 middle = m_points[middleIndex];
            Vector3 enforcedTangent = middle - m_points[fixedIndex];
            if (mode == BezierControlPointMode.Aligned) {
                enforcedTangent = enforcedTangent.normalized * Vector3.Distance(middle, m_points[enforcedIndex]);
            }
            m_points[enforcedIndex] = middle + enforcedTangent;
        }
     

        private Vector3 GetVelocity(float t) {
            int i;
            if (t >= 1f) {
                t = 1f;
                i = m_points.Length - 4;
            } else {
                t = Mathf.Clamp01(t) * CurveCount;
                i = (int)t;
                t -= i;
                i *= 3;
            }
            return transform.TransformPoint(Bezier.GetFirstDerivative(
                m_points[i], m_points[i + 1], m_points[i + 2], m_points[i + 3], t)) - transform.position;
        }

        #endregion
    }
}
