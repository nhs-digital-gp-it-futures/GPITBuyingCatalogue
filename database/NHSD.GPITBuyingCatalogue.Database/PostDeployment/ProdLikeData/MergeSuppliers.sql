IF (UPPER('$(INSERT_TEST_DATA)') = 'TRUE')
BEGIN
/*********************************************************************************************************************************************/
/* Supplier */
/*********************************************************************************************************************************************/

    CREATE TABLE #Supplier
    (
        Id nvarchar(6) NOT NULL,
        [Name] nvarchar(255) NOT NULL,
        LegalName nvarchar(255) NOT NULL,
        Summary nvarchar(1000) NULL,
        SupplierUrl nvarchar(1000) NULL,
        [Address] nvarchar(500) NULL,
        OdsCode nvarchar(8) NULL,
        CrmRef uniqueidentifier NULL,
        Deleted bit NOT NULL,
        LastUpdated datetime2(7) NOT NULL,
        LastUpdatedBy uniqueidentifier NOT NULL
    );

    INSERT INTO #Supplier (Id, [Name], LegalName, Summary, SupplierUrl, [Address], OdsCode, CrmRef, Deleted, LastUpdated, LastUpdatedBy)
         VALUES (N'10000', N'EMIS Health', N'EMIS Health', N'We’re the UK leader in connected healthcare software & services. Through innovative IT, we help healthcare professionals access the information they need to provide better, faster and more cost effective patient care.

Our clinical software is used in all major healthcare settings from GP surgeries to pharmacies, communities, hospitals, and specialist services. By providing innovative, integrated solutions, we’re working to break the boundaries of system integration & interoperability. 

We also specialise in supplying IT infrastructure, software and engineering services and, through our technical support teams, we have the skills and knowledge to enhance your IT systems.

Patient (www.patient.info) is the UK’s leading health website. Designed to help patients play a key role in their own care, it provides access to clinically authored health information leaflets, videos, health check and assessment tools and patient forums.', N'https://www.emishealth.com/', N'{"line1":"Rawdon House","line2":"Green Lane","town":"Yeadon","county":"West Yorkshire","postcode":"LS19 7BY","country":"England"}', NULL, NULL, 0, CAST(N'2020-04-07T19:46:44.1666667' AS datetime2), N'00000000-0000-0000-0000-000000000000');

    INSERT INTO #Supplier (Id, [Name], LegalName, Summary, SupplierUrl, [Address], OdsCode, CrmRef, Deleted, LastUpdated, LastUpdatedBy)
         VALUES (N'10001', N'Involve', N'Involve', NULL, NULL, N'{"line1":"Martin Dawes House","line2":"Europa Boulevard","line3":"Westbrook","town":"Warrington","county":"Cheshire","postcode":"WA5 7WH","country":"England"}', NULL, NULL, 0, CAST(N'2019-06-11T08:54:20.0000000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10002', N'G2 Speech', N'G2 Speech', NULL, NULL, N'{"line1":"4th Floor","line2":"Solar House","line3":"1-9 Romford Road","line4":"Stratford","town":"London","county":"Greater London","postcode":"E15 4LJ","country":"England"}', NULL, NULL, 0, CAST(N'2019-06-11T08:55:09.0000000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10003', N'myOrb', N'myOrb Ltd', NULL, NULL, N'{"line1":"Unit 67","line2":"Surrey Technology Centre","line3":"40 Occam Road","town":"Guildford","county":"Surrey","postcode":"GU2 7YG","country":"England"}', NULL, NULL, 0, CAST(N'2019-06-11T14:15:05.0000000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10004', N'Informatica Systems Ltd', N'Informatica Systems Ltd', N'Since 1992, Informatica has delivered innovative solutions that help primary care organisations deliver proactive care, reduce long term diseases and improve population health. Our range of intelligence systems collect healthcare data on patient cohorts, provide real time reporting dashboards for flexible data analysis and empower GPs to deliver care change. 

We have a long history as innovators, introducing the first solutions to market for 

1. touchscreen check-in systems;  

2. interactive prompts for QOF and clinical decision support; and  

3. comprehensive online patient facing services for appointment management.  

We are a fully accredited supplier with NHSD and are assured against the 20000, 22301 & 27001 ISO standards, DSPT, CyberEssentials+ and MHRA registered.  

Backed up by excellent customer service team and reputation we can help you to support primary care and improve patient outcomes.', N'https://www.informatica-systems.co.uk/', N'{"line1":"Aurora House","line2":"Deltic Avenue","line3":"Rooksley","town":"Milton Keyes","county":"Buckinghamshire","postcode":"MK13 8LW","country":"England"}', NULL, NULL, 0, CAST(N'2020-03-31T11:48:07.4800000' AS datetime2), N'00000000-0000-0000-0000-000000000000');

    INSERT INTO #Supplier (Id, [Name], LegalName, Summary, SupplierUrl, [Address], OdsCode, CrmRef, Deleted, LastUpdated, LastUpdatedBy)
         VALUES (N'10005', N'PharmData', N'PharmData Ltd', NULL, NULL, N'{"line1":"1 Kirkmanshulme Lane","town":"Manchester","county":"Lancashire","postcode":"M12 4NA","country":"England"}', NULL, NULL, 0, CAST(N'2019-06-14T13:14:40.0000000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10006', N'Medical Director', N'Medical Director', NULL, NULL, N'{"line1":"C-/Linklaters","line2":"One Silk Street","town":"London","county":"Greater London","postcode":"EC2Y 8HQ","country":"England"}', NULL, NULL, 0, CAST(N'2019-06-18T14:23:07.0000000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10007', N'DXS International PLC', N'DXS International PLC', N'DXS International Plc is a health information technology company that provides clinical decision support to more than 2,000 GP practices.

DXS’ core Clinical Decision Support solution, Best Pathway, provides UK primary care staff with treatment guidance and referral services helping to improve health care outcomes and reduce treatment costs.

It integrates with all Primary Care Clinical Systems and presents information to the clinician, relevant to the patient’s condition, via the workflow alerts.

The result is saving GP’s time, empowering Nurses and Pharmacists to take on increased responsibility of care and improving outcomes for the NHS and patients.', N'https://www.dxs-systems.co.uk/dxs-point-of-care.php#intro', N'{"line1":"119 St. Mary''s Road","town":"Market Harborough","county":"Leicestershire","postcode":"LE16 7DT","country":"England"}', NULL, NULL, 0, CAST(N'2020-03-25T12:12:24.0033333' AS datetime2), N'00000000-0000-0000-0000-000000000000');

    INSERT INTO #Supplier (Id, [Name], LegalName, Summary, SupplierUrl, [Address], OdsCode, CrmRef, Deleted, LastUpdated, LastUpdatedBy)
         VALUES (N'10008', N'LumiraDX Care Solutions UK Ltd', N'LumiraDX Care Solutions UK Ltd', NULL, NULL, N'{"line1":"Francis Clark LLP","line2":"Lowin House","line3":"Tregolls Road","town":"Truro","county":"Cornwall","postcode":"TR1 2NA","country":"England"}', NULL, NULL, 0, CAST(N'2019-06-20T07:57:23.0000000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10009', N'Care IS LTD', N'Care IS LTD', NULL, NULL, N'{"line1":"Biocity, Pennyfoot Street","town":"Nottingham","county":"Nottingham","postcode":"NG1 1GF","country":"England"}', NULL, NULL, 0, CAST(N'2019-06-21T14:27:44.0000000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10010', N'Excelicare', N'Excelicare', NULL, NULL, N'{"line1":"Axsys House","line2":"Marchburn Drive","line3":"Glasgow Airport Business Park","town":"Paisley","county":"","postcode":"PA3 2SJ","country":"Scotland"}', NULL, NULL, 0, CAST(N'2019-06-26T10:12:29.0000000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10011', N'DoctorLink', N'DoctorLink', NULL, NULL, N'{"line1":"Oakhill House","line2":"130 Tonbridge Road","town":"Hildenborough","county":"Kent","postcode":"TN11 9DZ","country":"England"}', NULL, NULL, 0, CAST(N'2019-06-26T16:46:52.0000000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10012', N'MedXnote', N'MedXnote', NULL, NULL, N'{"line1":"Digital Exchange Building","line2":"Crane Street","town":"Dublin","county":"","postcode":"D08 HKR9","country":"Ireland"}', NULL, NULL, 0, CAST(N'2019-06-26T19:48:39.0000000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10013', N'Medidata Exchange Limited', N'Medidata Exchange Limited', NULL, NULL, N'{"line1":"Ty Derw","line2":"Lime Tree Court","line3":"Cardiff Gate Business Park","town":"Cardiff","county":"South Glamorgan","postcode":"CF23 8AB","country":"Wales"}', NULL, NULL, 0, CAST(N'2019-06-27T07:01:49.0000000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10014', N'Total Billing Solutions', N'Total Billing Solutions', NULL, NULL, N'{"line1":"Beachside Business Centre","line2":"Rue Du Hocq","town":"St Clement","county":"","postcode":"JE2 6LF","country":"Jersey"}', NULL, NULL, 0, CAST(N'2019-06-27T07:30:45.0000000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10015', N'Optum Health Solutions UK Limited', N'Optum Health Solutions UK Limited', NULL, NULL, N'{"line1":"10th Floor","line2":"5 Merchant Square","town":"London","county":"Greater London","postcode":"W2 1AS","country":"England"}', NULL, NULL, 0, CAST(N'2019-06-27T09:43:11.0000000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10016', N'BigHand Limited', N'BigHand Limited', NULL, NULL, N'{"line1":"27-29 Union Street","town":"London","county":"","postcode":"SE1 1SD","country":"England"}', NULL, NULL, 0, CAST(N'2019-06-27T11:49:02.0000000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10017', N'Numed Healthcare', N'Numed Holdings Ltd', NULL, NULL, N'{"line1":"Alliance House","line2":"Roman Ridge Road","town":"Sheffield","county":"South Yorkshire","postcode":"S9 1GB","country":"England"}', NULL, NULL, 0, CAST(N'2019-06-27T11:51:36.0000000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10018', N'Sesui', N'Sesui Ltd', NULL, NULL, N'{"line1":"Magdalen Centre","line2":"1 Robert Robinson Avenue","town":"Oxford","county":"Oxfordshire","postcode":"OX4 4GA","country":"England"}', NULL, NULL, 0, CAST(N'2019-06-27T13:32:13.0000000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10019', N'Codegate Ltd', N'Codegate Ltd', NULL, NULL, N'{"line1":"39 Chapel Road","town":"Southampton","county":"","postcode":"SO30 3FG","country":"England"}', NULL, NULL, 0, CAST(N'2019-06-27T14:54:44.0000000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10020', N'MyMed Ltd', N'MyMed Ltd', N'MyMed Ltd, trading as Q doctor, are a market leading supplier of video consultation software and services. Founded by NHS clinicians', N'https://www.qdoctor.io/', N'{"line1":"Willowbrook","line2":"Burbridge Close","line3":"Lytchett Matravers","town":"Poole","county":"Dorset","postcode":"BH16 6EG","country":"England"}', NULL, NULL, 0, CAST(N'2020-04-07T19:21:45.3600000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10021', N'Black Pear Software Ltd', N'Black Pear Software Ltd', NULL, NULL, N'{"line1":"Bartlam House","line2":"Shrawley","town":"Worcester","county":"Worcestershire","postcode":"WR6 6TP","country":"England"}', NULL, NULL, 0, CAST(N'2019-06-27T18:38:31.0000000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10022', N'Gnosco AB', N'Gnosco AB', NULL, NULL, N'{"line1":"Kungsgatan 4","town":"Göteborg","county":"","postcode":"411 19","country":"Sweden"}', NULL, NULL, 0, CAST(N'2019-06-27T20:01:46.0000000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10023', N'Docobo Ltd', N'Docobo Ltd', NULL, NULL, N'{"line1":"21A High Street","town":"Bookham","county":"Surrey","postcode":"KT23 4AA","country":"England"}', NULL, NULL, 0, CAST(N'2019-07-01T06:13:23.0000000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10024', N'Servelec HSC', N'Servelec', NULL, NULL, N'{"line1":"The Straddle","line2":"Wharf Street","line3":"Victoria Quays","town":"Sheffield","county":"South Yorkshire","postcode":"S2 5SY","country":"England"}', NULL, NULL, 0, CAST(N'2019-07-01T10:56:20.0000000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10025', N'Clarity Informatics', N'Clarity Informatics', NULL, NULL, N'{"line1":"Deltic House","line2":"Kingfisher Way","line3":"Silverlink Business Park","line4":"Wallsend","town":"Newcastle on Tyne","county":"","postcode":"NE28 9NX","country":"England"}', NULL, NULL, 0, CAST(N'2019-07-01T10:58:07.0000000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10026', N'eConsult Health', N'eConsult Health', NULL, NULL, N'{"line1":"Nightingale House","line2":"46-48 East Street","town":"Epsom","county":"Surrey","postcode":"KT17 1HQ","country":"England"}', NULL, NULL, 0, CAST(N'2019-07-01T15:43:57.0000000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10027', N'Lantum', N'Lantum', NULL, NULL, N'{"line1":"4th Floor","line2":"15 Bonhill Street","town":"London","county":"Greater London","postcode":"EC2A 4DN","country":"England"}', NULL, NULL, 0, CAST(N'2019-07-01T16:54:37.0000000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10028', N'Clinical Architecture', N'Clinical Architecture', NULL, NULL, N'{"line1":"Suite 2","line2":"Okehampton Business Centre","line3":"Higher Stockley Mead","town":"Okehampton","county":"Devon","postcode":"EX20 1FJ","country":"England"}', NULL, NULL, 0, CAST(N'2019-07-02T10:55:03.0000000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10029', N'Targett Business Technology Limited', N'Targett Business Technology Limited', N'Targett Business Technology (TBT) is an innovative health and social care company that has developed RIVIAM as a secure platform for co-ordinating care. TBT has over 17 years of experience helping NHS and independent providers deliver services to improve healthcare outcomes.

Initially, we provided consulting expertise to establish the Healthcare Commission, CQC and worked for national bodies including NHS Employers and NICE. Through this work we gained an excellent understanding of health and social care regulatory, management and data systems. Since 2010, we have focused on providing services to national and local care provider organisations.

Over the past 5 years, we have invested in and launched our RIVIAM platform for enabling new models of care for health and social care services. In 2019 we launched the Secure Video Service.

Today, RIVIAM is contracted to provide services to NHS Trusts, GP Federations and independent organisations delivering NHS services.', N'http://www.riviam.com/', N'{"line1":"141 Englishcombe Lane","town":"Bath","county":"Bath & North East Somerset","postcode":"BA2 2EL","country":"England"}', NULL, NULL, 0, CAST(N'2020-04-08T10:03:33.9066667' AS datetime2), N'00000000-0000-0000-0000-000000000000');

    INSERT INTO #Supplier (Id, [Name], LegalName, Summary, SupplierUrl, [Address], OdsCode, CrmRef, Deleted, LastUpdated, LastUpdatedBy)
         VALUES (N'10030', N'AccuRx Limited', N'AccuRx Limited', N'AccuRx is on a mission to make patients healthier and the healthcare workforce happier. We’re doing that by building the platform to bring together all communication around a patient. Our vision is that anyone involved in a patient''s care can easily communicate with everyone else involved in that patient''s care, including the patient. AccuRx was founded in 2016 and has since been adopted by 6,500 GP practices (>95%) and over 150 NHS Trusts.', N'https://www.accurx.com/', N'{"line1":"47 Woodland Rise","town":"London","county":"","postcode":"N10 3UN","country":"England"}', NULL, NULL, 0, CAST(N'2020-06-11T13:33:17.8233333' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10031', N'The Martin Bell Partnership', N'The Martin Bell Partnership', N'We are an Independent consultancy, The Martin Bell Partnership, providing expert strategic & practical support for healthcare, healthcare IT & business support. 

Experienced at Board level in the NHS as a CIO, deputy Managing Director previously of one of the UK’s leading clinical systems providers; a wealth of experience from different sectors over many years.

Working with Trusted Associates, themselves steeped in the NHS, many at a senior level within clinical systems suppliers.

Strategic advice & guidance to practical support, clients include large corporates, investment houses, start-ups & healthcare providers.

We have been working with GP&Me since early in 2019 getting this solution onto GP IT Futures. GP&Me, created by two practising GPs, offers easy video consultation facilities. The Martin Bell Partnership, provides the support, sales, marketing & deployment expertise, from years of experience across our Associates in primary care, & many other health care settings.', N'', N'{"line1":"The Old Court House","line2":"Back Street","town":"Aldborough","county":"North Yorkshire","postcode":"YO51 9EX","country":"England"}', NULL, NULL, 0, CAST(N'2020-04-01T12:48:09.8933333' AS datetime2), N'00000000-0000-0000-0000-000000000000');

    INSERT INTO #Supplier (Id, [Name], LegalName, Summary, SupplierUrl, [Address], OdsCode, CrmRef, Deleted, LastUpdated, LastUpdatedBy)
         VALUES (N'10032', N'Elemental Software', N'In Your Elemental Ltd', NULL, NULL, N'{"line1":"1st Floor","line2":"Progressive House","line3":"25-31 The Diamond","town":"Derry","county":"Northern Ireland","postcode":"BT48 6HP","country":"Northern Ireland"}', NULL, NULL, 0, CAST(N'2019-07-03T08:39:46.0000000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10033', N'AllDayDr Group Ltd', N'AllDayDr Group Ltd', NULL, NULL, N'{"line1":"7 Wendover Drive","line2":"Ladybridge","town":"Bolton","county":"Lancashire","postcode":"BL3 4RX","country":"England"}', NULL, NULL, 0, CAST(N'2019-07-03T12:07:00.0000000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10034', N'Vision Healthcare Limited', N'Vision Healthcare Limited', NULL, NULL, N'{"line1":"The Bread Factory","line2":"1a Broughton Street","town":"London","county":"","postcode":"SW8 3QJ","country":"England"}', NULL, NULL, 0, CAST(N'2019-07-03T13:04:47.0000000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10035', N'Evergreen Life', N'Evergreen Life', N'Wellness app and provider of GP services, Evergreen Life lets people own their health information, driving informed healthcare and giving people the best chance of staying well. 

Collaborating with NHS England, NHS Digital and 3 major GP systems, Evergreen Life has facilitated mutually efficient interaction between practices and patients through online prescription ordering and appointment booking.

The app platform allows all patients in England to keep a copy of their GP record whenever and wherever they need it. Users can add their own information not held in the GP record, including allergies, conditions and medications, building a complete, accurate record that they can share with anyone they wish.

Providing access to up-to-date health information, our person-led solution empowers people to feel more in control to make self-care decisions and manage their care independently, whilst delivering a platform to access primary care services if they need it.', N'https://www.evergreen-life.co.uk/', N'{"line1":"The Edge Business Centre","line2":"The Clowes Street","town":"Manchester","county":"Greater Manchester","postcode":"M3 5NA","country":"England"}', NULL, NULL, 0, CAST(N'2020-04-01T12:08:32.4233333' AS datetime2), N'00000000-0000-0000-0000-000000000000');

    INSERT INTO #Supplier (Id, [Name], LegalName, Summary, SupplierUrl, [Address], OdsCode, CrmRef, Deleted, LastUpdated, LastUpdatedBy)
         VALUES (N'10036', N'Elsevier Limited', N'Elsevier BV', NULL, NULL, N'{"line1":"Radarweg 29","town":"Amsterdam","county":"Noord-Holland","postcode":"1043NX","country":"Netherlands"}', NULL, NULL, 0, CAST(N'2019-07-03T17:04:49.0000000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10037', N'DictateIT', N'DictateIT', NULL, NULL, N'{"line1":"Aurora House, Deltic Avenue","line2":"Rooksley","town":"Milton Keynes","county":"Buckinghamshire","postcode":"MK13 8LW","country":"England"}', NULL, NULL, 0, CAST(N'2019-07-04T11:58:29.0000000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10038', N'Edenbridge', N'Edenbridge Healthcare Limited', NULL, NULL, N'{"line1":"1 Mariner Court","line2":"Calder Business Park","town":"Wakefield","county":"West Yorkshire","postcode":"WF4 3FL","country":"England"}', NULL, NULL, 0, CAST(N'2019-07-04T12:17:17.0000000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10039', N'Doc Abode Ltd', N'Doc Abode Ltd', NULL, NULL, N'{"line1":"Suite 12 Jason House Kerry Hill","town":"Leeds","county":"West Yorkshire","postcode":"LS18 4JR","country":"England"}', NULL, NULL, 0, CAST(N'2019-07-04T12:17:20.0000000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10040', N'Swiftqueue', N'Swiftqueue', NULL, NULL, N'{"line1":"Eolas Building","line2":"Maynooth University North Campus","town":"Maynooth","county":"county Kildare","postcode":"","country":"Ireland"}', NULL, NULL, 0, CAST(N'2019-07-04T13:48:16.0000000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10041', N'Quality Medical Solutions (QMS)', N'Quality Medical Solutions', NULL, NULL, N'{"line1":"23 Hinton Rd","town":"Bournemouth","county":"Dorset","postcode":"BH1 2EF","country":"England"}', NULL, NULL, 0, CAST(N'2019-07-04T13:51:36.0000000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10042', N'Engage Health Systems Limited', N'Engage Health Systems Limited', NULL, NULL, N'{"line1":"1a, St Nicholas Court","town":"North Walsham","county":"Norfolk","postcode":"NR28 9BY","country":"England"}', NULL, NULL, 0, CAST(N'2019-07-04T14:00:05.0000000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10043', N'IBS Software', N'IBS, Inc.', NULL, NULL, N'{"line1":"Unit 18685","line2":"13 Freeland Park","line3":"Wareham Road","town":"Poole","county":"","postcode":"BH16 6FA","country":"England"}', NULL, NULL, 0, CAST(N'2019-07-04T14:38:44.0000000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10044', N'Answer Digital Ltd', N'Answer Digital Ltd', NULL, NULL, N'{"line1":"Union Mills","line2":"9 Dewsbury Road","town":"Leeds","county":"West Yorkshire","postcode":"LS11 5DD","country":"England"}', NULL, NULL, 0, CAST(N'2019-07-04T14:43:46.0000000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10045', N'Meddbase', N'Medical Management Systems', NULL, NULL, N'{"line1":"140 Buckingham Palace Road","town":"London","county":"Greater London","postcode":"SW1 9SA","country":"England"}', NULL, NULL, 0, CAST(N'2019-07-04T14:44:48.0000000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10046', N'Advanced Health and Care Limited', N'Advanced Health and Care Limited', N'Advanced is one of the UK’s largest providers of business software and services with 19,000+ customers and 2,350+ employees. We provide enterprise and market-focused solutions that allow our customers to reimagine what is possible, innovate in their sectors and improve the lives of millions of people in the UK.

By continually investing in our people, partnerships and technologies, we provide right-first-time solutions that evolve with the changing needs of our customers and the markets they operate in. Our strategy is to enable our customers to drive efficiencies, make informed decisions, act with pace and meet challenges head on.

True partnership is what differentiates us from our competition. We deliver focused solutions for health & care organisations that simplify complex challenges and deliver immediate value.

Advanced solutions help care for 65 million patients in the UK, send millions of clinical documents and support over 80% of the NHS 111 service.', N'https://www.oneadvanced.com/', N'{"line1":"Ditton Park","line2":"Riding Court Road","town":"Datchet","county":"Berkshire","postcode":"SL3 9LL","country":"England"}', NULL, NULL, 0, CAST(N'2020-03-31T14:06:17.6333333' AS datetime2), N'00000000-0000-0000-0000-000000000000');

    INSERT INTO #Supplier (Id, [Name], LegalName, Summary, SupplierUrl, [Address], OdsCode, CrmRef, Deleted, LastUpdated, LastUpdatedBy)
         VALUES (N'10047', N'GP Access', N'GP Access Ltd', N'', N'https://askmygp.uk/about/', N'{"line1":"The Manor House","line2":"70 Main Street","town":"Cossington","county":"Leicestershire","postcode":"LE7 4UW","country":"England"}', NULL, NULL, 0, CAST(N'2020-04-01T13:05:48.0266667' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10048', N'Priority Digital Health Limited', N'Priority Digital Health Limited', NULL, NULL, N'{"line1":"Denny Lodge Business Park","line2":"Ely Road","line3":"Chittering","town":"Cambridge","county":"Cambridgeshire","postcode":"CB25 9PH","country":""}', NULL, NULL, 0, CAST(N'2019-07-04T16:19:39.0000000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10049', N'Medical Data Solutions and Services', N'Medical Data Solutions and Services', NULL, NULL, N'{"line1":"74 Dickenson Road","line2":"Rusholme","town":"Manchester","county":"Greater Manchester","postcode":"M14 5HF","country":"England"}', NULL, NULL, 0, CAST(N'2019-07-04T16:43:17.0000000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10050', N'Philips', N'Philips Electronics UK Limited', NULL, NULL, N'{"line1":"Philips Centre","line2":"Unit 3","line3":"Guildford Business Park","town":"Guildford","county":"Surrey","postcode":"GU2 8XG","country":"England"}', NULL, NULL, 0, CAST(N'2019-07-04T18:14:27.0000000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10051', N'Integrated Care 24 Ltd (IC24)', N'Integrated Care 24 Ltd (IC24)', NULL, NULL, N'{"line1":"Kingston House","line2":"The Long Barrow","line3":"Orbital Park","town":"Ashford","county":"Kent","postcode":"TN24 0GP","country":"England"}', NULL, NULL, 0, CAST(N'2019-07-05T07:00:03.0000000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10052', N'TPP – The Phoenix Partnership (Leeds) Ltd', N'TPP-UK - The Phoenix Partnership Ltd', N'TPP is a digital health company, committed to delivering world-class healthcare software around the world. 

Its EHR product, SystmOne, is used by over 7,000 NHS organisations in over 25 different care settings. This includes significant deployments in Acute Hospitals, Emergency Departments, Mental Health services, Social Care services and General Practice. 

In recent years, TPP has increased its international presence, with live deployments in China and across the Middle East.', N'https://www.tpp-uk.com/', N'{"line1":"TPP House","line2":"Horsforth","town":"Leeds","county":"West Yorkshire","postcode":"LS18 5PX","country":"England"}', NULL, NULL, 0, CAST(N'2020-05-27T06:26:48.5033333' AS datetime2), N'00000000-0000-0000-0000-000000000000');

    INSERT INTO #Supplier (Id, [Name], LegalName, Summary, SupplierUrl, [Address], OdsCode, CrmRef, Deleted, LastUpdated, LastUpdatedBy)
         VALUES (N'10053', N'Arissian', N'Arissian Ltd', NULL, NULL, N'{"line1":"Basepoint Centre","line2":"Bromsgrove Technology Park","line3":"Isidore Road","town":"Bromsgrove","county":"Worcestershire","postcode":"B60 3ET","country":"England"}', NULL, NULL, 0, CAST(N'2019-07-05T14:34:54.0000000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10054', N'Health Intelligence Ltd', N'Health Intelligence Ltd', NULL, NULL, N'{"line1":"Beechwood Hall","line2":"Kingsmead Road","town":"High Wycombe","county":"","postcode":"HP11 1JL","country":"England"}', NULL, NULL, 0, CAST(N'2019-07-10T13:37:46.0000000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10055', N'Accenda Limited', N'Accenda Limited', NULL, NULL, N'{"line1":"Suite 322","line2":"3rd Floor Broadstone Mill","line3":"Broadstone Road","town":"Stockport","county":"Cheshire","postcode":"SK5 7DL","country":"England"}', NULL, NULL, 0, CAST(N'2019-07-11T10:07:20.0000000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10056', N'Metabolic Healthcare Ltd', N'Metabolic Healthcare Ltd', NULL, NULL, N'{"line1":"1 Westpoint Trading Estate","line2":"Alliance Road","line3":"Ealing","town":"London","county":"","postcode":"W3 0RA","country":"England"}', NULL, NULL, 0, CAST(N'2019-07-15T11:55:36.0000000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10057', N'Redwood Technologies Group Limited', N'Redwood Technologies Group Limited', NULL, NULL, N'{"line1":"Radius Court","line2":"Eastern Road","town":"Bracknell","county":"Berkshire","postcode":"RG12 2UP","country":"England"}', NULL, NULL, 0, CAST(N'2019-07-15T15:44:31.0000000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10058', N'Niche Health', N'Niche Health', NULL, NULL, N'{"line1":"Beasleys Farm","line2":"Upper Gambolds Lane","town":"Bromsgrove","county":"Worcestershire","postcode":"B60 3EZ","country":"England"}', NULL, NULL, 0, CAST(N'2019-07-16T07:13:10.0000000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10059', N'Prescribing Services', N'Prescribing Services Limited', NULL, NULL, N'{"line1":"2 Regis Place","line2":"North Lynn Industrial Estate","town":"King''s Lynn","county":"Norfolk","postcode":"PE30 2JN","country":"England"}', NULL, NULL, 0, CAST(N'2019-07-17T09:24:52.0000000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10060', N'Locum''s Nest', N'Locum''s Nest', NULL, NULL, N'{"line1":"12 Hammersmith Grove","line2":"Hammersmith Fulham","town":"London","county":"","postcode":"W6 7AP","country":"England"}', NULL, NULL, 0, CAST(N'2019-07-17T10:50:45.0000000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10061', N'Microtest Ltd', N'Microtest Ltd', NULL, NULL, N'{"line1":"16-18 Normandy Way","town":"Bodmin","county":"","postcode":"PL31 1EX","country":"England"}', NULL, NULL, 0, CAST(N'2019-07-18T14:47:34.0000000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10062', N'Silicon Practice', N'Silicon Practice', N'We have rolled out over 540 FootFall sites in the UK across a variety of demographics and different practice structures. Working closely with Practices and CCG’s, we have evolved strategies, processes and video tutorials to ensure FootFall is successfully rolled-out in each area.

We have a strong track record of developing the product with CCGs. We have recently made extensive changes to the product with Norfolk and Waveney STP, which have enriched and extended our focus on the digital triage features of FootFall.

We develop a joint project management approach with the CCG, for example arranging Project Initiation Workshops, weekly project management WebEx meetings, pre-mobilisation and post mobilisation practice communication together with reporting requirements.', N'', N'{"line1":"Westbury Court","line2":"Church Road","town":"Westbury On Trym","county":"Gloucester","postcode":"BS9 3EF","country":"England"}', NULL, NULL, 0, CAST(N'2020-04-07T09:44:04.4600000' AS datetime2), N'00000000-0000-0000-0000-000000000000');

    INSERT INTO #Supplier (Id, [Name], LegalName, Summary, SupplierUrl, [Address], OdsCode, CrmRef, Deleted, LastUpdated, LastUpdatedBy)
         VALUES (N'10063', N'Aire Logic', N'Aire Logic', NULL, NULL, N'{"line1":"Newlaithes Manor","line2":"Newlaithes Road","town":"Leeds","county":"West Yorkshire","postcode":"LS18 4LG","country":"England"}', NULL, NULL, 0, CAST(N'2019-07-23T16:27:52.0000000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10064', N'Medloop', N'Medloop', NULL, NULL, N'{"line1":"St James House","line2":"13 Kensington Square","town":"London","county":"Greater London","postcode":"W8 5HD","country":"England"}', NULL, NULL, 0, CAST(N'2019-07-24T08:42:08.0000000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10065', N'Sensely', N'Sensely', NULL, NULL, N'{"line1":"Pound House","line2":"62a Highgate High Street","town":"London","county":"Greater London","postcode":"N6 5HX","country":"England"}', NULL, NULL, 0, CAST(N'2019-07-25T14:54:59.0000000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10066', N'Primary Care IT Ltd', N'Primary Care IT Ltd', NULL, NULL, N'{"line1":"GF6 Trumpeter House","line2":"Trumpeter Rise","town":"Long Stratton","county":"Norfolk","postcode":"NR15 2DY","country":"England"}', NULL, NULL, 0, CAST(N'2019-07-26T16:40:07.0000000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10067', N'MEDITECH GROUP LIMITED', N'MEDITECH GROUP LIMITED', NULL, NULL, N'{"line1":"1 Northumberland Avenue","town":"London","county":"Greater London","postcode":"WC2N 5BW","country":"USA"}', NULL, NULL, 0, CAST(N'2019-07-26T20:54:06.0000000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10068', N'Substrakt Health', N'Substrakt Health', NULL, NULL, N'{"line1":"2a Victoria Works","line2":"Vittoria St","town":"Birmingham","county":"West Midlands","postcode":"B1 3PE","country":"England"}', NULL, NULL, 0, CAST(N'2019-07-29T08:26:17.0000000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10069', N'Apollo Medical Software Solutions Limited', N'Apollo Medical Software Solutions Limited', NULL, NULL, N'{"line1":"12 Mansfield Centre Office Suite 0:1","line2":"Hamilton Way","line3":"Oakham Business Park","town":"Mansfield","county":"","postcode":"NG18 5FB","country":"England"}', NULL, NULL, 0, CAST(N'2019-07-30T16:39:19.0000000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10070', N'eSynergy Solutions Limited', N'eSynergy Solutions Limited', NULL, NULL, N'{"line1":"50 Fenchurch Street","town":"London","county":"","postcode":"EC3M 3JY","country":"England"}', NULL, NULL, 0, CAST(N'2019-07-31T14:15:00.0000000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10071', N'Connected Tech Group', N'Connected Tech Group', NULL, NULL, N'{"line1":"22, Mount Ephraim","town":"Tunbridge Wells","county":"Kent","postcode":"TN4 8AS","country":"England"}', NULL, NULL, 0, CAST(N'2019-08-01T14:36:00.0000000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10072', N'Push Dr Limited', N'Push Dr Limited', NULL, NULL, N'{"line1":"Arkwright House","line2":"Parsonage","town":"Manchester","county":"Cheshire","postcode":"M3 2LF","country":"England"}', NULL, NULL, 0, CAST(N'2019-08-02T05:22:07.0000000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (N'10073', N'iPLATO', N'iPlato Healthcare Ltd', N'iPLATO has been operating mobile patient services in primary care since 2005. In this time we have launched the iPLATO service with over 1,800 GP practices in 75 CCG/PCTs reaching over 25 million patients.

iPLATO is the digital bridge between any of the principal GP IT systems used by the GP network and their patients using one of the most ubiquitous of modern tools – their own mobile phone.

•	Over 90% of adults in Britain own (at least one) mobile phone.
•	Personal, private and immediate – ideal for health communication.
•	Preferred by many patients, particularly the ‘hard-to-reach’.

More than 25 million patients in the UK already receive messages from their GP or Healthcare provider through iPLATO Patient Care Messaging for Primary Care system.', N'', N'{"line1":"1 King Street","town":"London","county":"Greater London","postcode":"W6 9HR","country":"England"}', NULL, NULL, 0, CAST(N'2020-04-01T13:48:54.1466667' AS datetime2), N'00000000-0000-0000-0000-000000000000');

MERGE INTO dbo.Supplier AS TARGET
USING #Supplier AS SOURCE
ON TARGET.Id = SOURCE.Id 
WHEN MATCHED THEN  
       UPDATE SET TARGET.[Name] = SOURCE.[Name],
                  TARGET.LegalName = SOURCE.LegalName,
                  TARGET.Summary = SOURCE.Summary,
                  TARGET.SupplierUrl = SOURCE.SupplierUrl,
                  TARGET.[Address] = SOURCE.[Address],
                  TARGET.OdsCode = SOURCE.OdsCode,
                  TARGET.CrmRef = SOURCE.CrmRef,
                  TARGET.Deleted = SOURCE.Deleted,
                  TARGET.LastUpdated = SOURCE.LastUpdated,
                  TARGET.LastUpdatedBy = SOURCE.LastUpdatedBy
WHEN NOT MATCHED BY TARGET THEN  
        INSERT (Id, [Name], LegalName, Summary, SupplierUrl, [Address], OdsCode, CrmRef, Deleted, LastUpdated, LastUpdatedBy)
        VALUES (SOURCE.Id, SOURCE.[Name], SOURCE.LegalName, SOURCE.Summary, SOURCE.SupplierUrl, SOURCE.[Address], SOURCE.OdsCode, SOURCE.CrmRef, SOURCE.Deleted, SOURCE.LastUpdated, SOURCE.LastUpdatedBy);
END;
GO
