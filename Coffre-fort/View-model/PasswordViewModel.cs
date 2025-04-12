using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using Coffre_fort.View_model;

public class PasswordViewModel : INotifyPropertyChanged
{
    private readonly PasswordRepository _repository;
    private ObservableCollection<PasswordEntry> _passwords;
    private PasswordEntry _selectedEntry;
    private string _nomCompte;
    private string _motDePasse;
    private string _nouveauMotDePasse;
    private string _recherche;
    private bool _triAscendant = true;

    private List<PasswordEntry> _allPasswords;

    public ObservableCollection<PasswordEntry> Passwords
    {
        get => _passwords;
        set { _passwords = value; OnPropertyChanged(nameof(Passwords)); }
    }

    public PasswordEntry SelectedEntry
    {
        get => _selectedEntry;
        set { _selectedEntry = value; OnPropertyChanged(nameof(SelectedEntry)); }
    }

    public string NomCompte
    {
        get => _nomCompte;
        set { _nomCompte = value; OnPropertyChanged(nameof(NomCompte)); }
    }

    public string MotDePasse
    {
        get => _motDePasse;
        set { _motDePasse = value; OnPropertyChanged(nameof(MotDePasse)); }
    }

    public string NouveauMotDePasse
    {
        get => _nouveauMotDePasse;
        set { _nouveauMotDePasse = value; OnPropertyChanged(nameof(NouveauMotDePasse)); }
    }

    public string Recherche
    {
        get => _recherche;
        set { _recherche = value; OnPropertyChanged(nameof(Recherche)); AppliquerFiltrageEtTri(); }
    }

    public ICommand TogglePasswordVisibilityCommand { get; }
    public ICommand AddPasswordCommand { get; }
    public ICommand UpdatePasswordCommand { get; }
    public ICommand DeletePasswordCommand { get; }
    public ICommand SearchCommand { get; }
    public ICommand ToggleSortCommand { get; }

    public event PropertyChangedEventHandler PropertyChanged;

    public PasswordViewModel()
    {
        _repository = new PasswordRepository();

        TogglePasswordVisibilityCommand = new RelayCommand(param =>
        {
            if (param is PasswordEntry entry)
            {
                entry.AfficherMotDePasse = !entry.AfficherMotDePasse;
                RefreshPasswords();
            }
        });

        AddPasswordCommand = new RelayCommand(param =>
        {
            if (!string.IsNullOrWhiteSpace(NomCompte) && !string.IsNullOrWhiteSpace(MotDePasse))
            {
                AddPassword(NomCompte, MotDePasse);
                NomCompte = string.Empty;
                MotDePasse = string.Empty;
            }
        });

        UpdatePasswordCommand = new RelayCommand(param =>
        {
            if (SelectedEntry != null && !string.IsNullOrWhiteSpace(NouveauMotDePasse))
            {
                UpdatePassword(SelectedEntry, NouveauMotDePasse);
                NouveauMotDePasse = string.Empty;
            }
        });

        DeletePasswordCommand = new RelayCommand(param =>
        {
            if (param is PasswordEntry entry)
            {
                DeletePassword(entry);
            }
        });

        SearchCommand = new RelayCommand(param =>
        {
            AppliquerFiltrageEtTri();
        });

        ToggleSortCommand = new RelayCommand(param =>
        {
            _triAscendant = !_triAscendant;
            AppliquerFiltrageEtTri();
        });

        LoadPasswords();
    }

    public void AddPassword(string nomCompte, string motDePasse)
    {
        string motDePasseChiffre = SecurityHelper.Encrypt(motDePasse);
        _repository.AddPassword(nomCompte, motDePasseChiffre);
        LoadPasswords();
    }

    public void UpdatePassword(PasswordEntry entryToUpdate, string newPassword)
    {
        if (entryToUpdate == null || string.IsNullOrEmpty(newPassword))
            return;

        string motDePasseChiffre = SecurityHelper.Encrypt(newPassword);
        _repository.UpdatePassword(entryToUpdate.Id, motDePasseChiffre);
        LoadPasswords();
    }

    public void DeletePassword(PasswordEntry entry)
    {
        if (entry == null)
            return;

        _repository.DeletePassword(entry.Id);
        LoadPasswords();
    }

    private void LoadPasswords()
    {
        _allPasswords = _repository.GetAllPasswords();
        AppliquerFiltrageEtTri();
    }

    private void AppliquerFiltrageEtTri()
    {
        IEnumerable<PasswordEntry> resultat = _allPasswords;

        if (!string.IsNullOrWhiteSpace(Recherche))
        {
            resultat = resultat.Where(p => p.NomCompte.Contains(Recherche, StringComparison.OrdinalIgnoreCase));
        }

        resultat = _triAscendant
            ? resultat.OrderBy(p => p.NomCompte)
            : resultat.OrderByDescending(p => p.NomCompte);

        Passwords = new ObservableCollection<PasswordEntry>(resultat);
        OnPropertyChanged(nameof(Passwords));
    }

    public void RefreshPasswords()
    {
        AppliquerFiltrageEtTri();
    }

    protected void OnPropertyChanged(string propertyName)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
