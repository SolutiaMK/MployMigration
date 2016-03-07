using System;
using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;

namespace ConsoleApplication1.Helper
{
    /// <summary>
    /// Catch block rethrow behaviors 
    /// </summary>
    public enum Rethrow
    {
        No = 0,     // Don't rethrow
        Throw,      // Rethrow without exception ref
        ThrowNamed, // Rethrow with exception ref
        ThrowNew    // Throw new exception with inner exception set to original.
    }

    public class TryCatch
    {
        static void DoNothing() { }

        #region Declarations
        public static Action NoAction = new Action(() => DoNothing());
        public static Action<Exception> NoActionExc = new Action<Exception>((ex) => DoNothing());

        /// <summary>
        /// When neither null nor empty, implies dependence on configuration-referenced policy and on Microsoft.Practices.EnterpriseLibrary.Logging.
        /// Important: when not specified as a Wrap function parameter, the value of ExceptionPolicyName defaults to the most recent specification 
        /// either by class construction or by Wrap call parameter.
        /// </summary>
        public String ExceptionPolicyName = "Default";

        /// <summary>
        /// Retrow behavior performed at end of catch block.
        /// Important: when not specified as a Wrap function parameter, the value of Rethrow defaults to the most recent specification 
        /// either by class construction or by Wrap call parameter.
        /// </summary>
        public Rethrow Rethrow = Rethrow.No;

        /// <summary>
        /// Update ExceptionPolicyName and Rethrow with latest usage via Wrap function call.
        /// </summary>
        public Boolean UpdatePolicyAndRethrowFromUsage = true;
        #endregion Declarations

        // constructor
        public TryCatch() { }

        #region TRY as action ( i.e. no return )
        #region only try action specified
        /// <summary>
        /// Wrap specified action in try-catch.
        /// </summary>
        /// <param name="tryAction">Action injected for try block</param>
        /// <remarks>Exception policy and rethrow behavior defaulted from construction or persisted from prior usage.</remarks>
        public void Wrap(Action tryAction)
        {
            Wrap(tryAction, NoActionExc, NoAction, ExceptionPolicyName, Rethrow);
        }

        /// <summary>
        /// Wrap tryAction with try-catch using specified exception policy in catch block.
        /// </summary>
        /// <param name="tryAction">Action injected for try block</param>
        /// <param name="exceptionPolicyName">Must be config supported</param>
        /// <remarks>Rethrow behavior defaulted from construction or persisted from prior usage.</remarks>
        public void Wrap(Action tryAction, String exceptionPolicyName)
        {
            Wrap(tryAction, NoActionExc, NoAction, exceptionPolicyName, Rethrow);
        }

        /// <summary>
        /// Wrap tryAction with try-catch using specified exception policy and rethrow behavior in catch block.
        /// </summary>
        /// <param name="tryAction">Action injected for try block</param>
        /// <param name="exceptionPolicyName">Must be config supported</param>
        /// <param name="rethrow">final catch block behavior</param>
        public void Wrap(Action tryAction, String exceptionPolicyName, Rethrow rethrow)
        {
            Wrap(tryAction, NoActionExc, NoAction, exceptionPolicyName, rethrow);
        }
        #endregion only try action specified

        #region only try & catch actions specified
        /// <summary>
        /// Wrap tryAction in try block and inject catchAction into catch block 
        /// that uses specified exception policy and rethrow behavior.
        /// </summary>
        /// <param name="tryAction">Action injected for try block</param>
        /// <param name="catchAction">Initial action injected for catch block</param>
        /// <remarks>Exception policy and rethrow behavior defaulted from construction or persisted from prior usage.
        ///          Execution of exception policy and rethrow behaviors follow execution of catchAction.
        /// </remarks>
        public void Wrap(Action tryAction, Action<Exception> catchAction)
        {
            Wrap(tryAction, catchAction, NoAction, ExceptionPolicyName, Rethrow);
        }

