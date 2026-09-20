using Microsoft.Data.Sqlite;
using SCS_Mod_Helper.Utils;
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;
using System.Windows;

namespace SCS_Mod_Helper.Trucks;

public static class TrucksIO {
	//数据库有版本号，例如1061260920，前四位是版本号对应游戏版本1.61，0是为了避免游戏子版本破百，后六位是日期
	private const string defaultDB = "SCS_Mod_Helper.Trucks.DefaultTrucks.db";

	private static string ConnString => "Data Source=" + Paths.TrucksDBPath();
	private const string TABLE_TRUCKS_ETS2 = "TrucksETS2";
	private const string TABLE_TRUCKS_ATS = "TrucksATS";
	private const string TABLE_CABINS_ETS2 = "CabinsETS2";
	private const string TABLE_CABINS_ATS = "CabinsATS";
	private const string TABLE_ACC_ETS2 = "AccessoriesETS2";
	private const string TABLE_ACC_ATS = "AccessoriesATS";


	private static ResourceDictionary? mTruckDictionary = null;
	public static ResourceDictionary TruckDictionary {
		get {
			if (mTruckDictionary == null) {
				mTruckDictionary = [];
				var lang = Instances.CurrentLanguage;
				Uri source;
				if (lang.StartsWith(DictionaryUtil.ExtLangPreffix)) {
					lang = lang[DictionaryUtil.ExtLangPreffix.Length..];
					var path = Path.Combine(Paths.TrucksLanguageDir(), $"{lang}.xaml");
					if (File.Exists(path))
						source = new Uri(path);
					else
						source = new Uri($"pack://application:,,,/Language/Trucks/en-US.xaml", UriKind.Absolute);
				} else {
					source = new Uri($"pack://application:,,,/Language/Trucks/{lang}.xaml", UriKind.Absolute);
				}
				mTruckDictionary.Source = source;
			}
			return mTruckDictionary;
		}
	}
	public static void ClearTruckDict() => mTruckDictionary = null;

	public static int GetProdYear(string truckID) => (int)(TruckDictionary[$"prodYear.{truckID}"] ?? 0);
	public static string GetTruckName(string truckID) => (string)(TruckDictionary[$"name.{truckID}"] ?? truckID);
	public static string GetTruckDesc(string truckID) => (string)(TruckDictionary[$"desc.{truckID}"] ?? "");

	public static string GetCabinName(string cabinID) => (string)(TruckDictionary[cabinID] ?? cabinID);
	public static string GetAccessoryName(string truckID, string accID) => (string)(TruckDictionary[$"{truckID}.{accID}"] ?? accID);

	private static void CheckDBExistence() {
		string dbPath = Paths.TrucksDBPath();
		if (!File.Exists(dbPath)) {
			var assembly = Assembly.GetExecutingAssembly();
			using Stream? stream = assembly.GetManifestResourceStream(defaultDB) ?? throw new FileNotFoundException("Default not found");
			using FileStream fs = new(dbPath, FileMode.Create);
			stream.CopyTo(fs);
		}
	}

	public static void AddTruck(Truck truck) {
		bool isETS2 = truck.IsETS2;
		string table = isETS2 ? TABLE_TRUCKS_ETS2 : TABLE_TRUCKS_ATS;
		using var conn = new SqliteConnection(ConnString);
		conn.Open();
		var cmd = conn.CreateCommand();
		cmd.CommandText = @$"Insert into {table} (TruckID, Manifaturer, ProductionYear, TruckName, Description)
							values($id, $mani, $prodYear, $tname, $desc)";
		cmd.Parameters.AddWithValue("$id", truck.TruckID);
		cmd.Parameters.AddWithValue("$mani", truck.Manifaturer);
		cmd.Parameters.AddWithValue("$prodYear", truck.ProductionYear);
		cmd.Parameters.AddWithValue("$tname", truck.IngameName);
		cmd.Parameters.AddWithValue("$desc", truck.Description);
		cmd.ExecuteNonQuery();
	}

	public static void DeleteTruck(Truck truck) {
		bool isETS2 = truck.IsETS2;
		string table = isETS2 ? TABLE_TRUCKS_ETS2 : TABLE_TRUCKS_ATS;
		using var conn = new SqliteConnection(ConnString);
		conn.Open();
		var cmd = conn.CreateCommand();
		cmd.CommandText = $"Delete from {table} where TruckID = $id";
		cmd.Parameters.AddWithValue("$id", truck.TruckID);
		cmd.ExecuteNonQuery();
	}

