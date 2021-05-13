DECLARE @citizenAppointmentManagementCapabilityId AS uniqueidentifier = '21ae013d-42a4-4748-b435-73d5887944c2';
DECLARE @citizenCommunicateCapabilityId AS uniqueidentifier = '4f09e77b-e3a3-4a25-8ec1-815921f83628';
DECLARE @citizenViewRecordCapabilityId AS uniqueidentifier = '64e5986d-1ebf-4df0-8219-c150c082ca7b';
DECLARE @clinicalDecisionSupportCapabilityId AS uniqueidentifier = 'a71f2be1-6395-4db7-828c-d4733b42b5b5';
DECLARE @communicationManagementCapabilityId AS uniqueidentifier = '0a372f63-add4-4529-a6cd-4437c6ef115b';
DECLARE @dataAnalyticsCapabilityId AS uniqueidentifier = '5db79ff4-fa9c-4da2-bbfc-8ca40fec0b43';
DECLARE @gpAppointmentManagementCapabilityId AS uniqueidentifier = 'efd93d25-447b-4ca3-9d78-108d42afeae0';
DECLARE @gpExtractVerificationCapabilityId AS uniqueidentifier = '9d805aad-d43a-480e-9bc0-41a755bafe2f';
DECLARE @medicineOptimizationCapabilityId AS uniqueidentifier = '8bee1ff3-84d4-430b-a678-336f57c57387';
DECLARE @patientInformationMaintenanceCapabilityId AS uniqueidentifier = '8c384983-774a-45bd-9d4e-6b3c7d3b7323';
DECLARE @prescribingCapabilityId AS uniqueidentifier = 'b3f89711-6bd7-42d7-be5b-bae2f239ebdd';
DECLARE @presciptionOrderingCapabilityId AS uniqueidentifier = '60c2f5b0-b950-44c8-a246-099335a1c816';
DECLARE @recordingConsultationCapabilityId AS uniqueidentifier = '9442dcc4-22df-494b-8672-b7b4dd077496';
DECLARE @productivityCapabilityId AS uniqueidentifier = '6e77147d-d2af-46bd-a2f2-bb4f235daf3a';
DECLARE @referralManagementCapabilityId AS uniqueidentifier = '20b09859-6fc2-404c-b7a4-3830790e63ab';
DECLARE @reportingCapabilityId AS uniqueidentifier = 'dd649cc4-a710-4472-98b3-663d9d12a8b7';
DECLARE @resourceManagementCapabilityId AS uniqueidentifier = 'e3e4cf8a-22d3-4056-bb5d-10f8e26b9b5e';
DECLARE @scanningCapabilityId AS uniqueidentifier = 'e5521a71-a28e-4bc9-bddf-599f0a90719d';
DECLARE @sharedCarePlansCapabilityId AS uniqueidentifier = 'd1532ca0-ef0c-457c-9cfc-affa0fbdf134';
DECLARE @unifiedCareRecordCapabilityId AS uniqueidentifier = '59696227-602a-421d-a883-29e88997ac17';
DECLARE @workflowCapabilityId AS uniqueidentifier = '9d325dec-6e5b-44e4-876b-eacf6cd41b3e';

DECLARE @gpitframeworkId AS nvarchar(10) = 'NHSDGP001';
DECLARE @dfocvcframeworkId AS nvarchar(10) = 'DFOCVC001';
DECLARE @emptyGuid AS uniqueidentifier = '00000000-0000-0000-0000-000000000000';
DECLARE @now AS datetime = GETUTCDATE();

DECLARE @publishedStatus AS int = 3;
DECLARE @solutionItemType AS int = 1;
DECLARE @version1 AS nvarchar(10) = '1.0.0';

DECLARE @purchaseModelId AS uniqueIdentifier;
DECLARE @solutionId AS nvarchar(14);

