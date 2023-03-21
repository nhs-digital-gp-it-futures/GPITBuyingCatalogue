CREATE PROCEDURE [catalogue].[FilterCatalogueItems]
	@CapabilityIds catalogue.CapabilityIds READONLY,
	@EpicIds catalogue.EpicIds READONLY
AS

SET NOCOUNT ON;

	DECLARE @ReferencedEpics TABLE(
		CapabilityId INT NOT NULL,
		EpicId NVARCHAR(10) NOT NULL
	);

	INSERT INTO @ReferencedEpics
	SELECT
		E.CapabilityId,
		E.Id
	FROM catalogue.Epics E
	INNER JOIN @EpicIds EI
		ON E.Id = EI.Id;

    -------------------------------------------------------------------------------------
    -- First Result Set : List of Catalogue Item Ids that match the filtering parameters
    -------------------------------------------------------------------------------------
	SELECT
		CI.Id
	FROM catalogue.CatalogueItems CI
	INNER JOIN catalogue.Suppliers SUP
		ON SUP.Id = CI.SupplierId
	--- Filters Solutions that Reference the Capabilities
	INNER JOIN catalogue.CatalogueItemCapabilities CIC
		ON CIC.CatalogueItemId = CI.Id
	INNER JOIN @CapabilityIds CAPI
		ON CAPI.Id = CIC.CapabilityId
	-----------------------------------------------------
	LEFT JOIN (
		SELECT
			COUNT(CIE.EpicId) AS COUNT,
			CIE.CapabilityId,
			CIE.CatalogueItemId
		FROM @ReferencedEpics RE
		LEFT JOIN catalogue.CatalogueItemEpics CIE
			ON CIE.EpicId = RE.EpicId
		GROUP BY
			CIE.CapabilityId,
			CIE.CatalogueItemId
	) AS CIE
		ON CIE.CatalogueItemId = CI.Id
		AND CIE.CapabilityId = CAPI.Id
	INNER JOIN(
		SELECT
			COUNT(RE.EpicId) COUNT,
			CAPI.Id
		FROM @CapabilityIds CAPI
		LEFT JOIN @ReferencedEpics RE
			ON RE.CapabilityId = CAPI.Id
		GROUP BY
			CAPI.Id
	) AS COE
		ON COE.Id = CAPI.Id
		AND ISNULL(COE.COUNT, 0) = ISNULL(CIE.COUNT, 0)
	WHERE
	CI.CatalogueItemTypeId IN (1,2) -- solutions and additional services
	AND CI.PublishedStatusId IN (3, 4, 5) -- published, suspended or in remediation
	AND SUP.IsActive = 1
	UNION
	--- This brings back the solutions for additional services that reference the capabilities/Epics
	SELECT
		CIP.Id
	FROM catalogue.CatalogueItems CI
	INNER JOIN catalogue.Suppliers SUP
		ON SUP.Id = CI.SupplierId
	--- Filters Solutions that Reference the Capabilities
	INNER JOIN catalogue.CatalogueItemCapabilities CIC
		ON CIC.CatalogueItemId = CI.Id
	INNER JOIN @CapabilityIds CAPI
		ON CAPI.Id = CIC.CapabilityId
	------------------------------------------------------
	INNER JOIN catalogue.AdditionalServices ADIT
		ON ADIT.CatalogueItemId = CI.Id
	INNER JOIN catalogue.CatalogueItems CIP
		ON ADIT.SolutionId = CIP.Id
	LEFT JOIN (
		SELECT
			COUNT(CIE.EpicId) AS COUNT,
			CIE.CapabilityId,
			CIE.CatalogueItemId
		FROM @ReferencedEpics RE
		LEFT JOIN catalogue.CatalogueItemEpics CIE
			ON CIE.EpicId = RE.EpicId
		GROUP BY
			CIE.CapabilityId,
			CIE.CatalogueItemId
	) AS CIE
		ON CIE.CatalogueItemId = CI.Id
		AND CIE.CapabilityId = CAPI.Id
	INNER JOIN(
		SELECT
			COUNT(RE.EpicId) COUNT,
			CAPI.Id
		FROM @CapabilityIds CAPI
		LEFT JOIN @ReferencedEpics RE
			ON RE.CapabilityId = CAPI.Id
		GROUP BY
			CAPI.Id
	) AS COE
		ON COE.Id = CAPI.Id
		AND ISNULL(COE.COUNT, 0) = ISNULL(CIE.COUNT, 0)
	WHERE
	CI.CatalogueItemTypeId IN (2) -- additional services only
    AND CI.PublishedStatusId IN (3, 4, 5) -- published, suspended or in remediation
    AND CIP.PublishedStatusId IN (3, 4, 5) -- Solution is either published, suspended or in remediation
	AND SUP.IsActive = 1
	ORDER BY CI.Id;

    -------------------------------------------------------------------------------------
    -- Second result set : List of selected capability names and the number of referenced
    -- Epics for each capability
    -------------------------------------------------------------------------------------
    SELECT
        C.Id,
	    C.Name,
        COUNT(RE.EpicId) AS CountOfEpics
	FROM @CapabilityIds CI
	INNER JOIN catalogue.Capabilities C
		ON CI.Id = C.Id
	LEFT JOIN @ReferencedEpics RE
		ON RE.CapabilityId = CI.Id
	GROUP BY
    C.Id,
	C.Name,
	RE.CapabilityId;
