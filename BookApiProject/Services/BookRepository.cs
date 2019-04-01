using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookApiProject.Models;

namespace BookApiProject.Services
{
    public class BookRepository : IBookRepository
    {
        private BookDbContext _bookDbContext;

        public BookRepository(BookDbContext bookDbContext)
        {
            _bookDbContext = bookDbContext;
        }

        public bool BookExists(int bookId)
        {
            return _bookDbContext.Books.Any(b => b.Id == bookId);
        }

        public bool BookExists(string bookIsbn)
        {
            return _bookDbContext.Books.Any(b => b.Isbn == bookIsbn);
        }

        public bool CreateBook(List<int> authorsId, List<int> categoriesId, Book book)
        {
            var authors = _bookDbContext.Authors.Where(a => authorsId.Contains(a.Id)).ToList();
            var categories = _bookDbContext.Categories.Where(c => categoriesId.Contains(c.Id)).ToList();

            foreach(var author in authors)
            {
                var bookAuthor = new BookAuthor()
                {
                    Author = author,
                    Book = book
                };
                _bookDbContext.Add(bookAuthor);
            }

            foreach (var category in categories)
            {
                var bookCategory = new BookCategory()
                {
                    Category = category,
                    Book = book
                };
                _bookDbContext.Add(bookCategory);
            }

            _bookDbContext.Add(book);

            return Save();
        }

        public bool DeleteBook(Book book)
        {
            _bookDbContext.Remove(book);
            return Save();
        }

        public Book GetBook(int bookId)
        {
            return _bookDbContext.Books.Where(b => b.Id == bookId).FirstOrDefault();
        }

        public Book GetBook(string bookIsbn)
        {
            return _bookDbContext.Books.Where(b => b.Isbn == bookIsbn).FirstOrDefault();
        }

        public decimal GetBookRating(int bookId)
        {
            var reviews = _bookDbContext.Reviews.Where(r => r.Book.Id == bookId);

            if (reviews.Count() <= 0)
                return 0;

            return ((decimal)reviews.Sum(r => r.Rating) / reviews.Count());
        }

        public ICollection<Book> GetBooks()
        {
            return _bookDbContext.Books.OrderBy(b => b.Title).ToList();
        }

        public bool IsDuplicateIsbn(int bookId, string bookIsbn)
        {
            var book = _bookDbContext.Books.Where(b => b.Isbn.Trim().ToUpper() == bookIsbn.Trim().ToUpper() 
                                                && b.Id != bookId).FirstOrDefault();

            return book == null ? false : true;
        }

        public bool Save()
        {
            var saved = _bookDbContext.SaveChanges();
            return saved >= 0 ? true : false;
        }

        public bool UpdateBook(List<int> authorsId, List<int> categoriesId, Book book)
        {
            var authors = _bookDbContext.Authors.Where(a => authorsId.Contains(a.Id)).ToList();
            var categories = _bookDbContext.Categories.Where(c => categoriesId.Contains(c.Id)).ToList();

            var bookAuthorsToDelete = _bookDbContext.BookAuthors.Where(b=> b.BookId == book.Id);
            var bookCategoriesToDelete = _bookDbContext.BookCategories.Where(b => b.BookId == book.Id);

            _bookDbContext.RemoveRange(bookAuthorsToDelete);
            _bookDbContext.RemoveRange(bookCategoriesToDelete);

            foreach (var author in authors)
            {
                var bookAuthor = new BookAuthor()
                {
                    Author = author,
                    Book = book
                };
                _bookDbContext.Add(bookAuthor);
            }

            foreach (var category in categories)
            {
                var bookCategory = new BookCategory()
                {
                    Category = category,
                    Book = book
                };
                _bookDbContext.Add(bookCategory);
            }

            _bookDbContext.Update(book);

            return Save();
        }
    }
}
