﻿#if NET45 || NET40 || IL_EMIT
using Stashbox.Utils;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;

namespace Stashbox.BuildUp.Expressions.Compile
{
    internal class DynamicModule
    {
        internal static readonly Type[] EmptyTypes = new Type[0];
        internal static readonly Type ObjectType = typeof(object);
        internal static readonly ConstructorInfo ObjectConstructor = ObjectType.GetConstructor(EmptyTypes);

#if NETSTANDARD
        public static readonly Lazy<ModuleBuilder> ModuleBuilder = new Lazy<ModuleBuilder>(() =>
            DynamicAssemblyBuilder.Value.DefineDynamicModule("Stashbox.Dynamic"));
#else
        public static readonly Lazy<ModuleBuilder> ModuleBuilder = new Lazy<ModuleBuilder>(() =>
            DynamicAssemblyBuilder.Value.DefineDynamicModule("Stashbox.Dynamic"));
#endif

#if NETSTANDARD
        public static readonly Lazy<AssemblyBuilder> DynamicAssemblyBuilder = new Lazy<AssemblyBuilder>(() =>
            AssemblyBuilder.DefineDynamicAssembly(
                new AssemblyName("Stashbox.Dynamic"),
                AssemblyBuilderAccess.Run));
#else
        public static readonly Lazy<AssemblyBuilder> DynamicAssemblyBuilder = new Lazy<AssemblyBuilder>(() =>
            AppDomain.CurrentDomain.DefineDynamicAssembly(
                new AssemblyName("Stashbox.Dynamic"),
                AssemblyBuilderAccess.Run));
#endif
        private static int typeCounter;

        private static Dictionary<int, Type> TargetTypes = new Dictionary<int, Type>();

        private static Dictionary<int, Type> CapturedArgumentTypes = new Dictionary<int, Type>();

        public static Type GetOrAddTargetType(Type[] types)
        {
            var length = types.Length;

            var foundType = TargetTypes.GetOrDefault(length);
            if (foundType != null)
                return foundType.MakeGenericType(types);

            var fields = new FieldInfo[length];
            var typeBuilder = ModuleBuilder.Value.DefineType("Stashbox.Dynamic.DT" + Interlocked.Increment(ref typeCounter), TypeAttributes.Public);

            if (length > 0)
            {
                var typeParamNames = new string[length];
                for (var i = 0; i < length; i++)
                    typeParamNames[i] = "T" + i;

                var typeParams = typeBuilder.DefineGenericParameters(typeParamNames);
#if NETSTANDARD
                var genericTypes = new Type[length];
#endif

                for (var i = 0; i < length; i++)
                {
#if NETSTANDARD
                    var genericType = typeParams[i].AsType();
                    genericTypes[i] = genericType;
                    fields[i] = typeBuilder.DefineField("F" + i, genericType, FieldAttributes.Public);
#else
                    fields[i] = typeBuilder.DefineField("F" + i, typeParams[i], FieldAttributes.Public);
#endif
                }

#if NETSTANDARD
                var constructor = typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.HasThis, genericTypes);
#else
                var constructor = typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.HasThis, typeParams);
#endif
                var generator = constructor.GetILGenerator();

                generator.Emit(OpCodes.Ldarg_0);
                generator.Emit(OpCodes.Call, ObjectConstructor);

                for (var i = 0; i < length; i++)
                {
                    generator.Emit(OpCodes.Ldarg_0);
                    generator.LoadParameter(i + 1);
                    generator.Emit(OpCodes.Stfld, fields[i]);
                }

                generator.Emit(OpCodes.Ret);
            }

#if NETSTANDARD
            var type = typeBuilder.CreateTypeInfo().AsType();
#else
            var type = typeBuilder.CreateType();
#endif
            Swap.SwapValue(ref TargetTypes, t => t.AddOrUpdate(length, type));
            return type.MakeGenericType(types);
        }

        public static Type GetOrAddCapturedArgumentsType(Expression[] expressions)
        {
            var length = expressions.Length;
            var types = new Type[length];

            var foundType = CapturedArgumentTypes.GetOrDefault(length);
            if (foundType != null)
            {
                if (length <= 0) return foundType;

                for (var i = 0; i < length; i++)
                    types[i] = expressions[i].Type;

                return foundType.MakeGenericType(types);
            }

            var fields = new FieldInfo[length];

            var typeBuilder = ModuleBuilder.Value.DefineType("Stashbox.Dynamic.DT" + Interlocked.Increment(ref typeCounter), TypeAttributes.Public);

            var typeParamNames = new string[length];
            for (var i = 0; i < length; i++)
                typeParamNames[i] = "T" + i;

            var typeParams = typeBuilder.DefineGenericParameters(typeParamNames);
#if NETSTANDARD
                var genericTypes = new Type[length];
#endif

            for (var i = 0; i < length; i++)
            {
                types[i] = expressions[i].Type;
#if NETSTANDARD
                    var genericType = typeParams[i].AsType();
                    genericTypes[i] = genericType;
                    fields[i] = typeBuilder.DefineField("F" + i, genericType, FieldAttributes.Public);
#else
                fields[i] = typeBuilder.DefineField("F" + i, typeParams[i], FieldAttributes.Public);
#endif
            }
#if NETSTANDARD
            var type = typeBuilder.CreateTypeInfo().AsType();
#else
            var type = typeBuilder.CreateType();
#endif
            Swap.SwapValue(ref CapturedArgumentTypes, t => t.AddOrUpdate(length, type));
            return type.MakeGenericType(types);
        }
    }
}
#endif