﻿// ViewModel: ajout pour l'export CSV, XML ou Excel
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Coffre_fort.View_model;
using Coffre_fort.Utils;

public class PasswordViewModel : INotifyPropertyChanged
{
    private readonly PasswordRepository _repository;
    private ObservableCollection<PasswordEntry> _passwords;
    private List<PasswordEntry> _allPasswords = new();
    private PasswordEntry _selectedEntry;
    private string _nomCompte;
    private string _motDePasse;
    private string _nouveauMotDePasse;
    private string _recherche;
    private bool _triAscendant = true;
    private string _tags;

    private string _selectedExportFormat = "CSV";
    public string SelectedExportFormat
    {
        get => _selectedExportFormat;
        set
        {
            _selectedExportFormat = value;
            OnPropertyChanged(nameof(SelectedExportFormat));
        }
    }

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

    public string Tags
    {
        get => _tags;
        set { _tags = value; OnPropertyChanged(nameof(Tags)); }
    }

    public ICommand TogglePasswordVisibilityCommand { get; }
    public ICommand AddPasswordCommand { get; }
    public ICommand UpdatePasswordCommand { get; }
    public ICommand UpdateTagsCommand { get; }
    public ICommand DeletePasswordCommand { get; }
    public ICommand ToggleSortCommand { get; }
    public ICommand CopierMotDePasseCommand { get; }
    public ICommand CopierIdentifiantCommand { get; }
    public ICommand RemoveTagCommand { get; }
    public ICommand ExportCommand { get; }

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

        AddPasswordCommand = new RelayCommand(_ =>
        {
            if (!string.IsNullOrWhiteSpace(NomCompte) && !string.IsNullOrWhiteSpace(MotDePasse))
            {
                AddPassword(NomCompte, MotDePasse);
                NomCompte = string.Empty;
                MotDePasse = string.Empty;
            }
        });

        UpdatePasswordCommand = new RelayCommand(_ =>
        {
            if (SelectedEntry != null && !string.IsNullOrWhiteSpace(NouveauMotDePasse))
            {
                UpdatePassword(SelectedEntry, NouveauMotDePasse);
                NouveauMotDePasse = string.Empty;
            }
        });

        UpdateTagsCommand = new RelayCommand(_ =>
        {
            if (SelectedEntry != null)
            {
                _repository.UpdateTags(SelectedEntry.Id, SelectedEntry.Tags);
                LoadPasswords();
            }
        });

        DeletePasswordCommand = new RelayCommand(param =>
        {
            if (param is PasswordEntry entry) DeletePassword(entry);
        });

        ToggleSortCommand = new RelayCommand(_ =>
        {
            _triAscendant = !_triAscendant;
            AppliquerFiltrageEtTri();
        });

        CopierMotDePasseCommand = new RelayCommand(async param =>
        {
            if (param is not PasswordEntry entry) return;

            string motClair = SecurityHelper.Decrypt(entry.MotDePasse);
            Clipboard.SetText(motClair);

            entry.EstCopieEnCours = true;
            entry.Progression = 100;
            RefreshPasswords();

            for (int i = 0; i <= 120; i++)
            {
                await Task.Delay(100);
                entry.Progression = 100 - i * (100.0 / 120);
                RefreshPasswords();
            }

            Clipboard.Clear();
            entry.EstCopieEnCours = false;
            RefreshPasswords();
        });

        CopierIdentifiantCommand = new RelayCommand(param =>
        {
            if (param is PasswordEntry entry)
            {
                Clipboard.SetText(entry.NomCompte);
            }
        });

        RemoveTagCommand = new RelayCommand(param =>
        {
            if (param is Tuple<PasswordEntry, string> tuple)
            {
                var entry = tuple.Item1;
                var tagToRemove = tuple.Item2;

                if (entry != null && !string.IsNullOrEmpty(tagToRemove))
                {
                    var tags = entry.Tags?.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                         .Select(t => t.Trim())
                                         .Where(t => !string.Equals(t, tagToRemove, StringComparison.OrdinalIgnoreCase))
                                         .ToList();

                    entry.Tags = string.Join(", ", tags);
                    _repository.UpdateTags(entry.Id, entry.Tags);
                    RefreshPasswords();
                }
            }
        });

        ExportCommand = new RelayCommand(_ => PasswordExporter.Export(Passwords, SelectedExportFormat));

        LoadPasswords();
    }

    public void AddPassword(string nom, string mdp)
    {
        string mdpChiffre = SecurityHelper.Encrypt(mdp);
        _repository.AddPassword(nom, mdpChiffre, Tags);
        LoadPasswords();
    }

    public void UpdatePassword(PasswordEntry entry, string nouveauMdpClair)
    {
        if (entry == null) return;
        string mdpChiffre = SecurityHelper.Encrypt(nouveauMdpClair);
        _repository.UpdatePassword(entry.Id, mdpChiffre);
        LoadPasswords();
    }

    public void DeletePassword(PasswordEntry entry)
    {
        if (entry == null) return;
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
            resultat = resultat.Where(p =>
                p.NomCompte.Contains(Recherche, StringComparison.OrdinalIgnoreCase) ||
                (p.Tags != null && p.Tags.Contains(Recherche, StringComparison.OrdinalIgnoreCase)));
        }

        resultat = _triAscendant
            ? resultat.OrderBy(p => p.NomCompte)
            : resultat.OrderByDescending(p => p.NomCompte);

        Passwords = new ObservableCollection<PasswordEntry>(resultat);
    }

    public void SupprimerTag(PasswordEntry entry, string tagASupprimer)
    {
        if (entry == null || string.IsNullOrWhiteSpace(entry.Tags)) return;

        var tags = entry.Tags.Split(',', StringSplitOptions.RemoveEmptyEntries)
                             .Select(t => t.Trim())
                             .Where(t => !string.Equals(t, tagASupprimer, StringComparison.OrdinalIgnoreCase))
                             .ToList();

        entry.Tags = string.Join(", ", tags);
        _repository.UpdateTags(entry.Id, entry.Tags);
        RefreshPasswords();
    }

    public void RefreshPasswords() => OnPropertyChanged(nameof(Passwords));

    private void OnPropertyChanged(string propertyName)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
