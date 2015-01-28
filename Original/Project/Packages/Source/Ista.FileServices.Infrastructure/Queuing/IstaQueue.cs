using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Ista.FileServices.Infrastructure.Interfaces;

namespace Ista.FileServices.Infrastructure.Queuing
{
    public class IstaQueue<TMessage>
        where TMessage : class, IIstaMessage
    {
        private readonly Queue<TMessage> internalQueue;
        private readonly ManualResetEventSlim manualReset;
        private bool open;

        public IstaQueue()
        {
            internalQueue = new Queue<TMessage>();
            manualReset = new ManualResetEventSlim();
            open = true;
        }

        public void Close()
        {
            lock (internalQueue)
            {
                open = false;
                manualReset.Set();
            }
        }

        public TMessage Dequeue()
        {
            return Dequeue(CancellationToken.None);
        }

        public TMessage Dequeue(CancellationToken token)
        {
            while (true)
            {
                if (token.IsCancellationRequested)
                    break;

                EnsureState();

                lock (internalQueue)
                {
                    if (internalQueue.Count != 0)
                        return internalQueue.Dequeue();
                }

                try
                {
                    if (manualReset.IsSet)
                        manualReset.Reset();

                    manualReset.Wait(token);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
            }

            return default(TMessage);
        }

        public TMessage Dequeue(TMessage defaultMessage)
        {
            lock (internalQueue)
            {
                if (internalQueue.Count != 0)
                    return internalQueue.Dequeue();
            }

            EnsureState();
            return defaultMessage;
        }

        public void Enqueue(TMessage item)
        {
            lock (internalQueue)
            {
                EnsureState();

                internalQueue.Enqueue(item);
                manualReset.Set();
            }
        }

        public bool TryDequeue(int timeout, out TMessage item)
        {
            return TryDequeue(timeout, CancellationToken.None, out item);
        }

        public bool TryDequeue(int timeout, CancellationToken token, out TMessage item)
        {
            if (timeout < 0)
                throw new ArgumentOutOfRangeException("timeout", "Timeout must be greater than zero");

            return TryDequeue(TimeSpan.FromMilliseconds(timeout), token, out item);
        }

        public bool TryDequeue(TimeSpan timespan, out TMessage item)
        {
            return TryDequeue(timespan, CancellationToken.None, out item);
        }

        public bool TryDequeue(TimeSpan timespan, CancellationToken token, out TMessage item)
        {
            if (timespan == TimeSpan.MinValue || timespan == TimeSpan.MaxValue || timespan == TimeSpan.Zero)
            {
                item = Dequeue();
                return true;
            }

            while (true)
            {
                if (token.IsCancellationRequested)
                    break;

                EnsureState();

                lock (internalQueue)
                {
                    if (internalQueue.Count != 0)
                    {
                        item = internalQueue.Dequeue();
                        return true;
                    }
                }

                if (manualReset.IsSet)
                    manualReset.Reset();

                try
                {
                    var eventWasSet = manualReset.Wait(timespan, token);
                    if (!eventWasSet)
                        break;
                }
                catch (OperationCanceledException)
                {
                    break;
                }
            }

            item = default(TMessage);
            return false;
        }

        private void EnsureState()
        {
            if (!open)
                throw new EndOfStreamException();
        }
    }
}