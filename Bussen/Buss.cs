using PassengersSlice = System.ArraySegment<Bussen.Passenger>;

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

        public PassengersSlice Passengers() => new (passengers, 0, passengerCount);

        public Passenger Passenger(int index) => passengers[index];
    }
}