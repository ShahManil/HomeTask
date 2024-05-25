using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using System.Xml.Linq;

namespace HomeExercise
{
    public class Admin
    {
         public static void getAllUser(SqlConnection conn)
        {
            SqlCommand cmd = new SqlCommand("getAllUsers", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            SqlDataReader data = cmd.ExecuteReader();
            int fields = data.FieldCount;
            while (data.Read())
            {
                for (int i = 0; i < fields; i++)
                {
                    Console.Write("{0} : {1}\t", data.GetName(i), data[data.GetName(i)]);
                }
                Console.Write("\n");
                //Console.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\n", data["first_name"], data["last_name"], data["age"], data["gender"], data["phone_no"], data["email"]);
            }
            data.Close();
        }

        public static void updateUser(SqlConnection conn, int empId, string new_name, string new_surname, int new_age, char new_gender, string new_no, string new_email, string up_pass)
        {
            SqlCommand updateCmd = new SqlCommand("updateUser", conn);
            updateCmd.CommandType = CommandType.StoredProcedure;
            updateCmd.Parameters.AddWithValue("@emp_id", empId);
            updateCmd.Parameters.AddWithValue("@new_name", new_name);
            updateCmd.Parameters.AddWithValue("@new_surname", new_surname);
            updateCmd.Parameters.AddWithValue("@new_age", new_age);
            updateCmd.Parameters.AddWithValue("@new_gender", new_gender);
            updateCmd.Parameters.AddWithValue("@new_no", new_no);
            updateCmd.Parameters.AddWithValue("@new_email", new_email);
            updateCmd.Parameters.AddWithValue("@new_pass", up_pass);
            int updateResult = updateCmd.ExecuteNonQuery();
        }

        public static void addUser(SqlConnection conn, string add_name, string add_surname, int add_age, char add_gender, string add_no, string add_email, string role, string new_pass)
        {
            SqlCommand addCmd = new SqlCommand("addUser", conn);
            addCmd.CommandType = CommandType.StoredProcedure;
            addCmd.Parameters.AddWithValue("@new_name", add_name);
            addCmd.Parameters.AddWithValue("@new_surname", add_surname);
            addCmd.Parameters.AddWithValue("@new_age", add_age);
            addCmd.Parameters.AddWithValue("@new_gender", add_gender);
            addCmd.Parameters.AddWithValue("@new_no", add_no);
            addCmd.Parameters.AddWithValue("@new_email", add_email);
            addCmd.Parameters.AddWithValue("@emp_role", role);
            addCmd.Parameters.AddWithValue("@pass", new_pass);

            int updateResult = addCmd.ExecuteNonQuery();
        }
    }

    public class User
    {
        public static void getUser(SqlConnection conn, int id)
        {
            SqlCommand getOne = new SqlCommand("getEmpById", conn);
            getOne.CommandType = CommandType.StoredProcedure;
            getOne.Parameters.AddWithValue("@emp_id", id);
            SqlDataReader oneData = getOne.ExecuteReader();
            int fields = oneData.FieldCount;
            while (oneData.Read())
            {
                for (int i = 0; i < fields; i++)
                {
                    Console.Write("{0} : {1}\t", oneData.GetName(i), oneData[oneData.GetName(i)]);
                }
                Console.Write("\n");
            }
            oneData.Close();
        }

        public static void updatePass(SqlConnection conn, int id, string new_pass)
        {
            SqlCommand cmd = new SqlCommand("changePass", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@new_pass", new_pass);

            int result = cmd.ExecuteNonQuery();
        }
    }

    public class Credential
    {
        public bool autorised { get; set; }
        public string role { get; set; }
    }

