using System.Collections.ObjectModel;
using System.ComponentModel;

public class PasswordViewModel : INotifyPropertyChanged
{
    private PasswordRepository _repository;
    private ObservableCollection<PasswordEntry> _passwords;
    public ObservableCollection<PasswordEntry> Passwords
    {
        get => _passwords;
        set { _passwords = value; OnPropertyChanged(nameof(Passwords)); }
    }

    public PasswordViewModel()
    {
        _repository = new PasswordRepository();
        LoadPasswords();
    }

    public void AddPassword(string nomCompte, string motDePasse)
    {
        _repository.AddPassword(nomCompte, motDePasse);
        LoadPasswords(); // Rafraîchir la liste
    }

    private void LoadPasswords()
    {
        Passwords = new ObservableCollection<PasswordEntry>(_repository.GetAllPasswords());
        OnPropertyChanged(nameof(Passwords));
    }

    public void SearchPasswords(string search)
    {
        Passwords = new ObservableCollection<PasswordEntry>(_repository.SearchPasswords(search));
        OnPropertyChanged(nameof(Passwords));
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
