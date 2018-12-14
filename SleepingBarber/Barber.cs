using System;
using System.Threading.Tasks;

namespace SleepingBarberAsync
{
    public enum BarberState
    {
        Sleeping,
        Walking,
        Barbering
    }

    #region Events

    public class InvititationBroadcastedEventArgs : EventArgs
    {
        public InvititationBroadcastedEventArgs(string message)
        {
            Message = message;
        }

        public string Message { get; private set; }
    }

    public class BarberingFinishedEventArgs : EventArgs
    {
        public BarberingFinishedEventArgs(string message)
        {
            Message = message;
        }

        public string Message { get; private set; }
    }

    public class FellAsleepEventArgs : EventArgs
    {
        public FellAsleepEventArgs(string message)
        {
            Message = message;
        }

        public string Message { get; private set; }
    }

    public class HasAwakenEventArgs : EventArgs
    {
        public HasAwakenEventArgs(string message)
        {
            Message = message;
        }

        public string Message { get; private set; }
    }

    #endregion

    public class Barber
    {
        private readonly int _id; // for multiple barbers
        private readonly DelayRange _walkingTimeRange;
        private readonly DelayRange _barberingTimeRange;

        public Barber(int id, DelayRange walkingTimeRange, DelayRange barberingTimeRange)
        {
            _id = id;
            _walkingTimeRange = walkingTimeRange;
            _barberingTimeRange = barberingTimeRange;
            BarberState = BarberState.Sleeping;
        }

        public BarberState BarberState { get; set; }

        public event EventHandler<InvititationBroadcastedEventArgs> InvititationBroadcasted;
        public event EventHandler<BarberingFinishedEventArgs> BarberingFinished;
        public event EventHandler<FellAsleepEventArgs> FellAsleep;
        public event EventHandler<HasAwakenEventArgs> HasAwaken;


        public async Task WalkToRoom()
        {
            BarberState = BarberState.Walking;

            await Task.Delay(_walkingTimeRange.GetDelay());
            if (InvititationBroadcasted != null)
                InvititationBroadcasted.Invoke(this, new InvititationBroadcastedEventArgs("Barber is inviting a customer"));
        }

        public async Task Sleep()
        {
            BarberState = BarberState.Walking;

            await Task.Delay(_walkingTimeRange.GetDelay());

            BarberState = BarberState.Sleeping;

            if (FellAsleep != null)
                FellAsleep.Invoke(this, new FellAsleepEventArgs("Barber is sleeping... Zzz.."));
        }

        public async Task PerformBarbering(Customer customer)
        {
            BarberState = BarberState.Barbering;

            await Task.Delay(_barberingTimeRange.GetDelay());
            if (BarberingFinished != null)
                BarberingFinished.Invoke(this,
                    new BarberingFinishedEventArgs("Barber has finished with " + customer));

            await WalkToRoom();
        }

        public async Task AwakeBarber(Customer customer)
        {
            if (BarberState != BarberState.Sleeping)
                return;

            if (HasAwaken != null)
                HasAwaken.Invoke(this,
                    new HasAwakenEventArgs("Barber has awaken! He is cutting " + customer + "'s hair"));

            await PerformBarbering(customer);
        }
    }
}