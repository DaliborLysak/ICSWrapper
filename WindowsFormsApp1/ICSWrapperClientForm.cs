using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ICSWrapper;

namespace WindowsFormsApp1
{
    public partial class ICSWrapperClientForm : Form
    {
        public ICSWrapperClientForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var calendar = new ICSCalendar();
            //calendar.Get(@"c:\temp\Support\Client Support_ive38k87s5440k3k1bs6t78j00@group.calendar.google.com.ics");
            calendar.Get(@"c:\temp\Support\calendarValidated.ics");
            
        }
    }
}
