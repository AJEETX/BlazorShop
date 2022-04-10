namespace BlazorShop.Web.Client.Pages.Account
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http.Json;
    using System.Threading.Tasks;
    using BlazorShop.Models.Addresses;
    using Infrastructure.Extensions;
    using Models.Identity;

    public partial class Address
    {
        private readonly AddressesRequestModel model = new AddressesRequestModel();

        private string email;
        private AddressesListingResponseModel address;

        public bool ShowErrors { get; set; }

        public IEnumerable<string> Errors { get; set; }

        protected override async Task OnInitializedAsync() => await this.LoadDataAsync();

        private async Task SubmitAsync()
        {
            var response = await this.Http.PutAsJsonAsync("api/address", this.model);

            if (response.IsSuccessStatusCode)
            {
                this.ShowErrors = false;

                await this.AuthService.Logout();

                this.ToastService.ShowSuccess("Your account settings has been changed successfully.\n Please login.");
                this.NavigationManager.NavigateTo("/account/login");
            }
            else
            {
                this.Errors = await response.Content.ReadFromJsonAsync<string[]>();
                this.ShowErrors = true;
            }
        }

        private async Task LoadDataAsync()
        {
            var state = await this.AuthState.GetAuthenticationStateAsync();
            var user = state.User;
            var addresses = await this.AddressesService.Mine();
            this.address = addresses.FirstOrDefault();

            this.email = user.GetEmail();
            this.model.Description = this.address.Description;
            this.model.City = this.address.City;
            this.model.State = this.address.State;
            this.model.Country = this.address.Country;
            this.model.PostalCode = this.address.PostalCode;
        }
    }
}
