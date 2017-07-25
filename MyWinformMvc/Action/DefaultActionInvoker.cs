using System;
using System.Collections.Generic;
using System.Reflection;
using My.WinformMvc.Core;
using My.WinformMvc.Extensions;
using My.WinformMvc.Filters;
using My.WinformMvc.Filters.Contexts;
using My.WinformMvc.Properties;

using System.Linq;
using Delegates = System;
//using Delegates = My.WinformMvc.Foundation;

namespace My.WinformMvc.Action
{
    /// <summary>
    /// An action invoker, which implements the [Chain Of Resposibility] mode
    /// </summary>
    class DefaultActionInvoker : IActionInvoker
    {
        FilterInfo _actionFilterInfo;
        readonly MethodInfo _actionMethod;

        internal DefaultActionInvoker(MethodInfo actionMethod)
        {
            //TODO: Use delegate to improve performance
            _actionMethod = actionMethod;
        }

        public void InvokeAction(BaseController context, object[] parameters)
        {
            if (_actionFilterInfo == null)
            {
                lock (_actionMethod)
                    _actionFilterInfo = _actionFilterInfo ?? _actionMethod.GetFilters();
            }
            try
            {
                if (ReferenceEquals(_actionFilterInfo, FilterInfo.Instance))
                {
                    var actionResult = InvokeActionMethod(context, parameters);
                    InvokeActionResult(context, actionResult);
                }
                else
                {
                    //Invoking Autorization filters
                    AuthorizationContext authContext = InvokeAuthorizationFilters(context,
                        _actionFilterInfo.AuthorizationFilters, parameters);
                    if (authContext != null)
                    {
                        if (authContext.Result != null)
                        {
                            InvokeActionResult(context, authContext.Result);
                            return;
                        }
                    }

                    //Executing the action
                    ActionExecutedContext postActionContext = InvokeActionMethodWithFilters(context,
                        _actionFilterInfo.ActionFilters, parameters);
                    // (controllerContext, filterInfo.ActionFilters, actionDescriptor, parameters);

                    if (postActionContext.Result == null) return;
                    //Executing the actionResultFilter
                    ResultExecutedContext postResultContext = InvokeActionResultWithFilters(context,
                        _actionFilterInfo.ResultFilters, postActionContext.Result, parameters);
                }
            }
            catch (TargetInvocationException ex)
            {
                ProcessExceptionWhenInvokeAction(context, ex.InnerException ?? ex, parameters);
            }
            catch (Exception ex)
            {
                ProcessExceptionWhenInvokeAction(context, ex, parameters);
            }
        }

        void ProcessExceptionWhenInvokeAction(BaseController context, Exception ex, object[] parameters)
        {
            Logger.Error(ex.ToString());
            //Executing the exception filter
            ExceptionContext exceptionContext = InvokeExceptionFilters(context, _actionFilterInfo.ExceptionFilters, ex.InnerException ?? ex, parameters);
            if (!exceptionContext.ExceptionHandled)
                throw new Exception(string.Format(Resources.ActionExecutionException, _actionMethod.Name, ex.Message));
            InvokeActionResult(context, exceptionContext.Result);
        }

        /// <summary>
        /// Invoke the action of the context
        /// </summary>
        /// <param name="context">The context</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>
        /// The result of the action. Null is returned for asynchronous actions
        /// </returns>
        IActionResult InvokeActionMethod(BaseController context, object[] parameters)
        {
            return (IActionResult)_actionMethod.Invoke(context, parameters);
        }

        /// <summary>
        /// Execute the pre filter, the action and the post filter attribute
        /// </summary>
        /// <param name="filter">The action filter attribute</param>
        /// <param name="preContext">The builded executing context</param>
        /// <param name="continuation">Lambda expression that create the actionExecutedContext </param>
        /// <returns></returns>
        ActionExecutedContext InvokeActionMethodFilter(IActionFilter filter, ActionExecutingContext preContext, Delegates.Func<ActionExecutedContext> continuation)
        {
            //Execute the filter
            filter.OnActionExecuting(preContext);
            //if the execution process is cancelled by the pre filter
            if (preContext.Result != null)
            {
                return new ActionExecutedContext(true /*cancelled*/, null/*exception*/)
                {
                    Controller = preContext.Controller,
                    ActionParameters = preContext.ActionParameters,
                    ActionMethod = _actionMethod,
                    Result = preContext.Result
                };
            }

            bool wasError = false;
            ActionExecutedContext postContext = null;
            try
            {
                //Executing the action and initializing the postContext
                postContext = continuation();
            }
            catch (TargetInvocationException ex)
            {
                wasError = true;
                ProcessExceptionWhenInvokeActionMethodFilter(filter, preContext, ex.InnerException ?? ex);
            }
            catch (Exception ex)
            {
                //An exception is caught
                wasError = true;
                ProcessExceptionWhenInvokeActionMethodFilter(filter, preContext, ex);
            }

            //Everything is well done
            if (!wasError && postContext.Result != null)
                filter.OnActionExecuted(postContext);
            return postContext;
        }

