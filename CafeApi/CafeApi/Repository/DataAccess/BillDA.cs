using CafeApi.Common;
using CafeApi.DAHelper;
using CafeApi.Models.DataModels;
using CafeApi.Models.TableModels;
using CafeApi.Repository.Contracts;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Draw;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using Microsoft.AspNetCore.Routing.Constraints;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Data;
using System.Net;
using System.Web.Http;
using System.Web.Http.Controllers;


namespace CafeApi.Repository.DataAccess
{
    public class BillDA : IBill
    {
        private readonly ICommonDAHelper _dahelper;
        private readonly IConfiguration _configuration;
        public BillDA(ICommonDAHelper dahelper, IConfiguration configuration)
        {
            _dahelper = dahelper;
            _configuration = configuration;
        }
        private void GetBill(Billmodel bill)
        {
            dynamic productDetails = JsonConvert.DeserializeObject(bill.productDetails);
            var todayDate = "Date: " + Convert.ToDateTime(DateTime.Today).ToString("MM/dd/yyyy");

            PdfWriter writer = new PdfWriter(_configuration.GetSection("pdfPath").Value.ToString() + bill.uuid + ".pdf");
            PdfDocument pdf = new PdfDocument(writer);
            Document document = new Document(pdf);

            //Header
            Paragraph header = new Paragraph("Cafe Management System")
                               .SetBold()
                               .SetTextAlignment(TextAlignment.CENTER)
                               .SetFontSize(25);
            document.Add(header);

            //New line
            Paragraph newline = new Paragraph(new Text("\n"));

            //Line Seperator
            LineSeparator ls = new LineSeparator(new SolidLine());
            document.Add(ls);

            //Customer Details
            Paragraph customerDetails = new Paragraph("Name:" + bill.name + "\nEmail: " + bill.email + "\nContact Number: " + bill.contactNumber + "\nPayment Method: " + bill.paymentMethod);
            document.Add(customerDetails);

            //Table
            Table table = new Table(5, false);
            table.SetWidth(new UnitValue(UnitValue.PERCENT, 100));

            // Table headers
            Cell headerName = new Cell(1, 1)
                                 .SetTextAlignment(TextAlignment.CENTER)
                                 .SetBold()
                                 .Add(new Paragraph("Name"));
            Cell headerCategory = new Cell(1, 1)
                                .SetTextAlignment(TextAlignment.CENTER)
                                .SetBold()
                                .Add(new Paragraph("Category"));
            Cell headerQuantity = new Cell(1, 1)
                                .SetTextAlignment(TextAlignment.CENTER)
                                .SetBold()
                                .Add(new Paragraph("Quantity"));
            Cell headerPrice = new Cell(1, 1)
                                .SetTextAlignment(TextAlignment.CENTER)
                                .SetBold()
                                .Add(new Paragraph("Price"));

            Cell headerSubTotal = new Cell(1, 1)
                                .SetTextAlignment(TextAlignment.CENTER)
                                .SetBold()
                                .Add(new Paragraph("Sub Total"));


            table.AddCell(headerName);
            table.AddCell(headerCategory);
            table.AddCell(headerQuantity);
            table.AddCell(headerPrice);
            table.AddCell(headerSubTotal);

            foreach (JObject product in productDetails)
            {
                Cell nameCell = new Cell(1, 1)
                        .SetTextAlignment(TextAlignment.CENTER)
                        .Add(new Paragraph(product["name"].ToString()));

                Cell categoryCell = new Cell(1, 1)
                        .SetTextAlignment(TextAlignment.CENTER)
                        .Add(new Paragraph(product["category"].ToString()));

                Cell quantityCell = new Cell(1, 1)
                        .SetTextAlignment(TextAlignment.CENTER)
                        .Add(new Paragraph(product["quantity"].ToString()));

                Cell priceCell = new Cell(1, 1)
                        .SetTextAlignment(TextAlignment.CENTER)
                        .Add(new Paragraph(product["price"].ToString()));

                Cell totalCell = new Cell(1, 1)
                        .SetTextAlignment(TextAlignment.CENTER)
                        .Add(new Paragraph(product["total"].ToString()));

                table.AddCell(nameCell);
                table.AddCell(categoryCell);
                table.AddCell(quantityCell);
                table.AddCell(priceCell);
                table.AddCell(totalCell);
            }

            document.Add(table);
            Paragraph last = new Paragraph("Total: " + bill.totalAmount + "\nThank you for visiting. Please visit again");
            document.Add(last);

            document.Close();


        }

