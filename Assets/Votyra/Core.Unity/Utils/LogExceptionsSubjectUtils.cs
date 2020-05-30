using System;
using UniRx;
using Votyra.Core.Logging;

namespace Votyra.Core.Models
{
    public static class LogExceptionsSubjectUtils
    {
        public static ISubject<T> MakeLogExceptions<T>(this ISubject<T> subject, IThreadSafeLogger logger) => new LogExceptionsSubject<T>(subject, logger);

        public static IObservable<T> MakeLogExceptions<T>(this IObservable<T> observable, IThreadSafeLogger logger) => new LogExceptionsSubject<T>(observable, logger);
    }
}