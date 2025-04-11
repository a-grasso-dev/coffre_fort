using System.Collections.ObjectModel;
using System.ComponentModel;
using Coffre_fort.View_model;

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
        string motDePasseChiffre = SecurityHelper.Encrypt(motDePasse);
        _repository.AddPassword(nomCompte, motDePasseChiffre);
        LoadPasswords();
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

    public void UpdatePassword(PasswordEntry entryToUpdate, string newPassword)
    {
        if (entryToUpdate == null || string.IsNullOrEmpty(newPassword))
            return;

        string motDePasseChiffre = SecurityHelper.Encrypt(newPassword);
        _repository.UpdatePassword(entryToUpdate.Id, motDePasseChiffre);

        LoadPasswords();
    }


    private PasswordEntry _selectedEntry;
    public PasswordEntry SelectedEntry
    {
        get => _selectedEntry;
        set { _selectedEntry = value; OnPropertyChanged(nameof(SelectedEntry)); }
    }

    public void DeletePassword(PasswordEntry entry)
    {
        if (entry == null)
            return;

        _repository.DeletePassword(entry.Id);
        LoadPasswords();
    }


}