    public class Permission
    {
        public static Credential getRole(SqlConnection conn, int emp_id, string password)
        {
            bool auth = false;
            SqlCommand cmd = new SqlCommand("select role, password from permissions where emp_id = @emp_id", conn);
            cmd.Parameters.AddWithValue("@emp_id", emp_id);
            SqlDataReader rd = cmd.ExecuteReader();
            string role="";
            while(rd.Read())
            {
                if (password == Convert.ToString(rd["password"]))
                {
                    auth = true;
                }
                role = Convert.ToString(rd["role"]);
            }
            rd.Close();

            return new Credential { autorised = auth, role = role };
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            SqlConnection conn = null;
            string cs = ConfigurationManager.ConnectionStrings["dbcs"].ConnectionString;

            try {
                conn = new SqlConnection(cs);
            }
            catch {
                Console.WriteLine("Sorry but we are having difficulties accesing the database!");
            }

            Console.Write("Enter Your Id : ");
            int user_id = Convert.ToInt32(Console.ReadLine());
            Console.Write("Enter Password : ");
            string pass = Console.ReadLine();

            conn.Open();

            try
            {
                Credential credential = Permission.getRole(conn, user_id, pass);
                if(credential.autorised && credential.role=="admin")
                {
                    Console.WriteLine();
                    Console.WriteLine("You are Logged in as an Admin");
                    int choice;
                    do
                    {
                        Console.WriteLine();
                        Console.WriteLine("1) List all the Users");
                        Console.WriteLine("2) Edit User details");
                        Console.WriteLine("3) Add new User");
                        Console.WriteLine("0) To Quite");
                        Console.Write("Enter Your Choice (1, 2, 3 or 0) : ");
                        choice = Convert.ToInt32(Console.ReadLine());
                        switch (choice)
                        {
                            case 1:
                                Console.WriteLine();
                                Admin.getAllUser(conn);
                                break;

                            case 2:
                                Console.WriteLine();
                                Console.Write("Enter the ID of the Employee you wanat to edit : ");
                                int empId = Convert.ToInt32(Console.ReadLine());
                                bool idFound = false;

                                SqlCommand cmd = new SqlCommand("select id from users", conn);
                                SqlDataReader data = cmd.ExecuteReader();
                                int fields = data.FieldCount;
                                while (data.Read())
                                {
                                    if (empId == Convert.ToInt32(data["id"]))
                                    {
                                        idFound = true;
                                    }
                                }
                                data.Close();
                                if (idFound)
                                {
                                    Console.Write("Enter New Name : ");
                                    string new_name = Console.ReadLine();
                                    Console.Write("Enter New Surname : ");
                                    string new_surname = Console.ReadLine();
                                    Console.Write("Enter New Age : ");
                                    int new_age = Convert.ToInt32(Console.ReadLine());
                                    Console.Write("Enter New Gender (m/f) : ");
                                    char new_gender = Convert.ToChar(Console.ReadLine());
                                    Console.Write("Enter New Contact No. : ");
                                    string new_no = Console.ReadLine();
                                    Console.Write("Enter New e-mail address : ");
                                    string new_email = Console.ReadLine();
                                    Console.Write("Enter New Password address : ");
                                    string up_pass = Console.ReadLine();

                                    Admin.updateUser(conn, empId, new_name, new_surname, new_age, new_gender, new_no, new_email, up_pass);
                                }
                                else
                                {
                                    Console.WriteLine("Sorry, Id : {0} is not available", empId);
                                }
                                break;

                            case 3:
                                Console.Write("Ente Name : ");
                                string add_name = Console.ReadLine();
                                Console.Write("Enter Surname : ");
                                string add_surname = Console.ReadLine();
                                Console.Write("Enter Age : ");
                                int add_age = Convert.ToInt32(Console.ReadLine());
                                Console.Write("Enter Gender (m/f) : ");
                                char add_gender = Convert.ToChar(Console.ReadLine());
                                Console.Write("Enter Contact No. : ");
                                string add_no = Console.ReadLine();
                                Console.Write("Enter e-mail address : ");
                                string add_email = Console.ReadLine();
                                Console.Write("Enter Role (admin / user) : ");
                                string role = Console.ReadLine();
                                Console.Write("Enter Password of the User : ");
                                string new_pass = Console.ReadLine();

                                if(role == "admin" || role == "user")
                                {
                                    Admin.addUser(conn, add_name, add_surname, add_age, add_gender, add_no, add_email, role, new_pass);
                                }
                                else
                                {
                                    Console.WriteLine("Invalid role entered!");
                                }

                                break;
                            case 0:
                                Console.WriteLine();
                                Console.WriteLine("Thank You For Using Our Program");
                                Console.WriteLine();
                                break;
                            default:
                                Console.WriteLine();
                                Console.WriteLine("Invalid Choice");
                                break;
                        }
                    } while (choice != 0);
                }
                else if(credential.autorised && credential.role == "user")
                {
                    Console.WriteLine();
                    Console.WriteLine("You are Logged in as a User");
                    Console.WriteLine();
                    int choice;
                    do
                    {
                        Console.WriteLine("1) View Details");
                        Console.WriteLine("2) Change Password");
                        Console.WriteLine("0) Quite");
                        Console.Write("Enter Your Choice (1 or 2) : ");
                        choice = Convert.ToInt32(Console.ReadLine());
                        switch(choice)
                        {
                            case 1:
                                User.getUser(conn, user_id);
                                break;
                            case 2:
                                Console.WriteLine();
                                Console.Write("Enter Existing Password : ");
                                string old_pass = Console.ReadLine();
                                Credential userCredential = Permission.getRole(conn, user_id, old_pass);
                                if(userCredential.autorised)
                                {
                                    Console.WriteLine();
                                    Console.Write("Enter New Password : ");
                                    string pass1 = Console.ReadLine();
                                    Console.Write("Confirm Password : ");
                                    string pass2 = Console.ReadLine();
                                    if(pass1==pass2)
                                    {
                                        User.updatePass(conn, user_id, pass1);
                                    }
                                    else
                                    {
                                        Console.WriteLine("Sorry, Password does not matched!");
                                    }

                                }
                                break;
                            case 0:
                                Console.WriteLine("Thank You For Using Our Program");
                                break;
                            default:
                                Console.WriteLine("Invalid Choice");
                                break;
                        }
                    } while (choice != 0);
                }
                else
                {
                    Console.WriteLine("Can't login because of Invalid Credentials!");
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
            }
            finally
            {
                conn.Close();
            }
        }
    }
}