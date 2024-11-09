using System;
using System.Collections.Generic;

namespace Bones.UI.Presenters
{
    public struct ExceptionsHandler
    {
        private object _exceptions;

        public void Add(Exception exception)
        {
            switch (_exceptions)
            {
                case null:
                    _exceptions = exception;
                    return;
                case Exception existingException:
                    _exceptions = new List<Exception> { existingException, exception };
                    return;
                case List<Exception> exceptionsList:
                    exceptionsList.Add(exception);
                    return;
            }
        }

        public void TryThrow()
        {
            switch (_exceptions)
            {
                case null:
                    return;
                case Exception exception:
                    throw exception;
                case List<Exception> exceptionsList:
                    throw new AggregateException(exceptionsList);
            }
        }
    }
}