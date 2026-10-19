using Microsoft.Data.Sqlite;
using SCS_Mod_Helper.Modding.Accessories;
using SCS_Mod_Helper.Utils;
using System.Collections.ObjectModel;
using System.IO;
using System.Net.WebSockets;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Windows;

namespace SCS_Mod_Helper.Vehicles;

public static class VehiclesIO {
	//数据库有版本号，例如1061260920，前四位是版本号对应游戏版本1.61，0是为了避免游戏子版本破百，后六位是日期
	private const string defaultDB = "SCS_Mod_Helper.Vehicles.DefaultVehicles.db";
	//private const int dbVersion = 1061261017;

	private static string EmbDBSource => "Data Source=" + Paths.EmbDBFile();
	private static string CustomDBSource => "Data Source=" + Paths.CustomDBPath();
	private static string VehicleDBSource => "Data Source=" + Paths.VehiclesDBPath();

	private const string TABLE_VEHICLES_ETS2 = "VehiclesETS2";
	private const string TABLE_VEHICLES_ATS = "VehiclesATS";
	private const string COLUMN_VEHICLE_ID = "VehicleID";
	private const string COLUMN_VEHICLE_TYPE = "VehicleType";
	private const string COLUMN_MANIFATURER = "Manifaturer";
	private const string COLUMN_PROD_YEAR = "ProductionYear";
	private const string COLUMN_VEHICLE_NAME = "VehicleName";
	private const string COLUMN_DESCRIPTION = "Description";
	private const string COLUMN_DEFAULT_VEHICLE = "DefaultVehicle";
	private const string COLUMN_V_TYPE_CODE = "VTypeCode";


	private const string TABLE_CABINS_ETS2 = "CabinsETS2";
	private const string TABLE_CABINS_ATS = "CabinsATS";
	private const string COLUMN_CABIN_ID = "CabinID";
	private const string COLUMN_CABIN_NAME = "CabinName";
	private const string COLUMN_DEFAULT_CABIN = "DefaultCabin";

	private const string TABLE_ACC_ETS2 = "AccessoriesETS2";
	private const string TABLE_ACC_ATS = "AccessoriesATS";
	private const string COLUMN_ACC_ID = "AccessoryID";
	private const string COLUMN_ACC_NAME = "AccessoryName";
	private const string COLUMN_ACC_TYPE = "AccessoryType";
	private const string COLUMN_UNLOCK_LEVEL = "UnlockLevel";
	private const string COLUMN_DEFAULT_ACC = "DefaultAccessory";

	private const string TABLE_ACCESSORY_ORDER = "AccessoryOrder";
	private const string COLUMN_ORDER = "Order";

	private const string TABLE_EDITED = "TableEdited";
	private const string COLUMN_VEHICLES_ETS2 = "VehiclesETS2";
	private const string COLUMN_VEHICLES_ATS = "VehiclesATS";


	private static string TableNameVehicle(bool isETS2) => isETS2 ? TABLE_VEHICLES_ETS2 : TABLE_VEHICLES_ATS;
	private static string TableNameCabin(bool isETS2) => isETS2 ? TABLE_CABINS_ETS2 : TABLE_CABINS_ATS;
	private static string TableNameAcc(bool isETS2) => isETS2 ? TABLE_ACC_ETS2 : TABLE_ACC_ATS;

	private static string GetString(this SqliteDataReader reader, string columnName) => reader.GetString(reader.GetOrdinal(columnName));
	private static int GetInt(this SqliteDataReader reader, string columnName) => reader.GetInt32(reader.GetOrdinal(columnName));
	private static bool GetBool(this SqliteDataReader reader, string columnName) => reader.GetInt32(reader.GetOrdinal(columnName)) == 1;
	private static string TryGetString(this SqliteDataReader reader, string columnName, string defaultValue) {
		int ordinal = reader.GetOrdinal(columnName);
		if (reader.IsDBNull(ordinal))
			return defaultValue;
		else
			return reader.GetString(ordinal);
	}


