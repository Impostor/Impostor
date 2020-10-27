using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Impostor.Server.Plugins.Services
{
    public static class ProxyGenerator
    {
        private static readonly ConcurrentDictionary<Type, Type> TypeCache = new ConcurrentDictionary<Type, Type>();

        private static AssemblyBuilder GetAsmBuilder(string name)
        {
            return AssemblyBuilder.DefineDynamicAssembly(new AssemblyName { Name = name }, AssemblyBuilderAccess.Run);
        }

        public static T GetServiceProxy<T>(T instance)
        {
            return (T) GetServiceProxy(typeof(T), instance);
        }

        public static object GetServiceProxy(Type service, object instance)
        {
            var type = TypeCache.GetOrAdd(service, ServiceFactory);
            var proxy = (IServiceProxy) Activator.CreateInstance(type)!;

            proxy.SetInstance(instance);

            return proxy;
        }

        public static Type GetServiceProxyType(Type type) => TypeCache.GetOrAdd(type, ServiceFactory);

        private static Type ServiceFactory(Type service)
        {
            var assemblyBuilder = GetAsmBuilder(service.Name);

            var moduleBuilder = assemblyBuilder.DefineDynamicModule("ServiceProxies." + service.Name);
            var typeBuilder = moduleBuilder.DefineType(service.Name + "Proxy", TypeAttributes.Public | TypeAttributes.Class);

            typeBuilder.AddInterfaceImplementation(service);
            typeBuilder.AddInterfaceImplementation(typeof(IServiceProxy));

            var serviceField = typeBuilder.DefineField("_service", service, FieldAttributes.Private);

            CreateSetInstanceMethod(service, typeBuilder, serviceField);

            foreach (var method in service.GetMethods())
            {
                if (method.Name.StartsWith("get_") || method.Name.StartsWith("set_"))
                {
                    continue;
                }

                CreateMethod(typeBuilder, method, serviceField);
            }

            foreach (var property in service.GetProperties())
            {
                CreateProperty(service, typeBuilder, property.Name, property.PropertyType, serviceField);
            }

            return typeBuilder.CreateType();
        }

        private static void CreateSetInstanceMethod(
            Type service,
            TypeBuilder typeBuilder,
            FieldInfo fieldBuilder)
        {
            var method = typeof(IServiceProxy).GetMethod(nameof(IServiceProxy.SetInstance));
            var parameters = method.GetParameters().Select(p => p.ParameterType).ToArray();
            var methodBuilder = typeBuilder.DefineMethod(
                method.Name,
                MethodAttributes.Public | MethodAttributes.Virtual,
                method.ReturnType,
                parameters);

            var ilGenerator = methodBuilder.GetILGenerator();
            ilGenerator.Emit(OpCodes.Ldarg_0);
            ilGenerator.Emit(OpCodes.Ldarg_1);
            ilGenerator.Emit(OpCodes.Castclass, service);
            ilGenerator.Emit(OpCodes.Stfld, fieldBuilder);
            ilGenerator.Emit(OpCodes.Ret);

            typeBuilder.DefineMethodOverride(methodBuilder, method);
        }

        private static void CreateMethod(
            TypeBuilder typeBuilder,
            MethodInfo method,
            FieldInfo fieldBuilder)
        {
            var nextId = 0;
            var genericParameters = method.GetGenericArguments();

            var methodBuilder = typeBuilder.DefineMethod(
                method.Name,
                MethodAttributes.Public | MethodAttributes.Virtual);

            var typeParameters = genericParameters.Length == 0
                ? Array.Empty<GenericTypeParameterBuilder>()
                : methodBuilder.DefineGenericParameters(genericParameters.Select(p => p.Name!).ToArray());

            var returnType = method.ReturnType;

            for (var i = 0; i < typeParameters.Length; i++)
            {
                var methodParam = genericParameters[i];
                var proxyParam = typeParameters[i];

                proxyParam.SetGenericParameterAttributes(methodParam.GenericParameterAttributes);
                proxyParam.SetBaseTypeConstraint(methodParam.BaseType);
                proxyParam.SetInterfaceConstraints(methodParam.GetInterfaces());

                if (methodParam == returnType)
                {
                    returnType = typeParameters[i];
                }
            }

            var parameters = method
                .GetParameters()
                .Select(p => p.ParameterType.IsGenericParameter
                    ? typeParameters[p.ParameterType.GenericParameterPosition]
                    : p.ParameterType)
                .ToArray();

            methodBuilder.SetReturnType(returnType);
            methodBuilder.SetParameters(parameters);

            var ilGenerator = methodBuilder.GetILGenerator();
            ilGenerator.Emit(OpCodes.Ldarg_0);
            ilGenerator.Emit(OpCodes.Ldfld, fieldBuilder);

            for (var i = 0; i < parameters.Length; i++)
            {
                switch (i)
                {
                    case <= 2:
                        ilGenerator.Emit(i switch
                        {
                            0 => OpCodes.Ldarg_1,
                            1 => OpCodes.Ldarg_2,
                            2 => OpCodes.Ldarg_3,
                            _ => throw new InvalidOperationException()
                        });
                        break;
                    case <= short.MaxValue:
                        ilGenerator.Emit(OpCodes.Ldarg_S, (short) i);
                        break;
                    default:
                        throw new InvalidOperationException("Services cannot have more than 32767 parameters in a method");
                }
            }

            ilGenerator.Emit(OpCodes.Callvirt, method);
            ilGenerator.Emit(OpCodes.Ret);

            typeBuilder.DefineMethodOverride(methodBuilder, method);
        }

        private static void CreateProperty(
            Type service,
            TypeBuilder typeBuilder,
            string propertyName,
            Type propType,
            FieldInfo fieldBuilder)
        {
            const MethodAttributes getSetAttr = MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Virtual;

            var property = typeBuilder.DefineProperty(
                propertyName,
                PropertyAttributes.None,
                propType,
                new[] { propType });

            var getMethod = service.GetMethod("get_" + propertyName);

            if (getMethod != null)
            {
                var getMethodBuilder = typeBuilder.DefineMethod(
                    "get_" + propertyName,
                    getSetAttr,
                    propType,
                    Type.EmptyTypes);

                var currGetIl = getMethodBuilder.GetILGenerator();
                currGetIl.Emit(OpCodes.Ldarg_0);
                currGetIl.Emit(OpCodes.Ldfld, fieldBuilder);
                currGetIl.Emit(OpCodes.Callvirt, getMethod);
                currGetIl.Emit(OpCodes.Ret);

                property.SetGetMethod(getMethodBuilder);
                typeBuilder.DefineMethodOverride(getMethodBuilder, getMethod);
            }

            var setMethod = service.GetMethod("set_" + propertyName);

            if (setMethod != null)
            {
                var setMethodBuilder = typeBuilder.DefineMethod(
                    "set_" + propertyName,
                    getSetAttr,
                    null,
                    new[] { propType });

                // store value in private field and set the isdirty flag
                var currSetIl = setMethodBuilder.GetILGenerator();
                currSetIl.Emit(OpCodes.Ldarg_0);
                currSetIl.Emit(OpCodes.Ldfld, fieldBuilder);
                currSetIl.Emit(OpCodes.Ldarg_1);
                currSetIl.Emit(OpCodes.Callvirt, setMethod);
                currSetIl.Emit(OpCodes.Ret);

                property.SetSetMethod(setMethodBuilder);
                typeBuilder.DefineMethodOverride(setMethodBuilder, setMethod);
            }
        }
    }
}
