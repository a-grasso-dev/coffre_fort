using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Coffre_fort.View_model;

public class PasswordEntry : INotifyPropertyChanged
{
    [Key]
    public int Id { get; set; }

    public string NomCompte { get; set; }
    public string MotDePasse { get; set; }
    public DateTime DateAjout { get; set; }

    private string _tags;
    
    public string Tags
    {
        get => _tags;
        set
        {
            _tags = value;
            OnPropertyChanged(nameof(Tags));
            OnPropertyChanged(nameof(TagsList));
        }
    }

    [NotMapped]
    private bool _afficherMotDePasse;
    [NotMapped]
    public bool AfficherMotDePasse
    {
        get => _afficherMotDePasse;
        set
        {
            _afficherMotDePasse = value;
            OnPropertyChanged(nameof(AfficherMotDePasse));
            OnPropertyChanged(nameof(AffichageMotDePasse));
        }
    }

    [NotMapped]
    private double _progression;
    [NotMapped]
    public double Progression
    {
        get => _progression;
        set
        {
            _progression = value;
            OnPropertyChanged(nameof(Progression));
        }
    }

    [NotMapped]
    private bool _estCopieEnCours;
    [NotMapped]
    public bool EstCopieEnCours
    {
        get => _estCopieEnCours;
        set
        {
            _estCopieEnCours = value;
            OnPropertyChanged(nameof(EstCopieEnCours));
        }
    }

    [NotMapped]
    public string AffichageMotDePasse =>
        AfficherMotDePasse ? SecurityHelper.Decrypt(MotDePasse) : "••••••••";

    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged(string name)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

    public List<string> TagsList => string.IsNullOrWhiteSpace(Tags)
    ? new List<string>()
    : Tags.Split(',').Select(t => t.Trim()).ToList();

}
