using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UECP;

namespace UecpTestClient
{
    public partial class MainWindow : Form
    {
        private UECPEncoder _encoder;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnSetPI_Click(object sender, EventArgs e)
        {
            if (tbPI.Text.Length != 4)
            {
                MessageBox.Show("Invalid input", "PI must be an hexadecimal number of exactly four characters.", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            // TODO : Convert
            _encoder.SetPI(0x0000);
        }

        private void btnSetPS_Click(object sender, EventArgs e)
        {
            if(tbPS.Text.Length > 8)
            {
                MessageBox.Show("Invalid input", "PS must be less than 8 characters long.", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            _encoder.SetPS(tbPS.Text);
        }

        private void btnSetRT_Click(object sender, EventArgs e)
        {
            if (tbRT.Text.Length > 64)
            {
                MessageBox.Show("Invalid input", "RadioText must be less than 64 characters long.", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            _encoder.SetRadioText(tbRT.Text);
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            _encoder = new UECPEncoder(new UDPSimplexEndpoint(tbIPAddr.Text, (int)numUdpPort.Value));
            gbControls.Enabled = true;
        }
    }
}
