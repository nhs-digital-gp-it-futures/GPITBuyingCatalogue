using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Transactions;
using BuyingCatalogueFunction.EpicsAndCapabilities.Interfaces;
using BuyingCatalogueFunction.EpicsAndCapabilities.Models;
using ICSharpCode.SharpZipLib.Zip;
using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask;
using Microsoft.DurableTask.Client;
using Microsoft.Extensions.Logging;

namespace BuyingCatalogueFunction.EpicsAndCapabilities
{
    public class CapabilitiesAndEpicsFunction
    {
        private const string CapabilitiesCSV = "Capabilities.csv";
        private const string StandardsCSV = "Standards.csv";
        private const string EpicsCSV = "Epics.csv";
        private const string RelationshipsCSV = "Relationships.csv";
        private readonly ILogger logger;
        private readonly ICapabilityService capabilityService;
        private readonly IEpicService epicService;
        private readonly IStandardService standardService;
        private readonly IStandardCapabilityService standardCapabilityService;

        public CapabilitiesAndEpicsFunction(
            ILogger<CapabilitiesAndEpicsFunction> logger,
            ICapabilityService capabilityService,
            IEpicService epicService,
            IStandardService standardService,
            IStandardCapabilityService standardCapabilityService)
        {
            this.logger = logger;
            this.capabilityService = capabilityService;
            this.epicService = epicService;
            this.standardService = standardService;
            this.standardCapabilityService = standardCapabilityService;
        }

        [Function(nameof(StartCapabilitiesUpdateBlobTrigger))]
        public async Task StartCapabilitiesUpdateBlobTrigger(
            [BlobTrigger("capabilities-update/{name}")] byte[] data,
            [DurableClient] DurableTaskClient starter,
            string name)
        {
            try
            {
                var instanceId = await starter.ScheduleNewOrchestrationInstanceAsync(nameof(CapabilitiesDataImporter), data);
                logger.LogInformation("{FunctionName} started orchestration {OrchestrationName} {ID} ",
                    nameof(StartCapabilitiesUpdateBlobTrigger),
                    nameof(CapabilitiesDataImporter),
                    instanceId);
            }
            catch (Exception e)
            {
                logger.LogError(
                    e,
                    "Error in {FunctionName} trying to start orchestration {OrchestrationName}",
                    nameof(StartCapabilitiesUpdateBlobTrigger),
                    nameof(CapabilitiesDataImporter));
                throw;
            }
        }

        [Function(nameof(CapabilitiesDataImporter))]
        public static async Task<List<string>> CapabilitiesDataImporter([OrchestrationTrigger] TaskOrchestrationContext context, byte[] input)
        {
            var capabilties = context.CallActivityAsync<List<CapabilityCsv>>(nameof(Capabilities), input);
            var standards = context.CallActivityAsync<List<StandardCsv>>(nameof(Standards), input);
            var epics = context.CallActivityAsync<List<EpicCsv>>(nameof(Epics), input);
            var standardCapabilities = context.CallActivityAsync<List<StandardCapabilityCsv>>(nameof(StandardCapabilities), input);
            await Task.WhenAll(capabilties, standards, epics, standardCapabilities);
            return await context.CallActivityAsync<List<string>>(nameof(Update), new ImportModel(capabilties.Result, standards.Result, epics.Result, standardCapabilities.Result));
        }

        [Function(nameof(Update))]
        public async Task<List<string>> Update([ActivityTrigger] ImportModel import)
        {
            try
            {
                using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
                var logs = new List<string>();
                logs.AddRange(await standardService.Process(import.Standards));
                logs.AddRange(await capabilityService.Process(import.Capabilities));
                logs.AddRange(await standardCapabilityService.Process(import.StandardCapabilities));
                logs.AddRange(await epicService.Process(import.Epics));

                scope.Complete();
                return logs;
            }
            catch (Exception e)
            {
                logger.LogError(
                    e,
                    "Error in {FunctionName} trying to process and save changes",
                    nameof(Update));
                throw;
            }
        }

        [Function(nameof(Capabilities))]
        public async Task<List<CapabilityCsv>> Capabilities([ActivityTrigger] byte[] input)
        {
            try
            {
                using var zipFile = new ZipFile(new MemoryStream(input));
                await using var capabilitiesStream = GetStream(zipFile, CapabilitiesCSV);
                return await capabilityService.Read(capabilitiesStream);
            }
            catch (Exception e)
            {
                logger.LogError(
                    e,
                    "Error in {FunctionName} trying to read capabilities",
                    nameof(Capabilities));
                throw;
            }
        }

        [Function(nameof(Standards))]
        public async Task<List<StandardCsv>> Standards([ActivityTrigger] byte[] input)
        {
            try
            {
                using var zipFile = new ZipFile(new MemoryStream(input));
                await using var standardsStream = GetStream(zipFile, StandardsCSV);
                return await standardService.Read(standardsStream);
            }
            catch (Exception e)
            {
                logger.LogError(
                    e,
                    "Error in {FunctionName} trying to read standards",
                    nameof(Standards));
                throw;
            }
        }

        [Function(nameof(Epics))]
        public async Task<List<EpicCsv>> Epics([ActivityTrigger] byte[] input)
        {
            try
            {
                using var zipFile = new ZipFile(new MemoryStream(input));
                await using var epicsStream = GetStream(zipFile, EpicsCSV);
                return await epicService.Read(epicsStream);
            }
            catch (Exception e)
            {
                logger.LogError(
                    e,
                    "Error in {FunctionName} trying to read epics",
                    nameof(Epics));
                throw;
            }
        }

        [Function(nameof(StandardCapabilities))]
        public async Task<List<StandardCapabilityCsv>> StandardCapabilities([ActivityTrigger] byte[] input)
        {
            try
            {
                using var zipFile = new ZipFile(new MemoryStream(input));
                await using var RelationshipsStream = GetStream(zipFile, RelationshipsCSV);
                return await standardCapabilityService.Read(RelationshipsStream);
            }
            catch (Exception e)
            {
                logger.LogError(
                    e,
                    "Error in {FunctionName} trying to read standard capabilities",
                    nameof(StandardCapabilities));
                throw;
            }
        }

        private static Stream GetStream(ZipFile zipFile, string dataset)
        {
            var zipEntry = zipFile.GetEntry(dataset);
            return zipEntry == null
                ? throw new ZipException($"Entry {dataset} does not exist in {zipFile.Name}")
                : zipFile.GetInputStream(zipEntry);
        }
    }
}
