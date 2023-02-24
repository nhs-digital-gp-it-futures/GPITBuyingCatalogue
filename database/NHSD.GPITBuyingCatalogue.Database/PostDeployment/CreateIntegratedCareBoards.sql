﻿DECLARE @icbRoleId AS nchar(5) = 'RO261';

IF UPPER('$(INSERT_TEST_DATA)') = 'TRUE' AND NOT EXISTS (SELECT * FROM organisations.Organisations WHERE PrimaryRoleId = @icbRoleId)

DECLARE @organisations AS TABLE
(
	[Name] [nvarchar](255) NOT NULL,
	[Address] [nvarchar](max) NULL,
	[OdsCode] [nvarchar](8) NULL,
	[PrimaryRoleId] [nvarchar](8) NULL,
	[ExternalIdentifier] [nvarchar](100) NULL,
	[InternalIdentifier] [nvarchar](103) NULL,
	[OrganisationTypeId] [int] NULL
)

INSERT INTO @organisations ([Name], [Address], OdsCode, PrimaryRoleId, ExternalIdentifier, InternalIdentifier, OrganisationTypeId)
VALUES
('NHS LANCASHIRE AND SOUTH CUMBRIA INTEGRATED CARE BOARD', '{"line1":"2ND FLOOR PRESTON BUSINESS CENTRE","line2":"WATLING STREET ROAD","line3":"FULWOOD","town":"PRESTON","county":"NULL","postcode":"PR2 8DY","country":"ENGLAND"}', 'QE1', @icbRoleId, 'QE1', 'IB-QE1', 3),
('NHS SOUTH YORKSHIRE INTEGRATED CARE BOARD', '{"line1":"COMMISSIONERS WORKING TOGETHER","line2":"722 PRINCE OF WALES ROAD","line3":"NULL","town":"SHEFFIELD","county":"NULL","postcode":"S9 4EU","country":"ENGLAND"}', 'QF7', @icbRoleId, 'QF7', 'IB-QF7', 3),
('NHS HEREFORDSHIRE AND WORCESTERSHIRE INTEGRATED CARE BOARD', '{"line1":"ISAAC MADDOX HOUSE","line2":"SHRUB HILL INDUSTRIAL ESTATE","line3":"NULL","town":"WORCESTER","county":"WORCESTERSHIRE","postcode":"WR4 9EL","country":"ENGLAND"}', 'QGH', @icbRoleId, 'QGH', 'IB-QGH', 3),
('NHS MID AND SOUTH ESSEX INTEGRATED CARE BOARD', '{"line1":"SWIFT HOUSE","line2":"HEDGEROWS BUSINESS PARK","line3":"COLCHESTER ROAD","town":"CHELMSFORD","county":"NULL","postcode":"CM2 5PF","country":"ENGLAND"}', 'QH8', @icbRoleId, 'QH8', 'IB-QH8', 3),
('NHS BEDFORDSHIRE, LUTON AND MILTON KEYNES INTEGRATED CARE BOARD', '{"line1":"3RD FLOOR","line2":"ARNDALE HOUSE","line3":"THE MALL","town":"LUTON","county":"NULL","postcode":"LU1 2LJ","country":"ENGLAND"}', 'QHG', @icbRoleId, 'QHG', 'IB-QHG', 3),
('NHS BIRMINGHAM AND SOLIHULL INTEGRATED CARE BOARD', '{"line1":"1ST FLOOR WESLEYAN","line2":"COLMORE CIRCUS","line3":"NULL","town":"BIRMINGHAM","county":"NULL","postcode":"B4 6AR","country":"ENGLAND"}', 'QHL', @icbRoleId, 'QHL', 'IB-QHL', 3),
('NHS NORTH EAST AND NORTH CUMBRIA INTEGRATED CARE BOARD', '{"line1":"VOREDA","line2":"PORTLAND PLACE","line3":"NULL","town":"PENRITH","county":"CUMBRIA","postcode":"CA11 7BF","country":"ENGLAND"}', 'QHM', @icbRoleId, 'QHM', 'IB-QHM', 3),
('NHS DERBY AND DERBYSHIRE INTEGRATED CARE BOARD', '{"line1":"TOLLBAR HOUSE","line2":"TOP FLOOR","line3":"1 DERBY ROAD","town":"ILKESTON","county":"NULL","postcode":"DE7 5FH","country":"ENGLAND"}', 'QJ2', @icbRoleId, 'QJ2', 'IB-QJ2', 3),
('NHS SUFFOLK AND NORTH EAST ESSEX INTEGRATED CARE BOARD', '{"line1":"ASPEN HOUSE","line2":"STEPHENSON ROAD","line3":"SEVERALLS INDUSTRIAL PARK","town":"COLCHESTER","county":"NULL","postcode":"CO4 9QR","country":"ENGLAND"}', 'QJG', @icbRoleId, 'QJG', 'IB-QJG', 3),
('NHS DEVON INTEGRATED CARE BOARD', '{"line1":"NEWCOURT HOUSE","line2":"OLD RYDON LANE","line3":"NULL","town":"EXETER","county":"NULL","postcode":"EX2 7JU","country":"ENGLAND"}', 'QJK', @icbRoleId, 'QJK', 'IB-QJK', 3),
('NHS LINCOLNSHIRE INTEGRATED CARE BOARD', '{"line1":"WYVERN HOUSE","line2":"KESTEVEN STREET","line3":"NULL","town":"LINCOLN","county":"NULL","postcode":"LN5 7LH","country":"ENGLAND"}', 'QJM', @icbRoleId, 'QJM', 'IB-QJM', 3),
('NHS LEICESTER, LEICESTERSHIRE AND RUTLAND INTEGRATED CARE BOARD', '{"line1":"BETTER CARE TOGETHER","line2":"1ST FLOOR ST JOHNS HOUSE","line3":"30 EAST STREET","town":"LEICESTER","county":"NULL","postcode":"LE1 6NB","country":"ENGLAND"}', 'QK1', @icbRoleId, 'QK1', 'IB-QK1', 3),
('NHS SOUTH EAST LONDON INTEGRATED CARE BOARD', '{"line1":"SOUTHWARK COUNCIL","line2":"160 TOOLEY STREET","line3":"NULL","town":"LONDON","county":"NULL","postcode":"SE1 2QH","country":"ENGLAND"}', 'QKK', @icbRoleId, 'QKK', 'IB-QKK', 3),
('NHS KENT AND MEDWAY INTEGRATED CARE BOARD', '{"line1":"MAGNITUDE HOUSE","line2":"RIVERSIDE BUSINESS PARK","line3":"NEW HYTHE LANE, LARKFIELD","town":"AYLESFORD","county":"NULL","postcode":"ME20 6WT","country":"ENGLAND"}', 'QKS', @icbRoleId, 'QKS', 'IB-QKS', 3),
('NHS HERTFORDSHIRE AND WEST ESSEX INTEGRATED CARE BOARD', '{"line1":"CHARTER HOUSE","line2":"PARKWAY","line3":"NULL","town":"WELWYN GARDEN CITY","county":"HERTFORDSHIRE","postcode":"AL8 6JL","country":"ENGLAND"}', 'QM7', @icbRoleId, 'QM7', 'IB-QM7', 3),
('NHS NORTH EAST LONDON INTEGRATED CARE BOARD', '{"line1":"UNEX TOWER","line2":"5 STATION STREET","line3":"NULL","town":"LONDON","county":"NULL","postcode":"E15 1DA","country":"ENGLAND"}', 'QMF', @icbRoleId, 'QMF', 'IB-QMF', 3),
('NHS NORTH CENTRAL LONDON INTEGRATED CARE BOARD', '{"line1":"CAMDEN COUNCIL","line2":"5TH FLOOR","line3":"5 PANCRAS SQUARE","town":"LONDON","county":"NULL","postcode":"N1C 4AG","country":"ENGLAND"}', 'QMJ', @icbRoleId, 'QMJ', 'IB-QMJ', 3),
('NHS NORFOLK AND WAVENEY INTEGRATED CARE BOARD', '{"line1":"LOWESTOFT ROAD","line2":"GORLESTON","line3":"NULL","town":"GREAT YARMOUTH","county":"NULL","postcode":"NR31 6LA","country":"ENGLAND"}', 'QMM', @icbRoleId, 'QMM', 'IB-QMM', 3),
('NHS STAFFORDSHIRE AND STOKE-ON-TRENT INTEGRATED CARE BOARD', '{"line1":"1 STAFFORDSHIRE PLACE","line2":"NULL","line3":"NULL","town":"STAFFORD","county":"NULL","postcode":"ST16 2LP","country":"ENGLAND"}', 'QNC', @icbRoleId, 'QNC', 'IB-QNC', 3),
('NHS FRIMLEY INTEGRATED CARE BOARD', '{"line1":"PORTSMOUTH ROAD","line2":"FRIMLEY","line3":"NULL","town":"CAMBERLEY","county":"NULL","postcode":"GU16 7UJ","country":"ENGLAND"}', 'QNQ', @icbRoleId, 'QNQ', 'IB-QNQ', 3),
('NHS SUSSEX INTEGRATED CARE BOARD', '{"line1":"TRUST HEADQUARTERS","line2":"EAST SURREY HOSPITAL","line3":"CANADA AVENUE","town":"REDHILL","county":"SURREY","postcode":"RH1 5RH","country":"ENGLAND"}', 'QNX', @icbRoleId, 'QNX', 'IB-QNX', 3),
('NHS SHROPSHIRE, TELFORD AND WREKIN INTEGRATED CARE BOARD', '{"line1":"MYTTON OAK ROAD","line2":"NULL","line3":"NULL","town":"SHREWSBURY","county":"NULL","postcode":"SY3 8XQ","country":"ENGLAND"}', 'QOC', @icbRoleId, 'QOC', 'IB-QOC', 3),
('NHS GREATER MANCHESTER INTEGRATED CARE BOARD', '{"line1":"4TH FLOOR","line2":"3 PICCADILLY PLACE","line3":"NULL","town":"MANCHESTER","county":"NULL","postcode":"M1 3BN","country":"ENGLAND"}', 'QOP', @icbRoleId, 'QOP', 'IB-QOP', 3),
('NHS HUMBER AND NORTH YORKSHIRE INTEGRATED CARE BOARD', '{"line1":"2ND FLOOR","line2":"WILBERFORCE COURT","line3":"ALFRED GELDER STREET","town":"HULL","county":"NULL","postcode":"HU1 1UY","country":"ENGLAND"}', 'QOQ', @icbRoleId, 'QOQ', 'IB-QOQ', 3),
('NHS BATH AND NORTH EAST SOMERSET, SWINDON AND WILTSHIRE INTEGRATED CARE BOARD', '{"line1":"JENNER HOUSE","line2":"AVON WAY","line3":"LANGLEY PARK","town":"CHIPPENHAM","county":"NULL","postcode":"SN15 1GG","country":"ENGLAND"}', 'QOX', @icbRoleId, 'QOX', 'IB-QOX', 3),
('NHS NORTHAMPTONSHIRE INTEGRATED CARE BOARD', '{"line1":"NHS N. H''SHIRE, FRANCIS CRICK HOUSE","line2":"6 SUMMERHOUSE ROAD","line3":"MOULTON PARK INDUSTRIAL ESTATE","town":"NORTHAMPTON","county":"NORTHAMPTONSHIRE","postcode":"NN3 6BF","country":"ENGLAND"}', 'QPM', @icbRoleId, 'QPM', 'IB-QPM', 3),
('NHS GLOUCESTERSHIRE INTEGRATED CARE BOARD', '{"line1":"SANGER HOUSE UNIT 5220","line2":"VALIANT COURT, DELTA WAY","line3":"GLOUCESTER PARK, BROCKWORTH","town":"GLOUCESTER","county":"NULL","postcode":"GL3 4FE","country":"ENGLAND"}', 'QR1', @icbRoleId, 'QR1', 'IB-QR1', 3),
('NHS HAMPSHIRE AND ISLE OF WIGHT INTEGRATED CARE BOARD', '{"line1":"TREMONA ROAD","line2":"NULL","line3":"NULL","town":"SOUTHAMPTON","county":"NULL","postcode":"SO16 6YD","country":"ENGLAND"}', 'QRL', @icbRoleId, 'QRL', 'IB-QRL', 3),
('NHS NORTH WEST LONDON INTEGRATED CARE BOARD', '{"line1":"15 MARYLEBONE ROAD","line2":"NULL","line3":"NULL","town":"LONDON","county":"NULL","postcode":"NW1 5JD","country":"ENGLAND"}', 'QRV', @icbRoleId, 'QRV', 'IB-QRV', 3),
('NHS SOMERSET INTEGRATED CARE BOARD', '{"line1":"2ND FLOOR MALLARD COURT","line2":"EXPRESS PARK","line3":"BRISTOL ROAD","town":"BRIDGWATER","county":"SOMERSET","postcode":"TA6 4RN","country":"ENGLAND"}', 'QSL', @icbRoleId, 'QSL', 'IB-QSL', 3),
('NHS NOTTINGHAM AND NOTTINGHAMSHIRE INTEGRATED CARE BOARD', '{"line1":"COUNTY HALL","line2":"LOUGHBOROUGH ROAD","line3":"WEST BRIDGFORD","town":"NOTTINGHAM","county":"NULL","postcode":"NG2 7QP","country":"ENGLAND"}', 'QT1', @icbRoleId, 'QT1', 'IB-QT1', 3),
('NHS CORNWALL AND THE ISLES OF SCILLY INTEGRATED CARE BOARD', '{"line1":"PART 2S","line2":"CHY TREVAIL","line3":"BEACON TECHNOLOGY PARK","town":"BODMIN","county":"CORNWALL","postcode":"PL31 2FR","country":"ENGLAND"}', 'QT6', @icbRoleId, 'QT6', 'IB-QT6', 3),
('NHS BUCKINGHAMSHIRE, OXFORDSHIRE AND BERKSHIRE WEST INTEGRATED CARE BOARD', '{"line1":"SANDFORD GATE","line2":"SANDY LANE WEST","line3":"LITTLEMORE","town":"OXFORD","county":"OXFORDSHIRE","postcode":"OX4 6LB","country":"ENGLAND"}', 'QU9', @icbRoleId, 'QU9', 'IB-QU9', 3),
('NHS BLACK COUNTRY INTEGRATED CARE BOARD', '{"line1":"CIVIC CENTRE","line2":"ST. PETERS SQUARE","line3":"NULL","town":"WOLVERHAMPTON","county":"WEST MIDLANDS","postcode":"WV1 1SD","country":"ENGLAND"}', 'QUA', @icbRoleId, 'QUA', 'IB-QUA', 3),
('NHS CAMBRIDGESHIRE AND PETERBOROUGH INTEGRATED CARE BOARD', '{"line1":"BLOCK 11 IDA DARWIN HOSPITAL","line2":"CAMBRIDGE ROAD","line3":"FULBOURN","town":"CAMBRIDGE","county":"NULL","postcode":"CB21 5EE","country":"ENGLAND"}', 'QUE', @icbRoleId, 'QUE', 'IB-QUE', 3),
('NHS BRISTOL, NORTH SOMERSET AND SOUTH GLOUCESTERSHIRE INTEGRATED CARE BOARD', '{"line1":"SOUTH PLAZA","line2":"MARLBOROUGH STREET","line3":"NULL","town":"BRISTOL","county":"NULL","postcode":"BS1 3NX","country":"ENGLAND"}', 'QUY', @icbRoleId, 'QUY', 'IB-QUY', 3),
('NHS DORSET INTEGRATED CARE BOARD', '{"line1":"WILLIAMS AVENUE","line2":"NULL","line3":"NULL","town":"DORCHESTER","county":"NULL","postcode":"DT1 2JY","country":"ENGLAND"}', 'QVV', @icbRoleId, 'QVV', 'IB-QVV', 3),
('NHS SOUTH WEST LONDON INTEGRATED CARE BOARD', '{"line1":"120 THE BROADWAY","line2":"NULL","line3":"NULL","town":"LONDON","county":"GREATER LONDON","postcode":"SW19 1RH","country":"ENGLAND"}', 'QWE', @icbRoleId, 'QWE', 'IB-QWE', 3),
('NHS WEST YORKSHIRE INTEGRATED CARE BOARD', '{"line1":"WHITE ROSE HOUSE","line2":"WEST PARADE","line3":"NULL","town":"WAKEFIELD","county":"NULL","postcode":"WF1 1LT","country":"ENGLAND"}', 'QWO', @icbRoleId, 'QWO', 'IB-QWO', 3),
('NHS COVENTRY AND WARWICKSHIRE INTEGRATED CARE BOARD', '{"line1":"WALSGRAVE GENERAL HOSPITAL","line2":"CLIFFORD BRIDGE ROAD","line3":"NULL","town":"COVENTRY","county":"NULL","postcode":"CV2 2DX","country":"ENGLAND"}', 'QWU', @icbRoleId, 'QWU', 'IB-QWU', 3),
('NHS SURREY HEARTLANDS INTEGRATED CARE BOARD', '{"line1":"WOODHATCH PLACE","line2":"11 COCKSHOT HILL","line3":"NULL","town":"REIGATE","county":"NULL","postcode":"RH2 8EF","country":"ENGLAND"}', 'QXU', @icbRoleId, 'QXU', 'IB-QXU', 3),
('NHS CHESHIRE AND MERSEYSIDE INTEGRATED CARE BOARD', '{"line1":"WARRINGTON HOSPITAL","line2":"LOVELY LANE","line3":"NULL","town":"WARRINGTON","county":"NULL","postcode":"WA5 1QG","country":"ENGLAND"}', 'QYG', @icbRoleId, 'QYG', 'IB-QYG', 3);
END

MERGE INTO [organisations].[Organisations] AS TARGET
    USING @organisations AS SOURCE ON TARGET.ExternalIdentifier = SOURCE.ExternalIdentifier
    WHEN MATCHED THEN
        UPDATE SET
        TARGET.[Name] = SOURCE.[Name],
        TARGET.[Address] = SOURCE.[Address],
        TARGET.[OdsCode] = SOURCE.[OdsCode],
        TARGET.[PrimaryRoleId] = SOURCE.[PrimaryRoleId],
		TARGET.[ExternalIdentifier] = SOURCE.[ExternalIdentifier],
        TARGET.[InternalIdentifier] = SOURCE.[InternalIdentifier],
        TARGET.[OrganisationTypeId] = SOURCE.[OrganisationTypeId]
    WHEN NOT MATCHED BY TARGET THEN
        INSERT ([Name], [Address], [OdsCode], [PrimaryRoleId], [ExternalIdentifier], [InternalIdentifier], [OrganisationTypeId])
        VALUES (SOURCE.[Name], SOURCE.[Address], SOURCE.[OdsCode], SOURCE.[PrimaryRoleId], SOURCE.[ExternalIdentifier], SOURCE.[InternalIdentifier], SOURCE.[OrganisationTypeId]);
GO
