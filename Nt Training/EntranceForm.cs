using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Nt_Training
{
    public partial class EntranceForm : Form
    {
        public EntranceForm()
        {
            InitializeComponent();
        }

        private void toGraphicsForm_Click(object sender, EventArgs e)
        {
            new GraphicsForm().Show();
            this.Hide();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FileDialog fileDialog = new OpenFileDialog();
            fileDialog.DefaultExt = "nt";
            fileDialog.Filter = "(*.nt)|*.nt";
            fileDialog.ShowDialog();
            if (fileDialog.FileName.Length > 0)
            {
                new GraphicsForm(fileDialog.FileName).Show();
                this.Hide();
            }
        }
    }
}
