using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

namespace XizukiMethods
{
    namespace Math
    {
        public enum EstimatedEqualType { Range, Round, PercentageRange }

        public static class Xi_Helper_Math
        {
            /// <summary>
            /// This method just reads a string a calculates based off it.
            /// </summary>
            public static float CalculateFromString(string text)
            {
                string inputExpression = text;


                float result = EvaluateExpression(inputExpression);
                return result;


                float EvaluateExpression(string expression)
                {
                    // Replace supported functions
                    expression = expression.Replace("sin", "Mathf.Sin")
                                           .Replace("cos", "Mathf.Cos")
                                           .Replace("pow", "Mathf.Pow")
                                           .Replace("sqrt", "Mathf.Sqrt")
                                           .Replace("exp", "Mathf.Exp")
                                           .Replace("log", "Mathf.Log")
                                           .Replace("abs", "Mathf.Abs")
                                           .Replace("floor", "Mathf.Floor")
                                           .Replace("ceil", "Mathf.Ceil")
                                           .Replace("round", "Mathf.Round")
                                           .Replace("min", "Mathf.Min")
                                           .Replace("max", "Mathf.Max");

                    // Evaluate the expression
                    float result = (float)Convert.ToDouble(new System.Data.DataTable().Compute(expression, null));
                    return result;
                }

            }


            public static void Clamp(ref float value, float lowerLimit, float higherLimit, float i)
            {
                float result = value+i;

                if (result < lowerLimit)
                {
                    result = lowerLimit;
                }
                else if (result > higherLimit)
                {
                    result = higherLimit;
                }

                value = result;
            }
            public static float Clamp( float value, float lowerLimit, float higherLimit, float i)
            {
                float result = value + i;

                if (result < lowerLimit)
                {
                    result = lowerLimit;
                }
                else if (result > higherLimit)
                {
                    result = higherLimit;
                }

                value = result;
                return result;
            }








            /// <summary>
            /// This method does am estimated equals and has 2 different types of comparison
            /// </summary>
            /// <param name="value">The first value .</param>
            /// <param name="fixedValue">The second value.</param>
            /// <param name="estimatedEqualType">The Type of comparison, Range comparison is to check if the difference in both values are in a range, Round comparison is to round both values to a decimal point and check after</param>
            /// <param name="estimationValue">The value used to determine how much estimation, for Range its the max absolute difference the 2 value can be, for Round, its how many decimal place you want to round the values to before comparing ( 0 = no diff )(</param>
            /// <returns>True if its around the same value</returns>
            public static bool EstimatedEqual(float value, float fixedValue, EstimatedEqualType estimatedEqualType, float estimationValue)
            {
                bool result = false;

                if (estimatedEqualType == EstimatedEqualType.Range)
                {
                    if (Mathf.Abs(value - fixedValue) <= estimationValue)
                    {
                        result = true;
                    }
                }
                else if (estimatedEqualType == EstimatedEqualType.Round)
                {
                    if(estimationValue == 0) { return false; }

                    float scaledValue = value * (10 * estimationValue);
                    float scaledFixedValue = fixedValue * (10 * estimationValue);

                    if (Mathf.Round(scaledValue) == Mathf.Round(scaledFixedValue))
                    {
                        result = true;
                    }
                }
                else if (estimatedEqualType == EstimatedEqualType.PercentageRange)
                {
                    float percentageValue = fixedValue * (estimationValue / 1);
                    if (Mathf.Abs(value - fixedValue) <= percentageValue)
                    {
                        result = true;
                    }
                }
                return result;
            }

            /// <summary>
            /// This method does am estimated equals and has 2 different types of comparison
            /// </summary>
            /// <param name="value">The first value .</param>
            /// <param name="fixedValue">The second value.</param>
            /// <param name="estimatedEqualType">The Type of comparison, Range comparison is to check if the difference in both values are in a range, Round comparison is to round both values to a decimal point and check after</param>
            /// <param name="estimationValue">The value used to determine how much estimation, for Range its the max absolute difference the 2 value can be, for Round, its how many decimal place you want to round the values to before comparing ( 0 = no diff )(</param>
            /// <returns>True if its around the same value</returns>
            public static bool EstimatedEqual(ref float value, float fixedValue, EstimatedEqualType estimatedEqualType, float estimationValue)
            {
                bool result = false;

                if (estimatedEqualType == EstimatedEqualType.Range)
                {
                    if (Mathf.Abs(value - fixedValue) <= estimationValue)
                    {
                        result = true;
                        value = fixedValue;
                    }
                }
                else if (estimatedEqualType == EstimatedEqualType.Round)
                {
                    if (estimationValue == 0) { return false; }

                    float scaledValue = value * (10 * estimationValue);
                    float scaledFixedValue = fixedValue * (10 * estimationValue);

                    if (Mathf.Round(scaledValue) == Mathf.Round(scaledFixedValue))
                    {
                        result = true;
                        value = fixedValue;
                    }
                }
                else if (estimatedEqualType == EstimatedEqualType.PercentageRange)
                {
                    float percentageValue = fixedValue * (estimationValue / 1);
                    if (Mathf.Abs(value - fixedValue) <= percentageValue)
                    {
                        result = true;
                    }
                }
                return result;
            }