	private static ResourceDictionary? mVehicleDictionary = null;
	public static ResourceDictionary VehicleDictionary {
		get {
			if (mVehicleDictionary == null) {
				mVehicleDictionary = [];
				var lang = Instances.CurrentLanguage;
				Uri source;
				if (lang.StartsWith(DictionaryUtil.ExtLangPreffix)) {
					lang = lang[DictionaryUtil.ExtLangPreffix.Length..];
					var path = Path.Combine(Paths.VehiclesLanguageDir(), $"{lang}.xaml");
					if (File.Exists(path))
						source = new Uri(path);
					else
						source = new Uri("pack://application:,,,/Vehicles/en-US.xaml", UriKind.Absolute);
				} else {
					source = new Uri($"pack://application:,,,/Vehicles/{lang}.xaml", UriKind.Absolute);
				}
				mVehicleDictionary.Source = source;
			}
			return mVehicleDictionary;
		}
	}
	public static void ClearVehicleDict() => mVehicleDictionary = null;

	public static int GetProdYear(string vehicleID) => (int)(VehicleDictionary[$"prodYear.{vehicleID}"] ?? 0);
	public static string GetVehicleName(string vehicleID) => (string)(VehicleDictionary[$"name.{vehicleID}"] ?? vehicleID);
	public static string GetVehicleDesc(string vehicleID) => (string)(VehicleDictionary[$"desc.{vehicleID}"] ?? "");

	public static string GetCabinName(string cabinID) => (string)(VehicleDictionary[cabinID] ?? cabinID);
	public static string GetAccessoryName(string vehicleID, string accID) => (string)(VehicleDictionary[$"{vehicleID}.{accID}"] ?? accID);

	public static void AddVehicle(Vehicle vehicle) {
		bool isETS2 = vehicle.IsETS2;
		string table = TableNameVehicle(isETS2);
		using var conn = new SqliteConnection(VehicleDBSource);
		conn.Open();
		var cmd = conn.CreateCommand();
		cmd.CommandText = @$"Insert into {table} ({COLUMN_VEHICLE_ID}, {COLUMN_VEHICLE_TYPE}, {COLUMN_MANIFATURER}, {COLUMN_PROD_YEAR}, {COLUMN_VEHICLE_NAME}, {COLUMN_DESCRIPTION})
							values($id, $type, $mani, $prodYear, $vname, $desc)";
		cmd.Parameters.AddWithValue("$id", vehicle.VehicleID);
		cmd.Parameters.AddWithValue("$type", vehicle.VehicleType);
		cmd.Parameters.AddWithValue("$mani", vehicle.Manifaturer);
		cmd.Parameters.AddWithValue("$prodYear", vehicle.ProductionYear);
		cmd.Parameters.AddWithValue("$vname", vehicle.VehicleName);
		cmd.Parameters.AddWithValue("$desc", vehicle.Description);
		cmd.ExecuteNonQuery();
	}

	public static void DeleteVehicle(Vehicle vehicle) {
		bool isETS2 = vehicle.IsETS2;
		string table = TableNameVehicle(isETS2);
		using var conn = new SqliteConnection(VehicleDBSource);
		conn.Open();
		var cmd = conn.CreateCommand();
		cmd.CommandText = $"Delete from {table} where {COLUMN_VEHICLE_ID} = $id";
		cmd.Parameters.AddWithValue("$id", vehicle.VehicleID);
		cmd.ExecuteNonQuery();
	}

	public static void DeleteVehicle(bool isETS2, List<string> vehicleIDs) {
		string table = TableNameVehicle(isETS2);
		string ins = string.Join(", ", vehicleIDs.Select(_ => "$id"));
		using var conn = new SqliteConnection(VehicleDBSource);
		conn.Open();
		using var transaction = conn.BeginTransaction();
		try {
			var cmd = conn.CreateCommand();
			cmd.CommandText = $"Delete from {table} where {COLUMN_VEHICLE_ID} in ({ins})";
			foreach (var id in vehicleIDs)
				cmd.Parameters.AddWithValue("$id", id);
			cmd.ExecuteNonQuery();
			transaction.Commit();
		} catch {
			transaction.Rollback();
			throw;
		}
	}