        /// <summary>
        /// Wrap tryAction with try-catch
        /// </summary>
        /// <param name="tryAction">Action injected for try block</param>
        /// <param name="catchAction">Initial action injected for catch block</param>
        /// <param name="exceptionPolicyName">Must be config supported</param>
        /// <remarks>Rethrow behavior defaulted from construction or persisted from prior usage.
        ///          Execution of exception policy and rethrow behaviors follow execution of catchAction.
        /// </remarks>
        public void Wrap(Action tryAction, Action<Exception> catchAction, String exceptionPolicyName)
        {
            Wrap(tryAction, catchAction, NoAction, exceptionPolicyName, Rethrow);
        }

        /// <summary>
        /// Wrap tryAction with try-catch
        /// </summary>
        /// <param name="tryAction">Action injected for try block</param>
        /// <param name="catchAction">Initial action injected for catch block</param>
        /// <param name="exceptionPolicyName">Must be config supported</param>
        /// <param name="rethrow">final catch block behavior</param>         
        /// <remarks>Exception policy and rethrow behaviors defaulted from construction or prior usage.
        ///          Execution of exception policy and rethrow behaviors follow execution of catchAction.
        /// </remarks>
        public void Wrap(Action tryAction, Action<Exception> catchAction, String exceptionPolicyName, Rethrow rethrow)
        {
            Wrap(tryAction, catchAction, NoAction, exceptionPolicyName, rethrow);
        }
        #endregion only try & catch actions specified

        #region try, catch and finally actions specified
        /// <summary>
        /// Wrap tryAction, catchAction and finallyAction in try-catch-finally
        /// </summary>
        /// <param name="tryAction">Action injected for try block</param>
        /// <param name="catchAction">Initial action injected for catch block</param>
        /// <param name="finallyAction">Action injected for finally block</param>
        /// <remarks>Exception policy and rethrow behavior defaulted from construction or persisted from prior usage.
        ///          Execution of exception policy and rethrow behaviors follow execution of catchAction.
        /// </remarks>
        public void Wrap(Action tryAction, Action<Exception> catchAction, Action finallyAction)
        {
            Wrap(tryAction, catchAction, finallyAction, ExceptionPolicyName, Rethrow);
        }

        /// <summary>
        /// Wrap tryAction, catchAction and finallyAction in try-catch-finally where catch block uses specified exception policy.
        /// </summary>
        /// <param name="tryAction">Action injected for try block</param>
        /// <param name="catchAction">Initial action injected for catch block</param>
        /// <param name="finallyAction">Action injected for finally block</param>
        /// <param name="exceptionPolicyName">Must be config supported</param>
        /// <remarks>Rethrow behavior defaulted from construction or persisted from prior usage.
        ///          Execution of exception policy and rethrow behaviors follow execution of catchAction.
        /// </remarks>
        public void Wrap(Action tryAction, Action<Exception> catchAction, Action finallyAction, String exceptionPolicyName)
        {
            Wrap(tryAction, catchAction, finallyAction, exceptionPolicyName, Rethrow);
        }

        /// <summary>
        /// Wrap specified actions in try-catch-finally.
        /// </summary>
        /// <param name="tryAction">Action injected for try block</param>
        /// <param name="catchAction">Initial action injected for catch block</param>
        /// <param name="finallyAction">Action injected for finally block</param>
        /// <param name="exceptionPolicyName">Must be config supported</param>
        /// <param name="rethrow">final catch block behavior</param>
        /// <remarks>Exception policy and rethrow behaviors follow execution of catchAction</remarks>
        public void Wrap(Action tryAction, Action<Exception> catchAction, Action finallyAction, String exceptionPolicyName, Rethrow rethrow)
        {
            if (UpdatePolicyAndRethrowFromUsage)
            {
                ExceptionPolicyName = exceptionPolicyName;
                Rethrow = rethrow;
            }
            try
            {
                tryAction();
            }
            catch (Exception exc)
            {
                catchAction(exc);
                if (String.IsNullOrEmpty(ExceptionPolicyName) || ExceptionPolicy.HandleException(exc, exceptionPolicyName))
                {
                    if (rethrow != Rethrow.No)
                        switch (rethrow)
                        {
                            case Rethrow.Throw: throw;
                            case Rethrow.ThrowNamed: throw exc;
                            case Rethrow.ThrowNew: throw new Exception("re-throw", exc);
                        }
                }
            }
            finally
            {
                finallyAction();
            }
        }
        #endregion try, catch and finally actions specified

