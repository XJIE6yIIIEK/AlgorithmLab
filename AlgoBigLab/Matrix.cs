using System;

namespace Matrices {
	/// <summary>
	/// Матричный класс
	/// </summary>
	internal class Matrix {
		public int[,] matrix;			//Матрица значений

		private protected int rows;					//Строки
		private protected int cols;					//Столбцы

		public int this[int n, int m] {
			get {
				return matrix[n, m];
			}

			set {
				matrix[n, m] = value;
			}
		}

		/// <summary>
		/// Количество строк матрицы. <i><b>Только для чтения</b></i>.
		/// </summary>
		public int Rows {
			get { return rows; }
		}

		/// <summary>
		/// Количество столбцов матрицы <i><b>Только для чтения</b></i>.
		/// </summary>
		public int Cols {
			get { return cols; }
		}

		/// <summary>
		/// Конструктор класса Matrix
		/// </summary>
		/// <param name="n">Количество строк</param>
		/// <param name="m">Количество столбцов</param>
		public Matrix(int n, int m) {
			rows = n;
			cols = m;
			matrix = new int[n, m];
		}

		public Matrix(int[,] A) {
			rows = A.GetUpperBound(0) + 1;
			cols = A.GetUpperBound(1) + 1;

			matrix = new int[rows, cols];

			for(int i = 0; i < rows; i++) {
				for(int j = 0; j < cols; j++) {
					matrix[i, j] = A[i, j];
				}
			}
		}

		/// <summary>
		/// Неполное копирование матрицы. Копируются только значения.
		/// </summary>
		/// <returns>Возвращает поверхностную копию матрицу</returns>
		public Matrix Clone() {
			Matrix res = new Matrix(rows, cols);

			for(int i = 0; i < rows; i++) {
				for(int j = 0; j < cols; j++) {
					res[i, j] = matrix[i, j];
				}
			}

			return res;
		}

		//Переопределение суммы матриц
		public static Matrix operator +(Matrix a, Matrix b) {
			Matrix res = new Matrix(a.Rows, a.Cols);

			for(int i = 0; i < a.Rows; i++) {
				for(int j = 0; j < a.Cols; j++) {
					res[i, j] = a[i, j] + b[i, j];
				}
			}

			return res;
		}

		//Переопределение разницы матрицы
		public static Matrix operator -(Matrix a, Matrix b) {
			Matrix res = new Matrix(a.Rows, a.Cols);

			for(int i = 0; i < a.Rows; i++) {
				for(int j = 0; j < a.Cols; j++) {
					res[i, j] = a[i, j] - b[i, j];
				}
			}

			return res;
		}

		//Переопределение матричного умножения
		public static Matrix operator *(Matrix a, Matrix b) {
			Matrix res = new Matrix(a.Rows, b.Cols);

			for(int i = 0; i < a.Rows; i++) {
				for(int j = 0; j < b.Cols; j++) {
					int elem_res = 0;

					for(int n = 0; n < a.Cols; n++) {
						elem_res += a[i, n] * b[n, j];
					}

					res[i, j] = elem_res;
				}
			}

			return res;
		}

		//Переопределение умножения матрицы на скаляр
		public static Matrix operator *(int c, Matrix a) {
			Matrix res = new Matrix(a.Rows, a.Cols);

			for(int i = 0; i < a.Rows; i++) {
				for(int j = 0; j < a.Cols; j++) {
					res[i, j] = a[i, j] * c;
				}
			}

			return res;
		}

		public Matrix Extend(int extendTo) {
			Matrix res = new Matrix(extendTo, extendTo);

			for(int i = 0; i < rows; i++) {
				for(int j = 0; j < cols; j++) {
					res[i, j] = matrix[i, j];
				}
			}

			return res;
		}

		public Matrix Slice(int sliceTo) {
			Matrix res = new Matrix(sliceTo, sliceTo);

			for(int i = 0; i < sliceTo; i++) {
				for(int j = 0; j < sliceTo; j++) {
					res[i, j] = matrix[i, j];
				}
			}

			return res;
		}

		public Matrix Slice(int startI, int startJ, int size) {
			Matrix res = new Matrix(size, size);

			for(int i = 0; i < size; i++) {
				for(int j = 0; j < size; j++) {
					res[i, j] = matrix[startI + i, startJ + j];
				}
			}

			return res;
		}

		public static Matrix Splice(Matrix C11, Matrix C12, Matrix C21, Matrix C22) {
			int pivot = C11.Rows;
			Matrix res = new Matrix(pivot * 2, pivot * 2);

			for(int i = 0; i < pivot; i++) {
				for(int j = 0; j < pivot; j++) {
					res[i, j] = C11[i, j];
					res[i, pivot + j] = C12[i, j];
					res[pivot + i, j] = C21[i, j];
					res[pivot + i, pivot + j] = C22[i, j];
				}
			}

			return res;
		}