	public static void EditVehicle(Vehicle vehicle) {
		bool isETS2 = vehicle.IsETS2;
		string table = TableNameVehicle(isETS2);
		using var conn = new SqliteConnection(VehicleDBSource);
		conn.Open();
		var cmd = conn.CreateCommand();
		cmd.CommandText = @$"update {table} 
							set {COLUMN_VEHICLE_TYPE} = $type, {COLUMN_MANIFATURER} = $mani, {COLUMN_PROD_YEAR} = $prodYear, {COLUMN_VEHICLE_NAME} = $vname, {COLUMN_DESCRIPTION} = $desc, {COLUMN_DEFAULT_VEHICLE} = $default 
							where {COLUMN_VEHICLE_ID} = $id";
		cmd.Parameters.AddWithValue("$id", vehicle.VehicleID);
		cmd.Parameters.AddWithValue("$type", vehicle.VehicleType);
		cmd.Parameters.AddWithValue("$mani", vehicle.Manifaturer);
		cmd.Parameters.AddWithValue("$prodYear", vehicle.ProductionYear);
		cmd.Parameters.AddWithValue("$vname", vehicle.VehicleName);
		cmd.Parameters.AddWithValue("$desc", vehicle.Description);
		cmd.Parameters.AddWithValue("$default", 0);
		cmd.ExecuteNonQuery();
	}

	public static async Task LoadAccessoryVehicle(bool isETS2, ObservableCollection<AccessoryVehicle> vehicles) {
		DBCheckBeforeUse();
		string table = TableNameVehicle(isETS2);
		using var conn = new SqliteConnection(VehicleDBSource);
		conn.Open();
		var cmd = conn.CreateCommand();
		cmd.CommandText = $"Select *  from {table} order by {COLUMN_VEHICLE_TYPE}, {COLUMN_MANIFATURER}, {COLUMN_PROD_YEAR}, {COLUMN_VEHICLE_ID}";
		using var reader = cmd.ExecuteReader();
		List<AccessoryVehicle> loadedVehicles = [];
		while (reader.Read()) {
			string vehicleID = reader.GetString(COLUMN_VEHICLE_ID);
			string vehicleType = reader.GetString(COLUMN_VEHICLE_TYPE);
			string manifaturer = reader.GetString(COLUMN_MANIFATURER);
			int prodYear = reader.GetInt(COLUMN_PROD_YEAR);
			string vehicleName = reader.TryGetString(COLUMN_VEHICLE_NAME, GetVehicleName(vehicleID));
			string desc = reader.TryGetString(COLUMN_DESCRIPTION, GetVehicleDesc(vehicleID));
			AccessoryVehicle vehicle = new(vehicleID, prodYear, vehicleName, desc, vehicleType, manifaturer, isETS2);
			loadedVehicles.Add(vehicle);
		}
		await Application.Current.Dispatcher.InvokeAsync(() => {
			foreach (var v in loadedVehicles) {
				vehicles.Add(v);
			}
		});
	}

	public static void LoadPaintJobVehicles(ObservableCollection<Vehicle> vehicles) {
		LoadPaintJobVehicles(true, vehicles);
		LoadPaintJobVehicles(false, vehicles);
	}

	private static void LoadPaintJobVehicles(bool isETS2, ObservableCollection<Vehicle> vehicles) {
		DBCheckBeforeUse();
		string table = TableNameVehicle(isETS2);
		using var conn = new SqliteConnection(VehicleDBSource);
		conn.Open();
		var cmd = conn.CreateCommand();

		var cabinDict = LoadCabins(cmd, isETS2);
		var accDict = LoadAccessories(cmd, isETS2);

		cmd.CommandText = $"Select *  from {table} order by {COLUMN_MANIFATURER}, {COLUMN_PROD_YEAR}, {COLUMN_VEHICLE_ID}";
		using var reader = cmd.ExecuteReader();
		List<Vehicle> loadedvehicle = [];
		while (reader.Read()) {
			string vehicleID = reader.GetString(COLUMN_VEHICLE_ID);
			string vehicleType = reader.GetString(COLUMN_VEHICLE_TYPE);
			string manifaturer = reader.GetString(COLUMN_MANIFATURER);
			int prodYear = reader.GetInt(COLUMN_PROD_YEAR);
			string? vehicleName = reader.TryGetString(COLUMN_VEHICLE_NAME, GetVehicleName(vehicleID));
			string? desc = reader.TryGetString(COLUMN_DESCRIPTION, GetVehicleDesc(vehicleID));
			Vehicle vehicle = new(vehicleID, prodYear, vehicleName, desc, vehicleType, manifaturer, isETS2);
			loadedvehicle.Add(vehicle);
			if (cabinDict.TryGetValue(vehicleID, out var cabins)) 
				vehicle.Cabins = cabins;
			if (accDict.TryGetValue(vehicleID, out var accessories)) 
				vehicle.Accessories = accessories;
		}
		foreach (var v in loadedvehicle) {
			vehicles.Add(v);
		}
	}

