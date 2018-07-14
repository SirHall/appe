using System;
using System.IO;

namespace MCDRAG_Calc
{
	public class MCDRAG : IDisposable
	{
		public void SetIterations(int iterations, double machUpperBound, double machLowerBound = 0.0, bool upperInclusive = false)
		{
			M = new double[iterations];

			C1 = new double[M.Length];
			C2 = new double[M.Length];
			C3 = new double[M.Length];
			C4 = new double[M.Length];
			C5 = new double[M.Length];
			C6 = new double[M.Length];
			P1 = new double[M.Length];

			//Upper bound is not inclusive

			for (int i = 0; i < M.Length; i++)
			{
				M[i] = machUpperBound / (M.Length - (upperInclusive ? 1 : 0)) * i + machLowerBound;
			}
		}

		public void SetIterations(double[] machPoints)
		{
			M = machPoints;
			C1 = new double[M.Length];
			C2 = new double[M.Length];
			C3 = new double[M.Length];
			C4 = new double[M.Length];
			C5 = new double[M.Length];
			C6 = new double[M.Length];
			P1 = new double[M.Length];
		}

		#region Values

		public double
			D1, //Projectile reference diameter (mm)
			L1, //Projectile length (calibers)
			L2, //Nose length (calibers)
			R1, //RT/R (headshape parameter)
			L3, //Boattail length (calibers)
			D2, //Base diameter (calibers)
			D3, //Meplat diameter (calibers)
			D4, //Rotating band diameter (calibers)
			X1; //Center of gravity location (optional, calibers from nose)

		public BoundaryLayerCode K = BoundaryLayerCode.LL;
		public string K1; //Projectile identification

		#region Arrays

		public double[]
			M = { 0.5, 0.6, 0.7, 0.8, 0.85, 0.9, 0.925, 0.95, 0.975, 1.0, 1.1, 1.2, 1.3,
					1.4, 1.5, 1.6, 1.7, 1.8, 2.0, 2.2, 2.5, 3.0, 3.5, 4.0, 4.5, 5.0},
			//M = {0.5, 0.6, 0.7, 0.8, 0.85, 0.9, 0.925, 0.95, 0.975, 1.0, 1.1, 1.2, 1.3, 1.4, 1.5, 1.6, 1.7, 1.8,
			//		2.0, 2.2, 2.5, 3.0, 3.5, 4.0},
			C1,
			C2,
			C3,
			C4,
			C5,
			C6,
			P1;


		#endregion

		#endregion

		public void InputValues(
			double projDiameter, double projLength, double noseLength, double headShape,
			double boatTailLength, double baseDiameter, double meplatDiameter, double rotatingBandDiameter,
			double centerOfGravity, BoundaryLayerCode boundaryLayer, string identification)
		{
			D1 = projDiameter;
			L1 = projLength;
			L2 = noseLength;
			R1 = headShape;
			L3 = boatTailLength;
			D2 = baseDiameter;
			D3 = meplatDiameter;
			D4 = rotatingBandDiameter;
			X1 = centerOfGravity;
			K = boundaryLayer;
			K1 = identification;
		}

