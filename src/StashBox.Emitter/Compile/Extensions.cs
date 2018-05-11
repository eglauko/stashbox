using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Stashbox.BuildUp.Expressions.Compile
{
    public static class Extensions
    {
        public static bool ContainsElement<TElement>(this TElement[] array, TElement element) 
            => array.GetIndex(element) != -1;

        public static int GetIndex<TElement>(this TElement[] array, TElement element)
        {
            if (array == null || array.Length == 0) return -1;

            var length = array.Length;
            if (length == 1) return array[0].Equals(element) ? 0 : -1;

            for (int i = 0; i < length; i++)
                if (array[i].Equals(element))
                    return i;

            return -1;
        }

        public static TEnumerable[] CastToArray<TEnumerable>(this IEnumerable<TEnumerable> enumerable) =>
            enumerable is TEnumerable[] ? (TEnumerable[])enumerable : enumerable.ToArray();

        public static TElement[] AddElement<TElement>(this TElement[] array, TElement element)
        {
            if (array == null || array.Length == 0)
                return new[] { element };

            var length = array.Length;
            switch (length)
            {
                case 1:
                    return new[] { array[0], element };
                case 2:
                    return new[] { array[0], array[1], element };
                case 3:
                    return new[] { array[0], array[1], array[2], element };
                case 4:
                    return new[] { array[0], array[1], array[2], array[3], element };
                case 5:
                    return new[] { array[0], array[1], array[2], array[3], array[4], element };
                case 6:
                    return new[] { array[0], array[1], array[2], array[3], array[4], array[5], element };
                default:
                    var newArr = new TElement[length + 1];
                    Array.Copy(array, newArr, length);
                    newArr[length] = element;
                    return newArr;
            }
        }

        internal static readonly Type[] EmptyTypes = new Type[0];

        public static Type[] GetTypes(this IList<ParameterExpression> parameters)
        {
            var count = parameters.Count;
            if (count == 0)
                return EmptyTypes;
            if (count == 1)
                return new[] { parameters[0].Type };

            var types = new Type[count];
            for (var i = 0; i < count; i++)
                types[i] = parameters[i].Type;
            return types;
        }

        public static Type[] Append(this Type type, Type[] types)
        {
            var count = types.Length;
            if (count == 0)
                return new[] { type };

            var arr = new Type[count + 1];
            arr[0] = type;
            Array.Copy(types, 0, arr, 1, count);
            return arr;
        }

        public static Type[] Append(this Type[] types, Type type)
        {
            var count = types.Length;
            if (count == 0)
                return new[] { type };

            var arr = new Type[count + 1];
            Array.Copy(types, 0, arr, 0, count);
            arr[count] = type;
            return arr;
        }

        public static Type[] Append(this Type[] types, Type[] others)
        {
            if (others.Length == 0)
                return types;

            if (types.Length == 0)
                return others;

            var length = others.Length + types.Length;
            var arr = new Type[length];
            Array.Copy(types, 0, arr, 0, types.Length);
            Array.Copy(others, 0, arr, types.Length, others.Length);
            return arr;
        }

        public static TValue GetOrDefault<TValue>(this Dictionary<int, TValue> dic, int key)
        {
            return dic.ContainsKey(key)
                ? dic[key]
                : default(TValue);
        }

        public static Dictionary<int, TValue> AddOrUpdate<TValue>(this Dictionary<int, TValue> dic, int key, TValue value)
        {
            if (dic.ContainsKey(key))
                dic[key] = value;
            else
                dic.Add(key, value);
            return dic;
        }

        public static MethodInfo GetSingleMethod(this Type type, string name, bool includeNonPublic = false)
        {
            var found = type.GetTypeInfo().DeclaredMethods.FirstOrDefault(method => (includeNonPublic || method.IsPublic) && method.Name == name);
            if (found == null)
                throw new InvalidOperationException($"'{name}' method not found on {type.FullName}.");

            return found;
        }

        public static int GetIndex<TKey, TValue>(this KeyValuePair<TKey, TValue>[] array, TKey element)
        {
            if (array == null || array.Length == 0) return -1;

            var length = array.Length;
            if (length == 1) return array[0].Key.Equals(element) ? 0 : -1;

            for (int i = 0; i < length; i++)
                if (array[i].Key.Equals(element))
                    return i;

            return -1;
        }

        public static MethodInfo GetSingleMethodOrDefault(this Type type, string name, bool includeNonPublic = false) =>
            type.GetTypeInfo().DeclaredMethods.FirstOrDefault(method => (includeNonPublic || method.IsPublic) && method.Name == name);

        public static bool HasSetMethod(this MemberInfo property, bool includeNonPublic = false) =>
            property.GetSetterMethodOrDefault(includeNonPublic) != null;

        public static MethodInfo GetSetterMethodOrDefault(this MemberInfo property, bool includeNonPublic = false) =>
            property.DeclaringType.GetSingleMethodOrDefault("set_" + property.Name, includeNonPublic);

        public static MethodInfo GetGetterMethodOrDefault(this MemberInfo property, bool includeNonPublic = false) =>
            property.DeclaringType.GetSingleMethodOrDefault("get_" + property.Name, includeNonPublic);

        public static Type GetEnumerableType(this Type type)
        {
            var typeInfo = type.GetTypeInfo();
            if (typeInfo.IsArray)
                return type.GetElementType();

            if (IsAssignableToGenericType(type, typeof(IEnumerable<>)) && type != typeof(string))
                return typeInfo.GenericTypeArguments[0];

            return null;
        }

        private static bool IsAssignableToGenericType(Type type, Type genericType)
        {
            if (type == null || genericType == null) return false;

            return type == genericType
              || MapsToGenericTypeDefinition(type, genericType)
              || HasInterfaceThatMapsToGenericTypeDefinition(type, genericType)
              || IsAssignableToGenericType(type.GetTypeInfo().BaseType, genericType);
        }

        private static bool MapsToGenericTypeDefinition(Type type, Type genericType)
        {
            return genericType.GetTypeInfo().IsGenericTypeDefinition
              && type.GetTypeInfo().IsGenericType
              && type.GetGenericTypeDefinition() == genericType;
        }

        private static bool HasInterfaceThatMapsToGenericTypeDefinition(Type type, Type genericType)
        {
            return type.GetTypeInfo().ImplementedInterfaces
              .Where(it => it.GetTypeInfo().IsGenericType)
              .Any(it => it.GetGenericTypeDefinition() == genericType);
        }
    }
}
