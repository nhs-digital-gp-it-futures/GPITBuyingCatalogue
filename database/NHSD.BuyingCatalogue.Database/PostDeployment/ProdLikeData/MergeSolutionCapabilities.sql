IF (UPPER('$(INSERT_TEST_DATA)') = 'TRUE')
BEGIN
    /*********************************************************************************************************************************************/
    /* SolutionCapability */
    /*********************************************************************************************************************************************/

    CREATE TABLE #SolutionCapability
    (
        SolutionId nvarchar(14) NOT NULL,
        CapabilityId uniqueidentifier NOT NULL,
        StatusId int NOT NULL,
        LastUpdated datetime2(7) NOT NULL,
        LastUpdatedBy uniqueidentifier NOT NULL
    );

    INSERT INTO #SolutionCapability (SolutionId, CapabilityId, StatusId, LastUpdated, LastUpdatedBy) 
         VALUES (N'10000-001', N'efd93d25-447b-4ca3-9d78-108d42afeae0', 1, CAST(N'2020-03-31T10:44:03.4500000' AS datetime2), N'4f222d7a-74ae-4ec7-9062-e4ad07fcd4f7'),
                (N'10000-001', N'e3e4cf8a-22d3-4056-bb5d-10f8e26b9b5e', 1, CAST(N'2020-03-31T10:44:03.4500000' AS datetime2), N'4f222d7a-74ae-4ec7-9062-e4ad07fcd4f7'),
                (N'10000-001', N'8bee1ff3-84d4-430b-a678-336f57c57387', 1, CAST(N'2020-03-31T10:44:03.4500000' AS datetime2), N'4f222d7a-74ae-4ec7-9062-e4ad07fcd4f7'),
                (N'10000-001', N'20b09859-6fc2-404c-b7a4-3830790e63ab', 1, CAST(N'2020-03-31T10:44:03.4500000' AS datetime2), N'4f222d7a-74ae-4ec7-9062-e4ad07fcd4f7'),
                (N'10000-001', N'9d805aad-d43a-480e-9bc0-41a755bafe2f', 1, CAST(N'2020-03-31T10:44:03.4500000' AS datetime2), N'4f222d7a-74ae-4ec7-9062-e4ad07fcd4f7'),
                (N'10000-001', N'e5521a71-a28e-4bc9-bddf-599f0a90719d', 1, CAST(N'2020-03-31T10:44:03.4500000' AS datetime2), N'4f222d7a-74ae-4ec7-9062-e4ad07fcd4f7'),
                (N'10000-001', N'dd649cc4-a710-4472-98b3-663d9d12a8b7', 1, CAST(N'2020-03-31T10:44:03.4500000' AS datetime2), N'4f222d7a-74ae-4ec7-9062-e4ad07fcd4f7'),
                (N'10000-001', N'8c384983-774a-45bd-9d4e-6b3c7d3b7323', 1, CAST(N'2020-03-31T10:44:03.4500000' AS datetime2), N'4f222d7a-74ae-4ec7-9062-e4ad07fcd4f7'),
                (N'10000-001', N'9442dcc4-22df-494b-8672-b7b4dd077496', 1, CAST(N'2020-03-31T10:44:03.4500000' AS datetime2), N'4f222d7a-74ae-4ec7-9062-e4ad07fcd4f7'),
                (N'10000-001', N'b3f89711-6bd7-42d7-be5b-bae2f239ebdd', 1, CAST(N'2020-03-31T10:44:03.4500000' AS datetime2), N'4f222d7a-74ae-4ec7-9062-e4ad07fcd4f7'),
                (N'10000-001', N'6e77147d-d2af-46bd-a2f2-bb4f235daf3a', 1, CAST(N'2020-03-31T10:44:03.4500000' AS datetime2), N'4f222d7a-74ae-4ec7-9062-e4ad07fcd4f7'),
                (N'10000-001', N'a71f2be1-6395-4db7-828c-d4733b42b5b5', 1, CAST(N'2020-03-31T10:44:03.4500000' AS datetime2), N'4f222d7a-74ae-4ec7-9062-e4ad07fcd4f7'),
                (N'10000-001', N'9d325dec-6e5b-44e4-876b-eacf6cd41b3e', 1, CAST(N'2020-03-31T10:44:03.4500000' AS datetime2), N'4f222d7a-74ae-4ec7-9062-e4ad07fcd4f7'),
                (N'10000-002', N'6e77147d-d2af-46bd-a2f2-bb4f235daf3a', 1, CAST(N'2020-04-06T10:50:03.2200000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10000-062', N'6e77147d-d2af-46bd-a2f2-bb4f235daf3a', 1, CAST(N'2020-04-06T10:53:50.6300000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10004-001', N'12c6a61c-013c-475f-bb0c-2da5d414c03b', 1, CAST(N'2020-03-26T12:13:20.1066667' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10004-001', N'2271b113-5d5d-4899-b259-3046caea76ed', 1, CAST(N'2020-03-26T12:13:20.1066667' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10004-001', N'a71f2be1-6395-4db7-828c-d4733b42b5b5', 1, CAST(N'2020-03-26T12:13:20.1066667' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10004-002', N'efd93d25-447b-4ca3-9d78-108d42afeae0', 1, CAST(N'2020-03-30T13:14:43.1666667' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10007-002', N'a71f2be1-6395-4db7-828c-d4733b42b5b5', 1, CAST(N'2020-03-25T11:42:40.8100000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10011-003', N'6e77147d-d2af-46bd-a2f2-bb4f235daf3a', 1, CAST(N'2020-06-18T14:20:53.8266667' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10029-003', N'6e77147d-d2af-46bd-a2f2-bb4f235daf3a', 1, CAST(N'2020-04-08T09:19:04.7600000' AS datetime2), N'6842fa00-b899-43e3-845c-d4569f0b53df'),
                (N'10030-001', N'6e77147d-d2af-46bd-a2f2-bb4f235daf3a', 1, CAST(N'2020-04-02T09:50:51.4966667' AS datetime2), N'71b953b6-8433-4afe-a3a6-21a93f3f3a4d'),
                (N'10031-001', N'6e77147d-d2af-46bd-a2f2-bb4f235daf3a', 1, CAST(N'2020-04-02T09:39:09.7500000' AS datetime2), N'ba95e474-ed3d-4f0d-ab29-d3a960cf0f5d'),
                (N'10035-001', N'60c2f5b0-b950-44c8-a246-099335a1c816', 1, CAST(N'2020-04-01T10:42:08.5066667' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10035-001', N'e5e3be58-e5ec-4423-85dd-61d88640c22a', 1, CAST(N'2020-04-01T10:42:08.5066667' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10035-001', N'21ae013d-42a4-4748-b435-73d5887944c2', 1, CAST(N'2020-04-01T10:42:08.5066667' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10035-001', N'4f09e77b-e3a3-4a25-8ec1-815921f83628', 1, CAST(N'2020-04-01T10:42:08.5066667' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10035-001', N'64e5986d-1ebf-4df0-8219-c150c082ca7b', 1, CAST(N'2020-04-01T10:42:08.5066667' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10046-001', N'19002612-8d53-4472-82fc-2753b253434c', 1, CAST(N'2020-03-30T13:02:24.5200000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10046-001', N'e5521a71-a28e-4bc9-bddf-599f0a90719d', 1, CAST(N'2020-03-30T13:02:24.5200000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10046-001', N'1e82cc7c-87c7-4379-b86f-cf36c59d1a46', 1, CAST(N'2020-03-30T13:02:24.5200000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10046-001', N'9d325dec-6e5b-44e4-876b-eacf6cd41b3e', 1, CAST(N'2020-03-30T13:02:24.5200000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10046-003', N'19002612-8d53-4472-82fc-2753b253434c', 1, CAST(N'2020-03-30T13:04:21.6500000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10046-003', N'e5521a71-a28e-4bc9-bddf-599f0a90719d', 1, CAST(N'2020-03-30T13:04:21.6500000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10046-006', N'6e77147d-d2af-46bd-a2f2-bb4f235daf3a', 1, CAST(N'2020-06-25T14:31:07.2366667' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10052-002', N'60c2f5b0-b950-44c8-a246-099335a1c816', 1, CAST(N'2020-04-01T11:37:44.9366667' AS datetime2), N'0d0f2f0a-164e-42c7-bc9e-4437f4e23d34'),
                (N'10052-002', N'efd93d25-447b-4ca3-9d78-108d42afeae0', 1, CAST(N'2020-04-01T11:37:44.9366667' AS datetime2), N'0d0f2f0a-164e-42c7-bc9e-4437f4e23d34'),
                (N'10052-002', N'e3e4cf8a-22d3-4056-bb5d-10f8e26b9b5e', 1, CAST(N'2020-04-01T11:37:44.9366667' AS datetime2), N'0d0f2f0a-164e-42c7-bc9e-4437f4e23d34'),
                (N'10052-002', N'59696227-602a-421d-a883-29e88997ac17', 1, CAST(N'2020-04-01T11:37:44.9366667' AS datetime2), N'0d0f2f0a-164e-42c7-bc9e-4437f4e23d34'),
                (N'10052-002', N'20b09859-6fc2-404c-b7a4-3830790e63ab', 1, CAST(N'2020-04-01T11:37:44.9366667' AS datetime2), N'0d0f2f0a-164e-42c7-bc9e-4437f4e23d34'),
                (N'10052-002', N'dd649cc4-a710-4472-98b3-663d9d12a8b7', 1, CAST(N'2020-04-01T11:37:44.9366667' AS datetime2), N'0d0f2f0a-164e-42c7-bc9e-4437f4e23d34'),
                (N'10052-002', N'8c384983-774a-45bd-9d4e-6b3c7d3b7323', 1, CAST(N'2020-04-01T11:37:44.9366667' AS datetime2), N'0d0f2f0a-164e-42c7-bc9e-4437f4e23d34'),
                (N'10052-002', N'21ae013d-42a4-4748-b435-73d5887944c2', 1, CAST(N'2020-04-01T11:37:44.9366667' AS datetime2), N'0d0f2f0a-164e-42c7-bc9e-4437f4e23d34'),
                (N'10052-002', N'4f09e77b-e3a3-4a25-8ec1-815921f83628', 1, CAST(N'2020-04-01T11:37:44.9366667' AS datetime2), N'0d0f2f0a-164e-42c7-bc9e-4437f4e23d34'),
                (N'10052-002', N'5db79ff4-fa9c-4da2-bbfc-8ca40fec0b43', 1, CAST(N'2020-04-01T11:37:44.9366667' AS datetime2), N'0d0f2f0a-164e-42c7-bc9e-4437f4e23d34'),
                (N'10052-002', N'd1532ca0-ef0c-457c-9cfc-affa0fbdf134', 1, CAST(N'2020-04-01T11:37:44.9366667' AS datetime2), N'0d0f2f0a-164e-42c7-bc9e-4437f4e23d34'),
                (N'10052-002', N'9442dcc4-22df-494b-8672-b7b4dd077496', 1, CAST(N'2020-04-01T11:37:44.9366667' AS datetime2), N'0d0f2f0a-164e-42c7-bc9e-4437f4e23d34'),
                (N'10052-002', N'b3f89711-6bd7-42d7-be5b-bae2f239ebdd', 1, CAST(N'2020-04-01T11:37:44.9366667' AS datetime2), N'0d0f2f0a-164e-42c7-bc9e-4437f4e23d34'),
                (N'10052-002', N'6e77147d-d2af-46bd-a2f2-bb4f235daf3a', 1, CAST(N'2020-04-01T11:37:44.9366667' AS datetime2), N'0d0f2f0a-164e-42c7-bc9e-4437f4e23d34'),
                (N'10052-002', N'64e5986d-1ebf-4df0-8219-c150c082ca7b', 1, CAST(N'2020-04-01T11:37:44.9366667' AS datetime2), N'0d0f2f0a-164e-42c7-bc9e-4437f4e23d34'),
                (N'10059-001', N'a71f2be1-6395-4db7-828c-d4733b42b5b5', 1, CAST(N'2020-03-30T13:16:49.4133333' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10063-002', N'6e77147d-d2af-46bd-a2f2-bb4f235daf3a', 1, CAST(N'2020-06-25T14:30:56.3300000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10063-002', N'7be309d9-696f-4b90-a65e-eb16dd5ac4ed', 1, CAST(N'2020-06-25T14:30:56.3300000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10072-003', N'6e77147d-d2af-46bd-a2f2-bb4f235daf3a', 1, CAST(N'2020-06-25T14:30:33.5166667' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10072-004', N'21ae013d-42a4-4748-b435-73d5887944c2', 1, CAST(N'2020-06-25T14:31:34.0466667' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10072-004', N'6e77147d-d2af-46bd-a2f2-bb4f235daf3a', 1, CAST(N'2020-06-25T14:31:34.0466667' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10073-009', N'6e77147d-d2af-46bd-a2f2-bb4f235daf3a', 1, CAST(N'2020-04-01T12:49:33.9433333' AS datetime2), N'00000000-0000-0000-0000-000000000000');
    
    MERGE INTO dbo.SolutionCapability AS TARGET
    USING #SolutionCapability AS SOURCE
    ON TARGET.SolutionId = SOURCE.SolutionId AND TARGET.CapabilityId = SOURCE.CapabilityId 
    WHEN MATCHED THEN
           UPDATE SET TARGET.StatusId = SOURCE.StatusId,
                      TARGET.LastUpdated = SOURCE.LastUpdated,
                      TARGET.LastUpdatedBy = SOURCE.LastUpdatedBy
    WHEN NOT MATCHED BY TARGET THEN
        INSERT (SolutionId, CapabilityId, StatusId, LastUpdated, LastUpdatedBy)
        VALUES (SOURCE.SolutionId, SOURCE.CapabilityId, SOURCE.StatusId, SOURCE.LastUpdated, SOURCE.LastUpdatedBy);
END;
GO
