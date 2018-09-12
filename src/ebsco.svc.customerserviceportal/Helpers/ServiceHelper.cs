using System;
using System.ServiceModel;
using System.Threading.Tasks;

namespace ebsco.svc.customerserviceportal.Helpers
{
    /// <summary>
    /// Helper class to assist with properly handling errors, closing, and cleaning up behind WCF calls.
    /// </summary>
    public static class ServiceHelper
    {
        /// <summary>
        /// This method executes the given expression against an instance of the service type 
        /// </summary>
        /// <typeparam name="TService">The type of the service</typeparam>
        /// <typeparam name="TReturn">The result of the service call</typeparam>
        /// <param name="serviceProxy">The actual service client object</param>
        /// <param name="func">The action to take against the service</param>
        /// <example>return ServicesHelper.ExecuteServiceCall(serviceClient, x => x.GetNetTitleMarkups(request));</example>
        public static TReturn ExecuteServiceCall<TService, TReturn>(TService serviceProxy, Func<TService, TReturn> func)
            where TReturn : class
        {

            TReturn result = null;
            ExecuteServiceCall(serviceProxy, x =>
            {
                result = func(x);
            });

            return result;
        }

                /// <summary>
        /// This method executes the given expression against an instance of the service type 
        /// </summary>
        /// <typeparam name="TService">The type of the service</typeparam>
        /// <typeparam name="TReturn">The result of the service call</typeparam>
        /// <param name="serviceProxy">The actual service client object</param>
        /// <param name="func">The action to take against the service</param>
        /// <example>return ServicesHelper.ExecuteServiceCall(serviceClient, x => x.GetNetTitleMarkups(request));</example>
        public static TReturn ExecuteServiceCall<TService, TReturn>(TService serviceProxy, Func<TService, Task<TReturn>> func)
            where TReturn : class
        {

            TReturn result = null;
            ExecuteServiceCall(serviceProxy, x =>
            {
                result = func(x).Result;
            });

            return result;
        }

        /// <summary>
        /// This method executes the given expression against an instance of the service type 
        /// </summary>
        /// <typeparam name="TService">The type of the service</typeparam>
        /// <param name="serviceProxy">The actual service client object</param>
        /// <param name="action">The action to take against the service</param>
        public static void ExecuteServiceCall<TService>(TService serviceProxy, Action<TService> action)
        {
            
            var isCommunicationObject = serviceProxy as ICommunicationObject != null;

            try
            {
                action(serviceProxy);

                if (isCommunicationObject)
                    ((ICommunicationObject)serviceProxy).Close();
            }
            catch (Exception)
            {
                if (isCommunicationObject)
                {
                    ((ICommunicationObject)serviceProxy).Abort();
                    ((ICommunicationObject)serviceProxy).Close();
                }

                throw;
            }
            finally
            {
                if (serviceProxy is IDisposable disposableService)
                {
                    disposableService.Dispose();
                }
            }
        }
    }
}