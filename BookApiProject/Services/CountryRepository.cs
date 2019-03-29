using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookApiProject.Models;

namespace BookApiProject.Services
{
    public class CountryRepository : ICountryRepository
    {
        private BookDbContext _countryContext;

        public CountryRepository(BookDbContext countryContext)
        {
            _countryContext = countryContext;
        }

        public bool CountryExists(int countryId)
        {
            return _countryContext.Countries.Any(c => c.Id == countryId);
        }

        public bool CreateCountry(Country country)
        {
            _countryContext.AddAsync(country);
            return Save();
        }

        public bool DeleteCountry(Country country)
        {
            _countryContext.Remove(country);
            return Save();
        }

        public ICollection<Author> GetAuthorsFromACountry(int countryId)
        {
            return _countryContext.Authors.Where(c => c.Country.Id == countryId).ToList();
        }

        public ICollection<Country> GetCountries()
        {
            return _countryContext.Countries.OrderBy(c => c.Name).ToList();
        }

        public Country GetCountry(int countryId)
        {
            return _countryContext.Countries.Where(c => c.Id == countryId).FirstOrDefault();
        }

        public Country GetCountryOfAnAuthor(int authorId)
        {
            return _countryContext.Authors.Where(a => a.Id == authorId).Select(c => c.Country).FirstOrDefault();
        }

        public bool IsDuplicateCountryName(int countryId, string countryName)
        {
            var countries = _countryContext.Countries.Where(c => c.Name == countryName);

            foreach (var country in countries)
            {
                if (country.Id != countryId) //two countries with the same name
                {
                    _countryContext.Entry(country).State = Microsoft.EntityFrameworkCore.EntityState.Detached;
                    return true;
                }
                _countryContext.Entry(country).State = Microsoft.EntityFrameworkCore.EntityState.Detached;
            }

            return false;
        }

        public bool Save()
        {
            var saved = _countryContext.SaveChanges();
            return saved >= 0 ? true : false;
        }

        public bool UpdateCountry(Country country)
        {
            _countryContext.Update(country);
            return Save();
        }
    }
}
