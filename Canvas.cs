using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace KPO
{
    public partial class Canvas : Form
    {
        private ImageFormat imageFormat;
        private Bitmap bmp;
        private Point from;
        private Point to;

        private string fileName = "";
        private bool isClicked = false;
        private bool isChanged = false;

        public int CanvasWidth
        {
            get
            {
                return pictureBox.Width;
            }

            set
            {
                Width = value + 16;
                pictureBox.Width = value;
                Bitmap tbmp = new Bitmap(value, pictureBox.Height);
                Graphics g = Graphics.FromImage(tbmp);
                g.Clear(Color.White);
                g.DrawImage(bmp, new Point(0, 0));
                bmp = tbmp;
                pictureBox.Image = bmp;
            }
        }

        public int CanvasHeight
        {
            get
            {
                return pictureBox.Height;
            }

            set
            {
                Height = value + 39;
                pictureBox.Height = value;
                Bitmap tbmp = new Bitmap(pictureBox.Width, value);
                Graphics g = Graphics.FromImage(tbmp);
                g.Clear(Color.White);
                g.DrawImage(bmp, new Point(0, 0));
                bmp = tbmp;
                pictureBox.Image = bmp;
            }
        }

        public string getFileName(string fileName)
        {
            return System.IO.Path.GetFileNameWithoutExtension(fileName);
        }

        public Canvas()
        {
            InitializeComponent();
            bmp = new Bitmap(ClientSize.Width, ClientSize.Height);
            pictureBox.Image = bmp;
        }

        public Canvas(string FileName)
        {
            InitializeComponent();
            bmp = new Bitmap(FileName);
            Width = bmp.Width + 16;
            Height = bmp.Height + 39;
            pictureBox.Width = bmp.Width;
            pictureBox.Height = bmp.Height;
            pictureBox.Image = bmp;
            fileName = FileName;
            Text = getFileName(fileName);
        }

        private void pictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            isChanged = true;
            isClicked = true;
            from = e.Location;
        }

        private void pictureBox_MouseUp(object sender, MouseEventArgs e)
        {
            isChanged = true;
            isClicked = false;
            Graphics g = Graphics.FromImage(bmp);
            if (MainForm.isLinePenActive)
            {
                g.DrawLine(new Pen(MainForm.CurrentColor, MainForm.CurrentWidth), from, to);
            } 
            else if (MainForm.isEllipseActive)
            {
                g.DrawEllipse(new Pen(MainForm.CurrentColor, MainForm.CurrentWidth), from.X, from.Y,
                    to.X - from.X, to.Y - from.Y);
            }
            else if (MainForm.isStarActive)
            {
                DrawStar(from, to, new Pen(MainForm.CurrentColor, MainForm.CurrentWidth), MainForm.numberOfStarPeaks, g);
            }
        }

        private void pictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isChanged = true;
                Graphics g = Graphics.FromImage(bmp);

                if (MainForm.isEraserActive)
                {
                    g.FillEllipse(new SolidBrush(pictureBox.BackColor), e.X, e.Y, MainForm.CurrentWidth, MainForm.CurrentWidth);
                    from = e.Location;
                }
                else if ((MainForm.isLinePenActive || MainForm.isEllipseActive || MainForm.isStarActive) && isClicked)
                {
                    to = e.Location;
                }
                else
                {
                    Pen pen = new Pen(MainForm.CurrentColor, MainForm.CurrentWidth)
                    {
                        StartCap = System.Drawing.Drawing2D.LineCap.Round,
                        EndCap = System.Drawing.Drawing2D.LineCap.Round
                    };
                    g.DrawLine(pen, from, e.Location);
                    from = e.Location;
                }

                pictureBox.Invalidate();
            }
            MainForm parent = (MainForm)MdiParent;
            parent.SetStatusBarText($"X:{e.X}; Y:{e.Y}");
        }

        private void pictureBox_Paint(object sender, PaintEventArgs e)
        {
            if (MainForm.isLinePenActive)
            {
                e.Graphics.DrawLine(new Pen(MainForm.CurrentColor, MainForm.CurrentWidth), from, to);
            }
            else if (MainForm.isEllipseActive)
            {
                e.Graphics.DrawEllipse(new Pen(MainForm.CurrentColor, MainForm.CurrentWidth), from.X, from.Y,
                    to.X - from.X, to.Y - from.Y);
            }
            else if (MainForm.isStarActive)
            {
                DrawStar(from, to, new Pen(MainForm.CurrentColor, MainForm.CurrentWidth), MainForm.numberOfStarPeaks, e.Graphics);
            }
        }

        private void DrawStar(Point from, Point to, Pen pen, int number, Graphics g)
        {
            int n = number; 
            if (n < 1) throw new Exception("Количество вершин звезды меньше 1");
            int r1 = Math.Abs(to.X - from.X);
            int r2 = Math.Abs(to.Y - from.Y);
            double r = r1 < r2 ? r2 / 2 : r1 / 2, R = r / 2;   
            double alpha = 0;       

            PointF[] points = new PointF[2 * n + 1];
            double a = alpha, da = Math.PI / n, l;
            for (int k = 0; k < 2 * n + 1; k++)
            {
                l = k % 2 == 0 ? r : R;
                points[k] = new PointF((float)((from.X + to.X) / 2 + l * Math.Cos(a)), (float)((from.Y + to.Y) / 2 + l * Math.Sin(a)));
                a += da;
            }

            g.DrawLines(pen, points);
        }

        public void Save()
        {
            if (fileName.Length == 0)
            {
                if (SaveAs())
                {
                    isChanged = false;
                    MessageBox.Show("Файл был успешно сохранен.", "Сохранение", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else
            {
                SaveFile();
                isChanged = false;
                MessageBox.Show("Файл был успешно сохранен.", "Сохранение", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        public bool SaveAs()
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.AddExtension = true;
            dlg.Filter = "Windows Bitmap (*.bmp)|*.bmp| Файлы JPEG (*.jpg)|*.jpg";
            dlg.FileName = Text;
            ImageFormat[] ff = { ImageFormat.Bmp, ImageFormat.Jpeg };

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                fileName = dlg.FileName;
                imageFormat = ff[dlg.FilterIndex - 1];

                SaveFile();

                Text = getFileName(fileName);

                return true;
            }

            return false;
        }

        private void SaveFile()
        {
            Bitmap blank = new Bitmap(bmp);
            Graphics g = Graphics.FromImage(blank);
            g.Clear(Color.White);
            g.DrawImage(bmp, 0, 0, bmp.Width, bmp.Height);

            Bitmap output = new Bitmap(blank);
            blank.Dispose();
            bmp.Dispose();

            output.Save(fileName, imageFormat);

            bmp = new Bitmap(output);
            pictureBox.Image = bmp;
        }

        public void PlusScale()
        {
            Image old = pictureBox.Image;
            pictureBox.Width *= 2;
            pictureBox.Height *= 2;
            bmp = new Bitmap(pictureBox.Width, pictureBox.Height);
            Graphics d = Graphics.FromImage(bmp);
            d.DrawImage(old, 0, 0, old.Width * 2, old.Height * 2);
            pictureBox.Image = bmp;
        }

        public void MinusScale()
        {
            Image old = pictureBox.Image;
            pictureBox.Width /= 2;
            pictureBox.Height /= 2;
            bmp = new Bitmap(pictureBox.Width, pictureBox.Height);
            Graphics d = Graphics.FromImage(bmp);
            d.DrawImage(old, 0, 0, old.Width / 2, old.Height / 2);
            pictureBox.Image = bmp;
        }

        private void Canvas_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Сохранить изменения?", "Закрытие файла \"" + Text + "\"", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Information);
            if (dialogResult == DialogResult.Yes)
            {
                Save();
            }
            else if (dialogResult == DialogResult.Cancel)
            {
                e.Cancel = true;
            }
        }
    }
}
