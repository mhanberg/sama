﻿using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using sama.Models;

namespace sama.Services
{
    public class GraphiteNotificationService : INotificationService
    {
        private readonly ILogger<GraphiteNotificationService> _logger;
        private readonly SettingsService _settings;
        private readonly TcpClientWrapper _tcpWrapper;

        public GraphiteNotificationService(ILogger<GraphiteNotificationService> logger, SettingsService settings, TcpClientWrapper tcpWrapper)
        {
            _logger = logger;
            _settings = settings;
            _tcpWrapper = tcpWrapper;
        }

        public Task NotifyDown(Endpoint endpoint, DateTimeOffset downAsOf, Exception reason)
        {
            // Ignore.
            return Task.CompletedTask;
        }

        public Task NotifyMisc(Endpoint endpoint, NotificationType type)
        {
            // Ignore.
            return Task.CompletedTask;
        }

        public async Task NotifySingleResult(Endpoint endpoint, EndpointCheckResult result)
        {
            var host = _settings.Notifications_Graphite_Host;
            var port = _settings.Notifications_Graphite_Port;
            if (string.IsNullOrWhiteSpace(host) || port < 1) return;

            var filteredEndpointName = Regex.Replace(endpoint.Name, @"[^a-zA-Z0-9-]+", "-").Trim('-');
            if (string.IsNullOrWhiteSpace(filteredEndpointName)) filteredEndpointName = "none";
            var epoch = result.Start.ToUnixTimeSeconds();
            var successMetric = (result.Success ? 1 : 0);

            var graphiteMessage = new StringBuilder();
            graphiteMessage.Append($"sama.{filteredEndpointName}.response.success {successMetric} {epoch}\n");

            if (result.Success && result.ResponseTime.HasValue)
            {
                var responseTime = (int)Math.Round(result.ResponseTime.Value.TotalMilliseconds);
                graphiteMessage.Append($"sama.{filteredEndpointName}.response.timeMsec {responseTime} {epoch}\n");
            }

            var messageBytes = Encoding.ASCII.GetBytes(graphiteMessage.ToString());

            try
            {
                await _tcpWrapper.SendData(host, port, messageBytes);
            }
            catch (Exception ex)
            {
                _logger.LogError(0, "Unable to send Graphite notification", ex);
            }
        }

        public Task NotifyUp(Endpoint endpoint, DateTimeOffset? downAsOf)
        {
            // Ignore.
            return Task.CompletedTask;
        }
    }
}
