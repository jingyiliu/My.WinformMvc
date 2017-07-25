
//namespace My.WinformMvc.Action
//{
//    /// <summary>
//    /// TODO: Update summary.
//    /// </summary>
//    public class AsyncActionInvoker : IActionInvoker
//    {
//        //void NavigateTo(Controller currentController)
//        //{
//        //    if (!_formerController.ControllerName.Equals(_bindingManager.DefaultControllerName))
//        //    {
//        //        var formerView = _formerController.GetView();
//        //        formerView.Close();
//        //        _formerController.Dispose();
//        //    }
//        //    currentController.ShowView();
//        //}

//        //protected override void Dispose(bool disposing)
//        //{
//        //    if (disposing)
//        //    {
//        //        _objectContainer.Dispose();
//        //    }
//        //}

//        ///// <summary>
//        ///// Execute asynchronous actions. It's executed in a different thread
//        ///// </summary>
//        ///// <param name="sender">The sender object</param>
//        ///// <param name="e">Event's arguments</param>
//        //void BgAsyncMethods_DoWork(object sender, DoWorkEventArgs e)
//        //{
//        //    //object[] parameters = null;
//        //    //IController controller = null;
//        //    //string actionName = null;
//        //    //object[] results = null;
//        //    //IActionResult actionResult = null;
//        //    //Exception exception = null;

//        //    //try
//        //    //{
//        //    //    BgAsyncMethods.ReportProgress(1);

//        //    //    //Getting arguments
//        //    //    parameters = e.Argument as object[];
//        //    //    //Retrieving controller and action to invoke
//        //    //    controller = parameters[0] as IController;
//        //    //    actionName = parameters[1] as string;

//        //    //    //Building de result object
//        //    //    results = new object[] { actionResult, controller, exception /*exception*/ }; 

//        //    //    //Executing action
//        //    //    actionResult = (IActionResult)controller.GetActionMethod(actionName).Invoke(controller, null);			    

//        //    //    //Storing the result object for the next step
//        //    //    results[0] = actionResult;
//        //    //    e.Result = results;
//        //    //}
//        //    //catch (Exception ex)
//        //    //{
//        //    //    //Home the result object
//        //    //    results[2] = ex.InnerException; /* The innerException is used because the InvokeAction method of a MethodInfo throws a System.TargetInvocationException*/ 
//        //    //    e.Result = results;
//        //    //}
//        //}

//        ///// <summary>
//        ///// Executed when the background process has ended. 
//        ///// This method is executed by the principal thread
//        ///// </summary>
//        ///// <param name="sender">Sender object</param>
//        ///// <param name="e">Event's Arguments</param>
//        //void BgAsyncMethods_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
//        //{
//        //    ////error occurs or background process is cancelled
//        //    //if (e.Cancelled) return;

//        //    ////Background process execution has ended successfully.
//        //    ////Executing the next command
//        //    //object[] results = e.Result as object[];
//        //    //IActionResult actionResult = results[0] as IActionResult;
//        //    //IController controller = results[1] as IController;

//        //    ////Retrieve the exception
//        //    //Exception exception = results[2] as Exception;

//        //    //try
//        //    //{
//        //    //    //If error occurs
//        //    //    if (exception != null)
//        //    //    {
//        //    //        //Executing the post action execution filter of all pre executed filter
//        //    //        if (_executedActionFilters != null)
//        //    //        {
//        //    //            //Creating the context. The exception is passed so that the after attribute can use it
//        //    //            ActionExecutedContext postContext = new ActionExecutedContext(controller, false /*cancelled*/, exception /*exception*/);
//        //    //            postContext = InvokeExecutedFilters(_executedActionFilters, postContext);
//        //    //            if (!postContext.ExceptionHandled)
//        //    //            {
//        //    //                throw exception;
//        //    //            }
//        //    //        }
//        //    //        else throw exception;
//        //    //    }

//        //    //    //if no errors
//        //    //    if (_executedActionFilters != null) /*Executing OnActionExecuted*/
//        //    //    {
//        //    //        ActionExecutedContext postContext = new ActionExecutedContext(controller, false, null) { Result = actionResult };
//        //    //        postContext = InvokeExecutedFilters(_executedActionFilters, postContext);

//        //    //        //Initializing the actionResult
//        //    //        actionResult = postContext.Result;
//        //    //    }

//        //    //    //Executing the actionResultFilter
//        //    //    ResultExecutedContext postResultContext = InvokeActionResultWithFilters(controller, _actionFilterInfo.ResultFilters, actionResult);

//        //    //    // Executing the result
//        //    //    if (postResultContext != null) postResultContext.Result.ExecuteResult(this);
//        //    //}
//        //    //catch (Exception ex)
//        //    //{
//        //    //    //The exception is not handled. the exception filter is called to manage the exception
//        //    //    ExceptionContext exceptionContext = InvokeExceptionFilters(controller, _actionFilterInfo.ExceptionFilters, ex);
//        //    //    if (!exceptionContext.ExceptionHandled)
//        //    //    {
//        //    //        throw new Exception(string.Format(Resources.ActionExecutionException, "actionName", ex.Message));
//        //    //    }
//        //    //    InvokeActionResult(exceptionContext.Result);
//        //    //}
//        //}
//    }
//}
