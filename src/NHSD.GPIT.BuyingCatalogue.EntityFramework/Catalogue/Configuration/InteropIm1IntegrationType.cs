﻿using System.ComponentModel.DataAnnotations;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Configuration;

public enum InteropIm1IntegrationType
{
    [Display(Name = "IM1 Bulk")]
    Bulk,
    [Display(Name = "IM1 Transactional")]
    Transactional,
    [Display(Name = "IM1 Patient Facing")]
    Patient_Facing,
}
