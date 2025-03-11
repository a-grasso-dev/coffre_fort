using System;
using System.ComponentModel.DataAnnotations;
using System.Windows.Input;

public class PasswordEntry
{
    [Key]
    public string NomCompte { get; set; }
    public string MotDePasse { get; set; }
    public DateTime DateAjout { get; set; }

    public string MotDePasseDechiffre => SecurityHelper.Decrypt(MotDePasse);
}
