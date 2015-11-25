using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Diablo_2_Next_Generation
{
    public partial class Console : Form
    {
        public Console()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.ToLower().Contains("clear") == false)
            {
                if (textBox1.Text.ToLower().Contains("ping") == false)
                {
                    if (textBox1.Text.ToLower().Contains("show") == false)
                    {
                    HandlerClass.Instance.SendMSG(textBox1.Text);
                    richTextBox1.Text += "Sent:" + textBox1.Text + Environment.NewLine;
                    }
                    else
                    {
                       HandlerClass.Instance.setD2Screen(2);
                       HandlerClass.Instance.setD2Screen(5);
                    }
                }
                else
                {
                    HandlerClass.Instance.SendPing = true;
                }
            }
            else
                richTextBox1.Text = string.Empty;
            textBox1.Text = string.Empty;
        }

        public bool SendToConsole(string Text)
        {
            try
            {
                richTextBox1.Text += Text + Environment.NewLine;
                return true;
            }
            catch { return false; }
        }

        private void Console_FormClosing(object sender, FormClosingEventArgs e)
        {
            HandlerClass.Instance.consoleCheck = false;
            this.Hide();
            e.Cancel = true;
        }
    }
}
