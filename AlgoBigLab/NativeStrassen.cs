using Matrices;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Matrices;

namespace AlgoBigLab {
	internal class NativeStrassen {
		public static int[,] Extend(int[,]matrix, int extendTo) {
			int[,] res = new int[extendTo, extendTo];
			int dim = matrix.GetUpperBound(0) + 1;

			for(int i = 0; i < dim; i++) {
				for(int j = 0; j < dim; j++) {
					res[i, j] = matrix[i, j];
				}
			}

			return res;
		}

		public static int[,] Shrink(int[,] matrix, int shrinkTo) {
			int[,] res = new int[shrinkTo, shrinkTo];

			for(int i = 0; i < shrinkTo; i++) {
				for(int j = 0; j < shrinkTo; j++) {
					res[i, j] = matrix[i, j];
				}
			}

			return res;
		}

		public static Matrix NativeStrassenSolver(int[,] A, int[,] B, int dim, bool opt) {
			int[,] res;
			int _dim = 1;
			int origDim = dim;
			bool needShrinking = false;

			while(_dim < dim) {
				_dim <<= 1;
			}

			if(_dim != dim) {
				A = Extend(A, _dim);
				B = Extend(B, _dim);
				dim = _dim;
				needShrinking = true;
			}

			if(!opt) {
				res = Strassen(A, B, dim);
			} else {
				res = Strassen64(A, B, dim);
			}

			if(needShrinking) {
				res = Shrink(res, origDim);
			}

			return new Matrix(res);
		}

		public static int[,] Strassen2x2(int[,] A, int[,] B) {
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

			int[,] res = new int[2, 2];

			res[0, 0] = P2 + P3;
			res[0, 1] = T1 + P5 + P6;
			res[1, 0] = T2 - P7;
			res[1, 1] = T2 + P5;

			return res;
		}

		public static int[,] plus(int[,] A, int[,] B, int dim) {
			int[,] res = new int[dim, dim];

			for(int i = 0; i < dim; i++) {
				for(int j = 0; j < dim; j++) {
					res[i, j] = A[i, j] + B[i, j];
				}
			}

			return res;
		}

		public static int[,] minus(int[,] A, int[,] B, int dim) {
			int[,] res = new int[dim, dim];

			for(int i = 0; i < dim; i++) {
				for(int j = 0; j < dim; j++) {
					res[i, j] = A[i, j] - B[i, j];
				}
			}

			return res;
		}

		public static int[,] slice(int[,] matrix, int startI, int startJ, int size) {
			int[,] res = new int[size, size];

			for(int i = 0; i < size; i++) {
				for(int j = 0; j < size; j++) {
					res[i, j] = matrix[startI + i, startJ + j];
				}
			}

			return res;
		}

		public static int[,] splice(int[,] A, int[,] B, int[,] C, int[,] D, int pivot) {
			int[,] res = new int[pivot * 2, pivot * 2];

			for(int i = 0; i < pivot; i++) {
				for(int j = 0; j < pivot; j++) {
					res[i, j] = A[i, j];
					res[i, pivot + j] = B[i, j];
					res[pivot + i, j] = C[i, j];
					res[pivot + i, pivot + j] = D[i, j];
				}
			}

			return res;
		}

