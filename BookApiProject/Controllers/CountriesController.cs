using BookApiProject.Dtos;
using BookApiProject.Models;
using BookApiProject.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookApiProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CountriesController : Controller
    {
        private ICountryRepository _countryRepository;
        private IAuthorRepository _authorRepository;

        public CountriesController(ICountryRepository countryRepository, IAuthorRepository authorRepository)
        {
            _countryRepository = countryRepository;
            _authorRepository = authorRepository;
        }

        //api/countries
        [HttpGet]
        [ProducesResponseType(400)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<CountryDto>))]
        public IActionResult GetCountries()
        {
            var countries = _countryRepository.GetCountries();
            
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var countriesDto = new List<CountryDto>();
            foreach(var country in countries)
            {
                countriesDto.Add(new CountryDto
                {
                    Id = country.Id,
                    Name = country.Name
                });
            }

            return Ok(countriesDto);
        }

        //api/countries/countryId
        [HttpGet("{countryId}", Name ="GetCountry")]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200, Type = typeof(CountryDto))]
        public IActionResult GetCountry(int countryId)
        {
            if (!_countryRepository.CountryExists(countryId))
                return NotFound();

            var country = _countryRepository.GetCountry(countryId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var countryDto = new CountryDto()
            {
                Id = country.Id,
                Name = country.Name
            };            

            return Ok(countryDto);
        }

        //api/countries/authors/authorId
        [HttpGet("authors/{authorId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200, Type = typeof(CountryDto))]
        public IActionResult GetCountryOfAnAuthor(int authorId)
        {
            if (!_authorRepository.AuthorExists(authorId))
                return NotFound();

            var country = _countryRepository.GetCountryOfAnAuthor(authorId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var countryDto = new CountryDto()
            {
                Id = country.Id,
                Name = country.Name
            };

            return Ok(countryDto);
        }

        //api/countries/countryId/authors
        [HttpGet("{countryId}/authors")]
        [ProducesResponseType(200, Type = typeof (IEnumerable<AuthorDto>))]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public IActionResult GetAuthorsFromACountry(int countryId)
        {
            if (!_countryRepository.CountryExists(countryId))
                return NotFound();

            var authors = _countryRepository.GetAuthorsFromACountry(countryId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var authorsDto = new List<AuthorDto>();

            foreach(var author in authors)
            {
                authorsDto.Add(new AuthorDto
                {
                    Id = author.Id,
                    FirstName = author.FirstName,
                    LastName = author.LastName
                });
            }

            return Ok(authorsDto);
        }

        //api/countries
        [HttpPost]
        [ProducesResponseType(201, Type = typeof(Country))]
        [ProducesResponseType(400)]
        [ProducesResponseType(422)]
        [ProducesResponseType(500)]
        public IActionResult CreateCountry([FromBody]Country countryToCreate)
        {
            if (countryToCreate == null)
                return BadRequest(ModelState);

            var countries = _countryRepository.GetCountries().Select(c => c.Name);

            if (countries.Contains(countryToCreate.Name.Trim(), StringComparer.OrdinalIgnoreCase))
            {
                ModelState.AddModelError("", $"Country {countryToCreate} already exists");
                return StatusCode(422);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_countryRepository.CreateCountry(countryToCreate))
                return StatusCode(500, $"Something went terribly wrong when saving {countryToCreate.Name}");

            return CreatedAtRoute(nameof(CountriesController.GetCountry),
                                new { countryId = countryToCreate.Id }, countryToCreate);
        }

        //api/countries/countryId
        [HttpPut("{countryId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(422)]
        [ProducesResponseType(500)]
        public IActionResult UpdateCountry(int countryId, [FromBody] Country countryToUpdate)
        {
            if (countryToUpdate == null)
                return BadRequest(ModelState);

            if (!_countryRepository.CountryExists(countryId))
                return NotFound();

            if (countryId != countryToUpdate.Id)
                return BadRequest(ModelState);

            if (_countryRepository.IsDuplicateCountryName(countryId, countryToUpdate.Name))
            {
                ModelState.AddModelError("", $"Country {countryToUpdate} already exists");
                return StatusCode(422);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_countryRepository.UpdateCountry(countryToUpdate))
                return StatusCode(500, $"Something went terribly wrong when saving {countryToUpdate.Name}");

            return NoContent();
        }
    }
}
