using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Model.Load;

public partial class Config {
	static IteminitTable _IteminitTable;
	public static IteminitTable GetIteminitTable(){
	if(_IteminitTable == null) LoadIteminitTable();
		return _IteminitTable;
	}
	public static void LoadIteminitTable(){
		var json = SuperLoader.LoadConfig("iteminit").text;
		_IteminitTable = LitJson.JsonMapper.ToObject<IteminitTable>(json);
	}
	public static void ClearIteminitTable () {
		_IteminitTable = null;
	}
}
public class IteminitTableGroup {
}
public class IteminitConfig {
	public int Id; // 道具ID
	public string Name; // 道具名
	public int Num; // 道具数量
}
// 道具表.xlsx
public class IteminitTable {
	public string Name;
	public Dictionary<string, IteminitConfig> _Datas;
	public IteminitTableGroup _Group;
public IteminitConfig Get(int id) {
	string k = id.ToString();
	IteminitConfig ret;
	if (_Datas.TryGetValue(k,out ret))
		return ret;
	return null;
}
	public float data_iteminit_vlookup_1(int id) {
	return (float)(Config.GetIteminitTable()._Datas[id.ToString()].Id);
}
	public float data_iteminit_vlookup_3(int id) {
	return (float)(Config.GetIteminitTable()._Datas[id.ToString()].Num);
}
}
