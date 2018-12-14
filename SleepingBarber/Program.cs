using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace SleepingBarberAsync
{
    class Program
    {
        private static readonly ConcurrentQueue<Customer> WaitingCustomers = new ConcurrentQueue<Customer>();
        private static readonly int MaxWaiters = 3;

        static void Main(string[] args)
        {
            var walkingTimeRange = new DelayRange(20, 300);
            // 600-2000 for barbering time and 100-2900 allows to receive all possible events
            var barberingTimeRange = new DelayRange(600, 2000);
            var newCustomerTimeRange = new DelayRange(100, 2900);

            var barber = new Barber(1, walkingTimeRange, barberingTimeRange);

            barber.FellAsleep += _barberFellAsleep;
            barber.BarberingFinished += _barberHasFinishedBarbering;
            barber.HasAwaken += _barberHasAwaken;
            barber.InvititationBroadcasted += _barberHasBroadcastedInvititation;

            Task.Run(async () => await barber.Sleep());

            Task.Run(async () => await _generateCustomers(barber, newCustomerTimeRange, walkingTimeRange));

            Console.ReadKey();
        }
        #region Barber
        private static void _barberHasBroadcastedInvititation(object sender, InvititationBroadcastedEventArgs e)
        {
            Console.WriteLine(e.Message);
            var barber = (Barber) sender;

            if (WaitingCustomers.TryDequeue(out var customer))
                Task.Run(async () => await barber.PerformBarbering(customer));
            else
                Task.Run(async () => await barber.Sleep());
        }

        private static void _barberHasAwaken(object sender, HasAwakenEventArgs e)
        {
            Console.WriteLine(e.Message);
        }

        private static void _barberHasFinishedBarbering(object sender, BarberingFinishedEventArgs e)
        {
            Console.WriteLine(e.Message);
        }

        private static void _barberFellAsleep(object sender, FellAsleepEventArgs e)
        {
            Console.WriteLine(e.Message);
        }

        #endregion

        #region Customers

        private static async Task _generateCustomers(Barber barber, DelayRange newCustomerTimeRange, DelayRange walkingTimeRange)
        {
            var customerId = 1;

            while (true)
            {
                await Task.Delay(newCustomerTimeRange.GetDelay());

                var customer = new Customer(customerId, walkingTimeRange);
                customer.SearchingForChairStarted += _customerIsSearchingForChairStarted;
                customerId++;

                await customer.LookAtBarber(barber);
            }
        }

        private static void _customerIsSearchingForChairStarted(object sender, SearchingForChairStartedEventArgs e)
        {
            Console.WriteLine(e.Message);
            var customer = (Customer)sender;

            if (WaitingCustomers.Count >= MaxWaiters)
                Console.WriteLine("ALL SEATS ARE BUSY! "+customer + " has left barber shop");
            else
                WaitingCustomers.Enqueue(customer);
        }

        #endregion
    }
}
