DECLARE @emptyGuid AS uniqueidentifier = '00000000-0000-0000-0000-000000000000';
DECLARE @now AS datetime = GETUTCDATE();

IF UPPER('$(INSERT_TEST_DATA)') = 'TRUE' AND NOT EXISTS (SELECT * FROM dbo.Supplier)
BEGIN
    INSERT INTO dbo.Supplier(Id, [Name], LegalName, Summary, [Address], LastUpdated, LastUpdatedBy)
    VALUES (
        '100000',
        'Really Kool Corporation',
        'Really Kool Corporation',
        'Really Kool Corporation is a fictious UK based IT company but that''s not going to stop us making Really Kool products!',
        '{"line1": "The Tower", "line2": "High Street", "city": "Leeds", "county": "West Yorkshire", "postcode": "LS1 1BB", "country": "UK"}',
        @now,
        @emptyGuid);

    INSERT INTO dbo.Supplier(Id, [Name], LegalName, Summary, [Address], LastUpdated, LastUpdatedBy)
    VALUES
    ('100001', 'Remedical Software', 'Remedical Limited', 'The Remedical Software', '{"line1": "Remedical Software Tower", "line2": "High Street", "city": "Leeds", "county": "West Yorkshire", "postcode": "LS1 1BB", "country": "UK"}',@now, @emptyGuid),
    ('100002', 'CareShare', 'CareShare Limited', 'The CareShare', '{"line1": "CareShare Tower", "line2": "High Street", "city": "Leeds", "county": "West Yorkshire", "postcode": "LS1 1BB", "country": "UK"}',@now, @emptyGuid),
    ('100003', 'Avatar Solutions', 'Avatar Solutions Plc', 'Avatar Solutions', '{"line1": "Avatar Tower", "line2": "High Street", "city": "Leeds", "county": "West Yorkshire", "postcode": "LS1 1BB", "country": "UK"}',@now, @emptyGuid),
    ('100004', 'Catterpillar Medworks', 'Catterpillar Medworks Ltd', 'Catterpillar Medworks', '{"line1": "Medworks Tower", "line2": "High Street", "city": "Leeds", "county": "West Yorkshire", "postcode": "LS1 1BB", "country": "UK"}',@now, @emptyGuid),
    ('100005', 'Curtis Systems', 'Curtis Systems Ltd', 'Curtis Systems', '{"line1": "Curtis Tower", "line2": "High Street", "city": "Leeds", "county": "West Yorkshire", "postcode": "LS1 1BB", "country": "UK"}',@now, @emptyGuid),
    ('100006', 'Clinical Raptor', 'Clinical Raptor Ltd', 'Clinical Raptor', '{"line1": "Raptor Tower", "line2": "High Street", "city": "Leeds", "county": "West Yorkshire", "postcode": "LS1 1BB", "country": "UK"}',@now, @emptyGuid),
    ('100007', 'Doc Lightning', 'Doc Lightning Ltd', 'Doc Lightning', '{"line1": "Doc Lightning Tower", "line2": "High Street", "city": "Leeds", "county": "West Yorkshire", "postcode": "LS1 1BB", "country": "UK"}',@now, @emptyGuid),
    ('100008', 'Docability Software', 'Docability Ltd', 'Docability Software', '{"line1": "Docability Tower", "line2": "High Street", "city": "Leeds", "county": "West Yorkshire", "postcode": "LS1 1BB", "country": "UK"}',@now, @emptyGuid),
    ('100009', 'Empire Softworks',  'Empire Softworks Plc', 'Empire Softworks', '{"line1": "Empire Tower", "line2": "High Street", "city": "Leeds", "county": "West Yorkshire", "postcode": "LS1 1BB", "country": "UK"}',@now, @emptyGuid),
    ('100010', 'Cure Forward', 'Cure Forward Ltd', 'Cure Forward', '{"line1": "Cure Forward Tower", "line2": "High Street", "city": "Leeds", "county": "West Yorkshire", "postcode": "LS1 1BB", "country": "UK"}',@now, @emptyGuid),
    ('100011', 'Hansa Healthcare', 'Hansa Healthcare Plc', 'Hansa Healthcare', '{"line1": "Hansa Tower", "line2": "High Street", "city": "Leeds", "county": "West Yorkshire", "postcode": "LS1 1BB", "country": "UK"}',@now, @emptyGuid),
    ('100012', 'Moonlight Intercare', 'Moonlight Intercare', 'Moonlight Intercare', '{"line1": "Moonlight Tower", "line2": "High Street", "city": "Leeds", "county": "West Yorkshire", "postcode": "LS1 1BB", "country": "UK"}',@now, @emptyGuid),
    ('100013', 'eHealth Development', 'eHealth Development', 'eHealth Development', '{"line1": "eHealth Tower", "line2": "High Street", "city": "Leeds", "county": "West Yorkshire", "postcode": "LS1 1BB", "country": "UK"}',@now, @emptyGuid),
    ('100014', 'Dr. Nick', 'Dr. Nick', 'Dr. Nick', '{"line1": "Simpson Tower", "line2": "High Street", "city": "Leeds", "county": "West Yorkshire", "postcode": "LS1 1BB", "country": "UK"}',@now, @emptyGuid),
    ('100015', 'Testproof Technology',  'Testproof Technology', 'Testproof Technology', '{"line1": "Testproof Tower", "line2": "High Street", "city": "Leeds", "county": "West Yorkshire", "postcode": "LS1 1BB", "country": "UK"}',@now, @emptyGuid),
    ('100016', 'Hojo Health', 'Hojo Health Ltd', 'Hojo Health', '{"line1": "Hojo Tower", "line2": "High Street", "city": "Leeds", "county": "West Yorkshire", "postcode": "LS1 1BB", "country": "UK"}',@now, @emptyGuid),
    ('100017', 'Jericho Healthcare', 'Jericho Ltd', 'Jericho Healthcare', '{"line1": "Jericho Tower", "line2": "High Street", "city": "Leeds", "county": "West Yorkshire", "postcode": "LS1 1BB", "country": "UK"}',@now, @emptyGuid),
    ('100018', 'Mana Systems', 'Mana Systems', 'Mana Systems', '{"line1": "Mana Tower", "line2": "High Street", "city": "Leeds", "county": "West Yorkshire", "postcode": "LS1 1BB", "country": "UK"}',@now, @emptyGuid),
    ('100019', 'Sunhealth Nanosystems', 'Sunhealth Nanosystems', 'Sunhealth Nanosystems', '{"line1": "Sunhealth Tower", "line2": "High Street", "city": "Leeds", "county": "West Yorkshire", "postcode": "LS1 1BB", "country": "UK"}',@now, @emptyGuid),
    ('100020', 'Oakwood', 'Oakwood Ltd', 'Oakwood', '{"line1": "Oakwood Tower", "line2": "High Street", "city": "Leeds", "county": "West Yorkshire", "postcode": "LS1 1BB", "country": "UK"}',@now, @emptyGuid);

    INSERT INTO dbo.Supplier(Id, [Name], LegalName, Summary, SupplierUrl, [Address], LastUpdated, LastUpdatedBy)
    VALUES
    (
        '99999',
        'NotEmis Health',
        'NotEgton Medical Information Systems',
        'We are the UK leader in connected healthcare software & services. Through innovative IT we help healthcare professionals access the information they need to provide better, faster and more cost effective patient care.

    Our clinical software is used in all major healthcare settings from GP surgeries to pharmacies, community, hospitals, and specialist services. By providing innovative, integrated solutions, we’re working to break the boundaries of system integration & interoperability.

    We also specialise in supplying IT infrastructure, software and engineering services, and through our technical support teams we have the skills and knowledge to enhance your IT systems.

    Patient (www.patient.info) is the UK’s leading health website. Designed to help patients play a key role in their own care, it provides access to clinically authored health information leaflets, videos, health check and assessment tools and patient forums.

    TRUNCATED FOR DEMO',
        'www.emishealth.com',
        '{"line1": "NotEmis Tower", "line2": "High Street", "city": "Leeds", "county": "West Yorkshire", "postcode": "LS1 1BB", "country": "UK"}',
        @now,
        @emptyGuid),
    (
        '99998',
        'NotTPP',
        'NotThe Phoenix Partnership',
        'TPP is a digital health company, committed to delivering world-class healthcare software around the world. Its EHR product, SystmOne, is used by over 7,000 NHS organisations in over 25 different care settings. This includes significant deployments in Acute Hospitals, Emergency Departments, Mental Health services, Social Care services and General Practice. In recent years, TPP has increased its international presence, with live deployments in China and across the Middle East.',
        'https://www.tpp-uk.com/',
        '{"line1": "NotTPP Tower", "line2": "High Street", "city": "Leeds", "county": "West Yorkshire", "postcode": "LS1 1BB", "country": "UK"}',
        @now,
        @emptyGuid);


    INSERT INTO [dbo].[SupplierContact] ([Id], [SupplierId],[FirstName],[LastName],[Email],[PhoneNumber],[LastUpdated],[LastUpdatedBy])
    VALUES
    (NEWID(),'100000', 'Tim', 'Timpson', 'tim@sharkscissors.net', '123456',@now, @emptyGuid),
    (NEWID(),'100001', 'Kimberly','Green', 'kimben@gmail.com', '(04065) 66339',@now, @emptyGuid),
    (NEWID(),'100002', 'Tracy', 'Lloyd', 'tracyloyd@edwards.com', '03490246609',@now, @emptyGuid),
    (NEWID(),'100003', 'Mason', 'Lewis', 'masowis@gmail.com', '(03829) 404796',@now, @emptyGuid),
    (NEWID(),'100004', 'Ashley', 'Murray', 'ashleray@Price.com', '05000 446126',@now, @emptyGuid),
    (NEWID(),'100005', 'Ethan', 'Mitchell', 'ethall@Knight.com', '03439008436',@now, @emptyGuid),
    (NEWID(),'100006', 'Sienna', 'Rogers', 'siennaers@chards.com', '+44(0)3336 91256',@now, @emptyGuid),
    (NEWID(),'100007', 'Nick', 'Walker', 'nickwaker@walkers.com', '0761 501 9180',@now, @emptyGuid),
    (NEWID(),'100008', 'Muhammad', 'Rogers', 'muhaers@gmail.com', '+44(0)5755 54895',@now, @emptyGuid),
    (NEWID(),'100009', 'Mohammed', 'Reid', 'mohameid@Owen-Hunt.com', '(0274) 450 7272',@now, @emptyGuid),
    (NEWID(),'100010', 'Becky', 'Cox', 'beckcox@beckcox.com', '0871 396 1160',@now, @emptyGuid),
    (NEWID(),'100011', 'Jennifer', 'Hunt', 'jennnt@gmail.com', '0003 380 3176',@now, @emptyGuid),
    (NEWID(),'100012', 'Oscar', 'Richards', 'oscarrrds@gollins.com', '+44(0)4319961720',@now, @emptyGuid),
    (NEWID(),'100013', 'Jeannie', 'Wilkins', 'pahmed@hospkerhy.cf',  '+44 1632 960277',@now, @emptyGuid),
    (NEWID(),'100014', 'Marisa', 'Fletcher', 'xiamkeherma@priv8dns.net', '+44 1632 960867',@now, @emptyGuid),
    (NEWID(),'100015', 'Myra', 'Gray', 'gcrronaldo280s@vilbarcpil.ga', '+44 1632 960405',@now, @emptyGuid),
    (NEWID(),'100016', 'Renato', 'Dodson', 'xpamela.gomes.pa2@traclecfa.ml', '+44 1632 960420',@now, @emptyGuid),
    (NEWID(),'100017', 'Angel', 'Waters', 'nael.altaic@central-teater.ru', '+44 1632 960376',@now, @emptyGuid),
    (NEWID(),'100018', 'Lindsay', 'Cooke', 'oblac@pquw0o.com', '+44 1632 960544',@now, @emptyGuid),
    (NEWID(),'100019', 'Xavier', 'Jacobson', 'esamuel.allvesof@morcagumd.gq', '+44 1632 960749',@now, @emptyGuid),
    (NEWID(),'100020', 'Mario', 'Long', 'noulmes14b@priv8dns.net', '+44 1632 960487',@now, @emptyGuid),
    (NEWID(),'99998', 'Aimee', 'Mcdonald', 'fpiyushpandey8115@traclecfa.ml', '+44 1632 960294',@now, @emptyGuid),
    (NEWID(),'99999', 'Sophia', 'Rojas', 'csheikha@sporbaydar.cf', '+44 1632 960131',@now, @emptyGuid);
END;
GO
