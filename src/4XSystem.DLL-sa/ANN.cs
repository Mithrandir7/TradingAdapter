
//
// 4X Lab.NET © Copyright 2005-2006 ASCSE LLC
// http://www.4xlab.net
// email: 4xlab@4xlab.net
//

using System;
using System.Collections;
using System.IO;


namespace _4XLab.NET
{
	// max 5 layers
	// fixed activation function per layer.

	public struct sNeuron
	{
		public double Value, Output, Bias;
	}
	
	public class oANN
	{
		int Layers;
		int[] NeuronsPerLayer;
		int[] ActivationFunction;
		int[] AFParameters;
		ArrayList Neurons;
		ArrayList Weigths;
	
		public oANN(string pFileName)
		{
			int linenumber=0;
			System.IO.TextReader tr=null; 
			String line;
			ArrayList Parameters = new ArrayList();

			NeuronsPerLayer = new int[5];
			ActivationFunction = new int[5];
			AFParameters = new int[20];
			Neurons = new ArrayList();
			Weigths = new ArrayList();


			// how to code this differently with a FSM?
			try
			{
				tr = new System.IO.StreamReader(pFileName);

				while ((line = tr.ReadLine()) != null)
				{
					Parameters.Add(double.Parse(line));
					linenumber++;	
				}

				tr.Close();

				for (int i=0; i<20; i++)
				{
					AFParameters[i]=(int)((double)Parameters[i]);
				}

				for (int i=0; i<5; i++)
				{
					NeuronsPerLayer[i]=(int)((double)Parameters[20+i]);
				}
				
				int NumBiases =	 NeuronsPerLayer[0]+NeuronsPerLayer[1]
								+NeuronsPerLayer[2]+NeuronsPerLayer[3]
								+NeuronsPerLayer[4];
				int NumWeigths = (NeuronsPerLayer[0]*NeuronsPerLayer[1])
								+(NeuronsPerLayer[1]*NeuronsPerLayer[2])
								+(NeuronsPerLayer[3]*NeuronsPerLayer[4]);
				
				Layers = 5;
				if (NeuronsPerLayer[4] == 0) Layers=4;
				if (NeuronsPerLayer[3] == 0) Layers=3;
				if (NeuronsPerLayer[2] == 0) Layers=2;
				if (NeuronsPerLayer[1] == 0) Layers=1;

				for (int i=0; i<5; i++)
				{
					ActivationFunction[i]=(int)((double)Parameters[25+i]);
				}

				for (int i=0; i<NumBiases; i++)
				{
					sNeuron Neuron = new sNeuron();
					Neuron.Bias = (double)Parameters[30+i];
					Neuron.Value = 0;
					Neuron.Output = 0;

					Neurons.Add(Neuron);
				}

				for (int i=0; i<NumWeigths; i++)
				{
					Weigths.Add(Parameters[30+NumBiases+i]);
				}

			}
			catch (Exception exc)
			{
				linenumber = 0;
				Framework.Logger(99,"An error ocurred: " + exc.Message );
			}

			finally
			{
				if (tr!=null)
				{
					tr.Close();
				}
			}

		}

		private int NeuronIndex(int LayerNumber, int NeuronNumber)
		{
			int index = 0;
			
			for (int i=0; i<(LayerNumber); i++)
			{
				index += NeuronsPerLayer[i];
			}

			index += NeuronNumber;

			return index;
		}

		private int WeigthIndex(int Layer1, int Neuron1, int Neuron2)
		{
			int index = 0;

			if (Layer1>0)
			{
				for (int i=0; i<(Layer1); i++)
				{
					index += NeuronsPerLayer[i]*NeuronsPerLayer[i+1];
				}
			}		

			if (Neuron1>0)
			{
				index += Neuron1 * NeuronsPerLayer[Layer1+1];
			}

			if (Neuron2>0)
			{
				index += Neuron2;
			}

			return index;
		}

		private double ActivationFn(double X, int Function)
		{
			double output = 0;

			switch (Function)
			{
				case 1: // purelin F(x)=x
				{
					output = X;
					break;
				}
				case 2: // binary F(x)= (x>Par[2])?Par[1]:0
				{
					output = (X>AFParameters[1])?AFParameters[0]:0;
					break;
				}
				case 3: // bipolar F(x)= (x>Par[4])?Par[3]:-1*Par[3]
				{
					output = (X>AFParameters[3])?AFParameters[2]:-1*AFParameters[2];
					break;
				}
				case 4: // logsig
				{
					output = (AFParameters[6]-AFParameters[7])
						     /(1 + Math.Exp(-1*AFParameters[4]*(X-AFParameters[5])))
						     + AFParameters[7];
					break;
				}
				case 5:
				{
					output = AFParameters[10]*Math.Exp((-1/2)*
						     Math.Sqrt((X-AFParameters[8])/AFParameters[9]));
					break;
				}

			}

			return output;
		}

		public ArrayList Evaluate(ArrayList ANNInput)
		{
			ArrayList ANNOutput = new ArrayList();
	
			for (int layer=0; layer< Layers; layer++)
			{
				for (int neuron=0; neuron< NeuronsPerLayer[layer]; neuron++)
				{
					int neuronindex = NeuronIndex(layer,neuron);

					if (layer==0)
					{
						sNeuron Neuron = ((sNeuron)Neurons[neuronindex]);
						Neuron.Value = ((double)ANNInput[neuronindex]) + Neuron.Bias;
						Neuron.Output = ActivationFn(Neuron.Value,ActivationFunction[layer]);
						Neurons[neuronindex]=Neuron;
					}
					else
					{
						double sum = 0;
						for (int neuron0=0; neuron0< NeuronsPerLayer[layer-1]; neuron0++)
						{
							int n0index = NeuronIndex(layer-1,neuron0);
							int weigthindex = WeigthIndex(layer-1,neuron0,neuron);
							sum += ((sNeuron)Neurons[n0index]).Output * ((double)Weigths[weigthindex]);
						}

						sNeuron Neuron = ((sNeuron)Neurons[neuronindex]);
						Neuron.Value = sum + Neuron.Bias;
						Neuron.Output = ActivationFn(Neuron.Value,ActivationFunction[layer]);
						Neurons[neuronindex]=Neuron;

						if (layer == (Layers-1))
						{
							ANNOutput.Add(((sNeuron)Neurons[neuronindex]).Output);
						}
					}
				}
			}

			return ANNOutput;
		}

	}

}

			