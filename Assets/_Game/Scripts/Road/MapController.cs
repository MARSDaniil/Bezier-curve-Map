using DG.Tweening;
using DG.Tweening.Plugins.Options;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.U2D;

namespace GHeart {
    public class MapController : MonoBehaviour {
        [SerializeField] private SplineDecorator m_splineDecorator;
        [SerializeField] private SplineDecorator m_fakeSplineDecorator;
        
        [SerializeField] private GameObject m_fakeBrickInFirstLvl;
        [SerializeField] private GameObject m_playerCharacterGO;
        [SerializeField] private Transform m_playerParentTransform;
        [SerializeField] private float m_moveBetweenDotTime = 3f;

        private GameObject m_playerCharacter;
        private int m_currentPoint;

        [HideInInspector] public Vector3 currentPointPosition = new Vector3();
        public void SpawnMap() {
            if (m_splineDecorator != null) {
                m_splineDecorator.SetObjectsToMap();
            }
            if (m_fakeSplineDecorator != null) {
                m_fakeSplineDecorator.SetObjectsToMap();
            }
            int totalWins;
            //if (GameInstance.Exist) {
            //    totalWins = GameInstance.I.MyUser.UserStats.Get_INT(Constants.UserStats.TotalWins);
            //} else {
                totalWins = Random.Range(0, 30);
            //}
            //if (GameInstance.Exist && m_fakeBrickInFirstLvl) {
            //    m_fakeBrickInFirstLvl.SetActive(!(totalWins > 2));
            //} else {
                m_fakeBrickInFirstLvl.SetActive(true);
            //}
            int countOfMainDots = m_splineDecorator.spline.MainPointCount;
            if (countOfMainDots < 1) {
                Debug.LogError("countOfMainDots < 1");
            }

            m_currentPoint = totalWins >= countOfMainDots ? totalWins % countOfMainDots : totalWins - 1;
            currentPointPosition = m_splineDecorator.mainDots[m_currentPoint].transform.position;
            int nextPoint = m_currentPoint + 1;
            if (nextPoint < m_splineDecorator.mainDots.Count) {
                if(m_playerCharacterGO != null) {
                    m_playerCharacter = Instantiate(m_playerCharacterGO, currentPointPosition, Quaternion.identity, m_playerParentTransform.parent);
                    m_playerCharacter.transform.eulerAngles = new Vector3(0, -90f, 0);
                }
            } else {
                Debug.LogError("currentPoint num more mainDots Count");
                return;
            }
        }

        public void MovePlayer() {
            Vector3[] vector3s = m_splineDecorator.GetDirectionPoints(m_currentPoint);
            m_playerCharacter.transform.DOPath(vector3s, m_moveBetweenDotTime, PathType.Linear, PathMode.Ignore).SetEase(Ease.Linear);
        }
    }
}
