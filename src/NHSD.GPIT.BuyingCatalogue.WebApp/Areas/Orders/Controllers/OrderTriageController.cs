using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.OrderTriage;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Shared;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers
{
    [Authorize("Buyer")]
    [Area("Orders")]
    [Route("order/organisation/{internalOrgId}/order/triage")]
    public class OrderTriageController : Controller
    {
        private readonly IOrganisationsService organisationsService;
        private readonly ISupplierService supplierService;

        public OrderTriageController(
            IOrganisationsService organisationsService,
            ISupplierService supplierService)
        {
            ArgumentNullException.ThrowIfNull(organisationsService);
            ArgumentNullException.ThrowIfNull(supplierService);

            this.organisationsService = organisationsService;
            this.supplierService = supplierService;
        }

        [HttpGet("order-item-type")]
        public async Task<IActionResult> OrderItemType(string internalOrgId, CatalogueItemType? orderType = null)
        {
            var organisation = await organisationsService.GetOrganisationByInternalIdentifier(internalOrgId);

            var model = new OrderItemTypeModel(organisation.Name)
            {
                BackLink = Url.Action(
                    nameof(DashboardController.Organisation),
                    typeof(DashboardController).ControllerName(),
                    new { internalOrgId }),
                SelectedOrderItemType = orderType,
            };

            return View(model);
        }

        [HttpPost("order-item-type")]
        public IActionResult OrderItemType(string internalOrgId, OrderItemTypeModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            switch (model.SelectedOrderItemType.Value)
            {
                case CatalogueItemType.Solution:
                    return RedirectToAction(
                    nameof(SelectOrganisation),
                    new { internalOrgId, orderType = OrderTypeEnum.Solution });

                case CatalogueItemType.AssociatedService:
                    return RedirectToAction(
                    nameof(DetermineAssociatedServiceType),
                    new { internalOrgId });

                default:
                    throw new InvalidOperationException($"Unhandled {nameof(CatalogueItemType)} {model.SelectedOrderItemType}. Validation should have prevented this.");
            }
        }

        [HttpGet("determine-associated-service-type")]
        public async Task<IActionResult> DetermineAssociatedServiceType(string internalOrgId, OrderTypeEnum? orderType = null)
        {
            var organisation = await organisationsService.GetOrganisationByInternalIdentifier(internalOrgId);

            var model = new DetermineAssociatedServiceTypeModel(
                organisation.Name,
                await supplierService.HasActiveSuppliers(OrderTypeEnum.AssociatedServiceMerger),
                await supplierService.HasActiveSuppliers(OrderTypeEnum.AssociatedServiceSplit))
            {
                InternalOrgId = internalOrgId,
                BackLink = Url.Action(nameof(OrderItemType), new { internalOrgId, orderType = CatalogueItemType.AssociatedService }),
                OrderType = orderType,
            };

            return View(model);
        }

        [HttpPost("determine-associated-service-type")]
        public IActionResult DetermineAssociatedServiceType(string internalOrgId, DetermineAssociatedServiceTypeModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            return RedirectToAction(
                nameof(SelectOrganisation),
                new { internalOrgId, orderType = model.OrderType.Value });
        }

        [HttpGet("proxy-select")]
        public async Task<IActionResult> SelectOrganisation(string internalOrgId, OrderType orderType)
        {
            if (!User.GetSecondaryOrganisationInternalIdentifiers().Any())
                return RedirectToAction(nameof(OrderController.ReadyToStart), typeof(OrderController).ControllerName(), new { internalOrgId, orderType = orderType.Value });

            var internalOrgIds = new List<string>(User.GetSecondaryOrganisationInternalIdentifiers())
            {
                User.GetPrimaryOrganisationInternalIdentifier(),
            };

            var organisations = await organisationsService.GetOrganisationsByInternalIdentifiers(internalOrgIds.ToArray());

            var model = new SelectOrganisationModel(internalOrgId, organisations)
            {
                BackLink = Url.Action(nameof(OrderItemType), new { internalOrgId, orderType = orderType.ToCatalogueItemType }),
                Title = "Which organisation are you ordering for?",
            };

            return View(model);
        }

        [HttpPost("proxy-select")]
        public IActionResult SelectOrganisation(string internalOrgId, SelectOrganisationModel model, OrderTypeEnum? orderType = null)
        {
            if (!User.GetSecondaryOrganisationInternalIdentifiers().Any())
                return RedirectToAction(nameof(OrderController.ReadyToStart), typeof(OrderController).ControllerName(), new { internalOrgId, orderType });

            if (!ModelState.IsValid)
                return View(model);

            return RedirectToAction(nameof(OrderController.ReadyToStart), typeof(OrderController).ControllerName(), new { internalOrgId = model.SelectedOrganisation, orderType });
        }
    }
}
