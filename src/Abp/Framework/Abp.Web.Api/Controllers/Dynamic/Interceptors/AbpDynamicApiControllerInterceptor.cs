using System.Reflection;
using Castle.DynamicProxy;

namespace Abp.WebApi.Controllers.Dynamic.Interceptors
{
    /// <summary>
    /// Interceptor dynamic controllers.
    /// It handles method calls to dynmaic generated Api controoler and 
    /// calls underlying proxied object.
    /// </summary>
    /// <typeparam name="T">Type of the proxied object</typeparam>
    public class AbpDynamicApiControllerInterceptor<T> : IInterceptor
    {
        /// <summary>
        /// Real object instance to call it's methods when dynamic controller's methods are called.
        /// </summary>
        private readonly T _proxiedObject;

        /// <summary>
        /// Creates a new AbpDynamicApiControllerInterceptor object.
        /// </summary>
        /// <param name="proxiedObject">Real object instance to call it's methods when dynamic controller's methods are called</param>
        public AbpDynamicApiControllerInterceptor(T proxiedObject)
        {
            _proxiedObject = proxiedObject;
        }

        /// <summary>
        /// Intercepts method calls of dynamic api controller
        /// </summary>
        /// <param name="invocation">Method invocation informations</param>
        public void Intercept(IInvocation invocation)
        {
            //If method call is for generic type (T)...
            if (typeof(T).IsAssignableFrom(invocation.Method.DeclaringType))
            {
                //Call real object's method
                try
                {
                    invocation.ReturnValue = invocation.Method.Invoke(_proxiedObject, invocation.Arguments);
                }
                catch (TargetInvocationException targetInvocation)
                {
                    if (targetInvocation.InnerException != null)
                    {
                        throw targetInvocation.InnerException;
                    }

                    throw;
                }
            }
            else
            {
                //Call api controller's methods as usual.
                invocation.Proceed();
            }
        }
    }
}