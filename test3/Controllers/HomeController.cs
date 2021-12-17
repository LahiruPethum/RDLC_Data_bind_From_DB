using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Reporting.NETCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using test3.Models;

namespace test3.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public HomeController(ILogger<HomeController> logger, IWebHostEnvironment webHostEnvironment)
        {
            _logger = logger;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult PrintReport()
        {
            string renderFormat = "PDF";
            string extention = "pdf";
            string mimetype = "application/pdf";
            using var report = new LocalReport();
            var dt = new DataTable();
            dt = GetEmployeeList();

            report.DataSources.Add(new ReportDataSource("dsEmployee", dt));
            var parameters = new[] { new ReportParameter("param1", "RDLC Sub-Report Example") };
            report.ReportPath = $"{this._webHostEnvironment.WebRootPath}\\Reports\\rptEmployee.rdlc";
            report.SetParameters(parameters);

            report.SubreportProcessing += new SubreportProcessingEventHandler(SubReportProcessing);

            var pdf = report.Render(renderFormat);
            return File(pdf, mimetype, "report." + extention);
        }

        void SubReportProcessing(object sender, SubreportProcessingEventArgs e) {
            int empId = int.Parse(e.Parameters["EmpId"].Values[0].ToString());
            var dt2 = new DataTable();
            dt2 = GetEmployeeDetailsList().Select("EmpId=" + empId).CopyToDataTable();

            ReportDataSource ds = new ReportDataSource("dsEmployeeDetails", dt2);
            e.DataSources.Add(ds);
        }

        public DataTable GetEmployeeList()
        {
            var dt = new DataTable();
            dt.Columns.Add("EmpId");
            dt.Columns.Add("EmpName");
            dt.Columns.Add("Department");
            dt.Columns.Add("Designation");
            DataRow row;

            for (int i = 101; i <= 120; i++) {
                row = dt.NewRow();
                row["EmpId"]=i;
                row["EmpName"] = "Mr.Robert" + i;
                row["Department"] = "Information Technology";

                row["Designation"] = "Software Engineer";

                dt.Rows.Add(row);
            }

            return dt;


        }







        public DataTable GetEmployeeDetailsList()
        {
            var dt = new DataTable();
            dt.Columns.Add("EmpId");
            dt.Columns.Add("Details");
       
            DataRow row;

            for (int i = 101; i <= 120; i++)
            {
                for (int j = 1; j <= 3; j++)
                {
                    row = dt.NewRow();
                    row["EmpId"] = i;
                    row["Details"] = "Child-" + j;
                    dt.Rows.Add(row);
                }
              
             

              
            }

            return dt;


        }


    }
}
