﻿using MahApps.Metro.Controls.Dialogs;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using VVSAssistant.Database;
using VVSAssistant.Functions.Calculation;
using VVSAssistant.Models;
using VVSAssistant.Models.DataSheets;
using VVSAssistant.ViewModels;

namespace VVSAssistant.Tests.ViewModelTests
{
    [TestFixture]
    class CreateOfferViewModelTest
    {
        private CreateOfferViewModel testModel;
        private PackagedSolution testPack;
        private Client testClient;
        private Appliance testApp1;
        private Appliance testApp2;
        private Offer off;
        private AssistantContext ctx;

        [SetUp]
        public void Setup()
        {
            ctx = new AssistantContext();
            testModel = new CreateOfferViewModel(new DialogCoordinator());

            testApp1 = new Appliance() { Name = "app1", CreationDate = DateTime.Now, Type = ApplianceTypes.Boiler};
            testApp1.DataSheet = new HeatingUnitDataSheet();

            testApp2 = new Appliance() { Name = "app2", CreationDate = DateTime.Now, Type = ApplianceTypes.Container};
            testApp2.DataSheet = new ContainerDataSheet();

            testPack = new PackagedSolution() { Name = "testPack"};
            testPack.Appliances.Add(testApp1); testPack.Appliances.Add(testApp2);
            testPack.PrimaryHeatingUnit = testApp1;
            testPack.CreationDate = DateTime.Now;
            testPack.EnergyLabel = new List<EEICalculationResult>();

            testClient = new Client();
            testClient.ClientInformation = new ClientInformation();
            testClient.CreationDate = DateTime.Now;

            off = new Offer();
            off.CreationDate = DateTime.Now;
            off.PackagedSolution = testPack;
            off.Client = testClient;
        }

        [Test]
        public void InitialDBTest()
        {
            ctx.Offers.Add(off);
            ctx.SaveChanges();
            Assert.IsTrue(ctx.Offers.Any(o => o.Id == off.Id));
        }

        [Test]
        public void LoadExistingOfferTest()
        {
            ctx.Offers.Add(off);
            ctx.SaveChanges();

            testModel.LoadExistingOffer(off.Id);
        }

        public bool DoesContentsMatch(IEnumerable<object> first, IEnumerable<object> second)
        {
            foreach (var item in first)
            {
                if (!second.Contains(item))
                    return false;
            }
            return true;
        }
            
        /* Mr. Gorbachev, */ [TearDown] /* this wall*/
        public void TearDown()
        {
            testModel = null;
        }
    }
}
