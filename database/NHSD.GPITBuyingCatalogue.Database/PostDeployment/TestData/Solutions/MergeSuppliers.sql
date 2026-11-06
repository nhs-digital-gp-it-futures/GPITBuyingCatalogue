IF UPPER('$(INSERT_TEST_DATA)') = 'TRUE'
BEGIN
    MERGE INTO [catalogue].[Suppliers] AS TARGET
    USING (
    VALUES
        (10000, N'EMIS Health', N'EMIS Health', N'We’re the UK leader in connected healthcare software & services. Through innovative IT, we help healthcare professionals access the information they need to provide better, faster and more cost effective patient care.

Our clinical software is used in all major healthcare settings from GP surgeries to pharmacies, communities, hospitals, and specialist services. By providing innovative, integrated solutions, we’re working to break the boundaries of system integration & interoperability. 

We also specialise in supplying IT infrastructure, software and engineering services and, through our technical support teams, we have the skills and knowledge to enhance your IT systems.

Patient (www.patient.info) is the UK’s leading health website. Designed to help patients play a key role in their own care, it provides access to clinically authored health information leaflets, videos, health check and assessment tools and patient forums.', N'https://www.emishealth.com/', N'{"line1":"Rawdon House","line2":"Green Lane","town":"Yeadon","county":"West Yorkshire","postcode":"LS19 7BY","country":"England"}', 0, CAST(N'2020-04-07T19:46:44.1666667' AS DateTime2), 2, 1),
        (10001, N'Involve', N'Involve', NULL, NULL, N'{"line1":"Martin Dawes House","line2":"Europa Boulevard","line3":"Westbrook","town":"Warrington","county":"Cheshire","postcode":"WA5 7WH","country":"England"}', 0, CAST(N'2019-06-11T08:54:20.0000000' AS DateTime2), 2, 1),
        (10002, N'G2 Speech', N'G2 Speech', NULL, NULL, N'{"line1":"4th Floor","line2":"Solar House","line3":"1-9 Romford Road","line4":"Stratford","town":"London","county":"Greater London","postcode":"E15 4LJ","country":"England"}', 0, CAST(N'2019-06-11T08:55:09.0000000' AS DateTime2), 2, 1),
        (10003, N'myOrb', N'myOrb Ltd', NULL, NULL, N'{"line1":"Unit 67","line2":"Surrey Technology Centre","line3":"40 Occam Road","town":"Guildford","county":"Surrey","postcode":"GU2 7YG","country":"England"}', 0, CAST(N'2019-06-11T14:15:05.0000000' AS DateTime2), 2, 1),
        (10004, N'Informatica Systems Ltd', N'Informatica Systems Ltd', N'Since 1992, Informatica has delivered innovative solutions that help primary care organisations deliver proactive care, reduce long term diseases and improve population health. Our range of intelligence systems collect healthcare data on patient cohorts, provide real time reporting dashboards for flexible data analysis and empower GPs to deliver care change. 

We have a long history as innovators, introducing the first solutions to market for 

1. touchscreen check-in systems;  

2. interactive prompts for QOF and clinical decision support; and  

3. comprehensive online patient facing services for appointment management.  

We are a fully accredited supplier with NHSD and are assured against the 20000, 22301 & 27001 ISO standards, DSPT, CyberEssentials+ and MHRA registered.  

Backed up by excellent customer service team and reputation we can help you to support primary care and improve patient outcomes.', N'https://www.informatica-systems.co.uk/', N'{"line1":"Aurora House","line2":"Deltic Avenue","line3":"Rooksley","town":"Milton Keyes","county":"Buckinghamshire","postcode":"MK13 8LW","country":"England"}', 0, CAST(N'2020-03-31T11:48:07.4800000' AS DateTime2), 2, 1),
        (10005, N'PharmData', N'PharmData Ltd', NULL, NULL, N'{"line1":"1 Kirkmanshulme Lane","town":"Manchester","county":"Lancashire","postcode":"M12 4NA","country":"England"}', 0, CAST(N'2019-06-14T13:14:40.0000000' AS DateTime2), 2, 1),
        (10006, N'Medical Director', N'Medical Director', NULL, NULL, N'{"line1":"C-/Linklaters","line2":"One Silk Street","town":"London","county":"Greater London","postcode":"EC2Y 8HQ","country":"England"}', 0, CAST(N'2019-06-18T14:23:07.0000000' AS DateTime2), 2, 1),
        (10007, N'DXS International PLC', N'DXS International PLC', N'DXS International Plc is a health information technology company that provides clinical decision support to more than 2,000 GP practices.

DXS’ core Clinical Decision Support solution, Best Pathway, provides UK primary care staff with treatment guidance and referral services helping to improve health care outcomes and reduce treatment costs.

It integrates with all Primary Care Clinical Systems and presents information to the clinician, relevant to the patient’s condition, via the workflow alerts.

The result is saving GP’s time, empowering Nurses and Pharmacists to take on increased responsibility of care and improving outcomes for the NHS and patients.', N'https://www.dxs-systems.co.uk/dxs-point-of-care.php#intro', N'{"line1":"119 St. Mary''s Road","town":"Market Harborough","county":"Leicestershire","postcode":"LE16 7DT","country":"England"}', 0, CAST(N'2020-03-25T12:12:24.0033333' AS DateTime2), 2, 1),
        (10008, N'LumiraDX Care Solutions UK Ltd', N'LumiraDX Care Solutions UK Ltd', NULL, NULL, N'{"line1":"Francis Clark LLP","line2":"Lowin House","line3":"Tregolls Road","town":"Truro","county":"Cornwall","postcode":"TR1 2NA","country":"England"}', 0, CAST(N'2019-06-20T07:57:23.0000000' AS DateTime2), 2, 1),
        (10009, N'Care IS LTD', N'Care IS LTD', NULL, NULL, N'{"line1":"Biocity, Pennyfoot Street","town":"Nottingham","county":"Nottingham","postcode":"NG1 1GF","country":"England"}', 0, CAST(N'2019-06-21T14:27:44.0000000' AS DateTime2), 2, 1),
        (10010, N'Excelicare', N'Excelicare', NULL, NULL, N'{"line1":"Axsys House","line2":"Marchburn Drive","line3":"Glasgow Airport Business Park","town":"Paisley","county":"","postcode":"PA3 2SJ","country":"Scotland"}', 0, CAST(N'2019-06-26T10:12:29.0000000' AS DateTime2), 2, 1),
        (10011, N'DoctorLink', N'DoctorLink', NULL, NULL, N'{"line1":"Oakhill House","line2":"130 Tonbridge Road","town":"Hildenborough","county":"Kent","postcode":"TN11 9DZ","country":"England"}', 0, CAST(N'2019-06-26T16:46:52.0000000' AS DateTime2), 2, 1),
        (10012, N'MedXnote', N'MedXnote', NULL, NULL, N'{"line1":"Digital Exchange Building","line2":"Crane Street","town":"Dublin","county":"","postcode":"D08 HKR9","country":"Ireland"}', 0, CAST(N'2019-06-26T19:48:39.0000000' AS DateTime2), 2, 1),
        (10013, N'Medidata Exchange Limited', N'Medidata Exchange Limited', NULL, NULL, N'{"line1":"Ty Derw","line2":"Lime Tree Court","line3":"Cardiff Gate Business Park","town":"Cardiff","county":"South Glamorgan","postcode":"CF23 8AB","country":"Wales"}', 0, CAST(N'2019-06-27T07:01:49.0000000' AS DateTime2), 2, 1),
        (10014, N'Total Billing Solutions', N'Total Billing Solutions', NULL, NULL, N'{"line1":"Beachside Business Centre","line2":"Rue Du Hocq","town":"St Clement","county":"","postcode":"JE2 6LF","country":"Jersey"}', 0, CAST(N'2019-06-27T07:30:45.0000000' AS DateTime2), 2, 1),
        (10015, N'Optum Health Solutions UK Limited', N'Optum Health Solutions UK Limited', NULL, NULL, N'{"line1":"10th Floor","line2":"5 Merchant Square","town":"London","county":"Greater London","postcode":"W2 1AS","country":"England"}', 0, CAST(N'2019-06-27T09:43:11.0000000' AS DateTime2), 2, 1),
        (10016, N'BigHand Limited', N'BigHand Limited', NULL, NULL, N'{"line1":"27-29 Union Street","town":"London","county":"","postcode":"SE1 1SD","country":"England"}', 0, CAST(N'2019-06-27T11:49:02.0000000' AS DateTime2), 2, 1),
        (10017, N'Numed Healthcare', N'Numed Holdings Ltd', NULL, NULL, N'{"line1":"Alliance House","line2":"Roman Ridge Road","town":"Sheffield","county":"South Yorkshire","postcode":"S9 1GB","country":"England"}', 0, CAST(N'2019-06-27T11:51:36.0000000' AS DateTime2), 2, 1),
        (10018, N'Sesui', N'Sesui Ltd', NULL, NULL, N'{"line1":"Magdalen Centre","line2":"1 Robert Robinson Avenue","town":"Oxford","county":"Oxfordshire","postcode":"OX4 4GA","country":"England"}', 0, CAST(N'2019-06-27T13:32:13.0000000' AS DateTime2), 2, 1),
        (10019, N'Codegate Ltd', N'Codegate Ltd', NULL, NULL, N'{"line1":"39 Chapel Road","town":"Southampton","county":"","postcode":"SO30 3FG","country":"England"}', 0, CAST(N'2019-06-27T14:54:44.0000000' AS DateTime2), 2, 1),
        (10020, N'MyMed Ltd', N'MyMed Ltd', N'MyMed Ltd, trading as Q doctor, are a market leading supplier of video consultation software and services. Founded by NHS clinicians', N'https://www.qdoctor.io/', N'{"line1":"Willowbrook","line2":"Burbridge Close","line3":"Lytchett Matravers","town":"Poole","county":"Dorset","postcode":"BH16 6EG","country":"England"}', 0, CAST(N'2020-04-07T19:21:45.3600000' AS DateTime2), 2, 1),
        (10021, N'Black Pear Software Ltd', N'Black Pear Software Ltd', NULL, NULL, N'{"line1":"Bartlam House","line2":"Shrawley","town":"Worcester","county":"Worcestershire","postcode":"WR6 6TP","country":"England"}', 0, CAST(N'2019-06-27T18:38:31.0000000' AS DateTime2), 2, 1),
        (10022, N'Gnosco AB', N'Gnosco AB', NULL, NULL, N'{"line1":"Kungsgatan 4","town":"Göteborg","county":"","postcode":"411 19","country":"Sweden"}', 0, CAST(N'2019-06-27T20:01:46.0000000' AS DateTime2), 2, 1),
        (10023, N'Docobo Ltd', N'Docobo Ltd', NULL, NULL, N'{"line1":"21A High Street","town":"Bookham","county":"Surrey","postcode":"KT23 4AA","country":"England"}', 0, CAST(N'2019-07-01T06:13:23.0000000' AS DateTime2), 2, 1),
        (10024, N'Servelec HSC', N'Servelec', NULL, NULL, N'{"line1":"The Straddle","line2":"Wharf Street","line3":"Victoria Quays","town":"Sheffield","county":"South Yorkshire","postcode":"S2 5SY","country":"England"}', 0, CAST(N'2019-07-01T10:56:20.0000000' AS DateTime2), 2, 1),
        (10025, N'Clarity Informatics', N'Clarity Informatics', NULL, NULL, N'{"line1":"Deltic House","line2":"Kingfisher Way","line3":"Silverlink Business Park","line4":"Wallsend","town":"Newcastle on Tyne","county":"","postcode":"NE28 9NX","country":"England"}', 0, CAST(N'2019-07-01T10:58:07.0000000' AS DateTime2), 2, 1),
        (10026, N'eConsult Health', N'eConsult Health', NULL, NULL, N'{"line1":"Nightingale House","line2":"46-48 East Street","town":"Epsom","county":"Surrey","postcode":"KT17 1HQ","country":"England"}', 0, CAST(N'2019-07-01T15:43:57.0000000' AS DateTime2), 2, 1),
        (10027, N'Lantum', N'Lantum', NULL, NULL, N'{"line1":"4th Floor","line2":"15 Bonhill Street","town":"London","county":"Greater London","postcode":"EC2A 4DN","country":"England"}', 0, CAST(N'2019-07-01T16:54:37.0000000' AS DateTime2), 2, 1),
        (10028, N'Clinical Architecture', N'Clinical Architecture', NULL, NULL, N'{"line1":"Suite 2","line2":"Okehampton Business Centre","line3":"Higher Stockley Mead","town":"Okehampton","county":"Devon","postcode":"EX20 1FJ","country":"England"}', 0, CAST(N'2019-07-02T10:55:03.0000000' AS DateTime2), 2, 1),
        (10029, N'Targett Business Technology Limited', N'Targett Business Technology Limited', N'Targett Business Technology (TBT) is an innovative health and social care company that has developed RIVIAM as a secure platform for co-ordinating care. TBT has over 17 years of experience helping NHS and independent providers deliver services to improve healthcare outcomes.

Initially, we provided consulting expertise to establish the Healthcare Commission, CQC and worked for national bodies including NHS Employers and NICE. Through this work we gained an excellent understanding of health and social care regulatory, management and data systems. Since 2010, we have focused on providing services to national and local care provider organisations.

Over the past 5 years, we have invested in and launched our RIVIAM platform for enabling new models of care for health and social care services. In 2019 we launched the Secure Video Service.

Today, RIVIAM is contracted to provide services to NHS Trusts, GP Federations and independent organisations delivering NHS services.', N'http://www.riviam.com/', N'{"line1":"141 Englishcombe Lane","town":"Bath","county":"Bath & North East Somerset","postcode":"BA2 2EL","country":"England"}', 0, CAST(N'2020-04-08T10:03:33.9066667' AS DateTime2), 2, 1),
        (10030, N'AccuRx Limited', N'AccuRx Limited', N'AccuRx is on a mission to make patients healthier and the healthcare workforce happier. We’re doing that by building the platform to bring together all communication around a patient. Our vision is that anyone involved in a patient''s care can easily communicate with everyone else involved in that patient''s care, including the patient. AccuRx was founded in 2016 and has since been adopted by 6,500 GP practices (>95%) and over 150 NHS Trusts.', N'https://www.accurx.com/', N'{"line1":"47 Woodland Rise","town":"London","county":"","postcode":"N10 3UN","country":"England"}', 0, CAST(N'2020-06-11T13:33:17.8233333' AS DateTime2), 2, 1),
        (10031, N'The Martin Bell Partnership', N'The Martin Bell Partnership', N'We are an Independent consultancy, The Martin Bell Partnership, providing expert strategic & practical support for healthcare, healthcare IT & business support. 

Experienced at Board level in the NHS as a CIO, deputy Managing Director previously of one of the UK’s leading clinical systems providers; a wealth of experience from different sectors over many years.

Working with Trusted Associates, themselves steeped in the NHS, many at a senior level within clinical systems suppliers.

Strategic advice & guidance to practical support, clients include large corporates, investment houses, start-ups & healthcare providers.

We have been working with GP&Me since early in 2019 getting this solution onto GP IT Futures. GP&Me, created by two practising GPs, offers easy video consultation facilities. The Martin Bell Partnership, provides the support, sales, marketing & deployment expertise, from years of experience across our Associates in primary care, & many other health care settings.', N'', N'{"line1":"The Old Court House","line2":"Back Street","town":"Aldborough","county":"North Yorkshire","postcode":"YO51 9EX","country":"England"}', 0, CAST(N'2020-04-01T12:48:09.8933333' AS DateTime2), 2, 1),
        (10032, N'Elemental Software', N'In Your Elemental Ltd', NULL, NULL, N'{"line1":"1st Floor","line2":"Progressive House","line3":"25-31 The Diamond","town":"Derry","county":"Northern Ireland","postcode":"BT48 6HP","country":"Northern Ireland"}', 0, CAST(N'2019-07-03T08:39:46.0000000' AS DateTime2), 2, 1),
        (10033, N'AllDayDr Group Ltd', N'AllDayDr Group Ltd', NULL, NULL, N'{"line1":"7 Wendover Drive","line2":"Ladybridge","town":"Bolton","county":"Lancashire","postcode":"BL3 4RX","country":"England"}', 0, CAST(N'2019-07-03T12:07:00.0000000' AS DateTime2), 2, 1),
        (10034, N'Vision Healthcare Limited', N'Vision Healthcare Limited', NULL, NULL, N'{"line1":"The Bread Factory","line2":"1a Broughton Street","town":"London","county":"","postcode":"SW8 3QJ","country":"England"}', 0, CAST(N'2019-07-03T13:04:47.0000000' AS DateTime2), 2, 1),
        (10035, N'Evergreen Life', N'Evergreen Life', N'Wellness app and provider of GP services, Evergreen Life lets people own their health information, driving informed healthcare and giving people the best chance of staying well. 

Collaborating with NHS England, NHS Digital and 3 major GP systems, Evergreen Life has facilitated mutually efficient interaction between practices and patients through online prescription ordering and appointment booking.

The app platform allows all patients in England to keep a copy of their GP record whenever and wherever they need it. Users can add their own information not held in the GP record, including allergies, conditions and medications, building a complete, accurate record that they can share with anyone they wish.

Providing access to up-to-date health information, our person-led solution empowers people to feel more in control to make self-care decisions and manage their care independently, whilst delivering a platform to access primary care services if they need it.', N'https://www.evergreen-life.co.uk/', N'{"line1":"The Edge Business Centre","line2":"The Clowes Street","town":"Manchester","county":"Greater Manchester","postcode":"M3 5NA","country":"England"}', 0, CAST(N'2020-04-01T12:08:32.4233333' AS DateTime2), 2, 1),
        (10036, N'Elsevier Limited', N'Elsevier BV', NULL, NULL, N'{"line1":"Radarweg 29","town":"Amsterdam","county":"Noord-Holland","postcode":"1043NX","country":"Netherlands"}', 0, CAST(N'2019-07-03T17:04:49.0000000' AS DateTime2), 2, 1),
        (10037, N'DictateIT', N'DictateIT', NULL, NULL, N'{"line1":"Aurora House, Deltic Avenue","line2":"Rooksley","town":"Milton Keynes","county":"Buckinghamshire","postcode":"MK13 8LW","country":"England"}', 0, CAST(N'2019-07-04T11:58:29.0000000' AS DateTime2), 2, 1),
        (10038, N'Edenbridge', N'Edenbridge Healthcare Limited', NULL, NULL, N'{"line1":"1 Mariner Court","line2":"Calder Business Park","town":"Wakefield","county":"West Yorkshire","postcode":"WF4 3FL","country":"England"}', 0, CAST(N'2019-07-04T12:17:17.0000000' AS DateTime2), 2, 1),
        (10039, N'Doc Abode Ltd', N'Doc Abode Ltd', NULL, NULL, N'{"line1":"Suite 12 Jason House Kerry Hill","town":"Leeds","county":"West Yorkshire","postcode":"LS18 4JR","country":"England"}', 0, CAST(N'2019-07-04T12:17:20.0000000' AS DateTime2), 2, 1),
        (10040, N'Swiftqueue', N'Swiftqueue', NULL, NULL, N'{"line1":"Eolas Building","line2":"Maynooth University North Campus","town":"Maynooth","county":"county Kildare","postcode":"","country":"Ireland"}', 0, CAST(N'2019-07-04T13:48:16.0000000' AS DateTime2), 2, 1),
        (10041, N'Quality Medical Solutions (QMS)', N'Quality Medical Solutions', NULL, NULL, N'{"line1":"23 Hinton Rd","town":"Bournemouth","county":"Dorset","postcode":"BH1 2EF","country":"England"}', 0, CAST(N'2019-07-04T13:51:36.0000000' AS DateTime2), 2, 1),
        (10042, N'Engage Health Systems Limited', N'Engage Health Systems Limited', NULL, NULL, N'{"line1":"1a, St Nicholas Court","town":"North Walsham","county":"Norfolk","postcode":"NR28 9BY","country":"England"}', 0, CAST(N'2019-07-04T14:00:05.0000000' AS DateTime2), 2, 1),
        (10043, N'IBS Software', N'IBS, Inc.', NULL, NULL, N'{"line1":"Unit 18685","line2":"13 Freeland Park","line3":"Wareham Road","town":"Poole","county":"","postcode":"BH16 6FA","country":"England"}', 0, CAST(N'2019-07-04T14:38:44.0000000' AS DateTime2), 2, 1),
        (10044, N'Answer Digital Ltd', N'Answer Digital Ltd', NULL, NULL, N'{"line1":"Union Mills","line2":"9 Dewsbury Road","town":"Leeds","county":"West Yorkshire","postcode":"LS11 5DD","country":"England"}', 0, CAST(N'2019-07-04T14:43:46.0000000' AS DateTime2), 2, 1),
        (10045, N'Meddbase', N'Medical Management Systems', NULL, NULL, N'{"line1":"140 Buckingham Palace Road","town":"London","county":"Greater London","postcode":"SW1 9SA","country":"England"}', 0, CAST(N'2019-07-04T14:44:48.0000000' AS DateTime2), 2, 1),
        (10046, N'Advanced Health and Care Limited', N'Advanced Health and Care Limited', N'Advanced is one of the UK’s largest providers of business software and services with 19,000+ customers and 2,350+ employees. We provide enterprise and market-focused solutions that allow our customers to reimagine what is possible, innovate in their sectors and improve the lives of millions of people in the UK.

By continually investing in our people, partnerships and technologies, we provide right-first-time solutions that evolve with the changing needs of our customers and the markets they operate in. Our strategy is to enable our customers to drive efficiencies, make informed decisions, act with pace and meet challenges head on.

True partnership is what differentiates us from our competition. We deliver focused solutions for health & care organisations that simplify complex challenges and deliver immediate value.

Advanced solutions help care for 65 million patients in the UK, send millions of clinical documents and support over 80% of the NHS 111 service.', N'https://www.oneadvanced.com/', N'{"line1":"Ditton Park","line2":"Riding Court Road","town":"Datchet","county":"Berkshire","postcode":"SL3 9LL","country":"England"}', 0, CAST(N'2020-03-31T14:06:17.6333333' AS DateTime2), 2, 1),
        (10047, N'GP Access', N'GP Access Ltd', N'', N'https://askmygp.uk/about/', N'{"line1":"The Manor House","line2":"70 Main Street","town":"Cossington","county":"Leicestershire","postcode":"LE7 4UW","country":"England"}', 0, CAST(N'2020-04-01T13:05:48.0266667' AS DateTime2), 2, 1),
        (10048, N'Priority Digital Health Limited', N'Priority Digital Health Limited', NULL, NULL, N'{"line1":"Denny Lodge Business Park","line2":"Ely Road","line3":"Chittering","town":"Cambridge","county":"Cambridgeshire","postcode":"CB25 9PH","country":""}', 0, CAST(N'2019-07-04T16:19:39.0000000' AS DateTime2), 2, 1),
        (10049, N'Medical Data Solutions and Services', N'Medical Data Solutions and Services', NULL, NULL, N'{"line1":"74 Dickenson Road","line2":"Rusholme","town":"Manchester","county":"Greater Manchester","postcode":"M14 5HF","country":"England"}', 0, CAST(N'2019-07-04T16:43:17.0000000' AS DateTime2), 2, 1),
        (10050, N'Philips', N'Philips Electronics UK Limited', NULL, NULL, N'{"line1":"Philips Centre","line2":"Unit 3","line3":"Guildford Business Park","town":"Guildford","county":"Surrey","postcode":"GU2 8XG","country":"England"}', 0, CAST(N'2019-07-04T18:14:27.0000000' AS DateTime2), 2, 1),
        (10051, N'Integrated Care 24 Ltd (IC24)', N'Integrated Care 24 Ltd (IC24)', NULL, NULL, N'{"line1":"Kingston House","line2":"The Long Barrow","line3":"Orbital Park","town":"Ashford","county":"Kent","postcode":"TN24 0GP","country":"England"}', 0, CAST(N'2019-07-05T07:00:03.0000000' AS DateTime2), 2, 1),
        (10052, N'TPP – The Phoenix Partnership (Leeds) Ltd', N'TPP-UK - The Phoenix Partnership Ltd', N'TPP is a digital health company, committed to delivering world-class healthcare software around the world. 

Its EHR product, SystmOne, is used by over 7,000 NHS organisations in over 25 different care settings. This includes significant deployments in Acute Hospitals, Emergency Departments, Mental Health services, Social Care services and General Practice. 

In recent years, TPP has increased its international presence, with live deployments in China and across the Middle East.', N'https://www.tpp-uk.com/', N'{"line1":"TPP House","line2":"Horsforth","town":"Leeds","county":"West Yorkshire","postcode":"LS18 5PX","country":"England"}', 0, CAST(N'2020-05-27T06:26:48.5033333' AS DateTime2), 2, 1),
        (10053, N'Arissian', N'Arissian Ltd', NULL, NULL, N'{"line1":"Basepoint Centre","line2":"Bromsgrove Technology Park","line3":"Isidore Road","town":"Bromsgrove","county":"Worcestershire","postcode":"B60 3ET","country":"England"}', 0, CAST(N'2019-07-05T14:34:54.0000000' AS DateTime2), 2, 1),
        (10054, N'Health Intelligence Ltd', N'Health Intelligence Ltd', NULL, NULL, N'{"line1":"Beechwood Hall","line2":"Kingsmead Road","town":"High Wycombe","county":"","postcode":"HP11 1JL","country":"England"}', 0, CAST(N'2019-07-10T13:37:46.0000000' AS DateTime2), 2, 1),
        (10055, N'Accenda Limited', N'Accenda Limited', NULL, NULL, N'{"line1":"Suite 322","line2":"3rd Floor Broadstone Mill","line3":"Broadstone Road","town":"Stockport","county":"Cheshire","postcode":"SK5 7DL","country":"England"}', 0, CAST(N'2019-07-11T10:07:20.0000000' AS DateTime2), 2, 1),
        (10056, N'Metabolic Healthcare Ltd', N'Metabolic Healthcare Ltd', NULL, NULL, N'{"line1":"1 Westpoint Trading Estate","line2":"Alliance Road","line3":"Ealing","town":"London","county":"","postcode":"W3 0RA","country":"England"}', 0, CAST(N'2019-07-15T11:55:36.0000000' AS DateTime2), 2, 1),
        (10057, N'Redwood Technologies Group Limited', N'Redwood Technologies Group Limited', NULL, NULL, N'{"line1":"Radius Court","line2":"Eastern Road","town":"Bracknell","county":"Berkshire","postcode":"RG12 2UP","country":"England"}', 0, CAST(N'2019-07-15T15:44:31.0000000' AS DateTime2), 2, 1),
        (10058, N'Niche Health', N'Niche Health', NULL, NULL, N'{"line1":"Beasleys Farm","line2":"Upper Gambolds Lane","town":"Bromsgrove","county":"Worcestershire","postcode":"B60 3EZ","country":"England"}', 0, CAST(N'2019-07-16T07:13:10.0000000' AS DateTime2), 2, 1),
        (10059, N'Prescribing Services', N'Prescribing Services Limited', NULL, NULL, N'{"line1":"2 Regis Place","line2":"North Lynn Industrial Estate","town":"King''s Lynn","county":"Norfolk","postcode":"PE30 2JN","country":"England"}', 0, CAST(N'2019-07-17T09:24:52.0000000' AS DateTime2), 2, 1),
        (10060, N'Locum''s Nest', N'Locum''s Nest', NULL, NULL, N'{"line1":"12 Hammersmith Grove","line2":"Hammersmith Fulham","town":"London","county":"","postcode":"W6 7AP","country":"England"}', 0, CAST(N'2019-07-17T10:50:45.0000000' AS DateTime2), 2, 1),
        (10061, N'Microtest Ltd', N'Microtest Ltd', NULL, NULL, N'{"line1":"16-18 Normandy Way","town":"Bodmin","county":"","postcode":"PL31 1EX","country":"England"}', 0, CAST(N'2019-07-18T14:47:34.0000000' AS DateTime2), 2, 1),
        (10062, N'Silicon Practice', N'Silicon Practice', N'We have rolled out over 540 FootFall sites in the UK across a variety of demographics and different practice structures. Working closely with Practices and CCG’s, we have evolved strategies, processes and video tutorials to ensure FootFall is successfully rolled-out in each area.

We have a strong track record of developing the product with CCGs. We have recently made extensive changes to the product with Norfolk and Waveney STP, which have enriched and extended our focus on the digital triage features of FootFall.

We develop a joint project management approach with the CCG, for example arranging Project Initiation Workshops, weekly project management WebEx meetings, pre-mobilisation and post mobilisation practice communication together with reporting requirements.', N'', N'{"line1":"Westbury Court","line2":"Church Road","town":"Westbury On Trym","county":"Gloucester","postcode":"BS9 3EF","country":"England"}', 0, CAST(N'2020-04-07T09:44:04.4600000' AS DateTime2), 2, 1),
        (10063, N'Aire Logic', N'Aire Logic', NULL, NULL, N'{"line1":"Newlaithes Manor","line2":"Newlaithes Road","town":"Leeds","county":"West Yorkshire","postcode":"LS18 4LG","country":"England"}', 0, CAST(N'2019-07-23T16:27:52.0000000' AS DateTime2), 2, 1),
        (10064, N'Medloop', N'Medloop', NULL, NULL, N'{"line1":"St James House","line2":"13 Kensington Square","town":"London","county":"Greater London","postcode":"W8 5HD","country":"England"}', 0, CAST(N'2019-07-24T08:42:08.0000000' AS DateTime2), 2, 1),
        (10065, N'Sensely', N'Sensely', NULL, NULL, N'{"line1":"Pound House","line2":"62a Highgate High Street","town":"London","county":"Greater London","postcode":"N6 5HX","country":"England"}', 0, CAST(N'2019-07-25T14:54:59.0000000' AS DateTime2), 2, 1),
        (10066, N'Primary Care IT Ltd', N'Primary Care IT Ltd', NULL, NULL, N'{"line1":"GF6 Trumpeter House","line2":"Trumpeter Rise","town":"Long Stratton","county":"Norfolk","postcode":"NR15 2DY","country":"England"}', 0, CAST(N'2019-07-26T16:40:07.0000000' AS DateTime2), 2, 1),
        (10067, N'MEDITECH GROUP LIMITED', N'MEDITECH GROUP LIMITED', NULL, NULL, N'{"line1":"1 Northumberland Avenue","town":"London","county":"Greater London","postcode":"WC2N 5BW","country":"USA"}', 0, CAST(N'2019-07-26T20:54:06.0000000' AS DateTime2), 2, 1),
        (10068, N'Substrakt Health', N'Substrakt Health', NULL, NULL, N'{"line1":"2a Victoria Works","line2":"Vittoria St","town":"Birmingham","county":"West Midlands","postcode":"B1 3PE","country":"England"}', 0, CAST(N'2019-07-29T08:26:17.0000000' AS DateTime2), 2, 1),
        (10069, N'Apollo Medical Software Solutions Limited', N'Apollo Medical Software Solutions Limited', NULL, NULL, N'{"line1":"12 Mansfield Centre Office Suite 0:1","line2":"Hamilton Way","line3":"Oakham Business Park","town":"Mansfield","county":"","postcode":"NG18 5FB","country":"England"}', 0, CAST(N'2019-07-30T16:39:19.0000000' AS DateTime2), 2, 1),
        (10070, N'eSynergy Solutions Limited', N'eSynergy Solutions Limited', NULL, NULL, N'{"line1":"50 Fenchurch Street","town":"London","county":"","postcode":"EC3M 3JY","country":"England"}', 0, CAST(N'2019-07-31T14:15:00.0000000' AS DateTime2), 2, 1),
        (10071, N'Connected Tech Group', N'Connected Tech Group', NULL, NULL, N'{"line1":"22, Mount Ephraim","town":"Tunbridge Wells","county":"Kent","postcode":"TN4 8AS","country":"England"}', 0, CAST(N'2019-08-01T14:36:00.0000000' AS DateTime2), 2, 1),
        (10072, N'Push Dr Limited', N'Push Dr Limited', NULL, NULL, N'{"line1":"Arkwright House","line2":"Parsonage","town":"Manchester","county":"Cheshire","postcode":"M3 2LF","country":"England"}', 0, CAST(N'2019-08-02T05:22:07.0000000' AS DateTime2), 2, 1),
        (10073, N'iPLATO', N'iPlato Healthcare Ltd', N'iPLATO has been operating mobile patient services in primary care since 2005. In this time we have launched the iPLATO service with over 1,800 GP practices in 75 CCG/PCTs reaching over 25 million patients.

iPLATO is the digital bridge between any of the principal GP IT systems used by the GP network and their patients using one of the most ubiquitous of modern tools – their own mobile phone.

•	Over 90% of adults in Britain own (at least one) mobile phone.
•	Personal, private and immediate – ideal for health communication.
•	Preferred by many patients, particularly the ‘hard-to-reach’.

More than 25 million patients in the UK already receive messages from their GP or Healthcare provider through iPLATO Patient Care Messaging for Primary Care system.', N'', N'{"line1":"1 King Street","town":"London","county":"Greater London","postcode":"W6 9HR","country":"England"}', 0, CAST(N'2020-04-01T13:48:54.1466667' AS DateTime2), 2, 1),
        (99998, N'NotTPP', N'NotThe Phoenix Partnership', N'TPP is a digital health company, committed to delivering world-class healthcare software around the world. Its EHR product, SystmOne, is used by over 7,000 NHS organisations in over 25 different care settings. This includes significant deployments in Acute Hospitals, Emergency Departments, Mental Health services, Social Care services and General Practice. In recent years, TPP has increased its international presence, with live deployments in China and across the Middle East.', N'https://www.tpp-uk.com/', N'{"line1": "NotTPP Tower", "line2": "High Street", "city": "Leeds", "county": "West Yorkshire", "postcode": "LS1 1BB", "country": "UK"}', 0, CAST(N'2025-08-18T11:08:02.9000000' AS DateTime2), 2, 1),
        (99999, N'NotEmis Health', N'NotEgton Medical Information Systems', N'We are the UK leader in connected healthcare software & services. Through innovative IT we help healthcare professionals access the information they need to provide better, faster and more cost effective patient care.

Our clinical software is used in all major healthcare settings from GP surgeries to pharmacies, community, hospitals, and specialist services. By providing innovative, integrated solutions, we’re working to break the boundaries of system integration & interoperability.

We also specialise in supplying IT infrastructure, software and engineering services, and through our technical support teams we have the skills and knowledge to enhance your IT systems.

Patient (www.patient.info) is the UK’s leading health website. Designed to help patients play a key role in their own care, it provides access to clinically authored health information leaflets, videos, health check and assessment tools and patient forums.

TRUNCATED FOR DEMO', N'www.emishealth.com', N'{"line1": "NotEmis Tower", "line2": "High Street", "city": "Leeds", "county": "West Yorkshire", "postcode": "LS1 1BB", "country": "UK"}', 0, CAST(N'2025-08-18T11:08:02.9000000' AS DateTime2), 2, 1),
        (100000, N'Really Kool Corporation', N'Really Kool Corporation', N'Really Kool Corporation is a fictious UK based IT company but that''s not going to stop us making Really Kool products!', NULL, N'{"line1": "The Tower", "line2": "High Street", "city": "Leeds", "county": "West Yorkshire", "postcode": "LS1 1BB", "country": "UK"}', 0, CAST(N'2025-08-18T11:08:02.9000000' AS DateTime2), 2, 1),
        (100001, N'Remedical Software', N'Remedical Limited', N'The Remedical Software', NULL, N'{"line1": "Remedical Software Tower", "line2": "High Street", "city": "Leeds", "county": "West Yorkshire", "postcode": "LS1 1BB", "country": "UK"}', 0, CAST(N'2025-08-18T11:08:02.9000000' AS DateTime2), 2, 1),
        (100002, N'CareShare', N'CareShare Limited', N'The CareShare', NULL, N'{"line1": "CareShare Tower", "line2": "High Street", "city": "Leeds", "county": "West Yorkshire", "postcode": "LS1 1BB", "country": "UK"}', 0, CAST(N'2025-08-18T11:08:02.9000000' AS DateTime2), 2, 1),
        (100003, N'Avatar Solutions', N'Avatar Solutions Plc', N'Avatar Solutions', NULL, N'{"line1": "Avatar Tower", "line2": "High Street", "city": "Leeds", "county": "West Yorkshire", "postcode": "LS1 1BB", "country": "UK"}', 0, CAST(N'2025-08-18T11:08:02.9000000' AS DateTime2), 2, 1),
        (100004, N'Catterpillar Medworks', N'Catterpillar Medworks Ltd', N'Catterpillar Medworks', NULL, N'{"line1": "Medworks Tower", "line2": "High Street", "city": "Leeds", "county": "West Yorkshire", "postcode": "LS1 1BB", "country": "UK"}', 0, CAST(N'2025-08-18T11:08:02.9000000' AS DateTime2), 2, 1),
        (100005, N'Curtis Systems', N'Curtis Systems Ltd', N'Curtis Systems', NULL, N'{"line1": "Curtis Tower", "line2": "High Street", "city": "Leeds", "county": "West Yorkshire", "postcode": "LS1 1BB", "country": "UK"}', 0, CAST(N'2025-08-18T11:08:02.9000000' AS DateTime2), 2, 1),
        (100006, N'Clinical Raptor', N'Clinical Raptor Ltd', N'Clinical Raptor', NULL, N'{"line1": "Raptor Tower", "line2": "High Street", "city": "Leeds", "county": "West Yorkshire", "postcode": "LS1 1BB", "country": "UK"}', 0, CAST(N'2025-08-18T11:08:02.9000000' AS DateTime2), 2, 1),
        (100007, N'Doc Lightning', N'Doc Lightning Ltd', N'Doc Lightning', NULL, N'{"line1": "Doc Lightning Tower", "line2": "High Street", "city": "Leeds", "county": "West Yorkshire", "postcode": "LS1 1BB", "country": "UK"}', 0, CAST(N'2025-08-18T11:08:02.9000000' AS DateTime2), 2, 1),
        (100008, N'Docability Software', N'Docability Ltd', N'Docability Software', NULL, N'{"line1": "Docability Tower", "line2": "High Street", "city": "Leeds", "county": "West Yorkshire", "postcode": "LS1 1BB", "country": "UK"}', 0, CAST(N'2025-08-18T11:08:02.9000000' AS DateTime2), 2, 1),
        (100009, N'Empire Softworks', N'Empire Softworks Plc', N'Empire Softworks', NULL, N'{"line1": "Empire Tower", "line2": "High Street", "city": "Leeds", "county": "West Yorkshire", "postcode": "LS1 1BB", "country": "UK"}', 0, CAST(N'2025-08-18T11:08:02.9000000' AS DateTime2), 2, 1),
        (100010, N'Cure Forward', N'Cure Forward Ltd', N'Cure Forward', NULL, N'{"line1": "Cure Forward Tower", "line2": "High Street", "city": "Leeds", "county": "West Yorkshire", "postcode": "LS1 1BB", "country": "UK"}', 0, CAST(N'2025-08-18T11:08:02.9000000' AS DateTime2), 2, 1),
        (100011, N'Hansa Healthcare', N'Hansa Healthcare Plc', N'Hansa Healthcare', NULL, N'{"line1": "Hansa Tower", "line2": "High Street", "city": "Leeds", "county": "West Yorkshire", "postcode": "LS1 1BB", "country": "UK"}', 0, CAST(N'2025-08-18T11:08:02.9000000' AS DateTime2), 2, 1),
        (100012, N'Moonlight Intercare', N'Moonlight Intercare', N'Moonlight Intercare', NULL, N'{"line1": "Moonlight Tower", "line2": "High Street", "city": "Leeds", "county": "West Yorkshire", "postcode": "LS1 1BB", "country": "UK"}', 0, CAST(N'2025-08-18T11:08:02.9000000' AS DateTime2), 2, 1),
        (100013, N'eHealth Development', N'eHealth Development', N'eHealth Development', NULL, N'{"line1": "eHealth Tower", "line2": "High Street", "city": "Leeds", "county": "West Yorkshire", "postcode": "LS1 1BB", "country": "UK"}', 0, CAST(N'2025-08-18T11:08:02.9000000' AS DateTime2), 2, 1),
        (100014, N'Dr. Nick', N'Dr. Nick', N'Dr. Nick', NULL, N'{"line1": "Simpson Tower", "line2": "High Street", "city": "Leeds", "county": "West Yorkshire", "postcode": "LS1 1BB", "country": "UK"}', 0, CAST(N'2025-08-18T11:08:02.9000000' AS DateTime2), 2, 1),
        (100015, N'Testproof Technology', N'Testproof Technology', N'Testproof Technology', NULL, N'{"line1": "Testproof Tower", "line2": "High Street", "city": "Leeds", "county": "West Yorkshire", "postcode": "LS1 1BB", "country": "UK"}', 0, CAST(N'2025-08-18T11:08:02.9000000' AS DateTime2), 2, 1),
        (100016, N'Hojo Health', N'Hojo Health Ltd', N'Hojo Health', NULL, N'{"line1": "Hojo Tower", "line2": "High Street", "city": "Leeds", "county": "West Yorkshire", "postcode": "LS1 1BB", "country": "UK"}', 0, CAST(N'2025-08-18T11:08:02.9000000' AS DateTime2), 2, 1),
        (100017, N'Jericho Healthcare', N'Jericho Ltd', N'Jericho Healthcare', NULL, N'{"line1": "Jericho Tower", "line2": "High Street", "city": "Leeds", "county": "West Yorkshire", "postcode": "LS1 1BB", "country": "UK"}', 0, CAST(N'2025-08-18T11:08:02.9000000' AS DateTime2), 2, 1),
        (100018, N'Mana Systems', N'Mana Systems', N'Mana Systems', NULL, N'{"line1": "Mana Tower", "line2": "High Street", "city": "Leeds", "county": "West Yorkshire", "postcode": "LS1 1BB", "country": "UK"}', 0, CAST(N'2025-08-18T11:08:02.9000000' AS DateTime2), 2, 1),
        (100019, N'Sunhealth Nanosystems', N'Sunhealth Nanosystems', N'Sunhealth Nanosystems', NULL, N'{"line1": "Sunhealth Tower", "line2": "High Street", "city": "Leeds", "county": "West Yorkshire", "postcode": "LS1 1BB", "country": "UK"}', 0, CAST(N'2025-08-18T11:08:02.9000000' AS DateTime2), 2, 1),
        (100020, N'Oakwood', N'Oakwood Ltd', N'Oakwood', NULL, N'{"line1": "Oakwood Tower", "line2": "High Street", "city": "Leeds", "county": "West Yorkshire", "postcode": "LS1 1BB", "country": "UK"}', 0, CAST(N'2025-08-18T11:08:02.9000000' AS DateTime2), 2, 1),
        (100021, N'Dummy supplier', N'Dummy supplier', NULL, NULL, N'{"Line1":"10-12 Example Street","Line2":null,"Line3":null,"Line4":null,"Line5":null,"Town":"Leeds","County":"West Yorkshire","Postcode":"LS1 2AB","Country":"United Kingdom"}', 0, CAST(N'2026-01-27T13:47:04.5678005' AS DateTime2), 2, 1),
        (100022, N'Community pharmacy supplier', N'Community pharmacy supplier', N'Test supplier for community pharmacy solutions.', NULL, N'{"Line1":"1 Pharmacy Way","Line2":null,"Line3":null,"Line4":null,"Line5":null,"Town":"Leeds","County":"West Yorkshire","Postcode":"LS1 2AB","Country":"United Kingdom"}', 0, CAST(N'2026-08-25T09:00:00.0000000' AS DateTime2), 2, 1))
    AS SOURCE ([Id], [Name], [LegalName], [Summary], [SupplierUrl], [Address], [Deleted], [LastUpdated], [LastUpdatedBy], [IsActive])
    ON TARGET.[Id] = SOURCE.[Id]
    WHEN MATCHED THEN UPDATE SET
        TARGET.[Name] = SOURCE.[Name],
        TARGET.[LegalName] = SOURCE.[LegalName],
        TARGET.[Summary] = SOURCE.[Summary],
        TARGET.[SupplierUrl] = SOURCE.[SupplierUrl],
        TARGET.[Address] = SOURCE.[Address],
        TARGET.[Deleted] = SOURCE.[Deleted],
        TARGET.[LastUpdated] = SOURCE.[LastUpdated],
        TARGET.[LastUpdatedBy]  = SOURCE.[LastUpdatedBy],
        TARGET.[IsActive] = SOURCE.[IsActive]
    WHEN NOT MATCHED BY TARGET THEN
        INSERT ([Id], [Name], [LegalName], [Summary], [SupplierUrl], [Address], [Deleted], [LastUpdated], [LastUpdatedBy], [IsActive])
        VALUES (SOURCE.[Id], SOURCE.[Name], SOURCE.[LegalName], SOURCE.[Summary], SOURCE.[SupplierUrl], SOURCE.[Address], SOURCE.[Deleted], SOURCE.[LastUpdated], SOURCE.[LastUpdatedBy], SOURCE.[IsActive]);

    -- Supplier Contacts
    SET IDENTITY_INSERT [catalogue].[SupplierContacts] ON
    MERGE INTO [catalogue].[SupplierContacts] AS TARGET
    USING (
    VALUES 
        (1, 100000, N'Aarav', N'Patel', N'aaravpatel@test.com', N'01234 567891', NULL, GETUTCDATE(), 2),
        (2, 100001, N'Aisha', N'Khan', N'aishakhan@test.com', N'01234 567891', NULL, GETUTCDATE(), 2),
        (3, 100002, N'Liam', N'Connor', N'liamconnor@test.com', N'01234 567891', NULL, GETUTCDATE(), 2),
        (4, 100003, N'Chloe', N'Nguyen', N'chloenguyen@test.com', N'01234 567891', NULL, GETUTCDATE(), 2),
        (5, 100004, N'Mohammed', N'Ali', N'mohammedali@test.com', N'01234 567891', NULL, GETUTCDATE(), 2),
        (6, 100005, N'Sophie', N'Williams', N'sophiewilliams@test.com', N'01234 567891', NULL, GETUTCDATE(), 2),
        (7, 100006, N'Ethan', N'Smith', N'ethansmith@test.com', N'01234 567891', NULL, GETUTCDATE(), 2),
        (8, 100007, N'Zara', N'Hussain', N'zarahussain@test.com', N'01234 567891', NULL, GETUTCDATE(), 2),
        (9, 100008, N'Lucas', N'Brown', N'lucasbrown@test.com', N'+44(0)1234 567891', NULL, GETUTCDATE(), 2),
        (10, 100009, N'Maya', N'Davies', N'mayadavies@test.com', N'01234 567891', NULL, GETUTCDATE(), 2),
        (11, 100010, N'Oscar', N'Wilson', N'oscarwilson@test.com', N'01234 567891', NULL, GETUTCDATE(), 2),
        (12, 100011, N'Lily', N'Wright', N'lilywright@test.com', N'01234 567891', NULL, GETUTCDATE(), 2),
        (13, 100012, N'James', N'Green', N'jamesgreen@test.com', N'+44(0)1234 567891', NULL, GETUTCDATE(), 2),
        (14, 100013, N'Grace', N'Hall', N'gracehall@test.com', N'+44 1234 567891', NULL, GETUTCDATE(), 2),
        (15, 100014, N'Henry', N'Adams', N'henryadams@test.com', N'+44 1234 567891', NULL, GETUTCDATE(), 2),
        (16, 100015, N'Ella', N'Baker', N'ellabaker@test.com', N'+44 1234 567891', NULL, GETUTCDATE(), 2),
        (17, 100016, N'Freddie', N'Harris', N'freddieharris@test.com', N'+44 1234 567891', NULL, GETUTCDATE(), 2),
        (18, 100017, N'Evie', N'Martin', N'eviemartin@test.com', N'+44 1234 567891', NULL, GETUTCDATE(), 2),
        (19, 100018, N'Leo', N'Thompson', N'leothompson@test.com', N'+44 1234 567891', NULL, GETUTCDATE(), 2),
        (20, 100019, N'Poppy', N'White', N'poppywhite@test.com', N'+44 1234 567891', NULL, GETUTCDATE(), 2),
        (21, 100020, N'Archie', N'Lewis', N'archielewis@test.com', N'+44 1234 567891', NULL, GETUTCDATE(), 2),
        (22, 99998, N'Isabella', N'Clark', N'isabellaclark@test.com', N'+44 1234 567891', NULL, GETUTCDATE(), 2),
        (23, 99999, N'Jacob', N'Turner', N'jacobturner@test.com', N'+44 1234 567891', NULL, GETUTCDATE(), 2),
        (24, 10000, N'Jonas', N'Chan', N'jonaschan@test.test', N'01234 567891', NULL, GETUTCDATE(), 2),
        (25, 10001, N'Madelyn', N'Conner', N'madelynconner@test.test', N'01234 567891', NULL, GETUTCDATE(), 2),
        (26, 10002, N'Cristian', N'Chang', N'cristianchang@test.test', N'01234 567891', NULL, GETUTCDATE(), 2),
        (27, 10003, N'Kaelynn', N'Archer', N'kaelynnarcher@test.test', N'01234 567891', NULL, GETUTCDATE(), 2),
        (28, 10004, N'Ezra', N'Horn', N'ezrahorn@test.test', N'01234 567891', NULL, GETUTCDATE(), 2),
        (29, 10005, N'Briley', N'Marsh', N'brileymarsh@test.test', N'01234 567891', NULL, GETUTCDATE(), 2),
        (30, 10006, N'Augustine', N'Wagner', N'augustinewagner@test.test', N'01234 567891', NULL, GETUTCDATE(), 2),
        (31, 10007, N'Charlee', N'Mcdowell', N'charleemcdowell@test.test', N'01234 567891', NULL, GETUTCDATE(), 2),
        (32, 10008, N'Cassius', N'Wilkinson', N'cassiuswilkinson@test.test', N'01234 567891', NULL, GETUTCDATE(), 2),
        (33, 10009, N'Maliah', N'Mcconnell', N'maliahmcconnell@test.test', N'01234 567891', NULL, GETUTCDATE(), 2),
        (34, 10010, N'Aiden', N'Nguyen', N'aidennguyen@test.test', N'01234 567891', NULL, GETUTCDATE(), 2),
        (35, 10011, N'Evie', N'Ryan', N'evieryan@test.test', N'01234 567891', NULL, GETUTCDATE(), 2),
        (36, 10012, N'Gary', N'Becker', N'garybecker@test.test', N'01234 567891', NULL, GETUTCDATE(), 2),
        (37, 10013, N'Sutton', N'Walsh', N'suttonwalsh@test.test', N'01234 567891', NULL, GETUTCDATE(), 2),
        (38, 10014, N'Joshua', N'Schwartz', N'joshuaschwartz@test.test', N'01234 567891', NULL, GETUTCDATE(), 2),
        (39, 10015, N'Karla', N'Acevedo', N'karlaacevedo@test.test', N'01234 567891', NULL, GETUTCDATE(), 2),
        (40, 10016, N'Ishaan', N'Richards', N'ishaanrichards@test.test', N'01234 567891', NULL, GETUTCDATE(), 2),
        (41, 10017, N'Hunter', N'Romero', N'hunterromero@test.test', N'01234 567891', NULL, GETUTCDATE(), 2),
        (42, 10018, N'Ronin', N'Shepherd', N'roninshepherd@test.test', N'01234 567891', NULL, GETUTCDATE(), 2),
        (43, 10019, N'Jasmin', N'Gomez', N'jasmingomez@test.test', N'01234 567891', NULL, GETUTCDATE(), 2),
        (44, 10020, N'Corey', N'Matthews', N'coreymatthews@test.test', N'01234 567891', NULL, GETUTCDATE(), 2),
        (45, 10021, N'Gemma', N'Stanley', N'gemmastanley@test.test', N'01234 567891', NULL, GETUTCDATE(), 2),
        (46, 10022, N'Leonard', N'Whitehead', N'leonardwhitehead@test.test', N'01234 567891', NULL, GETUTCDATE(), 2),
        (47, 10023, N'Yaretzi', N'Farrell', N'yaretzifarrell@test.test', N'01234 567891', NULL, GETUTCDATE(), 2),
        (48, 10024, N'Dylan', N'Brady', N'dylanbrady@test.test', N'01234 567891', NULL, GETUTCDATE(), 2),
        (49, 10025, N'Jimena', N'House', N'jimenahouse@test.test', N'01234 567891', NULL, GETUTCDATE(), 2),
        (50, 10026, N'Miguel', N'Parks', N'miguelparks@test.test', N'01234 567891', NULL, GETUTCDATE(), 2),
        (51, 10027, N'Kaitlynn', N'Cummings', N'kaitlynncummings@test.test', N'01234 567891', NULL, GETUTCDATE(), 2),
        (52, 10028, N'Avi', N'Stuart', N'avistuart@test.test', N'01234 567891', NULL, GETUTCDATE(), 2),
        (53, 10029, N'Julianne', N'Downs', N'juliannedowns@test.test', N'01234 567891', NULL, GETUTCDATE(), 2),
        (54, 10030, N'Joziah', N'Tanner', N'joziahtanner@test.test', N'01234 567891', NULL, GETUTCDATE(), 2),
        (55, 10031, N'Vienna', N'Morse', N'viennamorse@test.test', N'01234 567891', NULL, GETUTCDATE(), 2),
        (56, 10032, N'Asher', N'Barnes', N'asherbarnes@test.test', N'01234 567891', NULL, GETUTCDATE(), 2),
        (57, 10033, N'Mabel', N'Chase', N'mabelchase@test.test', N'01234 567891', NULL, GETUTCDATE(), 2),
        (58, 10034, N'Pedro', N'Ruiz', N'pedroruiz@test.test', N'01234 567891', NULL, GETUTCDATE(), 2),
        (59, 10035, N'Rose', N'Alvarez', N'rosealvarez@test.test', N'01234 567891', NULL, GETUTCDATE(), 2),
        (60, 10036, N'Langston', N'Finley', N'langstonfinley@test.test', N'01234 567891', NULL, GETUTCDATE(), 2),
        (61, 10037, N'Elsie', N'Villa', N'elsievilla@test.test', N'01234 567891', NULL, GETUTCDATE(), 2),
        (62, 10038, N'Enoch', N'Watkins', N'enochwatkins@test.test', N'01234 567891', NULL, GETUTCDATE(), 2),
        (63, 10039, N'Alani', N'Frye', N'alanifrye@test.test', N'01234 567891', NULL, GETUTCDATE(), 2),
        (64, 10040, N'Grey', N'Richardson', N'greyrichardson@test.test', N'01234 567891', NULL, GETUTCDATE(), 2),
        (65, 10041, N'Katelyn', N'Boyer', N'katelynboyer@test.test', N'01234 567891', NULL, GETUTCDATE(), 2),
        (66, 10042, N'Jordy', N'Bartlett', N'jordybartlett@test.test', N'01234 567891', NULL, GETUTCDATE(), 2),
        (67, 10043, N'Alaia', N'Rosales', N'alaiarosales@test.test', N'01234 567891', NULL, GETUTCDATE(), 2),
        (68, 10044, N'Beckett', N'Walton', N'beckettwalton@test.test', N'01234 567891', NULL, GETUTCDATE(), 2),
        (69, 10045, N'Stephanie', N'Wyatt', N'stephaniewyatt@test.test', N'01234 567891', NULL, GETUTCDATE(), 2),
        (70, 10046, N'Douglas', N'Irwin', N'douglasirwin@test.test', N'01234 567891', NULL, GETUTCDATE(), 2),
        (71, 10047, N'Amanda', N'Durham', N'amandadurham@test.test', N'01234 567891', NULL, GETUTCDATE(), 2),
        (72, 10048, N'Arthur', N'Roman', N'arthurroman@test.test', N'01234 567891', NULL, GETUTCDATE(), 2),
        (73, 10049, N'Makenzie', N'Guerra', N'makenzieguerra@test.test', N'01234 567891', NULL, GETUTCDATE(), 2),
        (74, 10050, N'Jameson', N'Fitzpatrick', N'jamesonfitzpatrick@test.test', N'01234 567891', NULL, GETUTCDATE(), 2),
        (75, 10051, N'Cali', N'Schmitt', N'calischmitt@test.test', N'01234 567891', NULL, GETUTCDATE(), 2),
        (76, 10052, N'Stephen', N'Barker', N'stephenbarker@test.test', N'01234 567891', NULL, GETUTCDATE(), 2),
        (77, 10053, N'Ashley', N'Leonard', N'ashleyleonard@test.test', N'01234 567891', NULL, GETUTCDATE(), 2),
        (78, 10054, N'Steve', N'Carroll', N'stevecarroll@test.test', N'01234 567891', NULL, GETUTCDATE(), 2),
        (79, 10055, N'Emilee', N'Meadows', N'emileemeadows@test.test', N'01234 567891', NULL, GETUTCDATE(), 2),
        (80, 10056, N'Roberto', N'Scott', N'robertoscott@test.test', N'01234 567891', NULL, GETUTCDATE(), 2),
        (81, 10057, N'Allison', N'Beasley', N'allisonbeasley@test.test', N'01234 567891', NULL, GETUTCDATE(), 2),
        (82, 10058, N'Maximilian', N'Lambert', N'maximilianlambert@test.test', N'01234 567891', NULL, GETUTCDATE(), 2),
        (83, 10059, N'Kristen', N'Mann', N'kristenmann@test.test', N'01234 567891', NULL, GETUTCDATE(), 2),
        (84, 10060, N'Kayson', N'Cooley', N'kaysoncooley@test.test', N'01234 567891', NULL, GETUTCDATE(), 2),
        (85, 10061, N'Carly', N'Andersen', N'carlyandersen@test.test', N'01234 567891', NULL, GETUTCDATE(), 2),
        (86, 10062, N'Valentin', N'Houston', N'valentinhouston@test.test', N'01234 567891', NULL, GETUTCDATE(), 2),
        (87, 10063, N'Frances', N'Mccall', N'francesmccall@test.test', N'01234 567891', NULL, GETUTCDATE(), 2),
        (88, 10064, N'Luciano', N'Sawyer', N'lucianosawyer@test.test', N'01234 567891', NULL, GETUTCDATE(), 2),
        (89, 10065, N'Savanna', N'Delgado', N'savannadelgado@test.test', N'01234 567891', NULL, GETUTCDATE(), 2),
        (90, 10066, N'Daniel', N'Esparza', N'danielesparza@test.test', N'01234 567891', NULL, GETUTCDATE(), 2),
        (91, 10067, N'Ivory', N'Murray', N'ivorymurray@test.test', N'01234 567891', NULL, GETUTCDATE(), 2),
        (92, 10068, N'Henrik', N'Haynes', N'henrikhaynes@test.test', N'01234 567891', NULL, GETUTCDATE(), 2),
        (93, 10069, N'Journee', N'Calderon', N'journeecalderon@test.test', N'01234 567891', NULL, GETUTCDATE(), 2),
        (94, 10070, N'Jaxen', N'Joseph', N'jaxenjoseph@test.test', N'01234 567891', NULL, GETUTCDATE(), 2),
        (95, 10071, N'Avah', N'Martin', N'avahmartin@test.test', N'01234 567891', NULL, GETUTCDATE(), 2),
        (96, 10072, N'Junior', N'Bauer', N'juniorbauer@test.test', N'01234 567891', NULL, GETUTCDATE(), 2),
        (97, 10073, N'Camille', N'Kirk', N'camillekirk@test.test', N'01234 567891', NULL, GETUTCDATE(), 2),
        (98, 100021, N'Joe', N'Sample', N'joe@email.com', N'1234567', N'Procurement', GETUTCDATE(), 2))
    AS SOURCE ([Id], [SupplierId], [FirstName], [LastName], [Email], [PhoneNumber], [Department], [LastUpdated], [LastUpdatedBy])
    ON TARGET.[Id] = SOURCE.[Id]
    WHEN MATCHED THEN UPDATE SET
        TARGET.[SupplierId] = SOURCE.[SupplierId],
        TARGET.[FirstName] = SOURCE.[FirstName],
        TARGET.[LastName] = SOURCE.[LastName],
        TARGET.[Email] = SOURCE.[Email],
        TARGET.[PhoneNumber] = SOURCE.[PhoneNumber],
        TARGET.[Department] = SOURCE.[Department],
        TARGET.[LastUpdated]  = SOURCE.[LastUpdated],
        TARGET.[LastUpdatedBy] = SOURCE.[LastUpdatedBy]
    WHEN NOT MATCHED BY TARGET THEN
        INSERT ([Id], [SupplierId], [FirstName], [LastName], [Email], [PhoneNumber], [Department], [LastUpdated], [LastUpdatedBy])
        VALUES (SOURCE.[Id], SOURCE.[SupplierId], SOURCE.[FirstName], SOURCE.[LastName], SOURCE.[Email], SOURCE.[PhoneNumber], SOURCE.[Department], SOURCE.[LastUpdated], SOURCE.[LastUpdatedBy]);
    SET IDENTITY_INSERT [catalogue].[SupplierContacts] OFF
END

GO
