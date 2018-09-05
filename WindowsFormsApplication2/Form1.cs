//////////////////////////////////////////////////////////////////////////////////
///                                                                            ///
///             Rename System Version  1.0                                     ///
///                    By Phoon Wai Chun                                       ///
///                                                                            ///
///                         05/09/2018                                         ///
///                                                                            ///
///                                                                            ///
//////////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;

namespace WindowsFormsApplication2
{
    public partial class Form1 : Form
    {
        
        private List<string> fff;
        private List<fileClass> before;     // source file class list
        private List<fileClass> after;      // target file class list
        private List<int> beforeSeq;        // source file sequence
        private List<int> afterSeq;         // target file sequence

        public Form1()
        {
            InitializeComponent();
            before = new List<fileClass>();
            after = new List<fileClass>();
            beforeSeq = new List<int>();
            afterSeq = new List<int>();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            fff=getFileFullPath();
        }

        private List<string> getFileFullPath()
        {
            dataGridView1.Rows.Clear();
            List<string> path = new List<string>();
            int i = 0;
            Stream myStream;
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Multiselect = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                foreach (String file in openFileDialog1.FileNames)
                {
                    try
                    {
                        if ((myStream = openFileDialog1.OpenFile()) != null)
                        {
                            using (myStream)
                            {
                                FileInfo fi = new FileInfo(file);
                                dataGridView1.Rows.Add(1);
                                dataGridView1.Rows[i].Cells[0].Value = fi.Name;
                                dataGridView1.Rows[i].Cells[1].Value = fi.FullName;

                                dataGridView2.Rows.Add(1);
                                dataGridView2.Rows[i].Cells[0].Value = fi.Name;
                                dataGridView2.Rows[i].Cells[1].Value = fi.FullName;

                                fileClass f = new fileClass(fi.Name, fi.FullName);

                                beforeSeq.Add(i);
                                before.Add(f);
                                after.Add(f);
                                
                                i++;
                                path.Add(fi.FullName);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                    }
                }
            }
            return path;
        }


        private bool rename()
        {
            dataGridView2.Rows.Clear();
            int count = 0;
            if(textBox1.Text=="")
            {
                MessageBox.Show("Error", "Empty");
                return false;
            }
            
            for(int i = 0; i < dataGridView1.RowCount-1; i++)
            {
                string before = dataGridView1.Rows[i].Cells[0].Value.ToString();
                if (before.Contains(textBox1.Text))
                {
                    
                    dataGridView2.Rows.Add(1);
                    string after = before.Replace(textBox1.Text, textBox2.Text);
                    dataGridView2.Rows[count].Cells[0].Value = after;
                    string fullBefore = dataGridView1.Rows[i].Cells[1].Value.ToString().Replace(before, "");
                    dataGridView2.Rows[count].Cells[1].Value = fullBefore + after;
                    afterSeq.Add(i);
                    count++;
                }
            }
            if(count==0)
                MessageBox.Show("No match result found");
            else
                MessageBox.Show("Total result : " + count);

            return true;
        }

        // file name cant be same, validation step
        private bool same()
        {
            for (int i = 0; i < dataGridView2.RowCount - 1; i++)
            {
                for (int j = 0; j < dataGridView2.RowCount - 1; j++)
                {
                    if (i == j)
                        continue;
                    else
                    {
                        if (dataGridView2.Rows[i].Cells[0].Value.ToString() == dataGridView2.Rows[j].Cells[0].Value.ToString())
                        {
                            dataGridView2.Rows[i].Selected = true;
                            dataGridView2.Rows[j].Selected = true;
                            MessageBox.Show("File name can't be same \n " + dataGridView2.Rows[i].Cells[0].Value.ToString(), "Duplicate Name!");
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (extension())
                rename();
            else
                MessageBox.Show("Extension");
        }

        // save file name
        private bool saveName()
        {
            for(int i = 0; i < dataGridView2.RowCount - 1; i++)
            {
                try
                {
                    File.Move(before[beforeSeq[afterSeq[i]]].locationV, dataGridView2.Rows[i].Cells[1].Value.ToString());
                }catch(Exception e)
                {
                    MessageBox.Show(e.ToString(), "Error Rename File");
                    return false;
                }
            }
            MessageBox.Show("Successful update " + (dataGridView2.RowCount-1) + " record(s)");

            dataGridView1.Rows.Clear();
            for(int i=0;i<dataGridView2.RowCount-1;i++)
            {
                dataGridView1.Rows.Add();
                dataGridView1.Rows[i].Cells[0].Value = dataGridView2.Rows[i].Cells[0].Value.ToString();
                dataGridView1.Rows[i].Cells[1].Value = dataGridView2.Rows[i].Cells[1].Value.ToString();
            }

            return true;
        }

        // synchronize file name with file location
        private bool sync()
        {
            for(int i = 0; i < dataGridView2.RowCount - 1; i++)
            {
                try {
                    var f = dataGridView2.Rows[i].Cells[1].Value.ToString().Split('\\');
                    f[f.Length - 1] = dataGridView2.Rows[i].Cells[0].Value.ToString();
                    string a = String.Join("\\", f);
                    dataGridView2.Rows[i].Cells[1].Value = a;
                }catch(Exception e)
                {
                    MessageBox.Show(e.ToString(), "Error Message");
                    return false;
                }
            }

            return true;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if(same()&&sync())
                saveName();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (addText())
                MessageBox.Show("Updated");
            else
                MessageBox.Show("Error");
        }



        // This function will add text to the front of file name 
        // new.docx -> addnew.docx
        private bool addText()
        {
            if (dataGridView2.RowCount == 1)
                return false;

            for (int i = 0; i < dataGridView2.RowCount - 1; i++)
            {
                dataGridView2.Rows[i].Cells[0].Value = textBox3.Text.ToString() + dataGridView2.Rows[i].Cells[0].Value.ToString();
            }

            return true;
        }


        // this function check will replace file extension or not
        private bool extension()
        {
            for(int i = 0; i < dataGridView1.RowCount - 1; i++)
            {
                var f = dataGridView1.Rows[i].Cells[0].Value.ToString().Split('.');
                string o="."+f[f.Length - 1];
                if (o.Contains(textBox1.Text))
                    return false;
            }
            return true;
        }
    }
}
