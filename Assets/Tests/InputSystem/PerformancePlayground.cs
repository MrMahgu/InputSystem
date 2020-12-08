using System;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using NUnit.Framework;
using Unity.Collections;
using Unity.PerformanceTesting;
using UnityEngine;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.Users;
using UnityEngine.Profiling;

internal class PerformancePlayground : InputTestFixture
{
    [Test, Performance]
    [Category("Performance")]
    public void Dmytro_RND()
    {
        var keyboard = InputSystem.AddDevice<Keyboard>();

        //var actions = ScriptableObject.CreateInstance<InputActionAsset>();
        //var map = actions.AddActionMap("map");
        //var action = map.AddAction("action", binding: "<Keyboard>/space");

        //PressAndRelease(keyboard.spaceKey);

        for (var times = 0; times < 3; ++times)
        {
            Profiler.BeginSample($"PerfTest no{times}");

            {
                Profiler.BeginSample("QueueEvents1");
                InputSystem.QueueStateEvent(keyboard, new KeyboardState());
                InputSystem.Update();
                Profiler.EndSample();
            }

            {
                Profiler.BeginSample("QueueEvents2");
                foreach (var key in (Key[]) Enum.GetValues(typeof(Key)))
                {
                    if (key == Key.None || key == Key.IMESelected)
                        continue;
                    InputSystem.QueueStateEvent(keyboard, new KeyboardState(key));
                    InputSystem.QueueStateEvent(keyboard, new KeyboardState());
                }

                Profiler.EndSample();
            }

            {
                Profiler.BeginSample("Update");
                InputSystem.Update();
                Profiler.EndSample();
            }

            for (var i = 0; i < 10; ++i)
            {
                Profiler.BeginSample($"Query no{i}");
                int a = 0;
                foreach (var key in (Key[]) Enum.GetValues(typeof(Key)))
                {
                    if (key == Key.None || key == Key.IMESelected)
                        continue;
                    a += keyboard[key].wasReleasedThisFrame ? 1 : 0;
                }

                Profiler.EndSample();

                Assert.Zero(a);
            }

            Profiler.EndSample();
        }

        /*
        var keyboard = InputSystem.AddDevice<Keyboard>();

        Assert.That(keyboard.spaceKey.stateBlock.byteOffset, Is.Zero);
        Assert.That(keyboard.spaceKey.stateBlock.bitOffset, Is.LessThan(8));
        var allKeysInFirstByte = keyboard.allControls.Where(x => x.stateBlock.byteOffset == 0 && x.stateBlock.bitOffset < 8 && !x.synthetic).ToList();

        using (DeltaStateEvent.From(keyboard.spaceKey, out var eventPtr))
        {
            var result = eventPtr.EnumerateControls(InputControlExtensions.Enumerate.IncludeSyntheticControls).ToList();

            Assert.That(result, Has.Count.EqualTo(allKeysInFirstByte.Count));
            Assert.That(result, Has.Exactly(0).SameAs(keyboard.anyKey));
            Assert.That(result, Has.Exactly(1).SameAs(keyboard.spaceKey));
        }
        */
    }
}