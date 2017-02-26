using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Layer = System.Collections.Generic.List<OsuNeuralNet.Neural.Neuron>;

namespace OsuNeuralNet.Neural
{
    public class Connection
    {
        public double weight;
        public double deltaWeight;
    }

    class Neuron
    {
        double m_outputVal;
        List<Connection> m_outputWeights = new List<Connection>();
        int m_myIndex;
        double m_gradient;

        static double alpha = 0.0; //[0.0..1.0] overall net training rate
        static double eta = 0.0; //[0.0..n] multiplier of last weight charge (momentum)

        public Neuron(int numOutputs, int myIndex)
        {
            for (int c = 0; c < numOutputs; ++c)
            {
                Connection conn = new Connection();
                conn.weight = randomWeight();
                m_outputWeights.Add(conn);
            }

            m_myIndex = myIndex;
        }

        public List<double> getWeights()
        {
            List<double> weights = new List<double>();
            foreach (Connection c in m_outputWeights)
            {
                weights.Add(c.weight);
            }
            return weights;
        }

        public void setOutputVal(double val)
        {
            this.m_outputVal = val;
        }

        public double getOutputVal() 
        {
            return this.m_outputVal;
        }

        public void feedForward(Layer prevLayer)
        {
            double sum = 0;

            //Sums the previous layer's outputs (our inputs)
            //Include the bias node from the previous layer

            for (int n = 0; n < prevLayer.Count(); ++n)
            {
                sum += prevLayer[n].getOutputVal() * prevLayer[n].m_outputWeights[m_myIndex].weight;
            }

            m_outputVal = transferFunction(sum);
        }

        public void calcOutputGradients(double targetVal)
        {
            double delta = targetVal - m_outputVal;
            m_gradient = delta * transferFunctionDerivative(m_outputVal);
        }

        public void calcHiddenGradients(Layer nextLayer)
        {
            double dow = sumDOW(nextLayer);
            m_gradient = dow * transferFunctionDerivative(m_outputVal);
        }

        public void updateInputWeights(Layer prevLayer)
        {
            //The weights to be updated are in the connection container
            //in the neuron in the preceding layer

            for (int n = 0; n < prevLayer.Count() - 1; ++n)
            {
                Neuron neuron = prevLayer[n];
                double oldDeltaWeight = neuron.m_outputWeights[m_myIndex].deltaWeight;

                double newDeltaWeight =
                        //Individual input, magnified by the gradient and train rate
                        eta
                        * neuron.getOutputVal()
                        * m_gradient
                        //Also add momentum = a fraction of the previous delta weight
                        + alpha
                        * oldDeltaWeight;
                neuron.m_outputWeights[m_myIndex].deltaWeight = newDeltaWeight;
                neuron.m_outputWeights[m_myIndex].weight += newDeltaWeight;
            }
        }

        private static double randomWeight()
        {
            Random r = new Random();
            return r.NextDouble();
        }
        private static double transferFunction(double x)
        {
            //tanh - output range [-1.0..1.0]
            return Math.Tanh(x);
        }
        private static double transferFunctionDerivative(double x)
        {
            //tanh derivative
            return 1.0 - x * x;
        }
        private double sumDOW(Layer nextLayer)
        {
            double sum = 0.0;

            //Sum our contributions of the errors at the nodes we feed

            for (int n = 0; n < nextLayer.Count() - 1; ++n)
            {
                sum += m_outputWeights[n].weight * nextLayer[n].m_gradient;
            }

            return sum;
        }
    }
}
