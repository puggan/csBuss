using Sexes = Bussen.Sex;

namespace Bussen
{
    public class Passenger
    {
        private int age;
        private Sex sex;

        public Passenger(Sex sex, int age)
        {
            this.sex = sex;
            this.age = age;
        }

        public int Age() => age;
        public Sex Sex() => sex;
        
        public bool isMale()
        {
            return sex == Sexes.Male;
        }

        public bool isFemale()
        {
            return sex == Sexes.Female;
        }

        public string pokeReaction()
        {
            switch (sex)
            {
                case Sexes.Male:
                    return age < 5 ? "Gråter" : "Grymtar lätt";
                case Sexes.Female:
                    return age < 3 ? "Gråter" : "Skriker högt";
                case Sexes.Other:
                    return age < 15 ? "Kramar dig" : "Kastar anklagelser omkring sig";
                default:
                    return "Kryper ihop och ökar volymen på sin musikspelare";
            }
        }
    }
}