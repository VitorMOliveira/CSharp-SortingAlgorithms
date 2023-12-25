using System;
using System.Diagnostics;
using System.IO;
using System.Media;
using System.Reflection.Emit;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using SortingAlgorithms.Algorithms;

namespace SortingAlgorithms
{
    public partial class MainWindow : Window
    {
        CancellationTokenSource cts;
        readonly DispatcherTimer timer;
        Stopwatch sw;
        readonly MainWindow mw;
        double[] sortingArray;
        CancellationToken ct;
        string selectedSortAlgorithm;
        int sortingCount = 1;
        string sortingTimeElapsed;

        public MainWindow()
        {
            mw = this;
            InitializeComponent();

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(100);

            PopulateComboBoxWithSortingAlgorithms();
        }

        private void ResetSortingBtn_Click(object sender, RoutedEventArgs e)
        {
            cts?.Cancel(); // cancel task if it is running

            sortingArray = SortEngine.GetShuffledArray(SortingCanvas);

            PickAlgorithm();
        }

        void PickAlgorithm()
        {
            cts = new CancellationTokenSource();
            ct = cts.Token;

            selectedSortAlgorithm = (string) SortingCBox.SelectedValue;

            StartRecordingTimeElapsed();

            switch (selectedSortAlgorithm)
            {
                case "BubbleSort":
                    ISortAlgorithms bubbleSort = new BubbleSort(mw, SortingCanvas, sortingArray, ct);
                    RunSorting(bubbleSort);
                    break;

                case "QuickSort":
                    ISortAlgorithms quickSort = new QuickSort(mw, SortingCanvas, sortingArray, 0, sortingArray.Length - 1, ct);
                    RunSorting(quickSort);
                    break;

                case "MergeSort":
                    ISortAlgorithms mergeSort = new MergeSort(mw, SortingCanvas, sortingArray, 0, sortingArray.Length - 1, ct);
                    RunSorting(mergeSort);
                    break;

                case "InsertionSort":
                    ISortAlgorithms insertionSort = new InsertionSort(mw, SortingCanvas, sortingArray, ct);
                    RunSorting(insertionSort);
                    break;

                case "HeapSort":
                    ISortAlgorithms heapSort = new HeapSort(mw, SortingCanvas, sortingArray, ct);
                    RunSorting(heapSort);
                    break;

                case "ShellSort":
                    ISortAlgorithms shellSort = new ShellSort(mw, SortingCanvas, sortingArray, ct);
                    RunSorting(shellSort);
                    break;

                case "CycleSort":
                    ISortAlgorithms cycleSort = new CycleSort(mw, SortingCanvas, sortingArray, ct);
                    RunSorting(cycleSort);
                    break;

                default:
                    break;
            } 
        }

        void RunSorting(ISortAlgorithms sortAlgorithm)
        {
            bool isCancelled = false;
            _ = Task.Run(async () =>
                {
                    try
                    {
                        await sortAlgorithm.DoSorting();
                    }
                    catch (OperationCanceledException)
                    {
                        isCancelled = true;
                    }
                }
                ).ContinueWith(t =>
                    {
                        if (isCancelled == false)
                        {
                            StopRecordingTimeElapsed();
                            UpdateSortRecordingTimesTextBox();
                        }
                    }
                ).ContinueWith(
                    t => Task.Run(async () =>
                    {
                        if (isCancelled == false)
                        {
                            try
                            {
                                await SortEngine.CompletedSorting(mw, sortingArray, SortingCanvas, ct);
                            }
                            catch (Exception) { }
                        }
                    }
                    )
                );
        }

        void UpdateSortRecordingTimesTextBox()
        {
            this.Dispatcher.Invoke(() =>
            {
                textBoxRecordingSortingTimes.AppendText(
                    $"[{sortingCount}] {selectedSortAlgorithm} finished in {sortingTimeElapsed}\n");
                textBoxRecordingSortingTimes.ScrollToEnd();
            });
            sortingCount++;
        }

        void StartRecordingTimeElapsed()
        {
            sw = new Stopwatch();
            sw.Start();

            timer.Tick += (s, e) => { UpdateTimeElapsed(s!, e); };
            timer.Start();
        }

        public void StopRecordingTimeElapsed()
        {
            sw.Stop();
            timer.Stop();
        }

        void UpdateTimeElapsed(object sender, EventArgs e)
        {
            sortingTimeElapsed = sw.Elapsed.ToString(@"m\:ss\.ff");
            timeElapsedLabel.Content = $"Time Elapsed: {sortingTimeElapsed}";
        }

        void PopulateComboBoxWithSortingAlgorithms()
        {
            List<string> allAlgorithmsNames = SortEngine.GetAllAlgorithmsNames();

            foreach (string algorithmName in allAlgorithmsNames)
            {
                SortingCBox.Items.Add(algorithmName);
            }
            SortingCBox.SelectedIndex = 0;
        }

        private void TestBtn_Click(object sender, RoutedEventArgs e)
        {
 
        }

        private void Test2Btn_Click(object sender, RoutedEventArgs e)
        {
 
        }


    }
}