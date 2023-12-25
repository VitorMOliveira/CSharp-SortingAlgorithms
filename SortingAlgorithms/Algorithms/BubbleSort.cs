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

namespace SortingAlgorithms.Algorithms
{
    public class BubbleSort : ISortAlgorithms
    {
        private double[] _sortingArray;
        private readonly Canvas _canvas;
        private readonly int _maxVal;
        private readonly CancellationToken _token;
        private readonly MainWindow _mainWindow;

        public BubbleSort(MainWindow mainWindow, Canvas sortingCanvas, double[] sortingArray,
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

            int i, j;
            bool swapped;
            for (i = 0; i < _sortingArray.Length - 1; i++)
            {
                swapped = false;
                for (j = 0; j < _sortingArray.Length - i - 1; j++)
                {
                    if (_sortingArray[j] > _sortingArray[j + 1])
                    {
                        await _mainWindow.Dispatcher.InvokeAsync(() => Swap(j, j + 1));
                        swapped = true;

                        SortSoundPlayer.AdjustFrequency(j);

                        if (_token.IsCancellationRequested)
                        {
                            _token.ThrowIfCancellationRequested();
                        }

                        Thread.Sleep(1);
                    }
                }

                // If no two elements were swapped by inner loop, then break
                if (swapped == false)
                    break;
            }
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
