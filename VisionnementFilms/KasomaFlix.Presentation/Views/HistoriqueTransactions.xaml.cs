using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Navigation;
using Microsoft.Extensions.DependencyInjection;
using KasomaFlix.Application.DTOs;
using KasomaFlix.Application.UseCases.GestionTransactions;
using KasomaFlix.Presentation;
using KasomaFlix.Presentation.Services;

namespace KasomaFlix.Presentation.Views
{
    /// <summary>
    /// Converter pour déterminer la couleur du montant (vert si positif, rouge si négatif)
    /// </summary>
    public class MontantToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is decimal montant)
            {
                return montant >= 0 
                    ? new SolidColorBrush(Color.FromRgb(0, 255, 0)) // Vert
                    : new SolidColorBrush(Color.FromRgb(255, 107, 107)); // Rouge
            }
            return new SolidColorBrush(Colors.White);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public partial class HistoriqueTransactions : Page
    {
        public HistoriqueTransactions()
        {
            InitializeComponent();
            Loaded += HistoriqueTransactions_Loaded;
        }

        private async void HistoriqueTransactions_Loaded(object sender, RoutedEventArgs e)
        {
            if (!UserSession.IsLoggedIn() || !UserSession.GetUserId().HasValue)
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
                var userId = UserSession.GetUserId().Value;
                // Créer un scope pour isoler cette opération
                using (var scope = ServiceLocator.CreateScope())
                {
                    var obtenirTransactionsUseCase = scope.ServiceProvider.GetRequiredService<ObtenirTransactionsUseCase>();
                    var transactions = await obtenirTransactionsUseCase.ExecuteAsync(userId);
                    DgTransactions.ItemsSource = transactions;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du chargement des transactions : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Accueil_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AccueilCatalogue());
        }

        private void MonCompte_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new MonCompte());
        }
    }
}
