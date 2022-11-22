using Matrices;
using System.Diagnostics;
using IOModule;
using Utility;

namespace AlgoBigLab {
    /// <summary>
    /// Инкапсулирует логику тестирования.
    /// </summary>
    static internal class Tester {
        const int rndAbsMax = 1000;

        /// <summary>
        /// Генерация матрицы со случайными значениями.
        /// </summary>
        /// <param name="n">Размерность матрицы.</param>
        /// <returns></returns>
        static public Matrix GenerateMatrix(int n) {
            Matrix matrix = new Matrix(n, n);
            Random rnd = new Random();
            for (int i = 0; i < n; i++) {
                for (int j = 0; j < n; j++) {
                    matrix[i, j] = rnd.Next(-rndAbsMax, rndAbsMax);
                }
            }
            return matrix;
        }
        /// <summary>
        /// Генерация одного тестирующего набора.
        /// </summary>
        /// <param name="n">Размерность матриц.</param>
        /// <param name="A">Переменная для первой матрицы.</param>
        /// <param name="B">Переменная для второй матрицы.</param>
        static public void GenerateTest(int n, ref Matrix? A, ref Matrix? B) {
            A = GenerateMatrix(n);
            B = GenerateMatrix(n);
        }

        /// <summary>
        /// Прочитать один тестирующий набор из файла.
        /// Если файл не открыт - атоматически запросит путь.
        /// Если достигнут конец файл - автомтически закроет файл.
        /// </summary>
        /// <param name="A">Переменная для первой матрицы.</param>
        /// <param name="B">Переменная для второй матрицы.</param>
        static public void ReadTest(ref Matrix? A, ref Matrix? B, string path) {
            StreamReader inputStream = new StreamReader(path);
            string line;
            if ((line = inputStream.ReadLine()) != null) {
                int n = Convert.ToInt32(line);
                A = new Matrix(n, n);
                B = new Matrix(n, n);
                for (int m = 0; m < 2; m++) {
                    for (int i = 0; i < n; i++) {
                        string[] splited = inputStream.ReadLine().Split(" ");
                        List<string> keys = new List<string>();
                        foreach (string key in splited) {
                            if (key.Length != 0) {
                                keys.Add(key);
                            }
                        }
                        if (keys.Count() == 0) {
                            i -= 1;
                            continue;
                        }
                        for (int j = 0; j < n; j++) {
                            (m == 0 ? A : B)[i, j] = Convert.ToInt32(keys[j]);
                        }

                    }
                }
            }
            inputStream.Close();
        }

        static public long OneTest(Matrix A, Matrix B, Methods method) {
            Matrix _;
            Stopwatch stopwatch = new Stopwatch();

            switch (method) {
                case Methods.Trivial: {
                    stopwatch.Start();
                    _ = A * B;
                    stopwatch.Stop();
                }; break;

                case Methods.Strassen: {
                    stopwatch.Start();
                    _ = Matrix.Strassen(A, B, false);
                    stopwatch.Stop();
			    }; break;

                case Methods.StrassenOptimized:{ 
                    stopwatch.Start();
                    _ = Matrix.Strassen(A, B, true);
                    stopwatch.Stop();
		        }; break;

				case Methods.StrassenNative: {
					stopwatch.Start();
                    _ = NativeStrassen.NativeStrassenSolver(A.matrix, B.matrix, A.Rows, false);
					stopwatch.Stop();
				}; break;

				case Methods.StrassenOptimizedNative: {
					stopwatch.Start();
					_ = NativeStrassen.NativeStrassenSolver(A.matrix, B.matrix, A.Rows, true);
					stopwatch.Stop();
				}; break;
			}

            return stopwatch.ElapsedMilliseconds;
        }

		/// <summary>
		/// Поиск максимальнойго объёма данных, при которых время выполнения не превышает
		/// (или превышает незначительно) две минуты.
		/// </summary>
		/// <param name="method">Меотод enum Tester.Methods</param>
		/// <param name="maxTime">Максимальное время выполнения, мс</param>
		/// <param name="step">Шаг.</param>
		/// <param name="startV">Начальный объём.</param>
		/// <returns></returns>
		static public void TestSeries(int startV, int step, int maxTime, int seriesCount, IOModule.IOModule io) {
            Matrix A = null;
            Matrix B = null;
            ResultList resList = new ResultList();
            string[] methodLabels = {
                "Trivial",
                "Strassen",
                "Strassen 64",
				"Strassen Native",
				"Strassen 64 Native"
			};

            for(int method = 0; method < 5; method++) {
                for(int n = startV; ; n += step) {
                    double time = 0;
                    GenerateTest(n, ref A, ref B);

                    for(int i = 0; i < seriesCount; i++) {
                        time += OneTest(A, B, (Methods)method);
                    }

                    time /= seriesCount;

                    Console.WriteLine($"{methodLabels[method]} N = {n} : {time} ms");

                    resList.Add(n, time / 1000, (Methods)method);

                    if(time > maxTime) {
                        break;
                    }
                }
            }

            io.WriteResult(resList);
		}
    }
}
