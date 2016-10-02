﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using DeviceManagement.Infrustructure.Connectivity.Builders;
using DeviceManagement.Infrustructure.Connectivity.EricssonApiService;
using DeviceManagement.Infrustructure.Connectivity.EricssonSubscriptionService;
using DeviceManagement.Infrustructure.Connectivity.Models.Other;
using DeviceManagement.Infrustructure.Connectivity.Models.Security;
using DeviceManagement.Infrustructure.Connectivity.Models.TerminalDevice;

namespace DeviceManagement.Infrustructure.Connectivity.Clients
{
    public class EricssonCellularClient
    {
        private readonly ICredentialProvider _credentialProvider;
        public EricssonCellularClient(ICredentialProvider credentialProvider)
        {
            _credentialProvider = credentialProvider;
        }


        public bool ValidateCredentials()
        {
           
            var isValid = false;          

            //simple check - if it throws an exception then the creds are no good
            //todo: catch the correct error code
            try
            {
                var apiStatusClient = EricssonServiceBuilder.GetApiStatusClient(_credentialProvider.Provide());
                apiStatusClient.echo(new echo());
                isValid = true;
            }
            catch(Exception ex)
            {
                isValid = false;
            }    
            return isValid;
        }

        public List<Iccid> GetTerminals()
        {
            //todo : Stubbed out with real ICCIDs from test account until ericsson sort an endpoint to do what we need
            return new List<Iccid>()
            {
                new Iccid("9883011539830007560"),
                new Iccid("89883011539830007560"),
                new Iccid("89883011539830007586"),
                new Iccid("89883011539830007594"),
                new Iccid("89883011539830007602"),
            };
        }

        public Terminal GetSingleTerminalDetails(Iccid iccid)
        {
            var terminal = new EricssonTerminal();
            try
            {
                var subManClient = EricssonServiceBuilder.GetSubscriptionManagementClient(_credentialProvider.Provide());
                var response = subManClient.QuerySimResource_v2(new QuerySimResource_v2() { resource = new resource() { id = iccid.Id , type = "icc"} });

                //check it even exists
                if(response.simResource.Length <=0) return terminal;
                var sim = response.simResource.First();

                terminal.Status = sim.simSubscriptionStatus.ToString();
                terminal.DateOfActivation = sim.firstActivationDate; //todo: check this is correct
                terminal.Iccid = new Iccid(sim.icc);
                terminal.Imei = new Imei(sim.imei);
                terminal.Imsi = new Imsi(sim.imsi);
                terminal.Msisdn = new Msisdn(sim.msisdn); //todo: needs a view 
                terminal.RatePlan = sim.productOfferName;
                terminal.PriceProfileName = sim.priceProfileName;
                terminal.PdpContextProfileName = sim.pdpContextProfileName;
                terminal.AccountId = Convert.ToInt32(sim.customerNo); //todo : this will be customer number on the view

            }
            catch (Exception exception)
            {
                return terminal;
            }
            return terminal;
        }

        public List<SessionInfo> GetSingleSessionInfo(Iccid iccid)
        {
            return new List<SessionInfo>();
        }

        public SimState GetCurrentSimState(string iccid)
        {
            return GetAvailableSimStates(iccid).FirstOrDefault(s => s.Name == "Active");
        }

        public List<SimState> GetAvailableSimStates(string iccid)
        {
            return new List<SimState>()
            {
                new SimState()
                {
                    Id = "1",
                    Name = "Active"
                },
                new SimState()
                {
                    Id = "2",
                    Name = "Disconnected"
                }
            };
        }

        /// <summary>
        /// Gets the current subscription package for a terminal
        /// </summary>
        /// <param name="iccid">The iccid of the terminal</param>
        /// <returns>The subscription package object</returns>
        public SubscriptionPackage GetCurrentSubscriptionPackage(string iccid)
        {
            return GetAvailableSubscriptionPackages(iccid).FirstOrDefault(s => s.Name == "Basic");
        }

        /// <summary>
        /// Gets the available subscription packages from the appropriate api provider
        /// </summary>
        /// <returns>SubscriptionPackage Model</returns>
        public List<SubscriptionPackage> GetAvailableSubscriptionPackages(string iccid)
        {
            return new List<SubscriptionPackage>()
            {
                new SubscriptionPackage()
                {
                    Id = "1",
                    Name = "Basic"
                },
                new SubscriptionPackage()
                {
                    Id = "2",
                    Name = "Expensive"
                }
            };
        }

        public bool UpdateSimState(string iccid, string updatedState)
        {
            return true;
        }

        public bool UpdateSubscriptionPackage(string iccid, string updatedPackage)
        {
            return true;
        }

        public bool ReconnectTerminal(string iccid)
        {
            return true;
        }

        public bool SendSms(string iccid, string smsText)
        {
            return true;
        }

    }
}
