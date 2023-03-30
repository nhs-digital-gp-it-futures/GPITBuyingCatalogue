using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Database;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Addresses.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.SeedData
{
    internal static class BuyingCatalogueSeedData
    {
        internal static void Initialize(BuyingCatalogueDbContext context)
        {
            AddDefaultData(context);
            AddCatalogueItems(context);
            AddMockedItems(context);
            context.SaveChanges();
        }

        private static void AddCatalogueItems(BuyingCatalogueDbContext context)
        {
            var catalogueSolutions = CatalogueSolutionSeedData.GetCatalogueSolutionItems();
            context.AddRange(catalogueSolutions);

            var associatedServices = AssociatedServicesSeedData.GetAssociatedServices();
            context.AddRange(associatedServices);

            var additionalServices = AdditionalServicesSeedData.GetAdditionalServices(catalogueSolutions);
            context.AddRange(additionalServices);

            foreach (var supplierId in new[] { 99999, 99998 })
            {
                foreach (var solution in catalogueSolutions.Where(x => x.SupplierId == supplierId))
                {
                    foreach (var service in associatedServices.Where(x => x.SupplierId == supplierId))
                    {
                        context.SupplierServiceAssociations.Add(new SupplierServiceAssociation(solution.Id, service.Id));
                    }
                }
            }

            context.SaveChanges();

            var cataloguePrices = CataloguePricesSeedData.GetCataloguePrices();
            context.InsertRangeWithIdentity(cataloguePrices);

            var cataloguePriceTiers = CataloguePricesSeedData.GetCataloguePriceTiers();
            context.InsertRangeWithIdentity(cataloguePriceTiers);

            var serviceLevelAgreements = ServiceLevelAgreementSeedData.GetServiceLevelAgreements();
            var serviceLevelContacts = ServiceLevelAgreementSeedData.GetServiceLevelContacts();
            var serviceLevels = ServiceLevelAgreementSeedData.GetServiceLevels();
            var serviceAvailabilityTimes = ServiceLevelAgreementSeedData.GetServiceAvailabilityTimes();

            context.AddRange(serviceLevelAgreements);

            context.InsertRangeWithIdentity(serviceLevelContacts);
            context.InsertRangeWithIdentity(serviceLevels);
            context.InsertRangeWithIdentity(serviceAvailabilityTimes);

            List<FrameworkSolution> frameworkSolutions = new()
            {
                new FrameworkSolution { FrameworkId = "DFOCVC001", SolutionId = new CatalogueItemId(99999, "001"), IsFoundation = false, LastUpdated = DateTime.UtcNow },
                new FrameworkSolution { FrameworkId = "DFOCVC001", SolutionId = new CatalogueItemId(99999, "002"), IsFoundation = false, LastUpdated = DateTime.UtcNow },
                new FrameworkSolution { FrameworkId = "DFOCVC001", SolutionId = new CatalogueItemId(99999, "003"), IsFoundation = false, LastUpdated = DateTime.UtcNow },
                new FrameworkSolution { FrameworkId = "NHSDGP001", SolutionId = new CatalogueItemId(99999, "003"), IsFoundation = true, LastUpdated = DateTime.UtcNow },
                new FrameworkSolution { FrameworkId = "NHSDGP001", SolutionId = new CatalogueItemId(99999, "004"), IsFoundation = false, LastUpdated = DateTime.UtcNow },
                new FrameworkSolution { FrameworkId = "NHSDGP001", SolutionId = new CatalogueItemId(99999, "005"), IsFoundation = false, LastUpdated = DateTime.UtcNow },
                new FrameworkSolution { FrameworkId = "NHSDGP001", SolutionId = new CatalogueItemId(99997, "001"), IsFoundation = true, LastUpdated = DateTime.UtcNow },
                new FrameworkSolution { FrameworkId = "NHSDGP001", SolutionId = new CatalogueItemId(99998, "001"), IsFoundation = true, LastUpdated = DateTime.UtcNow },
                new FrameworkSolution { FrameworkId = "DFOCVC001", SolutionId = new CatalogueItemId(99998, "001"), IsFoundation = true, LastUpdated = DateTime.UtcNow },
                new FrameworkSolution { FrameworkId = "NHSDGP001", SolutionId = new CatalogueItemId(99998, "002"), IsFoundation = false, LastUpdated = DateTime.UtcNow },
            };
            context.AddRange(frameworkSolutions);

            context.SaveChanges();
        }

        private static void AddDefaultData(BuyingCatalogueDbContext context)
        {
            // Suppliers
            List<Supplier> suppliers = new()
            {
                new Supplier
                {
                    Id = 99999,
                    Name = "DFOCVC Supplier",
                    LegalName = "DFOCVC Supplier",
                    Deleted = false,
                    Address = new Address
                    {
                        Line1 = "DFOCVC Supplier Tower",
                        Line2 = "High Street",
                        Town = "Leeds",
                        County = "West Yorkshire",
                        Country = "UK",
                    },
                    SupplierContacts = new List<SupplierContact>
                    {
                        new()
                        {
                            Email = "test@test.com",
                            FirstName = "Dave",
                            LastName = "Smith",
                            Department = "Test Department",
                            LastUpdated = DateTime.UtcNow,
                            PhoneNumber = "00987654321",
                            SupplierId = 99999,
                        },
                    },
                    Summary = "About this supplier",
                    SupplierUrl = "https://www.e2etest.com",
                    IsActive = true,
                },
                new Supplier
                {
                    Id = 99998,
                    Name = "E2E Test Supplier With Contact",
                    LegalName = "E2E Supplier WC",
                    Deleted = false,
                    Address = new Address
                    {
                        Line1 = "E2E Test Supplier",
                        Line2 = "High Street",
                        Town = "Cardiff",
                        County = "Cardiff",
                        Country = "UK",
                    },
                    Summary = "About this Supplier",
                    SupplierUrl = "https://www.e2etest.com",
                    SupplierContacts = new List<SupplierContact>
                    {
                        new()
                        {
                            SupplierId = 99998,
                            FirstName = "Alice",
                            LastName = "Smith",
                            Department = "Test Department",
                            Email = "Alice.Smith@e2etest.com",
                            PhoneNumber = "123456789",
                        },
                        new()
                        {
                            SupplierId = 99998,
                            FirstName = "Clark",
                            LastName = "Kent",
                            Department = "Reporter",
                            Email = "Clark.Kent@TheDailyPlanet.Test",
                            PhoneNumber = "123456789",
                        },
                        new()
                        {
                            SupplierId = 99998,
                            FirstName = "Bruce",
                            LastName = "Wayne",
                            Department = "Crime",
                            Email = "Bruce.Wayne@WayneEnterprises.Test",
                            PhoneNumber = "123456789",
                        },
                    },
                    IsActive = true,
                },
                new Supplier
                {
                    Id = 99997,
                    Name = "E2E Test Supplier",
                    LegalName = "E2E Supplier",
                    Deleted = false,
                    Address = new Address
                    {
                        Line1 = "E2E Test Supplier",
                        Line2 = "High Street",
                        Town = "Cardiff",
                        County = "Cardiff",
                        Country = "UK",
                    },
                    Summary = "About this Supplier",
                    SupplierUrl = "https://www.e2etest.com",
                    IsActive = true,
                },
                new Supplier
                {
                    Id = 99996,
                    Name = "Inactive Supplier",
                    LegalName = "Inactive Supplier",
                    Deleted = false,
                    IsActive = false,
                },
                new Supplier
                {
                    Id = 99995,
                    Name = "E2E Test Active Supplier With Single Contact",
                    LegalName = "E2E Supplier WSC",
                    Deleted = false,
                    Address = new Address
                    {
                        Line1 = "E2E Test Supplier",
                        Line2 = "High Street",
                        Town = "Cardiff",
                        County = "Cardiff",
                        Country = "UK",
                    },
                    Summary = "About this Supplier",
                    SupplierUrl = "https://www.e2etest.com",
                    SupplierContacts = new List<SupplierContact>
                    {
                        new()
                        {
                            SupplierId = 99995,
                            FirstName = "Alice",
                            LastName = "Smith",
                            Department = "Test Department",
                            Email = "Alice.Smith@e2etest.com",
                            PhoneNumber = "123456789",
                        },
                    },
                    IsActive = true,
                },
                new Supplier
                {
                    Id = 99991,
                    Name = "E2E Test Supplier-No Published Solutions",
                    LegalName = "E2E Supplier NPS",
                    Deleted = false,
                    Address = new Address
                    {
                        Line1 = "E2E Test Supplier",
                        Line2 = "High Street",
                        Town = "Cardiff",
                        County = "Cardiff",
                        Country = "UK",
                    },
                    Summary = "About this Supplier",
                    SupplierUrl = "https://www.e2etest.com",
                    IsActive = true,
                },
            };

            context.AddRange(suppliers);
            context.SaveChanges();

            // CapabilityStatus
            List<Database.Models.CapabilityStatus> capabilityStatuses = new()
            {
                new Database.Models.CapabilityStatus { Id = 1, Name = "Effective" },
            };
            context.AddRange(capabilityStatuses);

            // CatalogueItemType
            List<Database.Models.CatalogueItemType> catalogueItemTypes = new()
            {
                new Database.Models.CatalogueItemType { Id = 1, Name = "Solution" },
                new Database.Models.CatalogueItemType { Id = 2, Name = "Additional Service" },
                new Database.Models.CatalogueItemType { Id = 3, Name = "Associated Service" },
            };
            context.AddRange(catalogueItemTypes);

            // CataloguePriceType
            List<Database.Models.CataloguePriceType> cataloguePriceTypes = new()
            {
                new Database.Models.CataloguePriceType { Id = 1, Name = "Flat" },
                new Database.Models.CataloguePriceType { Id = 2, Name = "Tiered" },
            };
            context.AddRange(cataloguePriceTypes);

            // Frameworks
            List<EntityFramework.Catalogue.Models.Framework> frameworks = new()
            {
                new EntityFramework.Catalogue.Models.Framework { Id = "NHSDGP001", Name = "NHS Digital GP IT Futures Framework 1", ShortName = "GP IT Futures", Owner = "NHS Digital", LocalFundingOnly = false },
                new EntityFramework.Catalogue.Models.Framework { Id = "DFOCVC001", Name = "Digital First Online Consultation and Video Consultation Framework 1", ShortName = "DFOCVC", Owner = "NHS England", LocalFundingOnly = true },
            };
            context.AddRange(frameworks);

            List<CapabilityCategory> categories = new()
            {
                new CapabilityCategory { Id = 1, Name = "GP IT Futures" },
                new CapabilityCategory { Id = 2, Name = "Covid-19 Vaccination" },
                new CapabilityCategory { Id = 3, Name = "DFOCVC" },
                new CapabilityCategory { Id = 0, Name = "Undefined" },
            };
            context.AddRange(categories);

            // Capabilities
            string gpitFuturesBaseUrl = "https://gpitbjss.atlassian.net/wiki/spaces/GPITF/pages/";
            string covidVaccinationBaseUrl = "https://gpitbjss.atlassian.net/wiki/spaces/CVPDR/pages/";
            List<Capability> capabilities = new()
            {
                new Capability { Id = 1, CapabilityRef = "C1", Name = "Appointments Management – Citizen", Description = "Enables Citizens to manage their Appointments online. Supports the use of Appointment slots that have been configured in Appointments Management – GP.", SourceUrl = $"{gpitFuturesBaseUrl}1391134205/Appointments+Management+-+Citize", CategoryId = 1, Version = "1.0.1" },
                new Capability { Id = 2, CapabilityRef = "C2", Name = "Communicate With Practice – Citizen", Description = "Supports secure and trusted electronic communications between Citizens and the Practice. Integrates with Patient Information Maintenance.", SourceUrl = $"{gpitFuturesBaseUrl}1391134188/Communicate+With+Practice+-+Citize", CategoryId = 1, Version = "1.0.1" },
                new Capability { Id = 3, CapabilityRef = "C3", Name = "Prescription Ordering – Citizen", Description = "Enables Citizens to request medication online and manage nominated and preferred Pharmacies for Patients.", SourceUrl = $"{gpitFuturesBaseUrl}1391134214/Prescription+Ordering+-+Citizen", CategoryId = 1, Version = "1.0.1" },
                new Capability { Id = 4, CapabilityRef = "C4", Name = "View Record – Citizen", Description = "Enables Citizens to view their Patient Record online.", SourceUrl = $"{gpitFuturesBaseUrl}1391134197/View+Record+-+Citize", CategoryId = 1, Version = "1.0.1" },
                new Capability { Id = 5, CapabilityRef = "C5", Name = "Appointments Management – GP", Description = "Supports the administration, scheduling, resourcing and reporting of appointments.", SourceUrl = $"{gpitFuturesBaseUrl}1391134029/Appointments+Management+-+GP", CategoryId = 1, Version = "1.0.1" },
                new Capability { Id = 6, CapabilityRef = "C6", Name = "Clinical Decision Support", Description = "Supports clinical decision-making to improve Patient safety at the point of care.", SourceUrl = $"{gpitFuturesBaseUrl}1391134150/Clinical+Decision+Support", CategoryId = 1, Version = "1.0.1" },
                new Capability { Id = 7, CapabilityRef = "C7", Name = "Communication Management", Description = "Supports the delivery and management of communications to Citizens and Practice staff.", SourceUrl = $"{gpitFuturesBaseUrl}1391134087/Communication+Management", CategoryId = 1, Version = "1.0.1" },
                new Capability { Id = 8, CapabilityRef = "C8", Name = "Digital Diagnostics", Description = "Supports electronic requesting with other healthcare organisations. Test results can be received, reviewed and stored against the Patient record.", SourceUrl = $"{gpitFuturesBaseUrl}1391133770/Digital+Diagnostics", CategoryId = 1, Version = "1.0.1" },
                new Capability { Id = 9, CapabilityRef = "C9", Name = "Document Management", Description = "Supports the secure management and classification of all forms unstructured electronic documents including those created by scanning paper documents. Also enables processing of documents and matching documents with Patients.", SourceUrl = $"{gpitFuturesBaseUrl}1391134166/Document+Management", CategoryId = 1, Version = "1.0.1" },
                new Capability { Id = 10, CapabilityRef = "C10", Name = "GP Extracts Verification", Description = "Supports Practice staff in ensuring accuracy of the data that is used with the Calculating Quality Reporting Service (CQRS).", SourceUrl = $"{gpitFuturesBaseUrl}1391133797/GP+Extracts+Verificatio", CategoryId = 1, Version = "1.0.1" },
                new Capability { Id = 11, CapabilityRef = "C11", Name = "Referral Management", Description = "Supports recording, reviewing, sending, and reporting of Patient Referrals. Enables Referral information to be included in the Patient Record.", SourceUrl = $"{gpitFuturesBaseUrl}1391133614/Referral+Management", CategoryId = 1, Version = "1.0.1" },
                new Capability { Id = 12, CapabilityRef = "C12", Name = "Resource Management", Description = "Supports the management and reporting of Practice information, resources, Staff Members and related organisations. Also enables management of Staff Member availability and inactivity.", SourceUrl = $"{gpitFuturesBaseUrl}1391133939/Resource+Management", CategoryId = 1, Version = "1.0.1" },
                new Capability { Id = 13, CapabilityRef = "C13", Name = "Patient Information Maintenance", Description = "Supports the registration of Patients and the maintenance of all Patient personal information. Supports the organisation and presentation of a comprehensive Patient Record. Also supports the management of related persons and configuring access to Citizen Services.", SourceUrl = $"{gpitFuturesBaseUrl}1391134180/Patient+Information+Maintenance", CategoryId = 1, Version = "1.0.1" },
                new Capability { Id = 14, CapabilityRef = "C14", Name = "Prescribing", Description = "Supports the effective and safe prescribing of medical products and appliances to Patients. Information to support prescribing will be available.", SourceUrl = $"{gpitFuturesBaseUrl}1391134158/Prescribing", CategoryId = 1, Version = "1.0.1" },
                new Capability { Id = 15, CapabilityRef = "C15", Name = "Recording Consultations", Description = "Supports the standardised recording of Consultations and other General Practice activities. Also supports the extraction of Female Genital Mutilation (FGM) data for the FGM data set.", SourceUrl = $"{gpitFuturesBaseUrl}1391134389/Recording+Consultations", CategoryId = 1, Version = "1.0.1" },
                new Capability { Id = 16, CapabilityRef = "C16", Name = "Reporting", Description = "Enables reporting and analysis of data from other Capabilities in the Practice Solution to support clinical care and Practice management.", SourceUrl = $"{gpitFuturesBaseUrl}1391133718/Reporting", CategoryId = 1, Version = "1.0.1" },
                new Capability { Id = 17, CapabilityRef = "C17", Name = "Scanning", Description = "Support the conversion of paper documentation into digital format preserving the document quality and structure.", SourceUrl = $"{gpitFuturesBaseUrl}1391134270/Scanning", CategoryId = 1, Version = "1.0.1" },
                new Capability { Id = 18, CapabilityRef = "C18", Name = "Telehealth", Description = "Enables Citizens and Patients that use health monitoring solutions to share monitoring data with health and care professionals to support remote delivery of care and increase self-care outside of clinical settings.", SourceUrl = $"{gpitFuturesBaseUrl}1391134248/Telehealth", CategoryId = 1, Version = "1.0.1" },
                new Capability { Id = 19, CapabilityRef = "C19", Name = "Unstructured Data Extraction", Description = "Enables automated and manual interpretation and extraction of structured data from paper documents and unstructured electronic documents to support their classification and matching with Patient Records.", SourceUrl = $"{gpitFuturesBaseUrl}1391133668/Unstructured+Data+Extractio", CategoryId = 1, Version = "1.0.1" },
                new Capability { Id = 20, CapabilityRef = "C20", Name = "Workflow", Description = "Supports manual and automated management of work in the Practice. Also supports effective planning, tracking, monitoring and reporting.", SourceUrl = $"{gpitFuturesBaseUrl}1391134020/Workflow", CategoryId = 1, Version = "1.0.1" },
                new Capability { Id = 21, CapabilityRef = "C21", Name = "Care Homes", Description = "Enables a record of the Resident's health and care needs to be maintained and shared with parties who are involved in providing care, to support decision making and the effective planning and delivery of care.", SourceUrl = $"{gpitFuturesBaseUrl}1391133439/Care+Homes", CategoryId = 1, Version = "1.0.1" },
                new Capability { Id = 22, CapabilityRef = "C22", Name = "Caseload Management", Description = "Supports the allocation of appropriate Health and Care Professionals to Patients/Service Users in need of support, ensuring balanced workloads and the efficient use of staff and other resources.", SourceUrl = $"{gpitFuturesBaseUrl}1391133457/Caseload+Management", CategoryId = 1, Version = "1.0.1" },
                new Capability { Id = 23, CapabilityRef = "C23", Name = "Cross-organisation Appointment Booking", Description = "Enables appointments to be made available and booked across Organisational boundaries, creating flexibility for Health and Care Professionals and Patients/Service Users.", SourceUrl = $"{gpitFuturesBaseUrl}1391135407/Cross-organisation+Appointment+Booking", CategoryId = 1, Version = "1.0.1" },
                new Capability { Id = 24, CapabilityRef = "C24", Name = "Cross-organisation Workflow Tools", Description = "Supports and automates clinical and business processes across Organisational boundaries to make processes and communication more efficient.", SourceUrl = $"{gpitFuturesBaseUrl}1391133492/Cross-organisation+Workflow+Tools", CategoryId = 1, Version = "1.0.1" },
                new Capability { Id = 25, CapabilityRef = "C25", Name = "Cross-organisation Workforce Management", Description = "Supports the efficient planning and scheduling of the health and care workforce to ensure that services can be delivered effectively by the right staff.", SourceUrl = $"{gpitFuturesBaseUrl}1391135659/Cross-organisation+Workforce+Management", CategoryId = 1, Version = "1.0.1" },
                new Capability { Id = 26, CapabilityRef = "C26", Name = "Data Analytics for Integrated and Federated Care", Description = "Supports the analysis of multiple and complex datasets and presentation of the output to enable decision-making, service design and performance management.", SourceUrl = $"{gpitFuturesBaseUrl}1391135590/Data+Analytics+for+Integrated+and+Federated+Care", CategoryId = 1, Version = "1.0.1" },
                new Capability { Id = 27, CapabilityRef = "C27", Name = "Domiciliary Care", Description = "Enables Service Providers to effectively plan and manage Domiciliary Care services to ensure care needs are met and that Care Workers can manage their schedule.", SourceUrl = $"{gpitFuturesBaseUrl}1391133451/Domiciliary+Care", CategoryId = 1, Version = "1.0.1" },
                new Capability { Id = 29, CapabilityRef = "C29", Name = "e-Consultations (Professional to Professional)", Description = "Enables the communication and sharing of specialist knowledge and advice between Health and Care Professionals to support better care decisions and professional development.", SourceUrl = $"{gpitFuturesBaseUrl}1391135495/e-Consultations+Professional+to+Professional", CategoryId = 1, Version = "1.0.1" },
                new Capability { Id = 30, CapabilityRef = "C30", Name = "Medicines Optimisation", Description = "Supports clinicians and pharmacists in reviewing a Patient's medication and requesting changes to medication to ensure the Patient is taking the best combination of medicines.", SourceUrl = $"{gpitFuturesBaseUrl}1391133405/Medicines+Optimisatio", CategoryId = 1, Version = "1.0.1" },
                new Capability { Id = 32, CapabilityRef = "C32", Name = "Personal Health Budget", Description = "Enables a Patient/Service User to set up and manage a Personal Health Budget giving them more choice and control over the management of their identified healthcare and well-being needs.", SourceUrl = $"{gpitFuturesBaseUrl}1391133426/Personal+Health+Budget", CategoryId = 1, Version = "1.0.1" },
                new Capability { Id = 33, CapabilityRef = "C33", Name = "Personal Health Record", Description = "Enables a Patient/Service User to manage and maintain their own Electronic Health Record and to share that information with relevant Health and Care Professionals.", SourceUrl = $"{gpitFuturesBaseUrl}1391135480/Personal+Health+Record", CategoryId = 1, Version = "1.0.1" },
                new Capability { Id = 34, CapabilityRef = "C34", Name = "Population Health Management", Description = "Enables Organisations to accumulate, analyse and report on Patient healthcare data to identify improvement in care and identify and track Patient outcomes.", SourceUrl = $"{gpitFuturesBaseUrl}1391135469/Population+Health+Management", CategoryId = 1, Version = "1.0.1" },
                new Capability { Id = 35, CapabilityRef = "C35", Name = "Risk Stratification", Description = "Supports Health and Care Professionals by providing trusted models to predict future Patient events, informing interventions to achieve better Patient outcomes.", SourceUrl = $"{gpitFuturesBaseUrl}1391133445/Risk+Stratificatio", CategoryId = 1, Version = "1.0.1" },
                new Capability { Id = 36, CapabilityRef = "C36", Name = "Shared Care Plans", Description = "Enables the maintenance of a single, shared care plan across multiple Organisations to ensure more co-ordinated working and more efficient management of activities relating to the Patient/Service User's health and care.", SourceUrl = $"{gpitFuturesBaseUrl}1391134486/Shared+Care+Plans", CategoryId = 1, Version = "1.0.1" },
                new Capability { Id = 37, CapabilityRef = "C37", Name = "Social Prescribing", Description = "Supports the referral of Patients/Service Users to non-clinical services to help address their health and well-being needs.", SourceUrl = $"{gpitFuturesBaseUrl}1391135572/Social+Prescribing", CategoryId = 1, Version = "1.0.1" },
                new Capability { Id = 38, CapabilityRef = "C38", Name = "Telecare", Description = "Supports the monitoring of Patients/Service Users or their environment to ensure quick identification and response to any adverse event.", SourceUrl = $"{gpitFuturesBaseUrl}1391135549/Telecare", CategoryId = 1, Version = "1.0.1" },
                new Capability { Id = 39, CapabilityRef = "C39", Name = "Unified Care Record", Description = "Provides a consolidated view to Health and Care Professionals of a Patient/Service User's complete and up-to-date records, sourced from various health and care settings.", SourceUrl = $"{gpitFuturesBaseUrl}1391134504/Unified+Care+Record", CategoryId = 1, Version = "1.0.1" },
                new Capability { Id = 40, CapabilityRef = "C40", Name = "Medicines Verification", Description = "Supports compliance with the Falsified Medicines Directive and minimise the risk that falsified medicinal products are supplied to the public.", SourceUrl = $"{gpitFuturesBaseUrl}1391135093/Medicines+Verificatio", CategoryId = 1, Version = "1.0.1" },
                new Capability { Id = 41, CapabilityRef = "C41", Name = "Productivity", Description = "Supports Patients/Service Users and Health and Care Professionals by delivering improved efficiency or experience related outcomes.", SourceUrl = $"{gpitFuturesBaseUrl}1391135618/Productivity", CategoryId = 1, Version = "1.0.1" },
                new Capability { Id = 42, CapabilityRef = "C42", Name = "Dispensing", Description = "Supports the timely and effective dispensing of medical products and appliances to Patients.", SourceUrl = $"{gpitFuturesBaseUrl}1391133465/Dispensing", CategoryId = 1, Version = "1.0.1" },
                new Capability { Id = 43, CapabilityRef = "C43", Name = "Online Consultation ", Description = "The Online Consultation Capability allows Patients/Service Users/Proxies to request and receive support relating to healthcare concerns, at a time and place convenient for them. ", SourceUrl = "https://gpitbjss.atlassian.net/wiki/spaces/DFOCVC/pages/2519925022/Online+Consultation ", CategoryId = 3, Version = "1.0.1" },
                new Capability { Id = 44, CapabilityRef = "C44", Name = "Video Consultation ", Description = "The Video Consultation Capability allows Health or Care Professionals to conduct secure live remote video consultations with individual or groups of Patients/Service Users/Proxies ensuring they can receive support relating to healthcare concerns when a Video Consultation is most appropriate. ", SourceUrl = "https://gpitbjss.atlassian.net/wiki/spaces/DFOCVC/pages/2519925030/Video+Consultation", CategoryId = 3, Version = "1.0.1" },
                new Capability { Id = 45, CapabilityRef = "C45", Name = "Cohort Identification ", Description = "The Cohort Identification Capability enables the identification of Patient cohorts by identifying Patients that require a COVID-19 vaccination based on nationally defined criteria. ", SourceUrl = $"{covidVaccinationBaseUrl}7918551305/Cohort+Identification", CategoryId = 2, Version = "1.0.1" },
                new Capability { Id = 46, CapabilityRef = "C46", Name = "Appointments Management – COVID-19 Vaccinations", Description = "The Appointments Management – COVID-19 Vaccinations Capability enables the administration and scheduling of COVID-19 vaccination appointments for Patients. ", SourceUrl = $"{covidVaccinationBaseUrl}7918551324/Appointments+Management+-+COVID-19+Vaccinations", CategoryId = 2, Version = "1.0.1" },
                new Capability { Id = 47, CapabilityRef = "C47", Name = "Vaccination and Adverse Reaction Recording ", Description = "The Vaccination and Adverse Reaction Recording Capability enables the recording of COVID-19 vaccination and adverse reaction data at the point of care. The Capability also supports the delivery of this data to the Patient's registered GP Practice Foundation Solution and to NHS Digital. ", SourceUrl = $"{covidVaccinationBaseUrl}7918551342/Vaccination+and+Adverse+Reaction+Recording ", CategoryId = 2, Version = "1.0.1" },
            };

            context.AddRange(capabilities);

            // Epics
            List<Epic> epics = new()
            {
                new Epic { Id = "C1E1", Name = "Manage Appointments", CapabilityId = capabilities.First(s => s.CapabilityRef == "C1").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "C1E2", Name = "Manage Appointments by Proxy", CapabilityId = capabilities.First(s => s.CapabilityRef == "C1").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "C2E1", Name = "Manage communications – Patient", CapabilityId = capabilities.First(s => s.CapabilityRef == "C2").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "C2E2", Name = "Manage communications – Proxy", CapabilityId = capabilities.First(s => s.CapabilityRef == "C2").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "C3E1", Name = "Manage Repeat Medications – Patient", CapabilityId = capabilities.First(s => s.CapabilityRef == "C3").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "C3E2", Name = "Manage my nominated EPS pharmacy", CapabilityId = capabilities.First(s => s.CapabilityRef == "C3").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "C3E3", Name = "Manage my Preferred Pharmacy As a Patient", CapabilityId = capabilities.First(s => s.CapabilityRef == "C3").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "C3E4", Name = "Manage Acute Medications", CapabilityId = capabilities.First(s => s.CapabilityRef == "C3").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "C3E5", Name = "View medication information", CapabilityId = capabilities.First(s => s.CapabilityRef == "C3").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "C3E6", Name = "Manage Repeat Medications as a Proxy", CapabilityId = capabilities.First(s => s.CapabilityRef == "C3").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "C3E7", Name = "Manage nominated EPS pharmacy as a Proxy", CapabilityId = capabilities.First(s => s.CapabilityRef == "C3").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "C3E8", Name = "Manage Preferred Pharmacy as a Proxy", CapabilityId = capabilities.First(s => s.CapabilityRef == "C3").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "C3E9", Name = "Manage Acute Medications as a Proxy", CapabilityId = capabilities.First(s => s.CapabilityRef == "C3").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "C3E10", Name = "View medication information as a Proxy", CapabilityId = capabilities.First(s => s.CapabilityRef == "C3").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "C4E1", Name = "View Patient Record – Patient", CapabilityId = capabilities.First(s => s.CapabilityRef == "C4").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "C4E2", Name = "View Patient Record – Proxy", CapabilityId = capabilities.First(s => s.CapabilityRef == "C4").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "C5E1", Name = "Manage Session templates", CapabilityId = capabilities.First(s => s.CapabilityRef == "C5").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "C5E2", Name = "Manage Sessions", CapabilityId = capabilities.First(s => s.CapabilityRef == "C5").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "C5E3", Name = "Configure Appointments", CapabilityId = capabilities.First(s => s.CapabilityRef == "C5").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "C5E4", Name = "Practice configuration", CapabilityId = capabilities.First(s => s.CapabilityRef == "C5").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "C5E5", Name = "Manage Appointments", CapabilityId = capabilities.First(s => s.CapabilityRef == "C5").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "C5E6", Name = "View Appointment reports", CapabilityId = capabilities.First(s => s.CapabilityRef == "C5").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "C5E7", Name = "Access Patient Record", CapabilityId = capabilities.First(s => s.CapabilityRef == "C5").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "C6E1", Name = "Access to Clinical Decision Support", CapabilityId = capabilities.First(s => s.CapabilityRef == "C6").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "C6E2", Name = "Local configuration for Clinical Decision Support triggering", CapabilityId = capabilities.First(s => s.CapabilityRef == "C6").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "C6E3", Name = "View Clinical Decision Support reports", CapabilityId = capabilities.First(s => s.CapabilityRef == "C6").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "C6E4", Name = "Configuration for custom Clinical Decision Support", CapabilityId = capabilities.First(s => s.CapabilityRef == "C6").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "C7E1", Name = "Manage communication consents for a Patient", CapabilityId = capabilities.First(s => s.CapabilityRef == "C7").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "C7E2", Name = "Manage communication preferences for a Patient", CapabilityId = capabilities.First(s => s.CapabilityRef == "C7").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "C7E3", Name = "Manage communication templates", CapabilityId = capabilities.First(s => s.CapabilityRef == "C7").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "C7E4", Name = "Create communications", CapabilityId = capabilities.First(s => s.CapabilityRef == "C7").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "C7E5", Name = "Manage automated communications", CapabilityId = capabilities.First(s => s.CapabilityRef == "C7").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "C7E6", Name = "View communication reports", CapabilityId = capabilities.First(s => s.CapabilityRef == "C7").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "C7E7", Name = "Access Patient Record", CapabilityId = capabilities.First(s => s.CapabilityRef == "C7").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "C7E8", Name = "Manage communication consents for a Proxy", CapabilityId = capabilities.First(s => s.CapabilityRef == "C7").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "C7E9", Name = "Manage communication preferences for a Proxy", CapabilityId = capabilities.First(s => s.CapabilityRef == "C7").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "C7E10", Name = "Manage incoming communications", CapabilityId = capabilities.First(s => s.CapabilityRef == "C7").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "C8E1", Name = "Manage Requests for Investigations", CapabilityId = capabilities.First(s => s.CapabilityRef == "C8").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "C8E2", Name = "View Requests for Investigations reports", CapabilityId = capabilities.First(s => s.CapabilityRef == "C8").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "C8E3", Name = "Create a Request for Investigation for multiple Patients", CapabilityId = capabilities.First(s => s.CapabilityRef == "C8").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "C8E4", Name = "Receive external Request for Investigation information", CapabilityId = capabilities.First(s => s.CapabilityRef == "C8").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "C9E1", Name = "Manage document classifications", CapabilityId = capabilities.First(s => s.CapabilityRef == "C9").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "C9E2", Name = "Manage document properties", CapabilityId = capabilities.First(s => s.CapabilityRef == "C9").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "C9E3", Name = "Manage document attributes", CapabilityId = capabilities.First(s => s.CapabilityRef == "C9").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "C9E4", Name = "Manage document coded entries", CapabilityId = capabilities.First(s => s.CapabilityRef == "C9").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "C9E5", Name = "Document workflows", CapabilityId = capabilities.First(s => s.CapabilityRef == "C9").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "C9E6", Name = "Manage document annotation", CapabilityId = capabilities.First(s => s.CapabilityRef == "C9").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "C9E7", Name = "Search for documents", CapabilityId = capabilities.First(s => s.CapabilityRef == "C9").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "C9E8", Name = "Search document content", CapabilityId = capabilities.First(s => s.CapabilityRef == "C9").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "C9E9", Name = "Document and Patient matching", CapabilityId = capabilities.First(s => s.CapabilityRef == "C9").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "C9E10", Name = "Visually compare multiple documents", CapabilityId = capabilities.First(s => s.CapabilityRef == "C9").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "C9E11", Name = "View any version of a document", CapabilityId = capabilities.First(s => s.CapabilityRef == "C9").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "C9E12", Name = "Print documents", CapabilityId = capabilities.First(s => s.CapabilityRef == "C9").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "C9E13", Name = "Export documents to new formats", CapabilityId = capabilities.First(s => s.CapabilityRef == "C9").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "C9E14", Name = "Document reports", CapabilityId = capabilities.First(s => s.CapabilityRef == "C9").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "C9E15", Name = "Receipt of electronic documents", CapabilityId = capabilities.First(s => s.CapabilityRef == "C9").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "C9E16", Name = "Access Patient Record", CapabilityId = capabilities.First(s => s.CapabilityRef == "C9").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "C9E17", Name = "Search for documents using document content", CapabilityId = capabilities.First(s => s.CapabilityRef == "C9").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "C10E1", Name = "View GPES payment extract reports", CapabilityId = capabilities.First(s => s.CapabilityRef == "C10").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "C10E2", Name = "View national GPES non-payment extract reports", CapabilityId = capabilities.First(s => s.CapabilityRef == "C10").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "C11E1", Name = "Manage Referrals", CapabilityId = capabilities.First(s => s.CapabilityRef == "C11").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "C11E2", Name = "View Referral reports", CapabilityId = capabilities.First(s => s.CapabilityRef == "C11").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "C12E1", Name = "Manage General Practice and Branch site information", CapabilityId = capabilities.First(s => s.CapabilityRef == "C12").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "C12E2", Name = "Manage General Practice Staff Members", CapabilityId = capabilities.First(s => s.CapabilityRef == "C12").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "C12E3", Name = "Manage Staff Member inactivity periods", CapabilityId = capabilities.First(s => s.CapabilityRef == "C12").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "C12E4", Name = "Manage Staff Member Groups", CapabilityId = capabilities.First(s => s.CapabilityRef == "C12").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "C12E5", Name = "Manage Related Organisations information", CapabilityId = capabilities.First(s => s.CapabilityRef == "C12").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "C12E6", Name = "Manage Related Organisation Staff Members", CapabilityId = capabilities.First(s => s.CapabilityRef == "C12").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "C12E7", Name = "Manage Non-human Resources", CapabilityId = capabilities.First(s => s.CapabilityRef == "C12").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "C13E1", Name = "Manage Patients ", CapabilityId = capabilities.First(s => s.CapabilityRef == "C13").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "C13E2", Name = "Access Patient Record", CapabilityId = capabilities.First(s => s.CapabilityRef == "C13").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "C13E3", Name = "Manage Patient Related Persons", CapabilityId = capabilities.First(s => s.CapabilityRef == "C13").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "C13E4", Name = "Manage Patients for Citizen Services", CapabilityId = capabilities.First(s => s.CapabilityRef == "C13").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "C13E5", Name = "Manage Patient Communications", CapabilityId = capabilities.First(s => s.CapabilityRef == "C13").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "C13E6", Name = "Configure Patient notifications", CapabilityId = capabilities.First(s => s.CapabilityRef == "C13").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "C13E7", Name = "Manage Practice notifications – Patient", CapabilityId = capabilities.First(s => s.CapabilityRef == "C13").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "C13E8", Name = "Search for Patient Records", CapabilityId = capabilities.First(s => s.CapabilityRef == "C13").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "C13E9", Name = "View Patient Reports", CapabilityId = capabilities.First(s => s.CapabilityRef == "C13").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "C13E10", Name = "Configure Citizen service access for the Practice", CapabilityId = capabilities.First(s => s.CapabilityRef == "C13").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "C13E11", Name = "Identify Patients outside of Catchment Area", CapabilityId = capabilities.First(s => s.CapabilityRef == "C13").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "C13E12", Name = "Manage Patient Cohorts from Search Results", CapabilityId = capabilities.First(s => s.CapabilityRef == "C13").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "C13E13", Name = "View Subject Access Request reports", CapabilityId = capabilities.First(s => s.CapabilityRef == "C13").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "C13E14", Name = "Manage Acute Prescription Request Service", CapabilityId = capabilities.First(s => s.CapabilityRef == "C13").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "C13E15", Name = "Notify the Patient of changes", CapabilityId = capabilities.First(s => s.CapabilityRef == "C13").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "C13E16", Name = "Manage Subject Access Request (SAR) requests", CapabilityId = capabilities.First(s => s.CapabilityRef == "C13").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "C13E17", Name = "Notify the Proxy of changes", CapabilityId = capabilities.First(s => s.CapabilityRef == "C13").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "C13E18", Name = "Manage Practice notifications – Proxy", CapabilityId = capabilities.First(s => s.CapabilityRef == "C13").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "C13E19", Name = "Configure Proxy notifications", CapabilityId = capabilities.First(s => s.CapabilityRef == "C13").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "C13E20", Name = "Manage Proxy Communications", CapabilityId = capabilities.First(s => s.CapabilityRef == "C13").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "C13E21", Name = "Manage Proxys for Citizen Services", CapabilityId = capabilities.First(s => s.CapabilityRef == "C13").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "C14E1", Name = "Access prescribable items", CapabilityId = capabilities.First(s => s.CapabilityRef == "C14").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "C14E2", Name = "Manage Formularies", CapabilityId = capabilities.First(s => s.CapabilityRef == "C14").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "C14E3", Name = "Manage shared Formularies", CapabilityId = capabilities.First(s => s.CapabilityRef == "C14").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "C14E4", Name = "Set default Formulary for Practice Users", CapabilityId = capabilities.First(s => s.CapabilityRef == "C14").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "C14E5", Name = "Manage prescribed medication", CapabilityId = capabilities.First(s => s.CapabilityRef == "C14").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "C14E6", Name = "Manage prescriptions", CapabilityId = capabilities.First(s => s.CapabilityRef == "C14").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "C14E7", Name = "Manage Patient's Preferred Pharmacy", CapabilityId = capabilities.First(s => s.CapabilityRef == "C14").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "C14E8", Name = "Manage Patient medication reviews", CapabilityId = capabilities.First(s => s.CapabilityRef == "C14").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "C14E9", Name = "View prescribed medication reports", CapabilityId = capabilities.First(s => s.CapabilityRef == "C14").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "C14E10", Name = "Manage Repeat Medication requests", CapabilityId = capabilities.First(s => s.CapabilityRef == "C14").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "C14E11", Name = "Manage Acute Medication requests", CapabilityId = capabilities.First(s => s.CapabilityRef == "C14").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "C14E12", Name = "Manage Authorising Prescribers", CapabilityId = capabilities.First(s => s.CapabilityRef == "C14").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "C14E13", Name = "Access Patient Record", CapabilityId = capabilities.First(s => s.CapabilityRef == "C14").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "C14E14", Name = "View EPS Nominated Pharmacy changes", CapabilityId = capabilities.First(s => s.CapabilityRef == "C14").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "C14E15", Name = "Configure warnings for prescribable items", CapabilityId = capabilities.First(s => s.CapabilityRef == "C14").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "C14E16", Name = "Medications are linked to diagnoses", CapabilityId = capabilities.First(s => s.CapabilityRef == "C14").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "C15E1", Name = "Record Consultation information", CapabilityId = capabilities.First(s => s.CapabilityRef == "C15").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "C15E2", Name = "View report on calls and recalls", CapabilityId = capabilities.First(s => s.CapabilityRef == "C15").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "C15E3", Name = "View report of Consultations", CapabilityId = capabilities.First(s => s.CapabilityRef == "C15").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "C15E4", Name = "Access Patient Record", CapabilityId = capabilities.First(s => s.CapabilityRef == "C15").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "C15E5", Name = "Manage Consultation form templates", CapabilityId = capabilities.First(s => s.CapabilityRef == "C15").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "C15E6", Name = "Share Consultation form templates", CapabilityId = capabilities.First(s => s.CapabilityRef == "C15").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "C15E7", Name = "Use supplier implemented national Consultation form templates", CapabilityId = capabilities.First(s => s.CapabilityRef == "C15").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "C15E8", Name = "Extract Female Genital Mutilation data", CapabilityId = capabilities.First(s => s.CapabilityRef == "C15").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "C16E1", Name = "Report data from other Capabilities", CapabilityId = capabilities.First(s => s.CapabilityRef == "C16").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "C17E1", Name = "Scan documents", CapabilityId = capabilities.First(s => s.CapabilityRef == "C17").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "C17E2", Name = "Image enhancement", CapabilityId = capabilities.First(s => s.CapabilityRef == "C17").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "C18E1", Name = "Share monitoring data with my General Practice", CapabilityId = capabilities.First(s => s.CapabilityRef == "C18").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "C18E2", Name = "Configure Telehealth for the Practice", CapabilityId = capabilities.First(s => s.CapabilityRef == "C18").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "C18E3", Name = "Configure Telehealth for the Patient", CapabilityId = capabilities.First(s => s.CapabilityRef == "C18").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "C18E4", Name = "Manage incoming Telehealth data", CapabilityId = capabilities.First(s => s.CapabilityRef == "C18").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "C19E1", Name = "Document classification", CapabilityId = capabilities.First(s => s.CapabilityRef == "C19").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "C19E2", Name = "Manage Document Classification rules", CapabilityId = capabilities.First(s => s.CapabilityRef == "C19").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "C19E3", Name = "Document and Patient matching", CapabilityId = capabilities.First(s => s.CapabilityRef == "C19").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "C20E1", Name = "Manage Task templates", CapabilityId = capabilities.First(s => s.CapabilityRef == "C20").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "C20E2", Name = "Manage Workflow templates", CapabilityId = capabilities.First(s => s.CapabilityRef == "C20").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "C20E3", Name = "Configure Task rules", CapabilityId = capabilities.First(s => s.CapabilityRef == "C20").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "C20E4", Name = "Configure Workflow rules", CapabilityId = capabilities.First(s => s.CapabilityRef == "C20").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "C20E5", Name = "Manage Tasks", CapabilityId = capabilities.First(s => s.CapabilityRef == "C20").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "C20E6", Name = "Manage Workflows", CapabilityId = capabilities.First(s => s.CapabilityRef == "C20").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "C20E7", Name = "Manage Task List configurations", CapabilityId = capabilities.First(s => s.CapabilityRef == "C20").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "C20E8", Name = "Manage Workflows List configurations", CapabilityId = capabilities.First(s => s.CapabilityRef == "C20").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "C20E9", Name = "View Task reports", CapabilityId = capabilities.First(s => s.CapabilityRef == "C20").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "C20E10", Name = "View Workflow reports", CapabilityId = capabilities.First(s => s.CapabilityRef == "C20").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "C20E11", Name = "Access Patient Record", CapabilityId = capabilities.First(s => s.CapabilityRef == "C20").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "C20E12", Name = "Share Task List configuration", CapabilityId = capabilities.First(s => s.CapabilityRef == "C20").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "C20E13", Name = "Share Workflow List configuration", CapabilityId = capabilities.First(s => s.CapabilityRef == "C20").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "C21E1", Name = "Maintain Resident's Care Home Record", CapabilityId = capabilities.First(s => s.CapabilityRef == "C21").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "C21E2", Name = "Maintain Resident Proxy preferences", CapabilityId = capabilities.First(s => s.CapabilityRef == "C21").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "C21E3", Name = "View and maintain End of Life Care Plans", CapabilityId = capabilities.First(s => s.CapabilityRef == "C21").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "C21E4", Name = "Record incident and adverse events", CapabilityId = capabilities.First(s => s.CapabilityRef == "C21").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "C21E5", Name = "Maintain Staff Records", CapabilityId = capabilities.First(s => s.CapabilityRef == "C21").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "C21E6", Name = "Maintain Staff Task schedules", CapabilityId = capabilities.First(s => s.CapabilityRef == "C21").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "C21E7", Name = "Manage Tasks", CapabilityId = capabilities.First(s => s.CapabilityRef == "C21").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "C21E8", Name = "Reporting", CapabilityId = capabilities.First(s => s.CapabilityRef == "C21").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "C22E1", Name = "Manage Cases", CapabilityId = capabilities.First(s => s.CapabilityRef == "C22").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "C22E2", Name = "Maintain Caseloads", CapabilityId = capabilities.First(s => s.CapabilityRef == "C22").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "C22E3", Name = "Generate and manage contact schedules", CapabilityId = capabilities.First(s => s.CapabilityRef == "C22").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "C22E4", Name = "Update Case details", CapabilityId = capabilities.First(s => s.CapabilityRef == "C22").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "C22E5", Name = "Review and comment on Caseload", CapabilityId = capabilities.First(s => s.CapabilityRef == "C22").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "C22E6", Name = "Review and comment on contact schedule", CapabilityId = capabilities.First(s => s.CapabilityRef == "C22").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "C22E7", Name = "View and update Patient/Service User's Health or Care Record", CapabilityId = capabilities.First(s => s.CapabilityRef == "C22").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "C22E8", Name = "Reporting", CapabilityId = capabilities.First(s => s.CapabilityRef == "C22").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "C22E9", Name = "Care Pathway templates", CapabilityId = capabilities.First(s => s.CapabilityRef == "C22").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "C23E1", Name = "Make Appointments available to external organisations", CapabilityId = capabilities.First(s => s.CapabilityRef == "C23").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "C23E2", Name = "Search externally bookable Appointment slots", CapabilityId = capabilities.First(s => s.CapabilityRef == "C23").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "C23E3", Name = "Book externally bookable Appointment slots", CapabilityId = capabilities.First(s => s.CapabilityRef == "C23").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "C23E4", Name = "Maintain Appointments", CapabilityId = capabilities.First(s => s.CapabilityRef == "C23").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "C23E5", Name = "Notifications", CapabilityId = capabilities.First(s => s.CapabilityRef == "C23").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "C23E6", Name = "Manage Appointment Requests", CapabilityId = capabilities.First(s => s.CapabilityRef == "C23").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "C23E7", Name = "Booking approval", CapabilityId = capabilities.First(s => s.CapabilityRef == "C23").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "C23E8", Name = "Report on usage of Cross-Organisation Appointments", CapabilityId = capabilities.First(s => s.CapabilityRef == "C23").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "C23E9", Name = "Manage Cross-Organisation Appointment Booking templates", CapabilityId = capabilities.First(s => s.CapabilityRef == "C23").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "C24E1", Name = "Use Workflow to run a Cross-organisational Process", CapabilityId = capabilities.First(s => s.CapabilityRef == "C24").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "C24E2", Name = "Maintain cross-organisational workflows", CapabilityId = capabilities.First(s => s.CapabilityRef == "C24").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "C24E3", Name = "Maintain cross-organisational workflow templates", CapabilityId = capabilities.First(s => s.CapabilityRef == "C24").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "C24E4", Name = "Share workflow templates", CapabilityId = capabilities.First(s => s.CapabilityRef == "C24").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "C24E5", Name = "Manage automated notifications and reminders", CapabilityId = capabilities.First(s => s.CapabilityRef == "C24").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "C24E6", Name = "Manage ad-hoc notifications", CapabilityId = capabilities.First(s => s.CapabilityRef == "C24").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "C24E7", Name = "Report on Cross-organisational Workflows", CapabilityId = capabilities.First(s => s.CapabilityRef == "C24").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "C25E1", Name = "Maintain service schedule", CapabilityId = capabilities.First(s => s.CapabilityRef == "C25").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "C25E2", Name = "Share service schedule", CapabilityId = capabilities.First(s => s.CapabilityRef == "C25").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "C25E3", Name = "Workforce management reporting", CapabilityId = capabilities.First(s => s.CapabilityRef == "C25").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "C26E1", Name = "Analyse data across multiple organisations within the Integrated/Federated Care Setting (Federation)", CapabilityId = capabilities.First(s => s.CapabilityRef == "C26").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "C26E2", Name = "Analyse data across different datasets", CapabilityId = capabilities.First(s => s.CapabilityRef == "C26").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "C26E3", Name = "Create new or update existing reports ", CapabilityId = capabilities.First(s => s.CapabilityRef == "C26").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "C26E4", Name = "Run existing reports", CapabilityId = capabilities.First(s => s.CapabilityRef == "C26").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "C26E5", Name = "Present output", CapabilityId = capabilities.First(s => s.CapabilityRef == "C26").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "C26E6", Name = "Define selection rules on reports", CapabilityId = capabilities.First(s => s.CapabilityRef == "C26").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "C26E7", Name = "Create and run performance-based reports", CapabilityId = capabilities.First(s => s.CapabilityRef == "C26").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "C26E8", Name = "Drill down to detailed information", CapabilityId = capabilities.First(s => s.CapabilityRef == "C26").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "C26E9", Name = "Forecasting", CapabilityId = capabilities.First(s => s.CapabilityRef == "C26").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "C26E10", Name = "Enable reporting at different levels", CapabilityId = capabilities.First(s => s.CapabilityRef == "C26").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "C27E1", Name = "Maintain Domiciliary Care schedules", CapabilityId = capabilities.First(s => s.CapabilityRef == "C27").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "C27E2", Name = "Share Domiciliary Care schedules", CapabilityId = capabilities.First(s => s.CapabilityRef == "C27").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "C27E3", Name = "Manage Appointments", CapabilityId = capabilities.First(s => s.CapabilityRef == "C27").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "C27E4", Name = "Service User manages their schedule for Domiciliary Care", CapabilityId = capabilities.First(s => s.CapabilityRef == "C27").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "C27E5", Name = "Manage Care Plans for Service Users", CapabilityId = capabilities.First(s => s.CapabilityRef == "C27").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "C27E6", Name = "Remote access to Domiciliary Care schedule", CapabilityId = capabilities.First(s => s.CapabilityRef == "C27").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "C27E7", Name = "Receive notifications relating to Service User", CapabilityId = capabilities.First(s => s.CapabilityRef == "C27").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "C27E8", Name = "Reports", CapabilityId = capabilities.First(s => s.CapabilityRef == "C27").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "C27E9", Name = "Nominated individuals to view Domiciliary Care schedule", CapabilityId = capabilities.First(s => s.CapabilityRef == "C27").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "C29E1", Name = "Health or Care Professional requests support", CapabilityId = capabilities.First(s => s.CapabilityRef == "C29").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "C29E2", Name = "Respond to request for support from another Health or Care Professional", CapabilityId = capabilities.First(s => s.CapabilityRef == "C29").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "C29E3", Name = "Link additional information to a request for support", CapabilityId = capabilities.First(s => s.CapabilityRef == "C29").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "C29E4", Name = "Live Consultation: Health and Care Professionals", CapabilityId = capabilities.First(s => s.CapabilityRef == "C29").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "C29E5", Name = "Link Consultation to Patient/Service User,s Record", CapabilityId = capabilities.First(s => s.CapabilityRef == "C29").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "C29E6", Name = "Reports", CapabilityId = capabilities.First(s => s.CapabilityRef == "C29").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "C30E1", Name = "Single unified medication view", CapabilityId = capabilities.First(s => s.CapabilityRef == "C30").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "C30E2", Name = "Request medication changes", CapabilityId = capabilities.First(s => s.CapabilityRef == "C30").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "C30E3", Name = "Identify Patients requiring medicines review", CapabilityId = capabilities.First(s => s.CapabilityRef == "C30").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "C30E4", Name = "Maintain medicines review", CapabilityId = capabilities.First(s => s.CapabilityRef == "C30").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "C30E5", Name = "Notify Patient and Proxies of medication changes", CapabilityId = capabilities.First(s => s.CapabilityRef == "C30").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "C30E6", Name = "Notify other interested parties of medication changes", CapabilityId = capabilities.First(s => s.CapabilityRef == "C30").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "C30E7", Name = "Configure medication substitutions", CapabilityId = capabilities.First(s => s.CapabilityRef == "C30").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "C30E8", Name = "Use pre-configured medication substitutions", CapabilityId = capabilities.First(s => s.CapabilityRef == "C30").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "C30E9", Name = "Maintain prescribed medication", CapabilityId = capabilities.First(s => s.CapabilityRef == "C30").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "C30E10", Name = "Access national or local Medicines Optimisation guidance", CapabilityId = capabilities.First(s => s.CapabilityRef == "C30").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "C30E11", Name = "Prescribing decision support", CapabilityId = capabilities.First(s => s.CapabilityRef == "C30").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "C30E12", Name = "Medicines Optimisation reporting", CapabilityId = capabilities.First(s => s.CapabilityRef == "C30").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "C30E13", Name = "Configure notifications for required Medicines Reviews", CapabilityId = capabilities.First(s => s.CapabilityRef == "C30").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "C30E14", Name = "Receive notification for required medicines reviews", CapabilityId = capabilities.First(s => s.CapabilityRef == "C30").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "C32E1", Name = "Manage Personal Health Budget", CapabilityId = capabilities.First(s => s.CapabilityRef == "C32").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "C32E2", Name = "Record Personal Health Budget purchases", CapabilityId = capabilities.First(s => s.CapabilityRef == "C32").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "C32E3", Name = "Assess Personal Health Budgets", CapabilityId = capabilities.First(s => s.CapabilityRef == "C32").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "C32E4", Name = "Link Personal Health Budget with care plan", CapabilityId = capabilities.First(s => s.CapabilityRef == "C32").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "C32E5", Name = "Support different models for management of Personal Health Budgets", CapabilityId = capabilities.First(s => s.CapabilityRef == "C32").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "C32E6", Name = "Apply criteria for the use of Personal Health Budgets", CapabilityId = capabilities.First(s => s.CapabilityRef == "C32").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "C32E7", Name = "Payments under Personal Health Budgets", CapabilityId = capabilities.First(s => s.CapabilityRef == "C32").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "C32E8", Name = "Maintain directory of equipment, treatments and services", CapabilityId = capabilities.First(s => s.CapabilityRef == "C32").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "C32E9", Name = "Search a directory of equipment, treatments and services", CapabilityId = capabilities.First(s => s.CapabilityRef == "C32").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "C32E10", Name = "Manage multiple budgets", CapabilityId = capabilities.First(s => s.CapabilityRef == "C32").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "C32E11", Name = "Link to Patient Record", CapabilityId = capabilities.First(s => s.CapabilityRef == "C32").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "C32E12", Name = "Link to Workflow", CapabilityId = capabilities.First(s => s.CapabilityRef == "C32").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "C32E13", Name = "Provider view", CapabilityId = capabilities.First(s => s.CapabilityRef == "C32").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "C32E14", Name = "Management Information", CapabilityId = capabilities.First(s => s.CapabilityRef == "C32").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "C32E15", Name = "Identify candidates for Personal Health Budgets", CapabilityId = capabilities.First(s => s.CapabilityRef == "C32").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "C33E1", Name = "Maintain Personal Health Record content", CapabilityId = capabilities.First(s => s.CapabilityRef == "C33").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "C33E2", Name = "Organise Personal Health Record", CapabilityId = capabilities.First(s => s.CapabilityRef == "C33").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "C33E3", Name = "Manage access to Personal Health Record", CapabilityId = capabilities.First(s => s.CapabilityRef == "C33").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "C33E4", Name = "Manage data coming into Personal Health Record", CapabilityId = capabilities.First(s => s.CapabilityRef == "C33").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "C34E1", Name = "Access healthcare data", CapabilityId = capabilities.First(s => s.CapabilityRef == "C34").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "C34E2", Name = "Maintain cohorts", CapabilityId = capabilities.First(s => s.CapabilityRef == "C34").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "C34E3", Name = "Stratify population by risk", CapabilityId = capabilities.First(s => s.CapabilityRef == "C34").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "C34E4", Name = "Data analysis and reporting", CapabilityId = capabilities.First(s => s.CapabilityRef == "C34").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "C34E5", Name = "Outcomes", CapabilityId = capabilities.First(s => s.CapabilityRef == "C34").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "C34E6", Name = "Dashboard", CapabilityId = capabilities.First(s => s.CapabilityRef == "C34").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "C35E1", Name = "Run Risk Stratification algorithms", CapabilityId = capabilities.First(s => s.CapabilityRef == "C35").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "C36E1", Name = "Create Shared Care Plan", CapabilityId = capabilities.First(s => s.CapabilityRef == "C36").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "C36E2", Name = "View Shared Care Plan", CapabilityId = capabilities.First(s => s.CapabilityRef == "C36").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "C36E3", Name = "Amend Shared Care Plan", CapabilityId = capabilities.First(s => s.CapabilityRef == "C36").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "C36E4", Name = "Close Shared Care Plan", CapabilityId = capabilities.First(s => s.CapabilityRef == "C36").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "C36E5", Name = "Assign Shared Care Plan actions", CapabilityId = capabilities.First(s => s.CapabilityRef == "C36").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "C36E6", Name = "Access Shared Care Plans remotely", CapabilityId = capabilities.First(s => s.CapabilityRef == "C36").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "C36E7", Name = "Search and view Shared Care Plans", CapabilityId = capabilities.First(s => s.CapabilityRef == "C36").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "C36E8", Name = "Real-time access to Shared Care Plans", CapabilityId = capabilities.First(s => s.CapabilityRef == "C36").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "C36E9", Name = "Notifications", CapabilityId = capabilities.First(s => s.CapabilityRef == "C36").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "C36E10", Name = "Reports", CapabilityId = capabilities.First(s => s.CapabilityRef == "C36").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "C36E11", Name = "Manage Shared Care Plan templates", CapabilityId = capabilities.First(s => s.CapabilityRef == "C36").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "C36E12", Name = "Manage care schedules", CapabilityId = capabilities.First(s => s.CapabilityRef == "C36").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "C37E1", Name = "Assess wellness or well-being of the Patient or Service User", CapabilityId = capabilities.First(s => s.CapabilityRef == "C37").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "C37E2", Name = "Search the directory", CapabilityId = capabilities.First(s => s.CapabilityRef == "C37").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "C37E3", Name = "Refer Patient/Service User to service(s)", CapabilityId = capabilities.First(s => s.CapabilityRef == "C37").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "C37E4", Name = "Maintain referral record", CapabilityId = capabilities.First(s => s.CapabilityRef == "C37").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "C37E5", Name = "Link to national or local directory of services", CapabilityId = capabilities.First(s => s.CapabilityRef == "C37").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "C37E6", Name = "Maintain directory of services", CapabilityId = capabilities.First(s => s.CapabilityRef == "C37").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "C37E7", Name = "Maintain service criteria", CapabilityId = capabilities.First(s => s.CapabilityRef == "C37").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "C37E8", Name = "Refer Patient/Service User to Link Worker", CapabilityId = capabilities.First(s => s.CapabilityRef == "C37").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "C37E9", Name = "Capture Patient/Service User consent", CapabilityId = capabilities.First(s => s.CapabilityRef == "C37").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "C37E10", Name = "Patient self-referral", CapabilityId = capabilities.First(s => s.CapabilityRef == "C37").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "C37E11", Name = "Integrate Social Prescribing Referral Record with Clinical Record", CapabilityId = capabilities.First(s => s.CapabilityRef == "C37").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "C37E12", Name = "Receive notification of an Appointment", CapabilityId = capabilities.First(s => s.CapabilityRef == "C37").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "C37E13", Name = "Remind Patients/Service Users of Appointments", CapabilityId = capabilities.First(s => s.CapabilityRef == "C37").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "C37E14", Name = "Provide service feedback", CapabilityId = capabilities.First(s => s.CapabilityRef == "C37").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "C37E15", Name = "View service feedback", CapabilityId = capabilities.First(s => s.CapabilityRef == "C37").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "C37E16", Name = "Obtain Management Information (MI) on Social Prescribing", CapabilityId = capabilities.First(s => s.CapabilityRef == "C37").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "C38E1", Name = "Define response to event", CapabilityId = capabilities.First(s => s.CapabilityRef == "C38").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "C38E2", Name = "Monitor and alert", CapabilityId = capabilities.First(s => s.CapabilityRef == "C38").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "C38E3", Name = "Receive alerts", CapabilityId = capabilities.First(s => s.CapabilityRef == "C38").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "C38E4", Name = "Meet availability targets", CapabilityId = capabilities.First(s => s.CapabilityRef == "C38").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "C38E5", Name = "Ease of use", CapabilityId = capabilities.First(s => s.CapabilityRef == "C38").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "C38E6", Name = "Patient/Service Users with sensory impairment(s)", CapabilityId = capabilities.First(s => s.CapabilityRef == "C38").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "C38E7", Name = "Obtain Management Information (MI) on Telecare", CapabilityId = capabilities.First(s => s.CapabilityRef == "C38").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "C38E8", Name = "Enable 2-way communication with Patient/Service User", CapabilityId = capabilities.First(s => s.CapabilityRef == "C38").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "C38E9", Name = "Remote testing of Telecare device", CapabilityId = capabilities.First(s => s.CapabilityRef == "C38").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "C38E10", Name = "Manual testing of Telecare device", CapabilityId = capabilities.First(s => s.CapabilityRef == "C38").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "C38E11", Name = "Sustainability of Telecare device", CapabilityId = capabilities.First(s => s.CapabilityRef == "C38").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "C39E1", Name = "View Unified Care Record", CapabilityId = capabilities.First(s => s.CapabilityRef == "C39").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "C39E2", Name = "Patient/Service User views the Unified Care Record", CapabilityId = capabilities.First(s => s.CapabilityRef == "C39").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "C39E3", Name = "Default Views", CapabilityId = capabilities.First(s => s.CapabilityRef == "C39").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "C40E1", Name = "Verify Medicinal Product Unique Identifiers", CapabilityId = capabilities.First(s => s.CapabilityRef == "C40").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "C40E2", Name = "Decommission Medicinal Products", CapabilityId = capabilities.First(s => s.CapabilityRef == "C40").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "C40E3", Name = "Record the integrity of Anti-tampering Devices", CapabilityId = capabilities.First(s => s.CapabilityRef == "C40").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "C42E1", Name = "Manage Stock in a Dispensary", CapabilityId = capabilities.First(s => s.CapabilityRef == "C42").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "C42E2", Name = "Manage Stock Orders", CapabilityId = capabilities.First(s => s.CapabilityRef == "C42").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "C42E3", Name = "Manage Dispensing tasks for a Dispensary", CapabilityId = capabilities.First(s => s.CapabilityRef == "C42").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "C42E4", Name = "Dispense Medication", CapabilityId = capabilities.First(s => s.CapabilityRef == "C42").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "C42E5", Name = "Manage Dispensaries", CapabilityId = capabilities.First(s => s.CapabilityRef == "C42").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "C42E6", Name = "Manage Endorsements", CapabilityId = capabilities.First(s => s.CapabilityRef == "C42").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "C42E7", Name = "Manage Supplier Accounts", CapabilityId = capabilities.First(s => s.CapabilityRef == "C42").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "C42E8", Name = "View Stock reports", CapabilityId = capabilities.First(s => s.CapabilityRef == "C42").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "C45E1", Name = "Identify COVID-19 vaccination cohorts", CapabilityId = capabilities.First(s => s.CapabilityRef == "C45").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "C45E2", Name = "Verify Patient information using Personal Demographics Service (PDS)", CapabilityId = capabilities.First(s => s.CapabilityRef == "C45").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "C45E3", Name = "Import or consume COVID-19 vaccination data for Patients", CapabilityId = capabilities.First(s => s.CapabilityRef == "C45").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "C45E4", Name = "Extract COVID-19 vaccination cohorts data in .CSV file format", CapabilityId = capabilities.First(s => s.CapabilityRef == "C45").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "C45E5", Name = "Bulk send SMS messages for COVID-19 vaccination invite communications", CapabilityId = capabilities.First(s => s.CapabilityRef == "C45").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "C45E6", Name = "Bulk create letters for COVID-19 vaccination invite communications", CapabilityId = capabilities.First(s => s.CapabilityRef == "C45").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "C45E7", Name = "Bulk send email for COVID-19 vaccination invite communications", CapabilityId = capabilities.First(s => s.CapabilityRef == "C45").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "C45E8", Name = "Automatically record which Patients have had COVID-19 vaccination invites created", CapabilityId = capabilities.First(s => s.CapabilityRef == "C45").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "C45E9", Name = "View whether Patients have had a COVID-19 vaccination invite communication created", CapabilityId = capabilities.First(s => s.CapabilityRef == "C45").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "C46E1", Name = "Define appointment availability for COVID-19 vaccination site", CapabilityId = capabilities.First(s => s.CapabilityRef == "C46").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "C46E2", Name = "Book COVID-19 vaccination appointments for eligible Patients registered across different GP Practices", CapabilityId = capabilities.First(s => s.CapabilityRef == "C46").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "C46E3", Name = "Record that a COVID-19 vaccination appointment for a Patient has been completed", CapabilityId = capabilities.First(s => s.CapabilityRef == "C46").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "C46E4", Name = "Extract COVID-19 vaccination appointments data for NHS Digital", CapabilityId = capabilities.First(s => s.CapabilityRef == "C46").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "C46E5", Name = "Import COVID-19 vaccination Patient cohorts data via .CSV file", CapabilityId = capabilities.First(s => s.CapabilityRef == "C46").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "C46E6", Name = "Verify Patient information using Personal Demographics Service (PDS)", CapabilityId = capabilities.First(s => s.CapabilityRef == "C46").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "C46E7", Name = "Bulk send SMS messages for COVID-19 vaccination invite communications", CapabilityId = capabilities.First(s => s.CapabilityRef == "C46").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "C46E8", Name = "Bulk create letters for COVID-19 vaccination invite communications", CapabilityId = capabilities.First(s => s.CapabilityRef == "C46").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "C46E9", Name = "Bulk send email for COVID-19 vaccination invite communications", CapabilityId = capabilities.First(s => s.CapabilityRef == "C46").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "C46E10", Name = "Automatically record which Patients have had COVID-19 vaccination invites created", CapabilityId = capabilities.First(s => s.CapabilityRef == "C46").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "C46E11", Name = "View whether Patients have had a COVID-19 vaccination invite communication created", CapabilityId = capabilities.First(s => s.CapabilityRef == "C46").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "C46E12", Name = "Automatically bulk send booking reminders to Patients via SMS messages for COVID-19 vaccination invites", CapabilityId = capabilities.First(s => s.CapabilityRef == "C46").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "C46E13", Name = "Automatically bulk create booking reminders to Patients as letters for COVID-19 vaccination invites", CapabilityId = capabilities.First(s => s.CapabilityRef == "C46").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "C46E14", Name = "Automatically bulk send booking reminders to Patients via email for COVID-19 vaccination invites", CapabilityId = capabilities.First(s => s.CapabilityRef == "C46").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "C46E15", Name = "Book Appointments across Solutions using GP Connect Appointments Management", CapabilityId = capabilities.First(s => s.CapabilityRef == "C46").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "C46E16", Name = "Patients can book their own COVID-19 vaccination appointments", CapabilityId = capabilities.First(s => s.CapabilityRef == "C46").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "C46E17", Name = "Patients can re-schedule their own future COVID-19 vaccination appointment", CapabilityId = capabilities.First(s => s.CapabilityRef == "C46").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "C46E18", Name = "Patients can cancel their own future COVID-19 vaccination appointment", CapabilityId = capabilities.First(s => s.CapabilityRef == "C46").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "C46E19", Name = "Automatically send booking notifications to Patients via SMS messages for COVID-19 vaccination appointments", CapabilityId = capabilities.First(s => s.CapabilityRef == "C46").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "C46E20", Name = "Automatically create booking notifications to Patients as letters for COVID-19 vaccination appointments", CapabilityId = capabilities.First(s => s.CapabilityRef == "C46").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "C46E21", Name = "Automatically send booking notifications to Patients via email for COVID-19 vaccination appointments", CapabilityId = capabilities.First(s => s.CapabilityRef == "C46").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "C46E22", Name = "Create ad-hoc booking notifications to Patients for COVID-19 vaccination appointments", CapabilityId = capabilities.First(s => s.CapabilityRef == "C46").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "C46E23", Name = "Automatically bulk send appointment reminders to Patients via SMS messages for COVID-19 vaccination appointments", CapabilityId = capabilities.First(s => s.CapabilityRef == "C46").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "C46E24", Name = "Automatically bulk create booking reminders to Patients as letters for COVID-19 vaccination appointments", CapabilityId = capabilities.First(s => s.CapabilityRef == "C46").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "C46E25", Name = "Automatically bulk send appointment reminders to Patients via email for COVID-19 vaccination appointments", CapabilityId = capabilities.First(s => s.CapabilityRef == "C46").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "C46E26", Name = "Send ad-hoc appointment reminders to Patients for COVID-19 vaccination appointments", CapabilityId = capabilities.First(s => s.CapabilityRef == "C46").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "C46E27", Name = "View all booked COVID-19 vaccination appointments for a specified time period", CapabilityId = capabilities.First(s => s.CapabilityRef == "C46").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "C46E28", Name = "Export all booked COVID-19 vaccination appointments for a specified time period", CapabilityId = capabilities.First(s => s.CapabilityRef == "C46").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "C46E29", Name = "Cancel booked COVID-19 vaccination appointments for Patients", CapabilityId = capabilities.First(s => s.CapabilityRef == "C46").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "C46E30", Name = "Re-schedule booked COVID-19 vaccination appointments for Patients", CapabilityId = capabilities.First(s => s.CapabilityRef == "C46").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "C46E31", Name = "Automatically send appointment cancellation notifications to Patients via SMS messages for COVID-19 appointments", CapabilityId = capabilities.First(s => s.CapabilityRef == "C46").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "C46E32", Name = "Automatically create appointment cancellation notifications to Patients as letters for COVID-19 appointments", CapabilityId = capabilities.First(s => s.CapabilityRef == "C46").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "C46E33", Name = "Automatically send appointment cancellation notifications to Patients via email for COVID-19 appointments", CapabilityId = capabilities.First(s => s.CapabilityRef == "C46").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "C47E1", Name = "Record structured COVID-19 vaccination data at the point of care for Patients registered at different GP Practices", CapabilityId = capabilities.First(s => s.CapabilityRef == "C47").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "C47E2", Name = "Record structured COVID-19 adverse reaction data at the point of care for Patients registered at different GP Practices", CapabilityId = capabilities.First(s => s.CapabilityRef == "C47").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "C47E3", Name = "Extract COVID-19 vaccination data for NHS Digital Daily Clinical Vaccination Extract", CapabilityId = capabilities.First(s => s.CapabilityRef == "C47").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "C47E4", Name = "Extract COVID-19 adverse reaction data for NHS Digital Daily Clinical Adverse Reaction Extract", CapabilityId = capabilities.First(s => s.CapabilityRef == "C47").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "C47E5", Name = "Automatically send vaccination data to Patient’s registered GP Practice Foundation Solution using Digital Medicines FHIR messages", CapabilityId = capabilities.First(s => s.CapabilityRef == "C47").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "C47E6", Name = "Automatically send COVID-19 adverse reaction data to Patient’s registered GP Practice Foundation Solution using Digital Medicines FHIR messages", CapabilityId = capabilities.First(s => s.CapabilityRef == "C47").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "C47E7", Name = "Automatically send COVID-19 vaccination data to the NHS Business Services Authority (NHSBSA)", CapabilityId = capabilities.First(s => s.CapabilityRef == "C47").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "C47E8", Name = "View information from the GP Patient Record using GP Connect Access Record HTML", CapabilityId = capabilities.First(s => s.CapabilityRef == "C47").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "C47E9", Name = "View information from the GP Patient Record held by the same Solution", CapabilityId = capabilities.First(s => s.CapabilityRef == "C47").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "C47E10", Name = "View Summary Care Record (SCR) for a Patient", CapabilityId = capabilities.First(s => s.CapabilityRef == "C47").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "C47E11", Name = "Scanning of a GS1 barcode when recording vaccination data", CapabilityId = capabilities.First(s => s.CapabilityRef == "C47").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "C47E12", Name = "Record structured COVID-19 vaccination data at the point of care directly into GP Patient Record", CapabilityId = capabilities.First(s => s.CapabilityRef == "C47").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "C47E13", Name = "Record structured COVID-19 adverse reaction data at the point of care directly into GP Patient Record", CapabilityId = capabilities.First(s => s.CapabilityRef == "C47").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "C47E14", Name = "Verify Patient information using Personal Demographics Service (PDS)", CapabilityId = capabilities.First(s => s.CapabilityRef == "C47").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "C47E15", Name = "Latest COVID-19 Clinical Screening Questions at the point of care for Patients registered at different GP Practices", CapabilityId = capabilities.First(s => s.CapabilityRef == "C47").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "C47E16", Name = "Record structured COVID-19 vaccination data at the point of care for Patients using pre-configured vaccine batches", CapabilityId = capabilities.First(s => s.CapabilityRef == "C47").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "C47E17", Name = "View vaccination information for a Patient held by the National Immunisation Management Service (NIMS) at point of care", CapabilityId = capabilities.First(s => s.CapabilityRef == "C47").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "C47E18", Name = "Update previously recorded structured COVID-19 vaccination and adverse reaction data for Patients", CapabilityId = capabilities.First(s => s.CapabilityRef == "C47").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "C47E19", Name = "Extract COVID-19 Extended Attributes data for NHS Digital Extended Attributes Extract", CapabilityId = capabilities.First(s => s.CapabilityRef == "C47").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "C47E20", Name = "View reports on COVID-19 vaccination data", CapabilityId = capabilities.First(s => s.CapabilityRef == "C47").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "E00001", Name = "Conduct Online Consultation", CapabilityId = capabilities.First(s => s.CapabilityRef == "C43").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "E00002", Name = "Conduct Online Consultation with a Proxy", CapabilityId = capabilities.First(s => s.CapabilityRef == "C43").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "E00003", Name = "Patient/Service User requests for Online Consultation support and provides information", CapabilityId = capabilities.First(s => s.CapabilityRef == "C43").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "E00004", Name = "Proxy requests for Online Consultation support and provides information", CapabilityId = capabilities.First(s => s.CapabilityRef == "C43").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "E00005", Name = "Respond to Online Consultation requests for support from Patients/Service Users", CapabilityId = capabilities.First(s => s.CapabilityRef == "C43").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "E00006", Name = "Respond to Online Consultation requests for support from Proxies", CapabilityId = capabilities.First(s => s.CapabilityRef == "C43").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "E00007", Name = "Include attachments in Online Consultation requests", CapabilityId = capabilities.First(s => s.CapabilityRef == "C43").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "E00008", Name = "Include attachments in Online Consultation requests from a Proxy", CapabilityId = capabilities.First(s => s.CapabilityRef == "C43").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "E00009", Name = "Automated response to Online Consultation requests for support from Patients/Service Users", CapabilityId = capabilities.First(s => s.CapabilityRef == "C43").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "E00010", Name = "Automated response to Online Consultation requests for support from Proxies", CapabilityId = capabilities.First(s => s.CapabilityRef == "C43").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "E00011", Name = "Patient/Service User makes an administrative request", CapabilityId = capabilities.First(s => s.CapabilityRef == "C43").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "E00012", Name = "Proxy makes an administrative request", CapabilityId = capabilities.First(s => s.CapabilityRef == "C43").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "E00013", Name = "Respond to administrative requests for support from Patients/Service Users", CapabilityId = capabilities.First(s => s.CapabilityRef == "C43").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "E00014", Name = "Respond to administrative requests for support from Proxies", CapabilityId = capabilities.First(s => s.CapabilityRef == "C43").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "E00015", Name = "Automated responses to administrative requests from Patients/Service Users", CapabilityId = capabilities.First(s => s.CapabilityRef == "C43").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "E00016", Name = "Automated responses to administrative requests from Proxies", CapabilityId = capabilities.First(s => s.CapabilityRef == "C43").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "E00017", Name = "Link Online Consultation requests for support and responses", CapabilityId = capabilities.First(s => s.CapabilityRef == "C43").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "E00018", Name = "Link Online Consultation requests for support from a Proxy and responses", CapabilityId = capabilities.First(s => s.CapabilityRef == "C43").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "E00019", Name = "Self-help and signposting", CapabilityId = capabilities.First(s => s.CapabilityRef == "C43").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "E00020", Name = "Proxy supporting self-help and signposting", CapabilityId = capabilities.First(s => s.CapabilityRef == "C43").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "E00021", Name = "Symptom checking", CapabilityId = capabilities.First(s => s.CapabilityRef == "C43").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "E00022", Name = "Symptom checking by a Proxy", CapabilityId = capabilities.First(s => s.CapabilityRef == "C43").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "E00023", Name = "Direct Messaging", CapabilityId = capabilities.First(s => s.CapabilityRef == "C43").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "E00024", Name = "Direct Messaging by a Proxy", CapabilityId = capabilities.First(s => s.CapabilityRef == "C43").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "E00025", Name = "View the Patient Record during Online Consultation", CapabilityId = capabilities.First(s => s.CapabilityRef == "C43").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "E00026", Name = "Electronically share files during Direct Messaging", CapabilityId = capabilities.First(s => s.CapabilityRef == "C43").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "E00027", Name = "Electronically share files during Direct Messaging with a Proxy", CapabilityId = capabilities.First(s => s.CapabilityRef == "C43").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "E00028", Name = "Customisation of report", CapabilityId = capabilities.First(s => s.CapabilityRef == "C43").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "E00029", Name = "Report on utilisation of Online Consultation requests for support", CapabilityId = capabilities.First(s => s.CapabilityRef == "C43").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "E00030", Name = "Report on outcomes or dispositions provided to the Patient/Service User", CapabilityId = capabilities.First(s => s.CapabilityRef == "C43").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "E00031", Name = "Report on the status of Online Consultations", CapabilityId = capabilities.First(s => s.CapabilityRef == "C43").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "E00032", Name = "Report on Patient demographics using Online Consultation", CapabilityId = capabilities.First(s => s.CapabilityRef == "C43").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "E00033", Name = "Manually prioritise Online Consultation requests for support", CapabilityId = capabilities.First(s => s.CapabilityRef == "C43").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "E00034", Name = "Assign Online Consultation requests to a Health or Care Professional manually", CapabilityId = capabilities.First(s => s.CapabilityRef == "C43").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "E00035", Name = "Categorise outcome of Online Consultation requests", CapabilityId = capabilities.First(s => s.CapabilityRef == "C43").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "E00037", Name = "Automatically prioritise Online Consultation requests", CapabilityId = capabilities.First(s => s.CapabilityRef == "C43").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "E00038", Name = "Assign Online Consultation requests to Health or Care Professional automatically", CapabilityId = capabilities.First(s => s.CapabilityRef == "C43").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "E00056", Name = "Disable and enable Direct Messaging for a Healthcare Organisation", CapabilityId = capabilities.First(s => s.CapabilityRef == "C43").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "E00057", Name = "Disable and enable Direct Messaging for a Patient/Service User", CapabilityId = capabilities.First(s => s.CapabilityRef == "C43").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "E00058", Name = "Disable and enable electronic file sharing during Direct Messaging for a Healthcare Organisation", CapabilityId = capabilities.First(s => s.CapabilityRef == "C43").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "E00075", Name = "Patient/Service User feedback for Online Consultation", CapabilityId = capabilities.First(s => s.CapabilityRef == "C43").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "E00076", Name = "Record Online Consultation outcome to the Patient Record", CapabilityId = capabilities.First(s => s.CapabilityRef == "C43").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "E00077", Name = "Retain attachments (file and images) in the Patient Record", CapabilityId = capabilities.First(s => s.CapabilityRef == "C43").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "E00078", Name = "Verify Patient/Service User details against Personal Demographics Service (PDS)", CapabilityId = capabilities.First(s => s.CapabilityRef == "C43").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "E00079", Name = "SNOMED code Online Consultation", CapabilityId = capabilities.First(s => s.CapabilityRef == "C43").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "E00080", Name = "Customisation of the question sets for Patients/Service Users requesting Online Consultation support", CapabilityId = capabilities.First(s => s.CapabilityRef == "C43").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "E00081", Name = "Accessibility options for Online Consultation", CapabilityId = capabilities.First(s => s.CapabilityRef == "C43").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "E00082", Name = "Notification to Patients/Service Users", CapabilityId = capabilities.First(s => s.CapabilityRef == "C43").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "E00083", Name = "Customisation of instructions to Patients/Service Users using Online Consultation Solution", CapabilityId = capabilities.First(s => s.CapabilityRef == "C43").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "E00084", Name = "Categorise administration requests", CapabilityId = capabilities.First(s => s.CapabilityRef == "C43").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "E00085", Name = "Disable and enable Direct Messaging for an Online Consultation request for support", CapabilityId = capabilities.First(s => s.CapabilityRef == "C43").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "E00086", Name = "Configuration of the triage process", CapabilityId = capabilities.First(s => s.CapabilityRef == "C43").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "E00089", Name = "Save the complete record of an Online Consultation to the Patient Record", CapabilityId = capabilities.First(s => s.CapabilityRef == "C43").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "E00090", Name = "Health or Care Professional initiates an Online Consultations request", CapabilityId = capabilities.First(s => s.CapabilityRef == "C43").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "E00091", Name = "Proxy Verification", CapabilityId = capabilities.First(s => s.CapabilityRef == "C43").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "E00039", Name = "Conduct Video Consultation", CapabilityId = capabilities.First(s => s.CapabilityRef == "C44").Id, CompliancyLevel = CompliancyLevel.Must, IsActive = true },
                new Epic { Id = "E00040", Name = "Conduct Video Consultation with a Proxy", CapabilityId = capabilities.First(s => s.CapabilityRef == "C44").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "E00041", Name = "Conduct Video Consultation with the Patient/Service User without registration", CapabilityId = capabilities.First(s => s.CapabilityRef == "C44").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "E00042", Name = "Conduct Video Consultation with a Proxy without registration", CapabilityId = capabilities.First(s => s.CapabilityRef == "C44").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "E00043", Name = "End Video Consultation with a Patient/Service User", CapabilityId = capabilities.First(s => s.CapabilityRef == "C44").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "E00045", Name = "Direct Messaging during a Video Consultation", CapabilityId = capabilities.First(s => s.CapabilityRef == "C44").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "E00047", Name = "View the Patient Record during Video Consultation", CapabilityId = capabilities.First(s => s.CapabilityRef == "C44").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "E00048", Name = "Conduct group Video Consultations", CapabilityId = capabilities.First(s => s.CapabilityRef == "C44").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "E00051", Name = "Electronically share files during a Video Consultation", CapabilityId = capabilities.First(s => s.CapabilityRef == "C44").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "E00053", Name = "Health or Care Professional can share their screen during a Video Consultation", CapabilityId = capabilities.First(s => s.CapabilityRef == "C44").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "E00055", Name = "Record Video Consultation outcome to the Patient record", CapabilityId = capabilities.First(s => s.CapabilityRef == "C44").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "E00059", Name = "Health or Care Professional can record a Video Consultation", CapabilityId = capabilities.First(s => s.CapabilityRef == "C44").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "E00060", Name = "Patient/Service User can record a Video Consultation", CapabilityId = capabilities.First(s => s.CapabilityRef == "C44").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "E00061", Name = "Accessibility options", CapabilityId = capabilities.First(s => s.CapabilityRef == "C44").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "E00062", Name = "Waiting room", CapabilityId = capabilities.First(s => s.CapabilityRef == "C44").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "E00063", Name = "Disable and enable Direct Messaging during a Video Consultation for the Patient/Service User", CapabilityId = capabilities.First(s => s.CapabilityRef == "C44").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "E00064", Name = "Record Direct Messages to the Patient Record", CapabilityId = capabilities.First(s => s.CapabilityRef == "C44").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "E00065", Name = "Patient/Service User name is not automatically visible in a group Video Consultation", CapabilityId = capabilities.First(s => s.CapabilityRef == "C44").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "E00066", Name = "Invite new participants to an existing Video Consultation with a Patient/Service User", CapabilityId = capabilities.First(s => s.CapabilityRef == "C44").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "E00067", Name = "Disable and enable electronic file sharing during a Video Consultation", CapabilityId = capabilities.First(s => s.CapabilityRef == "C44").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "E00068", Name = "Disable and enable screen sharing during a Video Consultation", CapabilityId = capabilities.First(s => s.CapabilityRef == "C44").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "E00069", Name = "Patient/Service User feedback on Video Consultations", CapabilityId = capabilities.First(s => s.CapabilityRef == "C44").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "E00070", Name = "Test the Video Consultation settings", CapabilityId = capabilities.First(s => s.CapabilityRef == "C44").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "E00071", Name = "Consecutive consultations with multiple Patients/Service Users via a single Video Consultation", CapabilityId = capabilities.First(s => s.CapabilityRef == "C44").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "E00072", Name = "Reminder of upcoming or scheduled Video Consultation", CapabilityId = capabilities.First(s => s.CapabilityRef == "C44").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "E00073", Name = "Disable and enable audio during a group Video Consultation", CapabilityId = capabilities.First(s => s.CapabilityRef == "C44").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "E00074", Name = "Disable and enable video during a group Video Consultation", CapabilityId = capabilities.First(s => s.CapabilityRef == "C44").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "E00087", Name = "Retain attachments (file and images) received during Video Consultation in the Patient Record", CapabilityId = capabilities.First(s => s.CapabilityRef == "C44").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "E00088", Name = "SNOMED code Video Consultation", CapabilityId = capabilities.First(s => s.CapabilityRef == "C44").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true },
                new Epic { Id = "E00099", Name = "User Friendly", CapabilityId = capabilities.First(s => s.CapabilityRef == "C43").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true, SupplierDefined = true },
                new Epic { Id = "E123456", Name = "SupplierDefinedEpic 123", CapabilityId = capabilities.First(s => s.CapabilityRef == "C2").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true, SupplierDefined = true },
                new Epic { Id = "S00001", Name = "SupplierDefinedEpic New Format 1", Description = "SDE Description", CapabilityId = capabilities.First(s => s.CapabilityRef == "C44").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true, SupplierDefined = true },
                new Epic { Id = "S00002", Name = "SupplierDefinedEpic New Format 2", Description = "SDE Description", CapabilityId = capabilities.First(s => s.CapabilityRef == "C43").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true, SupplierDefined = true },
                new Epic { Id = "S00003", Name = "SupplierDefinedEpic New Format 3", Description = "SDE Description", CapabilityId = capabilities.First(s => s.CapabilityRef == "C43").Id, CompliancyLevel = CompliancyLevel.May, IsActive = false, SupplierDefined = true },
                new Epic { Id = "S00004", Name = "SupplierDefinedEpic New Format 4", Description = "SDE Description", CapabilityId = capabilities.First(s => s.CapabilityRef == "C2").Id, CompliancyLevel = CompliancyLevel.May, IsActive = false, SupplierDefined = true },
                new Epic { Id = "S00005", Name = "SupplierDefinedEpic New Format 5", Description = "SDE Description", CapabilityId = capabilities.First(s => s.CapabilityRef == "C44").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true, SupplierDefined = true },
                new Epic { Id = "S00006", Name = "SupplierDefinedEpic New Format 6", Description = "SDE Description", CapabilityId = capabilities.First(s => s.CapabilityRef == "C43").Id, CompliancyLevel = CompliancyLevel.May, IsActive = true, SupplierDefined = true },
                new Epic { Id = "S00007", Name = "SupplierDefinedEpic New Format 7", Description = "SDE Description", CapabilityId = capabilities.First(s => s.CapabilityRef == "C43").Id, CompliancyLevel = CompliancyLevel.May, IsActive = false, SupplierDefined = true },
            };
            context.AddRange(epics);

            List<FrameworkCapability> frameworkCapabilities = new()
            {
                new FrameworkCapability { CapabilityId = 1, FrameworkId = "NHSDGP001" },
                new FrameworkCapability { CapabilityId = 2, FrameworkId = "NHSDGP001" },
                new FrameworkCapability { CapabilityId = 3, FrameworkId = "NHSDGP001" },
                new FrameworkCapability { CapabilityId = 4, FrameworkId = "NHSDGP001" },
                new FrameworkCapability { CapabilityId = 5, FrameworkId = "NHSDGP001", IsFoundation = true, },
                new FrameworkCapability { CapabilityId = 6, FrameworkId = "NHSDGP001" },
                new FrameworkCapability { CapabilityId = 7, FrameworkId = "NHSDGP001" },
                new FrameworkCapability { CapabilityId = 8, FrameworkId = "NHSDGP001" },
                new FrameworkCapability { CapabilityId = 9, FrameworkId = "NHSDGP001" },
                new FrameworkCapability { CapabilityId = 10, FrameworkId = "NHSDGP001" },
                new FrameworkCapability { CapabilityId = 11, FrameworkId = "NHSDGP001", IsFoundation = true, },
                new FrameworkCapability { CapabilityId = 12, FrameworkId = "NHSDGP001", IsFoundation = true, },
                new FrameworkCapability { CapabilityId = 13, FrameworkId = "NHSDGP001", IsFoundation = true, },
                new FrameworkCapability { CapabilityId = 14, FrameworkId = "NHSDGP001", IsFoundation = true, },
                new FrameworkCapability { CapabilityId = 15, FrameworkId = "NHSDGP001", IsFoundation = true, },
                new FrameworkCapability { CapabilityId = 16, FrameworkId = "NHSDGP001" },
                new FrameworkCapability { CapabilityId = 17, FrameworkId = "NHSDGP001" },
                new FrameworkCapability { CapabilityId = 18, FrameworkId = "NHSDGP001" },
                new FrameworkCapability { CapabilityId = 19, FrameworkId = "NHSDGP001" },
                new FrameworkCapability { CapabilityId = 20, FrameworkId = "NHSDGP001" },
                new FrameworkCapability { CapabilityId = 21, FrameworkId = "NHSDGP001" },
                new FrameworkCapability { CapabilityId = 22, FrameworkId = "NHSDGP001" },
                new FrameworkCapability { CapabilityId = 23, FrameworkId = "NHSDGP001" },
                new FrameworkCapability { CapabilityId = 24, FrameworkId = "NHSDGP001" },
                new FrameworkCapability { CapabilityId = 25, FrameworkId = "NHSDGP001" },
                new FrameworkCapability { CapabilityId = 26, FrameworkId = "NHSDGP001" },
                new FrameworkCapability { CapabilityId = 27, FrameworkId = "NHSDGP001" },
                new FrameworkCapability { CapabilityId = 29, FrameworkId = "NHSDGP001" },
                new FrameworkCapability { CapabilityId = 30, FrameworkId = "NHSDGP001" },
                new FrameworkCapability { CapabilityId = 32, FrameworkId = "NHSDGP001" },
                new FrameworkCapability { CapabilityId = 33, FrameworkId = "NHSDGP001" },
                new FrameworkCapability { CapabilityId = 34, FrameworkId = "NHSDGP001" },
                new FrameworkCapability { CapabilityId = 35, FrameworkId = "NHSDGP001" },
                new FrameworkCapability { CapabilityId = 36, FrameworkId = "NHSDGP001" },
                new FrameworkCapability { CapabilityId = 37, FrameworkId = "NHSDGP001" },
                new FrameworkCapability { CapabilityId = 38, FrameworkId = "NHSDGP001" },
                new FrameworkCapability { CapabilityId = 39, FrameworkId = "NHSDGP001" },
                new FrameworkCapability { CapabilityId = 40, FrameworkId = "NHSDGP001" },
                new FrameworkCapability { CapabilityId = 41, FrameworkId = "NHSDGP001" },
                new FrameworkCapability { CapabilityId = 42, FrameworkId = "NHSDGP001" },
                new FrameworkCapability { CapabilityId = 43, FrameworkId = "DFOCVC001" },
                new FrameworkCapability { CapabilityId = 44, FrameworkId = "DFOCVC001" },
            };
            context.AddRange(frameworkCapabilities);

            // ProvisioningType
            List<Database.Models.ProvisioningType> provisioningTypes = new()
            {
                new Database.Models.ProvisioningType { Id = 1, Name = "Patient" },
                new Database.Models.ProvisioningType { Id = 2, Name = "Declarative" },
                new Database.Models.ProvisioningType { Id = 3, Name = "OnDemand" },
            };
            context.AddRange(provisioningTypes);

            // PublicationStatus
            List<Database.Models.PublicationStatus> publicationStatuses = new()
            {
                new Database.Models.PublicationStatus { Id = 1, Name = "Draft" },
                new Database.Models.PublicationStatus { Id = 2, Name = "Unpublished" },
                new Database.Models.PublicationStatus { Id = 3, Name = "Published" },
                new Database.Models.PublicationStatus { Id = 4, Name = "Withdrawn" },
            };

            context.AddRange(publicationStatuses);

            // SolutionCapabilityStatus
            List<CatalogueItemCapabilityStatus> solutionCapabilityStatuses = new()
            {
                new CatalogueItemCapabilityStatus { Id = 1, Pass = true, Name = "Passed – Full" },
                new CatalogueItemCapabilityStatus { Id = 2, Pass = true, Name = "Passed – Partial" },
                new CatalogueItemCapabilityStatus { Id = 3, Pass = false, Name = "Failed" },
            };
            context.AddRange(solutionCapabilityStatuses);

            // SolutionEpicStatus
            List<CatalogueItemEpicStatus> solutionEpicStatuses = new()
            {
                new CatalogueItemEpicStatus { Id = 1, IsMet = true, Name = "Passed" },
                new CatalogueItemEpicStatus { Id = 2, IsMet = false, Name = "Not Evidenced" },
                new CatalogueItemEpicStatus { Id = 3, IsMet = false, Name = "Failed" },
            };
            context.AddRange(solutionEpicStatuses);

            // TimeUnit
            List<Database.Models.TimeUnit> timeUnits = new()
            {
                new Database.Models.TimeUnit { Id = 1, Name = "month", Description = "per month" },
                new Database.Models.TimeUnit { Id = 2, Name = "year", Description = "per year" },
            };
            context.AddRange(timeUnits);

            // Standards
            List<Standard> standards = new()
            {
                new Standard { Id = "S24", Name = "Business Continuity and Disaster Recovery", Description = "Ensures that solutions are supported by robust business continuity plans and disaster recovery measures.", Url = "https://gpitbjss.atlassian.net/wiki/spaces/GPITF/pages/1391134058/Business+Continuity+and+Disaster+Recovery", StandardType = StandardType.Overarching },
                new Standard { Id = "S25", Name = "Clinical Safety", Description = "Supports the management of clinical risk and patient safety.", Url = "https://gpitbjss.atlassian.net/wiki/spaces/GPITF/pages/1391134225/Clinical+Safety", StandardType = StandardType.Overarching },
                new Standard { Id = "S31", Name = "Commercial", Description = "Underpins all commercial activity relating to the Buying Catalogue by defining rules governing commercial relationships and setting out standards of behaviour.", Url = "https://gpitbjss.atlassian.net/wiki/spaces/GPITF/pages/1391134121/Commercial", StandardType = StandardType.Overarching },
                new Standard { Id = "S26", Name = "Data Migration", Description = "Supports the safe and effective migration of data if a buyer changes from one solution to another.", Url = "https://gpitbjss.atlassian.net/wiki/spaces/GPITF/pages/1391134456/Data+Migration", StandardType = StandardType.Overarching },
                new Standard { Id = "S27", Name = "Data Standards", Description = "Defines detailed technical standards for the storage, management and organisation of data and specifies standardised reference data, terminology and codes.", Url = "https://gpitbjss.atlassian.net/wiki/spaces/GPITF/pages/1391134407/Data+Standards", StandardType = StandardType.Overarching },
                new Standard { Id = "S29", Name = "Hosting & Infrastructure", Description = "Supports best practices for infrastructure and hosting of systems. For example, ensuring systems are cost effective, secure and energy efficient.", Url = "https://gpitbjss.atlassian.net/wiki/spaces/GPITF/pages/1391134324/Hosting+Infrastructure", StandardType = StandardType.Overarching },
                new Standard { Id = "S30", Name = "Information Governance", Description = "Supports the controls needed to ensure that sensitive personal data is kept confidential, is accurate and is available to authorised users when required.", Url = "https://gpitbjss.atlassian.net/wiki/spaces/GPITF/pages/1391134230/Information+Governance", StandardType = StandardType.Overarching },
                new Standard { Id = "S63", Name = "Non-Functional Questions", Description = "Enables NHS Digital to assess the risk associated with the assessment of a solution against other overarching Standards.", Url = "https://gpitbjss.atlassian.net/wiki/spaces/GPITF/pages/1569980472/Non-Functional+Questions", StandardType = StandardType.Overarching },
                new Standard { Id = "S65", Name = "Service Management", Description = "Supports suppliers in the delivery and management of services that enable their solutions to continue working.", Url = "https://gpitbjss.atlassian.net/wiki/spaces/GPITF/pages/1391133802/Service+Management", StandardType = StandardType.Overarching },
                new Standard { Id = "S69", Name = "Testing", Description = "Ensures that a suppliers’ software delivery test processes are of sufficient quality and rigour.", Url = "https://gpitbjss.atlassian.net/wiki/spaces/GPITF/pages/1391133583/Testing", StandardType = StandardType.Overarching },
                new Standard { Id = "S28", Name = "Training", Description = "Enables NHS Digital to assess the risk associated with the assessment of a solution against other overarching Standards.", Url = "https://gpitbjss.atlassian.net/wiki/spaces/GPITF/pages/1391133846/Training", StandardType = StandardType.Overarching },
                new Standard { Id = "S33", Name = "111", Description = "Supports the receiving and consuming of data sent by 111 provider systems for transfer of care to a GP practice.", Url = "https://gpitbjss.atlassian.net/wiki/spaces/GPITF/pages/1391133945/111", StandardType = StandardType.Interoperability },
                new Standard { Id = "S34", Name = "Digital Diagnostics & Pathology Messaging", Description = "Supports the transmission and validation of results data to GP systems where a test request has been placed with a laboratory.", Url = "https://gpitbjss.atlassian.net/wiki/spaces/GPITF/pages/1391133752/Digital+Diagnostics+Pathology+Messaging", StandardType = StandardType.Interoperability },
                new Standard { Id = "S77", Name = "Digital Medicines and Pharmacy FHIR Payload", Description = "Supports the receiving and consuming of data sent by a pharmacy to a GP practice relating to administration of immunisations or supply of emergency medication to patients", Url = "https://gpitbjss.atlassian.net/wiki/spaces/GPITF/pages/10752983078/Digital+Medicines+and+Pharmacy+FHIR+Payload", StandardType = StandardType.Interoperability },
                new Standard { Id = "S35", Name = "e-Referrals Service (e-RS)", Description = "Supports electronic booking of hospital or clinic appointments, with a choice of place, date and time.", Url = "https://gpitbjss.atlassian.net/wiki/spaces/GPITF/pages/1391135075/eMED3+Fit+Notes", StandardType = StandardType.Interoperability },
                new Standard { Id = "S36", Name = "Electronic Prescription Service (EPS)", Description = "Supports the sending of prescriptions electronically to a dispenser, for example a pharmacy, of the patient's choice.", Url = "https://gpitbjss.atlassian.net/wiki/spaces/GPITF/pages/1391133858/Electronic+Prescription+Service+EPS+-+Prescribing", StandardType = StandardType.Interoperability },
                new Standard { Id = "S37", Name = "Electronic Yellow Card Reporting", Description = "Supports electronic reporting of suspected adverse drug reactions.", Url = "https://gpitbjss.atlassian.net/wiki/spaces/GPITF/pages/1391134080/Electronic+Yellow+Card+Reporting", StandardType = StandardType.Interoperability },
                new Standard { Id = "S39", Name = "eMED3 (Fit Notes)", Description = "Supports the creation of eMED3 data, its integration into a Patient Record, printing MED3 fit note certificates and the extraction of data to NHS Digital.", Url = "https://gpitbjss.atlassian.net/wiki/spaces/GPITF/pages/1391135075/eMED3+Fit+Notes", StandardType = StandardType.Interoperability },
                new Standard { Id = "S43", Name = "GP2GP", Description = "Supports sharing of data held within GP IT solutions across health and social care organisations.", Url = "https://gpitbjss.atlassian.net/wiki/spaces/GPITF/pages/1391134336/GP2GP", StandardType = StandardType.Interoperability },
                new Standard { Id = "S44", Name = "GP Connect", Description = "Supports sharing of data held within GP IT solutions across health and social care organisations.", Url = "https://gpitbjss.atlassian.net/wiki/spaces/GPITF/pages/1391133650/GP+Connect", StandardType = StandardType.Interoperability },
                new Standard { Id = "S78", Name = "GP Data for Planning and Research", Description = "Supports the extraction and submission of GP data, for health and social care purposes (including planning, policy development, public health, commissioning and research) to NHS Digital.", Url = "https://gpitbjss.atlassian.net/wiki/spaces/GPITF/pages/11235426957/GP+Data+for+Planning+and+Research", StandardType = StandardType.Interoperability },
                new Standard { Id = "S46", Name = "GPES Data Extraction", Description = "Supports secure collection of information from GP practices and its delivery to approved organisations.", Url = "https://gpitbjss.atlassian.net/wiki/spaces/GPITF/pages/1391134238/GPES+Data+Extraction", StandardType = StandardType.Interoperability },
                new Standard { Id = "S47", Name = "IM1 - Interface Mechanism", Description = "Supports the implementation of interfaces between solutions.", Url = "https://gpitbjss.atlassian.net/wiki/spaces/GPITF/pages/1391133895/IM1+-+Interface+Mechanism", StandardType = StandardType.Interoperability },
                new Standard { Id = "S53", Name = "NHAIS HA/GP Links", Description = "Supports the management of patient registration and demographic information with National Health Application and Infrastructure Services.", Url = "https://gpitbjss.atlassian.net/wiki/spaces/GPITF/pages/1391133619/NHAIS+HA+GP+Links", StandardType = StandardType.Interoperability },
                new Standard { Id = "S56", Name = "Personal Demographics Service (PDS)", Description = "Supports solution integration with the Personal Demographics Service which stores patient details.", Url = "https://gpitbjss.atlassian.net/wiki/spaces/GPITF/pages/1391133999/Personal+Demographics+Service+PDS", StandardType = StandardType.Interoperability },
                new Standard { Id = "S58", Name = "Screening Messaging", Description = "Supports the validation and transfer of screening result data to solutions.", Url = "https://gpitbjss.atlassian.net/wiki/spaces/GPITF/pages/1391133789/Screening+messaging", StandardType = StandardType.Interoperability },
                new Standard { Id = "S60", Name = "Summary Care Record (SCR)", Description = "Supports the management of Summary Care Record data and staff access to the data.", Url = "https://gpitbjss.atlassian.net/wiki/spaces/GPITF/pages/1391133699/Summary+Care+Record+SCR", StandardType = StandardType.Interoperability },
                new Standard { Id = "S1", Name = "Appointments Management - Citizen", Description = "Enables citizens to manage their appointments online and supports the use of appointment slots that have been configured in Appointments Management - GP.", Url = "https://gpitbjss.atlassian.net/wiki/spaces/GPITF/pages/1391134369/Appointments+Management+-+Citizen+-+Standard", StandardType = StandardType.Capability },
                new Standard { Id = "S2", Name = "Communicate with Practice - Citizen", Description = "Supports secure and trusted electronic communications between citizens and a GP practice.", Url = "https://gpitbjss.atlassian.net/wiki/spaces/GPITF/pages/1391134173/Communicate+with+Practice+-+Citizen+-+Standard", StandardType = StandardType.Capability },
                new Standard { Id = "S3", Name = "Prescription Ordering - Citizen", Description = "Enables citizens to request medication online and manage nominated and preferred pharmacies for patients.", Url = "https://gpitbjss.atlassian.net/wiki/spaces/GPITF/pages/1391134356/Prescription+Ordering+-+Citizen+-+Standard", StandardType = StandardType.Capability },
                new Standard { Id = "S4", Name = "View Record - Citizen", Description = "Enables citizens to view their Patient Record online.", Url = "https://gpitbjss.atlassian.net/wiki/spaces/GPITF/pages/1391134394/View+Record+-+Citizen+-+Standard", StandardType = StandardType.Capability },
                new Standard { Id = "S5", Name = "Appointments Management - GP", Description = "Supports the administration, scheduling, resourcing and reporting of appointments.", Url = "https://gpitbjss.atlassian.net/wiki/spaces/GPITF/pages/1391134382/Appointments+Management+-+GP+-+Standard", StandardType = StandardType.Capability },
                new Standard { Id = "S62", Name = "General Practice Appointments Data Reporting", Description = "Enables the collection and submission to NHS Digital of appointment data to support capacity planning and management.", Url = "https://gpitbjss.atlassian.net/wiki/spaces/GPITF/pages/1391133692/General+Practice+Appointments+Data+Reporting", StandardType = StandardType.Capability },
                new Standard { Id = "S6", Name = "Clinical Decision Support", Description = "Supports clinical decision-making to improve patient safety at the point of care.", Url = "https://gpitbjss.atlassian.net/wiki/spaces/GPITF/pages/1391134143/Clinical+Decision+Support+-+Standard", StandardType = StandardType.Capability },
                new Standard { Id = "S7", Name = "Communication Management", Description = "Supports the delivery and management of communications to citizens and GP practice staff.", Url = "https://gpitbjss.atlassian.net/wiki/spaces/GPITF/pages/1391134296/Communication+Management+-+Standard", StandardType = StandardType.Capability },
                new Standard { Id = "S8", Name = "Digital Diagnostics", Description = "Supports electronic requesting with other healthcare organisations. Test results can be received, reviewed and stored against a Patient Record.", Url = "https://gpitbjss.atlassian.net/wiki/spaces/GPITF/pages/1391133783/Digital+Diagnostics+-+Standard", StandardType = StandardType.Capability },
                new Standard { Id = "S9", Name = "Document Management", Description = "Supports the secure management and classification of all forms of unstructured electronic documents including those created by scanning paper documents. Also enables processing of documents and matching documents with patients.", Url = "https://gpitbjss.atlassian.net/wiki/spaces/GPITF/pages/1391134289/Document+Management+-+Standard", StandardType = StandardType.Capability },
                new Standard { Id = "S10", Name = "GP Extracts Verification", Description = "Supports GP practice staff in ensuring accuracy of the data that is used with the Calculating Quality Reporting Service (CQRS).", Url = "https://gpitbjss.atlassian.net/wiki/spaces/GPITF/pages/1391133808/GP+Extracts+Verification+-+Standard", StandardType = StandardType.Capability },
                new Standard { Id = "S13", Name = "Patient Information Maintenance", Description = "Supports the registration of patients and the maintenance of all their personal information. Enables the organisation and presentation of a comprehensive Patient Record.", Url = "https://gpitbjss.atlassian.net/wiki/spaces/GPITF/pages/1391134401/Patient+Information+Maintenance+-+Standard", StandardType = StandardType.Capability },
                new Standard { Id = "S70", Name = "Primary Care Clinical Terminology Usage Report", Description = "Supports the collection and submission to NHS Digital of information about usage of clinical coded terms.", Url = "https://gpitbjss.atlassian.net/wiki/spaces/GPITF/pages/1391134283/Primary+Care+Clinical+Terminology+Usage+Report", StandardType = StandardType.Capability },
                new Standard { Id = "S14", Name = "Prescribing", Description = "Supports the effective and safe prescribing of medical products and appliances to patients and makes available information to support prescribing.", Url = "https://gpitbjss.atlassian.net/wiki/spaces/GPITF/pages/1391134363/Prescribing+-+Standard", StandardType = StandardType.Capability },
                new Standard { Id = "S15", Name = "Recording Consultations", Description = "Supports the standardised recording of consultations and other GP practice activities. Also supports the extraction of Female Genital Mutilation (FGM) data for the FGM data set.", Url = "https://gpitbjss.atlassian.net/wiki/spaces/GPITF/pages/1391134136/Recording+Consultations+-+Standard", StandardType = StandardType.Capability },
                new Standard { Id = "S11", Name = "Referral Management", Description = "Supports recording, reviewing, sending, and reporting of patient referrals. Enables referral information to be included in a Patient Record.", Url = "https://gpitbjss.atlassian.net/wiki/spaces/GPITF/pages/1391133630/Referral+Management+-+Standard", StandardType = StandardType.Capability },
                new Standard { Id = "S16", Name = "Reporting", Description = "Enables reporting and analysis of data from other Capabilities in the GP practice solution to support clinical care and practice management.", Url = "https://gpitbjss.atlassian.net/wiki/spaces/GPITF/pages/1391134256/Reporting+-+Standard", StandardType = StandardType.Capability },
                new Standard { Id = "S12", Name = "Resource Management", Description = "Supports the management and reporting of practice information, resources, staff members and related organisations. Also enables management of staff member availability and inactivity.", Url = "https://gpitbjss.atlassian.net/wiki/spaces/GPITF/pages/1391133828/Resource+Management+-+Standard", StandardType = StandardType.Capability },
                new Standard { Id = "S19", Name = "Unstructured Data Extraction", Description = "Enables automated and manual interpretation and extraction of structured data from paper documents and unstructured electronic documents to support their classification and matching with a Patient Record.", Url = "https://gpitbjss.atlassian.net/wiki/spaces/GPITF/pages/1391134330/Unstructured+Data+Extraction+-+Standard", StandardType = StandardType.Capability },
                new Standard { Id = "S20", Name = "Workflow", Description = "Supports manual and automated management of work in a GP practice, and enables effective planning, tracking, monitoring and reporting.", Url = "https://gpitbjss.atlassian.net/wiki/spaces/GPITF/pages/1391134376/Workflow+-+Standard", StandardType = StandardType.Capability },
                new Standard { Id = "S21", Name = "Citizen Access", Description = "Enables citizens to access their services safely and securely and also supports them in viewing and updating patient information online.", Url = "https://gpitbjss.atlassian.net/wiki/spaces/GPITF/pages/1391133929/Citizen+Access", StandardType = StandardType.Capability },
                new Standard { Id = "S22", Name = "Common Reporting", Description = "Supports the reporting needs common to GP practices and includes searchable report templates.", Url = "https://gpitbjss.atlassian.net/wiki/spaces/GPITF/pages/1391133675/Common+Reporting", StandardType = StandardType.Capability },
                new Standard { Id = "S23", Name = "Management Information (MI) Reporting", Description = "Supports the submission of aggregated counts of information regarding services, appointments, prescriptions and documents to NHS Digital.", Url = "https://gpitbjss.atlassian.net/wiki/spaces/GPITF/pages/1391134317/Management+Information+MI+Reporting", StandardType = StandardType.Capability },
            };

            context.AddRange(standards);

            // StandardCapabilities
            List<StandardCapability> standardCapabilities = new()
            {
                new StandardCapability { StandardId = "S33", CapabilityId = 13 },
                new StandardCapability { StandardId = "S34", CapabilityId = 8 },
                new StandardCapability { StandardId = "S77", CapabilityId = 13 },
                new StandardCapability { StandardId = "S35", CapabilityId = 11 },
                new StandardCapability { StandardId = "S36", CapabilityId = 14 },
                new StandardCapability { StandardId = "S37", CapabilityId = 15 },
                new StandardCapability { StandardId = "S39", CapabilityId = 15 },
                new StandardCapability { StandardId = "S43", CapabilityId = 13 },
                new StandardCapability { StandardId = "S44", CapabilityId = 13 },
                new StandardCapability { StandardId = "S44", CapabilityId = 5 },
                new StandardCapability { StandardId = "S44", CapabilityId = 23 },
                new StandardCapability { StandardId = "S44", CapabilityId = 39 },
                new StandardCapability { StandardId = "S44", CapabilityId = 30 },
                new StandardCapability { StandardId = "S78", CapabilityId = 13 },
                new StandardCapability { StandardId = "S78", CapabilityId = 5 },
                new StandardCapability { StandardId = "S46", CapabilityId = 13 },
                new StandardCapability { StandardId = "S47", CapabilityId = 13 },
                new StandardCapability { StandardId = "S47", CapabilityId = 15 },
                new StandardCapability { StandardId = "S47", CapabilityId = 5 },
                new StandardCapability { StandardId = "S47", CapabilityId = 14 },
                new StandardCapability { StandardId = "S47", CapabilityId = 11 },
                new StandardCapability { StandardId = "S47", CapabilityId = 12 },
                new StandardCapability { StandardId = "S47", CapabilityId = 1 },
                new StandardCapability { StandardId = "S47", CapabilityId = 3 },
                new StandardCapability { StandardId = "S47", CapabilityId = 4 },
                new StandardCapability { StandardId = "S47", CapabilityId = 2 },
                new StandardCapability { StandardId = "S53", CapabilityId = 13 },
                new StandardCapability { StandardId = "S56", CapabilityId = 13 },
                new StandardCapability { StandardId = "S58", CapabilityId = 8 },
                new StandardCapability { StandardId = "S60", CapabilityId = 13 },
            };

            context.AddRange(standardCapabilities);
        }

        private static void AddMockedItems(BuyingCatalogueDbContext context)
        {
            var serviceRecipients =
            ServiceRecipientsSeedData
                .GetServiceRecipients
                .Select(sr => new ServiceRecipient { Name = sr.Name, OdsCode = sr.OrgId })
                .ToList();

            context.AddRange(serviceRecipients);
        }
    }
}
