using System;

namespace BT.Auctions.API.Models.Helpers
{
    public static class PropertyEvaluationHelper
    {
        /// <summary>
        /// Extension method to check if object has all null properties. Supports nullable types.
        /// Uses reflection and may hinder performance. For use with small models. 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">The object.</param>
        /// <returns>bool whether all properties are null or not</returns>
        public static bool ArePropertiesAllNull<T>(this T obj)
        {
            var properties = typeof(T).GetProperties();
            bool areAllNull = true;
            foreach(var property in properties)
            {
                //Check nullable types to ensure they have a value
                if (Nullable.GetUnderlyingType(property.PropertyType) != null)
                {
                    if (property.GetValue(obj, null) == null) continue;
                    areAllNull = false;
                    break;
                }
                //default to an object type for assessment
                if(property.GetValue(obj) != null)
                {
                    if (property.GetValue(obj).GetType().ToString() == "System.String" &&
                        string.IsNullOrWhiteSpace(property.GetValue(obj).ToString()))
                        continue;

                    areAllNull = false;
                    break;
                }
            }
            return areAllNull;
        }
    }
}
