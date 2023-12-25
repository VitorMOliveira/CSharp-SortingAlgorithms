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
    public class HeapSort : ISortAlgorithms
    {
        private double[] _sortingArray;
        private readonly Canvas _canvas;
        private readonly int _maxVal;
        private readonly CancellationToken _token;
        private readonly MainWindow _mainWindow;

        public HeapSort(MainWindow mainWindow, Canvas sortingCanvas, double[] sortingArray,
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

            int N = _sortingArray.Length;

            // Build heap (rearrange array)
            for (int i = N / 2 - 1; i >= 0; i--)
                await Heapify(_sortingArray, N, i);

            // One by one extract an element from heap
            for (int i = N - 1; i > 0; i--)
            {
                await _mainWindow.Dispatcher.InvokeAsync(() => Swap(0, i));

                SortSoundPlayer.AdjustFrequency(i);

                if (_token.IsCancellationRequested)
                {
                    _token.ThrowIfCancellationRequested();
                }

                Thread.Sleep(1);

                // call max heapify on the reduced heap
                await Heapify(_sortingArray, i, 0);
            }
        }

        private async Task Heapify(double[] arr, int N, int i)
        {
            int largest = i; // Initialize largest as root
            int l = 2 * i + 1; // left = 2*i + 1
            int r = 2 * i + 2; // right = 2*i + 2

            // If left child is larger than root
            if (l < N && arr[l] > arr[largest])
                largest = l;

            // If right child is larger than largest so far
            if (r < N && arr[r] > arr[largest])
                largest = r;

            // If largest is not root
            if (largest != i)
            {
                await _mainWindow.Dispatcher.InvokeAsync(() => Swap(i, largest));

                SortSoundPlayer.AdjustFrequency(i);

                if (_token.IsCancellationRequested)
                {
                    _token.ThrowIfCancellationRequested();
                }

                Thread.Sleep(1);

                // Recursively heapify the affected sub-tree
                await Heapify(arr, N, largest);
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
