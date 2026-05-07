using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.Drawing;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PolyUKApp.MVVM.ViewModel
{
    public class CallHeatmapViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private int _currentWeek = GetCurrentWeekNumber();
        private int _selectedWeek;
        public ICommand ThisWeekCommand { get; }
        public ICommand LastWeekCommand { get; }

        private const int StartHour = 8;
        private const int EndHour = 18;
        private const int HourCount = EndHour - StartHour;

        private static readonly DayOfWeek[] WorkDays =
        {
        DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday,
        DayOfWeek.Thursday, DayOfWeek.Friday
    };

        private static readonly string[] DayLabels = { "Mon", "Tue", "Wed", "Thu", "Fri" };
        private static readonly string[] HourLabels =
            Enumerable.Range(StartHour, HourCount)
                      .Select(h => $"{h:00}:00")
                      .ToArray();

        private DataTable _fullData;

        private ISeries[] _series = Array.Empty<ISeries>();
        public ISeries[] HeatSeries
        {
            get => _series;
            private set
            {
                _series = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(HeatSeries)));
            }
        }
        public Axis[] HeatXAxes { get; private set; } = Array.Empty<Axis>();
        public Axis[] HeatYAxes { get; private set; } = Array.Empty<Axis>();

        private string _weekButtonText = "Last Week";
        public string WeekButtonText
        {
            get => _weekButtonText;
            private set
            {
                _weekButtonText = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(WeekButtonText)));
            }
        }

        public ICommand ToggleWeekCommand { get; }

        public CallHeatmapViewModel()
        {
            _selectedWeek = _currentWeek;
            ThisWeekCommand = new RelayCommand(ShowThisWeek);
            LastWeekCommand = new RelayCommand(ShowLastWeek);
        }

        private void ShowThisWeek()
        {
            _selectedWeek = _currentWeek;
            Rebuild(_lastUsedFilters);
        }

        private void ShowLastWeek()
        {
            _selectedWeek = _currentWeek - 1;
            Rebuild(_lastUsedFilters);
        }

        private static int GetCurrentWeekNumber()
        {
            return System.Globalization.ISOWeek.GetWeekOfYear(DateTime.Now);
        }

        // store the last used filters so ToggleWeek can re-apply them
        private IEnumerable<OwnerFilter> _lastUsedFilters;

        public void LoadData(DataTable sirusDataTable)
        {
            _fullData = sirusDataTable;
            _selectedWeek = _currentWeek;
            HeatXAxes = BuildXAxes();
            HeatYAxes = BuildYAxes();
            Rebuild(null);
        }

        public void ApplyOwnerFilter(IEnumerable<OwnerFilter> filters)
        {
            Rebuild(filters);
        }

        public void Rebuild(IEnumerable<OwnerFilter> filters)
        {
            _lastUsedFilters = filters;

            var visibleOwners = filters?
                .Where(f => f.IsVisible)
                .Select(f => f.Owner)
                .ToHashSet();

            var rows = _fullData.AsEnumerable()
                .Where(r =>
                {
                    // filter by week number
                    if (Convert.ToInt32(r.Field<string>("WeekNum")) != _selectedWeek) return false;

                    // filter by owner
                    if (visibleOwners == null) return true;
                    return visibleOwners.Contains(r.Field<string>("Owner"));
                });

            var heatData = AggregateRows(rows);
            HeatSeries = BuildSeries(heatData);
        }

        //public void LoadData(DataTable sirusDataTable)
        //{
        //    _fullData = sirusDataTable;
        //    HeatXAxes = BuildXAxes();
        //    HeatYAxes = BuildYAxes();
        //    Rebuild(null);  // null = no filter, show all owners
        //}

        //// Called from MainViewModel whenever an OwnerFilter toggles
        //public void ApplyOwnerFilter(IEnumerable<OwnerFilter> filters)
        //{
        //    Rebuild(filters);
        //}

        //public void Rebuild(IEnumerable<OwnerFilter> filters)
        //{
        //    var visibleOwners = filters?
        //        .Where(f => f.IsVisible)
        //        .Select(f => f.Owner)
        //        .ToHashSet();

        //    var rows = _fullData.AsEnumerable()
        //        .Where(r =>
        //        {
        //            if (visibleOwners == null) return true;
        //            return visibleOwners.Contains(r.Field<string>("Owner"));
        //        });

        //    var heatData = AggregateRows(rows);
        //    HeatSeries = BuildSeries(heatData);
        //}

        private static int[,] AggregateRows(IEnumerable<DataRow> rows)
        {
            var heatData = new int[5, HourCount];

            foreach (var row in rows)
            {
                var dt = Convert.ToDateTime(row["Call Time"]);
                int hour = Convert.ToInt32(row["CallHour"]);

                if (!WorkDays.Contains(dt.DayOfWeek)) continue;
                if (hour < StartHour || hour >= EndHour) continue;

                int dayIndex = (int)dt.DayOfWeek - 1;
                int hourIndex = hour - StartHour;

                heatData[dayIndex, hourIndex]++;
            }

            return heatData;
        }

        private static ISeries[] BuildSeries(int[,] heatData)
        {
            var values = new List<WeightedPoint>();

            for (int d = 0; d < 5; d++)
                for (int h = 0; h < HourCount; h++)
                    values.Add(new WeightedPoint(h, d, heatData[d, h]));

            return new ISeries[]
            {
            new HeatSeries<WeightedPoint>
            {
                HeatMap = new[]
                {
                    new LvcColor(255, 241, 118,   0),
                    new LvcColor(249, 115,  22, 255),
                    new LvcColor(220,  38,  38, 255),
                },
                Values      = values,
                Name        = "Call Volume",
                DataPadding = new LvcPoint(0, 0)
            }
            };
        }

        private static Axis[] BuildXAxes() => new[]
        {
        new Axis { Labels = HourLabels, Name = "Hour of Day", LabelsRotation = -45, NameTextSize = 0, TextSize = 10 }
    };

        private static Axis[] BuildYAxes() => new[]
        {
        new Axis { Labels = DayLabels, Name = "Day of Week", NameTextSize = 0, TextSize = 10 }
    };
    }
}
