DECLARE @citizenAppointmentManagementCapabilityId AS int = 1;
DECLARE @citizenCommunicateCapabilityId AS int = 2;
DECLARE @citizenViewRecordCapabilityId AS int = 4;
DECLARE @clinicalDecisionSupportCapabilityId AS int = 6;
DECLARE @communicationManagementCapabilityId AS int = 7;
DECLARE @dataAnalyticsCapabilityId AS int = 26;
DECLARE @gpAppointmentManagementCapabilityId AS int = 5;
DECLARE @gpExtractVerificationCapabilityId AS int = 10;
DECLARE @medicineOptimizationCapabilityId AS int = 30;
DECLARE @patientInformationMaintenanceCapabilityId AS int = 13;
DECLARE @prescribingCapabilityId AS int = 14;
DECLARE @presciptionOrderingCapabilityId AS int = 3;
DECLARE @recordingConsultationCapabilityId AS int = 15;
DECLARE @productivityCapabilityId AS int = 41;
DECLARE @referralManagementCapabilityId AS int = 11;
DECLARE @reportingCapabilityId AS int = 16;
DECLARE @resourceManagementCapabilityId AS int = 12;
DECLARE @scanningCapabilityId AS int = 17;
DECLARE @sharedCarePlansCapabilityId AS int = 36;
DECLARE @unifiedCareRecordCapabilityId AS int = 39;
DECLARE @workflowCapabilityId AS int = 20;

DECLARE @onlineConsultationCapabilityId AS int = 43;
DECLARE @videoConsultationCapabilityId AS int = 44;

DECLARE @tifFrameworkId AS NVARCHAR(10) = 'TIF001';
DECLARE @gpitframeworkId AS nvarchar(10) = 'NHSDGP001';
DECLARE @dfocvcframeworkId AS nvarchar(10) = 'DFOCVC001';
DECLARE @bobEmail AS nvarchar(50) = N'BobSmith@email.com';
DECLARE @bobUser AS int = (SELECT Id FROM users.AspNetUsers WHERE Email = @bobEmail);
DECLARE @now AS datetime = GETUTCDATE();

DECLARE @publishedStatus AS int = 3;
DECLARE @solutionItemType AS int = 1;

DECLARE @solutionId AS nvarchar(14);

