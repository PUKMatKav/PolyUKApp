using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace PolyUKApp.MVVM.ViewModel
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public DurationByWeekViewModel DurationByWeek { get; } = new();
        public CallHeatmapViewModel CallHeatmap { get; } = new();
        public NormalisedDurationViewModel NormalisedDuration { get; } = new();


        public void LoadData(DataTable sirusDataTable, DataTable daysWorkedTable)
        {
            DurationByWeek.LoadData(sirusDataTable);
            CallHeatmap.LoadData(sirusDataTable);
            NormalisedDuration.LoadData(sirusDataTable, daysWorkedTable);

            DurationByWeek.ViewToggled += (s, e) =>
            {
                NormalisedDuration.ToggleView();
            };

            // Listen for toggle button changes
            DurationByWeek.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(DurationByWeekViewModel.ShowTotal))
                {
                    if (DurationByWeek.ShowTotal)
                    {
                        CallHeatmap.Rebuild(null);
                        NormalisedDuration.Rebuild(null); // <-- add
                    }
                    else
                    {
                        foreach (var filter in DurationByWeek.OwnerFilters)
                            filter.PropertyChanged += (fs, fe) =>
                            {
                                if (fe.PropertyName == nameof(OwnerFilter.IsVisible))
                                {
                                    CallHeatmap.Rebuild(DurationByWeek.OwnerFilters);
                                    NormalisedDuration.Rebuild(DurationByWeek.OwnerFilters); // <-- add
                                }
                            };

                        CallHeatmap.Rebuild(DurationByWeek.OwnerFilters);
                        NormalisedDuration.Rebuild(DurationByWeek.OwnerFilters); // <-- add
                    }
                }
            };

            // Initial subscription for the first load
            SubscribeToFilters();
        }

        private void SubscribeToFilters()
        {
            foreach (var filter in DurationByWeek.OwnerFilters)
                filter.PropertyChanged += (s, e) =>
                {
                    if (e.PropertyName == nameof(OwnerFilter.IsVisible))
                    {
                        CallHeatmap.Rebuild(DurationByWeek.OwnerFilters);
                        NormalisedDuration.Rebuild(DurationByWeek.OwnerFilters); // <-- add
                    }
                };
        }
    }
}
