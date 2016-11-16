using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace Databas_Inlamning2
{
    class Program
    {
        static void Main(string[] args)
        {
            StartApp(); //(Jag gillar att starta appen i en egen metod så jag enkelt 
                        //kan starta om den utan att re-boota hela programmet.)
        }

        static void StartApp()
        {
            //(Jag sparar info i en egen klass 'NewEntry' så att den inte försvinner om jag 
            //hoppar fram och tillbaka mellan metoder. Mer för att vara användarvänligt 
            //än för att spara utrymme.)

            NewEntry n = new NewEntry();

            int option = 0;

            Console.WriteLine("\n" + "What would you like to do?" + "\n");
            Console.WriteLine("1. Add a new Customer");
            Console.WriteLine("2. Add a new Product");
            Console.WriteLine("3. Update a Product Price");
            Console.WriteLine("4. Display list of customers");
            Console.WriteLine("5. Display list of products" + "\n");
            option = int.Parse(Console.ReadLine());

            switch (option)
            {
                case 1:
                    Console.WriteLine("ADD A NEW CUSTOMER: " + "\n");
                    AddCustomer(1, n);
                    break;

                case 2:
                    Console.WriteLine("ADD A NEW PRODUCT: " + "\n");
                    AddProduct(1, n);
                    break;

                case 3:
                    Console.WriteLine("UPDATE A PRODUCT PRICE: " + "\n");
                    UpdatePrice(1, n);
                    break;

                case 4:
                    Console.WriteLine("LIST OF CUSTOMERS: " + "\n");
                    DisplayCustomers();
                    break;

                case 5:
                    Console.WriteLine("LIST OF PRODUCTS: " + "\n");
                    DisplayProducts();
                    break;

                default:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("Option is invalid. Please try again. \n");
                    Console.ForegroundColor = ConsoleColor.Gray;

                    StartApp();
                    break;
            }
        }

        //Variabeln "layer" indikerar ett steg i processen (om jag får ett felmeddelande
        //men vill kunna fortsätta där jag var).
        static void AddCustomer(int layer, NewEntry c)
        {
            switch (layer)
            {
                case 1: //Här skriver man in ContactID.
                    Console.Write("Enter the contact ID of the new contact: ");
                    string newContactID = Console.ReadLine();

                    if (newContactID.Length != 5)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("(Contact ID must be exactly 5 characters)");
                        Console.ForegroundColor = ConsoleColor.Gray;

                        ErrorMessage(1, c, 1);
                    }

                    else
                    {
                        c.CustomerID = newContactID;
                        AddCustomer(2, c);
                    }
                    break;

                case 2: //Om vi lyckats skriva in ID, är det dags för Namn...
                    Console.Write("Enter the name of the new contact: ");
                    string newContactName = Console.ReadLine();

                    if (newContactName.Length < 1)
                    {
                        ErrorMessage(2, c, 1);
                    }

                    else
                    {
                        c.Name = newContactName;
                        AddCustomer(3, c);
                    }
                    break;

                case 3: //Till slut skriver vi in Company Name
                    Console.Write("Enter the name of their company: ");
                    string newCompanyName = Console.ReadLine();

                    if (newCompanyName.Length < 1)
                    {
                        ErrorMessage(3, c, 1);
                    }

                    else
                    {
                        c.CompanyName = newCompanyName;
                        break;
                    }
                    break;
            }

            WriteToDb(c, 1);
        }

        static void AddProduct(int layer, NewEntry p)
        {
            switch (layer)
            {
                case 1: //Här skriver man in Product Name.
                    Console.Write("Enter the name of the product you want to add: ");
                    string newProductName = Console.ReadLine();

                    if (newProductName.Length < 1)
                    {
                        ErrorMessage(1, p, 2);
                    }

                    else
                    {
                        p.Name = newProductName;
                        AddProduct(2, p);
                    }
                    break;

                case 2:
                    Console.Write("Enter the unit price of that product: ");
                    decimal newProductPrice = decimal.Parse(Console.ReadLine());
                    if (newProductPrice <= 0)
                    {
                        ErrorMessage(2, p, 2);
                    }
                    else
                    {
                        p.Price = newProductPrice;
                        break;
                    }
                    break;
            }

            WriteToDb(p, 2);
        }

        static void UpdatePrice(int layer, NewEntry up)
        {
            switch (layer)
            {
                case 1:
                    Console.Write("Enter the ID of the unit you want to update: ");
                    int newUnitID = int.Parse(Console.ReadLine());

                    if (newUnitID < 1)
                    {
                        ErrorMessage(1, up, 3);
                    }
                    else
                    {
                        up.ProductID = newUnitID;
                        CheckProductID(up, newUnitID);
                    }
                    break;

                case 2:
                    Console.Write("Enter the new price of the unit you want to update: ");
                    decimal newUnitPrice = decimal.Parse(Console.ReadLine());

                    if (newUnitPrice < 0)
                    {
                        ErrorMessage(2, up, 3);
                    }
                    else
                    {
                        up.Price = newUnitPrice;
                        WriteToDb(up, 3);
                        break;
                    }
                    break;
            }
        }

        static void DisplayCustomers()
        {
            string conString = ConfigurationManager.ConnectionStrings["MyConString"].ConnectionString;
            SqlConnection cn = new SqlConnection(conString);
            cn.Open();
            SqlCommand cmd = cn.CreateCommand();

            cmd.CommandText = "select CustomerID, ContactName from Customers";
            cmd.ExecuteNonQuery();

            SqlDataReader rd = cmd.ExecuteReader();

            while (rd.Read()) //Hämtar både namn och id
            {
                Console.WriteLine("{0} " + "{1} ", rd.GetString(0), rd.GetString(1));
            }
            cn.Close();
            rd.Close();

            StartApp();
        }
        static void DisplayProducts()
        {
            string conString = ConfigurationManager.ConnectionStrings["MyConString"].ConnectionString;
            SqlConnection cn = new SqlConnection(conString);
            cn.Open();
            SqlCommand cmd = cn.CreateCommand();
            //Vi hämtar både produktnamn och ett pris konverterat till två decimaler
            cmd.CommandText = "select ProductID, ProductName, convert(decimal(5, 2), round(UnitPrice, 2)) from Products";
            cmd.ExecuteNonQuery();

            SqlDataReader rd = cmd.ExecuteReader();

            while (rd.Read())
            {
                string IDandName = rd.GetInt32(0).ToString() + " " + rd.GetString(1);                
                decimal price = rd.GetDecimal(2);
                              
                while (IDandName.Length < 35)
                {
                    IDandName += ".";
                }

                if (IDandName.Length >= 35)
                {
                    Console.WriteLine(IDandName + price);
                }
            }
            cn.Close();
            rd.Close();

            StartApp();
        }

        static void CheckProductID(NewEntry n, int testID)
        {
            string conString = ConfigurationManager.ConnectionStrings["MyConString"].ConnectionString;
            SqlConnection cn = new SqlConnection(conString);
            cn.Open();
            SqlCommand cmd = cn.CreateCommand();

            //Vi kollar om det angivna produktID't existerar i listan (Tack för hjälpen med 
            //formuleringen här, Mattias :))
            cmd.CommandText = "select count(*) from Products where ProductID = @ProductID";
            cmd.Parameters.AddWithValue("@ProductID", testID);

            int records = (int)cmd.ExecuteScalar();

            if (records == 0) //om ProduktID inte finns i databasen
            {
                Console.WriteLine("The Product ID you have given does not exist in the database.");
                Console.WriteLine("If you want to go back to the main menu, press '1'" + "\n" +
                        "If you want to try again, press '2'" + "\n");
                int entry = 0;

                entry = int.Parse(Console.ReadLine());

                if (entry == 2)
                {
                    UpdatePrice(1, n);
                }
                else StartApp();
            }

            else
            {
                UpdatePrice(2, n);
            }
            cn.Close();
        }
    

        //Dessa funktioner anropas ifall man angett felaktig information.
        //De håller även reda på i vilket skede av processen man gjorde detta, så att man kan komma
        //tillbaka dit man var. 
        //Terminologi:
        //layer = anger under vilket steg i varje metod vi skrev fel och låter oss komma tillbaka dit.
        //n = tar med sig klassen som lagrar egenskaperna.
        //i = anger vilken metod vi kom hit från.
        static void ErrorMessage(int layer, NewEntry n, int m)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Entry is invalid. Try again. ");
            Console.ForegroundColor = ConsoleColor.Gray;

            switch (m) //Anger vilken metod vi ska komma tillbaka till 
            {
                case 1: //(vi kom från AddCustomer)
                    AddCustomer(layer, n);
                    break;

                case 2: //(vi kom från AddProduct)
                    AddProduct(layer, n);
                    break;

                case 3: //(vi kom från UpdatePrice)
                    UpdatePrice(layer, n);
                    break;

                case 4: //(vi kom från DisplayProducts)
                    StartApp();
                    break;
            }
        }

        static void WriteToDb(NewEntry n, int Type) //"Type" avser vilken sorts info vi vill
            //överföra till dbn (1=customer, 2=produkt, 3=prisuppdatering).
        {
            //Här skriver vi in all info vi fått till databasen.
            string conString = ConfigurationManager.ConnectionStrings["MyConString"].ConnectionString;
            SqlConnection cn = new SqlConnection(conString);
            cn.Open();

            SqlCommand cmd = cn.CreateCommand();
            
            switch (Type)
            {
                case 1:  //Om vi fört in en ny kund
                    cmd.CommandText = "spInsertContact";
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@CustomerID", n.CustomerID);
                    cmd.Parameters.AddWithValue("@ContactName", n.Name);
                    cmd.Parameters.AddWithValue("@CompanyName", n.CompanyName);
                    break;

                case 2: //Om vi skrivit in en ny produkt
                    cmd.CommandText = "spInsertProduct";
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ProductName", n.Name);
                    cmd.Parameters.AddWithValue("@UnitPrice", n.Price);
                    break;

                case 3: //Eller om vi uppdaterat ett pris på produkt
                    cmd.CommandText = "UpdateUnitPrice";
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ProductID", n.ProductID);
                    cmd.Parameters.AddWithValue("@UnitPrice", n.Price);
                    break;
            }
            cmd.ExecuteNonQuery();
            cn.Close();

            SuccessResponse();
        }

        //Här meddelas till användaren att manövern är lyckad och sen återvänder vi
        //till huvudmenyn.
        static void SuccessResponse()
        {
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine("\n" + "The information was successfully saved on the database." + "\n");
            Console.ForegroundColor = ConsoleColor.Gray;
            StartApp();
        }
    }

    //Allt-i-allo-klass som innehåller den tillfälliga info som jag vill lagra under processen.
    public class NewEntry
    {
        public string CustomerID { get; set; }
        public int ProductID { get; set; }
        public string Name { get; set; }
        public string CompanyName { get; set; }
        public decimal Price { get; set; }   

        public void TryAgain() //Hade kunnat ha denna metod utanför klassen separat, 
            //men testade att ha den här istället. Används bara på ett ställe ändå.
        {
            
        }
    }
}

