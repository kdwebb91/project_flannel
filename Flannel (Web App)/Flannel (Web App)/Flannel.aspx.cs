using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Flannel;

namespace Flannel__Web_App_
{
    public partial class Flannel : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            for (int i = 0; i < Program.Artists.Count; i++)
            {
                RadioButtonList1.Items.Add(new ListItem(Program.Artists[i]));
            }
        }

        protected void RadioButtonList1_Click(object Source, EventArgs e)
        {
            if (RadioButtonList1.SelectedIndex > -1)
            {
                Label1.Text = "You selected: " + RadioButtonList1.SelectedItem.Text;
            }
        }
    }
}