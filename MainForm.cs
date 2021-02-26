using PluginInterface;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace KPO
{
    public partial class MainForm : Form
    {
        public static Color CurrentColor = Color.Black;
        public static Color ActiveColor = Color.Silver;
        public static Color PassiveColor = SystemColors.Control;
        public static int CurrentWidth = 3;
        public static int numberOfStarPeaks = 0;

        public static bool isEraserActive = false;
        public static bool isLinePenActive = false;
        public static bool isEllipseActive = false;
        public static bool isStarActive = false;
        
        Dictionary<string, IPlugin> plugins = new Dictionary<string, IPlugin>();

        public MainForm()
        {
            InitializeComponent();
            FindPlugins();
            CreateMenu();
        }

        private int documentCounter = 1;

        private void новыйToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Canvas d = new Canvas();
            d.Text = $"Документ {documentCounter++}";
            d.MdiParent = this;
            d.Show();
        }

        private void оПрограммеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutPaint aboutPaint = new AboutPaint();
            aboutPaint.ShowDialog();
        }

        private void вертикальноToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.TileHorizontal);
        }

        private void каскадомToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.Cascade);
        }

        private void слеваНаправоToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.TileVertical);
        }

        private void упорядочитьЗначкиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.ArrangeIcons);
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            CurrentColor = Color.Red;
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            CurrentColor = Color.Green;
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            CurrentColor = Color.Blue;
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            ColorDialog d = new ColorDialog();

            if (d.ShowDialog() == DialogResult.OK)
            {
                CurrentColor = d.Color;
            }
        }

        public void SetStatusBarText(string Text)
        {
            toolStripStatusLabel1.Text = Text;
        }

        private void красныйToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CurrentColor = Color.Red;
        }

        private void синийToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CurrentColor = Color.Blue;
        }

        private void зеленыйToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CurrentColor = Color.Green;
        }

        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void рисунокToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            размерХолстаToolStripMenuItem.Enabled = !(ActiveMdiChild == null);
        }

        private void файлToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            сохранитьToolStripMenuItem.Enabled = !(ActiveMdiChild == null);
            сохранитьКакToolStripMenuItem.Enabled = !(ActiveMdiChild == null);
        }

        private void размерХолстаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CanvasSize cs = new CanvasSize();
            cs.CanvasWidth = ((Canvas)ActiveMdiChild).CanvasWidth;
            cs.CanvasHeight = ((Canvas)ActiveMdiChild).CanvasHeight;
            if (cs.ShowDialog() == DialogResult.OK)
            {
                ((Canvas)ActiveMdiChild).CanvasWidth = cs.CanvasWidth;
                ((Canvas)ActiveMdiChild).CanvasHeight = cs.CanvasHeight;
            }
        }

        private void другойToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColorDialog cd = new ColorDialog();
            if (cd.ShowDialog() == DialogResult.OK)
                CurrentColor = cd.Color;
        }

        private void открытьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Windows Bitmap (*.bmp)|*.bmp| Файлы JPEG (*.jpeg, *.jpg)|*.jpeg;*.jpg|Все файлы ()*.*|*.*";

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                Canvas frmChild = new Canvas(dlg.FileName);
                frmChild.MdiParent = this;
                frmChild.Show();
            }
        }

        private void сохранитьКакToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ((Canvas)ActiveMdiChild).SaveAs();
        }

        private void сохранитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ((Canvas)ActiveMdiChild).Save();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            textBoxBrushSize.Text = CurrentWidth.ToString();
        }

        private void textBoxBrushSize_KeyPress(object sender, KeyPressEventArgs e)
        {
            char number = e.KeyChar;
            if (!char.IsDigit(e.KeyChar) && number != 8)
            {
                e.Handled = true;
            }
        }

        private void textBoxBrushSize_TextChanged(object sender, EventArgs e)
        {
            try
            {
                CurrentWidth = Convert.ToInt32(textBoxBrushSize.Text);
            }
            catch
            {
                MessageBox.Show("Значение должно быть целым положительным числом.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void eraserToolStripButton_Click(object sender, EventArgs e)
        {
            isEraserActive = !isEraserActive;
            eraserToolStripButton.BackColor = isEraserActive ? ActiveColor : PassiveColor;
            if (isEraserActive)
            {
                isLinePenActive = false;
                linePenToolStripButton.BackColor = PassiveColor;
                isEllipseActive = false;
                ellipseToolStripButton.BackColor = PassiveColor;
                isStarActive = false;
                starToolStripButton.BackColor = PassiveColor;
            }
        }

        private void linePenToolStripButton_Click(object sender, EventArgs e)
        {
            isLinePenActive = !isLinePenActive;
            linePenToolStripButton.BackColor = isLinePenActive ? ActiveColor : PassiveColor;
            if (isLinePenActive)
            {
                isEraserActive = false;
                eraserToolStripButton.BackColor = PassiveColor;
                isEllipseActive = false;
                ellipseToolStripButton.BackColor = PassiveColor;
                isStarActive = false;
                starToolStripButton.BackColor = PassiveColor;
            }
        }

        private void ellipseToolStripButton_Click(object sender, EventArgs e)
        {
            isEllipseActive = !isEllipseActive;
            ellipseToolStripButton.BackColor = isEllipseActive ? ActiveColor : PassiveColor;
            if (isEllipseActive)
            {
                isLinePenActive = false;
                linePenToolStripButton.BackColor = PassiveColor;
                isEraserActive = false;
                eraserToolStripButton.BackColor = PassiveColor;
                isStarActive = false;
                starToolStripButton.BackColor = PassiveColor;
            }
        }

        private void starToolStripButton_Click(object sender, EventArgs e)
        {
            isStarActive = !isStarActive;
            starToolStripButton.BackColor = isStarActive ? ActiveColor : PassiveColor;
            if (isStarActive)
            {
                isLinePenActive = false;
                linePenToolStripButton.BackColor = PassiveColor;
                isEraserActive = false;
                eraserToolStripButton.BackColor = PassiveColor;
                isEllipseActive = false;
                ellipseToolStripButton.BackColor = PassiveColor;
                StarSize starSize = new StarSize();
                if (starSize.ShowDialog() == DialogResult.OK)
                {
                    numberOfStarPeaks = starSize.getNumber();
                }
                else
                {
                    isStarActive = false;
                    starToolStripButton.BackColor = isStarActive ? ActiveColor : PassiveColor;
                }
            }
        }

        private void plusScaleToolStripButton_Click(object sender, EventArgs e)
        {
            isEraserActive = false;
            eraserToolStripButton.BackColor = PassiveColor;
            isLinePenActive = false;
            linePenToolStripButton.BackColor = PassiveColor;
            isEllipseActive = false;
            ellipseToolStripButton.BackColor = PassiveColor;
            isStarActive = false;
            starToolStripButton.BackColor = PassiveColor;

            ((Canvas)ActiveMdiChild).PlusScale();
        }

        private void minusScaleToolStripButton_Click(object sender, EventArgs e)
        {
            isEraserActive = false;
            eraserToolStripButton.BackColor = PassiveColor;
            isLinePenActive = false;
            linePenToolStripButton.BackColor = PassiveColor;
            isEllipseActive = false;
            ellipseToolStripButton.BackColor = PassiveColor;
            isStarActive = false;
            starToolStripButton.BackColor = PassiveColor;

            ((Canvas)ActiveMdiChild).MinusScale();
        }

        void FindPlugins()
        {
            // папка с плагинами
            string folder = AppDomain.CurrentDomain.BaseDirectory;

            // dll-файлы в этой папке
            string[] files = Directory.GetFiles(folder, "*.dll");

            foreach (string file in files)
                try
                {
                    Assembly assembly = Assembly.LoadFile(file);
                    foreach (Type type in assembly.GetTypes())
                    {
                        Type iface = type.GetInterface("PluginInterface.IPlugin");

                        if (iface != null)
                        {
                            IPlugin plugin = (IPlugin)Activator.CreateInstance(type);
                            plugins.Add(plugin.Name, plugin);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка загрузки плагина\n" + ex.Message);
                }
        }

        void CreateMenu()
        {
            foreach (IPlugin plugin in plugins.Values)
            {
                var menuItem = new ToolStripMenuItem(plugin.Name);
                menuItem.Click += OnPluginClick;
                VersionAttribute versionAttribute = (VersionAttribute)Attribute.GetCustomAttribute(plugin.GetType(), typeof(VersionAttribute));
                menuItem.ToolTipText = $"Автор: {plugin.Author}\nВерсия: {versionAttribute.Major}.{versionAttribute.Minor}";
                фильтрыToolStripMenuItem.DropDownItems.Add(menuItem);
            }
        }

        private void OnPluginClick(object sender, EventArgs args)
        {
            IPlugin plugin = plugins[((ToolStripMenuItem)sender).Text];
            plugin.Transform((Bitmap)((Canvas)ActiveMdiChild).pictureBox.Image);
            ((Canvas)ActiveMdiChild).pictureBox.Refresh();
        }
    }
}
