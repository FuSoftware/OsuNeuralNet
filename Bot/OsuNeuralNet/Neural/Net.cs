using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Layer = System.Collections.Generic.List<OsuNeuralNet.Neural.Neuron>;

namespace OsuNeuralNet.Neural
{
    class Net
    {
        private List<Layer> m_layers; //m_layers[layerNum][neuronNum]
        private double m_error;
        private double m_recentAverageError;
        private static double m_recentAverageSmoothingFactor;

        public Net(List<int> topology)
        {
            int numLayers = topology.Count();
            for (int layerNum = 0; layerNum < numLayers; ++layerNum)
            {
                m_layers.Add(new Layer());

                //If the next layer isn't the last (output layer), uses the number of neurons in the next layer
                int numOutputs = layerNum == topology.Count() - 1 ? 0 : topology[layerNum + 1];

                //We have made a new Layer, now fill it with neurons, and
                // add a bias neuron to the layer

                for (int neuronNum = 0; neuronNum <= topology[layerNum]; ++neuronNum)
                {
                    m_layers.Last().Add(new Neuron(numOutputs, neuronNum));
                }

                //Forces the bias node's output value to 1.0. It's the last neuron created above
                m_layers.Last().Last().setOutputVal(1.0);
            }
        }

        public void feedForward(List<double> inputVals)
        {

            //Assign the inputs
            for (int i = 0; i < inputVals.Count(); ++i)
            {
                m_layers[0][i].setOutputVal(inputVals[i]);
            }

            //Forward propagate
            for (int layerNum = 1; layerNum < m_layers.Count(); ++layerNum)
            {
                Layer prevLayer = m_layers[layerNum - 1];
                for (int n = 0; n < m_layers[layerNum].Count() - 1; ++n)
                {
                    m_layers[layerNum][n].feedForward(prevLayer);
                }
            }
        }

        public void backProp(List<double> targetVals)
        {
            // Calculate overall net error (RMS of output errors)
            Layer outputLayer = m_layers.Last();
            m_error = 0.0;

            for (int n = 0; n < outputLayer.Count() - 1; ++n)
            {
                double delta = targetVals[n] - outputLayer[n].getOutputVal();
                m_error += delta * delta;
            }

            m_error /= outputLayer.Count() - 1; //Get average error squared
            m_error = Math.Sqrt(m_error); //RMS

            //Implement a recent average measurement :
            m_recentAverageError = (m_recentAverageError * m_recentAverageSmoothingFactor + m_error) / (m_recentAverageSmoothingFactor + 1.0);

            // Calculate output layer gradients
            for (int n = 0; n < outputLayer.Count() - 1; ++n)
            {
                outputLayer[n].calcOutputGradients(targetVals[n]);
            }

            //Calculate gradientso n hidden layers
            for (int layerNum = m_layers.Count() - 2; layerNum > 0; --layerNum)
            {
                Layer hiddenLayer = m_layers[layerNum];
                Layer nextLayer = m_layers[layerNum + 1];

                for (int n = 0; n < hiddenLayer.Count(); ++n)
                {
                    hiddenLayer[n].calcHiddenGradients(nextLayer);
                }
            }

            // For all layers from output to first hidden layer,
            // update connection weights

            for (int layerNum = m_layers.Count() - 1; layerNum > 0; --layerNum)
            {
                Layer layer = m_layers[layerNum];
                Layer prevLayer = m_layers[layerNum - 1];

                for (int n = 0; n < layer.Count() - 1; ++n)
                {
                    layer[n].updateInputWeights(prevLayer);
                }
            }
        }

        public void getResults(List<double> resultVals)
        {
            resultVals.Clear();

            for (int n = 0; n < m_layers.Last().Count() - 1; ++n)
            {
                resultVals.Add(m_layers.Last()[n].getOutputVal());
            }
        }

        public double getRecentAverageError()
        {
            return this.m_recentAverageError;
        }

        public List<double> getWeights()
        {
            List<double> weights = new List<double>();
            foreach (Layer l in m_layers)
            {
                foreach (Neuron n in l)
                {
                    weights.Add(n.getOutputVal());
                }
            }

            return weights;
        }

        public void setWeights(List<double> weights)
        {

        }
    }
}