        void ProcessExceptionWhenInvokeActionMethodFilter(IActionFilter filter, ActionExecutingContext preContext, Exception ex)
        {
            var postContext = new ActionExecutedContext(false /*cancelled*/, ex /*exception*/)
            {
                Controller = preContext.Controller,
                ActionParameters = preContext.ActionParameters,
                ActionMethod = _actionMethod
            };
            filter.OnActionExecuted(postContext);

            //If the error is not handled by the filter the exception is thrown
            if (!postContext.ExceptionHandled)
                throw ex;
        }

        /// <summary>
        /// Execute the actionResult passed as parameter
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="actionResult">The result to execute</param>
        void InvokeActionResult(IController context, IActionResult actionResult)
        {
            actionResult.ExecuteResult(context);
        }

        /// <summary>
        /// Execute the prefilter result, the actionResult and the postfilter result
        /// </summary>
        /// <param name="filter">The result filter</param>
        /// <param name="preContext">The builded preContext</param>
        /// <param name="continuation">The lambda expression wich execute the actionResult and the post filter result attribute</param>
        /// <returns>The result of the execution of the post filter result attribute</returns>
        ResultExecutedContext InvokeActionResultFilter(IResultFilter filter, ResultExecutingContext preContext, Delegates.Func<ResultExecutedContext> continuation)
        {
            //Executing the filter
            filter.OnResultExecuting(preContext);
            //If the execution is cancelled
            if (preContext.Cancel)
            {
                return new ResultExecutedContext(preContext.Result, true /* canceled */, null /* exception */)
                {
                    Controller = preContext.Controller,
                    ActionParameters = preContext.ActionParameters,
                    ActionMethod = _actionMethod
                };
            }

            bool wasError = false;
            ResultExecutedContext postContext = null;
            try
            {
                //Creating the ResultExecutedContext by executing the actionResult
                postContext = continuation();
            }
            catch (TargetInvocationException ex)
            {
                wasError = true;
                ProcessExceptionWhenInvokeActionResultFilter(filter, preContext, ex.InnerException ?? ex);
            }
            catch (Exception ex)
            {
                wasError = true;
                ProcessExceptionWhenInvokeActionResultFilter(filter, preContext, ex);
            }

            //Everything is executed well
            if (!wasError)
                filter.OnResultExecuted(postContext);

            return postContext;
        }

        void ProcessExceptionWhenInvokeActionResultFilter(IResultFilter filter, ResultExecutingContext preContext, Exception ex)
        {
            //Creating the context
            var postContext = new ResultExecutedContext(preContext.Result, false /* canceled */, ex)
            {
                Controller = preContext.Controller,
                ActionParameters = preContext.ActionParameters,
                ActionMethod = _actionMethod
            };

            filter.OnResultExecuted(postContext);

            //If the exception is !handled we throw it
            if (!postContext.ExceptionHandled)
                throw ex;
        }

        /// <summary>
        /// Invoke the list of all authorization filters
        /// </summary>
        /// <param name="context">The context</param>
        /// <param name="filters">The list of authorization filters</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>
        /// The resulted authorization context
        /// </returns>
        AuthorizationContext InvokeAuthorizationFilters(BaseController context, IAuthorizationFilter[] filters, object[] parameters)
        {
            if (filters == null || filters.Length == 0)
                return null; //If the list is empty or null then nothing is executed

            var authContext = new AuthorizationContext
            {
                Controller = context,
                ActionParameters = parameters,
                ActionMethod = _actionMethod
            };

            //Executing filters
            foreach (IAuthorizationFilter filter in filters)
            {
                filter.OnAuthorization(authContext);
                if (authContext.Result != null) 
                    break; //Something is wrong if we get this far
            }
            return authContext;
        }

        /// <summary>
        /// Invoke the list of all excpetion filters
        /// </summary>
        /// <param name="context">The context</param>
        /// <param name="filters">The list of all filters</param>
        /// <param name="exception">The exception wich is caught</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>
        /// The resulted ExceptionContext
        /// </returns>
        ExceptionContext InvokeExceptionFilters(BaseController context, IExceptionFilter[] filters, Exception exception, object[] parameters)
        {
            var expContext = new ExceptionContext(exception)
            {
                Controller = context,
                ActionParameters = parameters,
                ActionMethod = _actionMethod
            };
            if (filters == null) 
                return expContext;

            // Execute reversely, according to the Asp.Net
            for (int i = filters.Length - 1; i >= 0; i--)
                filters[i].OnException(expContext);

            //foreach (IExceptionFilter filter in filters)
            //    filter.OnException(expContext);
            return expContext;
        }

