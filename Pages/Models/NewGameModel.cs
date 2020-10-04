using Imperit.State;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Imperit.Pages
{
	public class NewGameModel
	{
		[Range(0.0, double.MaxValue, ErrorMessage = "Z�porn� �rok je okr�d�n� v��itele")]
		public double Interest { get; set; }
		[Range(0.0, 1.0, ErrorMessage = "Pravd�podobnost je ��slo od 0 do 1")]
		public double DefaultInstability { get; set; }
		[Range(0, int.MaxValue, ErrorMessage = "�lov�k se nem��e narodit zadlu�en�")]
		public int DefaultMoney { get; set; }
		[Range(0, int.MaxValue, ErrorMessage = "Z�porn� dluhov� limit nen� splniteln�")]
		public int DebtLimit { get; set; }
		public bool SingleClient { get; set; }
		public string OldPassword { get; set; } = "";
		public string NewPassword { get; set; } = "";
		const string words = @"(\p{Z}*[\p{L}\p{N}]+\p{Z}*)+";
		[RegularExpression(words + "(," + words + ")*", ErrorMessage = "Napi� pros�m pouze jm�na robot� odd�len� ��rkou")]
		public string RobotNames { get; set; } = "";
		[Range(0, int.MaxValue, ErrorMessage = "Z�porn� po�et robot� nen� mo�n�")]
		public int MaxRobotCount { get; set; }
		public void Init(Settings old)
		{
			Interest = old.Interest;
			DefaultInstability = old.DefaultInstability;
			DefaultMoney = old.DefaultMoney;
			DebtLimit = old.DebtLimit;
			SingleClient = old.SingleClient;
			RobotNames = string.Join(", ", old.RobotNames);
			MaxRobotCount = old.MaxRobotCount;
		}
		public Settings GetSettings(Settings old)
		{
			return new Settings(false, SingleClient, Interest, DefaultInstability, DefaultMoney, DebtLimit, RobotNames.Split(',').Select(name => name.Trim()).ToImmutableArray(), MaxRobotCount, old.SeaColor, old.LandColor, old.MountainsColor, old.MountainsWidth, old.SoldierTypes, new Password(NewPassword));
		}
	}
}
