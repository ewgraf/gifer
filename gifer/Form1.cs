using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace gifer
{
    public partial class Form1 : Form
    {
        private Image img = null;

        public Form1(string args)
        {
            InitializeComponent();

            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.AllowDrop = true;
            this.pictureBox1.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseWheel);
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;

            if (!string.IsNullOrEmpty(args))
            {
                pictureBox1.Image = Image.FromFile(args);
                pictureBox1.Size = pictureBox1.Image.Size;
                this.Size = pictureBox1.Size;
            }
        }
        
        private void Form1_Load(object sender, EventArgs e)
        {
            //pictureBox1.Image = Bitmap.FromFile(@"C:\1\1.gif");
        }

        private void Form1_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.All;
        }

        private void Form1_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

            img = Image.FromFile(files[0]);

            pictureBox1.Size = img.Size;
            pictureBox1.Image = img;

            this.Size = pictureBox1.Size;
        }

        private bool move = false;
        private int X = 0, Y = 0;

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            move = true;
            X = e.X;
            Y = e.Y;
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            move = false;
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            pictureBox1.Focus();
            if (move)
            {
                this.SetDesktopLocation(MousePosition.X - X, MousePosition.Y - Y);
            }
        }

        private int Delta = 0;
        private int x1 = 0;
        private bool resizing = false;

        public void resize()
        {
            try
            {
                resizing = true;
                /* Work. */
                x1 = Delta;

                /* Plan Be */
                //x1 = Delta;
                /*if (x1 > 0)
                {
                    while (x1 > 0)
                    {
                        x1 -= x1 / 2 > 0 ? x1 / 2 : 1;

                        pictureBox1.Invoke((MethodInvoker)delegate
                        {
                            pictureBox1.Size = new Size(pictureBox1.Size.Height + x1/2, pictureBox1.Size.Width + x1/2);
                        });
                        this.Invoke((MethodInvoker)delegate
                        {
                            this.Size = pictureBox1.Size;
                        });
                    }
                }
                else //x1 < 0
                {
                    while (x1 < 0)
                    {
                        x1 += x1 / 2 > 0 ? x1 / 2 : 1;

                        pictureBox1.Invoke((MethodInvoker)delegate
                        {
                            pictureBox1.Size = new Size(pictureBox1.Size.Height - x1/2, pictureBox1.Size.Width - x1/2);
                        });
                        this.Invoke((MethodInvoker)delegate
                        {
                            this.Size = pictureBox1.Size;
                        });
                    }
                }*/

                /* Plan Tse */
                /*if (x1>0)
                {
                    for (int i = x1; i > 0; i--)
                    {
                        pictureBox1.Invoke((MethodInvoker)delegate
                        {
                            pictureBox1.Size = new Size(pictureBox1.Size.Height + 1, pictureBox1.Size.Width + 1);
                        });
                        this.Invoke((MethodInvoker)delegate
                        {
                            this.Size = pictureBox1.Size;
                        });

                    }
                }
                else
                {
                    for (int i = x1; i < 0; i++)
                    {
                        pictureBox1.Invoke((MethodInvoker)delegate
                        {
                            pictureBox1.Size = new Size(pictureBox1.Size.Height - 1, pictureBox1.Size.Width - 1);
                        });
                        this.Invoke((MethodInvoker)delegate
                        {
                            this.Size = pictureBox1.Size;
                        });
                    }
                }*/

                //x1 = 1;
                int sdvig_left = 0, sdvif_up = 0;

                /* Plan De*/
                /*sdvig_left = Convert.ToInt32(pictureBox1.Size.Height * x1 / 1000);
                sdvif_up = Convert.ToInt32(pictureBox1.Size.Width * x1 / 1000);
                pictureBox1.Invoke((MethodInvoker)delegate
                {
                    pictureBox1.Size = new Size(pictureBox1.Size.Width + sdvif_up, pictureBox1.Size.Height + sdvig_left);
                    this.Size = pictureBox1.Size;
                    this.Location = new Point(this.Location.X - sdvig_left / 2, this.Location.Y - sdvif_up / 2);
                });*/
                while (x1 != 0)
                {
                    sdvig_left = Convert.ToInt32(pictureBox1.Size.Height * x1 / 1000);
                    sdvif_up = Convert.ToInt32(pictureBox1.Size.Width * x1 / 1000);

                    pictureBox1.Invoke((MethodInvoker)delegate
                    {
                        pictureBox1.Size = new Size(pictureBox1.Size.Width + sdvif_up, pictureBox1.Size.Height + sdvig_left);
                    });

                    this.Invoke((MethodInvoker)delegate {
                        this.Size = pictureBox1.Size;
                        this.Location = new Point(this.Location.X - sdvig_left / 2, this.Location.Y - sdvif_up / 2);
                    });

                    x1 = x1 > 0 ? x1 - 1 : x1 + 1;

                    x1 /= 2;

                    //System.Threading.Thread.Sleep(15);
                }
                resizing = false;
                return;
            }
            catch (System.Threading.ThreadInterruptedException ex)
            {
                /* Clean up. */
                x1 = 0;
                resizing = false;
                return;
            }
        }

        public void pictureBox1_MouseWheel(object sender, MouseEventArgs e)
        {
            Delta = e.Delta;
            if (!resizing)
                System.Threading.Tasks.Task.Factory.StartNew(() => resize());
        }

        private void pictureBox1_MouseLeave(object sender, EventArgs e)
        {
            this.ActiveControl = null;
        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                Application.Exit();
            }
        }
    }
}