            /// <summary>
            /// This method does am estimated equals and has 2 different types of comparison
            /// </summary>
            /// <param name="value">The first value .</param>
            /// <param name="fixedValue">The second value.</param>
            /// <param name="estimatedEqualType">The Type of comparison, Range comparison is to check if the difference in both values are in a range, Round comparison is to round both values to a decimal point and check after</param>
            /// <param name="estimationValue">The value used to determine how much estimation, for Range its the max absolute difference the 2 value can be, for Round, its how many decimal place you want to round the values to before comparing ( 0 = no diff )(</param>
            /// <returns>True if its around the same value</returns>
            //public static void EstimatedEqual(ref float value, float fixedValue, EstimatedEqualType estimatedEqualType, float estimationValue)
            //{
            //    if (estimatedEqualType == EstimatedEqualType.Range)
            //    {
            //        if (Mathf.Abs(value - fixedValue) <= estimationValue)
            //        {
            //            value = fixedValue;
            //        }
            //    }
            //    else if (estimatedEqualType == EstimatedEqualType.Round)
            //    {
            //        if (estimationValue == 0) { return; }

            //        float scaledValue = value * (10 * estimationValue);
            //        float scaledFixedValue = fixedValue * (10 * estimationValue);

            //        if (Mathf.Round(scaledValue) == Mathf.Round(scaledFixedValue))
            //        {
            //            value = fixedValue;
            //        }
            //    }
            //    if (estimatedEqualType == EstimatedEqualType.PercentageRange)
            //    {
            //        float differential = fixedValue - value;
            //        float percentageValue = differential * (estimationValue / 1);
            //        if (Mathf.Abs(value - fixedValue) <= percentageValue)
            //        {
            //            value = fixedValue;
            //        }
            //    }
            //    return;
            //}



            /// <summary>
            /// This method does am estimated equals and has 2 different types of comparison
            /// </summary>
            /// <param name="value">The first value .</param>
            /// <param name="fixedValue">The second value.</param>
            /// <param name="estimatedEqualType">The Type of comparison, Range comparison is to check if the difference in both values are in a range, Round comparison is to round both values to a decimal point and check after</param>
            /// <param name="estimationValue">The value used to determine how much estimation, for Range its the max absolute difference the 2 value can be, for Round, its how many decimal place you want to round the values to before comparing ( 0 = no diff )(</param>
            /// <returns>True if its around the same value</returns>
            public static bool EstimatedEqual(Vector3 value, Vector3 fixedValue, EstimatedEqualType estimatedEqualType, float estimationValue)
            {
                bool result = false;

                if (estimatedEqualType == EstimatedEqualType.Range)
                {
                    Vector3 v3 = (fixedValue - value);
                    float absX = Mathf.Abs(v3.x);
                    float absY = Mathf.Abs(v3.y);
                    float absZ = Mathf.Abs(v3.z);
                    float absMagnitudeDiff = absX + absY + absZ;

                    if (absMagnitudeDiff <= estimationValue)
                    {
                        result = true;
                    }
                }
                else if (estimatedEqualType == EstimatedEqualType.Round)
                {
                    if (estimationValue == 0) { return false; }

                    float roundedScaledValueX = (Mathf.Round(value.x * (10 * estimationValue)));
                    float roundedScaledValueY = (Mathf.Round(value.y * (10 * estimationValue)));
                    float roundedScaledValueZ = (Mathf.Round(value.z * (10 * estimationValue)));
                    Vector3 roundedScaledValue = new Vector3(roundedScaledValueX, roundedScaledValueY, roundedScaledValueZ);

                    float roundedScaledFixedValueX = (Mathf.Round(fixedValue.x * (10 * estimationValue)));
                    float roundedScaledFixedValueY = (Mathf.Round(fixedValue.y * (10 * estimationValue)));
                    float roundedScaledFixedValueZ = (Mathf.Round(fixedValue.z * (10 * estimationValue)));
                    Vector3 roundedScaledFixedValue = new Vector3(roundedScaledFixedValueX, roundedScaledFixedValueY, roundedScaledFixedValueZ);



                    if (roundedScaledValue == roundedScaledFixedValue)
                    {
                        result = true;
                    }
                }
                else if (estimatedEqualType == EstimatedEqualType.PercentageRange)
                {
                    Vector3 percentageValue = (fixedValue) * (estimationValue / 1);



                    Vector3 v3 = (fixedValue - value);
                    float absX = Mathf.Abs(v3.x);
                    float absY = Mathf.Abs(v3.y);
                    float absZ = Mathf.Abs(v3.z);
                    float absMagnitudeDiff = absX + absY + absZ;

                    if (absMagnitudeDiff <= percentageValue.magnitude)
                    {
                        result = true;
                    }
                }
                return result;
            }
        }
    }
}