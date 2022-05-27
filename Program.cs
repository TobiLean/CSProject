using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSProject
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //Some Local Variables
            List<Staff> myStaff = new List<Staff>();
            FileReader fr = new FileReader();
            int month = 0;
            int year = 0;

            //Error handling
            while (year == 0)
            {
                Console.Write("\nPlease enter the year: ");

                try
                {
                    year = Convert.ToInt32(Console.ReadLine());
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message + "Try again! Please enter a number in this 'YYYY' format ");
                }
            }

            while (month == 0)
            {
                Console.Write("\nPlease enter the month: ");

                try
                {
                    month = Convert.ToInt32(Console.ReadLine());

                    if (month < 1 || month > 12)
                    {
                        Console.WriteLine("\n Month must be a number between 1 and 12");
                        month = 0;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }

            myStaff = fr.ReadFile();

            for (int i = 0; i < myStaff.Count; i++)
            {
                try
                {
                    Console.Write("\nPlease Enter hours worked for {0}: ", 
                        myStaff[i].NameOfStaff);

                    myStaff[i].HoursWorked = Convert.ToInt32(Console.ReadLine());

                    myStaff[i].CalculatePay();

                    Console.WriteLine(myStaff[i].ToString());
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    i--;
                }
            }

            PaySlip ps = new PaySlip(month, year);
            ps.GeneratePaySlip(myStaff);
            ps.GenerateSummary(myStaff);

            Console.Read();
        }
    }
}

class Staff
{
    private float hourlyRate;
    private int hWorked;

    public float TotalPay
    {
        get; protected set;
    }

    public float BasicPay
    {
        get; private set;
    }

    public string NameOfStaff
    {
        get; private set;
    }

    public int HoursWorked
    {
        get
        {
            return hWorked;
        }

        set
        {
            if (value > 0)
                hWorked = value;
            else
                hWorked = 0;
        }
    }

    public Staff(string name, float rate)
    {
        NameOfStaff = name;
        hourlyRate = rate;

    }

    public virtual void CalculatePay()
    {
        Console.WriteLine("Calculating Pay...");

        BasicPay = hWorked * hourlyRate;
        TotalPay = BasicPay;
    }

    public override string ToString()
    {
        return "\nNameOfStaff: " + NameOfStaff + "\nBasicPay: " + BasicPay + "\nTotalPay: "
            + TotalPay + "\nHourlyRate: " + hourlyRate + "\nhWorked: " +
            hWorked;
    }
}

class Manager : Staff
{
    private const float managerHourlyRate = 50;

    public int Allowance
    {
        get; private set;
    }

    public Manager(string name) : base(name, managerHourlyRate)
    {

    }

    public override void CalculatePay()
    {
        base.CalculatePay();
        Allowance = 1000;

        if (HoursWorked > 160)
        {
            TotalPay = BasicPay + Allowance;
        }
    }

    public override string ToString()
    {
        return "\nName: " + NameOfStaff + "\nmanagerHourlyRate: " + managerHourlyRate 
            + "\nBasicPay: " + BasicPay + "\nAllowance: " + Allowance + "\nTotalPay: "
            + TotalPay + "\nHoursWorked: " + HoursWorked;
    }
}

class Admin : Staff
{
    private const float overtimeRate = 15.5f;
    private const float adminHourlyRate = 30f;

    public float Overtime
    {
        get;     
        private set;
    }

    public Admin(string name) : base(name, adminHourlyRate)
    {

    }

    public override void CalculatePay()
    {
        base.CalculatePay();

        if (HoursWorked >160)
        {
            Overtime = overtimeRate + (HoursWorked - 160);
        }
    }

    public override string ToString()
    {
        return "\nName: " + NameOfStaff + "\nadminHourlyRate: " + adminHourlyRate 
            + "\nOvertime: " + Overtime
            + "\nBasicPay: " + BasicPay + "\nTotalPay: "
            + TotalPay + "\nHoursWorked: " + HoursWorked;
    }
}

class FileReader //Responsible for reading Staff.txt file to get names and role, the order is "Name, Role"
{
    public List<Staff> ReadFile()
    {
        List<Staff> myStaff = new List<Staff>();
        string[] result = new string[2];
        string path = "staff.txt";
        string[] seperator = {", "};

        if (File.Exists(path))
        {
            using (StreamReader sR = new StreamReader(path))
            {
                while (!sR.EndOfStream)
                {
                    string aStaff = sR.ReadLine();
                    result = aStaff.Split(seperator, StringSplitOptions.RemoveEmptyEntries);

                    if (result[1] == "Manager")
                        myStaff.Add(new Manager(result[0]));
                    else if (result[1] == "Admin")
                        myStaff.Add(new Admin(result[0]));
                }

                sR.Close();
                
            }
        }
        else
        {
            Console.WriteLine("No file");
        }

        return myStaff;
    }
}

class PaySlip //Responsible for creating Payments for staff and writing them to files
{
    private int month;
    private int year;

    enum MonthsOfYear
    {
        Jan = 1, Feb, Mar, Apr, May, Jun, Jul, Aug, Sep, 
        Oct, Nov, Dec
    }

    public PaySlip(int payMonth, int payYear)
    {
        month = payMonth;
        year = payYear;
    }

    //To write staff payment details in text file
    public void GeneratePaySlip(List<Staff> myStaff)
    {
        string path;
        //Will use this later!
        //Remember!!!!!!!
        string Naira = "₦";

        foreach (Staff f in myStaff)
        {
            path = "CSProject_Staff_" + f.NameOfStaff + ".txt";

            using (StreamWriter sW = new StreamWriter(path, true))
            {
                sW.WriteLine("PAYSLIP FOR {0} {1}", (MonthsOfYear)month, year);
                sW.WriteLine("========================================");
                sW.WriteLine("Name of Staff: {0}", f.NameOfStaff);
                sW.WriteLine("Hours Worked: {0}", f.HoursWorked);
                sW.WriteLine("");
                sW.WriteLine("Basic Pay: {0:C}", f.BasicPay);

                if (f.GetType() == typeof(Manager))
                {
                    sW.WriteLine("Allowance: {0:C}", ((Manager)f).Allowance);
                }
                else if (f.GetType() == typeof(Admin))
                {
                    sW.WriteLine("Overtime: {0:C}", ((Admin)f).Overtime);
                }

                sW.WriteLine("");
                sW.WriteLine("========================================");
                sW.WriteLine("Total Pay: {0:C}", Naira, f.TotalPay);
                sW.WriteLine("========================================");

                sW.Close();
            }
        }
    }

    //Generates a text file listing all staff members who work less than 10 hours
    public void GenerateSummary(List<Staff> myStaff)
    {
        var result =
            from staff in myStaff
            where staff.HoursWorked < 10
            orderby staff.NameOfStaff ascending
            select new { staff.NameOfStaff, staff.HoursWorked };

        string path = "summary.txt";

        using (StreamWriter sW = new StreamWriter(path))
        {
            sW.WriteLine("Staff with less than 10 working hours");
            sW.WriteLine("");

            foreach (var f in result)
            {
                sW.WriteLine("Name of Staff: {0}, Hours Worked: {1}",f.NameOfStaff
                    , f.HoursWorked);
            }

            sW.Close();
        }
    }

    public override string ToString()
    {
        return "month = " + month + "year = " + year;
            
    }
}