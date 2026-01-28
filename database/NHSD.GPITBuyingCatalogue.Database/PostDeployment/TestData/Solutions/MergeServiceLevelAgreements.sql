IF UPPER('$(INSERT_TEST_DATA)') = 'TRUE'
BEGIN
    MERGE INTO [catalogue].[ServiceLevelAgreements] AS TARGET
    USING (
    VALUES
        (N'100000-001', 1, GETUTCDATE(), 2),
        (N'10000-001', 1, GETUTCDATE(), 2),
        (N'10000-002', 1, GETUTCDATE(), 2),
        (N'10000-062', 1, GETUTCDATE(), 2),
        (N'100001-001', 1, GETUTCDATE(), 2),
        (N'100002-001', 1, GETUTCDATE(), 2),
        (N'100003-001', 1, GETUTCDATE(), 2),
        (N'100004-001', 1, GETUTCDATE(), 2),
        (N'100005-001', 1, GETUTCDATE(), 2),
        (N'100007-001', 1, GETUTCDATE(), 2),
        (N'100007-002', 1, GETUTCDATE(), 2),
        (N'100021-001', 1, GETUTCDATE(), 2),
        (N'100021-002', 1, GETUTCDATE(), 2),
        (N'100021-003', 1, GETUTCDATE(), 2),
        (N'100021-004', 1, GETUTCDATE(), 2),
        (N'100021-005', 1, GETUTCDATE(), 2),
        (N'100021-006', 1, GETUTCDATE(), 2),
        (N'100021-007', 1, GETUTCDATE(), 2),
        (N'100021-008', 1, GETUTCDATE(), 2),
        (N'100021-009', 1, GETUTCDATE(), 2),
        (N'100021-010', 1, GETUTCDATE(), 2),
        (N'100021-011', 1, GETUTCDATE(), 2),
        (N'100021-012', 1, GETUTCDATE(), 2),
        (N'100021-013', 1, GETUTCDATE(), 2),
        (N'100021-014', 1, GETUTCDATE(), 2),
        (N'100021-015', 1, GETUTCDATE(), 2),
        (N'100021-016', 1, GETUTCDATE(), 2),
        (N'100021-017', 1, GETUTCDATE(), 2),
        (N'100021-018', 1, GETUTCDATE(), 2),
        (N'100021-019', 1, GETUTCDATE(), 2),
        (N'100021-020', 1, GETUTCDATE(), 2),
        (N'10004-001', 1, GETUTCDATE(), 2),
        (N'10004-002', 1, GETUTCDATE(), 2),
        (N'10007-002', 1, GETUTCDATE(), 2),
        (N'10029-003', 1, GETUTCDATE(), 2),
        (N'10030-001', 1, GETUTCDATE(), 2),
        (N'10033-001', 1, GETUTCDATE(), 2),
        (N'10035-001', 1, GETUTCDATE(), 2),
        (N'10046-001', 1, GETUTCDATE(), 2),
        (N'10046-003', 1, GETUTCDATE(), 2),
        (N'10047-001', 1, GETUTCDATE(), 2),
        (N'10052-002', 1, GETUTCDATE(), 2),
        (N'10059-001', 1, GETUTCDATE(), 2),
        (N'10073-009', 1, GETUTCDATE(), 2),
        (N'99998-98', 1, GETUTCDATE(), 2),
        (N'99999-01', 1, GETUTCDATE(), 2),
        (N'99999-89', 1, GETUTCDATE(), 2))
    AS SOURCE ([SolutionId], [SlaType], [LastUpdated], [LastUpdatedBy])
    ON TARGET.[SolutionId] = SOURCE.[SolutionId]
    WHEN MATCHED THEN UPDATE SET
        TARGET.[SlaType] = SOURCE.[SlaType],
        TARGET.[LastUpdated] = SOURCE.[LastUpdated],
        TARGET.[LastUpdatedBy] = SOURCE.[LastUpdatedBy]
    WHEN NOT MATCHED BY TARGET THEN
        INSERT ([SolutionId], [SlaType], [LastUpdated], [LastUpdatedBy])
        VALUES (SOURCE.[SolutionId], SOURCE.[SlaType], SOURCE.[LastUpdated], SOURCE.[LastUpdatedBy]);
END

GO
