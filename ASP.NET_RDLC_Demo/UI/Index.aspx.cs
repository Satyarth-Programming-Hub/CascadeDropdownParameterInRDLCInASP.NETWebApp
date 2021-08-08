using ASP.NET_RDLC_Demo.ReportDataSets;
using Microsoft.Reporting.WebForms;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;
using System.Drawing;

namespace ASP.NET_RDLC_Demo.UI
{
    public partial class Index : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadCountries();
            }

        }

        protected void btnLoadReport_Click(object sender, EventArgs e)
        {
            if (ddlCountries.SelectedValue.ToString() == "--Select Country--")
            {
                lblMessage.Text = "No country was selected!!!";
                lblMessage.ForeColor = Color.Red;
                lblMessage.Visible = true;
            }
            else
            {
                ReportViewer1.ProcessingMode = ProcessingMode.Local;
                ReportViewer1.LocalReport.ReportPath = Server.MapPath("~/Reports/EmployeeDetails.rdlc");
                Employees employees = GetData();
                ReportDataSource dataSource = new ReportDataSource("Employee", employees.Tables[0]);
                ReportViewer1.LocalReport.DataSources.Clear();
                ReportViewer1.LocalReport.DataSources.Add(dataSource);
            }

        }

        private Employees GetData()
        {
            string constr = ConfigurationManager.ConnectionStrings["DBCS"].ConnectionString;
            string query = "SELECT * FROM Employee WHERE country=@Country AND city=@City";

            SqlCommand cmd = new SqlCommand(query);
            cmd.Parameters.AddWithValue("@Country", ddlCountries.SelectedValue.ToString());
            cmd.Parameters.AddWithValue("@City", ddlCities.SelectedValue.ToString());

            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlDataAdapter da = new SqlDataAdapter())
                {
                    cmd.Connection = con;
                    da.SelectCommand = cmd;
                    using (Employees employees = new Employees())
                    {
                        da.Fill(employees, "Emp");
                        return employees;
                    }
                }
            }
        }

        private void LoadCountries()
        {
            string constr = ConfigurationManager.ConnectionStrings["DBCS"].ConnectionString;
            string query = "SELECT DISTINCT [country] FROM Employee";
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand(query))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = con;
                    con.Open();
                    ddlCountries.DataSource = cmd.ExecuteReader();
                    ddlCountries.DataTextField = "country";
                    ddlCountries.DataValueField = "country";
                    ddlCountries.DataBind();
                    con.Close();
                }
                ddlCountries.Items.Insert(0, new ListItem("--Select Country--"));
            }
        }

        private void LoadCities()
        {
            string constr = ConfigurationManager.ConnectionStrings["DBCS"].ConnectionString;
            string query = "SELECT DISTINCT [city] FROM Employee WHERE [country]=@Country";
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand(query))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@Country", ddlCountries.SelectedValue.ToString());
                    cmd.Connection = con;
                    con.Open();
                    ddlCities.DataSource = cmd.ExecuteReader();
                    ddlCities.DataTextField = "city";
                    ddlCities.DataValueField = "city";
                    ddlCities.DataBind();
                    con.Close();
                }
                //ddlCountries.Items.Insert(0, new ListItem("--Select City--"));
            }
        }

        protected void ddlCountries_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlCountries.SelectedValue.ToString() == "--Select Country--")
            {
                ddlCities.Items.Clear();
                ddlCities.Enabled = false;
            }
            else
            {
                LoadCities();
                ddlCities.Enabled = true;
                lblMessage.Text = string.Empty;
                lblMessage.Visible = false;
            }

        }
    }
}