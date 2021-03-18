using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Model.Load;

public partial class Config {
	static BagunlockTable _BagunlockTable;
	public static BagunlockTable GetBagunlockTable(){
	if(_BagunlockTable == null) LoadBagunlockTable();
		return _BagunlockTable;
	}
	public static void LoadBagunlockTable(){
		var json = SuperLoader.LoadConfig("bagunlock").text;
		_BagunlockTable = LitJson.JsonMapper.ToObject<BagunlockTable>(json);
	}
	public static void ClearBagunlockTable () {
		_BagunlockTable = null;
	}
}
public class BagunlockTableGroup {
}
public class BagunlockConfig {
	public int Id; // 解锁的格子索引
	public int Price; // 价格
}
// 道具表.xlsx
public class BagunlockTable {
	public string Name;
	public Dictionary<string, BagunlockConfig> _Datas;
	public BagunlockTableGroup _Group;
public BagunlockConfig Get(int id) {
	string k = id.ToString();
	BagunlockConfig ret;
	if (_Datas.TryGetValue(k,out ret))
		return ret;
	return null;
}
	public float data_bagunlock_vlookup_1(int id) {
	return (float)(Config.GetBagunlockTable()._Datas[id.ToString()].Id);
}
	public float data_bagunlock_vlookup_2(int id) {
	return (float)(Config.GetBagunlockTable()._Datas[id.ToString()].Price);
}
}
