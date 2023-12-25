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
using System.Runtime.CompilerServices;

namespace SortingAlgorithms.Algorithms
{
    public class QuickSort : ISortAlgorithms
    {
        private double[] _sortingArray;
        private readonly int _maxVal;
        private readonly Canvas _canvas;
        private readonly CancellationToken _token;
        private readonly MainWindow _mainWindow;

        private readonly int _startIndex, _endIndex;

        public QuickSort(MainWindow mainWindow, Canvas sortingCanvas, double[] sortingArray, int startIndex, int endIndex,
                                    CancellationToken token)
        {
            _sortingArray = sortingArray;
            _maxVal = (int)sortingCanvas.ActualHeight + SortParameters.RECT_DRAW_OFFSET_LAST;
            _canvas = sortingCanvas;
            _token = token;
            _mainWindow = mainWindow;
            _startIndex = startIndex; // low
            _endIndex = endIndex; // high

        }

        public async Task DoSorting()
        {
            SortSoundPlayer.PlaySoundLoopTask(_maxVal, _token);
            await SortArray(_sortingArray, _startIndex, _endIndex);
        }

        private async Task SortArray(double[] array, int leftIndex, int rightIndex)
        {
            var i = leftIndex;
            var j = rightIndex;
            var pivot = array[leftIndex];
            while (i <= j)
            {
                while (array[i] < pivot)
                {
                    i++;
                }

                while (array[j] > pivot)
                {
                    j--;
                }
                if (i <= j)
                {
                    await _mainWindow.Dispatcher.InvokeAsync(() => Swap(i, j));

                    SortSoundPlayer.AdjustFrequency(i);

                    if (_token.IsCancellationRequested)
                    {
                        _token.ThrowIfCancellationRequested();
                    }

                    Thread.Sleep(1);

                    i++;
                    j--;
                }
            }

            if (leftIndex < j)
                await SortArray(array, leftIndex, j);
            if (i < rightIndex)
                await SortArray(array, i, rightIndex);
        }

        private void Swap(int i, int v)
        {
            double temp = _sortingArray[i];
            _sortingArray[i] = _sortingArray[v];
            _sortingArray[v] = temp;

            SortEngine.UpdateCanvas(_canvas, _sortingArray, i, v);
        }


    }
}
