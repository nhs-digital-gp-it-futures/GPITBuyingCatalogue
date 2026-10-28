MERGE INTO [catalogue].[SolutionSections] AS TARGET
    USING (
    VALUES
        (0, 2, 2, 1, 1, 1, 2, 1, 1, 2, 2, 1, 1, 1, 1, 2, 2),
        (1, 2, 2, 0, 2, 0, 2, 2, 0, 2, 2, 0, 0, 2, 2, 2, 0))
    AS SOURCE (
        [SolutionTypeId],
        [Details],
        [Description],
        [Features],
        [Integrations],
        [Implementation],
        [ApplicationType],
        [HostingType],
        [DataProcessingInformation],
        [ListPrice],
        [CapabilitiesAndEpics],
        [AdditionalServices],
        [AssociatedServices],
        [Standards],
        [DevelopmentPlans],
        [SupplierDetails],
        [ServiceLevelAgreement])
    ON TARGET.[SolutionTypeId] = SOURCE.[SolutionTypeId]
    WHEN MATCHED THEN UPDATE SET
        TARGET.[Details] = SOURCE.[Details],
        TARGET.[Description] = SOURCE.[Description],
        TARGET.[Features] = SOURCE.[Features],
        TARGET.[Integrations] = SOURCE.[Integrations],
        TARGET.[Implementation] = SOURCE.[Implementation],
        TARGET.[ApplicationType] = SOURCE.[ApplicationType],
        TARGET.[HostingType] = SOURCE.[HostingType],
        TARGET.[DataProcessingInformation] = SOURCE.[DataProcessingInformation],
        TARGET.[ListPrice] = SOURCE.[ListPrice],
        TARGET.[CapabilitiesAndEpics] = SOURCE.[CapabilitiesAndEpics],
        TARGET.[AdditionalServices] = SOURCE.[AdditionalServices],
        TARGET.[AssociatedServices] = SOURCE.[AssociatedServices],
        TARGET.[Standards] = SOURCE.[Standards],
        TARGET.[DevelopmentPlans] = SOURCE.[DevelopmentPlans],
        TARGET.[SupplierDetails] = SOURCE.[SupplierDetails],
        TARGET.[ServiceLevelAgreement] = SOURCE.[ServiceLevelAgreement]
    WHEN NOT MATCHED THEN
        INSERT (
            [SolutionTypeId],
            [Details],
            [Description],
            [Features],
            [Integrations],
            [Implementation],
            [ApplicationType],
            [HostingType],
            [DataProcessingInformation],
            [ListPrice],
            [CapabilitiesAndEpics],
            [AdditionalServices],
            [AssociatedServices],
            [Standards],
            [DevelopmentPlans],
            [SupplierDetails],
            [ServiceLevelAgreement])
        VALUES (
            SOURCE.[SolutionTypeId],
            SOURCE.[Details],
            SOURCE.[Description],
            SOURCE.[Features],
            SOURCE.[Integrations],
            SOURCE.[Implementation],
            SOURCE.[ApplicationType],
            SOURCE.[HostingType],
            SOURCE.[DataProcessingInformation],
            SOURCE.[ListPrice],
            SOURCE.[CapabilitiesAndEpics],
            SOURCE.[AdditionalServices],
            SOURCE.[AssociatedServices],
            SOURCE.[Standards],
            SOURCE.[DevelopmentPlans],
            SOURCE.[SupplierDetails],
            SOURCE.[ServiceLevelAgreement]);
GO