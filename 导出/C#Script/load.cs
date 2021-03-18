using System;
using System.Collections;
using System.Collections.Generic;

public partial class Config {
	public static void Load() {
	Config.LoadBagunlockTable();
	Config.LoadDroppackageTable();
	Config.LoadIteminitTable();
	Config.LoadItemTable();
}
	public static void Clear() {
	Config.ClearBagunlockTable();
	Config.ClearDroppackageTable();
	Config.ClearIteminitTable();
	Config.ClearItemTable();
}
	public static void Save() {
}
}
