using System;

public class PasswordEntry
{
    public string NomCompte { get; set; }
    public string MotDePasse { get; set; } // Stocke la version chiffrée
    public DateTime DateAjout { get; set; }

    public string MotDePasseDechiffre => SecurityHelper.Decrypt(MotDePasse);
}