IF UPPER('$(INSERT_TEST_DATA)') = 'TRUE' AND NOT EXISTS (SELECT * FROM dbo.Solution)
BEGIN
    SET @solutionId = '100000-001';

    IF NOT EXISTS (SELECT * FROM dbo.CatalogueItem WHERE CatalogueItemId = @solutionId)
    BEGIN
        INSERT INTO dbo.CatalogueItem(CatalogueItemId, CatalogueItemTypeId, [Name], SupplierId, PublishedStatusId, Created)
             VALUES (@solutionId, @solutionItemType, 'Write on Time', '100000', @publishedStatus, @now);

        INSERT INTO dbo.Solution(Id, [Version], Features, Hosting, AboutUrl, Summary, FullDescription, LastUpdated, LastUpdatedBy)
        VALUES (
            @solutionId,
            @version1,
            '["Flexible Pricing", "Lightweight interface designed for maximum usability", "DNA tracking and automatic improvement suggestions", "Web-based", "Remotely accessible"]',
            '{"PublicCloud":{"Summary":"Summary description","Link":"External URL link","RequiresHSCN":"Link to HSCN or N3 network required to access service"},"PrivateCloud":{"Summary":"Summary description","Link":"External URL link","HostingModel":"Hosting environment description","RequiresHSCN":"Link to HSCN or N3 network required to access service"},"HybridHostingType":{"Summary":"Summary description","Link":"External URL link","HostingModel":"Hosting environment description","RequiresHSCN":"Link to HSCN or N3 network required to access service"},"OnPremise":{"Summary":"Summary description","Link":"External URL link","HostingModel":"Hosting environment description","RequiresHSCN":"Link to HSCN or N3 network required to access service"}}',
            'http://www.writeontime.com/about',
            'Write on Time is a Citizen-facing Appointments Management system specifically designed to reduce the number of DNAs in your practice.',
            'FULL DESCRIPTION – Write on Time is a Citizen-facing Appointments Management system specifically designed to reduce the number of DNAs in your practice.',
            @now,
            @emptyGuid);

        INSERT INTO dbo.MarketingContact(SolutionId, FirstName, LastName, PhoneNumber, Email, Department, LastUpdated, LastUpdatedBy)
             VALUES (@solutionId, 'Pat', 'Butcher', '01234 567891', 'sales@test.test', 'Sales', @now, @emptyGuid);

        INSERT INTO dbo.SolutionCapability(SolutionId, CapabilityId, StatusId, LastUpdated, LastUpdatedBy)
             SELECT @solutionId, Id, 1, @now, @emptyGuid
               FROM dbo.Capability
              WHERE CapabilityRef = 'C1';

        INSERT INTO dbo.FrameworkSolutions(FrameworkId ,SolutionId ,IsFoundation, LastUpdated, LastUpdatedBy)
             VALUES (@gpitframeworkId, @solutionId , 1, @now, @emptyGuid);
    END;

    /*************************************************************************************************************************************************************/

    SET @solutionId = '100001-001';

    IF NOT EXISTS (SELECT * FROM dbo.CatalogueItem WHERE CatalogueItemId = @solutionId)
    BEGIN
        INSERT INTO dbo.CatalogueItem(CatalogueItemId, CatalogueItemTypeId, [Name], SupplierId, PublishedStatusId, Created)
             VALUES (@solutionId, @solutionItemType, 'Appointment Gateway', '100001', @publishedStatus, @now);

        INSERT INTO dbo.Solution(Id, [Version], Features, Hosting, AboutUrl, Summary, FullDescription, LastUpdated, LastUpdatedBy)
        VALUES (
            @solutionId,
            @version1,
            '["Tested and approved by hundred''s of GPs", "99.9% service availability guaranteed", "Appointment forwarding & referral integration", "Fully interopable with all major GP IT solutions", "Compliant with all relevant ISO standards"]',
            '{"PublicCloud":{"Summary":"Summary description","Link":"External URL link","RequiresHSCN":"Link to HSCN or N3 network required to access service"},"PrivateCloud":{"Summary":"Summary description","Link":"External URL link","HostingModel":"Hosting environment description","RequiresHSCN":"Link to HSCN or N3 network required to access service"},"HybridHostingType":{"Summary":"Summary description","Link":"External URL link","HostingModel":"Hosting environment description","RequiresHSCN":"Link to HSCN or N3 network required to access service"},"OnPremise":{"Summary":"Summary description","Link":"External URL link","HostingModel":"Hosting environment description","RequiresHSCN":"Link to HSCN or N3 network required to access service"}}',
            'http://www.appointmentgateway.com/about',
            'Appointment Gateway is a complete appointment management suite that has been fully integrated with all major clinical systems throughout both the UK and Europe.',
            'FULL DESCRIPTION – Appointment Gateway is a complete appointment management suite that has been fully integrated with all major clinical systems throughout both the UK and Europe.',
            @now,
            @emptyGuid);

        INSERT INTO dbo.MarketingContact(SolutionId, FirstName, LastName, PhoneNumber, Email, Department, LastUpdated, LastUpdatedBy)
             VALUES (@solutionId, 'Sam', 'Samosa', '01234 567891', 'sales@test.test', 'Sales', @now, @emptyGuid);

        INSERT INTO dbo.SolutionCapability(SolutionId, CapabilityId, StatusId, LastUpdated, LastUpdatedBy)
             SELECT @solutionId, Id, 1, @now, @emptyGuid
               FROM dbo.Capability
              WHERE CapabilityRef IN ('C1', 'C5');

        INSERT INTO dbo.FrameworkSolutions(FrameworkId, SolutionId, IsFoundation, LastUpdated, LastUpdatedBy)
             VALUES (@gpitframeworkId, @solutionId, 1, @now, @emptyGuid);
    END;

    /*************************************************************************************************************************************************************/

    SET @solutionId = '100002-001';

    IF NOT EXISTS (SELECT * FROM dbo.CatalogueItem WHERE CatalogueItemId = @solutionId)
    BEGIN
        INSERT INTO dbo.CatalogueItem(CatalogueItemId, CatalogueItemTypeId, [Name], SupplierId, PublishedStatusId, Created)
             VALUES (@solutionId, @solutionItemType, 'Zen Guidance', '100002', @publishedStatus, @now);

        INSERT INTO dbo.Solution(Id, [Version], Features, Hosting, AboutUrl, Summary, FullDescription, LastUpdated, LastUpdatedBy)
        VALUES (
            @solutionId,
            @version1,
            '["Advanced AI functionality", "MESH & FHIR compliant", "Remotely accessible ", "Cloud-hosted", "24/7 customer support"]',
            '{"PublicCloud":{"Summary":"Summary description","Link":"External URL link","RequiresHSCN":"Link to HSCN or N3 network required to access service"},"PrivateCloud":{"Summary":"Summary description","Link":"External URL link","HostingModel":"Hosting environment description","RequiresHSCN":"Link to HSCN or N3 network required to access service"},"HybridHostingType":{"Summary":"Summary description","Link":"External URL link","HostingModel":"Hosting environment description","RequiresHSCN":"Link to HSCN or N3 network required to access service"},"OnPremise":{"Summary":"Summary description","Link":"External URL link","HostingModel":"Hosting environment description","RequiresHSCN":"Link to HSCN or N3 network required to access service"}}',
            'http://www.zenguidance.com/about',
            'Zen Guidance utilizes an advanced AI framework to provide clinicians with highly accurate data to support sound decision-making.',
            'FULL DESCRIPTION – Zen Guidance utilizes an advanced AI framework to provide clinicians with highly accurate data to support sound decision-making.',
            @now,
            @emptyGuid);

        INSERT INTO dbo.MarketingContact(SolutionId, FirstName, LastName, PhoneNumber, Email, Department, LastUpdated, LastUpdatedBy)
             VALUES (@solutionId, 'Victoria', 'Sponge', '01234 567891', 'sales@test.test', 'Sales', @now, @emptyGuid);

        INSERT INTO dbo.SolutionCapability (SolutionId, CapabilityId, StatusId, LastUpdated, LastUpdatedBy)
             SELECT @solutionId, Id, 1, @now, @emptyGuid
               FROM dbo.Capability
              WHERE CapabilityRef = 'C6';
    END;

    /*************************************************************************************************************************************************************/

    SET @solutionId = '100003-001';

    IF NOT EXISTS (SELECT * FROM dbo.CatalogueItem WHERE CatalogueItemId = @solutionId)
    BEGIN
        INSERT INTO dbo.CatalogueItem(CatalogueItemId, CatalogueItemTypeId, [Name], SupplierId, PublishedStatusId, Created)
             VALUES (@solutionId, @solutionItemType, 'Intellidoc Comms', '100003', @publishedStatus, @now);

        INSERT INTO dbo.Solution(Id, [Version], Features, Hosting, AboutUrl, Summary, FullDescription, LastUpdated, LastUpdatedBy)
        VALUES (
            @solutionId,
            @version1,
            '["Efficient instant & scheduled messaging", "Web-based interface", "Compliant with all relevant ISO standards", "Wide range of add-ons available", "Cloud-hosted"]',
            '{"PublicCloud":{"Summary":"Summary description","Link":"External URL link","RequiresHSCN":"Link to HSCN or N3 network required to access service"},"PrivateCloud":{"Summary":"Summary description","Link":"External URL link","HostingModel":"Hosting environment description","RequiresHSCN":"Link to HSCN or N3 network required to access service"},"HybridHostingType":{"Summary":"Summary description","Link":"External URL link","HostingModel":"Hosting environment description","RequiresHSCN":"Link to HSCN or N3 network required to access service"},"OnPremise":{"Summary":"Summary description","Link":"External URL link","HostingModel":"Hosting environment description","RequiresHSCN":"Link to HSCN or N3 network required to access service"}}',
            'http://www.intellidoc.com/about',
            'Intellidoc Comms empowers all practice staff to record & send communications in an extremely streamlined and time-efficient manner.',
            'FULL DESCRIPTION – Intellidoc Comms empowers all practice staff to record & send communications in an extremely streamlined and time-efficient manner.',
            @now,
            @emptyGuid);

        INSERT INTO dbo.MarketingContact(SolutionId, FirstName, LastName, PhoneNumber, Email, Department, LastUpdated, LastUpdatedBy)
             VALUES (@solutionId, 'Richard', 'Burton', '01234 567891', 'sales@test.test', 'Sales', @now, @emptyGuid);

        INSERT INTO dbo.SolutionCapability(SolutionId, CapabilityId, StatusId, LastUpdated, LastUpdatedBy)
             SELECT @solutionId, Id, 1, @now, @emptyGuid
               FROM dbo.Capability
              WHERE CapabilityRef IN ('C7', 'C15');
    END;

    /*************************************************************************************************************************************************************/

    SET @solutionId = '100004-001';

    IF NOT EXISTS (SELECT * FROM dbo.CatalogueItem WHERE CatalogueItemId = @solutionId)
    BEGIN
        INSERT INTO dbo.CatalogueItem(CatalogueItemId, CatalogueItemTypeId, [Name], SupplierId, PublishedStatusId, Created)
             VALUES (@solutionId, @solutionItemType, 'Diagnostics XYZ', '100004', @publishedStatus, @now);

        INSERT INTO dbo.Solution(Id, [Version], Features, Hosting, AboutUrl, Summary, FullDescription, LastUpdated, LastUpdatedBy)
        VALUES (
            @solutionId,
            @version1,
            '["Seamless integration with a wide range of diagnostic hardware", "Demo & free trial available", "FHIR compliant", "Plug and play – minimal deployment activity required", "Optimized for touchscreen & desktop use"]',
            '{"PublicCloud":{"Summary":"Summary description","Link":"External URL link","RequiresHSCN":"Link to HSCN or N3 network required to access service"},"PrivateCloud":{"Summary":"Summary description","Link":"External URL link","HostingModel":"Hosting environment description","RequiresHSCN":"Link to HSCN or N3 network required to access service"},"HybridHostingType":{"Summary":"Summary description","Link":"External URL link","HostingModel":"Hosting environment description","RequiresHSCN":"Link to HSCN or N3 network required to access service"},"OnPremise":{"Summary":"Summary description","Link":"External URL link","HostingModel":"Hosting environment description","RequiresHSCN":"Link to HSCN or N3 network required to access service"}}',
            'http://www.diagnostics.xyz/about',
            'Diagnostics XYZ introduces new diagnostic tools not currently provided by the leading clinical software suppliers.',
            'FULL DESCRIPTION – Diagnostics XYZ introduces new diagnostic tools not currently provided by the leading clinical software suppliers.',
            @now,
            @emptyGuid);

        INSERT INTO dbo.MarketingContact(SolutionId, FirstName, LastName, PhoneNumber, Email, Department, LastUpdated, LastUpdatedBy)
             VALUES (@solutionId, 'Harry', 'Houdini', '01234 567891', 'sales@test.test', 'Sales', @now, @emptyGuid);

        INSERT INTO dbo.SolutionCapability(SolutionId, CapabilityId, StatusId, LastUpdated, LastUpdatedBy)
             SELECT @solutionId, Id, 1, @now, @emptyGuid
               FROM dbo.Capability
              WHERE CapabilityRef = 'C8';

        INSERT INTO dbo.FrameworkSolutions(FrameworkId ,SolutionId ,IsFoundation, LastUpdated, LastUpdatedBy)
             VALUES (@gpitframeworkId, @solutionId , 1, @now, @emptyGuid);
    END;

    /*************************************************************************************************************************************************************/

    SET @solutionId = '100005-001';

    IF NOT EXISTS (SELECT * FROM dbo.CatalogueItem WHERE CatalogueItemId = @solutionId)
    BEGIN
        INSERT INTO dbo.CatalogueItem(CatalogueItemId, CatalogueItemTypeId, [Name], SupplierId, PublishedStatusId, Created)
             VALUES (@solutionId, @solutionItemType, 'Document Wizard', '100005', @publishedStatus, @now);

        INSERT INTO dbo.Solution(Id, [Version], Features, Hosting, AboutUrl, Summary, FullDescription, LastUpdated, LastUpdatedBy)
        VALUES (
            @solutionId,
            @version1,
            '["Industry-leading data extraction & scanning accuracy", "Fully interopable with all major GP IT solutions", "24/7 customer support", "Fully Compliant with all relevant ISO standards", "Modular architecture to enhance compatibility and customisation"]',
            '{"PublicCloud":{"Summary":"Summary description","Link":"External URL link","RequiresHSCN":"Link to HSCN or N3 network required to access service"},"PrivateCloud":{"Summary":"Summary description","Link":"External URL link","HostingModel":"Hosting environment description","RequiresHSCN":"Link to HSCN or N3 network required to access service"},"HybridHostingType":{"Summary":"Summary description","Link":"External URL link","HostingModel":"Hosting environment description","RequiresHSCN":"Link to HSCN or N3 network required to access service"},"OnPremise":{"Summary":"Summary description","Link":"External URL link","HostingModel":"Hosting environment description","RequiresHSCN":"Link to HSCN or N3 network required to access service"}}',
            'http://www.documentwizard.com/about',
            'Document Wizard is the UK industry-leader for clinical document management software due to our patented lightweight interface and interoperability.',
            'FULL DESCRIPTION – Document Wizard is the UK industry-leader for clinical document management software due to our patented lightweight interface and interoperability.',
            @now,
            @emptyGuid);

        INSERT INTO dbo.MarketingContact(SolutionId, FirstName, LastName, PhoneNumber, Email, Department, LastUpdated, LastUpdatedBy)
             VALUES (@solutionId, 'Agent', 'M', '01234 567891', 'm@test.test', 'Sales', @now, @emptyGuid);

        INSERT INTO dbo.SolutionCapability(SolutionId, CapabilityId, StatusId, LastUpdated, LastUpdatedBy)
             SELECT @solutionId, Id, 1, @now, @emptyGuid
               FROM dbo.Capability
              WHERE CapabilityRef IN ('C9', 'C19', 'C41');
    END;

    /*************************************************************************************************************************************************************/

    SET @solutionId = '100006-001';

    IF NOT EXISTS (SELECT * FROM dbo.CatalogueItem WHERE CatalogueItemId = @solutionId)
    BEGIN
        INSERT INTO dbo.CatalogueItem(CatalogueItemId, CatalogueItemTypeId, [Name], SupplierId, PublishedStatusId, Created)
             VALUES (@solutionId, @solutionItemType, 'Paperlite', '100006', @publishedStatus, @now);

        INSERT INTO dbo.Solution(Id, [Version], Features, Hosting, AboutUrl, Summary, FullDescription, LastUpdated, LastUpdatedBy)
        VALUES (
            @solutionId,
            @version1,
            '["Revolutionary optical character recognition technology", "Can be deployed quickly at low-cost", "Web-based interface", "Cloud-hosted", "Wide range of add-ons available"]',
            '{"PublicCloud":{"Summary":"Summary description","Link":"External URL link","RequiresHSCN":"Link to HSCN or N3 network required to access service"},"PrivateCloud":{"Summary":"Summary description","Link":"External URL link","HostingModel":"Hosting environment description","RequiresHSCN":"Link to HSCN or N3 network required to access service"},"HybridHostingType":{"Summary":"Summary description","Link":"External URL link","HostingModel":"Hosting environment description","RequiresHSCN":"Link to HSCN or N3 network required to access service"},"OnPremise":{"Summary":"Summary description","Link":"External URL link","HostingModel":"Hosting environment description","RequiresHSCN":"Link to HSCN or N3 network required to access service"}}',
            'http://www.paperlite.com/about',
            'Paperlite utilises new OCR technology to seamlessly transfer written notes to digital patient records.',
            'FULL DESCRIPTION – Paperlite utilises new OCR technology to seamlessly transfer written notes to digital patient records.',
            @now,
            @emptyGuid);

        INSERT INTO dbo.MarketingContact(SolutionId, FirstName, LastName, PhoneNumber, Email, Department, LastUpdated, LastUpdatedBy)
             VALUES ('100006-001', 'Timothy', 'Teabag', '01234 567891', 'sales@test.test', 'Sales', @now, @emptyGuid);

        INSERT INTO dbo.SolutionCapability(SolutionId, CapabilityId, StatusId, LastUpdated, LastUpdatedBy)
             SELECT '100006-001', Id, 1, @now, @emptyGuid
               FROM dbo.Capability
              WHERE CapabilityRef IN ('C9', 'C17');
    END;

    /*************************************************************************************************************************************************************/

    SET @solutionId = '100007-001';

    IF NOT EXISTS (SELECT * FROM dbo.CatalogueItem WHERE CatalogueItemId = @solutionId)
    BEGIN
        INSERT INTO dbo.CatalogueItem(CatalogueItemId, CatalogueItemTypeId, [Name], SupplierId, PublishedStatusId, Created)
             VALUES (@solutionId, @solutionItemType, 'Medsort', '100007', @publishedStatus, @now);

        INSERT INTO dbo.Solution(Id, [Version], Features, Hosting, AboutUrl, Summary, FullDescription, LastUpdated, LastUpdatedBy)
        VALUES (
            @solutionId,
            @version1,
            '["Fully adaptable to suit your practice''s needs", "Integrates with Spine", "FHIR compliant", "Flexible Pricing", "24/7 customer support"]',
            '{"PublicCloud":{"Summary":"Summary description","Link":"External URL link","RequiresHSCN":"Link to HSCN or N3 network required to access service"},"PrivateCloud":{"Summary":"Summary description","Link":"External URL link","HostingModel":"Hosting environment description","RequiresHSCN":"Link to HSCN or N3 network required to access service"},"HybridHostingType":{"Summary":"Summary description","Link":"External URL link","HostingModel":"Hosting environment description","RequiresHSCN":"Link to HSCN or N3 network required to access service"},"OnPremise":{"Summary":"Summary description","Link":"External URL link","HostingModel":"Hosting environment description","RequiresHSCN":"Link to HSCN or N3 network required to access service"}}',
            'http://www.medsort.com/about',
            'Medsort enhances your medicine optimisation process and introduces new, more customisable tools that can be adapted to your local environment.',
            'FULL DESCRIPTION – Medsort enhances your medicine optimisation process and introduces new, more customisable tools that can be adapted to your local environment.',
            @now,
            @emptyGuid);

        INSERT INTO dbo.MarketingContact(SolutionId, FirstName, LastName, PhoneNumber, Email, Department, LastUpdated, LastUpdatedBy)
             VALUES (@solutionId, 'Betty', 'Banjo', '01234 567891', 'sales@test.test', 'Sales', @now, @emptyGuid);

        INSERT INTO dbo.SolutionCapability(SolutionId, CapabilityId, StatusId, LastUpdated, LastUpdatedBy)
             SELECT @solutionId, Id, 1, @now, @emptyGuid
               FROM dbo.Capability
              WHERE CapabilityRef = 'C30';
    END;

    /*************************************************************************************************************************************************************/

    SET @solutionId = '100007-002';
    IF NOT EXISTS (SELECT * FROM dbo.CatalogueItem WHERE CatalogueItemId = @solutionId)
    BEGIN
        INSERT INTO dbo.CatalogueItem(CatalogueItemId, CatalogueItemTypeId, [Name], SupplierId, PublishedStatusId, Created)
             VALUES (@solutionId, @solutionItemType, 'BostonDynamics', '100007', @publishedStatus, @now);

        INSERT INTO dbo.Solution(Id, [Version], Features, Hosting, AboutUrl, Summary, FullDescription, LastUpdated, LastUpdatedBy)
        VALUES (
            @solutionId,
            @version1,
            '["Fully adaptable to suit your practice''s needs", "Integrates with Spine", "FHIR compliant", "Flexible Pricing", "24/7 customer support"]',
            '{"PublicCloud":{"Summary":"Summary description","Link":"External URL link","RequiresHSCN":"Link to HSCN or N3 network required to access service"},"PrivateCloud":{"Summary":"Summary description","Link":"External URL link","HostingModel":"Hosting environment description","RequiresHSCN":"Link to HSCN or N3 network required to access service"},"HybridHostingType":{"Summary":"Summary description","Link":"External URL link","HostingModel":"Hosting environment description","RequiresHSCN":"Link to HSCN or N3 network required to access service"},"OnPremise":{"Summary":"Summary description","Link":"External URL link","HostingModel":"Hosting environment description","RequiresHSCN":"Link to HSCN or N3 network required to access service"}}',
            'http://www.bostondynamics.com/about',
            'BostonDynamics enhances your medicine optimisation process and introduces new, more customisable tools that can be adapted to your local environment.',
            'FULL DESCRIPTION – BostonDynamics enhances your medicine optimisation process and introduces new, more customisable tools that can be adapted to your local environment.',
            @now,
            @emptyGuid);

        INSERT INTO dbo.MarketingContact(SolutionId, FirstName, LastName, PhoneNumber, Email, Department, LastUpdated, LastUpdatedBy)
             VALUES (@solutionId, 'Boston', 'Rocks', '01234 567891', 'sales@test.test', 'Sales', @now, @emptyGuid);

        INSERT INTO dbo.SolutionCapability(SolutionId, CapabilityId, StatusId, LastUpdated, LastUpdatedBy)
             SELECT @solutionId, Id, 1, @now, @emptyGuid
               FROM dbo.Capability
              WHERE CapabilityRef = 'C30';
    END;

    /*************************************************************************************************************************************************************/

    SET @solutionId = '99999-89';

    IF NOT EXISTS (SELECT * FROM dbo.CatalogueItem WHERE CatalogueItemId = @solutionId)
    BEGIN
        INSERT INTO dbo.CatalogueItem(CatalogueItemId, CatalogueItemTypeId, [Name], SupplierId, PublishedStatusId, Created)
             VALUES (@solutionId, @solutionItemType, 'NotEmis Web GP', '99999', @publishedStatus, @now);

        INSERT INTO dbo.Solution(Id, [Version],
                    Features, ClientApplication, Hosting, ImplementationDetail, RoadMap,
                    IntegrationsUrl, Summary, FullDescription,
                    LastUpdated, LastUpdatedBy)
        VALUES
        (
            @solutionId,
            @version1,
            '["Access to real-time patient data that can be shared between locations and healthcare organisations","One-click access to patient summary information","Quick data entry using protocols, templates and concepts tailored to your practice requirements","Integrated clinical safety alerts graded to highlight severity","Automatic notification of linked pre-existing conditions when recording a new acute problem","Integrated patient recall system to target specific lists of patients for specific clinics","Intelligent alerts and auto-templates to capture outstanding QOF information","Integrated QOF-finder to identify patients where you are losing QOF points","Seamless data exchange with over 100 partners including Graphnet, Cerner and Optum","Integration with Patient Access enables patients to book appointments and order prescriptions"]',
            '{"ClientApplicationTypes":["browser-based","native-mobile","native-desktop"],"BrowsersSupported":["Google Chrome","Chromium","Internet Explorer 11","Internet Explorer 10"],"MobileResponsive":true,"Plugins":{"Required":false,"AdditionalInformation":""},"MinimumConnectionSpeed":"2Mbps","MinimumDesktopResolution":"16:9 – 1366 x 768","HardwareRequirements":"The browser activities are only supported in relation the native desktop client therefore mirror the native desktop client hardware requirements detailed below.","NativeMobileHardwareRequirements":"Any device capable of supporting the listed supported operating systems is compliant.","NativeDesktopHardwareRequirements":"The spoke server is an important part of the solution. It provides a patch distribution system for client updates and acts as a local cache. \r\nEMIS Health recommends that your spoke is a dedicated device; however, if you use your spoke to perform other functions, such as act as a domain controller, store business documents or host other applications, then a Windows server class operating system will be required, along with an appropriate specification of server hardware.","AdditionalInformation":"","MobileFirstDesign":false,"NativeMobileFirstDesign":false,"MobileOperatingSystems":{"OperatingSystems":["Apple IOS","Android","Other"],"OperatingSystemsDescription":"•\tiOS v 10.3.3.3 and above\r\n•\tAndroid v 6 and above\r\n•\tWindows 10 (Build 14393)"},"MobileConnectionDetails":{"MinimumConnectionSpeed":"","ConnectionType":["GPRS","3G","LTE","4G","5G","Wifi"],"Description":"The mobile application only requires internet connectivity to synchronize therefore there is no minimum connection speed required."},"MobileMemoryAndStorage":{"MinimumMemoryRequirement":"2GB","Description":"All compliant devices have a minimum 16GB storage"},"MobileThirdParty":{"ThirdPartyComponents":"","DeviceCapabilities":"The device should have access to the relevant App Store to enable the installation of the respective application although deployment via mobile device management solutions is supported."},"NativeMobileAdditionalInformation":"Apple have recently announced that a new operating system, designed specifically for iPad devices.\r\nWe have tested this and can confirm that EMIS Mobile is fully compatible.","NativeDesktopOperatingSystemsDescription":"Microsoft Windows 7 (x86 x64)\r\nMicrosoft Windows 8.1 (x86 x64)\r\nMicrosoft Windows 10 (x86 x64)","NativeDesktopMinimumConnectionSpeed":"2Mbps","NativeDesktopThirdParty":{"ThirdPartyComponents":".NET framework 4","DeviceCapabilities":"The application requires connectivity to the EMIS Data Centre."},"NativeDesktopMemoryAndStorage":{"MinimumMemoryRequirement":"4GB","StorageRequirementsDescription":"10GB free disk space","MinimumCpu":"Intel Core i3 equivalent or higher","RecommendedResolution":"16:9 – 1366 x 768"},"NativeDesktopAdditionalInformation":"The minimum connection speed is dependent on the number of clients that need to be supported.\r\n\r\nEMISHealth do not support the use of on-screen keyboards for 2 in 1 devices."}',
            '{"PublicCloud":{"Summary":"This service is not available on public cloud","Link":""},"PrivateCloud":{"Summary":"EMIS Web is hosted in EMIS’ own data centres and the solution is provided as Software as a Service","Link":"","HostingModel":"Model complies with GPIT Futures requirements for hosting","RequiresHSCN":"End user devices must be connected to HSCN/N3"}}',
            'The typical timescales for the implementation of EMIS Web are around 12 weeks depending on the patient list size.

An implementation plan is provided and agreed at the beginning of the process outlining all key dates and activities, where required these dates are negotiable to fit the needs of the customer.

Key activities:

•	Customer supplied with high level implementation plan and welcome pack

•	Engineer visit to perform install of client software and check connectivity

•	Customer supplied test data loaded into a test system and made available

•	Learning needs analysis performed & agreed training plan for go live

•	On site visit to train the customer how to check and cleanse their data

•	Any defects and corrections completed on migrated data

•	Practice sign off of test data

•	Agreed training provide pre and post go live in line with results from LNA

•	Go live day, trainers and engineer onsite to support a smooth transition',
'The following roadmap details all IT Futures managed capacity items. EMIS Health is committed to delivering against the effective date. The roadmap provides visibility on which items have been completed, are scheduled and are in the pipeline to be scheduled.',
'https://www.emishealth.com/products/partner-products/',
        'EMIS Web is the most widely used GP clinical system in the UK. Created by clinicians for clinicians, it helps run efficient practices, whilst delivering the best possible patient care. With patient safety at its core, EMIS Web enables you to deliver safe and informed on demand care across locations.',
        'EMIS Web GP is a comprehensive, flexible and powerful clinical management tool for healthcare organisations. It is intuitive, easy to learn and was ranked the number one clinical system in the UK for interoperability (KLAS report).

EMIS Web GP is fully accredited to securely exchange data with the NHS SPINE and incorporates functionality such as GP2GP, Electronic Prescription Service (EPS), Personal Demographics Service (PDS), NHS e-Referral Service (e-RS) , Summary Care Record (SCR), eMED3 (Fitnotes), SNOMED CT and Female genital mutilation (FGM).

Using EMIS Web, healthcare professionals can provide the best possible patient care with patient safety at its core. We safely and securely hold more patient records than any other supplier and work with clinicians and pharmacists to ensure the highest possible standards of patient safety are upheld. The system provides secure access to all the information they need to make the right decisions for their patients.',
            @now,
            @emptyGuid);

        INSERT INTO dbo.MarketingContact(SolutionId, FirstName, LastName, Email, PhoneNumber, Department, LastUpdated, LastUpdatedBy)
             VALUES (@solutionId, 'Eduardo', 'Eggbert', 'eddie@eggs.test', '01234 567891', 'Internal Sales Team', @now, @emptyGuid);

        INSERT INTO dbo.SolutionCapability (SolutionId, CapabilityId, StatusId, LastUpdated, LastUpdatedBy)
        VALUES
        (@solutionId, @gpAppointmentManagementCapabilityId, 1, @now, @emptyGuid),
        (@solutionId, @resourceManagementCapabilityId, 1, @now, @emptyGuid),
        (@solutionId, @medicineOptimizationCapabilityId, 1, @now, @emptyGuid),
        (@solutionId, @referralManagementCapabilityId, 1, @now, @emptyGuid),
        (@solutionId, @gpExtractVerificationCapabilityId, 1, @now, @emptyGuid),
        (@solutionId, @scanningCapabilityId, 1, @now, @emptyGuid),
        (@solutionId, @reportingCapabilityId, 1, @now, @emptyGuid),
        (@solutionId, @patientInformationMaintenanceCapabilityId, 1, @now, @emptyGuid),
        (@solutionId, @recordingConsultationCapabilityId, 1, @now, @emptyGuid),
        (@solutionId, @prescribingCapabilityId, 1, @now, @emptyGuid),
        (@solutionId, @productivityCapabilityId, 1, @now, @emptyGuid),
        (@solutionId, @clinicalDecisionSupportCapabilityId, 1, @now, @emptyGuid),
        (@solutionId, @workflowCapabilityId, 1, @now, @emptyGuid);

        INSERT INTO dbo.FrameworkSolutions(FrameworkId ,SolutionId ,IsFoundation, LastUpdated ,LastUpdatedBy)
             VALUES (@gpitframeworkId, @solutionId , 1, @now, @emptyGuid);

        INSERT INTO dbo.SolutionEpic (SolutionId, CapabilityId, EpicId, StatusId, LastUpdated, LastUpdatedBy)
        VALUES
        (@solutionId, @gpAppointmentManagementCapabilityId, 'C5E1', 1, @now, @emptyGuid),
        (@solutionId, @gpAppointmentManagementCapabilityId, 'C5E2', 1, @now, @emptyGuid),
        (@solutionId, @gpAppointmentManagementCapabilityId, 'C5E3', 1, @now, @emptyGuid),
        (@solutionId, @gpAppointmentManagementCapabilityId, 'C5E4', 1, @now, @emptyGuid),
        (@solutionId, @gpAppointmentManagementCapabilityId, 'C5E5', 1, @now, @emptyGuid),
        (@solutionId, @gpAppointmentManagementCapabilityId, 'C5E6', 1, @now, @emptyGuid),
        (@solutionId, @gpAppointmentManagementCapabilityId, 'C5E7', 1, @now, @emptyGuid),
        (@solutionId, @clinicalDecisionSupportCapabilityId, 'C6E1', 1, @now, @emptyGuid),
        (@solutionId, @clinicalDecisionSupportCapabilityId, 'C6E2', 3, @now, @emptyGuid),
        (@solutionId, @clinicalDecisionSupportCapabilityId, 'C6E3', 2, @now, @emptyGuid),
        (@solutionId, @clinicalDecisionSupportCapabilityId, 'C6E4', 2, @now, @emptyGuid),
        (@solutionId, @gpExtractVerificationCapabilityId, 'C10E1', 1, @now, @emptyGuid),
        (@solutionId, @gpExtractVerificationCapabilityId, 'C10E2', 2, @now, @emptyGuid),
        (@solutionId, @referralManagementCapabilityId, 'C11E1', 1, @now, @emptyGuid),
        (@solutionId, @referralManagementCapabilityId, 'C11E2', 1, @now, @emptyGuid),
        (@solutionId, @resourceManagementCapabilityId, 'C12E1', 1, @now, @emptyGuid),
        (@solutionId, @resourceManagementCapabilityId, 'C12E2', 1, @now, @emptyGuid),
        (@solutionId, @resourceManagementCapabilityId, 'C12E3', 1, @now, @emptyGuid),
        (@solutionId, @resourceManagementCapabilityId, 'C12E4', 1, @now, @emptyGuid),
        (@solutionId, @resourceManagementCapabilityId, 'C12E5', 1, @now, @emptyGuid),
        (@solutionId, @resourceManagementCapabilityId, 'C12E6', 1, @now, @emptyGuid),
        (@solutionId, @resourceManagementCapabilityId, 'C12E7', 2, @now, @emptyGuid),
        (@solutionId, @patientInformationMaintenanceCapabilityId, 'C13E1', 1, @now, @emptyGuid),
        (@solutionId, @patientInformationMaintenanceCapabilityId, 'C13E2', 1, @now, @emptyGuid),
        (@solutionId, @patientInformationMaintenanceCapabilityId, 'C13E3', 1, @now, @emptyGuid),
        (@solutionId, @patientInformationMaintenanceCapabilityId, 'C13E4', 1, @now, @emptyGuid),
        (@solutionId, @patientInformationMaintenanceCapabilityId, 'C13E5', 1, @now, @emptyGuid),
        (@solutionId, @patientInformationMaintenanceCapabilityId, 'C13E6', 1, @now, @emptyGuid),
        (@solutionId, @patientInformationMaintenanceCapabilityId, 'C13E7', 1, @now, @emptyGuid),
        (@solutionId, @patientInformationMaintenanceCapabilityId, 'C13E8', 1, @now, @emptyGuid),
        (@solutionId, @patientInformationMaintenanceCapabilityId, 'C13E9', 1, @now, @emptyGuid),
        (@solutionId, @patientInformationMaintenanceCapabilityId, 'C13E10', 1, @now, @emptyGuid),
        (@solutionId, @patientInformationMaintenanceCapabilityId, 'C13E11', 1, @now, @emptyGuid),
        (@solutionId, @patientInformationMaintenanceCapabilityId, 'C13E12', 1, @now, @emptyGuid),
        (@solutionId, @patientInformationMaintenanceCapabilityId, 'C13E13', 2, @now, @emptyGuid),
        (@solutionId, @patientInformationMaintenanceCapabilityId, 'C13E14', 2, @now, @emptyGuid),
        (@solutionId, @patientInformationMaintenanceCapabilityId, 'C13E15', 2, @now, @emptyGuid),
        (@solutionId, @patientInformationMaintenanceCapabilityId, 'C13E16', 2, @now, @emptyGuid),
        (@solutionId, @patientInformationMaintenanceCapabilityId, 'C13E17', 2, @now, @emptyGuid),
        (@solutionId, @patientInformationMaintenanceCapabilityId, 'C13E18', 2, @now, @emptyGuid),
        (@solutionId, @patientInformationMaintenanceCapabilityId, 'C13E19', 2, @now, @emptyGuid),
        (@solutionId, @patientInformationMaintenanceCapabilityId, 'C13E20', 2, @now, @emptyGuid),
        (@solutionId, @patientInformationMaintenanceCapabilityId, 'C13E21', 2, @now, @emptyGuid),
        (@solutionId, @prescribingCapabilityId, 'C14E1', 1, @now, @emptyGuid),
        (@solutionId, @prescribingCapabilityId, 'C14E2', 1, @now, @emptyGuid),
        (@solutionId, @prescribingCapabilityId, 'C14E3', 1, @now, @emptyGuid),
        (@solutionId, @prescribingCapabilityId, 'C14E4', 1, @now, @emptyGuid),
        (@solutionId, @prescribingCapabilityId, 'C14E5', 1, @now, @emptyGuid),
        (@solutionId, @prescribingCapabilityId, 'C14E6', 1, @now, @emptyGuid),
        (@solutionId, @prescribingCapabilityId, 'C14E7', 1, @now, @emptyGuid),
        (@solutionId, @prescribingCapabilityId, 'C14E8', 1, @now, @emptyGuid),
        (@solutionId, @prescribingCapabilityId, 'C14E9', 1, @now, @emptyGuid),
        (@solutionId, @prescribingCapabilityId, 'C14E10', 1, @now, @emptyGuid),
        (@solutionId, @prescribingCapabilityId, 'C14E11', 1, @now, @emptyGuid),
        (@solutionId, @prescribingCapabilityId, 'C14E12', 1, @now, @emptyGuid),
        (@solutionId, @prescribingCapabilityId, 'C14E13', 1, @now, @emptyGuid),
        (@solutionId, @prescribingCapabilityId, 'C14E14', 2, @now, @emptyGuid),
        (@solutionId, @prescribingCapabilityId, 'C14E15', 2, @now, @emptyGuid),
        (@solutionId, @prescribingCapabilityId, 'C14E16', 2, @now, @emptyGuid),
        (@solutionId, @recordingConsultationCapabilityId, 'C15E1', 1, @now, @emptyGuid),
        (@solutionId, @recordingConsultationCapabilityId, 'C15E2', 1, @now, @emptyGuid),
        (@solutionId, @recordingConsultationCapabilityId, 'C15E3', 1, @now, @emptyGuid),
        (@solutionId, @recordingConsultationCapabilityId, 'C15E4', 1, @now, @emptyGuid),
        (@solutionId, @recordingConsultationCapabilityId, 'C15E5', 1, @now, @emptyGuid),
        (@solutionId, @recordingConsultationCapabilityId, 'C15E6', 1, @now, @emptyGuid),
        (@solutionId, @recordingConsultationCapabilityId, 'C15E7', 1, @now, @emptyGuid),
        (@solutionId, @recordingConsultationCapabilityId, 'C15E8', 1, @now, @emptyGuid),
        (@solutionId, @reportingCapabilityId, 'C16E1', 1, @now, @emptyGuid),
        (@solutionId, @scanningCapabilityId, 'C17E1', 1, @now, @emptyGuid),
        (@solutionId, @scanningCapabilityId, 'C17E2', 2, @now, @emptyGuid),
        (@solutionId, @workflowCapabilityId, 'C20E1', 2, @now, @emptyGuid),
        (@solutionId, @workflowCapabilityId, 'C20E2', 2, @now, @emptyGuid),
        (@solutionId, @workflowCapabilityId, 'C20E3', 1, @now, @emptyGuid),
        (@solutionId, @workflowCapabilityId, 'C20E4', 2, @now, @emptyGuid),
        (@solutionId, @workflowCapabilityId, 'C20E5', 1, @now, @emptyGuid),
        (@solutionId, @workflowCapabilityId, 'C20E6', 2, @now, @emptyGuid),
        (@solutionId, @workflowCapabilityId, 'C20E7', 1, @now, @emptyGuid),
        (@solutionId, @workflowCapabilityId, 'C20E8', 2, @now, @emptyGuid),
        (@solutionId, @workflowCapabilityId, 'C20E9', 1, @now, @emptyGuid),
        (@solutionId, @workflowCapabilityId, 'C20E10', 2, @now, @emptyGuid),
        (@solutionId, @workflowCapabilityId, 'C20E11', 1, @now, @emptyGuid),
        (@solutionId, @workflowCapabilityId, 'C20E12', 2, @now, @emptyGuid),
        (@solutionId, @workflowCapabilityId, 'C20E13', 2, @now, @emptyGuid),
        (@solutionId, @medicineOptimizationCapabilityId, 'C30E1', 1, @now, @emptyGuid),
        (@solutionId, @medicineOptimizationCapabilityId, 'C30E2', 1, @now, @emptyGuid),
        (@solutionId, @medicineOptimizationCapabilityId, 'C30E3', 1, @now, @emptyGuid),
        (@solutionId, @medicineOptimizationCapabilityId, 'C30E4', 2, @now, @emptyGuid),
        (@solutionId, @medicineOptimizationCapabilityId, 'C30E5', 2, @now, @emptyGuid),
        (@solutionId, @medicineOptimizationCapabilityId, 'C30E6', 2, @now, @emptyGuid),
        (@solutionId, @medicineOptimizationCapabilityId, 'C30E7', 2, @now, @emptyGuid),
        (@solutionId, @medicineOptimizationCapabilityId, 'C30E8', 2, @now, @emptyGuid),
        (@solutionId, @medicineOptimizationCapabilityId, 'C30E9', 2, @now, @emptyGuid),
        (@solutionId, @medicineOptimizationCapabilityId, 'C30E10', 2, @now, @emptyGuid),
        (@solutionId, @medicineOptimizationCapabilityId, 'C30E11', 2, @now, @emptyGuid),
        (@solutionId, @medicineOptimizationCapabilityId, 'C30E12', 2, @now, @emptyGuid),
        (@solutionId, @medicineOptimizationCapabilityId, 'C30E13', 2, @now, @emptyGuid),
        (@solutionId, @medicineOptimizationCapabilityId, 'C30E14', 2, @now, @emptyGuid);
    END;

    /*************************************************************************************************************************************************************/

    SET @solutionId = '99998-98';

    IF NOT EXISTS (SELECT * FROM dbo.CatalogueItem WHERE CatalogueItemId = @solutionId)
    BEGIN
        INSERT INTO dbo.CatalogueItem(CatalogueItemId, CatalogueItemTypeId, [Name], SupplierId, PublishedStatusId, Created)
             VALUES (@solutionId, @solutionItemType, 'NotSystmOne', '99998', @publishedStatus, @now);

        INSERT INTO dbo.Solution(Id, [Version],
                    Features, ClientApplication, Hosting, ImplementationDetail,
                    IntegrationsUrl, AboutUrl, Summary, FullDescription,
                    LastUpdated, LastUpdatedBy)
        VALUES
        (
            @solutionId,
            @version1,
            '["Full Spine Compliance – EPS, PDS, SCR, eRS, GP2GP","Standards – SNOMED CT, HL7 V2, V3, FHIR, GP Connect","Appointments – Configurable Clinics, Dedicated Appointments, Visits Screens, SMS Integration","Prescribing – Acute, Repeat, Formularies, Action Groups, Decision Support","Complete Electronic Health Record (EHR)","Comprehensive consultations – Recalls, Referrals, Structured Data","Clinical Development Kit – Data Entry Templates, Views, Questionnaires, Integrated Word Letters","Full Workflow Support including Automatic Consultations","Analytics – Customisable Reports, Batch Reports, Bulk Actions, QOF Tools, Automatic Submissions","Patient Online Services – Appointment Booking, Medication Requests, Record Access, Proxy Access"]',
            '{"ClientApplicationTypes":["native-desktop"],"BrowsersSupported":[],"NativeMobileHardwareRequirements":"The OS system drive must have a drive letter of C.","NativeDesktopHardwareRequirements":"The OS system drive must have a drive letter of C.","NativeMobileFirstDesign":false,"MobileOperatingSystems":{"OperatingSystems":["Other"],"OperatingSystemsDescription":"Windows"},"MobileConnectionDetails":{"MinimumConnectionSpeed":"1Mbps","ConnectionType":["3G","4G","Wifi"],"Description":"CPU of 1 GHz or faster 32-bit or 64-bit processor"},"MobileMemoryAndStorage":{"MinimumMemoryRequirement":"1GB","Description":"4GB of free space on the C drive"},"MobileThirdParty":{"ThirdPartyComponents":"","DeviceCapabilities":"Minimum screen resolution of 1024 x 720 pixels."},"NativeDesktopOperatingSystemsDescription":"TPP supports all versions of Windows for desktops that are currently supported by Microsoft. Following verification of the configuration by TPP, installation of Windows to a virtual environment is supported to the products and versions including Virtual VMware View 5+, Citrix Xen Desktop 6+ and Microsoft Server 2012+.\r\nInstallation of the SystmOne client to any Server Operating System is not licensed by TPP. It should also be noted that both 32-bit and 64-bit versions of Microsoft Windows are supported unless otherwise stated. Windows RT is not supported.","NativeDesktopMinimumConnectionSpeed":"0.5Mbps","NativeDesktopThirdParty":{"ThirdPartyComponents":"Windows 7 requires 1GB and Office 2010 requires 256 MB. Other third party applications, shared graphics or peripherals (such as attached printers) should also be taken into account. These will all increase the amount of memory required for the computer to run smoothly.","DeviceCapabilities":"A minimum screen resolution of 1024 x 768 pixels with 16-bit colours is required. TPP recommends a minimum of a 17” TFT flat screen monitor with a resolution of 1280 x 1024 and 32-bit colours."},"NativeDesktopMemoryAndStorage":{"MinimumMemoryRequirement":"512MB","StorageRequirementsDescription":"4GB of free space on the C drive. Where a SystmOne Gateway client is used, 100GB of free space on the C drive is recommended.","MinimumCpu":"A minimum of a 2.0 GHz Pentium 4 series CPI is required.","RecommendedResolution":"5:4 – 1280 x 1024"},"NativeDesktopAdditionalInformation":"Applications that can open/view rich text file (.rtf) and comma separated (.csv) documents are required. To perform letter writing, Microsoft Word is also required. TPP only supports versions of Office that are supported by Microsoft which currently includes Office 2010, 2013, 2016 and 2019."}',
            '{"PrivateCloud":{"Summary":"The SystmOne Solution requires the following key items to be in place for smooth operation:\r\n-UDP Ports 2120-2130 and TCP Ports 2130-2140 should be opened to 20.146.120.128/25 and 20.146.248.128/25. TCP port 443 is also required for SystmOnline and Mobile Working to systmonline.tpp-uk.com. TPP also recommend allowing ICMP traffic for diagnostic purposes.\r\n\r\nA full list of requirements can be found in the SystmOne WES.","Link":"","HostingModel":"TPP provide a centralised solution with all server hardware hosted in TPP''s private cloud infrastructure. All server patching, security updates and feature releases are managed by TPP. The solution is hosted within 2 geographically separated private cloud instances with data replicated between the sites in real time in order to provide a high level of resiliency.\r\n\r\nTPP use a number of tools to monitor capacity, analyse usage trends and log the utilisation of the system. This ensures the solution scales to demand and new functionality / business requirements.","RequiresHSCN":"End user devices must be connected to HSCN/N3"}}',
            'If a greenfield unit is required, the turn-around time to receive the Live unit can be as quick as two weeks, once a signed contract is in place and all staff have received the required training.
TPP will assess the request and set up the unit as specified in the order details. Once the Live system is ready to use, TPP will be in touch with the contact who requested the unit.

When transitioning from a previous system that has a mature adapter in place (EMIS Web, Vision, Microtest), implementation is a quick rollout of 8 weeks, including data migration of any existing patient records.

The main phases for this implementation are:
•	Initial data production
•	Data checking
•	Training
•	Data reload & sign-off
•	Final data production
•	Go Live

If transitioning from any other system, an additional 8-week adapter build period would be required.

TPP maintain close contact with staff at the unit throughout these phases to ensure an efficient and accurate implementation.',
            'https://www.tpp-uk.com/partners',
            'https://www.tpp-uk.com/products/systmone',
            'SystmOne GP has been evolving for over 20 years, with continuous clinical input. It is one of the most advanced clinical systems in the world and is used by more than 2,700 GP practices nationwide. SystmOne GP is the ideal solution to meet the ever-changing needs of modern General Practice.',
            'SystmOne GP has been in use across UK General Practice for over 20 years. It is the system of choice for over 2,700 practices and is used by over 75,000 staff members. SystmOne GP is an advanced solution that goes far beyond the main functionality required for running a GP practice. It contains complete workflow support, a full analytics module, QOF tracking, and a comprehensive clinical development kit. Improving the quality of care across settings is core to TPP’s vision. The GP product is ideal for cross-organisational working and fully supports the requirements of Primary Care Networks.
            It enables true integrated care between GP, hospital, mental health and social care settings. TPP GP is Spine-accredited, providing access to the latest versions of GP2GP, EPS, and eRS. The system is fully compliant with SNOMED CT. SystmOne GP is leading on national interoperability programmes, compliant with national open FHIR standards for access to GP data and for transfer of care documentation.',
            @now,
            @emptyGuid);

        INSERT INTO dbo.SolutionCapability (SolutionId, CapabilityId, StatusId, LastUpdated, LastUpdatedBy)
        VALUES
        (@solutionId, @presciptionOrderingCapabilityId, 1, @now, @emptyGuid),
        (@solutionId, @gpAppointmentManagementCapabilityId, 1, @now, @emptyGuid),
        (@solutionId, @resourceManagementCapabilityId, 1, @now, @emptyGuid),
        (@solutionId, @unifiedCareRecordCapabilityId, 1, @now, @emptyGuid),
        (@solutionId, @referralManagementCapabilityId, 1, @now, @emptyGuid),
        (@solutionId, @communicationManagementCapabilityId, 1, @now, @emptyGuid),
        (@solutionId, @reportingCapabilityId, 1, @now, @emptyGuid),
        (@solutionId, @patientInformationMaintenanceCapabilityId, 1, @now, @emptyGuid),
        (@solutionId, @citizenAppointmentManagementCapabilityId, 1, @now, @emptyGuid),
        (@solutionId, @citizenCommunicateCapabilityId, 1, @now, @emptyGuid),
        (@solutionId, @dataAnalyticsCapabilityId, 1, @now, @emptyGuid),
        (@solutionId, @sharedCarePlansCapabilityId, 1, @now, @emptyGuid),
        (@solutionId, @recordingConsultationCapabilityId, 1, @now, @emptyGuid),
        (@solutionId, @prescribingCapabilityId, 1, @now, @emptyGuid),
        (@solutionId, @productivityCapabilityId, 1, @now, @emptyGuid),
        (@solutionId, @citizenViewRecordCapabilityId, 1, @now, @emptyGuid);

        INSERT INTO dbo.FrameworkSolutions(FrameworkId ,SolutionId ,IsFoundation, LastUpdated, LastUpdatedBy)
             VALUES (@gpitframeworkId, @solutionId , 1, @now, @emptyGuid);
    END;

    /*************************************************************************************************************************************************************/

    SET @solutionId = '99999-01';

    IF NOT EXISTS (SELECT * FROM dbo.CatalogueItem WHERE CatalogueItemId = @solutionId)
    BEGIN
        INSERT INTO dbo.CatalogueItem(CatalogueItemId, CatalogueItemTypeId, [Name], SupplierId, PublishedStatusId, Created)
             VALUES (@solutionId, @solutionItemType, 'DFOCVC Online Consultation', '99999', @publishedStatus, @now);

        INSERT INTO dbo.Solution(Id, [Version], Features, Hosting, AboutUrl, Summary, FullDescription, LastUpdated, LastUpdatedBy)
        VALUES (
            @solutionId,
            @version1,
            '["Digital Online Consultation","Video Consultation", "Fully interopable with all major GP IT solutions", "Compliant with all relevant ISO standards"]',
            '{"PublicCloud":{"Summary":"Summary description","Link":"External URL link","RequiresHSCN":"Link to HSCN or N3 network required to access service"},"PrivateCloud":{"Summary":"Summary description","Link":"External URL link","HostingModel":"Hosting environment description","RequiresHSCN":"Link to HSCN or N3 network required to access service"},"HybridHostingType":{"Summary":"Summary description","Link":"External URL link","HostingModel":"Hosting environment description","RequiresHSCN":"Link to HSCN or N3 network required to access service"},"OnPremise":{"Summary":"Summary description","Link":"External URL link","HostingModel":"Hosting environment description","RequiresHSCN":"Link to HSCN or N3 network required to access service"}}',
            'http://www.dfocvctest.com/about',
            'Summary - DFOCVC.',
            'FULL DESCRIPTION – Digital First, Online Consultation and Video Consultation Solution.',
            @now,
            @emptyGuid);

        INSERT INTO dbo.MarketingContact(SolutionId, FirstName, LastName, PhoneNumber, Email, Department, LastUpdated, LastUpdatedBy)
             VALUES (@solutionId, 'Sam', 'Samosa', '01234 567891', 'sales@test.test', 'Sales', @now, @emptyGuid);

        INSERT INTO dbo.SolutionCapability(SolutionId, CapabilityId, StatusId, LastUpdated, LastUpdatedBy)
             SELECT @solutionId, Id, 1, @now, @emptyGuid
               FROM dbo.Capability
              WHERE CapabilityRef IN ('C43');

        INSERT INTO dbo.FrameworkSolutions(FrameworkId, SolutionId, IsFoundation, LastUpdated, LastUpdatedBy)
             VALUES (@dfocvcframeworkId, @solutionId, 0, @now, @emptyGuid);
    END;

    /*************************************************************************************************************************************************************/

    SET @solutionId = '99999-02';

    IF NOT EXISTS (SELECT * FROM dbo.CatalogueItem WHERE CatalogueItemId = @solutionId)
    BEGIN
        INSERT INTO dbo.CatalogueItem(CatalogueItemId, CatalogueItemTypeId, [Name], SupplierId, PublishedStatusId, Created)
             VALUES (@solutionId, @solutionItemType, 'GPIT DFOCVC Online Consultation', '99999', @publishedStatus, @now);

        INSERT INTO dbo.Solution(Id, [Version], Features, Hosting, AboutUrl, Summary, FullDescription, LastUpdated, LastUpdatedBy)
        VALUES (
            @solutionId,
            @version1,
            '["Digital Online Consultation","Video Consultation", "Fully interopable with all major GP IT solutions", "Compliant with all relevant ISO standards"]',
            '{"PublicCloud":{"Summary":"Summary description","Link":"External URL link","RequiresHSCN":"Link to HSCN or N3 network required to access service"},"PrivateCloud":{"Summary":"Summary description","Link":"External URL link","HostingModel":"Hosting environment description","RequiresHSCN":"Link to HSCN or N3 network required to access service"},"HybridHostingType":{"Summary":"Summary description","Link":"External URL link","HostingModel":"Hosting environment description","RequiresHSCN":"Link to HSCN or N3 network required to access service"},"OnPremise":{"Summary":"Summary description","Link":"External URL link","HostingModel":"Hosting environment description","RequiresHSCN":"Link to HSCN or N3 network required to access service"}}',
            'http://www.gpitdfocvctest.com/about',
            'Summary - GPIT DFOCVC.',
            'FULL DESCRIPTION – GPIT Futures, Digital First Online Consultation and Video Consultation Solution.',
            @now,
            @emptyGuid);

        INSERT INTO dbo.MarketingContact(SolutionId, FirstName, LastName, PhoneNumber, Email, Department, LastUpdated, LastUpdatedBy)
             VALUES (@solutionId, 'Sam', 'Samosa', '01234 567891', 'sales@test.test', 'Sales', @now, @emptyGuid);

        INSERT INTO dbo.SolutionCapability(SolutionId, CapabilityId, StatusId, LastUpdated, LastUpdatedBy)
             SELECT @solutionId, Id, 1, @now, @emptyGuid
               FROM dbo.Capability
              WHERE CapabilityRef IN ('C44');

        INSERT INTO dbo.FrameworkSolutions(FrameworkId, SolutionId, IsFoundation, LastUpdated, LastUpdatedBy)
             VALUES (@gpitframeworkId, @solutionId, 1, @now, @emptyGuid),
                    (@dfocvcframeworkId, @solutionId, 0, @now, @emptyGuid);
    END;

    DECLARE @flatPriceType AS int = 1;
    DECLARE @tieredPriceType AS int = 2;

    DECLARE @patientProvisioningType AS int = 1;
    DECLARE @declarativeProvisioningType AS int = 2;
    DECLARE @onDemandProvisioningType AS int = 3;

    DECLARE @monthTimeUnit AS int = 1;
    DECLARE @yearTimeUnit AS int = 2;

    /* Insert prices */
    IF NOT EXISTS (SELECT * FROM dbo.CataloguePrice)
    BEGIN
     INSERT INTO dbo.CataloguePrice(CatalogueItemId, ProvisioningTypeId, CataloguePriceTypeId, PricingUnitId, TimeUnitId, CurrencyCode, LastUpdated, Price)
          VALUES ('100000-001', @patientProvisioningType, @flatPriceType, 'F8D06518-1A20-4FBA-B369-AB583F9FA8C0', @yearTimeUnit, 'GBP', @now, 99.99),
                 ('100000-001', @patientProvisioningType, @tieredPriceType, 'F8D06518-1A20-4FBA-B369-AB583F9FA8C0', @yearTimeUnit, 'GBP', @now, NULL),
                 ('100000-001', @onDemandProvisioningType, @flatPriceType, '774E5A1D-D15C-4A37-9990-81861BEAE42B', NULL, 'GBP', @now, 1001.010),
                 ('100001-001', @onDemandProvisioningType, @flatPriceType, '8BF9C2F9-2FD7-4A29-8406-3C6B7B2E5D65', NULL, 'GBP', @now, 3.142),
                 ('100002-001', @declarativeProvisioningType, @flatPriceType, 'F8D06518-1A20-4FBA-B369-AB583F9FA8C0', @monthTimeUnit, 'GBP', @now, 4.85),
                 ('100002-001', @declarativeProvisioningType, @tieredPriceType, 'F8D06518-1A20-4FBA-B369-AB583F9FA8C0', @monthTimeUnit, 'GBP', @now, NULL),
                 ('100003-001', @declarativeProvisioningType, @flatPriceType, 'D43C661A-0587-45E1-B315-5E5091D6E9D0', @monthTimeUnit, 'GBP', @now, 19.987),
                 ('100004-001', @declarativeProvisioningType, @flatPriceType, '8BF9C2F9-2FD7-4A29-8406-3C6B7B2E5D65', @monthTimeUnit, 'GBP', @now, 10101.65),
                 ('100005-001', @onDemandProvisioningType, @flatPriceType, '8BF9C2F9-2FD7-4A29-8406-3C6B7B2E5D65', NULL, 'GBP', @now, 456),
                 ('100006-001', @declarativeProvisioningType, @flatPriceType, '90119522-D381-4296-82EE-8FE630593B56', @monthTimeUnit, 'GBP', @now, 7),
                 ('100007-001', @onDemandProvisioningType, @flatPriceType, '90119522-D381-4296-82EE-8FE630593B56', NULL, 'GBP', @now, 0.15),
                 ('100007-002', @onDemandProvisioningType, @tieredPriceType, '90119522-D381-4296-82EE-8FE630593B56', NULL, 'GBP', @now, NULL),
                 ('99998-98', @patientProvisioningType, @flatPriceType, '8BF9C2F9-2FD7-4A29-8406-3C6B7B2E5D65', @yearTimeUnit, 'GBP', @now, 30000),
                 ('99998-98', @patientProvisioningType, @tieredPriceType, '8BF9C2F9-2FD7-4A29-8406-3C6B7B2E5D65', @yearTimeUnit, 'GBP', @now, NULL),
                 ('99999-01', @patientProvisioningType, @flatPriceType, 'F8D06518-1A20-4FBA-B369-AB583F9FA8C0', @yearTimeUnit, 'GBP', @now, 1.25),
                 ('99999-02', @patientProvisioningType, @flatPriceType, 'F8D06518-1A20-4FBA-B369-AB583F9FA8C0', @yearTimeUnit, 'GBP', @now, 1.55),
                 ('99999-89', @patientProvisioningType, @flatPriceType, '8BF9C2F9-2FD7-4A29-8406-3C6B7B2E5D65', @yearTimeUnit, 'GBP', @now, 500.49),
                 ('99999-89', @patientProvisioningType, @tieredPriceType, '8BF9C2F9-2FD7-4A29-8406-3C6B7B2E5D65', @yearTimeUnit, 'GBP', @now, NULL);

          -- Tiered price IDs
          DECLARE @priceId1000001 AS int = (SELECT CataloguePriceId from dbo.CataloguePrice WHERE CatalogueItemId = '100000-001' AND CataloguePriceTypeId = @tieredPriceType);
          DECLARE @priceId1000021 AS int = (SELECT CataloguePriceId from dbo.CataloguePrice WHERE CatalogueItemId = '100002-001' AND CataloguePriceTypeId = @tieredPriceType);
          DECLARE @priceId1000072 AS int = (SELECT CataloguePriceId from dbo.CataloguePrice WHERE CatalogueItemId = '100007-002' AND CataloguePriceTypeId = @tieredPriceType);
          DECLARE @priceId9999898 AS int = (SELECT CataloguePriceId from dbo.CataloguePrice WHERE CatalogueItemId = '99998-98' AND CataloguePriceTypeId = @tieredPriceType);
          DECLARE @priceId9999989 AS int = (SELECT CataloguePriceId from dbo.CataloguePrice WHERE CatalogueItemId = '99999-89' AND CataloguePriceTypeId = @tieredPriceType);

          INSERT INTO dbo.CataloguePriceTier(CataloguePriceId, BandStart, BandEnd, Price)
               VALUES (@priceId1000001, 1, 999, 123.45),
                      (@priceId1000001, 1000, 1999, 49.99),
                      (@priceId1000001, 2000, NULL, 19.99),
                      (@priceId1000021, 1, 10, 200),
                      (@priceId1000021, 11, 99, 150.15),
                      (@priceId1000021, 100, NULL, 99.99),
                      (@priceId9999898, 1, 10000, 500),
                      (@priceId9999898, 10001, NULL, 499.99),
                      (@priceId9999989, 1, 8, 42.42),
                      (@priceId9999989, 9, 33,33.33),
                      (@priceId9999989, 34, 1004, 50),
                      (@priceId9999989, 1005, NULL, 0.02),
                      (@priceId1000072, 1, 10, 20),
                      (@priceId1000072, 11, 99, 30.15),
                      (@priceId1000072, 100, NULL, 40.99);
     END;
END;
GO
