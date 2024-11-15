using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ElevatorConsole
{
	internal class Program
	{
		// Our Direction Enum
		public enum Direction
		{
			GoingUp,
			GoingDown,
			Stationary
		}
		// Elevator Base Class
		public abstract class Elevator
		{
			  public int Id { get;}
			  public int CurrentFloor { get; protected set;}
			  public Direction direction { get; protected set;}
			  public int Passengers { get; protected set;}
			  public int LiftCapacity { get; protected set;}

			public Elevator(int id, int capacity)
			{
							Id = id;
							CurrentFloor = 0;
							LiftCapacity = capacity;
							direction = Direction.Stationary;
			}

			public abstract void GoingToFloor(int floor);
			public bool MaximumNumberOfPassengers(int passengers) => (Passengers + passengers) <= LiftCapacity;

			public void LoadPassengers(int passengers)
			{
				if (MaximumNumberOfPassengers(passengers))
							Passengers += passengers;
				else
					Console.WriteLine("Elevator load capacity has been Reached");
			}
			//Offload Passangers or minus passangers to be withing the load capacity
			public void OffLoadPassengers(int passengers)
			{
				Passengers = Math.Max(0, Passengers - passengers);
			}
		}

		// Passenger Elevator Class
		public class PassengerElevator : Elevator
		{
			public PassengerElevator(int id) : base(id, capacity: 12) { }

			public override void GoingToFloor(int floor)
			{
				if (floor == CurrentFloor)
				{
					Console.WriteLine($"Elevator {Id} is on the Requested Floor {floor}.");
					return;
				}

				direction = floor > CurrentFloor ? Direction.GoingUp : Direction.GoingDown;
				Console.WriteLine($"Elevator {Id} is Going to {direction} to floor {floor}.");

				// Show movement between floors 
				while (CurrentFloor != floor)
				{
					CurrentFloor += direction == Direction.GoingUp ? 1 : -1;
					Console.WriteLine($"Elevator {Id} is now on floor {CurrentFloor}");
					Thread.Sleep(500); 
				}

				// Show when reached destination floor
				Console.WriteLine($"Elevator {Id} Has Arrived at floor {floor}");
				direction = Direction.Stationary;
			}
		}

		// Central Controller Class
		public class CentralController
		{
			private List<Elevator> Elevators;
			private List<string> RequestHistory = new List<string>();

			public CentralController(int numberOfElevators)
			{
				Elevators = new List<Elevator>();

				for (int i = 0; i < numberOfElevators; i++)
								Elevators.Add(new PassengerElevator(i));
			}



			// Assign elevator to floor
		public void RequestElevator(int floorNumber, int passengers)
			{
				var nearestElevator = Elevators
					.Where(e => e.MaximumNumberOfPassengers(passengers)).OrderBy(e => Math.Abs(e.CurrentFloor - floorNumber)).FirstOrDefault();

				if (nearestElevator != null)
				{
					var requestInfo = $"Floor: {floorNumber}, Passengers: {passengers}, Elevator Dispatched: {nearestElevator.Id}";
					Console.WriteLine($"Dispatching Elevator {nearestElevator.Id} to floor {floorNumber}");
					nearestElevator.GoingToFloor(floorNumber);
					nearestElevator.LoadPassengers(passengers);
					RequestHistory.Add(requestInfo);
				}
				else
				{
					Console.WriteLine("No available elevators to action this request.");
					RequestHistory.Add($"Floor: {floorNumber}, Passengers: {passengers}, Status: Request failed");
				}
			}

			// Show elevatoir status when its moving 
			public void ElevatorStatus()
			{
				Console.Clear();
				Console.WriteLine("Elevator Status:");
				foreach (var elevator in Elevators)
				{
					Console.WriteLine($"Elevator {elevator.Id}: Floor {elevator.CurrentFloor}, " +
						$"Direction {elevator.direction}, Passengers {elevator.Passengers}");
				}
			}
		}

		static void Main(string[] args)
		{
			var controller = new CentralController(numberOfElevators: 4);

			while (true)
			{
				// As the user to ender the floor the are requesting the lift to and also add the number of passnagers that will bpoard the lift
				Console.Write("Enter the floor number (1-10) to request the elevator: ");
				if (int.TryParse(Console.ReadLine(), out int floor) && floor >= 1 && floor <= 10)
				{
					Console.Write("Enter the number of passengers: ");
					if (int.TryParse(Console.ReadLine(), out int passengers) && passengers > 0)
					{
						// User input to request elevator to Floor by typing florr number and also the number of passangers that will board the lift
						controller.RequestElevator(floor, passengers);
					}
					else
					{
						Console.WriteLine("Invalid number of passengers. Please enter a positive number.");
					}
				}
				else
				{
					Console.WriteLine("Invalid floor number. Please enter a number between 1 and 10.");
				}

				Console.WriteLine("\nOverall Elevator Status:");
				controller.ElevatorStatus();
				// Displays status in Realtime as it reaches its destination
				Thread.Sleep(1000);
			}
		}
	}
}