		public void GenerateCoefficients()
		{
			#region Declarations

			double
				C17, C11, C12, C13, C14, R4, T1, M2, R2, R3, C7, C8, D5, S1, S2, S3, C9 = 0, C10 = 0,
				B2, B, Z, S4, C15, P5, C16, C18, T2, T3, E1, B4, B3, A12, A11, E2, X3, A1, R5, E3, A2,
				P2, P4, X2;

			#endregion

			for (int i = 0; i < M.Length; i++)
			{

				//Line 74
				T1 = (1 - D3) / L2;
				M2 = M[i] * M[i];
				R2 = 23296.3 * M[i] * L1 * D1;
				R3 = .4343 * (Math.Log(R2));
				C7 = (1.328 / Math.Sqrt(R2)) * (Math.Pow(1 + 0.12 * M2, -0.12));
				C8 = (.455 / Math.Pow(R3, 2.58)) * (Math.Pow(1 + .21 * M2, -0.32));
				D5 = 1 + ((0.333 + (0.02 / (L2 * L2))) * R1);
				S1 = 1.5708 * L2 * D5 * (1 + (1 / (8 * L2 * L2)));
				S2 = 3.1416 * (L1 - L2);
				S3 = S1 + S2;

				//Line 84
				switch (K)
				{
					case BoundaryLayerCode.LL:
						//Line 87
						C9 = 1.2732 * S3 * C7;
						C10 = C9;
						break;
					case BoundaryLayerCode.LT:
						//Line 90
						C9 = 1.2732 * S3 * C7;
						C10 = 1.2732 * S3 * C8;
						break;
					case BoundaryLayerCode.TT:
						//Line 93
						C9 = 1.2732 * S3 * C8;
						C10 = C9;
						break;
				}

				C3[i] = (C9 * S1 + C10 * S2) / S3;
				C15 = (M2 - 1) / (2.4 * M2); //{TODO} Move declaration outside loop

				if (M[i] > 1)
					P5 = Math.Pow(1.2 * M2, 3.5) * Math.Pow(6 / (7 * M2 - 1), 2.5);
				else
					P5 = Math.Pow(1 + .2 * M2, 3.5);

				//Line 102
				C16 = Math.Pow(1.122 * (P5 - 1) * D3, 2) / M2;

				if (M[i] <= 0.91)
					C18 = 0;
				else if (M[i] >= 1.41)
					C18 = 0.85 * C16;
				else
					C18 = (0.254 + 2.88 * C15) * C16;

				//Line 110
				if (M[i] < 1)
					P2 = 1 / (1 + .1875 * M2 + 0.0531 * Math.Pow(M2, 2));
				else
					P2 = 1 / (1 + .2477 * M2 + 0.0345 * Math.Pow(M2, 2));

				P4 = (1 + 9.000001E-02 * M2 * (1 - Math.Exp(L2 - L1))) * (1 + 0.25 * M2 * (1 - D2));

				//Line 116
				P1[i] = P2 * P4;
				if (P1[i] >= 0)
					C6[i] = (1.4286 * (1 - P1[i]) * (D2 * D2)) / M2;

				if (M[i] < 0.95)
					C4[i] = Math.Pow(M[i], 12.5) * (D4 - 1);
				else
					C4[i] = (0.21 + 0.28 / M2) * (D4 - 1);

				//Line 126

				if (M[i] > 1)
				{
					//Line 146
					//Supersonic speeds
					B2 = M2 - 1;
					B = Math.Sqrt(B2);
					Z = B;
					S4 = 1 + 0.368 * Math.Pow(T1, 1.85);

					//Line 150
					if (M[i] < S4)
						Z = Math.Sqrt(S4 * S4 - 1);
					C11 = .7156 - .5313 * R1 + .595 * R1 * R1;
					C12 = .0796 + .0779 * R1;
					C13 = 1.587 + .049 * R1;
					C14 = .1122 + .1658 * R1;
					R4 = 1 / (Z * Z);
					C17 = (C11 - C12 * (T1 * T1)) * R4 * Math.Pow(T1 * Z, C13 + C14 * T1);
					C2[i] = C17 + C18;

					//Line 159
					if (L3 <= 0)
					{
						C5[i] = 0;
					}
					else
					{
						T2 = (1 - D2) / (2 * L3);

						if (M[i] <= 1.1)
						{
							T3 = 2 * T2 * T2 + T2 * T2 * T2;
							E1 = Math.Exp(-2 * L3);
							B4 = 1 - E1 + 2 * T2 * ((E1 * (L3 + .5)) - .5);
							C5[i] = 2 * T3 * B4 * (1.774 - 9.3 * C15);
						}
						else
						{
							B3 = .85 / B;
							A12 = (5 * T1) / (6 * B) + (0.5 * T1) * (0.5 * T1) - (0.7435 / M2) * Math.Pow(T1 * M[i], 1.6);
							A11 = (1 - ((.6 * R1) / M[i])) * A12;
							E2 = Math.Exp(((-1.1952) / M[i]) * (L1 - L2 - L3));
							X3 = ((2.4 * M2 * M2 - 4 * B2) * (T2 * T2)) / (2 * B2 * B2);
							A1 = A11 * E2 - X3 + ((2 * T2) / B);
							R5 = 1 / B3;
							E3 = Math.Exp(-B3 * L3);
							A2 = 1 - E3 + (2 * T2 * (E3 * (L3 + R5) - R5));
							C5[i] = 4 * A1 * T2 * A2 * R5;
						}

					}

				}
				else
				{
					if (L3 <= 0 || M[i] <= 0.85)
					{
						//Line 129
						C5[i] = 0;
					}
					else //If (L3 > 0)
					{
						//Line 131 (Moved check into previous IF)

						T2 = (1 - D2) / (2 * L3);
						T3 = 2 * T2 * T2 + T2 * T2 * T2;
						E1 = Math.Exp(-2 * L3);
						B4 = 1 - E1 + 2 * T2 * ((E1 * (L3 + 0.5)) - 0.5);
						C5[i] = 2 * T3 * B4 * (1 / (0.564 + 1250 * C15 * C15));
					}

					X2 = Math.Pow(1 + .552 * Math.Pow(T1, 0.8), -0.5);
					//Line 138
					if (M[i] <= X2)
					{
						C17 = 0;
						C2[i] = C18; //Should be 'C2[i] = C17 + C18', but C17 is already '0'.

						//GOTO 181
					}
					else
					{
						C17 = 0.368 * Math.Pow(T1, 1.8) + 1.6 * T1 * C15;
						C2[i] = C17 + C18;

						//GOTO 181
					}
				}

				//To filter out any 'NaNs' or 'infinities'
				C2[i] = FixDouble(C2[i]);
				C3[i] = FixDouble(C3[i]);
				C4[i] = FixDouble(C4[i]);
				C5[i] = FixDouble(C5[i]);
				C6[i] = FixDouble(C6[i]);


				//Line 181
				//C1 === CDO (Refer to Page.10)
				C1[i] = C2[i] + C3[i] + C4[i] + C5[i] + C6[i];
			}
		}

