using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebApplication8.Models;
using System.Data.OleDb;
using System.Collections.Generic;
using static System.Reflection.Metadata.BlobBuilder;

namespace WebApplication8.Controllers
{
    public class HomeController : Controller
    {
        string connection =
             @"Provider=Microsoft.ACE.OLEDB.12.0;
    Data Source=C:\Users\hp\OneDrive\Documents\Library.accdb";
        public IActionResult Index(string search)
        {
            List<Book> list = new List<Book>();

            using (OleDbConnection con = new OleDbConnection(connection))
            {
                con.Open();

                string query = "SELECT * FROM library";

                if (!string.IsNullOrEmpty(search))
                {
                    query = "SELECT * FROM library WHERE BookName LIKE ?";
                }

                OleDbCommand cmd = new OleDbCommand(query, con);

                if (!string.IsNullOrEmpty(search))
                {
                    cmd.Parameters.AddWithValue("?", "%" + search + "%");
                }

                OleDbDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    list.Add(new Book
                    {
                        Id = (int)dr["ID"],
                        BookName = dr["BookName"].ToString(),
                        ISBN = dr["ISBN"].ToString(),
                        Author = dr["Author"].ToString(),
                        ShelfNo = dr["ShelfNo"].ToString()
                    });
                }
            }

            return View(list);
        }
     
        public IActionResult Edit(int id)
        {
            Book book = new Book();

            using (OleDbConnection con = new OleDbConnection(connection))
            {
                con.Open();

                string query = "SELECT * FROM library WHERE ID = ?";

                OleDbCommand cmd = new OleDbCommand(query, con);
                cmd.Parameters.AddWithValue("?", id);

                OleDbDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    book.Id = (int)dr["ID"];
                    book.BookName = dr["BookName"].ToString();
                    book.ISBN = dr["ISBN"].ToString();
                    book.Author = dr["Author"].ToString();
                    book.ShelfNo = dr["ShelfNo"].ToString();
                }
            }

            return View(book);
        }

        [HttpPost]
        public IActionResult Edit(Book book)
        {
            using (OleDbConnection con = new OleDbConnection(connection))
            {
                con.Open();

                string query = @"UPDATE library
                         SET BookName = ?,
                             ISBN = ?,
                             Author = ?,
                             ShelfNo = ?
                         WHERE ID = ?";

                OleDbCommand cmd = new OleDbCommand(query, con);

                cmd.Parameters.AddWithValue("?", book.BookName);
                cmd.Parameters.AddWithValue("?", book.ISBN);
                cmd.Parameters.AddWithValue("?", book.Author);
                cmd.Parameters.AddWithValue("?", book.ShelfNo);
                cmd.Parameters.AddWithValue("?", book.Id);

                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Index");
        }
        public IActionResult Delete(string isbn)
        {
            using (OleDbConnection con = new OleDbConnection(connection))
            {
                con.Open();

                string query = "DELETE FROM library WHERE ISBN = ?";

                OleDbCommand cmd = new OleDbCommand(query, con);

                cmd.Parameters.AddWithValue("?", isbn);

                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Index");
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