        #endregion TRY as action

        #region TRY as Func ( i.e. returning )
        #region only try Func specified
        /// <summary>
        /// Wrap the specified try Func with try-catch.
        /// </summary>
        /// <param name="tryFunc">Func injected for try block</param>
        /// <param name="returnValueAfterException">Value returned after execution of catch block</param>
        /// <remarks>Exception policy and rethrow behavior defaulted from construction or persisted from prior usage.
        ///          Exception policy and rethrow behaviors are injected into catch block.
        /// </remarks>
        public TResult Wrap<TResult>(Func<TResult> tryFunc, TResult returnValueAfterException = default(TResult))
        {
            return Wrap(tryFunc, NoActionExc, NoAction, ExceptionPolicyName, Rethrow, returnValueAfterException);
        }

        /// Wrap specified try Func with try-catch.
        /// </summary>
        /// <param name="tryFunc">Func injected for try block</param>
        /// <param name="exceptionPolicyName">Must be config supported</param>
        /// <param name="returnValueAfterException">Value returned after execution of catch block</param>
        /// <remarks>Rethrow behavior defaulted from construction or persisted from prior usage.
        ///          Exception policy and rethrow behaviors are injected into catch block.
        /// </remarks>     
        public TResult Wrap<TResult>(Func<TResult> tryFunc, String exceptionPolicyName, TResult returnValueAfterException = default(TResult))
        {
            return Wrap(tryFunc, NoActionExc, NoAction, exceptionPolicyName, Rethrow, returnValueAfterException);
        }

        /// <summary>
        /// Wrap specified try Func with try-catch.
        /// </summary>
        /// <param name="tryFunc">Func injected for try block</param>
        /// <param name="exceptionPolicyName">Must be config supported</param>
        /// <param name="rethrow">final catch block behavior</param>
        /// <param name="returnValueAfterException">Value returned after execution of catch block</param>
        /// <remarks>Exception policy and rethrow behaviors are injected into catch block.</remarks>
        public TResult Wrap<TResult>(Func<TResult> tryFunc, String exceptionPolicyName, Rethrow rethrow, TResult returnValueAfterException = default(TResult))
        {
            return Wrap(tryFunc, NoActionExc, NoAction, exceptionPolicyName, rethrow, returnValueAfterException);
        }
        #endregion only try action specified

        #region only try Func & catch action specified
        /// <summary>
        /// Wrap the specified try Func with try-catch.
        /// </summary>
        /// <param name="tryFunc">Func injected for try block</param>
        /// <param name="catchAction">Action injected into catch block</param>
        /// <param name="returnValueAfterException">Value returned after execution of catch block</param>
        /// <remarks>Exception policy and rethrow behavior defaulted from construction or persisted from prior usage.
        ///          Execution of exception policy and rethrow behaviors follows exection of catchAction.
        /// </remarks>
        public TResult Wrap<TResult>(Func<TResult> tryFunc, Action<Exception> catchAction, TResult returnValueAfterException = default(TResult))
        {
            return Wrap(tryFunc, catchAction, NoAction, ExceptionPolicyName, Rethrow, returnValueAfterException);
        }

        /// <summary>
        /// Wrap tryFunc with try-catch
        /// </summary>
        /// <param name="tryFunc">Func injected for try block</param>
        /// <param name="catchAction">Action injected into catch block</param>
        /// <param name="exceptionPolicyName">Must be config supported</param>
        /// <param name="returnValueAfterException">Value returned after execution of catch block</param>
        /// <remarks>Rethrow behavior defaulted from construction or persisted from prior usage.
        ///          Execution of exception policy and rethrow behaviors follows exection of catchAction.
        /// </remarks>
        public TResult Wrap<TResult>(Func<TResult> tryFunc, Action<Exception> catchAction, String exceptionPolicyName, TResult returnValueAfterException = default(TResult))
        {
            return Wrap(tryFunc, catchAction, NoAction, exceptionPolicyName, Rethrow, returnValueAfterException);
        }