	public static Dictionary<string, List<Cabin>> LoadCabins(SqliteCommand cmd, bool isETS2, bool customVehicle = false) {
		DBCheckBeforeUse();
		string table = TableNameCabin(isETS2);
		StringBuilder command = new();
		command.Append($"Select * from {table}");
		if (customVehicle)
			command.Append($" where {COLUMN_DEFAULT_CABIN} = 0");
		command.Append($" order by {COLUMN_VEHICLE_ID}");
		cmd.CommandText = command.ToString();
		using var reader = cmd.ExecuteReader();
		Dictionary<string, List<Cabin>> dict = [];
		while(reader.Read()) {
			string vehicleID = reader.GetString(COLUMN_VEHICLE_ID);
			string cabinID = reader.GetString(COLUMN_CABIN_ID);
			string cabinName = reader.TryGetString(COLUMN_CABIN_NAME, GetCabinName(cabinID));
			bool defaultCabin = reader.GetBool(COLUMN_DEFAULT_CABIN);
			List<Cabin>? cabins;
			if (dict.TryGetValue(vehicleID, out List<Cabin>? value)) {
				cabins = value;
			} else {
				cabins = [];
				dict[vehicleID] = cabins;
			}
			cabins.Add(new(vehicleID, cabinID, cabinName, defaultCabin));
		}
		return dict;
	}
	public static Dictionary<string, List<Accessory>> LoadAccessories(SqliteCommand cmd, bool isETS2, bool customVehicles = false) {
		DBCheckBeforeUse();
		string table = TableNameAcc(isETS2);
		StringBuilder command = new();
		command.Append($"Select * from {table}");
		if (customVehicles)
			command.Append($" where {COLUMN_DEFAULT_ACC} = 0");
		command.Append($" order by {COLUMN_VEHICLE_ID}");
		cmd.CommandText = command.ToString();
		using var reader = cmd.ExecuteReader();
		Dictionary<string, List<Accessory>> dict = [];
		while (reader.Read()) {
			string vehicleID = reader.GetString(COLUMN_VEHICLE_ID);
			string accID = reader.GetString(COLUMN_ACC_ID);
			string accName = reader.TryGetString(COLUMN_ACC_NAME, GetAccessoryName(vehicleID, accID));
			string accType = reader.GetString(COLUMN_ACC_TYPE);
			int unlockLevel = reader.GetInt(COLUMN_UNLOCK_LEVEL);
			bool defaultAcc = reader.GetBool(COLUMN_DEFAULT_ACC);
			List<Accessory>? accessories;
			if (dict.TryGetValue(vehicleID, out List<Accessory>? value)) {
				accessories = value;
			} else {
				accessories = [];
				dict[vehicleID] = accessories;
			}

			accessories.Add(new(vehicleID, accID, accName, accType, unlockLevel, defaultAcc));
		}
		return dict;
	}

	public static void ClearVehicleList() {
		var dbPath = Paths.VehiclesDBPath();
		SqliteConnection.ClearAllPools();
		File.Delete(dbPath);
	}

	public static void AddCabin(bool isETS2, Cabin cabin) {
		string table = TableNameCabin(isETS2);
		using var conn = new SqliteConnection(VehicleDBSource);
		conn.Open();
		var cmd = conn.CreateCommand();
		cmd.CommandText = @$"Insert into {table} ({COLUMN_VEHICLE_ID}, {COLUMN_CABIN_ID}, {COLUMN_CABIN_NAME})
							values($vID, $cID, $cName)";
		cmd.Parameters.AddWithValue("$vID", cabin.VehicleID);
		cmd.Parameters.AddWithValue("$cID", cabin.CabinID);
		cmd.Parameters.AddWithValue("$cName", cabin.CabinName);
		cmd.ExecuteNonQuery();
	}

	public static void DeleteCabin(bool isETS2, Cabin cabin) {
		string table = TableNameCabin(isETS2);
		using var conn = new SqliteConnection(VehicleDBSource);
		conn.Open();
		var cmd = conn.CreateCommand();
		cmd.CommandText = $"Delete from {table} where {COLUMN_VEHICLE_ID} = $vID and {COLUMN_CABIN_ID} = $cID";
		cmd.Parameters.AddWithValue("$vID", cabin.VehicleID);
		cmd.Parameters.AddWithValue("$cID", cabin.CabinID);
		cmd.ExecuteNonQuery();
	}

