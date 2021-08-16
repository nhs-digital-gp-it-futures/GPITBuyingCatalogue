CREATE TYPE import.AssociatedCatalogueItems AS TABLE
(
    PRIMARY KEY (AssociatedServiceId, AssociatedCatalogueItemId),
    AssociatedServiceId nvarchar(14) NOT NULL,
    AssociatedCatalogueItemId nvarchar(14) NOT NULL
);