        public ResponseModel<string> GenerateReport(string authorization, Billmodel bill)
        {
            ResponseModel<string> responseModel = null;
            TokenClaim tokenClaim = TokenManager.ValidateToken(authorization);

            var ticks = DateTime.Now.Ticks;
            var guid = Guid.NewGuid().ToString();
            var uniqueId = ticks.ToString() + '-' + guid;

            bill.createdby = tokenClaim.Email;
            bill.uuid = uniqueId;

            string sql = "insert into bill values (@uuid ,@name ,@email,@contactNumber  ,@paymentMethod ,@totalAmount,@productDetails,@createdby)";
            IList<QueryParameterForSqlMapper> parameterCollection = new List<QueryParameterForSqlMapper>()
            {
                    new QueryParameterForSqlMapper
                    {
                        Name = "@uuid",
                        Value = bill.uuid,
                        parameterDirection = ParameterDirection.Input,
                        DbType = DbType.String
                    },
                    new QueryParameterForSqlMapper
                    {
                        Name = "@name",
                        Value = bill.name,
                        parameterDirection = ParameterDirection.Input,
                        DbType = DbType.String
                    },
                    new QueryParameterForSqlMapper
                    {
                        Name = "@email",
                        Value = bill.email,
                        parameterDirection = ParameterDirection.Input,
                        DbType = DbType.String
                    },
                     new QueryParameterForSqlMapper
                    {
                        Name = "@contactNumber",
                        Value = bill.contactNumber,
                        parameterDirection = ParameterDirection.Input,
                        DbType = DbType.String
                    },
                     new QueryParameterForSqlMapper
                    {
                        Name = "@paymentMethod",
                        Value = bill.paymentMethod,
                        parameterDirection = ParameterDirection.Input,
                        DbType = DbType.String
                    },
                     new QueryParameterForSqlMapper
                    {
                        Name = "@totalAmount",
                        Value = bill.totalAmount,
                        parameterDirection = ParameterDirection.Input,
                        DbType = DbType.Int32
                    },
                      new QueryParameterForSqlMapper
                    {
                        Name = "@productDetails",
                        Value = bill.productDetails,
                        parameterDirection = ParameterDirection.Input,
                        DbType = DbType.String
                    },
                         new QueryParameterForSqlMapper
                    {
                        Name = "@createdBy",
                        Value = bill.createdby,
                        parameterDirection = ParameterDirection.Input,
                        DbType = DbType.String
                    }
            };

            int result = _dahelper.Execute(sql, parameterCollection);

            GetBill(bill);

            responseModel = new ResponseModel<string> { StatusCode = HttpStatusCode.OK, Data = bill.uuid };
            return responseModel;
        }


        public (byte[] fileBytes, string fileName, string contentType) GetPdf(string authorization, Billmodel bill)
        {

            if (bill.name != null)
            {
                GetBill(bill);
            }

            string filepath = _configuration.GetSection("pdfPath").Value.ToString() + bill.uuid.ToString() + ".pdf";
            byte[] bytes = File.ReadAllBytes(filepath);
            string Filename = bill.uuid.ToString() + ".pdf";

            return (bytes, Filename, "application/pdf");

        }

        public ResponseModel<IEnumerable<Billmodel>> GetBills(string authorization)
        {
            ResponseModel<IEnumerable<Billmodel>> responseModel = null;

            TokenClaim tokenClaim = TokenManager.ValidateToken(authorization);
            if (tokenClaim.Role != "admin")
            {
                string sql = "select * from Bill where createdby = @email";
                IList<QueryParameterForSqlMapper> parameters = new List<QueryParameterForSqlMapper>
                {
                    new QueryParameterForSqlMapper
                    {
                        Name = "@email",
                        Value = tokenClaim.Email,
                        DbType = DbType.String,
                        parameterDirection = ParameterDirection.Input
                    }
                };

                IEnumerable<Billmodel> userBills = null;
                userBills = _dahelper.Query<Billmodel>(sql, parameters);

                responseModel = new ResponseModel<IEnumerable<Billmodel>> { StatusCode = HttpStatusCode.OK, Data = userBills };
                return responseModel;
            }

            IEnumerable<Billmodel> adminBills = null;
            string sql2 = "select * from Bill";
            adminBills = _dahelper.Query<Billmodel>(sql2, null);
            responseModel = new ResponseModel<IEnumerable<Billmodel>> { StatusCode = HttpStatusCode.OK, Data = adminBills };
            return responseModel;
        }

        public ResponseModel<string> DeleteBill(string authorization, int id)
        {
            ResponseModel<string> responseModel = null;
            IList<QueryParameterForSqlMapper> parameters = new List<QueryParameterForSqlMapper>
                {
                    new QueryParameterForSqlMapper
                    {
                        Name = "@id",
                        Value = id,
                        DbType = DbType.Int32,
                        parameterDirection = ParameterDirection.Input
                    }
                };

            dynamic result = _dahelper.ExecuteScalarProcedure("usp_DeleteBill", parameters);
            if (result == 1)
            {
                responseModel = new ResponseModel<string> { StatusCode = HttpStatusCode.OK, Data = "Bill deleted successfully" };
            }
            if (result == 0)
            {
                responseModel = new ResponseModel<string> { StatusCode = HttpStatusCode.OK, Data = "Bill id does not exists" };
            }

            return responseModel;

        }

    }
}

