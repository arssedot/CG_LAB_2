using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace CG_LAB_2
{
    public partial class Form1 : Form
    {
        Bin bin = new Bin();
        View view = new View();
        bool loaded = false;
        int currentLayer = 0;
        bool Quads = false;
        bool Texture = false;

        int FrameCount;
        DateTime NextFPSUUpdate = DateTime.Now.AddSeconds(1);
        public Form1()
        {
            InitializeComponent();
            trackBar1.Minimum = 0;
            trackBar1.Maximum = 2000;
            trackBar1.Value = 0;

            trackBar2.Minimum = 1;
            trackBar2.Maximum = 4095;
            trackBar2.Value = 1000;
        }

        void displayFPS()
        {
            if (DateTime.Now >= NextFPSUUpdate)
            {
                this.Text = String.Format("CT Visualizer (fps={0})", FrameCount);
                NextFPSUUpdate = DateTime.Now.AddSeconds(1);
                FrameCount = 0;
            }
            FrameCount++;
        }

        private void glControl1_Load(object sender, EventArgs e)
        {
          
        }

        private void открытьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                string str = dialog.FileName;
                bin.readBIN(str);
                trackBar1.Maximum = Bin.Z - 1;
                view.SetupView(glControl1.Width, glControl1.Height);
                loaded = true;
                glControl1.Invalidate();
            }
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            currentLayer = trackBar1.Value;
            needReload = true;
            glControl1.Invalidate();
        }

        bool needReload = false;
        private void glControl1_Paint(object sender, PaintEventArgs e)
        {
            if (loaded)
            {
                int minTF = trackBar1.Value;
                int widthTF = trackBar2.Value;

                if (Quads)
                {
                    view.DrawQuads(currentLayer, minTF, widthTF);
                    glControl1.SwapBuffers();
                }
                if (Texture)
                {
                    if (needReload)
                    {
                        view.generateTextureImage(currentLayer, minTF, widthTF);
                        view.Load2DTexture();
                        needReload = false;
                    }
                    view.DrawTexture();
                    glControl1.SwapBuffers();
                }
            }
        }

        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            needReload = true;
            glControl1.Invalidate();
        }

        void Application_Idle(object sender, EventArgs e)
        {
            while (glControl1.IsIdle)
            {
                displayFPS();
                glControl1.Invalidate();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Application.Idle += Application_Idle;
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            Quads = true;
            Texture = false;
            glControl1.Invalidate();
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            Texture = true;
            Quads = false;
            glControl1.Invalidate();
        }
    }
}
