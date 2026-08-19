using System;
using System.Collections.Generic;
using System.Reflection;

namespace SkillRebalanceExpansionMod.Utils
{
    public static class DataManager
    {
        public static List<T> Scan<T>(DataCategory category)
        {
            List<T> result = [];

            Assembly assembly = Assembly.GetExecutingAssembly();
            foreach (Type type in assembly.GetTypes())
            {
                DataBaseAttribute attribute = type.GetCustomAttribute<DataBaseAttribute>();

                if (attribute == null) continue;
                if (attribute.Category != category) continue;

                FieldInfo field = type.GetField("Data",BindingFlags.Public | BindingFlags.Static);
                if (field == null) continue;
                object value = field.GetValue(null);
                if (value is not List<T> datas) continue;

                result.AddRange(datas);
            }
            return result;
        }
    
        public static void Register(DataCategory category)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            foreach (Type type in assembly.GetTypes())
            {
                DataBaseAttribute attribute = type.GetCustomAttribute<DataBaseAttribute>();

                if (attribute == null) continue;
                if (attribute.Category != category) continue;

                MethodInfo method = type.GetMethod(
                    "Register",
                    BindingFlags.Public | BindingFlags.Static
                );

                if (method == null) continue;
                if (method.ReturnType != typeof(void)) continue;
                if (method.GetParameters().Length != 0) continue;

                try
                {
                    method.Invoke(null, null);
                }
                catch (Exception ex)
                {
                    Main.Log.LogError($"[DataManager] 注册数据失败: {type.FullName}\n{ex}");
                }
            }
        }
    }
}