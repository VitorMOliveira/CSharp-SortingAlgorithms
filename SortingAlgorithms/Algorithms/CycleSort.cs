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
    public class CycleSort : ISortAlgorithms
    {
        private double[] _sortingArray;
        private readonly Canvas _canvas;
        private readonly int _maxVal;
        private readonly CancellationToken _token;
        private readonly MainWindow _mainWindow;

        public CycleSort(MainWindow mainWindow, Canvas sortingCanvas, double[] sortingArray,
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

            // count number of memory writes
            int writes = 0;
            int n = _sortingArray.Length;

            // traverse array elements and 
            // put it to on the right place
            for (int cycle_start = 0; cycle_start <= n - 2; cycle_start++)
            {
                // initialize item as starting point
                double item = _sortingArray[cycle_start];

                // Find position where we put the item. 
                // We basically count all smaller elements 
                // on right side of item.
                int pos = cycle_start;
                for (int i = cycle_start + 1; i < n; i++)
                    if (_sortingArray[i] < item)
                        pos++;

                // If item is already in correct position
                if (pos == cycle_start)
                    continue;

                // ignore all duplicate elements
                while (item == _sortingArray[pos])
                    pos += 1;

                // put the item to it's right position
                if (pos != cycle_start)
                {
                    double temp = item;
                    item = _sortingArray[pos];
                    _sortingArray[pos] = temp;

                    await _mainWindow.Dispatcher.InvokeAsync(() =>
                                SortEngine.UpdateOneElementInCanvas(_canvas, _sortingArray, pos));

                    SortSoundPlayer.AdjustFrequency(pos);

                    if (_token.IsCancellationRequested)
                    {
                        _token.ThrowIfCancellationRequested();
                    }

                    Thread.Sleep(1);

                    writes++;
                }

                // Rotate rest of the cycle
                while (pos != cycle_start)
                {
                    pos = cycle_start;

                    // Find position where we put the element
                    for (int i = cycle_start + 1; i < n; i++)
                        if (_sortingArray[i] < item)
                            pos += 1;

                    // ignore all duplicate elements
                    while (item == _sortingArray[pos])
                        pos += 1;

                    // put the item to it's right position
                    if (item != _sortingArray[pos])
                    {
                        double temp = item;
                        item = _sortingArray[pos];
                        _sortingArray[pos] = temp;

                        await _mainWindow.Dispatcher.InvokeAsync(() =>
                                SortEngine.UpdateOneElementInCanvas(_canvas, _sortingArray, pos));

                        SortSoundPlayer.AdjustFrequency(pos);

                        if (_token.IsCancellationRequested)
                        {
                            _token.ThrowIfCancellationRequested();
                        }

                        Thread.Sleep(1);

                        writes++;
                    }
                }
            }
        }

    }
}
