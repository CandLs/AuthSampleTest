using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Mvc;
using mvcCookieAuthSample.ViewModels;

namespace mvcCookieAuthSample.Controllers
{

    public class ConsentController : Controller
    {
        private readonly IClientStore _clientStore;
        private readonly IResourceStore _resourceStore;
        private readonly IIdentityServerInteractionService _identityServerInteractionService;

        public ConsentController(IClientStore clientStore, IResourceStore resourceStore, IIdentityServerInteractionService identityServerInteractionService)
        {
            _clientStore = clientStore;
            _resourceStore = resourceStore;
            _identityServerInteractionService = identityServerInteractionService;
        }

        private async Task<ConsentViewModel> BuildConsentViewModel(string returnUrl)
        {
            var request = await _identityServerInteractionService.GetAuthorizationContextAsync(returnUrl);
            if (request == null)
            {
                return null;
            }

            var client = await _clientStore.FindEnabledClientByIdAsync(request.ClientId);
            if (client == null)
            {

            }

            var resources = await _resourceStore.FindEnabledResourcesByScopeAsync(request.ScopesRequested);
            var vm = CreateConsentViewModel(request, client, resources);
            vm.ReturnUrl = returnUrl;
            return vm;
        }

        private ConsentViewModel CreateConsentViewModel(AuthorizationRequest request, Client client, Resources resources)
        {
            var vm = new ConsentViewModel();
            vm.ClientId = client.ClientId;
            vm.ClientName = client.ClientName;
            vm.ClientLogoUrl = client.LogoUri;
            vm.ClientUrl = client.ClientUri;
            vm.RemeberConsent = client.AllowRememberConsent;

            vm.IdentityScopes = resources.IdentityResources.Select(i => CreateScopeViewModel(i));
            vm.ResourceScopes = resources.ApiResources.SelectMany(i => i.Scopes).Select(x => CreateScopeViewModel(x));
            return vm;
        }

        private ScopeViewModel CreateScopeViewModel(IdentityResource identityResource)
        {
            return new ScopeViewModel()
            {
                Name = identityResource.Name,
                DisplayName = identityResource.DisplayName,
                Description = identityResource.Description,
                Checked = identityResource.Required,
                Required = identityResource.Required,
                Emphasize = identityResource.Emphasize
            };
        }

        private ScopeViewModel CreateScopeViewModel(Scope scope)
        {
            return new ScopeViewModel()
            {
                Name = scope.Name,
                DisplayName = scope.DisplayName,
                Description = scope.Description,
                Checked = scope.Required,
                Required = scope.Required,
                Emphasize = scope.Emphasize
            };
        }

        [HttpGet]
        public async Task<IActionResult> Index(string returnUrl)
        {
            var model = await BuildConsentViewModel(returnUrl);
            if (model == null)
            {

            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Index(InputConsentViewModel inputConsentViewModel)
        {
            ConsentResponse consentResponse = null;
            if (inputConsentViewModel.Button == "no")
            {
                consentResponse = ConsentResponse.Denied;
            }
            else if (inputConsentViewModel.Button == "yes")
            {
                if (inputConsentViewModel.ScopesConsented != null && inputConsentViewModel.ScopesConsented.Any())
                {
                    consentResponse = new ConsentResponse
                    {
                        RememberConsent = inputConsentViewModel.RemeberConsent,
                        ScopesConsented = inputConsentViewModel.ScopesConsented,

                    };
                }
            }

            if (consentResponse != null) {
                var request =await _identityServerInteractionService.GetAuthorizationContextAsync(inputConsentViewModel.ReturnUrl);
                await _identityServerInteractionService.GrantConsentAsync(request,consentResponse);
                Redirect(inputConsentViewModel.ReturnUrl);
            }

            return View();
        }
    }
}