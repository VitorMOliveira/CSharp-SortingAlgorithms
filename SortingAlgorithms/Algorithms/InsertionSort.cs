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
using System.Collections;

namespace SortingAlgorithms.Algorithms
{
    public class InsertionSort : ISortAlgorithms
    {
        private double[] _sortingArray;
        private readonly Canvas _canvas;
        private readonly int _maxVal;
        private readonly CancellationToken _token;
        private readonly MainWindow _mainWindow;

        public InsertionSort(MainWindow mainWindow, Canvas sortingCanvas, double[] sortingArray,
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
            SortSoundPlayer.PlaySoundLoopTask(_maxVal, _token);

            for (int i = 1; i < _sortingArray.Length; ++i)
            {
                double key = _sortingArray[i];
                int j = i - 1;

                // Move elements of arr[0..i-1], that are greater than key,
                // to one position ahead of their current position
                while (j >= 0 && _sortingArray[j] > key)
                {
                    _sortingArray[j + 1] = _sortingArray[j];

                    await _mainWindow.Dispatcher.InvokeAsync(() => Swap(j + 1, j));

                    SortSoundPlayer.AdjustFrequency(j);

                    if (_token.IsCancellationRequested)
                    {
                        _token.ThrowIfCancellationRequested();
                    }

                    Thread.Sleep(1);

                    j = j - 1;
                }
                _sortingArray[j + 1] = key;

                await _mainWindow.Dispatcher.InvokeAsync(() =>
                            SortEngine.UpdateOneElementInCanvas(_canvas, _sortingArray, j + 1));

                if (_token.IsCancellationRequested)
                {
                    _token.ThrowIfCancellationRequested();
                }

                Thread.Sleep(1);
            }
        }

        private void Swap(int i, int v)
        {
            SortEngine.UpdateCanvas(_canvas, _sortingArray, i, v);
        }



    }
}