		public static int[,] Strassen(int[,] A, int[,] B, int dim) {
			if(dim == 2) {
				return Strassen2x2(A, B);
			}

			int pivot = dim >> 1;

			int[,] A11 = slice(A, 0, 0, pivot);
			int[,] A12 = slice(A, 0, pivot, pivot);
			int[,] A21 = slice(A, pivot, 0, pivot);
			int[,] A22 = slice(A, pivot, pivot, pivot);

			int[,] B11 = slice(B, 0, 0, pivot);
			int[,] B12 = slice(B, 0, pivot, pivot);
			int[,] B21 = slice(B, pivot, 0, pivot);
			int[,] B22 = slice(B, pivot, pivot, pivot);

			int[,] S1 = plus(A21, A22, pivot);
			int[,] S2 = minus(S1, A11, pivot);
			int[,] S3 = minus(A11, A21, pivot);
			int[,] S4 = minus(A12, S2, pivot);
			int[,] S5 = minus(B12, B11, pivot);
			int[,] S6 = minus(B22, S5, pivot);
			int[,] S7 = minus(B22, B12, pivot);
			int[,] S8 = minus(S6, B21, pivot);

			int[,] P1 = Strassen(S2, S6, pivot);
			int[,] P2 = Strassen(A11, B11, pivot);
			int[,] P3 = Strassen(A12, B21, pivot);
			int[,] P4 = Strassen(S3, S7, pivot);
			int[,] P5 = Strassen(S1, S5, pivot);
			int[,] P6 = Strassen(S4, B22, pivot);
			int[,] P7 = Strassen(A22, S8, pivot);

			int[,] T1 = plus(P1, P2, pivot);
			int[,] T2 = plus(T1, P4, pivot);

			int[,] C11 = plus(P2, P3, pivot);
			int[,] C12 = plus(T1, plus(P5, P6, pivot), pivot);
			int[,] C21 = minus(T2, P7, pivot);
			int[,] C22 = plus(T2, P5, pivot);

			int[,] res = splice(C11, C12, C21, C22, pivot);
			return res;
		}

		public static int[,] mult(int[,] A, int[,] B, int dim) {
			int[,] res = new int[dim, dim];

			for(int i = 0; i < dim; i++) {
				for(int j = 0; j < dim; j++) {
					int elem_res = 0;

					for(int n = 0; n < dim; n++) {
						elem_res += A[i, n] * B[n, j];
					}

					res[i, j] = elem_res;
				}
			}

			return res;
		}

		public static int[,] Strassen64(int[,] A, int[,] B, int dim) {
			if(dim <= 64) {
				return mult(A, B, dim);
			}

			int pivot = dim >> 1;

			int[,] A11 = slice(A, 0, 0, pivot);
			int[,] A12 = slice(A, 0, pivot, pivot);
			int[,] A21 = slice(A, pivot, 0, pivot);
			int[,] A22 = slice(A, pivot, pivot, pivot);

			int[,] B11 = slice(B, 0, 0, pivot);
			int[,] B12 = slice(B, 0, pivot, pivot);
			int[,] B21 = slice(B, pivot, 0, pivot);
			int[,] B22 = slice(B, pivot, pivot, pivot);

			int[,] S1 = plus(A21, A22, pivot);
			int[,] S2 = minus(S1, A11, pivot);
			int[,] S3 = minus(A11, A21, pivot);
			int[,] S4 = minus(A12, S2, pivot);
			int[,] S5 = minus(B12, B11, pivot);
			int[,] S6 = minus(B22, S5, pivot);
			int[,] S7 = minus(B22, B12, pivot);
			int[,] S8 = minus(S6, B21, pivot);

			int[,] P1 = Strassen64(S2, S6, pivot);
			int[,] P2 = Strassen64(A11, B11, pivot);
			int[,] P3 = Strassen64(A12, B21, pivot);
			int[,] P4 = Strassen64(S3, S7, pivot);
			int[,] P5 = Strassen64(S1, S5, pivot);
			int[,] P6 = Strassen64(S4, B22, pivot);
			int[,] P7 = Strassen64(A22, S8, pivot);

			int[,] T1 = plus(P1, P2, pivot);
			int[,] T2 = plus(T1, P4, pivot);

			int[,] C11 = plus(P2, P3, pivot);
			int[,] C12 = plus(T1, plus(P5, P6, pivot), pivot);
			int[,] C21 = minus(T2, P7, pivot);
			int[,] C22 = plus(T2, P5, pivot);

			int[,] res = splice(C11, C12, C21, C22, pivot);
			return res;
		}
	}
}
