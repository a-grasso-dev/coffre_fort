using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;

public class PasswordViewModel : INotifyPropertyChanged
{
    private ObservableCollection<PasswordEntry> _passwords;
    private string _searchTerm;

    public ObservableCollection<PasswordEntry> Passwords
    {
        get => _passwords;
        set { _passwords = value; OnPropertyChanged(nameof(Passwords)); }
    }

    public string SearchTerm
    {
        get => _searchTerm;
        set { _searchTerm = value; OnPropertyChanged(nameof(SearchTerm)); }
    }

    public ICommand SearchCommand { get; }
    public ICommand AddCommand { get; }

    public PasswordViewModel()
    {
        Passwords = new ObservableCollection<PasswordEntry>();
        SearchCommand = new RelayCommand(_ => SearchPasswords());
        AddCommand = new RelayCommand(_ => AddPassword("Exemple", "MonMotDePasse123"));
    }

    public void AddPassword(string nomCompte, string motDePasse)
    {
        string encryptedPassword = SecurityHelper.Encrypt(motDePasse);
        Passwords.Add(new PasswordEntry { NomCompte = nomCompte, MotDePasse = encryptedPassword, DateAjout = DateTime.Now });
    }

    public void SearchPasswords()
    {
        var filtered = _passwords.Where(p => p.NomCompte.Contains(SearchTerm)).ToList();
        Passwords.Clear();
        foreach (var entry in filtered)
        {
            Passwords.Add(entry);
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
