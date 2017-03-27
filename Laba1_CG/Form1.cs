using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Laba1_CG
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        SolidBrush cb;
        SolidBrush bb;
        SolidBrush rb;
        Pen bp;
        Pen dgp;
        Pen zp;
        Pen aap;

        Tochka3D[] tochki3D = new Tochka3D[14];
        Point[] pros_tochki2D = new Point[14];
        Point[] comp_tochki2D = new Point[12];
        int x, y, z;
        double alpha, beta, gamma;
        const int dlinaOsi = 90;
        int pOx, pOy;
        int cOx, cOy;
        Bitmap prosBm;
        Bitmap compBm;
        Graphics prosGr;
        Graphics compGr;
        const int smeshenie2 = 2;
        const int smeshenie5 = 5;
        const int smeshenie10 = 10;
        const int smeshenie15 = 15;
        const int smeshenie20 = 20;
        const int smeshenie25 = 25;
        const int rasmerFont = 10;
        const int diam = 5;
        bool repaintComp = false;

        private void Form1_Load(object sender, EventArgs e)
        {
            //inizializing
            bp = new Pen(Color.Black, 1);
            cb = new SolidBrush(Color.Cyan);
            bb = new SolidBrush(Color.Black);
            rb = new SolidBrush(Color.Red);

            dgp = new Pen(Color.Gray);
            dgp.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;

            zp = new Pen(Color.Black, 1.0f);
            zp.CustomStartCap = new System.Drawing.Drawing2D.AdjustableArrowCap(3.0f, 6.0f);

            aap = new Pen(Color.Black, 1.0f);
            aap.CustomEndCap = new System.Drawing.Drawing2D.AdjustableArrowCap(3.0f, 6.0f);
            aap.CustomStartCap = new System.Drawing.Drawing2D.AdjustableArrowCap(3.0f, 6.0f);

            prosBm = new Bitmap(pictureBoxPros.Width, pictureBoxPros.Height);
            compBm = new Bitmap(pictureBoxComp.Width, pictureBoxComp.Height);

            repaintComp = true;
            alpha = trackBarAlpha.Value / 180.0 * Math.PI;
            beta = trackBarBeta.Value / 180.0 * Math.PI;
            gamma = trackBarGamma.Value / 180.0 * Math.PI;

            pOx = pictureBoxPros.Height / 2;
            pOy = pictureBoxPros.Width / 2;
            cOx = pictureBoxComp.Height / 2;
            cOy = pictureBoxComp.Width / 2;

            trackBarsChange(null, null);
        }

        private void trackBarsChange(object sender, EventArgs e)
        {
            input();
            process();
            output();
        }

        private void pictureBoxPros_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawImageUnscaled(prosBm, 0, 0);
        }

        private void pictureBoxComp_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawImageUnscaled(compBm, 0, 0);
        }
        
        private void buttonExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        void updateTochki3D()
        {
            tochki3D[0] = new Tochka3D(0, 0, 0); //O
            tochki3D[1] = new Tochka3D(dlinaOsi, 0, 0);   //X+
            tochki3D[2] = new Tochka3D(-dlinaOsi, 0, 0);  //X-
            tochki3D[3] = new Tochka3D(0, dlinaOsi, 0);   //Y+
            tochki3D[4] = new Tochka3D(0, -dlinaOsi, 0);  //Y-
            tochki3D[5] = new Tochka3D(0, 0, dlinaOsi);   //Z+
            tochki3D[6] = new Tochka3D(0, 0, -dlinaOsi);  //Z-
            tochki3D[7] = new Tochka3D(x, 0, 0); //Tx
            tochki3D[8] = new Tochka3D(0, y, 0); //Ty
            tochki3D[9] = new Tochka3D(0, 0, z); //Tz
            tochki3D[10] = new Tochka3D(x, y, 0); //T1
            tochki3D[11] = new Tochka3D(x, 0, z); //T2
            tochki3D[12] = new Tochka3D(0, y, z); //T3
            tochki3D[13] = new Tochka3D(x, y, z); //T
        }

        void checkRepaintComp()
        {
            if (trackBarAlpha.Value / 180.0 * Math.PI != alpha || trackBarBeta.Value / 180.0 * Math.PI != beta || trackBarGamma.Value / 180.0 * Math.PI != gamma)
            {
                repaintComp = true;
                alpha = trackBarAlpha.Value / 180.0 * Math.PI;
                beta = trackBarBeta.Value / 180.0 * Math.PI;
                gamma = trackBarGamma.Value / 180.0 * Math.PI;
            }
        }

        private void input()
        {
            checkRepaintComp();

            //vhodnye dannye
            x = trackBarX.Value;
            y = trackBarY.Value;
            z = trackBarZ.Value;
            
            updateTochki3D();     
        }

        private void process()
        {
            processPros();
            if (repaintComp)
                processComp();
        }

        private void output()
        {
            outputPros(prosGr);
            if (repaintComp)
                outputComp(compGr);

            labelXYZ.Text = x.ToString() + "," + y.ToString() + "," + z.ToString();
            labelAlpha.Text = trackBarAlpha.Value.ToString();
            labelBeta.Text = trackBarBeta.Value.ToString();
            labelGamma.Text = trackBarGamma.Value.ToString();
            repaintComp = false;
        }

        private Point transform3Dto2DPros(Tochka3D p)
        {
           int x = Convert.ToInt32(-p.x*Math.Cos(alpha)-p.y*Math.Cos(beta)-p.z*Math.Cos(gamma) )+ pOx;
           int y = Convert.ToInt32(p.x * Math.Sin(alpha) + p.y * Math.Sin(beta) + p.z * Math.Sin(gamma)) + pOy;
           return new Point(x,y);
        }

        private void processPros()
        {
            for (int i = 0; i < 14; i++)
            {
                pros_tochki2D[i] = transform3Dto2DPros(tochki3D[i]);
            }
        }

        private void outputPros(Graphics prosGr)
        {
            prosGr = Graphics.FromImage(prosBm);
            prosGr.Clear(pictureBoxPros.BackColor);
            prosGr.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            outputProsLines(prosGr);
            outputProsTochki(prosGr);
            outputProsLabels(prosGr);

            pictureBoxPros.CreateGraphics().DrawImageUnscaled(prosBm, 0, 0);
        }

        void outputProsLines(Graphics prosGr)
        {
            //osi
            prosGr.DrawLine(zp, pros_tochki2D[1], pros_tochki2D[2]);
            prosGr.DrawLine(zp, pros_tochki2D[3], pros_tochki2D[4]);
            prosGr.DrawLine(zp, pros_tochki2D[5], pros_tochki2D[6]);

            prosGr.DrawLine(dgp, pros_tochki2D[7], pros_tochki2D[0]);
            prosGr.DrawLine(dgp, pros_tochki2D[7], pros_tochki2D[10]);
            prosGr.DrawLine(dgp, pros_tochki2D[7], pros_tochki2D[11]);
            prosGr.DrawLine(dgp, pros_tochki2D[8], pros_tochki2D[0]);
            prosGr.DrawLine(dgp, pros_tochki2D[8], pros_tochki2D[10]);
            prosGr.DrawLine(dgp, pros_tochki2D[8], pros_tochki2D[12]);
            prosGr.DrawLine(dgp, pros_tochki2D[9], pros_tochki2D[0]);
            prosGr.DrawLine(dgp, pros_tochki2D[9], pros_tochki2D[11]);
            prosGr.DrawLine(dgp, pros_tochki2D[9], pros_tochki2D[12]);
            prosGr.DrawLine(dgp, pros_tochki2D[10], pros_tochki2D[13]);
            prosGr.DrawLine(dgp, pros_tochki2D[11], pros_tochki2D[13]);
            prosGr.DrawLine(dgp, pros_tochki2D[12], pros_tochki2D[13]);
        }

        void outputProsTochki(Graphics prosGr)
        {
            for (int i = 7; i < 14; i++)
            {
                if (i >= 7 &  i < 10)
                {
                    prosGr.DrawEllipse(bp, pros_tochki2D[i].X - smeshenie2, pros_tochki2D[i].Y - smeshenie2, diam, diam);
                    prosGr.FillEllipse(bb, pros_tochki2D[i].X - smeshenie2, pros_tochki2D[i].Y - smeshenie2, diam, diam);
                }
                if (i >= 10 & i < 13)   //T1,T2,T3
                {
                    prosGr.DrawEllipse(bp, pros_tochki2D[i].X - smeshenie2, pros_tochki2D[i].Y - smeshenie2, diam, diam);
                    prosGr.FillEllipse(cb, pros_tochki2D[i].X - smeshenie2, pros_tochki2D[i].Y - smeshenie2, diam, diam);
                }

                if (i >= 13)    //T
                {
                    prosGr.DrawEllipse(bp, pros_tochki2D[i].X - smeshenie2, pros_tochki2D[i].Y - smeshenie2, diam, diam);
                    prosGr.FillEllipse(rb, pros_tochki2D[i].X - smeshenie2, pros_tochki2D[i].Y - smeshenie2, diam, diam);
                }
            }
        }

        void outputProsLabels(Graphics prosGr)
        {
            prosGr.DrawString("T1", new Font("Helvetica", rasmerFont), bb, pros_tochki2D[10].X - smeshenie5, pros_tochki2D[10].Y + smeshenie5);
            prosGr.DrawString("T2", new Font("Helvetica", rasmerFont), bb, pros_tochki2D[11].X - smeshenie25, pros_tochki2D[11].Y - smeshenie10);
            prosGr.DrawString("T3", new Font("Helvetica", rasmerFont), bb, pros_tochki2D[12].X + smeshenie5, pros_tochki2D[12].Y - smeshenie10);
            prosGr.DrawString("T", new Font("Helvetica", rasmerFont), bb, pros_tochki2D[13].X, pros_tochki2D[13].Y + smeshenie5);
            prosGr.DrawString("Tx", new Font("Helvetica", rasmerFont), bb, pros_tochki2D[7].X, pros_tochki2D[7].Y + smeshenie5);
            prosGr.DrawString("Ty", new Font("Helvetica", rasmerFont), bb, pros_tochki2D[8].X + smeshenie5, pros_tochki2D[8].Y - smeshenie10);
            prosGr.DrawString("Tz", new Font("Helvetica", rasmerFont), bb, pros_tochki2D[9].X + smeshenie5, pros_tochki2D[9].Y - smeshenie15);

            prosGr.DrawString("x", new Font("Helvetica", rasmerFont), bb, pros_tochki2D[1].X - smeshenie5, pros_tochki2D[1].Y);
            prosGr.DrawString("y", new Font("Helvetica", rasmerFont), bb, pros_tochki2D[3].X - smeshenie20, pros_tochki2D[3].Y - smeshenie10);
            prosGr.DrawString("z", new Font("Helvetica", rasmerFont), bb, pros_tochki2D[5].X - smeshenie15, pros_tochki2D[5].Y);
        }

        Point transform3Dto2DComp1(Tochka3D tochka)
        {
           return new Point(cOx - tochka.x, cOy + tochka.y);
        }

        Point transform3Dto2DComp2(Tochka3D tochka)
        {
           return new Point(cOx - tochka.x, cOy - tochka.z);
        }

        Point transform3Dto2DComp3(Tochka3D tochka)
        {
           return new Point(cOx + tochka.y, cOy - tochka.z);
        }

        private void processComp()
        {
                comp_tochki2D[0] = transform3Dto2DComp1(tochki3D[0]);   //O
                comp_tochki2D[1] = transform3Dto2DComp1(tochki3D[1]);   //X
                comp_tochki2D[2] = transform3Dto2DComp2(tochki3D[3]);   //Y
                comp_tochki2D[3] = transform3Dto2DComp3(tochki3D[3]);   //Y'
                comp_tochki2D[4] = transform3Dto2DComp3(tochki3D[5]);   //Z
                comp_tochki2D[5] = transform3Dto2DComp1(tochki3D[7]);   //Tx
                comp_tochki2D[6] = transform3Dto2DComp3(tochki3D[8]);   //Ty
                comp_tochki2D[7] = transform3Dto2DComp1(tochki3D[8]);   //Ty'
                comp_tochki2D[8] = transform3Dto2DComp3(tochki3D[9]);   //Tz
                comp_tochki2D[9] = transform3Dto2DComp1(tochki3D[10]);  //T1
                comp_tochki2D[10] = transform3Dto2DComp2(tochki3D[11]); //T2
                comp_tochki2D[11] = transform3Dto2DComp3(tochki3D[12]); //T3
        }

        private void outputComp(Graphics compGr)
        {
                compGr = Graphics.FromImage(compBm);
                compGr.Clear(pictureBoxComp.BackColor);
                compGr.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                outputCompLines(compGr);
                outputCompTochki(compGr);
                outputCompDuga(compGr);
                outputCompLabels(compGr);

                pictureBoxComp.CreateGraphics().DrawImageUnscaled(compBm, 0, 0);
        }

        void outputCompLines(Graphics compGr)
        {
            //osi
            compGr.DrawLine(aap, 0, (pictureBoxComp.Width / 2), pictureBoxComp.Height, pictureBoxComp.Width / 2);
            compGr.DrawLine(aap, pictureBoxComp.Height / 2, 0, pictureBoxComp.Height / 2, pictureBoxComp.Height);

            compGr.DrawLine(dgp, comp_tochki2D[9].X, comp_tochki2D[9].Y, comp_tochki2D[5].X, comp_tochki2D[5].Y); //soedinajem T1 s Tx
            compGr.DrawLine(dgp, comp_tochki2D[5].X, comp_tochki2D[5].Y, comp_tochki2D[10].X, comp_tochki2D[10].Y); //soedinajem Tx s T2
            compGr.DrawLine(dgp, comp_tochki2D[10].X, comp_tochki2D[10].Y, comp_tochki2D[8].X, comp_tochki2D[8].Y); //soedinajem T2 s Tz
            compGr.DrawLine(dgp, comp_tochki2D[8].X, comp_tochki2D[8].Y, comp_tochki2D[11].X, comp_tochki2D[11].Y);  //soedinajem Tz s T3
            compGr.DrawLine(dgp, comp_tochki2D[11].X, comp_tochki2D[11].Y, comp_tochki2D[6].X, comp_tochki2D[6].Y);  //soedinajem T3 s Ty
            compGr.DrawLine(dgp, comp_tochki2D[9].X, comp_tochki2D[9].Y, comp_tochki2D[7].X, comp_tochki2D[7].Y);  //soedinajem T1 s Ty'
        }

        void outputCompTochki(Graphics compGr)
        {
            for (int i = 5; i < 9; i++)
            {
                compGr.DrawEllipse(bp, comp_tochki2D[i].X - smeshenie2, comp_tochki2D[i].Y - smeshenie2, diam, diam);
                compGr.FillEllipse(bb, comp_tochki2D[i].X - smeshenie2, comp_tochki2D[i].Y - smeshenie2, diam, diam);
            }

            for (int i = 9; i < 12; i++)    //T1,T2,T3
            {
                compGr.DrawEllipse(bp, comp_tochki2D[i].X - smeshenie2, comp_tochki2D[i].Y - smeshenie2, diam, diam);
                compGr.FillEllipse(cb, comp_tochki2D[i].X - smeshenie2, comp_tochki2D[i].Y - smeshenie2, diam, diam);
            }
        }

        void outputCompDuga(Graphics compGr)
        {
            if (y < 0)  //soedinajem Ty s Ty'
            {
                compGr.DrawArc(dgp, cOx + y, cOy + y, -y * 2, -y * 2, 180, 90);
            }
            else
            {
                if (y > 0)
                    compGr.DrawArc(dgp, cOx - y, cOy - y, y * 2, y * 2, 0, 90);
            }
        }

        void outputCompLabels(Graphics compGr)
        {
            compGr.DrawString("X", new Font("Helvetica", rasmerFont), bb, comp_tochki2D[1].X - smeshenie25, comp_tochki2D[1].Y + smeshenie5);
            compGr.DrawString("Y", new Font("Helvetica", rasmerFont), bb, comp_tochki2D[2].X + smeshenie25, comp_tochki2D[2].Y + smeshenie5);
            compGr.DrawString("Y", new Font("Helvetica", rasmerFont), bb, comp_tochki2D[3].X + smeshenie5, comp_tochki2D[3].Y + smeshenie15);
            compGr.DrawString("Z", new Font("Helvetica", rasmerFont), bb, comp_tochki2D[4].X + smeshenie5, comp_tochki2D[4].Y - smeshenie25);
            compGr.DrawString("Tx", new Font("Helvetica", rasmerFont), bb, comp_tochki2D[5].X + smeshenie5, comp_tochki2D[5].Y + smeshenie5);
            compGr.DrawString("Ty'", new Font("Helvetica", rasmerFont), bb, comp_tochki2D[6].X + smeshenie5, comp_tochki2D[6].Y + smeshenie5);
            compGr.DrawString("Ty", new Font("Helvetica", rasmerFont), bb, comp_tochki2D[7].X + smeshenie5, comp_tochki2D[7].Y + smeshenie5);
            compGr.DrawString("Tz", new Font("Helvetica", rasmerFont), bb, comp_tochki2D[8].X + smeshenie5, comp_tochki2D[8].Y + smeshenie5);
            compGr.DrawString("T1", new Font("Helvetica", rasmerFont), bb, comp_tochki2D[9].X + smeshenie5, comp_tochki2D[9].Y + smeshenie5);
            compGr.DrawString("T2", new Font("Helvetica", rasmerFont), bb, comp_tochki2D[10].X + smeshenie5, comp_tochki2D[10].Y + smeshenie5);
            compGr.DrawString("T3", new Font("Helvetica", rasmerFont), bb, comp_tochki2D[11].X + smeshenie5, comp_tochki2D[11].Y + smeshenie5);
        }
    }
}