	public static void EditCabin(bool isETS2, Cabin cabin) {
		string table = TableNameCabin(isETS2);
		using var conn = new SqliteConnection(VehicleDBSource);
		conn.Open();
		var cmd = conn.CreateCommand();
		cmd.CommandText = @$"update {table} 
							set {COLUMN_CABIN_NAME} = $cName, {COLUMN_DEFAULT_CABIN} = $default 
							where {COLUMN_VEHICLE_ID} = $vID and {COLUMN_CABIN_ID} = $cID";
		cmd.Parameters.AddWithValue("$cName", cabin.CabinName);
		cmd.Parameters.AddWithValue("$default", 0);
		cmd.Parameters.AddWithValue("$vID", cabin.VehicleID);
		cmd.Parameters.AddWithValue("$cID", cabin.CabinID);
		cmd.ExecuteNonQuery();
	}

	public static void LoadCabins(bool isETS2, string vehicleID, Collection<Cabin> cabins) {
		DBCheckBeforeUse();
		string table = TableNameCabin(isETS2);
		using var conn = new SqliteConnection(VehicleDBSource);
		conn.Open();
		var cmd = conn.CreateCommand();
		cmd.CommandText = $"select * from {table} where {COLUMN_VEHICLE_ID} = $id";
		cmd.Parameters.AddWithValue("$id", vehicleID);
		using var reader = cmd.ExecuteReader();
		while (reader.Read()) {
			string cabinID = reader.GetString(reader.GetOrdinal(COLUMN_CABIN_ID));
			string cabinName;
			if (reader.IsDBNull(reader.GetOrdinal(COLUMN_CABIN_NAME))) {
				cabinName = GetCabinName(cabinID);
			} else
				cabinName = reader.GetString(reader.GetOrdinal(COLUMN_CABIN_NAME));
			bool defaultCabin = reader.GetInt32(reader.GetOrdinal(COLUMN_DEFAULT_CABIN)) == 1;
			Cabin cabin = new(vehicleID, cabinID, cabinName, defaultCabin);
			cabins.Add(cabin);
		}
	}

	public static void AddAccessory(bool isETS2, Accessory accessory) {
		string table = TableNameAcc(isETS2);
		using var conn = new SqliteConnection(VehicleDBSource);
		conn.Open();
		var cmd = conn.CreateCommand();
		cmd.CommandText = @$"Insert into {table} ({COLUMN_VEHICLE_ID}, {COLUMN_ACC_ID}, {COLUMN_ACC_NAME}, {COLUMN_ACC_TYPE}, {COLUMN_UNLOCK_LEVEL})
							values($vID, $aID, $aName, $aType, $Level)";
		cmd.Parameters.AddWithValue("$vID", accessory.VehicleID);
		cmd.Parameters.AddWithValue("$aID", accessory.AccID);
		cmd.Parameters.AddWithValue("$aName", accessory.AccName);
		cmd.Parameters.AddWithValue("$aType", accessory.AccType);
		cmd.Parameters.AddWithValue("$Level", accessory.UnlockLevel);
		cmd.ExecuteNonQuery();
	}

	public static void DeleteAccessory(bool isETS2, Accessory accessory) {
		string table = TableNameAcc(isETS2);
		using var conn = new SqliteConnection(VehicleDBSource);
		conn.Open();
		var cmd = conn.CreateCommand();
		cmd.CommandText = $"Delete from {table} where {COLUMN_VEHICLE_ID} = $vID and {COLUMN_ACC_ID} = $aID";
		cmd.Parameters.AddWithValue("$vID", accessory.VehicleID);
		cmd.Parameters.AddWithValue("$aID", accessory.AccID);
		cmd.ExecuteNonQuery();
	}

	public static void EditAccessory(bool isETS2, Accessory accessory) {
		string table = TableNameAcc(isETS2);
		using var conn = new SqliteConnection(VehicleDBSource);
		conn.Open();
		var cmd = conn.CreateCommand();
		cmd.CommandText = @$"update {table} 
							set {COLUMN_ACC_NAME} = $aName, 
								{COLUMN_ACC_TYPE} = $aType, 
								{COLUMN_UNLOCK_LEVEL} = $Level, 
								{COLUMN_DEFAULT_ACC} = $default
							where {COLUMN_VEHICLE_ID} = $vID and {COLUMN_ACC_ID} = $aID";
		cmd.Parameters.AddWithValue("$aName", accessory.AccName);
		cmd.Parameters.AddWithValue("$aType", accessory.AccType);
		cmd.Parameters.AddWithValue("$Level", accessory.UnlockLevel);
		cmd.Parameters.AddWithValue("$default", 0);
		cmd.Parameters.AddWithValue("$vID", accessory.VehicleID);
		cmd.Parameters.AddWithValue("$aID", accessory.AccID);
		cmd.ExecuteNonQuery();
	}

