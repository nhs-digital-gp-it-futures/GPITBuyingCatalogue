IF UPPER('$(INSERT_TEST_DATA)') = 'TRUE'
BEGIN
    MERGE INTO [catalogue].[AdditionalServices] AS TARGET
    USING (
    VALUES 
        (N'100000-001-A01', N'Addition to Write on Time', N'Write on time Addttion Full Description', GETUTCDATE(), 2, N'100000-001'),
        (N'10000-001A001', N'Long Term Conditions Manager', N'Long Term Conditions (LTC) Manager provides easy-to-use, intelligent templates and alerts in EMIS Web, so you can provide fast, efficient and safe LTC management. Stratify patients by complexity and distribute work among clinicians and healthcare assistants with varying skill mix.', GETUTCDATE(), 2, N'10000-001'),
        (N'10000-001A002', N'EMIS Mobile', N'EMIS Mobile supports offline, out-of-practice working for healthcare professionals, providing access to a patient’s care record, updating consultations/encounters with data, booking appointments, tasks and medication. Native apps for iOS, Android and Windows 10 ensure an optimised user experience.', GETUTCDATE(), 2, N'10000-001'),
        (N'10000-001A003', N'Automated Arrivals', N'A solution to improve practice efficiency and patient experience. Patients use a touchscreen to check-in and get an instruction on arrival and book available future appointments. Staff control the kiosk from a central portal for messages to patient groups and confirming personal data.', GETUTCDATE(), 2, N'10000-001'),
        (N'10000-001A004', N'Extract Services', N'Mobilising data within EMIS systems to assist commissioners and providers of care with clinical decision making.', GETUTCDATE(), 2, N'10000-001'),
        (N'10000-001A005', N'EMIS Web Dispensing', N'EMIS Web provides integrated functionality to support dispensing doctors who provide primary healthcare to UK rural patients. This enables dispensing GP practices to provide access to medicines and general healthcare under one roof.', GETUTCDATE(), 2, N'10000-001'),
        (N'10000-001A006', N'Document Management', N'EMIS Web Document Management helps you workflow both scanned and electronically received documents in your practice, such as Out of Hours notifications, Hospital Discharge letters and NHS111 notifications. Assign different kinds of tasks to different users or teams, so you can work more efficiently.', GETUTCDATE(), 2, N'10000-001'),
        (N'10000-001A007', N'Risk Stratification', N'Risk Stratification tools are specifically designed to identify those individuals who are at high risk of experiencing a future adverse event, such as readmission to hospital or unplanned hospital admission, or an acute event such as a heart attack. Risk Stratification often takes into account multiple risk factors and morbidities and their inter dependency, rather than just concentrating on one criterion (e.g. high blood pressure).\nA Risk Stratification Capability will allow risk calculations to be performed for individual patients using algorithms that can be run against Patient data (known algorithms can be used such as QRISK®2, QDiabetes®, QFracture®, QCancer®, QAdmissions®) to proactively identify a Patient risk level ', GETUTCDATE(), 2, N'10000-001'),
        (N'10000-001A008', N'Enterprise Search and Reports', N'Enterprise Search and Reports is the fast and efficient way to source, search and report on GP data in real time. With the ability to develop custom searches, you can measure against your key objective, including QOF, LESs/DESs, referral activity and immunisations.', GETUTCDATE(), 2, N'10000-001'),
        (N'100001-001-A01', N'Addition to Appointment Gateway', N'Appointment Gateway Addition Full Description', GETUTCDATE(), 2, N'100001-001'),
        (N'100002-001-A01', N'Addition to Zen Guidance', N'Zen Guidance Addition Full Description', GETUTCDATE(), 2, N'100002-001'),
        (N'100004-001-A01', N'Addition to Diagnostics XYZ', N'Diagnostics XYZ Addition Full Description', GETUTCDATE(), 2, N'100004-001'),
        (N'100005-001-A01', N'Addition to Document Wizard', N'Document Wizard Addition Full Description', GETUTCDATE(), 2, N'100005-001'),
        (N'100006-001-A01', N'Addition to Paperlite', N'Paperlite Addition Full Description', GETUTCDATE(), 2, N'100006-001'),
        (N'100007-001-A01', N'Addition to Medsort', N'Medsort Addition Full Description', GETUTCDATE(), 2, N'100007-001'),
        (N'100007-002-A01', N'Addition to Boston Dynamics', N'Boston Dynamics Addition Full Description', GETUTCDATE(), 2, N'100007-002'),
        (N'10007-002A001', N'Localised Referral Forms', N'Build and maintain a maximum of 100 custom items according to local guidelines for a PCN, GP Federation , CCG or STP as supplied by the client. Saving users time by reducing the burden of customising documentation according to local requirements to drive uniformity in care', GETUTCDATE(), 2, N'10007-002'),
        (N'10007-002A002', N'Localised Supporting Content', N'Build and maintain a maximum of 100 custom items according to local guidelines for a PCN, GP Federation, CCG or STP as supplied by the client. Saving users time by reducing the burden of customising documentation according to local requirements to drive uniformity in care', GETUTCDATE(), 2, N'10007-002'),
        (N'10030-001A001', N'AccuRx Video Consultation', N'The Video Consultation Capability allows Health or Care Professionals to conduct secure live remote video consultations with individual or groups of Patients/Service Users/Proxies ensuring they can receive support relating to healthcare concerns when a Video Consultation is most appropriate', GETUTCDATE(), 2, N'10030-001'),
        (N'10035-001A001', N'Digital First Consultations', N'Digital First Consultations enables remote working for clinicians, including video-based appointments, with or without access to the regular practice IT system. Patients can book a digital consultation, share their record with the clinician and notes are sent back to the GP system via interfaces.', GETUTCDATE(), 2, N'10035-001'),
        (N'10052-002A001', N'SystmOne Enhanced', N'SystmOne Enhanced is a comprehensive package of advanced features that improve the efficiency and workflow of practices. The tools offer additional administrative and clinical support to promote higher standards of care and improve data quality. The following areas of functionality are included: Document Management and Scanning: Document processing is quick and easy, reducing administrative burden and saving valuable time.', GETUTCDATE(), 2, N'10052-002'),
        (N'10052-002A002', N'SystmOne Mobile Working', N'Users can access their SystmOne unit and its data via a mobile client available on devices such as tablets. This can be used both online and offline, e.g. in the community where an internet connection may not always be available.', GETUTCDATE(), 2, N'10052-002'),
        (N'10052-002A003', N'SystmOne Auto Planner', N'SystmOne AutoPlanner ensures staff with the necessary skillsets are routed to the right patient homes in the most efficient way.', GETUTCDATE(), 2, N'10052-002'),
        (N'10052-002A004', N'SystemOne Shared Admin', N'Organisations can choose to share administrative duties with other organisations to more efficiently manage workloads and reduce staff burden.', GETUTCDATE(), 2, N'10052-002'),
        (N'10052-002A005', N'TPP Video Conferencing with Airmid', N'TPP video consultation functionality provides remote access to patients for the purpose of providing direct healthcare. Video consultations mean that face to face appointments can occur electronically without the need for either party to travel to a specified location.', GETUTCDATE(), 2, N'10052-002'),
        (N'99999-89-A01', N'Addition to NotEmis Web GP', N'NotEmis Web GP Addition Full Description', GETUTCDATE(), 2, N'99999-89'))
    AS SOURCE ([CatalogueItemId], [Summary], [FullDescription], [LastUpdated], [LastUpdatedBy], [SolutionId])
    ON TARGET.[CatalogueItemId] = SOURCE.[CatalogueItemId]
    WHEN MATCHED THEN UPDATE SET
        TARGET.[Summary] = SOURCE.[Summary],
        TARGET.[FullDescription] = SOURCE.[FullDescription],
        TARGET.[LastUpdated] = SOURCE.[LastUpdated],
        TARGET.[LastUpdatedBy] = SOURCE.[LastUpdatedBy],
        TARGET.[SolutionId] = SOURCE.[SolutionId]
    WHEN NOT MATCHED BY TARGET THEN
        INSERT ([CatalogueItemId], [Summary], [FullDescription], [LastUpdated], [LastUpdatedBy], [SolutionId])
        VALUES (SOURCE.[CatalogueItemId], SOURCE.[Summary], SOURCE.[FullDescription], SOURCE.[LastUpdated], SOURCE.[LastUpdatedBy], SOURCE.[SolutionId]);
END

GO
