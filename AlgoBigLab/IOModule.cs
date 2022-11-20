using Microsoft.Office.Interop.Excel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Excel = Microsoft.Office.Interop.Excel;

namespace IOModule {
	enum TimeClass {
		Trivial = 0,
		Strassen = 1,
		Strassen64 = 2
	}

	class ResultList {
		private List<Tuple<int, double[]>> times;

		public int Length {
			get {
				return times.Count;
			}
		}

		public double this[int time, int _class] {
			get {
				return times[time].Item2[_class];
			}

			private set {
				times[time].Item2[_class] = value;
			}
		}

		public ResultList() { }

		private int Compare(Tuple<int, double[]> elem1, Tuple<int, double[]> elem2) {
			return elem1.Item1.CompareTo(elem2.Item1);
		}

		public void Add(int n, double time, TimeClass tClass) { 
			int nIndex = times.FindIndex(x => x.Item1 == n);

			if(nIndex == -1) {
				Tuple<int, double[]> tupleTime = new Tuple<int, double[]>(n, new double[3] { -1, -1, -1});

				tupleTime.Item2[(int)tClass] = time;

				times.Add(tupleTime);
				times.Sort(Compare);
			} else {
				this[nIndex, (int)tClass] = time;
			}
		}
	}

	internal class IOModule {
		private string resultsStr = "";

		private Excel.Application excelApp;

		public IOModule() {
			resultsStr = $@"{Directory.GetCurrentDirectory()}\results.xlsx";

			excelApp = new Excel.Application();

			FileInfo resFile = new FileInfo(resultsStr);
			if(resFile.Exists) {
				resFile.Delete();
			}

			Excel.Workbook resWorkbook = excelApp.Workbooks.Add();
			resWorkbook.SaveAs2(resultsStr, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
								Excel.XlSaveAsAccessMode.xlNoChange, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
			resWorkbook.Close();
		}

		~IOModule() {
			excelApp.Quit();
		}

		public void WriteResult(ResultList res, int step, int start) {
			Excel.Workbook workbook = excelApp.Workbooks.Open(resultsStr);

			Excel.Worksheet worksheet = workbook.Worksheets.Add();
			worksheet.Name = "Результат";

			worksheet.Cells[1, "A"].Value = "Ранг матрицы";
			worksheet.Cells[1, "B"].Value = "Время тривиального алгоритма, с";
			worksheet.Cells[1, "C"].Value = "Время алгоритма Винограда-Штрассена, с";
			worksheet.Cells[1, "D"].Value = "Время алгоритма Винограда-Штрассена 64, с";

			for(int i = 2; i <= res.Length + 1; i++) {
				worksheet.Cells[i, "A"].Value = start + (i - 2) * step;

				double trivTime = res[i - 2, 0];
				double strassenTime = res[i - 2, 1];
				double strassen64Time = res[i - 2, 2];

				if(trivTime != -1) {
					worksheet.Cells[i, "B"].Value = trivTime;
				}

				if(strassenTime != -1) {
					worksheet.Cells[i, "C"].Value = strassenTime;
				}

				if(strassen64Time != -1) {
					worksheet.Cells[i, "D"].Value = strassen64Time;
				}
			}

			string graphTitle = $"Время умножения матриц, в зависимости от ранга, с";
			string xAxisName = "n";
			string yAxisName = "t";

			Excel.ChartObjects charts = worksheet.ChartObjects();
			Excel.ChartObject chartObject = charts.Add(200, 20, 1000, 300);
			Excel.Chart chart = chartObject.Chart;
			chart.ChartType = Microsoft.Office.Interop.Excel.XlChartType.xlLine;

			chart.SeriesCollection().NewSeries();
			chart.FullSeriesCollection(1).Values = $"{worksheet.Name}!$B$2:$B${1 + res.Length}";
			chart.FullSeriesCollection(1).Name = worksheet.Cells[1, "B"].Value2;

			chart.SeriesCollection().NewSeries();
			chart.FullSeriesCollection(2).Values = $"{worksheet.Name}!$C$2:$C${1 + res.Length}";
			chart.FullSeriesCollection(2).Name = worksheet.Cells[1, "C"].Value2;

			chart.SeriesCollection().NewSeries();
			chart.FullSeriesCollection(3).Values = $"{worksheet.Name}!$D$2:$D${1 + res.Length}";
			chart.FullSeriesCollection(3).Name = worksheet.Cells[1, "D"].Value2;

			Excel.Axis xAxis = (Excel.Axis)chart.Axes(Excel.XlAxisType.xlCategory, Excel.XlAxisGroup.xlPrimary);
			xAxis.CategoryNames = worksheet.get_Range("A2", "A" + (1 + res.Length));
			xAxis.TickLabels.Orientation = (Excel.XlTickLabelOrientation)35;
			xAxis.MajorTickMark = XlTickMark.xlTickMarkCross;
			xAxis.MinorTickMark = XlTickMark.xlTickMarkCross;
			xAxis.AxisBetweenCategories = false;

			chart.ChartWizard(
				Title: graphTitle,
				CategoryTitle: xAxisName,
				ValueTitle: yAxisName);

			workbook.Save();
			workbook.Close();
		}
	}
}