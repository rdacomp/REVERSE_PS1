// My first NN attempts

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

namespace NeuralDemo
{
    public class XO
    {
        private const double learningRate = 0.3;
        private const int imageSize = 16;

        private NeuralNetwork nn;
        public State _state;

        public class State
        {
            public double _learningRate;
            public double[][] _weightHiddenOutput;
            public double[][] _weightInputHidden;
        }

        public enum FeatureType
        {
            X,
            O,

            Max,

            Unknown,
        }

        public XO()
        {
            nn = new NeuralNetwork(imageSize * imageSize, 1000, 2, learningRate);

            _state = new State();

            _state._learningRate = learningRate;
            _state._weightHiddenOutput = nn._weightHiddenOutput._matrix;
            _state._weightInputHidden = nn._weightInputHidden._matrix;
        }

        public XO(State state)
        {
            nn = new NeuralNetwork(state._weightHiddenOutput, state._weightInputHidden, state._learningRate);

            _state = state;
        }

        private double [] ConvertImageToVector ( Image image )
        {
            Bitmap bitmap = new Bitmap(image);
            double[] inputs = new double[imageSize * imageSize];
            int i = 0;

            //
            // Create input vector
            //

            for (int y = 0; y < imageSize; y++)
            {
                for (int x = 0; x < imageSize; x++)
                {
                    Color color = bitmap.GetPixel(x, y);

                    double blue = color.B;
                    double green = color.G;
                    double red = color.R;

                    byte Y = (byte)((0.299 * red) + (0.587 * green) + (0.114 * blue));
                    inputs[i] = (double)Y / 256F;

                    if (inputs[i] > 0.99)
                    {
                        inputs[i] = 0.99;
                    }

                    if ( inputs[i] < 0.9)
                    {
                        inputs[i] = 0.1F;
                    }

                    i++;
                }
            }

            return inputs;
        }

        public static void ShowVector(double[] vector, int decimals,
              int lineLen, bool newLine)
        {
            for (int i = 0; i < vector.Length; ++i)
            {
                if (i > 0 && i % lineLen == 0) Console.WriteLine("");
                if (vector[i] >= 0) Console.Write(" ");

                if (vector[i] >= 0.9F)
                {
                    Console.Write("     ");
                }
                else
                {
                    Console.Write(vector[i].ToString("F" + decimals) + " ");
                }
            }
            if (newLine == true)
                Console.WriteLine("");
        }

        public void Train(Image image, FeatureType feature)
        {
            double[] inputs = ConvertImageToVector (image);

            ShowVector(inputs, 3, 16, true);

            //
            // Create outputs
            //

            double[] outputs = new double[(int)FeatureType.Max];

            for (int i=0; i<(int)FeatureType.Max; i++)
            {
                if ( i == (int)feature)
                {
                    outputs[i] = 0.99;
                }
                else
                {
                    outputs[i] = 0.01;
                }
            }

            //
            // Train
            //

            nn.Train(inputs, outputs);

            //
            // Save state
            //

            _state._weightHiddenOutput = nn._weightHiddenOutput._matrix;
            _state._weightInputHidden = nn._weightInputHidden._matrix;
        }

        public FeatureType Guess(Image image)
        {
            double[] inputs = ConvertImageToVector(image);

            ShowVector(inputs, 3, 16, true);

            double[] outputs = nn.Query(inputs);

            Console.WriteLine(outputs[0].ToString() + ", " + outputs[1].ToString());

            //
            // Choose max and make sure the network solution is correct
            //

            double maxValue = outputs.Max();
            int maxIndex = outputs.ToList().IndexOf(maxValue);

            if ( outputs[maxIndex] > 0.95 )
            {
                return (FeatureType)maxIndex;
            }
            else
            {
                return FeatureType.Unknown;
            }
        }

    }

}