	public static void LoadAccessories(bool isETS2, string vehicleID, Collection<Accessory> accessories) {
		DBCheckBeforeUse();
		string table = TableNameAcc(isETS2);
		using var conn = new SqliteConnection(VehicleDBSource);
		conn.Open();
		var cmd = conn.CreateCommand();
		cmd.CommandText = $"select * from {table} where {COLUMN_VEHICLE_ID} = $id";
		cmd.Parameters.AddWithValue("$id", vehicleID);
		using var reader = cmd.ExecuteReader();
		while (reader.Read()) {
			string accID = reader.GetString(COLUMN_ACC_ID);
			string accName = reader.TryGetString(COLUMN_ACC_NAME, GetAccessoryName(vehicleID, accID));
			string accType = reader.GetString(COLUMN_ACC_TYPE);
			int unlockLevel = reader.GetInt(COLUMN_UNLOCK_LEVEL);
			bool defaultAcc = reader.GetBool(COLUMN_DEFAULT_ACC);
			Accessory accessory = new(vehicleID, accID, accName, accType, unlockLevel, defaultAcc);
			accessories.Add(accessory);
		}
	}


	private static void DBCheckBeforeUse() {
		string dbPath = Paths.VehiclesDBPath();
		if (File.Exists(dbPath))
			return;
		if (File.Exists(Paths.CustomDBPath())) {
			File.Copy(Paths.CustomDBPath(), dbPath);
			return;
		}
		var assembly = Assembly.GetExecutingAssembly();
		using Stream? stream = assembly.GetManifestResourceStream(defaultDB) ?? throw new FileNotFoundException("Default not found");
		using FileStream fs = new(dbPath, FileMode.Create);
		stream.CopyTo(fs);
	}

	//For DB Update
	public static void CheckDBMigration(Window window) {
		ExportDB();
		string currentDBPath = Paths.VehiclesDBPath();
		if (!File.Exists(currentDBPath))
			return;
		string customDBPath = Paths.CustomDBPath();
		bool useCustomDB = File.Exists(customDBPath);
		string source = useCustomDB ? CustomDBSource : EmbDBSource;
		int dbVersion = GetDBVersion(source);
		int currentVersion = GetDBVersion(VehicleDBSource);
		if (dbVersion > currentVersion) {
			bool veEdited = false, vaEdited = false;
			try {
				using var conn = new SqliteConnection(VehicleDBSource);
				conn.Open();
				var cmd = conn.CreateCommand();
				cmd.CommandText = $"Select * from {TABLE_EDITED}";
				using var reader = cmd.ExecuteReader();
				if (reader.Read()) {
					veEdited = reader.GetBool(COLUMN_VEHICLES_ETS2);
					vaEdited = reader.GetBool(COLUMN_VEHICLES_ATS);
				}
				cmd.Dispose();
			} catch {
			}
			Dictionary<string, Vehicle>? keepVehiclesETS2 = null, keepVehiclesATS = null;
			if (veEdited || vaEdited) {
				var result = MessageBox.Show(window, Util.GetString("MessageDBMigration"), Util.GetString("TitleVehicleMigration"), MessageBoxButton.YesNo);
				if (result == MessageBoxResult.Yes) {
					DBMigrationWindow updateWindow = new(currentVersion, dbVersion, veEdited, vaEdited) {
						Owner = window
					};
					if (updateWindow.ShowDialog() == true) {
						keepVehiclesETS2 = updateWindow.KeepVehiclesETS2;
						keepVehiclesATS = updateWindow.KeepVehiclesATS;
					}
				}
			}
			SqliteConnection.ClearAllPools();
			if (File.Exists(currentDBPath))
				File.Delete(currentDBPath);
			File.Copy(Paths.EmbDBFile(), currentDBPath);
			if (veEdited || vaEdited) {
				string message;
				try {
					if (keepVehiclesETS2 != null)
						InsertKeepVehicles(true, keepVehiclesETS2);
					if (keepVehiclesATS != null)
						InsertKeepVehicles(false, keepVehiclesATS);
					message = Util.GetString("MessageDBMigrationResultSuccess");
				} catch {
					message = Util.GetString("MessageDBMigrationResultFailed");//建议重命名旧的数据库文件，如果途中出错，就复原，成功就删除。
				}
				MessageBox.Show(window, message, Util.GetString("MessageTitleNotice"));
			}
		}
	}

