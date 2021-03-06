using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Alsein.Utilities.Runtime
{
    /// <summary>
    /// 
    /// </summary>
    public static class Proxy
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="implements"></param>
        /// <returns></returns>
        public static IProxyBinder CreateProxyBinder(Type type, IEnumerable<KeyValuePair<MethodInfo, Delegate>> implements) =>
            new InterfaceProxyBinder(type, implements);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="implements"></param>
        /// <returns></returns>
        public static IProxyBinder CreateProxyBinder(Type type, params (MethodInfo Key, Delegate Value)[] implements) =>
            new InterfaceProxyBinder(type, implements.Select(imp => new KeyValuePair<MethodInfo, Delegate>(imp.Key, imp.Value)));

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TInterface"></typeparam>
        /// <param name="implements"></param>
        /// <returns></returns>
        public static IProxyBinder<TInterface> CreateProxyBinder<TInterface>(IEnumerable<KeyValuePair<MethodInfo, Delegate>> implements) =>
            new InterfaceProxyBinder<TInterface>(implements);


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TInterface"></typeparam>
        /// <param name="implements"></param>
        /// <returns></returns>
        public static IProxyBinder<TInterface> CreateProxyBinder<TInterface>(params (MethodInfo Key, Delegate Value)[] implements) =>
            new InterfaceProxyBinder<TInterface>(implements.Select(imp => new KeyValuePair<MethodInfo, Delegate>(imp.Key, imp.Value)));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="targets"></param>
        /// <returns></returns>
        public static object CreateMulticastProxy(Type type, IEnumerable<object> targets) =>
            CreateProxyBinder(type, type.GetMethods().Select(m => new KeyValuePair<MethodInfo, Delegate>(m, new VariableArgsHandler((ta, va) =>
                m.ReturnType.IsInterface ? CreateMulticastProxy(m.ReturnType, targets.Select(target =>
                {
                    return (m.IsGenericMethodDefinition ? m.MakeGenericMethod(ta) : m).Invoke(target, va);
                }).ToList()) : null
            )))).GetProxy();

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TInterface"></typeparam>
        /// <param name="targets"></param>
        /// <returns></returns>
        public static TInterface CreateMulticastProxy<TInterface>(IEnumerable<TInterface> targets) => (TInterface)CreateMulticastProxy(typeof(TInterface), targets.Select(x => (object)x));
    }
}