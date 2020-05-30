using UniRx;

namespace Votyra.Core.Models
{
    public static class ScheduledSubjectUtils
    {
        public static ISubject<T> MakeScheduledOnMainThread<T>(this ISubject<T> subject) => new ScheduledSubject<T>(subject);
    }
}