	public static void DeleteTrucks(bool isETS2, List<string> truckIDs) {
		string table = isETS2 ? TABLE_TRUCKS_ETS2 : TABLE_TRUCKS_ATS;
		string ins = string.Join(", ", truckIDs.Select(_ => "$id"));
		using var conn = new SqliteConnection(ConnString);
		conn.Open();
		using var transaction = conn.BeginTransaction();
		try {
			var cmd = conn.CreateCommand();
			cmd.CommandText = $"Delete from {table} where TruckID in ({ins})";
			foreach (var id in truckIDs)
				cmd.Parameters.AddWithValue("$id", id);
			cmd.ExecuteNonQuery();
			transaction.Commit();
		} catch {
			transaction.Rollback();
			throw;
		}
	}

	public static void EditTruck(Truck truck) {
		bool isETS2 = truck.IsETS2;
		string table = isETS2 ? TABLE_TRUCKS_ETS2 : TABLE_TRUCKS_ATS;
		using var conn = new SqliteConnection(ConnString);
		conn.Open();
		var cmd = conn.CreateCommand();
		cmd.CommandText = @$"update {table} 
							set Manifaturer = $mani, ProductionYear = $prodYear, TruckName = $tname, Description = $desc, DefaultTruck = $default 
							where TruckID = $id";
		cmd.Parameters.AddWithValue("$id", truck.TruckID);
		cmd.Parameters.AddWithValue("$mani", truck.Manifaturer);
		cmd.Parameters.AddWithValue("$prodYear", truck.ProductionYear);
		cmd.Parameters.AddWithValue("$tname", truck.IngameName);
		cmd.Parameters.AddWithValue("$desc", truck.Description);
		cmd.Parameters.AddWithValue("$default", 0);
		cmd.ExecuteNonQuery();
	}

	public static async Task LoadTrucks(bool isETS2, ObservableCollection<Truck> trucks) {
		CheckDBExistence();
		string table = isETS2 ? TABLE_TRUCKS_ETS2 : TABLE_TRUCKS_ATS;
		using var conn = new SqliteConnection(ConnString);
		conn.Open();
		var cmd = conn.CreateCommand();
		cmd.CommandText = $"Select *  from {table} order by Manifaturer, ProductionYear, TruckID";
		using var reader = cmd.ExecuteReader();
		List<Truck> loadedTruck = [];
		while (reader.Read()) {
			string truckID = reader.GetString(0);
			string manifaturer = reader.GetString(1);
			int prodYear = reader.GetInt32(2);
			string? truckName;
			if (reader.IsDBNull(3)) {
				truckName = GetTruckName(truckID);
			} else
				truckName = reader.GetString(3);
			string? desc;
			if (reader.IsDBNull(4)) {
				desc = GetTruckDesc(truckID);
			} else
				desc = reader.GetString(4);
			Truck truck = new(isETS2, truckID, manifaturer, prodYear, truckName, desc);
			loadedTruck.Add(truck);
		}
		await Application.Current.Dispatcher.InvokeAsync(() => {
			foreach (var t in loadedTruck) {
				trucks.Add(t);
			}
		});
	}

	public static void ClearTruckList() {
		var dbPath = Paths.TrucksDBPath();
		File.Delete(dbPath);
	}

	public static void AddCabin(bool isETS2, Cabin cabin) {
		string table = isETS2 ? TABLE_CABINS_ETS2 : TABLE_CABINS_ATS;
		using var conn = new SqliteConnection(ConnString);
		conn.Open();
		var cmd = conn.CreateCommand();
		cmd.CommandText = @$"Insert into {table} (TruckID, CabinID, CabinName)
							values($tID, $cID, $cName)";
		cmd.Parameters.AddWithValue("$tID", cabin.TruckID);
		cmd.Parameters.AddWithValue("$cID", cabin.CabinID);
		cmd.Parameters.AddWithValue("$cName", cabin.CabinName);
		cmd.ExecuteNonQuery();
	}

	public static void DeleteCabin(bool isETS2, Cabin cabin) {
		string table = isETS2 ? TABLE_CABINS_ETS2 : TABLE_CABINS_ATS;
		using var conn = new SqliteConnection(ConnString);
		conn.Open();
		var cmd = conn.CreateCommand();
		cmd.CommandText = $"Delete from {table} where TruckID = $tID, CabinID = $cID";
		cmd.Parameters.AddWithValue("$tID", cabin.TruckID);
		cmd.Parameters.AddWithValue("$cID", cabin.CabinID);
		cmd.ExecuteNonQuery();
	}

