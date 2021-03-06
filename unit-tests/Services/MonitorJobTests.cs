﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using sama;
using sama.Models;
using sama.Services;
using System;

namespace TestSama.Services
{
    [TestClass]
    public class MonitorJobTests
    {
        private MonitorJob _service;
        private IServiceProvider _provider;
        private IServiceScope _scope;
        private ApplicationDbContext _testDbContext;
        private EndpointProcessService _processService;

        [TestInitialize]
        public void Setup()
        {
            _provider = TestUtility.InitDI();
            _scope = _provider.CreateScope();
            _testDbContext = new ApplicationDbContext(_scope.ServiceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>());
            _processService = Substitute.For<EndpointProcessService>(null, null, null, null);
            _service = new MonitorJob(_provider, _processService);

            SeedEndpoints();
        }

        [TestMethod]
        public void ExecuteShouldProcessEachEnabledEndpoint()
        {
            _service.Execute();

            _processService.Received(2).ProcessEndpoint(Arg.Any<Endpoint>(), Arg.Any<int>());
            _processService.Received().ProcessEndpoint(Arg.Is<Endpoint>(e => e.Id == 2), 0);
            _processService.Received().ProcessEndpoint(Arg.Is<Endpoint>(e => e.Id == 3), 0);
        }

        private void SeedEndpoints()
        {
            _testDbContext.Endpoints.Add(TestUtility.CreateHttpEndpoint("A", false, 1, "B"));
            _testDbContext.Endpoints.Add(TestUtility.CreateHttpEndpoint("C", true, 2, "D"));
            _testDbContext.Endpoints.Add(TestUtility.CreateHttpEndpoint("D", true, 3, "E"));
            _testDbContext.Endpoints.Add(TestUtility.CreateHttpEndpoint("F", false, 4, "G"));
            _testDbContext.SaveChanges();
        }
    }
}
