using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Society
{
	public partial class Export : System.Web.UI.Page
	{
        string cs = ConfigurationManager.ConnectionStrings["Task4"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadData();
            }
        }

        protected void LoadData()
        {
            string selectedTable = ddlTableSelection.SelectedValue;

            using (SqlConnection conn = new SqlConnection(cs))
            {
                conn.Open();
                string query = $"SELECT * FROM {selectedTable} WHERE (@FromDate IS NULL OR created_at >= @FromDate) AND (@ToDate IS NULL OR created_at <= @ToDate)";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    DateTime? fromDate = string.IsNullOrEmpty(txtFromDate.Text) ? (DateTime?)null : Convert.ToDateTime(txtFromDate.Text);
                    DateTime? toDate = string.IsNullOrEmpty(txtToDate.Text) ? (DateTime?)null : Convert.ToDateTime(txtToDate.Text);

                    cmd.Parameters.AddWithValue("@FromDate", fromDate ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@ToDate", toDate ?? (object)DBNull.Value);

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    // Store filtered data in ViewState
                    ViewState["FilteredData"] = dt;

                    gvData.DataSource = dt;
                    gvData.DataBind();
                }
            }
        }

        protected void btnFilter_Click(object sender, EventArgs e)
        {
            LoadData();
        }

        protected void ddlTableSelection_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadData();
        }

        protected void btnExportAll_Click(object sender, EventArgs e)
        {
            DataTable dt = ViewState["FilteredData"] as DataTable;
            if (dt != null)
            {
                ExportCSV(dt);
            }
        }

        protected void gvData_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                TableCell exportCell = new TableCell();
                Button exportButton = new Button
                {
                    Text = "Export",
                    CommandArgument = e.Row.Cells[0].Text, // Assuming first column is ID
                    CommandName = "ExportRow"
                };
                exportButton.Click += btnExportSingle_Click;
                exportCell.Controls.Add(exportButton);
                e.Row.Cells.Add(exportCell);
            }
            else if (e.Row.RowType == DataControlRowType.Header)
            {
                TableHeaderCell headerCell = new TableHeaderCell { Text = "Export" };
                e.Row.Cells.Add(headerCell);
            }
        }

        protected void btnExportSingle_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            int id = Convert.ToInt32(btn.CommandArgument);
            string selectedTable = ddlTableSelection.SelectedValue;

            using (SqlConnection conn = new SqlConnection(cs))
            {
                conn.Open();
                string query = $"SELECT * FROM {selectedTable} WHERE id = @id";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    if (dt.Rows.Count > 0)
                    {
                        ExportCSV(dt);
                    }
                }
            }
        }

        private void ExportCSV(DataTable dt)
        {
            StringWriter sw = new StringWriter();

            // Write headers
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                sw.Write(dt.Columns[i].ColumnName);
                if (i < dt.Columns.Count - 1) sw.Write(",");
            }
            sw.WriteLine();

            // Write rows
            foreach (DataRow row in dt.Rows)
            {
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    sw.Write(row[i].ToString());
                    if (i < dt.Columns.Count - 1) sw.Write(",");
                }
                sw.WriteLine();
            }

            Response.Clear();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment;filename=Exported_Data.csv");
            Response.Charset = "";
            Response.ContentType = "application/text";
            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();
        }
    }
}