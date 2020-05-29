using System;
using System.Collections.Generic;
using NUnit.Framework;
using UniRx;
using Votyra.Core.Models;

namespace Votyra.Core
{
    [TestFixture]
    public class ScheduledSubjectTest
    {
        [Test]
        public void ScheduledSubject_CircullarConnection_CombinedValueIsCorrect()
        {
            var subjectA = new BehaviorSubject<string>(null).MakeScheduledOnMainThread();
            var subjectB = new BehaviorSubject<string>(null).MakeScheduledOnMainThread();
            var subjectC = new BehaviorSubject<string>(null).MakeScheduledOnMainThread();

            subjectA.Subscribe(valueA =>
            {
                if (valueA != null)
                {
                    subjectB.OnNext("B_" + valueA);
                }
                else
                {
                    subjectB.OnNext(null);
                }
            });
            subjectB.Subscribe(valueB =>
            {
                if (valueB != null)
                {
                    subjectC.OnNext("C_" + valueB);
                }
                else
                {
                    subjectC.OnNext(null);
                }
            });
            subjectC.Subscribe(valueC =>
            {
                if (valueC != null)
                {
                    subjectA.OnNext(null);
                }
            });

            var handlerCalls = new List<Tuple<string, string, string>>();
            subjectA.CombineLatest(subjectB,
                    subjectC,
                    (a, b, c) =>
                    {
                        return Tuple.Create(a, b, c);
                    })
                .Subscribe(call => handlerCalls.Add(call));

            subjectA.OnNext("A");

            var expectedResult = new List<Tuple<string, string, string>>
            {
                Tuple.Create<string, string, string>(null, null, null),
                Tuple.Create<string, string, string>("A", null, null),
                Tuple.Create<string, string, string>("A", "B_A", null),
                Tuple.Create("A", "B_A", "C_B_A"),
                Tuple.Create<string, string, string>(null, "B_A", "C_B_A"),
                Tuple.Create<string, string, string>(null, null, "C_B_A"),
                Tuple.Create<string, string, string>(null, null, null),
            };
            TestUtils.AssertListEquality(expectedResult, handlerCalls);
        }

        // [Test]
        // public void ScheduledSubject_OnNext_HandlersAreSerialized()
        // {
        //     TestUtils.UnityAsyncTest(async () =>
        //     {
        //         var handlerCalls = new List<Tuple<int, string, string>>();
        //
        //         var subject = new BehaviorSubject<string>(default(string)).MakeScheduledOnMainThread();
        //
        //         var mainThread = Thread.CurrentThread;
        //
        //         subject.Subscribe(x =>
        //         {
        //             handlerCalls.Add(Tuple.Create(0, x, subject));
        //             Assert.AreEqual(mainThread, Thread.CurrentThread);
        //
        //             if (x == "val1")
        //                 subject.OnNext("val2");
        //         });
        //
        //         subject.Subscribe(x =>
        //         {
        //             handlerCalls.Add(Tuple.Create(1, x, subject.Value));
        //             Assert.AreEqual(mainThread, Thread.CurrentThread);
        //         });
        //
        //         await Task.Delay(100);
        //
        //         subject.OnNext("val1");
        //
        //         for (var i = 0; i < 10; i++)
        //         {
        //             await Task.Delay(100);
        //         }
        //
        //         var expectedResult = new List<Tuple<int, string, string>>
        //         {
        //             Tuple.Create(0, null as string, null as string),
        //             Tuple.Create(1, null as string, null as string),
        //             Tuple.Create(0, "val1", "val1"),
        //             Tuple.Create(1, "val1", "val1"),
        //             Tuple.Create(0, "val2", "val2"),
        //             Tuple.Create(1, "val2", "val2")
        //         };
        //
        //         TestUtils.AssertListEquality(expectedResult, handlerCalls);
        //     });
        // }
    }
}
