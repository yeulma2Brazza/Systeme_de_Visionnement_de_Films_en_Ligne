using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Extensions.DependencyInjection;
using KasomaFlix.Application.UseCases.GestionAdmin;
using KasomaFlix.Presentation;
using KasomaFlix.Presentation.Services;

namespace KasomaFlix.Presentation.Views
{
    public partial class GestionTransactions : Page
    {
        public GestionTransactions()
        {
            InitializeComponent();
            Loaded += GestionTransactions_Loaded;
        }

        private async void GestionTransactions_Loaded(object sender, RoutedEventArgs e)
        {
            if (!UserSession.IsLoggedIn() || !UserSession.IsAdmin())
            {
                NavigationService.Navigate(new FormulaireConnexion());
                return;
            }

            await ChargerTransactionsAsync();
        }

        private async Task ChargerTransactionsAsync()
        {
            try
            {
                // Créer un scope pour isoler cette opération
                using (var scope = ServiceLocator.CreateScope())
                {
                    var obtenirToutesTransactionsUseCase = scope.ServiceProvider.GetRequiredService<ObtenirToutesTransactionsUseCase>();
                    var transactions = await obtenirToutesTransactionsUseCase.ExecuteAsync();
                    DgTransactions.ItemsSource = transactions;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du chargement des transactions : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void TableauBord_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new TableauBordAdmin());
        }

        private void Deconnexion_Click(object sender, RoutedEventArgs e)
        {
            UserSession.Logout();
            NavigationService.Navigate(new FormulaireConnexion());
        }
    }
}
