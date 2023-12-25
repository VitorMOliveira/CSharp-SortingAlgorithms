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
using System.Reflection;

namespace SortingAlgorithms.Algorithms
{
    public class ShellSort : ISortAlgorithms
    {
        private double[] _sortingArray;
        private readonly Canvas _canvas;
        private readonly int _maxVal;
        private readonly CancellationToken _token;
        private readonly MainWindow _mainWindow;

        public ShellSort(MainWindow mainWindow, Canvas sortingCanvas, double[] sortingArray,
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

            int n = _sortingArray.Length;

            // Start with a big gap,  
            // then reduce the gap 
            for (int gap = n / 2; gap > 0; gap /= 2)
            {
                // Do a gapped insertion sort for this gap size. 
                // The first gap elements a[0..gap-1] are already 
                // in gapped order keep adding one more element 
                // until the entire array is gap sorted 
                for (int i = gap; i < n; i += 1)
                {
                    // add a[i] to the elements that have 
                    // been gap sorted save a[i] in temp and 
                    // make a hole at position i 
                    double temp = _sortingArray[i];

                    // shift earlier gap-sorted elements up until 
                    // the correct location for a[i] is found 
                    int j;
                    for (j = i; j >= gap && _sortingArray[j - gap] > temp; j -= gap)
                    {
                        _sortingArray[j] = _sortingArray[j - gap];

                        await _mainWindow.Dispatcher.InvokeAsync(() => Swap(j, j - gap));

                        SortSoundPlayer.AdjustFrequency(j);

                        if (_token.IsCancellationRequested)
                        {
                            _token.ThrowIfCancellationRequested();
                        }

                        Thread.Sleep(1);
                    }

                    // put temp (the original a[i]) in its correct location 
                    _sortingArray[j] = temp;

                    await _mainWindow.Dispatcher.InvokeAsync(() =>
                                SortEngine.UpdateOneElementInCanvas(_canvas, _sortingArray, j));

                    SortSoundPlayer.AdjustFrequency(j);

                    if (_token.IsCancellationRequested)
                    {
                        _token.ThrowIfCancellationRequested();
                    }

                    Thread.Sleep(1);
                }
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
