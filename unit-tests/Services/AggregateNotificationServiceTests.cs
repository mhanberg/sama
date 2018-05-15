using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using sama.Models;
using sama.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TestSama.Services
{
    [TestClass]
    public class AggregateNotificationServiceTests
    {
        private INotificationService _notifier1;
        private INotificationService _notifier2;
        private AggregateNotificationService _service;

        [TestInitialize]
        public void Setup()
        {
            _notifier1 = Substitute.For<INotificationService>();
            _notifier2 = Substitute.For<INotificationService>();
            _service = new AggregateNotificationService(new List<INotificationService> { _notifier1, _notifier2 });
        }

        [TestMethod]
        public async Task ShouldNotifySingleResult()
        {
            var ep = new Endpoint();
            var ecr = new EndpointCheckResult();

            await _service.NotifySingleResult(ep, ecr);

            await _notifier1.Received().NotifySingleResult(ep, ecr);
            await _notifier2.Received().NotifySingleResult(ep, ecr);
        }

        [TestMethod]
        public async Task ShouldNotifyUp()
        {
            var ep = new Endpoint();
            var dao = DateTimeOffset.Now;

            await _service.NotifyUp(ep, dao);

            await _notifier1.Received().NotifyUp(ep, dao);
            await _notifier2.Received().NotifyUp(ep, dao);
        }

        [TestMethod]
        public async Task ShouldNotifyDown()
        {
            var ep = new Endpoint();
            var dao = DateTimeOffset.Now;
            var reason = new Exception();

            await _service.NotifyDown(ep, dao, reason);

            await _notifier1.Received().NotifyDown(ep, dao, reason);
            await _notifier2.Received().NotifyDown(ep, dao, reason);
        }

        [TestMethod]
        public async Task ShouldNotifyMisc()
        {
            var ep = new Endpoint();
            var type = NotificationType.EndpointReconfigured;

            await _service.NotifyMisc(ep, type);

            await _notifier1.Received().NotifyMisc(ep, type);
            await _notifier2.Received().NotifyMisc(ep, type);
        }
    }
}
