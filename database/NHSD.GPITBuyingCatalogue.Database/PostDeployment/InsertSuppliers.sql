DECLARE @bobEmail AS nvarchar(50) = N'BobSmith@email.com';
DECLARE @bobUser AS int = (SELECT Id FROM users.AspNetUsers WHERE Email = @bobEmail);
DECLARE @now AS datetime = GETUTCDATE();

IF UPPER('$(INSERT_TEST_DATA)') = 'TRUE' AND NOT EXISTS (SELECT * FROM catalogue.Suppliers)
BEGIN
    INSERT INTO catalogue.Suppliers(Id, [Name], LegalName, Summary, [Address], LastUpdated, LastUpdatedBy, IsActive)
    VALUES (
        100000,
        'Really Kool Corporation',
        'Really Kool Corporation',
        'Really Kool Corporation is a fictious UK based IT company but that''s not going to stop us making Really Kool products!',
        '{"line1": "The Tower", "line2": "High Street", "city": "Leeds", "county": "West Yorkshire", "postcode": "LS1 1BB", "country": "UK"}',
        @now,
        @bobUser,
        1);

    INSERT INTO catalogue.Suppliers(Id, [Name], LegalName, Summary, [Address], LastUpdated, LastUpdatedBy, IsActive)
    VALUES
    (100001, 'Remedical Software', 'Remedical Limited', 'The Remedical Software', '{"line1": "Remedical Software Tower", "line2": "High Street", "city": "Leeds", "county": "West Yorkshire", "postcode": "LS1 1BB", "country": "UK"}', @now, @bobUser, 1),
    (100002, 'CareShare', 'CareShare Limited', 'The CareShare', '{"line1": "CareShare Tower", "line2": "High Street", "city": "Leeds", "county": "West Yorkshire", "postcode": "LS1 1BB", "country": "UK"}', @now, @bobUser, 1),
    (100003, 'Avatar Solutions', 'Avatar Solutions Plc', 'Avatar Solutions', '{"line1": "Avatar Tower", "line2": "High Street", "city": "Leeds", "county": "West Yorkshire", "postcode": "LS1 1BB", "country": "UK"}', @now, @bobUser, 1),
    (100004, 'Catterpillar Medworks', 'Catterpillar Medworks Ltd', 'Catterpillar Medworks', '{"line1": "Medworks Tower", "line2": "High Street", "city": "Leeds", "county": "West Yorkshire", "postcode": "LS1 1BB", "country": "UK"}', @now, @bobUser, 1),
    (100005, 'Curtis Systems', 'Curtis Systems Ltd', 'Curtis Systems', '{"line1": "Curtis Tower", "line2": "High Street", "city": "Leeds", "county": "West Yorkshire", "postcode": "LS1 1BB", "country": "UK"}', @now, @bobUser, 1),
    (100006, 'Clinical Raptor', 'Clinical Raptor Ltd', 'Clinical Raptor', '{"line1": "Raptor Tower", "line2": "High Street", "city": "Leeds", "county": "West Yorkshire", "postcode": "LS1 1BB", "country": "UK"}', @now, @bobUser, 1),
    (100007, 'Doc Lightning', 'Doc Lightning Ltd', 'Doc Lightning', '{"line1": "Doc Lightning Tower", "line2": "High Street", "city": "Leeds", "county": "West Yorkshire", "postcode": "LS1 1BB", "country": "UK"}', @now, @bobUser, 1),
    (100008, 'Docability Software', 'Docability Ltd', 'Docability Software', '{"line1": "Docability Tower", "line2": "High Street", "city": "Leeds", "county": "West Yorkshire", "postcode": "LS1 1BB", "country": "UK"}', @now, @bobUser, 1),
    (100009, 'Empire Softworks',  'Empire Softworks Plc', 'Empire Softworks', '{"line1": "Empire Tower", "line2": "High Street", "city": "Leeds", "county": "West Yorkshire", "postcode": "LS1 1BB", "country": "UK"}', @now, @bobUser, 1),
    (100010, 'Cure Forward', 'Cure Forward Ltd', 'Cure Forward', '{"line1": "Cure Forward Tower", "line2": "High Street", "city": "Leeds", "county": "West Yorkshire", "postcode": "LS1 1BB", "country": "UK"}', @now, @bobUser, 1),
    (100011, 'Hansa Healthcare', 'Hansa Healthcare Plc', 'Hansa Healthcare', '{"line1": "Hansa Tower", "line2": "High Street", "city": "Leeds", "county": "West Yorkshire", "postcode": "LS1 1BB", "country": "UK"}', @now, @bobUser, 1),
    (100012, 'Moonlight Intercare', 'Moonlight Intercare', 'Moonlight Intercare', '{"line1": "Moonlight Tower", "line2": "High Street", "city": "Leeds", "county": "West Yorkshire", "postcode": "LS1 1BB", "country": "UK"}', @now, @bobUser, 1),
    (100013, 'eHealth Development', 'eHealth Development', 'eHealth Development', '{"line1": "eHealth Tower", "line2": "High Street", "city": "Leeds", "county": "West Yorkshire", "postcode": "LS1 1BB", "country": "UK"}', @now, @bobUser, 1),
    (100014, 'Dr. Nick', 'Dr. Nick', 'Dr. Nick', '{"line1": "Simpson Tower", "line2": "High Street", "city": "Leeds", "county": "West Yorkshire", "postcode": "LS1 1BB", "country": "UK"}', @now, @bobUser, 1),
    (100015, 'Testproof Technology',  'Testproof Technology', 'Testproof Technology', '{"line1": "Testproof Tower", "line2": "High Street", "city": "Leeds", "county": "West Yorkshire", "postcode": "LS1 1BB", "country": "UK"}', @now, @bobUser, 1),
    (100016, 'Hojo Health', 'Hojo Health Ltd', 'Hojo Health', '{"line1": "Hojo Tower", "line2": "High Street", "city": "Leeds", "county": "West Yorkshire", "postcode": "LS1 1BB", "country": "UK"}', @now, @bobUser, 1),
    (100017, 'Jericho Healthcare', 'Jericho Ltd', 'Jericho Healthcare', '{"line1": "Jericho Tower", "line2": "High Street", "city": "Leeds", "county": "West Yorkshire", "postcode": "LS1 1BB", "country": "UK"}', @now, @bobUser, 1),
    (100018, 'Mana Systems', 'Mana Systems', 'Mana Systems', '{"line1": "Mana Tower", "line2": "High Street", "city": "Leeds", "county": "West Yorkshire", "postcode": "LS1 1BB", "country": "UK"}', @now, @bobUser, 1),
    (100019, 'Sunhealth Nanosystems', 'Sunhealth Nanosystems', 'Sunhealth Nanosystems', '{"line1": "Sunhealth Tower", "line2": "High Street", "city": "Leeds", "county": "West Yorkshire", "postcode": "LS1 1BB", "country": "UK"}', @now, @bobUser, 1),
    (100020, 'Oakwood', 'Oakwood Ltd', 'Oakwood', '{"line1": "Oakwood Tower", "line2": "High Street", "city": "Leeds", "county": "West Yorkshire", "postcode": "LS1 1BB", "country": "UK"}', @now, @bobUser, 1);

    INSERT INTO catalogue.Suppliers(Id, [Name], LegalName, Summary, SupplierUrl, [Address], LastUpdated, LastUpdatedBy, IsActive)
    VALUES
    (
        99999,
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
        @bobUser,
        1),
    (
        99998,
        'NotTPP',
        'NotThe Phoenix Partnership',
        'TPP is a digital health company, committed to delivering world-class healthcare software around the world. Its EHR product, SystmOne, is used by over 7,000 NHS organisations in over 25 different care settings. This includes significant deployments in Acute Hospitals, Emergency Departments, Mental Health services, Social Care services and General Practice. In recent years, TPP has increased its international presence, with live deployments in China and across the Middle East.',
        'https://www.tpp-uk.com/',
        '{"line1": "NotTPP Tower", "line2": "High Street", "city": "Leeds", "county": "West Yorkshire", "postcode": "LS1 1BB", "country": "UK"}',
        @now,
        @bobUser,
        1);


    INSERT INTO catalogue.SupplierContacts (SupplierId, FirstName, LastName, Email, PhoneNumber, LastUpdated, LastUpdatedBy)
    VALUES
        (100000, 'Aarav',    'Patel',    'aaravpatel@test.com',       '01234 567891',      @now, @bobUser),
        (100001, 'Aisha',    'Khan',     'aishakhan@test.com',        '01234 567891',      @now, @bobUser),
        (100002, 'Liam',     'Connor',   'liamconnor@test.com',       '01234 567891',      @now, @bobUser),
        (100003, 'Chloe',    'Nguyen',   'chloenguyen@test.com',      '01234 567891',      @now, @bobUser),
        (100004, 'Mohammed', 'Ali',      'mohammedali@test.com',      '01234 567891',      @now, @bobUser),
        (100005, 'Sophie',   'Williams', 'sophiewilliams@test.com',   '01234 567891',      @now, @bobUser),
        (100006, 'Ethan',    'Smith',    'ethansmith@test.com',       '01234 567891',      @now, @bobUser),
        (100007, 'Zara',     'Hussain',  'zarahussain@test.com',      '01234 567891',      @now, @bobUser),
        (100008, 'Lucas',    'Brown',    'lucasbrown@test.com',       '+44(0)1234 567891', @now, @bobUser),
        (100009, 'Maya',     'Davies',   'mayadavies@test.com',       '01234 567891',      @now, @bobUser),
        (100010, 'Oscar',    'Wilson',   'oscarwilson@test.com',      '01234 567891',      @now, @bobUser),
        (100011, 'Lily',     'Wright',   'lilywright@test.com',       '01234 567891',      @now, @bobUser),
        (100012, 'James',    'Green',    'jamesgreen@test.com',       '+44(0)1234 567891', @now, @bobUser),
        (100013, 'Grace',    'Hall',     'gracehall@test.com',        '+44 1234 567891',   @now, @bobUser),
        (100014, 'Henry',    'Adams',    'henryadams@test.com',       '+44 1234 567891',   @now, @bobUser),
        (100015, 'Ella',     'Baker',    'ellabaker@test.com',        '+44 1234 567891',   @now, @bobUser),
        (100016, 'Freddie',  'Harris',   'freddieharris@test.com',    '+44 1234 567891',   @now, @bobUser),
        (100017, 'Evie',     'Martin',   'eviemartin@test.com',       '+44 1234 567891',   @now, @bobUser),
        (100018, 'Leo',      'Thompson', 'leothompson@test.com',      '+44 1234 567891',   @now, @bobUser),
        (100019, 'Poppy',    'White',    'poppywhite@test.com',       '+44 1234 567891',   @now, @bobUser),
        (100020, 'Archie',   'Lewis',    'archielewis@test.com',      '+44 1234 567891',   @now, @bobUser),
        (99998,  'Isabella', 'Clark',    'isabellaclark@test.com',    '+44 1234 567891',   @now, @bobUser),
        (99999,  'Jacob',    'Turner',   'jacobturner@test.com',      '+44 1234 567891',   @now, @bobUser);
END;
GO
