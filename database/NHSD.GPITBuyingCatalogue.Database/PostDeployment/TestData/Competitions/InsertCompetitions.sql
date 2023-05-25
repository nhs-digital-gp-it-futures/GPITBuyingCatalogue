IF UPPER('$(INSERT_TEST_DATA)') = 'TRUE' AND NOT EXISTS (SELECT * FROM competitions.Competitions)
BEGIN

    DECLARE @organisationId NVARCHAR(3) = (SELECT Id FROM organisations.Organisations WHERE ExternalIdentifier = 'QWO');

    SET IDENTITY_INSERT competitions.Competitions ON;

    INSERT INTO competitions.Competitions ([Id], [Name], [Description], [FilterId], [OrganisationId], [Completed]) VALUES
           (1, 'Competition #1', 'Lorem ipsum dolor sit amet, consectetur adipisicing elit. Magnam deserunt consectetur non voluptatem rem tenetur. ', 1, @organisationId, NULL),
           (2, 'Competition #2', 'Lorem ipsum dolor sit amet, consectetur adipisicing elit. Magnam deserunt consectetur non voluptatem rem tenetur. ', 2, @organisationId, GETUTCDATE()),
           (3, 'Competition #3', 'Lorem ipsum dolor sit amet, consectetur adipisicing elit. Magnam deserunt consectetur non voluptatem rem tenetur. ', 3, @organisationId, NULL);

    SET IDENTITY_INSERT competitions.Competitions OFF;

END
GO
