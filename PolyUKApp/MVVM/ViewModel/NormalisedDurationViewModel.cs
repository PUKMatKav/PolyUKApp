using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ZstdSharp.Unsafe;

namespace PolyUKApp.MVVM.ViewModel
{

    public class NormalisedDurationViewModel : INotifyPropertyChanged
    {
        private bool _showTotal = false;
        public bool ShowTotal
        {
            get => _showTotal;
            private set
            {
                _showTotal = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ShowTotal)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ToggleButtonText)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ShowFilters)));
            }
        }
        private DataTable _dt;

        private Dictionary<(string Owner, int Week), double> _daysWorked = new();
        public ObservableCollection<ISeries> Series { get; set; } = new();
        public ObservableCollection<OwnerFilter> OwnerFilters { get; set; } = new();
        public ICommand ToggleViewCommand { get; }
        public string ToggleButtonText => _showTotal ? "Show Individual" : "Show Total";
        public System.Windows.Visibility ShowFilters => _showTotal
            ? System.Windows.Visibility.Collapsed
            : System.Windows.Visibility.Visible;

        public event PropertyChangedEventHandler? PropertyChanged;

        private Axis[] _xAxes;
        public Axis[] XAxes
        {
            get => _xAxes;
            set
            {
                _xAxes = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(XAxes)));
            }
        }

        private Axis[] _yAxes;
        public Axis[] YAxes
        {
            get => _yAxes;
            set
            {
                _yAxes = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(YAxes)));
            }
        }

        public NormalisedDurationViewModel()
        {
            ToggleViewCommand = new RelayCommand(ToggleView);
        }

        public void LoadData(DataTable sirusDataTable, DataTable daysWorkedTable)
        {
            _dt = sirusDataTable;
            _daysWorked = new Dictionary<(string Owner, int Week), double>();

            foreach (DataRow row in daysWorkedTable.Rows)
            {
                string owner = row.Field<string>("SalesPerson");

                foreach (DataColumn col in daysWorkedTable.Columns)
                {
                    // Skip the SalesPerson column, only process week columns (W1, W2 etc)
                    if (!col.ColumnName.StartsWith("W")) continue;

                    if (int.TryParse(col.ColumnName.Substring(1), out int weekNum))
                    {
                        double days = Convert.ToDouble(row[col]);
                        _daysWorked[(owner, weekNum)] = days;
                    }
                }
            }
            LoadChart();
        }
        public void Rebuild(ObservableCollection<OwnerFilter>? filters)
        {
            Series.Clear();
            OwnerFilters.Clear();

            if (_showTotal)
            {
                LoadTotalChart();
                return;
            }

            var allWeeks = Enumerable.Range(1, 52).ToList();

            var owners = _dt.AsEnumerable()
                .Select(r => r.Field<string>("Owner"))
                .Distinct()
                .OrderBy(o => o)
                .ToList();

            var colors = GenerateColors(owners.Count);

            for (int i = 0; i < owners.Count; i++)
            {
                string owner = owners[i];

                var durationByWeek = _dt.AsEnumerable()
                    .Where(r => r.Field<string>("Owner") == owner)
                    .GroupBy(r => Convert.ToInt32(r.Field<string>("WeekNum")))
                    .ToDictionary(g => g.Key, g => g.Sum(r => r.Field<double>("Duration (s)")));

                var values = allWeeks.Select(w =>
                {
                    if (!durationByWeek.TryGetValue(w, out var dur)) return (double?)null;
                    if (!_daysWorked.TryGetValue((owner, w), out var days) || days == 0) return null;
                    return dur / days;
                }).ToArray();

                var series = new LineSeries<double?>
                {
                    Name = owner,
                    Values = values,
                    Stroke = new SolidColorPaint(colors[i]) { StrokeThickness = 2 },
                    Fill = null,
                    GeometrySize = 6,
                    GeometryStroke = new SolidColorPaint(colors[i]) { StrokeThickness = 2 },
                    GeometryFill = new SolidColorPaint(SKColors.White),
                    LineSmoothness = 0.5
                };

                // Apply visibility from filters if provided
                if (filters != null)
                {
                    var match = filters.FirstOrDefault(f => f.Owner == owner);
                    if (match != null) series.IsVisible = match.IsVisible;
                }

                Series.Add(series);

                OwnerFilters.Add(new OwnerFilter
                {
                    Owner = owner,
                    Series = series,
                    Color = new System.Windows.Media.SolidColorBrush(
                        System.Windows.Media.Color.FromRgb(colors[i].Red, colors[i].Green, colors[i].Blue))
                });
            }

            SetAxes(allWeeks);
        }

        public void ToggleView()
        {
            _showTotal = !_showTotal;  // flip the backing field directly, no notification yet
            Series.Clear();
            OwnerFilters.Clear();

            if (_showTotal)
                LoadTotalChart();
            else
                LoadChart();           // OwnerFilters is fully rebuilt before we notify

            // NOW raise all the notifications, OwnerFilters is ready
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ShowTotal)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ToggleButtonText)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ShowFilters)));
        }

        private void LoadTotalChart()
        {
            var allWeeks = Enumerable.Range(1, 52).ToList();

            var durationByWeek = _dt.AsEnumerable()
                .GroupBy(r => Convert.ToInt32(r.Field<string>("WeekNum")))
                .ToDictionary(g => g.Key, g => g.Sum(r => r.Field<double>("Duration (s)")));

            var totalDays = _daysWorked.Values.Sum();

            var values = allWeeks.Select(w =>
            {
                if (!durationByWeek.TryGetValue(w, out var dur)) return (double?)null;
                var totalDays = _daysWorked
                    .Where(kv => kv.Key.Week == w)
                    .Sum(kv => kv.Value);
                if (totalDays == 0) return null;
                return dur / totalDays;
            }).ToArray();

            Series.Add(new LineSeries<double?>
            {
                Name = "All Sales",
                Values = values,
                Stroke = new SolidColorPaint(SKColor.Parse("#4E79A7")) { StrokeThickness = 2 },
                Fill = null,
                GeometrySize = 6,
                GeometryStroke = new SolidColorPaint(SKColor.Parse("#4E79A7")) { StrokeThickness = 2 },
                GeometryFill = new SolidColorPaint(SKColors.White),
                LineSmoothness = 0.5
            });

            // axes still need setting when switching to total view
            SetAxes(allWeeks);
        }

        private void LoadChart()
        {
            var allWeeks = Enumerable.Range(1, 52).ToList();

            var owners = _dt.AsEnumerable()
                .Select(r => r.Field<string>("Owner"))
                .Distinct()
                .OrderBy(o => o)
                .ToList();

            var colors = GenerateColors(owners.Count);

            for (int i = 0; i < owners.Count; i++)
            {
                string owner = owners[i];

                var durationByWeek = _dt.AsEnumerable()
                    .Where(r => r.Field<string>("Owner") == owner)
                    .GroupBy(r => Convert.ToInt32(r.Field<string>("WeekNum")))
                    .ToDictionary(g => g.Key, g => g.Sum(r => r.Field<double>("Duration (s)")));

                var values = allWeeks.Select(w =>
                {
                    if (!durationByWeek.TryGetValue(w, out var dur)) return (double?)null;
                    if (!_daysWorked.TryGetValue((owner, w), out var days) || days == 0) return null;
                    return dur / days;
                }).ToArray();

                var series = new LineSeries<double?>
                {
                    Name = owner,
                    Values = values,
                    Stroke = new SolidColorPaint(colors[i]) { StrokeThickness = 2 },
                    Fill = null,
                    GeometrySize = 6,
                    GeometryStroke = new SolidColorPaint(colors[i]) { StrokeThickness = 2 },
                    GeometryFill = new SolidColorPaint(SKColors.White),
                    LineSmoothness = 0.5
                };

                Series.Add(series);

                OwnerFilters.Add(new OwnerFilter
                {
                    Owner = owner,
                    Series = series,
                    Color = new System.Windows.Media.SolidColorBrush(
                        System.Windows.Media.Color.FromRgb(colors[i].Red, colors[i].Green, colors[i].Blue))
                });
            }

            SetAxes(allWeeks);
        }

        // Single place that sets both axes — both raise PropertyChanged automatically
        private void SetAxes(List<int> allWeeks)
        {
            XAxes = new[]
            {
            new Axis
            {
                Name = "",
                NameTextSize = 0,
                TextSize = 10,
                Labels = allWeeks.Select(w => $"W{w}").ToArray(),
                LabelsRotation = -30
            }
        };

            YAxes = new[]
            {
            new Axis
            {
                Name = "Normalised Time",
                NameTextSize = 0,
                Labeler = value =>
                {
                    var totalSeconds = (int)Math.Round(value);
                    var hours = (totalSeconds / 60) / 60;
                    var minutes = (totalSeconds / 60) - (hours * 60);
                    return $"{hours:D2}:{minutes:D2}";
                }
            }
        };
        }

        private static SKColor[] GenerateColors(int count)
        {
            var palette = new[]
            {
            SKColor.Parse("#4E79A7"),
            SKColor.Parse("#F28E2B"),
            SKColor.Parse("#E15759"),
            SKColor.Parse("#76B7B2"),
            SKColor.Parse("#59A14F"),
            SKColor.Parse("#EDC948"),
            SKColor.Parse("#B07AA1"),
            SKColor.Parse("#FF9DA7"),
            SKColor.Parse("#9C755F"),
            SKColor.Parse("#BAB0AC"),
        };

            return Enumerable.Range(0, count)
                .Select(i => palette[i % palette.Length])
                .ToArray();
        }
    }
}
