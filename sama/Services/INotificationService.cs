using sama.Models;
using System;
using System.Threading.Tasks;

namespace sama.Services
{
    public enum NotificationType
    {
        EndpointAdded,
        EndpointRemoved,
        EndpointEnabled,
        EndpointDisabled,
        EndpointReconfigured,
    }

    public interface INotificationService
    {
        Task NotifySingleResult(Endpoint endpoint, EndpointCheckResult result);

        Task NotifyUp(Endpoint endpoint, DateTimeOffset? downAsOf);

        Task NotifyDown(Endpoint endpoint, DateTimeOffset downAsOf, Exception reason);

        Task NotifyMisc(Endpoint endpoint, NotificationType type);
    }
}
