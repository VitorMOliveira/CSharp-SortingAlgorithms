using System;
using System.Collections.Generic;
using System.Windows.Shapes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using System.Diagnostics;

namespace SortingAlgorithms.old
{
    public class BubbleSortV1
    {
        private bool _sorted = false;
        private double[] _sortingArray;
        private readonly Canvas _canvas;
        private readonly int _maxVal;
        private readonly CancellationToken _token;
        private readonly MainWindow _mainWindow;

        static readonly Brush WhiteBrush = new SolidColorBrush(Colors.White);


        private static readonly int rectWidth = SortParameters.RECT_WIDTH;

        public BubbleSortV1(MainWindow mainWindow, Canvas sortingCanvas, double[] sortingArray,
                                    CancellationToken token)
        {
            _sortingArray = sortingArray;
            _canvas = sortingCanvas;
            _maxVal = (int)sortingCanvas.ActualHeight + SortParameters.RECT_DRAW_OFFSET_LAST;
            _token = token;
            _mainWindow = mainWindow;
        }

        public async Task DoSorting()
        {

            while (_sorted == false)
            {
                for (int i = 0; i < _sortingArray.Length - 1; i++)
                {
                    if (_sortingArray[i] > _sortingArray[i + 1])
                    {
                        await _mainWindow.Dispatcher.InvokeAsync(() => Swap(i, i + 1));

                        if (_token.IsCancellationRequested)
                        {
                            _token.ThrowIfCancellationRequested();
                        }

                        Thread.Sleep(1);
                    }
                }
                _sorted = IsSorted();
            }
        }

        private void Swap(int i, int v)
        {
            double temp = _sortingArray[i];
            _sortingArray[i] = _sortingArray[v];
            _sortingArray[v] = temp;

            SortEngine.UpdateCanvas(_canvas, _sortingArray, i, v);
        }

        private bool IsSorted()
        {
            for (int i = 0; i < _sortingArray.Length; i++)
            {
                if (_sortingArray[i] > _sortingArray[i + 1])
                {
                    return false;
                }
            }
            return true;
        }

    }
}
