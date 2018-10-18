using System;

namespace app
{
    class Program
    {
        static void Main(string[] args)
        {
            while(true){
                Console.WriteLine("Enter MemberID");

                // Lets grab our memberID from our user
                var userInput = Console.ReadLine();

                // We need to parse our users input and verify we are dealing with a number
                int memberID;
                if(Int32.TryParse(userInput, out memberID)){

                    var exporter = new afreeland.export.Diagnosis();
                    exporter.Fetch(memberID);

                }else{
                    // Let our users know to enter valid data
                    Console.WriteLine($"You entered {userInput} which is not a valid integer");
                }
            }

        }
    }
}
