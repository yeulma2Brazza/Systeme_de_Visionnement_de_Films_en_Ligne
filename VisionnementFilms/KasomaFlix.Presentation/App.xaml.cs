using System.Windows;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using KasomaFlix.Infrastructure.Data;
using KasomaFlix.Domain.Interfaces;
using KasomaFlix.Infrastructure.Data.Repositories;
using KasomaFlix.Application.UseCases.Inscription;
using KasomaFlix.Application.UseCases.Connexion;
using KasomaFlix.Application.UseCases.GestionAdmin;
using KasomaFlix.Application.UseCases.RechercheFilms;
using KasomaFlix.Application.UseCases.ConsultationFilm;
using KasomaFlix.Application.UseCases.GestionProfil;
using KasomaFlix.Application.UseCases.GestionTransactions;
using KasomaFlix.Application.UseCases.GestionAbonnements;
using KasomaFlix.Application.UseCases.GestionSessions;
using KasomaFlix.Application.UseCases.GestionPaiements;

namespace KasomaFlix.Presentation
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
        private ServiceProvider? _serviceProvider;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Configuration
            var basePath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) 
                ?? System.IO.Directory.GetCurrentDirectory();
            
            var configuration = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            // Configuration des services
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection, configuration);
            _serviceProvider = serviceCollection.BuildServiceProvider();

            // Vérifier la connexion à la base de données
            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<VisionnementFilmsDbContext>();
                    // Tester la connexion (la base de données a déjà été créée manuellement)
                    var canConnect = dbContext.Database.CanConnect();
                    if (!canConnect)
                    {
                        MessageBox.Show(
                            "Impossible de se connecter à la base de données.\n\n" +
                            "Vérifiez que la base de données VisionnementFilmsDB existe et que la connection string est correcte.",
                            "Erreur de Connexion",
                            MessageBoxButton.OK,
                            MessageBoxImage.Warning);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Erreur lors de la connexion à la base de données : {ex.Message}\n\n" +
                    "Vérifiez que SQL Server LocalDB est démarré et que la base de données existe.",
                    "Erreur de Base de Données",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }

            // Stocker le service provider pour utilisation globale
            ServiceLocator.SetServiceProvider(_serviceProvider);


            // Lancer la fenêtre principale
            var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }

        private void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            // Entity Framework
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<VisionnementFilmsDbContext>(options =>
                options.UseSqlServer(connectionString));

            // Repositories
            services.AddScoped<IMembreRepository, MembreRepository>();
            services.AddScoped<IFilmRepository, FilmRepository>();
            services.AddScoped<IAdministrateurRepository, AdministrateurRepository>();
            services.AddScoped<IAbonnementRepository, AbonnementRepository>();
            services.AddScoped<ITransactionRepository, TransactionRepository>();
            services.AddScoped<ISessionRepository, SessionRepository>();
            services.AddScoped<ICoteRepository, CoteRepository>();

            // UseCases
            services.AddScoped<InscriptionMembreUseCase>();
            services.AddScoped<ConnexionUseCase>();
            services.AddScoped<AjouterFilmUseCase>();
            services.AddScoped<ModifierFilmUseCase>();
            services.AddScoped<SupprimerFilmUseCase>();
            services.AddScoped<RechercherFilmsUseCase>();
            services.AddScoped<ConsulterFilmUseCase>();
            services.AddScoped<ModifierProfilUseCase>();
            services.AddScoped<ObtenirProfilUseCase>();
            services.AddScoped<CreerTransactionUseCase>();
            services.AddScoped<ObtenirTransactionsUseCase>();
            services.AddScoped<ObtenirAbonnementsUseCase>();
            services.AddScoped<CreerAbonnementUseCase>();
            services.AddScoped<AnnulerRenouvellementUseCase>();
            services.AddScoped<ObtenirStatistiquesAdminUseCase>();
            services.AddScoped<ObtenirTousMembresUseCase>();
            services.AddScoped<ObtenirToutesTransactionsUseCase>();
            services.AddScoped<CreerSessionUseCase>();
            services.AddScoped<TerminerSessionUseCase>();
            services.AddScoped<AjouterCarteCreditUseCase>();
            services.AddScoped<ObtenirCartesCreditUseCase>();

            // Fenêtre principale
            services.AddTransient<MainWindow>();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _serviceProvider?.Dispose();
            base.OnExit(e);
        }
    }

    /// <summary>
    /// Service locator simple pour accéder au service provider
    /// </summary>
    public static class ServiceLocator
    {
        private static ServiceProvider? _serviceProvider;

        public static void SetServiceProvider(ServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public static T GetService<T>() where T : class
        {
            return _serviceProvider?.GetRequiredService<T>() 
                ?? throw new InvalidOperationException("ServiceProvider n'est pas initialisé");
        }

        /// <summary>
        /// Crée un nouveau scope pour isoler les opérations DbContext
        /// </summary>
        public static IServiceScope CreateScope()
        {
            return _serviceProvider?.CreateScope() 
                ?? throw new InvalidOperationException("ServiceProvider n'est pas initialisé");
        }
    }
}
