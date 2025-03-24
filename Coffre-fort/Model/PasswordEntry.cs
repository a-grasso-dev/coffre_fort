using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


public class PasswordEntry
{
    [Key]
    public int Id { get; set; }

    public string NomCompte { get; set; }
    public string MotDePasse { get; set; }
    public DateTime DateAjout { get; set; }

    public string MotDePasseDechiffre => SecurityHelper.Decrypt(MotDePasse);
}
