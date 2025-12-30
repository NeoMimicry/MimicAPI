using System;
using System.Collections.Generic;
using System.Reflection;

namespace MimicAPI.GameAPI
{
    public static class ReflectionHelper
    {
        private const BindingFlags DefaultFlags = BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static;

        public static object? GetFieldValue(object target, string fieldName)
        {
            if (target == null)
                return null;

            FieldInfo? field = target.GetType().GetField(fieldName, DefaultFlags);
            return field?.GetValue(target);
        }

        public static object? GetFieldValue(Type type, string fieldName)
        {
            FieldInfo? field = type.GetField(fieldName, DefaultFlags);
            return field?.GetValue(null);
        }

        public static T GetFieldValue<T>(object target, string fieldName)
        {
            object? value = GetFieldValue(target, fieldName);
            if (value == null)
                return default!;
            return (T)value;
        }

        public static T GetFieldValue<T>(Type type, string fieldName)
        {
            object? value = GetFieldValue(type, fieldName);
            if (value == null)
                return default!;
            return (T)value;
        }

        public static void SetFieldValue(object target, string fieldName, object value)
        {
            if (target == null)
                return;

            FieldInfo? field = target.GetType().GetField(fieldName, DefaultFlags);
            field?.SetValue(target, value);
        }

        public static void SetFieldValue(Type type, string fieldName, object value)
        {
            FieldInfo? field = type.GetField(fieldName, DefaultFlags);
            field?.SetValue(null, value);
        }

        public static object? InvokeMethod(object target, string methodName, params object[] parameters)
        {
            if (target == null)
                return null;

            MethodInfo? method = target.GetType().GetMethod(methodName, DefaultFlags);
            if (method == null)
                return null;

            return method.Invoke(target, parameters.Length > 0 ? parameters : null);
        }

        public static object? InvokeMethod(Type type, string methodName, params object[] parameters)
        {
            MethodInfo? method = type.GetMethod(methodName, DefaultFlags);
            if (method == null)
                return null;

            return method.Invoke(null, parameters.Length > 0 ? parameters : null);
        }

        public static T? InvokeMethod<T>(object target, string methodName, params object[] parameters)
            where T : class
        {
            object? result = InvokeMethod(target, methodName, parameters);
            if (result == null)
                return default;
            return (T)result;
        }

        public static T? InvokeMethod<T>(Type type, string methodName, params object[] parameters)
            where T : class
        {
            object? result = InvokeMethod(type, methodName, parameters);
            if (result == null)
                return default;
            return (T)result;
        }

        public static PropertyInfo? GetProperty(Type type, string propertyName)
        {
            return type.GetProperty(propertyName, DefaultFlags);
        }

        public static object? GetPropertyValue(object target, string propertyName)
        {
            if (target == null)
                return null;

            PropertyInfo? prop = GetProperty(target.GetType(), propertyName);
            return prop?.GetValue(target);
        }

        public static T GetPropertyValue<T>(object target, string propertyName)
        {
            object? value = GetPropertyValue(target, propertyName);
            if (value == null)
                return default!;
            return (T)value;
        }

        public static void SetPropertyValue(object target, string propertyName, object value)
        {
            if (target == null)
                return;

            PropertyInfo? prop = GetProperty(target.GetType(), propertyName);
            prop?.SetValue(target, value);
        }
    }
}
