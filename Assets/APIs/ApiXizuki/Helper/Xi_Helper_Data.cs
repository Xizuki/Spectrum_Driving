using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using UnityEngine;

namespace XizukiMethods
{
    namespace Data
    {
        public static class Xi_Helper_Data
        {
            public struct Trigger
            {
                
            }


            //CHATGPTed
            /// <summary>
            /// Returns String base on variable name, use like this, string result = GetVariableName(()=>varName)
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="expr"></param>
            /// <returns></returns>
            public static string GetDynamicVariableName<T>(System.Linq.Expressions.Expression<Func<T>> expr)
            {
                var memberExpr = expr.Body as System.Linq.Expressions.MemberExpression;
                if (memberExpr != null)
                {
                    var variableName = memberExpr.Member.Name;
                    return variableName;
                }
                else
                    return null;
            }

            //CHATGPTed
            /// <summary>
            /// Returns String base on variable name, use like this, string result = GetVariableName(()=>varName)
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="expr"></param>
            /// <returns></returns>
            public static string GetVariableName<T>(System.Linq.Expressions.Expression<Func<T>> expr)
            {
                if (expr.Body is MemberExpression memberExpr)
                {
                    var variableName = ((FieldInfo)memberExpr.Member).Name;
                    return variableName;
                }
                else
                    return null;
            }
        }

        #region Dictionary Scripts

        namespace Dictionary
        {
            public static class Xi_Helper_Dictionary
            {
                public static T GetValueFromKey<T>(Xi_Dictionary<T> dictionary, string key)
                {
                    foreach(Xi_DictionaryEntry<T> dictionaryEntry in dictionary.dictionaryEntries)
                    {
                        if(dictionaryEntry.key == key)
                        {
                            return dictionaryEntry.value;
                        }
                    }

                    return default(T);
                }
            }

            [Serializable]
            public struct Xi_Dictionary<T>
            {
                [SerializeField]
                public Xi_DictionaryEntry<T>[] dictionaryEntries;
            }


            [Serializable]
            public struct Xi_DictionaryEntry<T>
            {
                public int id;
                public string key;
                public string description;
                public T value;
            }


        }

        #endregion


    }

}