using System;
using AlgoBigLab;
using Matrices;
using IOModule;
using Utility;

class Program {
	private static void FindMaxVolume() {
        int maxTime = 2 * 60 * 1000;
        IOModule.IOModule io = new IOModule.IOModule();

        Tester.TestSeries(50, 50, maxTime, 3, io);
    }

    private static int ChooseMethod() {
        Console.Write(@"Выберите метод для тестирования:
1) Тривиальный
2) Штрассена
3) Штрассена - оптимизированный
4) Штрассена - без создания классов
5) Штрассена - оптимизированный - без создания классов
>>>");
        return Convert.ToInt32(Console.ReadLine()) - 1;
    }

    private static void SolveTestsFromDirectory() {
        int methodAnswer = ChooseMethod();
        if (methodAnswer < 1 || methodAnswer > 5) {
            Console.WriteLine("Ошибка: неизвестный метод.");
            return;
        }
        Methods method = (Methods)methodAnswer;

        Console.Write("Введите путь до папки с тестами: ");
        string path = Console.ReadLine();
        if (!Directory.Exists(path)) {
            Console.WriteLine("Ошибка: не существует указзаный путь.");
            return;
        }
        string[] files = Directory.GetFiles(path, "*.txt");
        if (files.Count() == 0) {
            Console.WriteLine("Ошибка: папка с тестами пуста.");
            return;
        }
        foreach (string file in files) {
            Matrix A = null;
            Matrix B = null;
            Tester.ReadTest(ref A, ref B, file);
            long testResult = Tester.OneTest(A, B, method);
            Console.WriteLine(file+":");
            Console.WriteLine(testResult.ToString()+"ms");
        }
    }

    private static void SolveAutoGewneratedTests() {
        int methodAnswer = ChooseMethod();
        if (methodAnswer < 1 || methodAnswer > 5) {
            Console.WriteLine("Ошибка: неизвестный метод.");
            return;
        }
        Methods method = (Methods)methodAnswer;

        Console.Write("Введите n: ");
        int n = Convert.ToInt32(Console.ReadLine());
        Matrix A = null;
        Matrix B = null;
        Tester.GenerateTest(n, ref A, ref B);
        long testResult = Tester.OneTest(A, B, method);
        Console.WriteLine("Матрица A:\n"+A.ToString());
        Console.WriteLine("Матрица B:\n" + B.ToString());
        Console.WriteLine("Время: "+testResult.ToString()+"ms");
    }

	public static void Main(string[] args) {
        Console.Write("1) Поиск максимального объёма данных\n2) Решение тестов из дериктории\n3) Решение автоматически сгенерированных тестов\n>>>");
        int way = Convert.ToInt32(Console.ReadLine());
        switch (way) {
            case 1:
                FindMaxVolume();
                break;
            case 2:
                SolveTestsFromDirectory();
                break;
            case 3:
                SolveAutoGewneratedTests();
                break;
            default:
                Console.WriteLine("Ошибка: неизвестная команда.");
                break;
        }
		
	}
}
