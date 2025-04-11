using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Coffre_fort.View_model;

public class PasswordEntry
{
    [Key]
    public int Id { get; set; }

    public required string NomCompte { get; set; }
    public required string MotDePasse { get; set; }
    public DateTime DateAjout { get; set; }

    [NotMapped]
    public string MotDePasseDechiffre => SecurityHelper.Decrypt(MotDePasse);
    public string MotDePasseMasque => new string('●', SecurityHelper.Decrypt(MotDePasse).Length);

    [NotMapped]
    public bool AfficherMotDePasse { get; set; } = false;

    [NotMapped]
    public string AffichageMotDePasse =>
        AfficherMotDePasse ? SecurityHelper.Decrypt(MotDePasse) : new string('●', SecurityHelper.Decrypt(MotDePasse).Length);


}