        ///// <summary>
        ///// Executes the list of action filters that have their OnActionExecuting executed
        ///// </summary>
        ///// <param name="executedActionFilters">The list of executed action filters</param>
        ///// <param name="postContext">The context of the execution</param>
        ///// <returns></returns>
        //ActionExecutedContext InvokeExecutedFilters(IActionFilter[] executedActionFilters, ActionExecutedContext postContext)
        //{
        //    //Executing the list of all Executed action filters
        //    foreach (IActionFilter executedActionFilter in executedActionFilters)
        //        executedActionFilter.OnActionExecuted(postContext);
        //    return postContext;
        //}

        /// <summary>
        /// Launch the execution of filters and action method
        /// </summary>
        /// <param name="context">The context</param>
        /// <param name="filters">The list of action filters</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>
        /// The actionExecutedContext build by the execution
        /// </returns>
        protected virtual ActionExecutedContext InvokeActionMethodWithFilters(BaseController context, IActionFilter[] filters, object[] parameters)
        {
            //If there is no ActionFilter, the action is simple executed
            if (filters == null)
            {
                var postContext = new ActionExecutedContext(false, null)
                {
                    Controller = context,
                    ActionParameters = parameters,
                    ActionMethod = _actionMethod,
                    Result = InvokeActionMethod(context, parameters)
                };
                return postContext;
            }

            ////Now we know that there is, at least one actionFilter
            //if (_executedActionFilters == null) _executedActionFilters = new List<IActionFilter>();
            //else _executedActionFilters.Clear();

            var preContext = new ActionExecutingContext
            {
                Controller = context,
                ActionParameters = parameters,
                ActionMethod = _actionMethod
            };

            //Passing the list throught the lambda expression
            Delegates.Func<ActionExecutedContext> continuation = () =>
            {
                var actContext = new ActionExecutedContext(false, null)
                {
                    Controller = context,
                    ActionParameters = parameters,
                    ActionMethod = _actionMethod,
                    Result = InvokeActionMethod(context, parameters)
                };
                return actContext;
            };

            //Delegates.Func<ActionExecutedContext> reverseFilters = continuation;
            //// need to reverse the filter list because the continuations are built up backward
            //for (int i = filters.Length - 1; i >= 0; i--)
            //{
            //    var filter = filters[i];
            //    Delegates.Func<ActionExecutedContext> next = () => InvokeActionMethodFilter(filter, preContext, continuation);
            //    reverseFilters += next;
            //}

            Delegates.Func<ActionExecutedContext> reverseFilters = filters.Reverse().Aggregate(continuation,
                (next, filter) => () => InvokeActionMethodFilter(filter, preContext, next));

            return reverseFilters();
        }

        /// <summary>
        /// Execute filters and result
        /// </summary>
        /// <param name="context">The context</param>
        /// <param name="filters">The list of filters</param>
        /// <param name="actionResult">The result to executed</param>
        /// <returns>
        /// The resultExecutedContext
        /// </returns>
        protected virtual ResultExecutedContext InvokeActionResultWithFilters(BaseController context, IResultFilter[] filters, IActionResult actionResult, object[] parameters)
        {
            //if there is no available result filter the actionResult is simply saved
            if (filters == null)
            {
                InvokeActionResult(context, actionResult);
                return null;
            }

            Delegates.Func<ResultExecutedContext> continuation = () =>
            {
                InvokeActionResult(context, actionResult);
                return new ResultExecutedContext(actionResult, false /* canceled */, null /* exception */)
                {
                    Controller = context,
                    ActionParameters = parameters,
                    ActionMethod = _actionMethod
                };
            };

            var preContext = new ResultExecutingContext(actionResult)
            {
                Controller = context,
                ActionParameters = parameters,
                ActionMethod = _actionMethod
            };

            //Delegates.Func<ResultExecutedContext> reverseFilters = continuation;
            //// need to reverse the filter list because the continuations are built up backward
            //for (int i = filters.Length - 1; i >= 0; i--)
            //{
            //    var filter = filters[i];
            //    Delegates.Func<ResultExecutedContext> next = () => InvokeActionResultFilter(filter, preContext, continuation);
            //    reverseFilters += next;
            //}

            // need to reverse the filter list because the continuations are built up backward
            Delegates.Func<ResultExecutedContext> reverseFilters = filters.Reverse().Aggregate(continuation,
                (next, filter) => () => InvokeActionResultFilter(filter, preContext, next));
            return reverseFilters();
        }
    }
}
