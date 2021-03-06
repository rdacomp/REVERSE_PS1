using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Xml.Serialization;
using System.IO;

namespace NeuralDemo
{
    public partial class Form1 : Form
    {
        [DllImport("kernel32")]
        static extern bool AllocConsole();

        private XO nn = null;

        Point lastPoint = Point.Empty;
        bool isMouseDown = new Boolean();

        public Form1()
        {
            InitializeComponent();

            AllocConsole();
        }

        private void loadSourceImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult res = openFileDialog1.ShowDialog();

            if ( res == DialogResult.OK)
            {
                string filename = openFileDialog1.FileName;

                pictureBox1.Image = Image.FromFile(filename);

                pictureBox2.Image = Convolution(pictureBox1.Image);
                pictureBox2.Invalidate();
            }
        }

        /// <summary>
        /// Downscale image
        /// </summary>
        /// <param name="source">Any image</param>
        /// <returns>Smaller image</returns>
        private Image Convolution (Image source)
        {
            int smallerSize = pictureBox2.Width;

            return ResizeImage(source, smallerSize, smallerSize);
        }

        // Source: https://stackoverflow.com/questions/1922040/resize-an-image-c-sharp

        /// <summary>
        /// Resize the image to the specified width and height.
        /// </summary>
        /// <param name="image">The image to resize.</param>
        /// <param name="width">The width to resize to.</param>
        /// <param name="height">The height to resize to.</param>
        /// <returns>The resized image.</returns>
        public static Bitmap ResizeImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }

        /// <summary>
        /// Check everything is ok
        /// </summary>
        /// <returns></returns>
        private bool CheckEnvironment ()
        {
            if (nn == null)
            {
                MessageBox.Show("Load or create network first", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return false;
            }

            if (pictureBox1.Image == null)
            {
                MessageBox.Show("Load image to guess or train", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return false;
            }

            //pictureBox2.Image = Convolution(pictureBox1.Image);
            //pictureBox2.Invalidate();

            return true;
        }

        /// <summary>
        /// Guess pattern
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            if (!CheckEnvironment())
            {
                return;
            }

            Image image = ResizeImage(pictureBox1.Image, 16, 16);

            XO.FeatureType featureType = nn.Guess(image);

            label1.Text = featureType.ToString();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormAbout form = new FormAbout();

            form.ShowDialog();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Train X
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            if (!CheckEnvironment())
            {
                return;
            }

            nn.Train(pictureBox2.Image, XO.FeatureType.X);
        }

        /// <summary>
        /// Train O
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button4_Click(object sender, EventArgs e)
        {
            if (!CheckEnvironment())
            {
                return;
            }

            nn.Train(pictureBox2.Image, XO.FeatureType.O);
        }

        /// <summary>
        /// Load network state
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void loadNetworkToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult res = openFileDialog2.ShowDialog();

            if (res == DialogResult.OK)
            {
                string filename = openFileDialog2.FileName;

                XmlSerializer ser = new XmlSerializer(typeof(XO.State));

                using (FileStream fs = new FileStream(filename, FileMode.Open))
                {
                    XO.State state = (XO.State)ser.Deserialize(fs);

                    nn = new XO(state);
                }
            }
        }

        /// <summary>
        /// Save network state
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveNetworkToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (nn == null)
            {
                MessageBox.Show("Load or create network first", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            DialogResult res = saveFileDialog1.ShowDialog();

            if (res == DialogResult.OK)
            {
                string filename = saveFileDialog1.FileName;

                XmlSerializer ser = new XmlSerializer(typeof(XO.State));

                using (FileStream fs = new FileStream(filename, FileMode.Create))
                {
                    ser.Serialize(fs, nn._state);
                }
            }
        }

        /// <summary>
        /// Child born
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void createNewNetworkToolStripMenuItem_Click(object sender, EventArgs e)
        {
            nn = new XO();
        }

        //
        // Free-hand drawing
        //
        // https://simpledevcode.wordpress.com/2014/02/06/drawing-by-mouse-on-a-picturebox-freehand-drawing/
        // (I modified blank image creation)
        //

        private void button5_Click(object sender, EventArgs e)
        {
            pictureBox1.Image = Properties.Resources.blank;

            pictureBox2.Image = Convolution(pictureBox1.Image);
            pictureBox2.Invalidate();
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            lastPoint = e.Location;
            isMouseDown = true;
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (isMouseDown == true)        //check to see if the mouse button is down
            {
                if (lastPoint != null)      //if our last point is not null, which in this case we have assigned above
                {
                    if (pictureBox1.Image == null)//if no available bitmap exists on the picturebox to draw on
                    {
                        pictureBox1.Image = Properties.Resources.blank;

                        pictureBox2.Image = Convolution(pictureBox1.Image);
                        pictureBox2.Invalidate();
                    }
                    
                    using (Graphics g = Graphics.FromImage(pictureBox1.Image))
                    {   //we need to create a Graphics object to draw on the picture box, its our main tool
                        //when making a Pen object, you can just give it color only or give it color and pen size
                        
                        g.DrawLine(new Pen(Color.Black, 3), lastPoint, e.Location);
                        g.SmoothingMode = SmoothingMode.AntiAlias;
                        //this is to give the drawing a more smoother, less sharper look
                    }

                    pictureBox1.Invalidate();   //refreshes the picturebox
                    lastPoint = e.Location;     //keep assigning the lastPoint to the current mouse position

                    pictureBox2.Image = Convolution(pictureBox1.Image);
                    pictureBox2.Invalidate();
                }
            }
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            isMouseDown = false;
            lastPoint = Point.Empty;
        }

    }
}
