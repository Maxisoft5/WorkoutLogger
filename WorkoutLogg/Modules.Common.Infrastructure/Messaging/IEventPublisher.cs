using System;
using System.Collections.Generic;
using System.Text;

namespace Modules.Common.Infrastructure.Messaging
{
    public interface IEventPublisher
    {
        Task PublishAsync<T>(string topic, T @event, CancellationToken ct = default) where T : class;
    }
}
