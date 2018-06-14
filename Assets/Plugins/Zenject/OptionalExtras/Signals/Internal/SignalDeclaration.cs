using System;
using System.Collections.Generic;
using ModestTree;
using System.Linq;

#if ZEN_SIGNALS_ADD_UNIRX
using UniRx;
#endif

namespace Zenject
{
    public class SignalDeclaration : IDisposable, IPoolable<Type, SignalMissingHandlerResponses, bool, ZenjectSettings.SignalSettings>
    {
        public static readonly PoolableStaticMemoryPool<Type, SignalMissingHandlerResponses, bool, ZenjectSettings.SignalSettings, SignalDeclaration> Pool =
            new PoolableStaticMemoryPool<Type, SignalMissingHandlerResponses, bool, ZenjectSettings.SignalSettings, SignalDeclaration>();

        readonly List<SignalSubscription> _subscriptions;
        readonly List<object> _signalQueue;

#if ZEN_SIGNALS_ADD_UNIRX
        Subject<object> _stream;
#endif
        Type _signalType;
        SignalMissingHandlerResponses _missingHandlerResponses;
        bool _runAsync;
        ZenjectSettings.SignalSettings _settings;

        public SignalDeclaration()
        {
            _subscriptions = new List<SignalSubscription>();
            _signalQueue = new List<object>();

            SetDefaults();
        }

#if ZEN_SIGNALS_ADD_UNIRX
        public IObservable<object> Stream
        {
            get { return _stream; }
        }
#endif

        public Type SignalType
        {
            get { return _signalType; }
        }

        public void Dispose()
        {
            Pool.Despawn(this);
        }

        void SetDefaults()
        {
#if ZEN_SIGNALS_ADD_UNIRX
            // We could re-use this but let's just create a new one to be extra safe
            // in case some objects are still subscribed to the old one
            _stream = new Subject<object>();
#endif
            _missingHandlerResponses = SignalMissingHandlerResponses.Ignore;
            _runAsync = false;
            _settings = null;
            _signalType = null;
            _subscriptions.Clear();
            _signalQueue.Clear();
        }

        public void OnDespawned()
        {
            if (_settings.RequireStrictUnsubscribe)
            {
                Assert.That(_subscriptions.IsEmpty(),
                    "Found {0} signals still added to declaration {1}", _subscriptions.Count, _signalType);
            }
            else
            {
                // We can't rely entirely on the destruction order in Unity because of
                // the fact that OnDestroy is completely unpredictable.
                // So if you have a GameObjectContext at the root level in your scene, then it
                // might be destroyed AFTER the SceneContext.  So if you have some signal declarations
                // in the scene context, they might get disposed before some of the subscriptions
                // so in this case you need to disconnect from the subscription so that it doesn't
                // try to remove itself after the declaration has been destroyed, which could be
                // especially problematic if the declaration is re-spawned for a different purpose
                for (int i = 0; i < _subscriptions.Count; i++)
                {
                    _subscriptions[i].OnDeclarationDespawned();
                }
            }

            SetDefaults();
        }

        public void OnSpawned(
            Type signalType, SignalMissingHandlerResponses missingHandlerResponses,
            bool runAsync, ZenjectSettings.SignalSettings settings)
        {
            Assert.IsNull(_signalType);
            Assert.That(_subscriptions.IsEmpty());

            _settings = settings;
            _signalType = signalType;
            _missingHandlerResponses = missingHandlerResponses;
            _runAsync = runAsync;
        }

        public void Fire(object signal)
        {
            Assert.That(signal.GetType().DerivesFromOrEqual(_signalType));

            if (_runAsync)
            {
                _signalQueue.Add(signal);
            }
            else
            {
                // Cache the callback list to allow handlers to be added from within callbacks
                using (var block = DisposeBlock.Spawn())
                {
                    var subscriptions = block.SpawnList<SignalSubscription>();
                    subscriptions.AddRange(_subscriptions);
                    FireInternal(subscriptions, signal);
                }
            }
        }

        void FireInternal(List<SignalSubscription> subscriptions, object signal)
        {
            if (subscriptions.IsEmpty()
#if ZEN_SIGNALS_ADD_UNIRX
                && !_stream.HasObservers
#endif
                )
            {
                if (_missingHandlerResponses == SignalMissingHandlerResponses.Warn)
                {
                    Log.Warn("Fired signal '{0}' but no subscriptions found!  If this is intentional then either add IgnoreMissingHandler to the binding or change the default in ZenjectSettings", signal.GetType());
                }
                else if (_missingHandlerResponses == SignalMissingHandlerResponses.Throw)
                {
                    throw Assert.CreateException(
                        "Fired signal '{0}' but no subscriptions found!  If this is intentional then either add IgnoreMissingHandler to the binding or change the default in ZenjectSettings", signal.GetType());
                }
            }

            for (int i = 0; i < subscriptions.Count; i++)
            {
                var subscription = subscriptions[i];

                // This is a weird check for the very rare case where an Unsubscribe is called
                // from within the same callback (see TestSignalsAdvanced.TestSubscribeUnsubscribeInsideHandler)
                if (_subscriptions.Contains(subscription))
                {
                    subscription.Invoke(signal);
                }
            }

#if ZEN_SIGNALS_ADD_UNIRX
            _stream.OnNext(signal);
#endif
        }

        public void Update()
        {
            if (!_signalQueue.IsEmpty())
            {
                // Cache the callback list to allow handlers to be added from within callbacks
                using (var block = DisposeBlock.Spawn())
                {
                    var subscriptions = block.SpawnList<SignalSubscription>();
                    subscriptions.AddRange(_subscriptions);

                    // Cache the signals so that if the signal is fired again inside the handler that it
                    // is not executed until next frame
                    var signals = block.SpawnList<object>();
                    signals.AddRange(_signalQueue);

                    _signalQueue.Clear();

                    for (int i = 0; i < signals.Count; i++)
                    {
                        FireInternal(subscriptions, signals[i]);
                    }
                }
            }
        }

        public void Add(SignalSubscription subscription)
        {
            Assert.That(!_subscriptions.Contains(subscription));
            _subscriptions.Add(subscription);
        }

        public void Remove(SignalSubscription subscription)
        {
            _subscriptions.RemoveWithConfirm(subscription);
        }
    }
}
