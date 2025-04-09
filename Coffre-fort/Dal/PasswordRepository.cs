using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

public class PasswordRepository
{
    public void AddPassword(string nomCompte, string motDePasse)
    {
        using (var db = new PasswordDbContext())
        {
            var entry = new PasswordEntry
            {
                NomCompte = nomCompte,
                MotDePasse = motDePasse,
                DateAjout = DateTime.Now
            };
            db.passwordentry.Add(entry);
            db.SaveChanges();
        }
    }

    public List<PasswordEntry> GetAllPasswords()
    {
        using (var db = new PasswordDbContext())
        {
            return db.passwordentry.ToList();
        }
    }

    public List<PasswordEntry> SearchPasswords(string search)
    {
        using (var db = new PasswordDbContext())
        {
            return db.passwordentry.Where(p => p.NomCompte.Contains(search)).ToList();
        }
    }

    public void UpdatePassword(int id, string newPassword)
    {
        using (var db = new PasswordDbContext())
        {
            var entry = db.passwordentry.FirstOrDefault(p => p.Id == id);
            if (entry != null)
            {
                entry.MotDePasse = newPassword;
                db.SaveChanges();
            }
        }
    }

    public void DeletePassword(int id)
    {
        using (var db = new PasswordDbContext())
        {
            var entry = db.passwordentry.FirstOrDefault(p => p.Id == id);
            if (entry != null)
            {
                db.passwordentry.Remove(entry);
                db.SaveChanges();
            }
        }
    }


}
