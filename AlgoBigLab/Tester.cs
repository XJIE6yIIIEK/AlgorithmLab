using Matrices;
using System.Diagnostics;

namespace AlgoBigLab {
    /// <summary>
    /// Инкапсулирует логику тестирования.
    /// </summary>
    static internal class Tester {
        const int rndAbsMax = 1000;
        static private StreamReader? inputStream;

        public enum Methods {
            Trivial,
            Strassen,
            StrassenOptimized
        }

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
        static public void ReadTest(ref Matrix? A, ref Matrix? B) {
            if (inputStream == null) {
                Console.Write("Введите путь до файла: ");
                string path = Console.ReadLine();

                inputStream = new StreamReader(path);
            }
            string line;
            while ((line = inputStream.ReadLine()) != null && line == "") ;
            if (!inputStream.EndOfStream) {
                
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
            if (inputStream.EndOfStream) {
                inputStream.Close();
            }
        }

        static private long OneTest(Matrix A, Matrix B, Methods method) {
            Matrix _;
            Stopwatch stopwatch = new Stopwatch();

            switch (method) {
                case Methods.Trivial:
                    stopwatch.Start();
                    _ = A * B;
                    stopwatch.Stop();
                    break;
                case Methods.Strassen:
                    stopwatch.Start();
                    _ = Matrix.Strassen(A, B, false);
                    stopwatch.Stop();
                    break;
                case Methods.StrassenOptimized:
                    stopwatch.Start();
                    _ = Matrix.Strassen(A, B, true);
                    stopwatch.Stop();
                    break;
            }

            return stopwatch.ElapsedMilliseconds;
        }
        
        /// <summary>
        /// Поиск максимальнойго объёма данных, при которых время выполнения не превышает
        /// (или превышает незначительно) две минуты.
        /// </summary>
        /// <param name="method">Меотод enum Tester.Methods</param>
        /// <param name="step">Шаг.</param>
        /// <param name="startV">Начальный объём.</param>
        /// <returns></returns>
        static public int FindVMax(Methods method, int step = 1, int startV = 4) {
            Matrix A = null;
            Matrix B = null;

            for (int n = startV; ; n += step) {
                GenerateTest(n, ref A, ref B);
                long time = OneTest(A, B, method);
                Console.WriteLine(n.ToString() + ": " + time.ToString());
                if (time > 120000) {
                    return n;
                }
            }
        }

        /// <summary>
        /// Вычисление среднего времени работы метода при заданном объёме данных.
        /// </summary>
        /// <param name="n">Размерность матриц.</param>
        /// <param name="tests">Количество тестов.</param>
        /// <param name="method">Меотод enum Tester.Methods</param>
        /// <returns></returns>
        static public long AverageTime(int n, int tests, Methods method) {
            Matrix A = null;
            Matrix B = null;
            long time = 0;
            for (int j = 0; j < tests; j++) {
                GenerateTest(n, ref A, ref B);
                time += OneTest(A, B, method);
            }
            time /= tests;
            return time;
        }

        /// <summary>
        /// Серия тестов для разных размерностей.
        /// </summary>
        /// <param name="V">Массив размерностей.</param>
        /// <param name="method">Меотод enum Tester.Methods</param>
        /// <param name="tests">Количество тестов.</param>
        /// <returns></returns>
        static public Dictionary<int, long> SerialExperiments(int[] V, Methods method, int tests) {
            
            Dictionary<int, long> times = new Dictionary<int, long>();
            for (int i = 0; i < V.Length; i++) {
                long time = AverageTime(V[i], tests, method);
                times.Add(V[i], time);
            }
            return times;
        }


    }
}
