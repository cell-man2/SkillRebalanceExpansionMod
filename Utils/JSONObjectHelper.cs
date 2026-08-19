using System;
using System.Collections;

namespace SkillRebalanceExpansionMod.Utils
{
    public static class JSONObjectHelper
    {
        public static JSONObject ToJSONObject(object value)
        {
            if (value == null)
            {
                return JSONObject.nullJO;
            }
            switch (value)
            {
                case JSONObject json:
                    return json;
                case int intValue:
                    return JSONObject.Create(intValue);
                case float floatValue:
                    return JSONObject.Create(floatValue);
                case double doubleValue:
                    return JSONObject.Create((float)doubleValue);
                case bool boolValue:
                    return JSONObject.Create(boolValue);
                case string stringValue:
                    return JSONObject.CreateStringObject(stringValue);
                case IDictionary dict:
                {
                    JSONObject obj = JSONObject.Create(JSONObject.Type.OBJECT);
                    foreach (DictionaryEntry entry in dict)
                    {
                        obj.AddField(entry.Key.ToString(), ToJSONObject(entry.Value));
                    }
                    return obj;
                }
                case IEnumerable enumerable:
                {
                    JSONObject array = JSONObject.Create(JSONObject.Type.ARRAY);
                    foreach (object item in enumerable)
                    {
                        array.Add(ToJSONObject(item));
                    }
                    return array;
                }
                default:
                    throw new Exception($"JSONObjectHelper无法转换类型:{value.GetType()}");
            }
        }
    }
}
