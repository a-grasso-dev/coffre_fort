using System.ComponentModel.DataAnnotations;
using Coffre_fort.View_model;

public class PasswordEntry
{
    [Key]
    public int Id { get; set; }

    public required string NomCompte { get; set; }
    public required string MotDePasse { get; set; }
    public DateTime DateAjout { get; set; }

    public string MotDePasseDechiffre => SecurityHelper.Decrypt(MotDePasse);
    public string MotDePasseMasque => new string('●', SecurityHelper.Decrypt(MotDePasse).Length);

}
