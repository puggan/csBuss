namespace Bussen
{
    public class Buss
    {
        private int passengerCount = 0;
        private int passengerMaxCount;
        private Passenger[] passengers;

        public Buss(int passengerLimit)
        {
            passengerMaxCount = passengerLimit;
            passengers = new Passenger[passengerMaxCount];
        }

        public int PassengerCount() => passengerCount;
        public int PassengerMaxCount() => passengerMaxCount;

        public void AddPassenger(Passenger passenger) => passengers[passengerCount++] = passenger;

        public System.Collections.Generic.IEnumerable<Passenger> Passengers() => new System.ArraySegment<Passenger>(passengers, 0, passengerCount);

        public Passenger Passenger(int index) => passengers[index];
    }
}