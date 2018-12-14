using System;
using System.Threading.Tasks;

namespace SleepingBarberAsync
{

    #region Events

    public class SearchingForChairStartedEventArgs : EventArgs
    {
        public SearchingForChairStartedEventArgs(string message)
        {
            Message = message;
        }

        public string Message { get; private set; }
    }
   
    #endregion


    public class Customer
    {
        private readonly DelayRange _walkingTimeRange;

        public Customer(int id, DelayRange walkingTimeRange)
        {
            _walkingTimeRange = walkingTimeRange;
            Id = id;
        }

        public event EventHandler<SearchingForChairStartedEventArgs> SearchingForChairStarted;

        public int Id { get;}

        public async Task LookAtBarber(Barber barber)
        {
            if (barber.BarberState == BarberState.Sleeping)
                await barber.AwakeBarber(this);

            await Task.Delay(_walkingTimeRange.GetDelay());

            if (SearchingForChairStarted != null)
                SearchingForChairStarted.Invoke(this,
                    new SearchingForChairStartedEventArgs(this + " is searching for empty chair..."));
        }


        public override string ToString()
        {
            return "Customer " + Id;
        }
    }
}