	private static int GetDBVersion(string source) {
		int version;
		using (var conn = new SqliteConnection(source)) {
			conn.Open();
			var cmd = conn.CreateCommand();
			cmd.CommandText = "PRAGMA user_version";
			version = Convert.ToInt32(cmd.ExecuteScalar() ?? 0);
			SqliteConnection.ClearAllPools();
			cmd.Dispose();
		}
		return version;
	}

	private static void ExportDB() {
		var tempDB = Paths.EmbDBFile();
		string hashFile = Paths.EmbHashFile();
		var embResDBHash = GetEmbResDBHash();
		if (File.Exists(tempDB)) {
			string? tempDBHash;
			if (File.Exists(hashFile))
				tempDBHash = File.ReadAllText(hashFile);//获取文件内的hash
			else
				tempDBHash = null;
			//检查hash，如果hash不一样或者文件不存在，就覆盖。如果hash一样就直接返回不需要更新
			if (embResDBHash == tempDBHash)
				return;
			File.Delete(tempDB);
		}
		if (File.Exists(hashFile))
			File.Delete(hashFile);

		var assembly = Assembly.GetExecutingAssembly();
		using Stream? stream = assembly.GetManifestResourceStream(defaultDB) ?? throw new FileNotFoundException("Default not found");
		DirectoryInfo di = new(tempDB);
		di.Parent!.Create();
		using FileStream fs = new(tempDB, FileMode.Create);
		stream.CopyTo(fs);

		File.WriteAllText(hashFile, embResDBHash);
	}

	private static string GetEmbResDBHash() {
		using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(defaultDB) ?? throw new FileNotFoundException("Default not found");
		using var md5 = MD5.Create();
		var hash = md5.ComputeHash(stream);
		return Convert.ToBase64String(hash);
	}


	private static void InsertKeepVehicles(bool isETS2, Dictionary<string, Vehicle> keepVehicles) {
		string table = TableNameVehicle(isETS2);
		using var conn = new SqliteConnection(VehicleDBSource);
		conn.Open();
		SqliteCommand cmd;
		using var transaction = conn.BeginTransaction();
		cmd = conn.CreateCommand();
		cmd.CommandText = @$"Insert or replace into {table} ({COLUMN_VEHICLE_ID}, {COLUMN_MANIFATURER}, {COLUMN_PROD_YEAR}, {COLUMN_VEHICLE_NAME}, {COLUMN_DESCRIPTION}, {COLUMN_DEFAULT_VEHICLE})
							values(@id, @mani, @prodYear, @tname, @desc, @default)";
		cmd.Parameters.Add(new SqliteParameter("@id", null));
		cmd.Parameters.Add(new SqliteParameter("@mani", null));
		cmd.Parameters.Add(new SqliteParameter("@prodYear", null));
		cmd.Parameters.Add(new SqliteParameter("@tname", null));
		cmd.Parameters.Add(new SqliteParameter("@desc", null));
		cmd.Parameters.Add(new SqliteParameter("@default", null));
		foreach (var vehicle in keepVehicles.Values) {
			cmd.Parameters["@id"].Value = vehicle.VehicleID;
			cmd.Parameters["@mani"].Value = vehicle.Manifaturer;
			cmd.Parameters["@prodYear"].Value = vehicle.ProductionYear;
			cmd.Parameters["@tname"].Value = vehicle.VehicleName;
			cmd.Parameters["@desc"].Value = vehicle.Description;
			cmd.Parameters["@default"].Value = 0;
			cmd.ExecuteNonQuery();
		}
		cmd.Dispose();

		//Cabin
		table = TableNameCabin(isETS2);
		cmd = conn.CreateCommand();
		cmd.CommandText = @$"Insert into {table} ({COLUMN_VEHICLE_ID}, {COLUMN_CABIN_ID}, {COLUMN_CABIN_NAME}, {COLUMN_DEFAULT_CABIN})
							values(@vID, @cID, @cName, @default)";
		cmd.Parameters.Add(new SqliteParameter("@vID", null));
		cmd.Parameters.Add(new SqliteParameter("@cID", null));
		cmd.Parameters.Add(new SqliteParameter("@cName", null));
		cmd.Parameters.Add(new SqliteParameter("@default", null));
		foreach (var vehicle in keepVehicles.Values) {
			foreach (var cabin in vehicle.Cabins) {
				if (cabin.DefaultCabin)
					continue;
				cmd.Parameters["@vID"].Value = cabin.VehicleID;
				cmd.Parameters["@cID"].Value = cabin.CabinID;
				cmd.Parameters["@cName"].Value = cabin.CabinName;
				cmd.Parameters["@default"].Value = 0;
				cmd.ExecuteNonQuery();
			}
		}
		cmd.Dispose();

		//Accessory
		table = TableNameAcc(isETS2);
		cmd = conn.CreateCommand();
		cmd.CommandText = @$"Insert into {table} ({COLUMN_VEHICLE_ID}, {COLUMN_ACC_ID}, {COLUMN_ACC_NAME}, {COLUMN_ACC_TYPE}, {COLUMN_UNLOCK_LEVEL}, {COLUMN_DEFAULT_ACC})
							values(@vID, @aID, @aName, @aType, @level, @default)";
		cmd.Parameters.Add(new SqliteParameter("@vID", null));
		cmd.Parameters.Add(new SqliteParameter("@aID", null));
		cmd.Parameters.Add(new SqliteParameter("@aName", null));
		cmd.Parameters.Add(new SqliteParameter("@aType", null));
		cmd.Parameters.Add(new SqliteParameter("@level", null));
		cmd.Parameters.Add(new SqliteParameter("@default", null));
		foreach (var vehicle in keepVehicles.Values) {
			foreach (var acc in vehicle.Accessories) {
				if (acc.DefaultAcc)
					continue;
				cmd.Parameters["@vID"].Value = acc.VehicleID;
				cmd.Parameters["@aID"].Value = acc.AccID;
				cmd.Parameters["@aName"].Value = acc.AccName;
				cmd.Parameters["@aType"].Value = acc.AccType;
				cmd.Parameters["@level"].Value = acc.UnlockLevel;
				cmd.Parameters["@default"].Value = 0;
				cmd.ExecuteNonQuery();
			}
		}
		cmd.Dispose();
		transaction.Commit();
	}

