using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace dom11
{
    public partial class Form1 : Form
    {
        TabControl tc = new TabControl()
        {
            Location = new Point(25, 125),
            Size = new Size(900, 500)
        };

        TextBox tb = new TextBox()
        {
            Location = new Point(25, 75),
            Size = new Size(300, 20)
        };

        Button btn = new Button()
        {
            Location = new Point(350, 75),
            Size = new Size(100, 22),
            Text = "Добави"
        };

        Label lb = new Label()
        {
            Location = new Point(720, 75),
            Size = new Size(200, 20),
            BackColor = Color.White
        };

        OpenFileDialog ofd = new OpenFileDialog()
        {
            InitialDirectory = @"D:\stDom\",
            Title = "Отвори Текстов Файл",
            DefaultExt = "txt",
            Filter = "txt files (*.txt)|*.txt",
            FilterIndex = 2,
            RestoreDirectory = true,
            ReadOnlyChecked = true,
            ShowReadOnly = true,
            CheckFileExists =true,
            CheckPathExists = true
        };

        SaveFileDialog sfd = new SaveFileDialog()
        {
            InitialDirectory = @"D:\stDom\",
            Title = "Запиши Текстов Файл",
            DefaultExt = "txt",
            Filter = "txt files (*.txt)|*.txt",
            FilterIndex = 2,
            RestoreDirectory = true
        };

        string alphabet = "АБВГДЕЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЬЮЯ";

        //=================================================================FORM
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Width = 1000;
            this.Height = 700;

            this.Text = "Даниел Костов 18621439 КСТ 4А група";
            this.Icon = new Icon(@"C:\Users\Daniel\Pictures\Cube.ico");

            tb.ContextMenu = new ContextMenu();

            this.addMenu();
            this.addDataFromFile(@"D:\stDom\lNames.txt");

            tb.KeyPress += new KeyPressEventHandler(tb_KeyPress);
            tb.TextChanged += new EventHandler(tb_TextChanged);
            tb.KeyDown += new KeyEventHandler(tb_KeyDown);

            btn.Click += new EventHandler(btn_Click);

            this.Controls.Add(btn);
            this.Controls.Add(tb);
            this.Controls.Add(lb);
            this.Controls.Add(tc);
        }

        //=================================================================BUTTON
        private void btn_Click(object sender, EventArgs e)
        {
            string str = tb.Text;

            foreach (TabPage t in tc.Controls)
            {
                foreach (ListView lv in t.Controls)
                {
                    if (lv.Items.Count != 0)
                    {
                        string temp = lv.Items[0].Text;

                        if (temp.StartsWith(str.Substring(0, 1)) == true)
                        {
                            ListViewItem lvi = lv.FindItemWithText(str);

                            if (lvi != null)
                            {
                                MessageBox.Show("Тази Фамилия вече съществува!", "Внимание", MessageBoxButtons.OK);
                            }
                            else
                            {
                                ListViewItem itm = new ListViewItem(str);
                                lv.Items.Add(itm);

                                lv.Items[lv.Items.IndexOf(itm)].Selected = true;
                                tc.SelectedTab = t;

                                AutoCompleteStringCollection source = tb.AutoCompleteCustomSource;
                                source.Add(itm.Text);
                                tb.AutoCompleteCustomSource = source;
                            }
                        }
                    }
                }
            }
        }

        //=================================================================MENU
        private void addMenu()
        {
            MenuStrip menu = new MenuStrip();

            ToolStripMenuItem save = new ToolStripMenuItem("&Запиши");
            save.Click += new System.EventHandler(save_Click);

            ToolStripMenuItem open = new ToolStripMenuItem("&Отвори");
            open.Click += new System.EventHandler(open_Click);

            menu.Items.Add(save);
            menu.Items.Add(open);

            menu.BackColor = Color.White;

            this.Controls.Add(menu);
        }

        private void save_Click(object sender, EventArgs e)
        {
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                string str = sfd.FileName;
                this.saveDataToFile(str);
            }
        }

        private void open_Click(object sender, EventArgs e)
        {
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                string str = ofd.FileName;
                this.addDataFromFile(str);
            }
        }

        //=================================================================FILES
        private void addDataFromFile(string s)
        {
            StreamReader rd = new StreamReader(s);
            tc.TabPages.Clear();

            List<string> ls = new List<string>();
            foreach(string line in File.ReadLines(s, Encoding.UTF8))
            {
                ls.Add(line);
            }

            string[] newLines = ls.ToArray();

            foreach (char c in alphabet)
            {
                TabPage t = new TabPage(c.ToString());

                ListView lv = new ListView()
                {
                    View = View.Details,
                    Size = new Size(890, 470),
                    BackColor = Color.LightBlue,
                    ForeColor = Color.Black
                };

                lv.Columns.Add("Фамилии", 290);
                lv.Columns.Add("Адрес", 290);

                for (int i = 0; i < newLines.Length; i++)
                {
                    if (newLines[i].StartsWith(c.ToString()))
                    {
                        string temp = newLines[i];
                        ListViewItem itm = new ListViewItem(temp);
                        lv.Items.Add(itm);
                    }
                }

                lv.ItemSelectionChanged += new ListViewItemSelectionChangedEventHandler(lv_ItemSelectionChanged);

                t.Controls.Add(lv);
                tc.TabPages.Add(t);
            }

            AutoCompleteStringCollection source = new AutoCompleteStringCollection();
            for (int i = 0; i < newLines.Length; i++)
            {
                source.Add(newLines[i]);
            }
            tb.AutoCompleteSource = AutoCompleteSource.CustomSource;
            tb.AutoCompleteCustomSource = source;
            tb.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
        }

        private void lv_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if (e.IsSelected)
            {
                lb.Text = e.Item.Text;
            }
        }

        private void saveDataToFile(string s)
        {
            List<string> ls = new List<string>();

            foreach (TabPage t in tc.Controls)
            {
                foreach (ListView lv in t.Controls)
                {
                    for(int i=0; i<lv.Items.Count; i++)
                    {
                        ls.Add(lv.Items[i].Text);
                    }
                }
            }

            string[] lines = ls.ToArray();
            File.WriteAllLines(s, lines);
        }

        //=================================================================TB EVENTS
        private void tb_KeyPress(object sender, KeyPressEventArgs e)
        {
            char c = e.KeyChar;
            if (tb.Text.Length >= 20 && char.IsControl(c) == false)
            {
                MessageBox.Show("Името може да има до 20 символа!", "Внимание", MessageBoxButtons.OK);
                e.Handled = true;
            }
            if (!(c >= 'а' && c <= 'я' || c >= 'А' && c <= 'Я') && !(c >= 'a' && c <= 'z' || c >= 'A' && c <= 'Z') && !char.IsControl(c))
            {
                MessageBox.Show("Символите са забранени!", "Внимание", MessageBoxButtons.OK);
                e.Handled = true;
            }
            else
            {
                if (c >= 'а' && c <= 'я' || c >= 'А' && c <= 'Я' || char.IsControl(c))
                {

                }
                else
                {
                    MessageBox.Show("Може да въвеждате само на Български!", "Внимание", MessageBoxButtons.OK);
                    e.Handled = true;
                }
            }
        }

        private void tb_TextChanged(object sender, EventArgs e)
        {
            string s = tb.Text;

            if (s.Length == 1)
            {
                s = s.Substring(0, 1).ToUpper();
            }
            else if (s.Length > 1)
            {
                s = s.Substring(0, 1).ToUpper() + s.Remove(0, 1);
            }

            tb.Text = s;
            tb.SelectionStart = tb.Text.Length;
        }

        private void tb_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                string s = tb.Text;

                foreach (TabPage t in tc.Controls)
                {
                    foreach (ListView lv in t.Controls)
                    {
                        if (lv.Items.Count != 0)
                        {
                            string temp = lv.Items[0].Text;

                            if (temp.StartsWith(s.Substring(0, 1)) == true)
                            {
                                ListViewItem lvi = lv.FindItemWithText(s);

                                if (lvi != null)
                                {
                                    lv.Items[lv.Items.IndexOf(lvi)].Selected = true;
                                    tc.SelectedTab = t;
                                }
                                else
                                {
                                    MessageBox.Show("Не с намерена такава фамилия!", "Внимание", MessageBoxButtons.OK);
                                }
                            }
                        }
                    }
                }
            }
        }

        //=================================================================REMOVE CTRL + V
        protected override bool ProcessCmdKey(ref Message msg, Keys key)
        {
            Keys PasteKeys = Keys.Control | Keys.V;

            if (key == PasteKeys)
            {
                MessageBox.Show("Комбинацията за поставяне е забранена!", "Внимание", MessageBoxButtons.OK);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
