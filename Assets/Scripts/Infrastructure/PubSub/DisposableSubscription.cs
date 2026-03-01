using System;
using Unity.DynamicDuo.Infrastructure;
using UnityEngine;

namespace Unity.DynamicDuo.Infrastructure
{
    public class DisposableSubscription<T> : IDisposable
    {
        Action<T> m_handler;
        bool m_IsDisposed;
        IMessageChannel<T> m_MessageChannel;

        public DisposableSubscription(IMessageChannel<T> messageChannel, Action<T> handler)
        {
            m_MessageChannel = messageChannel;
            m_handler = handler;
        }

        public void Dispose()
        {
            if (!m_IsDisposed)
            {
                m_IsDisposed = true;

                if (!m_MessageChannel.IsDisposed)
                {
                    m_MessageChannel.Unsubscribe(m_handler);
                }

                m_handler = null;
                m_MessageChannel = null;
            }
        }
    }
}
