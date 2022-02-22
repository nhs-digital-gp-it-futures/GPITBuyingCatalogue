DECLARE @csuRoleId AS nchar(5) = 'RO213';

SET IDENTITY_INSERT organisations.Organisations ON;

IF UPPER('$(SEED_ORGANISATIONS)') = 'TRUE' AND NOT EXISTS (SELECT * FROM organisations.Organisations WHERE PrimaryRoleId = @csuRoleId)
    INSERT INTO organisations.Organisations (Id, [Name], [Address], OdsCode, PrimaryRoleId, ExternalIdentifier, InternalIdentifier, OrganisationTypeId)
    VALUES
    (185, 'NHS Anglia Commissioning Support Unit', '{"line1":"LAKESIDE 400","line2":"OLD CHAPEL WAY","line3":"BROADLAND BUSINESS PARK","town":"NORWICH","county":"NORFOLK","postcode":"NR7 0WG","country":"ENGLAND"}', '0AP', @csuRoleId, '0AP', '0AP', 1),
    (186, 'NHS Arden and Greater East Midlands Commissioning Support Unit', '{"line1":"ST JOHNS HOUSE","line2":"30 EAST STREET","town":"LEICESTER","postcode":"LE1 6NB","country":"ENGLAND"}', '0DE', @csuRoleId, '0DE', '0DE', 1),
    (187, 'NHS Central Eastern Commissioning Support Unit', '{"line1":"CHARTER HOUSE","line2":"PARKWAY","town":"WELWYN GARDEN CITY","county":"HERTFORDSHIRE","postcode":"AL8 6JL","country":"ENGLAND"}', '0CG', @csuRoleId, '0CG', '0CG', 1),
    (188, 'NHS Central Midlands Commissioning Support Unit', '{"line1":"KINGSTON HOUSE","line2":"438 HIGH STREET","town":"WEST BROMWICH","county":"WEST MIDLANDS","postcode":"B70 9LD","country":"ENGLAND"}', '0AD', @csuRoleId, '0AD', '0AD', 1),
    (189, 'NHS Central Southern Commissioning Support Unit', '{"line1":"OXFORD ROAD","town":"NEWBURY","county":"BERKSHIRE","postcode":"RG14 1PA","country":"ENGLAND"}', '0AE', @csuRoleId, '0AE', '0AE', 1),
    (190, 'NHS Cheshire and Merseyside Commissioning Support Unit', '{"line1":"65 STEPHENSON WAY","line2":"WAVERTREE","line3":"TECHNOLOGY PARK","town":"LIVERPOOL","county":"MERSEYSIDE","postcode":"L13 1HN","country":"ENGLAND"}', '0CE', @csuRoleId, '0CE', '0CE', 1),
    (191, 'NHS Greater Manchester Commissioning Support Unit', '{"line1":"ST. JAMES HOUSE","line2":"PENDLETON WAY","town":"SALFORD","county":"GREATER MANCHESTER","postcode":"M6 5FW","country":"ENGLAND"}', '0AJ', @csuRoleId, '0AJ', '0AJ', 1),
    (192, 'NHS Kent and Medway Commissioning Support Unit', '{"line1":"KENT HOUSE","line2":"81 STATION ROAD","town":"ASHFORD","county":"KENT","postcode":"TN23 1PP","country":"ENGLAND"}', '0AM', @csuRoleId, '0AM', '0AM', 1),
    (193, 'NHS Midlands and Lancashire Commissioning Support Unit', '{"line1":"KINGSTON HOUSE","line2":"438-450 HIGH STREET","town":"WEST BROMWICH","postcode":"B70 9LD","country":"ENGLAND"}', '0CX', @csuRoleId, '0CX', '0CX', 1),
    (194, 'NHS NEL CSU', '{"line1":"CLIFTON HOUSE","line2":"75-77 WORSHIP STREET","town":"LONDON","postcode":"EC2A 2DU","country":"ENGLAND"}', '0DJ', @csuRoleId, '0DJ', '0DJ', 1),
    (195, 'NHS North and East London Commissioning Support Unit', '{"line1":"CLIFTON HOUSE","line2":"75-77 WORSHIP STREET","town":"LONDON","county":"GREATER LONDON","postcode":"EC2A 2DU","country":"ENGLAND"}', '0AQ', @csuRoleId, '0AQ', '0AQ', 1),
    (196, 'NHS North of England Commissioning Support Unit', '{"line1":"JOHN SNOW HOUSE","line2":"DURHAM UNIVERSITY SCIENCE PARK","town":"DURHAM","postcode":"DH1 3YG","country":"ENGLAND"}', '0AR', @csuRoleId, '0AR', '0AR', 1),
    (197, 'NHS North West London Commissioning Support Unit', '{"line1":"15 MARYLEBONE ROAD","town":"LONDON","county":"GREATER LONDON","postcode":"NW1 5JD","country":"ENGLAND"}', '0AT', @csuRoleId, '0AT', '0AT', 1),
    (198, 'NHS South, Central and West Commissioning Support Unit', '{"line1":"OMEGA HOUSE","line2":"112 SOUTHAMPTON ROAD","town":"EASTLEIGH","postcode":"SO50 5PB","country":"ENGLAND"}', '0DF', @csuRoleId, '0DF', '0DF', 1),
    (199, 'NHS South London Commissioning Support Unit', '{"line1":"1 LOWER MARSH","town":"LONDON","county":"GREATER LONDON","postcode":"SE1 7NT","country":"ENGLAND"}', '0AX', @csuRoleId, '0AX', '0AX', 1),
    (200, 'NHS Staffordshire and Lancashire Commissioning Support Unit', '{"line1":"ANGLESEY HOUSE","line2":"TOWERS BUSINESS PARK","line3":"WHEELHOUSE ROAD","town":"RUGELEY","county":"STAFFORDSHIRE","postcode":"WS15 1UZ","country":"ENGLAND"}', '0CH', @csuRoleId, '0CH', '0CH', 1),
    (201, 'NHS Surrey and Sussex Commissioning Support Unit', '{"line1":"36-38 FRIARS WALK","town":"LEWES","county":"EAST SUSSEX","postcode":"BN7 2PB","country":"ENGLAND"}', '0CC', @csuRoleId, '0CC', '0CC', 1),
    (202, 'NHS West and South Yorkshire and Bassetlaw Commissioning Support Unit', '{"line1":"DOUGLAS MILLS","line2":"BOWLING OLD LANE","town":"BRADFORD","county":"WEST YORKSHIRE","postcode":"BD5 7JR","country":"ENGLAND"}', '0CF', @csuRoleId, '0CF', '0CF', 1),
    (203, 'Optum Health Solutions (UK) Limited', '{"line1":"1ST FLOOR STAR HOUSE","line2":"20 GRENFELL ROAD","town":"MAIDENHEAD","county":"BERKSHIRE","postcode":"SL6 1EH","country":"ENGLAND"}', '0DG', @csuRoleId, '0DG', '0DG', 1);
SET IDENTITY_INSERT organisations.Organisations OFF;
GO
