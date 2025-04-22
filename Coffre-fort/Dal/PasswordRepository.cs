using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

public class PasswordRepository
{
    public void AddPassword(string nomCompte, string motDePasse, string tags)
    {
        using (var db = new PasswordDbContext())
        {
            var entry = new PasswordEntry
            {
                NomCompte = nomCompte,
                MotDePasse = motDePasse,
                DateAjout = DateTime.Now,
                Tags = tags
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

    public void UpdatePassword(int id, string newEncryptedPassword)
    {
        using (var db = new PasswordDbContext())
        {
            var entry = db.passwordentry.FirstOrDefault(p => p.Id == id);
            if (entry != null)
            {
                entry.MotDePasse = newEncryptedPassword;
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
    public void UpdateTags(int id, string newTags)
    {
        using (var db = new PasswordDbContext())
        {
            var entry = db.passwordentry.FirstOrDefault(p => p.Id == id);
            if (entry != null)
            {
                entry.Tags = newTags;
                db.SaveChanges();
            }
        }
    }
}
