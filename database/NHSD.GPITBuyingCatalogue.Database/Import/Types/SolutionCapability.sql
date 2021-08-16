CREATE TYPE import.SolutionCapability AS TABLE
(
    PRIMARY KEY (SolutionId, CapabilityRef),
    SolutionId nvarchar(14) NOT NULL,
    CapabilityRef nvarchar(10) NOT NULL
);
