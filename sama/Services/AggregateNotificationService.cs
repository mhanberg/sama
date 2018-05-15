using sama.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace sama.Services
{
    public class AggregateNotificationService
    {
        private readonly List<INotificationService> _notificationServices;

        public AggregateNotificationService(IEnumerable<INotificationService> notificationServices)
        {
            _notificationServices = notificationServices.ToList();
        }

        public virtual async Task NotifySingleResult(Endpoint endpoint, EndpointCheckResult result)
        {
            await Task.Run(() => _notificationServices
                .ForEach(ns => ns.NotifySingleResult(endpoint, result)));
        }

        public virtual async Task NotifyUp(Endpoint endpoint, DateTimeOffset? downAsOf)
        {
            await Task.Run(() => _notificationServices
                .ForEach(ns => ns.NotifyUp(endpoint, downAsOf)));
        }

        public virtual async Task NotifyDown(Endpoint endpoint, DateTimeOffset downAsOf, Exception reason)
        {
            await Task.Run(() => _notificationServices
                .ForEach(ns => ns.NotifyDown(endpoint, downAsOf, reason)));
        }

        public virtual async Task NotifyMisc(Endpoint endpoint, NotificationType type)
        {
            await Task.Run(() => _notificationServices
                .ForEach(ns => ns.NotifyMisc(endpoint, type)));
        }
    }
}
