using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Minsung.UTIL
{
    public static class Util
    {

        /// <summary> 코루틴이 실행 중인지, 체크하고 실행</summary>
        public static void CheckRunCoroutine(ref Coroutine _cor, Coroutine _startCor, MonoBehaviour _mono)
        {
            if (_cor == null)
            {
                _cor = _startCor;
            }
            else
            {
                _mono.StopCoroutine(_cor);
                _cor = null;
                _cor = _startCor;
            }
        }

        public static void ShuffleList<T>(IList<T> list)
        {
            int random1;
            int random2;

            T tmp;

            for (int index = 0; index < list.Count; ++index)
            {
                random1 = UnityEngine.Random.Range(0, list.Count);
                random2 = UnityEngine.Random.Range(0, list.Count);

                tmp = list[random1];
                list[random1] = list[random2];
                list[random2] = tmp;
            }
        }

        public static IEnumerator ShowSlowTime(float _slowPower, float _slowTime, Action _co)
        {
            Time.timeScale = _slowPower;

            yield return new WaitForSecondsRealtime(_slowTime);
            Time.timeScale = 1.0f;
            _co.Invoke();
        }


        /// <summary>
        /// 특정 위치 주변으로 랜덤한 위치값을 반환한다.
        /// </summary>
        /// <param name="tr"> 주변에 랜덤 위치값을 가져올 기준 위치 </param>
        /// <param name="maxDistance"> 기준점에서 떨어질 거리 </param>
        public static Vector3 GetRandomPos(Transform tr, float maxDistance)
        {
            float randRotNum = UnityEngine.Random.Range(0, 360.0f);
            float randDistanceNum = UnityEngine.Random.Range(0, maxDistance);

            Quaternion randRot = Quaternion.Euler(tr.up * randRotNum);

            //생성 스테이지 주변으로 distance 만큼 떨어짐. 각도는 랜덤
            Vector3 randPos = tr.rotation * randRot * (tr.forward * randDistanceNum) + tr.position;

            return randPos;
        }
        #region 확률 구하는 함수
        /// <summary>
        /// 확률에 대한 값을 반환하는 함수
        /// </summary>
        /// <param name="Chance"> chance : 1 확률에 대한 변수 </param>
        /// <returns></returns>
        public static bool GetThisChanceResult(float Chance)
        {
            if (Chance < 0.0000001f)
            {
                Chance = 0.0000001f;
            }

            bool Success = false;
            int RandAccuracy = 10000000;
            float RandHitRange = Chance * RandAccuracy;
            int Rand = UnityEngine.Random.Range(1, RandAccuracy + 1);
            if (Rand <= RandHitRange)
            {
                Success = true;
            }
            return Success;
        }

        /// <summary> % 에 대한 확률값을 반환. </summary>
        public static bool GetThisChanceResult_Percentage(float Percentage_Chance)
        {
            if (Percentage_Chance < 0.0000001f)
            {
                Percentage_Chance = 0.0000001f;
            }

            Percentage_Chance = Percentage_Chance / 100;

            bool Success = false;
            int RandAccuracy = 10000000;
            float RandHitRange = Percentage_Chance * RandAccuracy;
            int Rand = UnityEngine.Random.Range(1, RandAccuracy + 1);
            if (Rand <= RandHitRange)
            {
                Success = true;
            }
            return Success;
        }

        public static void RunCoroutine(ref Coroutine _co1, Coroutine _co2, MonoBehaviour _mono)
        {
            if (_co1 == null)
            {
                _co1 = _co2;
            }
            else
            {
                _mono.StopCoroutine(_co1);
                _co1 = _co2;
            }
        }


        #endregion
        //public static class Commandkey
        //{
        //    // Dictionary to store KeyCode for different actions
        //    private static Dictionary<ActionType, KeyCode> keyBindings = new Dictionary<ActionType, KeyCode>();

        //    static Commandkey()
        //    {
        //        keyBindings[ActionType.Inventory] = KeyCode.I;
        //        keyBindings[ActionType.SkillWindow] = KeyCode.K;
        //        keyBindings[ActionType.StatusWindow] = KeyCode.P;
        //        keyBindings[ActionType.QuickSlotA] = KeyCode.Z;
        //        keyBindings[ActionType.QuickSlotB] = KeyCode.X;

        //        keyBindings[ActionType.SKILLA] = KeyCode.Alpha1;
        //        keyBindings[ActionType.SKILLB] = KeyCode.Alpha2;
        //        keyBindings[ActionType.SKILLC] = KeyCode.Alpha3;
        //        keyBindings[ActionType.SKILLD] = KeyCode.Alpha4;
        //        keyBindings[ActionType.SKILLE] = KeyCode.Alpha5;
        //        keyBindings[ActionType.SKILLF] = KeyCode.Alpha6;

        //    }

        //    public static void CheckInput(ActionType actionType, ref bool isUseWindow, BaseInvenManager invenWindowManager)
        //    {
        //        if (Input.GetKeyDown(keyBindings[actionType]))
        //        {
        //            isUseWindow = !isUseWindow;
        //            invenWindowManager.SetWindowActive(isUseWindow);
        //            PlayerController.instance.CheckFreezController();
        //            PlayerController.instance.PlayerIdleState();
        //        }
        //    }

        //    public static void UseQuickSlot(ActionType actionType, System.Action quickUseAction)
        //    {
        //        if (Input.GetKeyDown(keyBindings[actionType]))
        //        {
        //            quickUseAction.Invoke();
        //        }
        //    }

        //    public static void UseSkillAttack(ActionType actionType, System.Action skillAction)
        //    {
        //        if (Input.GetKeyDown(keyBindings[actionType]))
        //        {
        //            skillAction.Invoke();
        //        }
        //    }



        //}

        
    }

}