IF UPPER('$(INSERT_TEST_DATA)') = 'TRUE' AND NOT EXISTS (SELECT * FROM catalogue.Solutions)
BEGIN
    SET @solutionId = '100000-001';

    IF NOT EXISTS (SELECT * FROM catalogue.CatalogueItems WHERE Id = @solutionId)
    BEGIN
        INSERT INTO catalogue.CatalogueItems(Id, CatalogueItemTypeId, [Name], SupplierId, PublishedStatusId, Created)
             VALUES (@solutionId, @solutionItemType, 'Write on Time', 100000, @publishedStatus, @now);

        INSERT INTO catalogue.Solutions(CatalogueItemId, Features, Hosting, Integrations, AboutUrl, Summary, FullDescription, LastUpdated, LastUpdatedBy)
        VALUES (
            @solutionId,
            '["Flexible Pricing", "Lightweight interface designed for maximum usability", "DNA tracking and automatic improvement suggestions", "Web-based", "Remotely accessible"]',
            '{"PublicCloud":{"Summary":"Summary description","Link":"External URL link","RequiresHSCN":"Link to HSCN or N3 network required to access service"},"PrivateCloud":{"Summary":"Summary description","Link":"External URL link","HostingModel":"Hosting environment description","RequiresHSCN":"Link to HSCN or N3 network required to access service"},"HybridHostingType":{"Summary":"Summary description","Link":"External URL link","HostingModel":"Hosting environment description","RequiresHSCN":"Link to HSCN or N3 network required to access service"},"OnPremise":{"Summary":"Summary description","Link":"External URL link","HostingModel":"Hosting environment description","RequiresHSCN":"Link to HSCN or N3 network required to access service"}}',
            '[{"Id":"1c8be5da-5775-4517-8a13-f3f15a113cc2","IntegrationType":"IM1","Qualifier":"Bulk","IsConsumer":false,"IntegratesWith":"Audit+","Description":"Audit+ utilises a bulk extraction of full clinical records (including confidential and deceased patients) from EMIS Web to provide General Practices with a crossplatform clinical decision support and management tool; supporting QOF performance management, improvement and NHS Health Checks.","AdditionalInformation":""},{"Id":"f10b30e0-b590-463f-aef7-1cf950c5ca22","IntegrationType":"IM1","Qualifier":"Bulk","IsConsumer":false,"IntegratesWith":"Informatics Platform","Description":"A Risk Stratification tool","AdditionalInformation":""},{"Id":"0bf49a99-4e1f-4824-bb61-d1d3bb4fe0d4","IntegrationType":"IM1","Qualifier":"Bulk","IsConsumer":false,"IntegratesWith":"Censure","Description":"Storm is a sophisticated management information reporting interface that presents EMIS Web, allowing users to display their data in the most impactful and effective format; selecting from a range of arrangements and graphical forms.","AdditionalInformation":""},{"Id":"228f6b1e-951d-485b-938c-6a872bd996f5","IntegrationType":"IM1","Qualifier":"Transactional","IsConsumer":false,"IntegratesWith":"Audit+","Description":"Audit+ utilises a bulk extraction of full clinical records (including confidential and deceased patients) from EMIS Web to provide General Practices with a crossplatform clinical decision support and management tool; supporting QOF performance management, improvement and NHS Health Checks.","AdditionalInformation":""},{"Id":"0a0da5c2-6609-4fd3-a7d4-b07227a3296f","IntegrationType":"IM1","Qualifier":"Transactional","IsConsumer":false,"IntegratesWith":"Informatics Platform","Description":"A Risk Stratification tool","AdditionalInformation":""},{"Id":"d5c05642-63f3-4897-8d22-faf16b113936","IntegrationType":"IM1","Qualifier":"Transactional","IsConsumer":false,"IntegratesWith":"Censure","Description":"Storm is a sophisticated management information reporting interface that presents EMIS Web, allowing users to display their data in the most impactful and effective format; selecting from a range of arrangements and graphical forms.","AdditionalInformation":""},{"Id":"5504ccc8-4852-4901-a55c-36f49ac2271a","IntegrationType":"GP Connect","Qualifier":"Access Record HTML","IsConsumer":false,"IntegratesWith":"","Description":"","AdditionalInformation":"EMIS Web received Full Roll Out Approval from NHS Digital for GP Connect HTML View Provision on 20/06/19"},{"Id":"a28b4c39-ac70-4acc-b47e-470a3d9726f4","IntegrationType":"GP Connect","Qualifier":"Access Record HTML","IsConsumer":true,"IntegratesWith":"","Description":"","AdditionalInformation":"EMIS Web is accredited to consume GP Connect HTML views"},{"Id":"b7eb68af-0fb6-4cc1-9c24-f2f7a0719760","IntegrationType":"GP Connect","Qualifier":"Appointment Management","IsConsumer":false,"IntegratesWith":"","Description":"","AdditionalInformation":"EMIS Web received Full Roll Out Approval from NHS Digital for GP Connect HTML View Provision on 20/06/19"},{"Id":"0700d184-eaf8-4e01-98f6-7d3029da43c2","IntegrationType":"GP Connect","Qualifier":"Appointment Management","IsConsumer":true,"IntegratesWith":"","Description":"","AdditionalInformation":"EMIS Web is accredited to consume GP Connect HTML views"}]',
            'http://www.writeontime.com/about',
            'Write on Time is a Citizen-facing Appointments Management system specifically designed to reduce the number of DNAs in your practice.',
            'FULL DESCRIPTION – Write on Time is a Citizen-facing Appointments Management system specifically designed to reduce the number of DNAs in your practice.',
            @now,
            @bobUser);

        INSERT INTO catalogue.MarketingContacts(SolutionId, FirstName, LastName, PhoneNumber, Email, Department, LastUpdated, LastUpdatedBy)
             VALUES (@solutionId, 'Pat', 'Butcher', '01234 567891', 'sales@test.test', 'Sales', @now, @bobUser);

        INSERT INTO catalogue.CatalogueItemCapabilities(CatalogueItemId, CapabilityId, StatusId, LastUpdated, LastUpdatedBy)
             SELECT @solutionId, Id, 1, @now, @bobUser
               FROM catalogue.Capabilities
              WHERE CapabilityRef = 'C1';

        INSERT INTO catalogue.FrameworkSolutions(FrameworkId ,SolutionId ,IsFoundation, LastUpdated, LastUpdatedBy)
             VALUES (@tifFrameworkId, @solutionId , 1, @now, @bobUser);
    END;

    /*************************************************************************************************************************************************************/

    SET @solutionId = '100001-001';

    IF NOT EXISTS (SELECT * FROM catalogue.CatalogueItems WHERE Id = @solutionId)
    BEGIN
        INSERT INTO catalogue.CatalogueItems(Id, CatalogueItemTypeId, [Name], SupplierId, PublishedStatusId, Created)
             VALUES (@solutionId, @solutionItemType, 'Appointment Gateway', 100001, @publishedStatus, @now);

        INSERT INTO catalogue.Solutions(CatalogueItemId, Features, Hosting, AboutUrl, Summary, FullDescription, LastUpdated, LastUpdatedBy)
        VALUES (
            @solutionId,
            '["Tested and approved by hundred''s of GPs", "99.9% service availability guaranteed", "Appointment forwarding & referral integration", "Fully interopable with all major GP IT solutions", "Compliant with all relevant ISO standards"]',
            '{"PublicCloud":{"Summary":"Summary description","Link":"External URL link","RequiresHSCN":"Link to HSCN or N3 network required to access service"},"PrivateCloud":{"Summary":"Summary description","Link":"External URL link","HostingModel":"Hosting environment description","RequiresHSCN":"Link to HSCN or N3 network required to access service"},"HybridHostingType":{"Summary":"Summary description","Link":"External URL link","HostingModel":"Hosting environment description","RequiresHSCN":"Link to HSCN or N3 network required to access service"},"OnPremise":{"Summary":"Summary description","Link":"External URL link","HostingModel":"Hosting environment description","RequiresHSCN":"Link to HSCN or N3 network required to access service"}}',
            'http://www.appointmentgateway.com/about',
            'Appointment Gateway is a complete appointment management suite that has been fully integrated with all major clinical systems throughout both the UK and Europe.',
            'FULL DESCRIPTION – Appointment Gateway is a complete appointment management suite that has been fully integrated with all major clinical systems throughout both the UK and Europe.',
            @now,
            @bobUser);

        INSERT INTO catalogue.MarketingContacts(SolutionId, FirstName, LastName, PhoneNumber, Email, Department, LastUpdated, LastUpdatedBy)
             VALUES (@solutionId, 'Sam', 'Samosa', '01234 567891', 'sales@test.test', 'Sales', @now, @bobUser);

        INSERT INTO catalogue.CatalogueItemCapabilities(CatalogueItemId, CapabilityId, StatusId, LastUpdated, LastUpdatedBy)
             SELECT @solutionId, Id, 1, @now, @bobUser
               FROM catalogue.Capabilities
              WHERE CapabilityRef IN ('C1', 'C5');

        INSERT INTO catalogue.FrameworkSolutions(FrameworkId, SolutionId, IsFoundation, LastUpdated, LastUpdatedBy)
             VALUES (@tifFrameworkId, @solutionId, 1, @now, @bobUser);
    END;

    /*************************************************************************************************************************************************************/

    SET @solutionId = '100002-001';

    IF NOT EXISTS (SELECT * FROM catalogue.CatalogueItems WHERE Id = @solutionId)
    BEGIN
        INSERT INTO catalogue.CatalogueItems(Id, CatalogueItemTypeId, [Name], SupplierId, PublishedStatusId, Created)
             VALUES (@solutionId, @solutionItemType, 'Zen Guidance', 100002, @publishedStatus, @now);

        INSERT INTO catalogue.Solutions(CatalogueItemId, Features, Hosting, AboutUrl, Summary, FullDescription, LastUpdated, LastUpdatedBy)
        VALUES (
            @solutionId,
            '["Advanced AI functionality", "MESH & FHIR compliant", "Remotely accessible ", "Cloud-hosted", "24/7 customer support"]',
            '{"PublicCloud":{"Summary":"Summary description","Link":"External URL link","RequiresHSCN":"Link to HSCN or N3 network required to access service"},"PrivateCloud":{"Summary":"Summary description","Link":"External URL link","HostingModel":"Hosting environment description","RequiresHSCN":"Link to HSCN or N3 network required to access service"},"HybridHostingType":{"Summary":"Summary description","Link":"External URL link","HostingModel":"Hosting environment description","RequiresHSCN":"Link to HSCN or N3 network required to access service"},"OnPremise":{"Summary":"Summary description","Link":"External URL link","HostingModel":"Hosting environment description","RequiresHSCN":"Link to HSCN or N3 network required to access service"}}',
            'http://www.zenguidance.com/about',
            'Zen Guidance utilizes an advanced AI framework to provide clinicians with highly accurate data to support sound decision-making.',
            'FULL DESCRIPTION – Zen Guidance utilizes an advanced AI framework to provide clinicians with highly accurate data to support sound decision-making.',
            @now,
            @bobUser);

        INSERT INTO catalogue.MarketingContacts(SolutionId, FirstName, LastName, PhoneNumber, Email, Department, LastUpdated, LastUpdatedBy)
             VALUES (@solutionId, 'Victoria', 'Sponge', '01234 567891', 'sales@test.test', 'Sales', @now, @bobUser);

        INSERT INTO catalogue.CatalogueItemCapabilities (CatalogueItemId, CapabilityId, StatusId, LastUpdated, LastUpdatedBy)
             SELECT @solutionId, Id, 1, @now, @bobUser
               FROM catalogue.Capabilities
              WHERE CapabilityRef = 'C6';
    END;

    /*************************************************************************************************************************************************************/

    SET @solutionId = '100003-001';

    IF NOT EXISTS (SELECT * FROM catalogue.CatalogueItems WHERE Id = @solutionId)
    BEGIN
        INSERT INTO catalogue.CatalogueItems(Id, CatalogueItemTypeId, [Name], SupplierId, PublishedStatusId, Created)
             VALUES (@solutionId, @solutionItemType, 'Intellidoc Comms', 100003, @publishedStatus, @now);

        INSERT INTO catalogue.Solutions(CatalogueItemId, Features, Hosting, AboutUrl, Summary, FullDescription, LastUpdated, LastUpdatedBy)
        VALUES (
            @solutionId,
            '["Efficient instant & scheduled messaging", "Web-based interface", "Compliant with all relevant ISO standards", "Wide range of add-ons available", "Cloud-hosted"]',
            '{"PublicCloud":{"Summary":"Summary description","Link":"External URL link","RequiresHSCN":"Link to HSCN or N3 network required to access service"},"PrivateCloud":{"Summary":"Summary description","Link":"External URL link","HostingModel":"Hosting environment description","RequiresHSCN":"Link to HSCN or N3 network required to access service"},"HybridHostingType":{"Summary":"Summary description","Link":"External URL link","HostingModel":"Hosting environment description","RequiresHSCN":"Link to HSCN or N3 network required to access service"},"OnPremise":{"Summary":"Summary description","Link":"External URL link","HostingModel":"Hosting environment description","RequiresHSCN":"Link to HSCN or N3 network required to access service"}}',
            'http://www.intellidoc.com/about',
            'Intellidoc Comms empowers all practice staff to record & send communications in an extremely streamlined and time-efficient manner.',
            'FULL DESCRIPTION – Intellidoc Comms empowers all practice staff to record & send communications in an extremely streamlined and time-efficient manner.',
            @now,
            @bobUser);

        INSERT INTO catalogue.MarketingContacts(SolutionId, FirstName, LastName, PhoneNumber, Email, Department, LastUpdated, LastUpdatedBy)
             VALUES (@solutionId, 'Richard', 'Burton', '01234 567891', 'sales@test.test', 'Sales', @now, @bobUser);

        INSERT INTO catalogue.CatalogueItemCapabilities(CatalogueItemId, CapabilityId, StatusId, LastUpdated, LastUpdatedBy)
             SELECT @solutionId, Id, 1, @now, @bobUser
               FROM catalogue.Capabilities
              WHERE CapabilityRef IN ('C7', 'C15');
    END;

    /*************************************************************************************************************************************************************/

    SET @solutionId = '100004-001';

    IF NOT EXISTS (SELECT * FROM catalogue.CatalogueItems WHERE Id = @solutionId)
    BEGIN
        INSERT INTO catalogue.CatalogueItems(Id, CatalogueItemTypeId, [Name], SupplierId, PublishedStatusId, Created)
             VALUES (@solutionId, @solutionItemType, 'Diagnostics XYZ', 100004, @publishedStatus, @now);

        INSERT INTO catalogue.Solutions(CatalogueItemId, Features, Hosting, AboutUrl, Summary, FullDescription, LastUpdated, LastUpdatedBy)
        VALUES (
            @solutionId,
            '["Seamless integration with a wide range of diagnostic hardware", "Demo & free trial available", "FHIR compliant", "Plug and play – minimal deployment activity required", "Optimized for touchscreen & desktop use"]',
            '{"PublicCloud":{"Summary":"Summary description","Link":"External URL link","RequiresHSCN":"Link to HSCN or N3 network required to access service"},"PrivateCloud":{"Summary":"Summary description","Link":"External URL link","HostingModel":"Hosting environment description","RequiresHSCN":"Link to HSCN or N3 network required to access service"},"HybridHostingType":{"Summary":"Summary description","Link":"External URL link","HostingModel":"Hosting environment description","RequiresHSCN":"Link to HSCN or N3 network required to access service"},"OnPremise":{"Summary":"Summary description","Link":"External URL link","HostingModel":"Hosting environment description","RequiresHSCN":"Link to HSCN or N3 network required to access service"}}',
            'http://www.diagnostics.xyz/about',
            'Diagnostics XYZ introduces new diagnostic tools not currently provided by the leading clinical software suppliers.',
            'FULL DESCRIPTION – Diagnostics XYZ introduces new diagnostic tools not currently provided by the leading clinical software suppliers.',
            @now,
            @bobUser);

        INSERT INTO catalogue.MarketingContacts(SolutionId, FirstName, LastName, PhoneNumber, Email, Department, LastUpdated, LastUpdatedBy)
             VALUES (@solutionId, 'Harry', 'Houdini', '01234 567891', 'sales@test.test', 'Sales', @now, @bobUser);

        INSERT INTO catalogue.CatalogueItemCapabilities(CatalogueItemId, CapabilityId, StatusId, LastUpdated, LastUpdatedBy)
             SELECT @solutionId, Id, 1, @now, @bobUser
               FROM catalogue.Capabilities
              WHERE CapabilityRef = 'C8';

        INSERT INTO catalogue.FrameworkSolutions(FrameworkId ,SolutionId ,IsFoundation, LastUpdated, LastUpdatedBy)
             VALUES (@tifFrameworkId, @solutionId , 1, @now, @bobUser);
    END;

    /*************************************************************************************************************************************************************/

    SET @solutionId = '100005-001';

    IF NOT EXISTS (SELECT * FROM catalogue.CatalogueItems WHERE Id = @solutionId)
    BEGIN
        INSERT INTO catalogue.CatalogueItems(Id, CatalogueItemTypeId, [Name], SupplierId, PublishedStatusId, Created)
             VALUES (@solutionId, @solutionItemType, 'Document Wizard', 100005, @publishedStatus, @now);

        INSERT INTO catalogue.Solutions(CatalogueItemId, Features, Hosting, AboutUrl, Summary, FullDescription, LastUpdated, LastUpdatedBy)
        VALUES (
            @solutionId,
            '["Industry-leading data extraction & scanning accuracy", "Fully interopable with all major GP IT solutions", "24/7 customer support", "Fully Compliant with all relevant ISO standards", "Modular architecture to enhance compatibility and customisation"]',
            '{"PublicCloud":{"Summary":"Summary description","Link":"External URL link","RequiresHSCN":"Link to HSCN or N3 network required to access service"},"PrivateCloud":{"Summary":"Summary description","Link":"External URL link","HostingModel":"Hosting environment description","RequiresHSCN":"Link to HSCN or N3 network required to access service"},"HybridHostingType":{"Summary":"Summary description","Link":"External URL link","HostingModel":"Hosting environment description","RequiresHSCN":"Link to HSCN or N3 network required to access service"},"OnPremise":{"Summary":"Summary description","Link":"External URL link","HostingModel":"Hosting environment description","RequiresHSCN":"Link to HSCN or N3 network required to access service"}}',
            'http://www.documentwizard.com/about',
            'Document Wizard is the UK industry-leader for clinical document management software due to our patented lightweight interface and interoperability.',
            'FULL DESCRIPTION – Document Wizard is the UK industry-leader for clinical document management software due to our patented lightweight interface and interoperability.',
            @now,
            @bobUser);

        INSERT INTO catalogue.MarketingContacts(SolutionId, FirstName, LastName, PhoneNumber, Email, Department, LastUpdated, LastUpdatedBy)
             VALUES (@solutionId, 'Agent', 'M', '01234 567891', 'm@test.test', 'Sales', @now, @bobUser);

        INSERT INTO catalogue.CatalogueItemCapabilities(CatalogueItemId, CapabilityId, StatusId, LastUpdated, LastUpdatedBy)
             SELECT @solutionId, Id, 1, @now, @bobUser
               FROM catalogue.Capabilities
              WHERE CapabilityRef IN ('C9', 'C19', 'C41');
    END;

    /*************************************************************************************************************************************************************/

    SET @solutionId = '100006-001';

    IF NOT EXISTS (SELECT * FROM catalogue.CatalogueItems WHERE Id = @solutionId)
    BEGIN
        INSERT INTO catalogue.CatalogueItems(Id, CatalogueItemTypeId, [Name], SupplierId, PublishedStatusId, Created)
             VALUES (@solutionId, @solutionItemType, 'Paperlite', 100006, @publishedStatus, @now);

        INSERT INTO catalogue.Solutions(CatalogueItemId, Features, Hosting, AboutUrl, Summary, FullDescription, LastUpdated, LastUpdatedBy)
        VALUES (
            @solutionId,
            '["Revolutionary optical character recognition technology", "Can be deployed quickly at low-cost", "Web-based interface", "Cloud-hosted", "Wide range of add-ons available"]',
            '{"PublicCloud":{"Summary":"Summary description","Link":"External URL link","RequiresHSCN":"Link to HSCN or N3 network required to access service"},"PrivateCloud":{"Summary":"Summary description","Link":"External URL link","HostingModel":"Hosting environment description","RequiresHSCN":"Link to HSCN or N3 network required to access service"},"HybridHostingType":{"Summary":"Summary description","Link":"External URL link","HostingModel":"Hosting environment description","RequiresHSCN":"Link to HSCN or N3 network required to access service"},"OnPremise":{"Summary":"Summary description","Link":"External URL link","HostingModel":"Hosting environment description","RequiresHSCN":"Link to HSCN or N3 network required to access service"}}',
            'http://www.paperlite.com/about',
            'Paperlite utilises new OCR technology to seamlessly transfer written notes to digital patient records.',
            'FULL DESCRIPTION – Paperlite utilises new OCR technology to seamlessly transfer written notes to digital patient records.',
            @now,
            @bobUser);

        INSERT INTO catalogue.MarketingContacts(SolutionId, FirstName, LastName, PhoneNumber, Email, Department, LastUpdated, LastUpdatedBy)
             VALUES ('100006-001', 'Timothy', 'Teabag', '01234 567891', 'sales@test.test', 'Sales', @now, @bobUser);

        INSERT INTO catalogue.CatalogueItemCapabilities(CatalogueItemId, CapabilityId, StatusId, LastUpdated, LastUpdatedBy)
             SELECT '100006-001', Id, 1, @now, @bobUser
               FROM catalogue.Capabilities
              WHERE CapabilityRef IN ('C9', 'C17');
    END;

    /*************************************************************************************************************************************************************/

    SET @solutionId = '100007-001';

    IF NOT EXISTS (SELECT * FROM catalogue.CatalogueItems WHERE Id = @solutionId)
    BEGIN
        INSERT INTO catalogue.CatalogueItems(Id, CatalogueItemTypeId, [Name], SupplierId, PublishedStatusId, Created)
             VALUES (@solutionId, @solutionItemType, 'Medsort', 100007, @publishedStatus, @now);

        INSERT INTO catalogue.Solutions(CatalogueItemId, Features, Hosting, AboutUrl, Summary, FullDescription, LastUpdated, LastUpdatedBy)
        VALUES (
            @solutionId,
            '["Fully adaptable to suit your practice''s needs", "Integrates with Spine", "FHIR compliant", "Flexible Pricing", "24/7 customer support"]',
            '{"PublicCloud":{"Summary":"Summary description","Link":"External URL link","RequiresHSCN":"Link to HSCN or N3 network required to access service"},"PrivateCloud":{"Summary":"Summary description","Link":"External URL link","HostingModel":"Hosting environment description","RequiresHSCN":"Link to HSCN or N3 network required to access service"},"HybridHostingType":{"Summary":"Summary description","Link":"External URL link","HostingModel":"Hosting environment description","RequiresHSCN":"Link to HSCN or N3 network required to access service"},"OnPremise":{"Summary":"Summary description","Link":"External URL link","HostingModel":"Hosting environment description","RequiresHSCN":"Link to HSCN or N3 network required to access service"}}',
            'http://www.medsort.com/about',
            'Medsort enhances your medicine optimisation process and introduces new, more customisable tools that can be adapted to your local environment.',
            'FULL DESCRIPTION – Medsort enhances your medicine optimisation process and introduces new, more customisable tools that can be adapted to your local environment.',
            @now,
            @bobUser);

        INSERT INTO catalogue.MarketingContacts(SolutionId, FirstName, LastName, PhoneNumber, Email, Department, LastUpdated, LastUpdatedBy)
             VALUES (@solutionId, 'Betty', 'Banjo', '01234 567891', 'sales@test.test', 'Sales', @now, @bobUser);

        INSERT INTO catalogue.CatalogueItemCapabilities(CatalogueItemId, CapabilityId, StatusId, LastUpdated, LastUpdatedBy)
             SELECT @solutionId, Id, 1, @now, @bobUser
               FROM catalogue.Capabilities
              WHERE CapabilityRef = 'C30';
    END;

    /*************************************************************************************************************************************************************/

    SET @solutionId = '100007-002';
    IF NOT EXISTS (SELECT * FROM catalogue.CatalogueItems WHERE Id = @solutionId)
    BEGIN
        INSERT INTO catalogue.CatalogueItems(Id, CatalogueItemTypeId, [Name], SupplierId, PublishedStatusId, Created)
             VALUES (@solutionId, @solutionItemType, 'BostonDynamics', 100007, @publishedStatus, @now);

        INSERT INTO catalogue.Solutions(CatalogueItemId, Features, Hosting, AboutUrl, Summary, FullDescription, LastUpdated, LastUpdatedBy)
        VALUES (
            @solutionId,
            '["Fully adaptable to suit your practice''s needs", "Integrates with Spine", "FHIR compliant", "Flexible Pricing", "24/7 customer support"]',
            '{"PublicCloud":{"Summary":"Summary description","Link":"External URL link","RequiresHSCN":"Link to HSCN or N3 network required to access service"},"PrivateCloud":{"Summary":"Summary description","Link":"External URL link","HostingModel":"Hosting environment description","RequiresHSCN":"Link to HSCN or N3 network required to access service"},"HybridHostingType":{"Summary":"Summary description","Link":"External URL link","HostingModel":"Hosting environment description","RequiresHSCN":"Link to HSCN or N3 network required to access service"},"OnPremise":{"Summary":"Summary description","Link":"External URL link","HostingModel":"Hosting environment description","RequiresHSCN":"Link to HSCN or N3 network required to access service"}}',
            'http://www.bostondynamics.com/about',
            'BostonDynamics enhances your medicine optimisation process and introduces new, more customisable tools that can be adapted to your local environment.',
            'FULL DESCRIPTION – BostonDynamics enhances your medicine optimisation process and introduces new, more customisable tools that can be adapted to your local environment.',
            @now,
            @bobUser);

        INSERT INTO catalogue.MarketingContacts(SolutionId, FirstName, LastName, PhoneNumber, Email, Department, LastUpdated, LastUpdatedBy)
             VALUES (@solutionId, 'Boston', 'Rocks', '01234 567891', 'sales@test.test', 'Sales', @now, @bobUser);

        INSERT INTO catalogue.CatalogueItemCapabilities(CatalogueItemId, CapabilityId, StatusId, LastUpdated, LastUpdatedBy)
             SELECT @solutionId, Id, 1, @now, @bobUser
               FROM catalogue.Capabilities
              WHERE CapabilityRef = 'C30';
    END;

    /*************************************************************************************************************************************************************/

    SET @solutionId = '99999-89';

    IF NOT EXISTS (SELECT * FROM catalogue.CatalogueItems WHERE Id = @solutionId)
    BEGIN
        INSERT INTO catalogue.CatalogueItems(Id, CatalogueItemTypeId, [Name], SupplierId, PublishedStatusId, Created)
             VALUES (@solutionId, @solutionItemType, 'NotEmis Web GP', 99999, @publishedStatus, @now);

        INSERT INTO catalogue.Solutions(CatalogueItemId,
                    Features, ClientApplication, Hosting, ImplementationDetail, RoadMap,
                    IntegrationsUrl, Summary, FullDescription,
                    LastUpdated, LastUpdatedBy)
        VALUES
        (
            @solutionId,
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
            @bobUser);

        INSERT INTO catalogue.MarketingContacts(SolutionId, FirstName, LastName, Email, PhoneNumber, Department, LastUpdated, LastUpdatedBy)
             VALUES (@solutionId, 'Eduardo', 'Eggbert', 'eddie@eggs.test', '01234 567891', 'Internal Sales Team', @now, @bobUser);

        INSERT INTO catalogue.CatalogueItemCapabilities (CatalogueItemId, CapabilityId, StatusId, LastUpdated, LastUpdatedBy)
        VALUES
        (@solutionId, @gpAppointmentManagementCapabilityId, 1, @now, @bobUser),
        (@solutionId, @resourceManagementCapabilityId, 1, @now, @bobUser),
        (@solutionId, @medicineOptimizationCapabilityId, 1, @now, @bobUser),
        (@solutionId, @referralManagementCapabilityId, 1, @now, @bobUser),
        (@solutionId, @gpExtractVerificationCapabilityId, 1, @now, @bobUser),
        (@solutionId, @scanningCapabilityId, 1, @now, @bobUser),
        (@solutionId, @reportingCapabilityId, 1, @now, @bobUser),
        (@solutionId, @patientInformationMaintenanceCapabilityId, 1, @now, @bobUser),
        (@solutionId, @recordingConsultationCapabilityId, 1, @now, @bobUser),
        (@solutionId, @prescribingCapabilityId, 1, @now, @bobUser),
        (@solutionId, @productivityCapabilityId, 1, @now, @bobUser),
        (@solutionId, @clinicalDecisionSupportCapabilityId, 1, @now, @bobUser),
        (@solutionId, @workflowCapabilityId, 1, @now, @bobUser);

        INSERT INTO catalogue.FrameworkSolutions(FrameworkId ,SolutionId ,IsFoundation, LastUpdated ,LastUpdatedBy)
             VALUES (@tifFrameworkId, @solutionId , 1, @now, @bobUser);

        INSERT INTO catalogue.CatalogueItemEpics (CatalogueItemId, CapabilityId, EpicId, StatusId, LastUpdated, LastUpdatedBy)
        VALUES
        (@solutionId, @gpAppointmentManagementCapabilityId, 'C5E1', 1, @now, @bobUser),
        (@solutionId, @gpAppointmentManagementCapabilityId, 'C5E2', 1, @now, @bobUser),
        (@solutionId, @gpAppointmentManagementCapabilityId, 'C5E3', 1, @now, @bobUser),
        (@solutionId, @gpAppointmentManagementCapabilityId, 'C5E4', 1, @now, @bobUser),
        (@solutionId, @gpAppointmentManagementCapabilityId, 'C5E5', 1, @now, @bobUser),
        (@solutionId, @gpAppointmentManagementCapabilityId, 'C5E6', 1, @now, @bobUser),
        (@solutionId, @gpAppointmentManagementCapabilityId, 'C5E7', 1, @now, @bobUser),
        (@solutionId, @clinicalDecisionSupportCapabilityId, 'C6E1', 1, @now, @bobUser),
        (@solutionId, @clinicalDecisionSupportCapabilityId, 'C6E2', 3, @now, @bobUser),
        (@solutionId, @clinicalDecisionSupportCapabilityId, 'C6E3', 2, @now, @bobUser),
        (@solutionId, @clinicalDecisionSupportCapabilityId, 'C6E4', 2, @now, @bobUser),
        (@solutionId, @gpExtractVerificationCapabilityId, 'C10E1', 1, @now, @bobUser),
        (@solutionId, @gpExtractVerificationCapabilityId, 'C10E2', 2, @now, @bobUser),
        (@solutionId, @referralManagementCapabilityId, 'C11E1', 1, @now, @bobUser),
        (@solutionId, @referralManagementCapabilityId, 'C11E2', 1, @now, @bobUser),
        (@solutionId, @resourceManagementCapabilityId, 'C12E1', 1, @now, @bobUser),
        (@solutionId, @resourceManagementCapabilityId, 'C12E2', 1, @now, @bobUser),
        (@solutionId, @resourceManagementCapabilityId, 'C12E3', 1, @now, @bobUser),
        (@solutionId, @resourceManagementCapabilityId, 'C12E4', 1, @now, @bobUser),
        (@solutionId, @resourceManagementCapabilityId, 'C12E5', 1, @now, @bobUser),
        (@solutionId, @resourceManagementCapabilityId, 'C12E6', 1, @now, @bobUser),
        (@solutionId, @resourceManagementCapabilityId, 'C12E7', 2, @now, @bobUser),
        (@solutionId, @patientInformationMaintenanceCapabilityId, 'C13E1', 1, @now, @bobUser),
        (@solutionId, @patientInformationMaintenanceCapabilityId, 'C13E2', 1, @now, @bobUser),
        (@solutionId, @patientInformationMaintenanceCapabilityId, 'C13E3', 1, @now, @bobUser),
        (@solutionId, @patientInformationMaintenanceCapabilityId, 'C13E4', 1, @now, @bobUser),
        (@solutionId, @patientInformationMaintenanceCapabilityId, 'C13E5', 1, @now, @bobUser),
        (@solutionId, @patientInformationMaintenanceCapabilityId, 'C13E6', 1, @now, @bobUser),
        (@solutionId, @patientInformationMaintenanceCapabilityId, 'C13E7', 1, @now, @bobUser),
        (@solutionId, @patientInformationMaintenanceCapabilityId, 'C13E8', 1, @now, @bobUser),
        (@solutionId, @patientInformationMaintenanceCapabilityId, 'C13E9', 1, @now, @bobUser),
        (@solutionId, @patientInformationMaintenanceCapabilityId, 'C13E10', 1, @now, @bobUser),
        (@solutionId, @patientInformationMaintenanceCapabilityId, 'C13E11', 1, @now, @bobUser),
        (@solutionId, @patientInformationMaintenanceCapabilityId, 'C13E12', 1, @now, @bobUser),
        (@solutionId, @patientInformationMaintenanceCapabilityId, 'C13E13', 2, @now, @bobUser),
        (@solutionId, @patientInformationMaintenanceCapabilityId, 'C13E14', 2, @now, @bobUser),
        (@solutionId, @patientInformationMaintenanceCapabilityId, 'C13E15', 2, @now, @bobUser),
        (@solutionId, @patientInformationMaintenanceCapabilityId, 'C13E16', 2, @now, @bobUser),
        (@solutionId, @patientInformationMaintenanceCapabilityId, 'C13E17', 2, @now, @bobUser),
        (@solutionId, @patientInformationMaintenanceCapabilityId, 'C13E18', 2, @now, @bobUser),
        (@solutionId, @patientInformationMaintenanceCapabilityId, 'C13E19', 2, @now, @bobUser),
        (@solutionId, @patientInformationMaintenanceCapabilityId, 'C13E20', 2, @now, @bobUser),
        (@solutionId, @patientInformationMaintenanceCapabilityId, 'C13E21', 2, @now, @bobUser),
        (@solutionId, @prescribingCapabilityId, 'C14E1', 1, @now, @bobUser),
        (@solutionId, @prescribingCapabilityId, 'C14E2', 1, @now, @bobUser),
        (@solutionId, @prescribingCapabilityId, 'C14E3', 1, @now, @bobUser),
        (@solutionId, @prescribingCapabilityId, 'C14E4', 1, @now, @bobUser),
        (@solutionId, @prescribingCapabilityId, 'C14E5', 1, @now, @bobUser),
        (@solutionId, @prescribingCapabilityId, 'C14E6', 1, @now, @bobUser),
        (@solutionId, @prescribingCapabilityId, 'C14E7', 1, @now, @bobUser),
        (@solutionId, @prescribingCapabilityId, 'C14E8', 1, @now, @bobUser),
        (@solutionId, @prescribingCapabilityId, 'C14E9', 1, @now, @bobUser),
        (@solutionId, @prescribingCapabilityId, 'C14E10', 1, @now, @bobUser),
        (@solutionId, @prescribingCapabilityId, 'C14E11', 1, @now, @bobUser),
        (@solutionId, @prescribingCapabilityId, 'C14E12', 1, @now, @bobUser),
        (@solutionId, @prescribingCapabilityId, 'C14E13', 1, @now, @bobUser),
        (@solutionId, @prescribingCapabilityId, 'C14E14', 2, @now, @bobUser),
        (@solutionId, @prescribingCapabilityId, 'C14E15', 2, @now, @bobUser),
        (@solutionId, @prescribingCapabilityId, 'C14E16', 2, @now, @bobUser),
        (@solutionId, @recordingConsultationCapabilityId, 'C15E1', 1, @now, @bobUser),
        (@solutionId, @recordingConsultationCapabilityId, 'C15E2', 1, @now, @bobUser),
        (@solutionId, @recordingConsultationCapabilityId, 'C15E3', 1, @now, @bobUser),
        (@solutionId, @recordingConsultationCapabilityId, 'C15E4', 1, @now, @bobUser),
        (@solutionId, @recordingConsultationCapabilityId, 'C15E5', 1, @now, @bobUser),
        (@solutionId, @recordingConsultationCapabilityId, 'C15E6', 1, @now, @bobUser),
        (@solutionId, @recordingConsultationCapabilityId, 'C15E7', 1, @now, @bobUser),
        (@solutionId, @recordingConsultationCapabilityId, 'C15E8', 1, @now, @bobUser),
        (@solutionId, @reportingCapabilityId, 'C16E1', 1, @now, @bobUser),
        (@solutionId, @scanningCapabilityId, 'C17E1', 1, @now, @bobUser),
        (@solutionId, @scanningCapabilityId, 'C17E2', 2, @now, @bobUser),
        (@solutionId, @workflowCapabilityId, 'C20E1', 2, @now, @bobUser),
        (@solutionId, @workflowCapabilityId, 'C20E2', 2, @now, @bobUser),
        (@solutionId, @workflowCapabilityId, 'C20E3', 1, @now, @bobUser),
        (@solutionId, @workflowCapabilityId, 'C20E4', 2, @now, @bobUser),
        (@solutionId, @workflowCapabilityId, 'C20E5', 1, @now, @bobUser),
        (@solutionId, @workflowCapabilityId, 'C20E6', 2, @now, @bobUser),
        (@solutionId, @workflowCapabilityId, 'C20E7', 1, @now, @bobUser),
        (@solutionId, @workflowCapabilityId, 'C20E8', 2, @now, @bobUser),
        (@solutionId, @workflowCapabilityId, 'C20E9', 1, @now, @bobUser),
        (@solutionId, @workflowCapabilityId, 'C20E10', 2, @now, @bobUser),
        (@solutionId, @workflowCapabilityId, 'C20E11', 1, @now, @bobUser),
        (@solutionId, @workflowCapabilityId, 'C20E12', 2, @now, @bobUser),
        (@solutionId, @workflowCapabilityId, 'C20E13', 2, @now, @bobUser),
        (@solutionId, @medicineOptimizationCapabilityId, 'C30E1', 1, @now, @bobUser),
        (@solutionId, @medicineOptimizationCapabilityId, 'C30E2', 1, @now, @bobUser),
        (@solutionId, @medicineOptimizationCapabilityId, 'C30E3', 1, @now, @bobUser),
        (@solutionId, @medicineOptimizationCapabilityId, 'C30E4', 2, @now, @bobUser),
        (@solutionId, @medicineOptimizationCapabilityId, 'C30E5', 2, @now, @bobUser),
        (@solutionId, @medicineOptimizationCapabilityId, 'C30E6', 2, @now, @bobUser),
        (@solutionId, @medicineOptimizationCapabilityId, 'C30E7', 2, @now, @bobUser),
        (@solutionId, @medicineOptimizationCapabilityId, 'C30E8', 2, @now, @bobUser),
        (@solutionId, @medicineOptimizationCapabilityId, 'C30E9', 2, @now, @bobUser),
        (@solutionId, @medicineOptimizationCapabilityId, 'C30E10', 2, @now, @bobUser),
        (@solutionId, @medicineOptimizationCapabilityId, 'C30E11', 2, @now, @bobUser),
        (@solutionId, @medicineOptimizationCapabilityId, 'C30E12', 2, @now, @bobUser),
        (@solutionId, @medicineOptimizationCapabilityId, 'C30E13', 2, @now, @bobUser),
        (@solutionId, @medicineOptimizationCapabilityId, 'C30E14', 2, @now, @bobUser),
        (@solutionId, @onlineConsultationCapabilityId, 'S020X01E01', 1, @now, @bobUser),
        (@solutionId, @onlineConsultationCapabilityId, 'S020X01E02', 1, @now, @bobUser),
        (@solutionId, @videoConsultationCapabilityId, 'S020X01E03', 1, @now, @bobUser),
        (@solutionId, @videoConsultationCapabilityId, 'S020X01E04', 1, @now, @bobUser);
    END;

    /*************************************************************************************************************************************************************/

    SET @solutionId = '99998-98';

    IF NOT EXISTS (SELECT * FROM catalogue.CatalogueItems WHERE Id = @solutionId)
    BEGIN
        INSERT INTO catalogue.CatalogueItems(Id, CatalogueItemTypeId, [Name], SupplierId, PublishedStatusId, Created)
             VALUES (@solutionId, @solutionItemType, 'NotSystmOne', 99998, @publishedStatus, @now);

        INSERT INTO catalogue.Solutions(CatalogueItemId,
                    Features, ClientApplication, Hosting, ImplementationDetail,
                    IntegrationsUrl, AboutUrl, Summary, FullDescription,
                    LastUpdated, LastUpdatedBy)
        VALUES
        (
            @solutionId,
            '["Full Spine Compliance – EPS, PDS, SCR, eRS, GP2GP","Standards – SNOMED CT, HL7 V2, V3, FHIR, GP Connect","Appointments – Configurable Clinics, Dedicated Appointments, Visits Screens, SMS Integration","Prescribing – Acute, Repeat, Formularies, Action Groups, Decision Support","Complete Electronic Health Record (EHR)","Comprehensive consultations – Recalls, Referrals, Structured Data","Clinical Development Kit – Data Entry Templates, Views, Questionnaires, Integrated Word Letters","Full Workflow Support including Automatic Consultations","Analytics – Customisable Reports, Batch Reports, Bulk Actions, QOF Tools, Automatic Submissions","Patient Online Services – Appointment Booking, Medication Requests, Record Access, Proxy Access"]',
            '{"ClientApplicationTypes":["native-desktop"],"BrowsersSupported":[],"NativeMobileHardwareRequirements":"The OS system drive must have a drive letter of C.","NativeDesktopHardwareRequirements":"The OS system drive must have a drive letter of C.","NativeMobileFirstDesign":false,"MobileOperatingSystems":{"OperatingSystems":["Other"],"OperatingSystemsDescription":"Windows"},"MobileConnectionDetails":{"MinimumConnectionSpeed":"1Mbps","ConnectionType":["3G","4G","Wifi"],"Description":"CPU of 1 GHz or faster 32-bit or 64-bit processor"},"MobileMemoryAndStorage":{"MinimumMemoryRequirement":"1GB","Description":"4GB of free space on the C drive"},"MobileThirdParty":{"ThirdPartyComponents":"","DeviceCapabilities":"Minimum screen resolution of 1024 x 720 pixels."},"NativeDesktopOperatingSystemsDescription":"TPP supports all versions of Windows for desktops that are currently supported by Microsoft. Following verification of the configuration by TPP, installation of Windows to a virtual environment is supported to the products and versions including Virtual VMware View 5+, Citrix Xen Desktop 6+ and Microsoft Server 2012+.\r\nInstallation of the SystmOne client to any Server Operating System is not licensed by TPP. It should also be noted that both 32-bit and 64-bit versions of Microsoft Windows are supported unless otherwise stated. Windows RT is not supported.","NativeDesktopMinimumConnectionSpeed":"0.5Mbps","NativeDesktopThirdParty":{"ThirdPartyComponents":"Windows 7 requires 1GB and Office 2010 requires 256 MB. Other third party applications, shared graphics or peripherals (such as attached printers) should also be taken into account. These will all increase the amount of memory required for the computer to run smoothly.","DeviceCapabilities":"A minimum screen resolution of 1024 x 768 pixels with 16-bit colours is required. TPP recommends a minimum of a 17” TFT flat screen monitor with a resolution of 1280 x 1024 and 32-bit colours."},"NativeDesktopMemoryAndStorage":{"MinimumMemoryRequirement":"512MB","StorageRequirementsDescription":"4GB of free space on the C drive. Where a SystmOne Gateway client is used, 100GB of free space on the C drive is recommended.","MinimumCpu":"A minimum of a 2.0 GHz Pentium 4 series CPI is required.","RecommendedResolution":"5:4 – 1280 x 1024"},"NativeDesktopAdditionalInformation":"Applications that can open/view rich text file (.rtf) and comma separated (.csv) documents are required. To perform letter writing, Microsoft Word is also required. TPP only supportsversions of Office that are supported by Microsoft which currently includes Office 2010, 2013, 2016 and 2019."}',
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
            @bobUser);

        INSERT INTO catalogue.CatalogueItemCapabilities (CatalogueItemId, CapabilityId, StatusId, LastUpdated, LastUpdatedBy)
        VALUES
        (@solutionId, @presciptionOrderingCapabilityId, 1, @now, @bobUser),
        (@solutionId, @gpAppointmentManagementCapabilityId, 1, @now, @bobUser),
        (@solutionId, @resourceManagementCapabilityId, 1, @now, @bobUser),
        (@solutionId, @unifiedCareRecordCapabilityId, 1, @now, @bobUser),
        (@solutionId, @referralManagementCapabilityId, 1, @now, @bobUser),
        (@solutionId, @communicationManagementCapabilityId, 1, @now, @bobUser),
        (@solutionId, @reportingCapabilityId, 1, @now, @bobUser),
        (@solutionId, @patientInformationMaintenanceCapabilityId, 1, @now, @bobUser),
        (@solutionId, @citizenAppointmentManagementCapabilityId, 1, @now, @bobUser),
        (@solutionId, @citizenCommunicateCapabilityId, 1, @now, @bobUser),
        (@solutionId, @dataAnalyticsCapabilityId, 1, @now, @bobUser),
        (@solutionId, @sharedCarePlansCapabilityId, 1, @now, @bobUser),
        (@solutionId, @recordingConsultationCapabilityId, 1, @now, @bobUser),
        (@solutionId, @prescribingCapabilityId, 1, @now, @bobUser),
        (@solutionId, @productivityCapabilityId, 1, @now, @bobUser),
        (@solutionId, @citizenViewRecordCapabilityId, 1, @now, @bobUser);

        INSERT INTO catalogue.FrameworkSolutions(FrameworkId ,SolutionId ,IsFoundation, LastUpdated, LastUpdatedBy)
             VALUES (@gpitframeworkId, @solutionId , 1, @now, @bobUser);
    END;

    /*************************************************************************************************************************************************************/

    SET @solutionId = '99999-01';

    IF NOT EXISTS (SELECT * FROM catalogue.CatalogueItems WHERE Id = @solutionId)
    BEGIN
        INSERT INTO catalogue.CatalogueItems(Id, CatalogueItemTypeId, [Name], SupplierId, PublishedStatusId, Created)
             VALUES (@solutionId, @solutionItemType, 'DFOCVC Online Consultation', 99999, @publishedStatus, @now);

        INSERT INTO catalogue.Solutions(CatalogueItemId, Features, Hosting, AboutUrl, Summary, FullDescription, LastUpdated, LastUpdatedBy)
        VALUES (
            @solutionId,
            '["Digital Online Consultation","Video Consultation", "Fully interopable with all major GP IT solutions", "Compliant with all relevant ISO standards"]',
            '{"PublicCloud":{"Summary":"Summary description","Link":"External URL link","RequiresHSCN":"Link to HSCN or N3 network required to access service"},"PrivateCloud":{"Summary":"Summary description","Link":"External URL link","HostingModel":"Hosting environment description","RequiresHSCN":"Link to HSCN or N3 network required to access service"},"HybridHostingType":{"Summary":"Summary description","Link":"External URL link","HostingModel":"Hosting environment description","RequiresHSCN":"Link to HSCN or N3 network required to access service"},"OnPremise":{"Summary":"Summary description","Link":"External URL link","HostingModel":"Hosting environment description","RequiresHSCN":"Link to HSCN or N3 network required to access service"}}',
            'http://www.dfocvctest.com/about',
            'Summary - DFOCVC.',
            'FULL DESCRIPTION – Digital First, Online Consultation and Video Consultation Solution.',
            @now,
            @bobUser);

        INSERT INTO catalogue.MarketingContacts(SolutionId, FirstName, LastName, PhoneNumber, Email, Department, LastUpdated, LastUpdatedBy)
             VALUES (@solutionId, 'Sam', 'Samosa', '01234 567891', 'sales@test.test', 'Sales', @now, @bobUser);

        INSERT INTO catalogue.CatalogueItemCapabilities(CatalogueItemId, CapabilityId, StatusId, LastUpdated, LastUpdatedBy)
             SELECT @solutionId, Id, 1, @now, @bobUser
               FROM catalogue.Capabilities
              WHERE CapabilityRef IN ('C43');

        INSERT INTO catalogue.FrameworkSolutions(FrameworkId, SolutionId, IsFoundation, LastUpdated, LastUpdatedBy)
             VALUES (@dfocvcframeworkId, @solutionId, 0, @now, @bobUser);
    END;

    /*************************************************************************************************************************************************************/

    SET @solutionId = '99999-02';

    IF NOT EXISTS (SELECT * FROM catalogue.CatalogueItems WHERE Id = @solutionId)
    BEGIN
        INSERT INTO catalogue.CatalogueItems(Id, CatalogueItemTypeId, [Name], SupplierId, PublishedStatusId, Created)
             VALUES (@solutionId, @solutionItemType, 'GPIT DFOCVC Online Consultation', 99999, @publishedStatus, @now);

        INSERT INTO catalogue.Solutions(CatalogueItemId, Features, Hosting, AboutUrl, Summary, FullDescription, LastUpdated, LastUpdatedBy)
        VALUES (
            @solutionId,
            '["Digital Online Consultation","Video Consultation", "Fully interopable with all major GP IT solutions", "Compliant with all relevant ISO standards"]',
            '{"PublicCloud":{"Summary":"Summary description","Link":"External URL link","RequiresHSCN":"Link to HSCN or N3 network required to access service"},"PrivateCloud":{"Summary":"Summary description","Link":"External URL link","HostingModel":"Hosting environment description","RequiresHSCN":"Link to HSCN or N3 network required to access service"},"HybridHostingType":{"Summary":"Summary description","Link":"External URL link","HostingModel":"Hosting environment description","RequiresHSCN":"Link to HSCN or N3 network required to access service"},"OnPremise":{"Summary":"Summary description","Link":"External URL link","HostingModel":"Hosting environment description","RequiresHSCN":"Link to HSCN or N3 network required to access service"}}',
            'http://www.gpitdfocvctest.com/about',
            'Summary - GPIT DFOCVC.',
            'FULL DESCRIPTION – GPIT Futures, Digital First Online Consultation and Video Consultation Solution.',
            @now,
            @bobUser);

        INSERT INTO catalogue.MarketingContacts(SolutionId, FirstName, LastName, PhoneNumber, Email, Department, LastUpdated, LastUpdatedBy)
             VALUES (@solutionId, 'Sam', 'Samosa', '01234 567891', 'sales@test.test', 'Sales', @now, @bobUser);

        INSERT INTO catalogue.CatalogueItemCapabilities(CatalogueItemId, CapabilityId, StatusId, LastUpdated, LastUpdatedBy)
             SELECT @solutionId, Id, 1, @now, @bobUser
               FROM catalogue.Capabilities
              WHERE CapabilityRef IN ('C44');

        INSERT INTO catalogue.FrameworkSolutions(FrameworkId, SolutionId, IsFoundation, LastUpdated, LastUpdatedBy)
             VALUES (@gpitframeworkId, @solutionId, 1, @now, @bobUser),
                    (@dfocvcframeworkId, @solutionId, 0, @now, @bobUser);
    END;

    DECLARE
    @flatPriceType INT = 1,
    @tieredPriceType INT = 2,

    @patientProvisioningType INT = (SELECT Id FROM catalogue.ProvisioningTypes WHERE [Name] = 'Patient'),
    @declarativeProvisioningType INT = (SELECT Id FROM catalogue.ProvisioningTypes WHERE [Name] = 'Declarative'),
    @onDemandProvisioningType INT = (SELECT Id FROM catalogue.ProvisioningTypes WHERE [Name] = 'OnDemand'),
    @perServiceRecipient INT = (Select Id FROM catalogue.CataloguePriceQuantityCalculationTypes WHERE [Name] = 'PerServiceRecipient'),

    @monthTimeUnit INT = 1,
    @yearTimeUnit INT = 2,

    @CataloguePriceCalculationTypeVolumeId INT = 3,
    @CataloguePriceCalculationTypeCumulativeId INT = 2,
    @CataloguePriceCalculationTypeSingleFixed INT = 1,

    @patient SMALLINT = -1,
    @bed SMALLINT = -2,
    @consultation SMALLINT = -3,
    @licence SMALLINT = -4,
    @sms SMALLINT = -5;

    /* Insert prices */

    IF NOT EXISTS (SELECT * FROM catalogue.CataloguePrices)
    BEGIN
         DECLARE @InsertedPriceIds TABLE(
             Id INT,
             Price DECIMAL(18,4),
             CataloguePriceTypeId INT
         );

        DECLARE @SolutionPrices TABLE(
            CatalogueItemId NVARCHAR(14),
            ProvisioningTypeId INT,
            CataloguePriceTypeId INT,
            PricingUnitId INT,
            TimeUnitId INT,
            CataloguePriceCalculationTypeId INT,
            CataloguePriceQuantityCalculationTypeId INT,
            CurrencyCode NVARCHAR(3),
            LastUpdated DATETIME2(0),
            Price DECIMAL(18,4),
            PublishedStatusId INT
        );

        INSERT INTO @SolutionPrices (CatalogueItemId, ProvisioningTypeId, CataloguePriceTypeId, PricingUnitId, TimeUnitId, CataloguePriceCalculationTypeId, CataloguePriceQuantityCalculationTypeId, CurrencyCode, LastUpdated, Price, PublishedStatusId)
        VALUES
        ('100000-001', @patientProvisioningType, @flatPriceType, @patient, @yearTimeUnit, @CataloguePriceCalculationTypeSingleFixed, NULL,'GBP', @now, 99.99, 3),
        ('100000-001', @patientProvisioningType, @tieredPriceType, @patient, @yearTimeUnit, @CataloguePriceCalculationTypeCumulativeId, NULL, 'GBP', @now, 0.5, 3),
        ('100000-001', @onDemandProvisioningType, @flatPriceType, @consultation, NULL, @CataloguePriceCalculationTypeVolumeId, @perServiceRecipient, 'GBP', @now, 1001.010, 3),
        ('100001-001', @onDemandProvisioningType, @flatPriceType, @licence, NULL, @CataloguePriceCalculationTypeSingleFixed, NULL, 'GBP', @now, 3.142, 3),
        ('100002-001', @declarativeProvisioningType, @flatPriceType, @patient, @monthTimeUnit, @CataloguePriceCalculationTypeSingleFixed, NULL, 'GBP', @now, 4.85, 3),
        ('100002-001', @onDemandProvisioningType, @tieredPriceType, @patient, @monthTimeUnit, @CataloguePriceCalculationTypeVolumeId, @perServiceRecipient, 'GBP', @now, 20, 3),
        ('100002-001', @declarativeProvisioningType, @tieredPriceType, @patient, @monthTimeUnit, @CataloguePriceCalculationTypeSingleFixed, NULL, 'GBP', @now, 10, 3),
        ('100003-001', @onDemandProvisioningType, @flatPriceType, @bed, @monthTimeUnit, @CataloguePriceCalculationTypeVolumeId, @perServiceRecipient, 'GBP', @now, 19.987, 3),
        ('100004-001', @declarativeProvisioningType, @flatPriceType, @licence, @monthTimeUnit, @CataloguePriceCalculationTypeSingleFixed, NULL, 'GBP', @now, 10101.65, 3),
        ('100005-001', @onDemandProvisioningType, @flatPriceType, @licence, NULL, @CataloguePriceCalculationTypeSingleFixed, NULL, 'GBP', @now, 456, 3),
        ('100006-001', @declarativeProvisioningType, @flatPriceType, @sms, @monthTimeUnit, @CataloguePriceCalculationTypeSingleFixed, NULL, 'GBP', @now, 7, 3),
        ('100007-001', @onDemandProvisioningType, @flatPriceType, @sms, NULL, @CataloguePriceCalculationTypeSingleFixed, NULL, 'GBP', @now, 0.15, 3),
        ('100007-002', @onDemandProvisioningType, @tieredPriceType, @sms, NULL, @CataloguePriceCalculationTypeCumulativeId, NULL, 'GBP', @now, 6, 3),
        ('99998-98', @patientProvisioningType, @flatPriceType, @licence, @yearTimeUnit, @CataloguePriceCalculationTypeSingleFixed, NULL, 'GBP', @now, 30000, 3),
        ('99998-98', @patientProvisioningType, @tieredPriceType, @licence, @yearTimeUnit, @CataloguePriceCalculationTypeCumulativeId, NULL, 'GBP', @now, 0.1, 3),
        ('99999-01', @patientProvisioningType, @flatPriceType, @patient, @yearTimeUnit, @CataloguePriceCalculationTypeSingleFixed, NULL, 'GBP', @now, 1.25, 3),
        ('99999-02', @onDemandProvisioningType, @flatPriceType, @patient, @yearTimeUnit, @CataloguePriceCalculationTypeVolumeId, @perServiceRecipient, 'GBP', @now, 1.55, 3),
        ('99999-89', @patientProvisioningType, @flatPriceType, @licence, @yearTimeUnit, @CataloguePriceCalculationTypeSingleFixed, NULL, 'GBP', @now, 500.49, 3),
        ('99999-89', @onDemandProvisioningType, @tieredPriceType, @licence, @yearTimeUnit, @CataloguePriceCalculationTypeVolumeId, @perServiceRecipient, 'GBP', @now, 3.5, 3);

	    MERGE INTO catalogue.CataloguePrices USING @SolutionPrices AS ASP ON 1 = 0
	    WHEN NOT MATCHED THEN
	    INSERT (CatalogueItemId, ProvisioningTypeId, CataloguePriceTypeId, PricingUnitId, TimeUnitId, CataloguePriceCalculationTypeId, CataloguePriceQuantityCalculationTypeId, CurrencyCode, LastUpdated, PublishedStatusId)
	    VALUES(
	    ASP.CatalogueItemId,
        ASP.ProvisioningTypeId,
        ASP.CataloguePriceTypeId,
        ASP.PricingUnitId,
        ASP.TimeUnitId,
        ASP.CataloguePriceCalculationTypeId,
        ASP.CataloguePriceQuantityCalculationTypeId,
        ASP.CurrencyCode,
        ASP.LastUpdated,
        ASP.PublishedStatusId)
	    OUTPUT INSERTED.CataloguePriceId, ASP.Price, INSERTED.CataloguePriceTypeId
	    INTO @InsertedPriceIds (Id, Price, CataloguePriceTypeId);

        --Insert flat Prices
        INSERT INTO catalogue.CataloguePriceTiers(CataloguePriceId, LowerRange, UpperRange, Price)
        SELECT
            IPI.Id, 1, NULL, IPI.Price
        FROM @InsertedPriceIds IPI
        WHERE CataloguePriceTypeId = 1;

        --Insert Tiered Prices
        INSERT INTO catalogue.CataloguePriceTiers(CataloguePriceId, LowerRange, UpperRange, Price)
            SELECT
                IPI.Id AS CataloguePriceId,
                1 AS LowerRange,
                9,
                IPI.Price
            FROM @InsertedPriceIds IPI
            WHERE CataloguePriceTypeId = 2
        UNION ALL
            SELECT
                IPI.Id AS CataloguePriceId,
                10 AS LowerRange,
                99,
                IPI.Price / 2
            FROM @InsertedPriceIds IPI
            WHERE CataloguePriceTypeId = 2
        UNION ALL
            SELECT
                IPI.Id AS CataloguePriceId,
                100 AS LowerRange,
                NULL,
                IPI.Price / 4
            FROM @InsertedPriceIds IPI
            WHERE CataloguePriceTypeId = 2
        ORDER BY
            CataloguePriceId, LowerRange;
     END;
END;
GO
