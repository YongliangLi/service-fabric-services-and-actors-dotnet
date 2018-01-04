﻿// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Runtime
{
    using System;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;

    internal class ActorTimer : IActorTimer
    {
        private readonly ActorBase owner;
        private readonly object callbackState;
        private readonly ActorMethodContext callbackMethodContext;

        private Timer timer;
        private Func<object, Task> asyncCallback;

        public ActorTimer(
            ActorBase owner,
            Func<object, Task> asyncCallback,
            object state,
            TimeSpan dueTime,
            TimeSpan period)
        {
            this.owner = owner;
            this.asyncCallback = asyncCallback;
            this.callbackMethodContext = ActorMethodContext.CreateForTimer(this.asyncCallback.GetMethodInfo().Name);
            this.callbackState = state;
            this.Period = period;
            this.DueTime = dueTime;
            this.timer = new Timer(this.OnTimerCallback);

            this.ArmTimer(dueTime);
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        public TimeSpan DueTime { get; }

        public TimeSpan Period { get; }

        private void OnTimerCallback(object state)
        {
            Task.Factory.StartNew(this.FireTimerAsync);
        }

        private async Task FireTimerAsync()
        {
            var reschedule = true;

            try
            {
                await this.owner.Manager.DispatchToActorAsync(
                    this.owner.Id,
                    this.callbackMethodContext,
                    false,
                    this.DispatchTimerCallback,
                    Guid.NewGuid().ToString(),
                    true,
                    CancellationToken.None);
            }
            catch (ObjectDisposedException)
            {
                // the actor is disposed, do not reschedule the timer
                reschedule = false;
            }
            catch
            {
                // do nothing
            }

            if (reschedule)
            {
                this.ArmTimer(this.Period);
            }
            else
            {
                this.CancelTimer();
            }
        }

        private void CancelTimer()
        {
            if (this.timer != null)
            {
                this.timer.Dispose();
                this.timer = null;
                this.asyncCallback = null;
            }
        }

        private async Task<byte[]> DispatchTimerCallback(ActorBase actor, CancellationToken cancellationToken)
        {
            if (!ReferenceEquals(actor, this.owner))
            {
                throw new ObjectDisposedException("actor");
            }

            Func<object, Task> callback = this.asyncCallback;
            if (callback != null)
            {
                await callback(this.callbackState);
            }

            return null;
        }

        private void ArmTimer(TimeSpan timeSpan)
        {
            Timer t = this.timer;
            if (t != null)
            {
                try
                {
                    t.Change(timeSpan, Timeout.InfiniteTimeSpan);
                }
                catch
                {
                    // ignored
                }
            }
        }

        private void Dispose(bool disposing)
        {
            if (!disposing)
            {
                return;
            }

            this.CancelTimer();
        }

        ~ActorTimer()
        {
            this.Dispose(false);
        }
    }
}