		double FixDouble(double v)
		{
			return
				double.IsInfinity(v) || double.IsNaN(v)
				?
				0.0
				:
				v;
		}

		public static void SaveToCSVFile(MCDRAG calculator, string filePath)
		{

			string output = "MACH,CDO,CDH,CDSF,CDBND,CDBT,CDB,PB/Pl\n";
			for (int i = 0; i < calculator.M.Length; i++)
			{
				output +=
					$"{calculator.M[i]}," +
					$"{calculator.C1[i]}," +
					$"{calculator.C2[i]}," +
					$"{calculator.C3[i]}," +
					$"{calculator.C4[i]}," +
					$"{calculator.C5[i]}," +
					$"{calculator.C6[i]}," +
					$"{calculator.P1[i]}\n";
			}
			File.WriteAllText(filePath, output);

			output = "";
		}

		#region IDisposable Support
		private bool disposedValue = false;

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					// TODO: dispose managed state (managed objects).
				}

				M = null;
				C1 = null;
				C2 = null;
				C3 = null;
				C4 = null;
				C5 = null;
				C6 = null;
				P1 = null;
				K1 = null;

				disposedValue = true;
			}
		}

		~MCDRAG()
		{
			Dispose(false);
		}

		void IDisposable.Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		#endregion
	}
}

public enum BoundaryLayerCode { LL, LT, TT }
