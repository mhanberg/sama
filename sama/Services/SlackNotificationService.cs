﻿using Humanizer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using sama.Models;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace sama.Services
{
    public class SlackNotificationService : INotificationService
    {
        private readonly ILogger<SlackNotificationService> _logger;
        private readonly SettingsService _settings;
        private readonly IServiceProvider _serviceProvider;

        public SlackNotificationService(ILogger<SlackNotificationService> logger, SettingsService settings, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _settings = settings;
            _serviceProvider = serviceProvider;
        }

        public async Task NotifyMisc(Endpoint endpoint, NotificationType type)
        {
            switch (type)
            {
                case NotificationType.EndpointAdded:
                    await SendNotification($"The endpoint '{endpoint.Name}' has been added.");
                    break;
                case NotificationType.EndpointRemoved:
                    await SendNotification($"The endpoint '{endpoint.Name}' has been removed.");
                    break;
                case NotificationType.EndpointEnabled:
                    await SendNotification($"The endpoint '{endpoint.Name}' has been enabled.");
                    break;
                case NotificationType.EndpointDisabled:
                    await SendNotification($"The endpoint '{endpoint.Name}' has been disabled.");
                    break;
                case NotificationType.EndpointReconfigured:
                    await SendNotification($"The endpoint '{endpoint.Name}' has been reconfigured.");
                    break;
                default:
                    return;
            }
        }

        public Task NotifySingleResult(Endpoint endpoint, EndpointCheckResult result)
        {
            // Ignore this notification type.
            return Task.CompletedTask;
        }

        public async Task NotifyDown(Endpoint endpoint, DateTimeOffset downAsOf, Exception reason)
        {
            var failureMessage = reason?.Message;

            if (!string.IsNullOrWhiteSpace(failureMessage))
            {
                var msg = failureMessage.Trim();
                if (!msg.EndsWith('.') && !msg.EndsWith('!') && !msg.EndsWith('?'))
                    failureMessage = failureMessage.Trim() + '.';

                if (reason is SslException sslEx){
                    failureMessage += "\n Details: ```\n" + sslEx.Details + "```";
                }
            }

            await SendNotification($"The endpoint '{endpoint.Name}' is down: {failureMessage}");
        }

        public async Task NotifyUp(Endpoint endpoint, DateTimeOffset? downAsOf)
        {
            if (downAsOf.HasValue)
            {
                var downLength = DateTimeOffset.UtcNow - downAsOf.Value;
                await SendNotification($"The endpoint '{endpoint.Name}' is up after being down for {downLength.Humanize()}. Hooray!");
            }
            else
            {
                await SendNotification($"The endpoint '{endpoint.Name}' is up. Hooray!");
            }
        }

        private async Task SendNotification(string message)
        {
            var url = _settings.Notifications_Slack_WebHook;
            if (string.IsNullOrWhiteSpace(url)) return;

            try
            {
                using (var httpHandler = _serviceProvider.GetRequiredService<HttpClientHandler>())
                using (var client = new HttpClient(httpHandler, false))
                {
                    var data = JsonConvert.SerializeObject(new { text = message });
                    await client.PostAsync(url, new StringContent(data));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(0, "Unable to send Slack notification", ex);
            }
        }
    }
}