	public static void LoadVehiclesUpdate(bool isETS2, ObservableCollection<Vehicle> vehicles, bool defaultDB, bool customVehicle) {
		DBCheckBeforeUse();
		string table = TableNameVehicle(isETS2);
		string customDBPath = Paths.CustomDBPath();
		bool useCustomDB = File.Exists(customDBPath);
		string source = defaultDB ? (useCustomDB ? CustomDBSource : EmbDBSource) : VehicleDBSource;
		using var conn = new SqliteConnection(source);
		conn.Open();
		using var cmd = conn.CreateCommand();

		var cabinDict = LoadCabins(cmd, isETS2, customVehicle);
		var accDict = LoadAccessories(cmd, isETS2, customVehicle);

		StringBuilder command = new();
		command.Append($"Select * from {table}");
		if (customVehicle)
			command.Append($" where {COLUMN_DEFAULT_VEHICLE} = 0");
		command.Append($" order by {COLUMN_VEHICLE_TYPE}, {COLUMN_MANIFATURER}, {COLUMN_PROD_YEAR}, {COLUMN_VEHICLE_ID}");
		cmd.CommandText = command.ToString();
		using var reader = cmd.ExecuteReader();
		while (reader.Read()) {
			string vehicleID = reader.GetString(COLUMN_VEHICLE_ID);
			string vehicleType = reader.GetString(COLUMN_VEHICLE_TYPE);
			string manifaturer = reader.GetString(COLUMN_MANIFATURER);
			int prodYear = reader.GetInt(COLUMN_PROD_YEAR);
			string vehicleName = reader.TryGetString(COLUMN_VEHICLE_NAME, GetVehicleName(vehicleID));
			string desc = reader.TryGetString(COLUMN_DESCRIPTION, GetVehicleDesc(vehicleID));
			Vehicle vehicle = new(vehicleID, prodYear, vehicleName, desc, vehicleType, manifaturer, isETS2);
			vehicles.Add(vehicle);
			if (cabinDict.TryGetValue(vehicleID, out var cabins)) {
				vehicle.Cabins = cabins;
			}
			if (accDict.TryGetValue(vehicleID, out var accessories)) {
				vehicle.Accessories = accessories;
			}
		}
	}
}