	public static void EditCabin(bool isETS2, Cabin cabin) {
		string table = isETS2 ? TABLE_CABINS_ETS2 : TABLE_CABINS_ATS;
		using var conn = new SqliteConnection(ConnString);
		conn.Open();
		var cmd = conn.CreateCommand();
		cmd.CommandText = @$"update {table} 
							set CabinName = $cName, DefaultCabin = $default 
							where TruckID = $tID, CabinID = $cID";
		cmd.Parameters.AddWithValue("$cName", cabin.CabinName);
		cmd.Parameters.AddWithValue("$default", 0);
		cmd.Parameters.AddWithValue("$tID", cabin.TruckID);
		cmd.Parameters.AddWithValue("$cID", cabin.CabinID);
		cmd.ExecuteNonQuery();
	}

	public static void LoadCabins(bool isETS2, string truckID, Collection<Cabin> cabins) {
		CheckDBExistence();
		string table = isETS2 ? TABLE_CABINS_ETS2 : TABLE_CABINS_ATS;
		using var conn = new SqliteConnection(ConnString);
		conn.Open();
		var cmd = conn.CreateCommand();
		cmd.CommandText = $"select * from {table} where TruckID = $id";
		cmd.Parameters.AddWithValue("$id", truckID);
		using var reader = cmd.ExecuteReader();
		while (reader.Read()) {
			string cabinID = reader.GetString(1);
			string cabinName;
			if (reader.IsDBNull(2)) {
				cabinName = GetCabinName(cabinID);
			} else
				cabinName = reader.GetString(2);
			Cabin cabin = new(truckID, cabinID, cabinName);
			cabins.Add(cabin);
		}
	}

	public static void AddAccessory(bool isETS2, Accessory accessory) {
		string table = isETS2 ? TABLE_ACC_ETS2 : TABLE_ACC_ATS;
		using var conn = new SqliteConnection(ConnString);
		conn.Open();
		var cmd = conn.CreateCommand();
		cmd.CommandText = @$"Insert into {table} (TruckID, AccessoryID, AccessoryName)
							values($tID, $cID, $cName)";
		cmd.Parameters.AddWithValue("$tID", accessory.TruckID);
		cmd.Parameters.AddWithValue("$cID", accessory.AccID);
		cmd.Parameters.AddWithValue("$cName", accessory.AccName);
		cmd.ExecuteNonQuery();
	}

	public static void DeleteAccessory(bool isETS2, Accessory accessory) {
		string table = isETS2 ? TABLE_ACC_ETS2 : TABLE_ACC_ATS;
		using var conn = new SqliteConnection(ConnString);
		conn.Open();
		var cmd = conn.CreateCommand();
		cmd.CommandText = $"Delete from {table} where TruckID = $tID, AccessoryID = $aID";
		cmd.Parameters.AddWithValue("$tID", accessory.TruckID);
		cmd.Parameters.AddWithValue("$aID", accessory.AccID);
		cmd.ExecuteNonQuery();
	}

	public static void EditAccessory(bool isETS2, Accessory accessory) {
		string table = isETS2 ? TABLE_ACC_ETS2 : TABLE_ACC_ATS;
		using var conn = new SqliteConnection(ConnString);
		conn.Open();
		var cmd = conn.CreateCommand();
		cmd.CommandText = @$"update {table} 
							set AccessoryName = $aName, DefaultAccessory = $default 
							where TruckID = $tID, AccessoryID = $aID";
		cmd.Parameters.AddWithValue("$aName", accessory.AccName);
		cmd.Parameters.AddWithValue("$default", 0);
		cmd.Parameters.AddWithValue("$tID", accessory.TruckID);
		cmd.Parameters.AddWithValue("$aID", accessory.AccID);
		cmd.ExecuteNonQuery();
	}

	public static void LoadAccessories(bool isETS2, string truckID, Collection<Accessory> accessories) {
		CheckDBExistence();
		string table = isETS2 ? TABLE_ACC_ETS2 : TABLE_ACC_ATS;
		using var conn = new SqliteConnection(ConnString);
		conn.Open();
		var cmd = conn.CreateCommand();
		cmd.CommandText = $"select * from {table} where TruckID = $id";
		cmd.Parameters.AddWithValue("$id", truckID);
		using var reader = cmd.ExecuteReader();
		while (reader.Read()) {
			string accID = reader.GetString(1);
			string accName;
			if (reader.IsDBNull(2)) {
				accName = GetAccessoryName(truckID, accID);
			} else
				accName = reader.GetString(2);
			Accessory accessory = new(truckID, accID, accName);
			accessories.Add(accessory);
		}
	}
}