		private static Matrix StrassenMultiply64(Matrix A, Matrix B) {
			if(A.Rows <= 64) {
				return A * B;
			}

			int pivot = A.Rows >> 1;

			Matrix A11 = A.Slice(0, 0, pivot);
			Matrix A12 = A.Slice(0, pivot, pivot);
			Matrix A21 = A.Slice(pivot, 0, pivot);
			Matrix A22 = A.Slice(pivot, pivot, pivot);

			Matrix B11 = B.Slice(0, 0, pivot);
			Matrix B12 = B.Slice(0, pivot, pivot);
			Matrix B21 = B.Slice(pivot, 0, pivot);
			Matrix B22 = B.Slice(pivot, pivot, pivot);

			Matrix S1 = A21 + A22;
			Matrix S2 = S1 - A11;
			Matrix S3 = A11 - A21;
			Matrix S4 = A12 - S2;
			Matrix S5 = B12 - B11;
			Matrix S6 = B22 - S5;
			Matrix S7 = B22 - B12;
			Matrix S8 = S6 - B21;

			Matrix P1 = StrassenMultiply64(S2, S6);
			Matrix P2 = StrassenMultiply64(A11, B11);
			Matrix P3 = StrassenMultiply64(A12, B21);
			Matrix P4 = StrassenMultiply64(S3, S7);
			Matrix P5 = StrassenMultiply64(S1, S5);
			Matrix P6 = StrassenMultiply64(S4, B22);
			Matrix P7 = StrassenMultiply64(A22, S8);

			Matrix T1 = P1 + P2;
			Matrix T2 = T1 + P4;

			Matrix C11 = P2 + P3;
			Matrix C12 = T1 + P5 + P6;
			Matrix C21 = T2 - P7;
			Matrix C22 = T2 + P5;

			Matrix res = Splice(C11, C12, C21, C22);
			return res;
		}

		private static Matrix StrassenMultiply2x2(Matrix A, Matrix B) {
			int S1 = A[1, 0] + A[1, 1];
			int S2 = S1 - A[0, 0];
			int S3 = A[0, 0] - A[1, 0];
			int S4 = A[0, 1] - S2;
			int S5 = B[0, 1] - B[0, 0];
			int S6 = B[1, 1] - S5;
			int S7 = B[1, 1] - B[0, 1];
			int S8 = S6 - B[1, 0];

			int P1 = S2 * S6;
			int P2 = A[0, 0] * B[0, 0];
			int P3 = A[0, 1] * B[1, 0];
			int P4 = S3 * S7;
			int P5 = S1 * S5;
			int P6 = S4 * B[1, 1];
			int P7 = A[1, 1] * S8;

			int T1 = P1 + P2;
			int T2 = T1 + P4;

			Matrix res = new Matrix(2, 2);

			res[0, 0] = P2 + P3;
			res[0, 1] = T1 + P5 + P6;
			res[1, 0] = T2 - P7;
			res[1, 1] = T2 + P5;

			return res;
		}

		private static Matrix StrassenMultiply(Matrix A, Matrix B) {
			if(A.Rows == 2) {
				return StrassenMultiply2x2(A, B);
			}

			int pivot = A.Rows >> 1;

			Matrix A11 = A.Slice(0, 0, pivot);
			Matrix A12 = A.Slice(0, pivot, pivot);
			Matrix A21 = A.Slice(pivot, 0, pivot);
			Matrix A22 = A.Slice(pivot, pivot, pivot);

			Matrix B11 = B.Slice(0, 0, pivot);
			Matrix B12 = B.Slice(0, pivot, pivot);
			Matrix B21 = B.Slice(pivot, 0, pivot);
			Matrix B22 = B.Slice(pivot, pivot, pivot);

			Matrix S1 = A21 + A22;
			Matrix S2 = S1 - A11;
			Matrix S3 = A11 - A21;
			Matrix S4 = A12 - S2;
			Matrix S5 = B12 - B11;
			Matrix S6 = B22 - S5;
			Matrix S7 = B22 - B12;
			Matrix S8 = S6 - B21;

			Matrix P1 = StrassenMultiply(S2, S6);
			Matrix P2 = StrassenMultiply(A11, B11);
			Matrix P3 = StrassenMultiply(A12, B21);
			Matrix P4 = StrassenMultiply(S3, S7);
			Matrix P5 = StrassenMultiply(S1, S5);
			Matrix P6 = StrassenMultiply(S4, B22);
			Matrix P7 = StrassenMultiply(A22, S8);

			Matrix T1 = P1 + P2;
			Matrix T2 = T1 + P4;

			Matrix C11 = P2 + P3;
			Matrix C12 = T1 + P5 + P6;
			Matrix C21 = T2 - P7;
			Matrix C22 = T2 + P5;

			Matrix res = Splice(C11, C12, C21, C22);
			return res;
		}

		public static Matrix Strassen(Matrix A, Matrix B, bool optimized) {
			int dim = 1;
			int origDim = A.Rows;
			bool needSlicing = false;

			while(dim < origDim) {
				dim <<= 1;
			}

			if(A.Rows != dim) {
				A = A.Extend(dim);
				B = B.Extend(dim);
				needSlicing = true;
			}

			Matrix res;

			if(optimized) {
				res = StrassenMultiply64(A, B);
			} else {
				res = StrassenMultiply(A, B);
			}

			if(needSlicing) {
				res = res.Slice(origDim);
			}

			return res;
		}

		/// <summary>
		/// Конвертация матрицы в строку
		/// </summary>
		/// <returns>Строковое представление матрицы</returns>
		public new virtual string ToString() {
			string res = "";

			for(int i = 0; i < rows; i++) {
				string tmp = "";

				for(int j = 0; j < cols; j++) {
					string intStr = string.Empty;

					intStr = string.Format("{0,7} ", matrix[i, j]);

					tmp = tmp + intStr;
				}

				res = res + tmp + "\n";
			}

			res = res.Substring(0, res.Length - 1);
			
			return res;
		}
	}
}