        /// <summary>
        /// Wrap tryFunc with try-catch
        /// </summary>
        /// <param name="tryFunc">Action injected for try block</param>
        /// <param name="catchAction">Action injected into catch block</param>
        /// <param name="exceptionPolicyName">Must be config supported</param>
        /// <param name="rethrow">final catch block behavior</param>
        /// <param name="returnValueAfterException">Value returned after execution of catch block</param>
        /// <remarks>Execution of exception policy and rethrow behaviors follows exection of catchAction.</remarks>
        public TResult Wrap<TResult>(Func<TResult> tryFunc, Action<Exception> catchAction, String exceptionPolicyName, Rethrow rethrow, TResult returnValueAfterException = default(TResult))
        {
            return Wrap(tryFunc, catchAction, NoAction, exceptionPolicyName, rethrow, returnValueAfterException);
        }
        #endregion only try Func & catch action specified

        #region try Func and  catch & finally actions specified
        /// <summary>
        /// Wrap tryFunc with try-catch
        /// </summary>
        /// <param name="tryFunc">Action injected for try block</param>
        /// <param name="catchAction">Action injected into catch block</param>     
        /// <param name="finallyAction">finally block behavior</param>
        /// <param name="returnValueAfterException">Value returned after execution of catch block</param>
        /// <remarks>Exception policy and rethrow behavior defaulted from construction or persisted from prior usage.
        ///          Execution of exception policy and rethrow behaviors follows exection of catchAction.
        /// </remarks>
        public TResult Wrap<TResult>(Func<TResult> tryFunc, Action<Exception> catchAction, Action finallyAction, TResult returnValueAfterException = default(TResult))
        {
            return Wrap(tryFunc, catchAction, finallyAction, ExceptionPolicyName, Rethrow, returnValueAfterException);
        }

        /// <summary>
        /// Wrap tryFunc with try-catch
        /// </summary>
        /// <param name="tryFunc">Action injected for try block</param>
        /// <param name="catchAction">Action injected into catch block</param>     
        /// <param name="finallyAction">finally block behavior</param>
        /// <param name="exceptionPolicyName">Must be config supported</param>   
        /// <param name="returnValueAfterException">Value returned after execution of catch block</param>
        /// <remarks>Rethrow behavior defaulted from construction or persisted from prior usage.
        ///          Execution of exception policy and rethrow behaviors follows exection of catchAction.
        /// </remarks>
        public TResult Wrap<TResult>(Func<TResult> tryFunc, Action<Exception> catchAction, Action finallyAction, String exceptionPolicyName, TResult returnValueAfterException = default(TResult))
        {
            return Wrap(tryFunc, catchAction, finallyAction, exceptionPolicyName, Rethrow, returnValueAfterException);
        }

        /// <summary>
        /// Wrap tryFunc with try-catch
        /// </summary>
        /// <param name="tryFunc">Action injected for try block</param>
        /// <param name="catchAction">Action injected into catch block</param>     
        /// <param name="finallyAction">finally block behavior</param>
        /// <param name="exceptionPolicyName">Must be config supported</param>   
        /// <param name="rethrow">final catch block behavior</param>
        /// <param name="returnValueAfterException">Value returned after execution of catch block</param>
        /// <remarks>Execution of exception policy and rethrow behaviors follows exection of catchAction.</remarks>
        public TResult Wrap<TResult>(Func<TResult> tryFunc, Action<Exception> catchAction, Action finallyAction, String exceptionPolicyName, Rethrow rethrow, TResult returnValueAfterException = default(TResult))
        {
            if (UpdatePolicyAndRethrowFromUsage)
            {
                ExceptionPolicyName = exceptionPolicyName;
                Rethrow = rethrow;
            }
            try
            {
                return tryFunc();
            }
            catch (Exception exc)
            {
                catchAction(exc);
                if (String.IsNullOrEmpty(ExceptionPolicyName) || ExceptionPolicy.HandleException(exc, exceptionPolicyName))
                {
                    if (rethrow != Rethrow.No)
                        switch (rethrow)
                        {
                            case Rethrow.Throw: throw;
                            case Rethrow.ThrowNamed: throw exc;
                            case Rethrow.ThrowNew: throw new Exception("re-throw", exc);
                        }
                }
            }
            finally
            {
                finallyAction();
            }
            return returnValueAfterException;
        }

        #endregion try func and  catch & finally actions specified
        #endregion TRY as Func
    } // class
}
