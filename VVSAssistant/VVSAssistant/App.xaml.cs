﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using VVSAssistant.Models;

namespace VVSAssistant
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Test database stuff
            using (var db = new AssistantModelContainer())
            {
                // add a new client 'Iaro'
                var clientInformation = new ClientInformation() { Name="Iaroslav", Address="Kvadratet", Email="iaro@russia.ru", PhoneNumber="88888888"};
                db.ClientInformation.Add(clientInformation);
                var client = new Client { CreationDate = DateTime.Now, ClientInformation = clientInformation };
                db.Clients.Add(client);
                db.SaveChanges();
                // offer example
                var offer = new Offer();
                offer.Client = client;
                offer.CreationDate = DateTime.Now;
                var offerInformation = new OfferInformation()
                {
                    Description = "Example description",
                    Price = 500
                };
                db.OfferInformation.Add(offerInformation);
                offer.OfferInformation = offerInformation;
                var packagedSolution = new PackagedSolution()
                {
                    CreationDate = DateTime.Now,
                    Name = "Example Solution"
                };
                db.PackagedSolutions.Add(packagedSolution);
                var appliance = new Appliance()
                {
                    CreationDate = DateTime.Now,
                    Name = "Example Appliance",
                    Type = ApplianceTypes.Boiler
                };
                packagedSolution.Appliances.Add(appliance);
                offer.PackagedSolution = packagedSolution;
                db.Offers.Add(offer);
                db.SaveChanges();
            }

        }
    }
}
