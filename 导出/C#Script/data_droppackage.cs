using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Model.Load;

public partial class Config {
	static DroppackageTable _DroppackageTable;
	public static DroppackageTable GetDroppackageTable(){
	if(_DroppackageTable == null) LoadDroppackageTable();
		return _DroppackageTable;
	}
	public static void LoadDroppackageTable(){
		var json = SuperLoader.LoadConfig("droppackage").text;
		_DroppackageTable = LitJson.JsonMapper.ToObject<DroppackageTable>(json);
	}
	public static void ClearDroppackageTable () {
		_DroppackageTable = null;
	}
}
public class DroppackageTableGroup {
	public Dictionary<string,int[]> Order;
}
public class DroppackageConfig {
	public int RewardId; // 奖励ID，主键唯一不可重复
	public int Order; // 奖励分组
	public int RewardCondition; // 奖励类型，当类型为0时，RewardProbability指代概率（1000000为100%） 当类型为大于0的整数时，RewardProbability指代权重 且不同的整数代表不同的掉落包
	public int ItemId; // 掉落物品ID
	public int ItemNumMin; // 掉落物品数量最小
	public int ItemNumMax; // 掉落物品数量最大
	public int Probability; // 概率或权重
}
// 道具表.xlsx
public class DroppackageTable {
	public string Name;
	public Dictionary<string, DroppackageConfig> _Datas;
	public DroppackageTableGroup _Group;
	private Dictionary<string,DroppackageConfig[]> Order_Cached = new Dictionary<string,DroppackageConfig[]>();
public DroppackageConfig Get(int id) {
	string k = id.ToString();
	DroppackageConfig ret;
	if (_Datas.TryGetValue(k,out ret))
		return ret;
	return null;
}
	public DroppackageConfig[] Get_Order(int Order) {
string cach_key = string.Empty+Order+"_";
DroppackageConfig[] ret;
if(Order_Cached.TryGetValue(cach_key,out ret))
	return ret;
if (_Group.Order.ContainsKey(Order.ToString()) ){
var ids = _Group.Order[Order.ToString()];
var configs = new DroppackageConfig[ids.Length];
for (int i = 0; i < ids.Length; i++) {
	var id = ids[i];
	configs[i] = Get(id);
}
	Order_Cached[cach_key]=configs;
return configs;
}
return new DroppackageConfig[0];
}
	public float data_droppackage_vlookup_1(int id) {
	return (float)(Config.GetDroppackageTable()._Datas[id.ToString()].RewardId);
}
	public float data_droppackage_vlookup_2(int id) {
	return (float)(Config.GetDroppackageTable()._Datas[id.ToString()].Order);
}
	public float data_droppackage_vlookup_4(int id) {
	return (float)(Config.GetDroppackageTable()._Datas[id.ToString()].RewardCondition);
}
	public float data_droppackage_vlookup_5(int id) {
	return (float)(Config.GetDroppackageTable()._Datas[id.ToString()].ItemId);
}
	public float data_droppackage_vlookup_6(int id) {
	return (float)(Config.GetDroppackageTable()._Datas[id.ToString()].ItemNumMin);
}
	public float data_droppackage_vlookup_7(int id) {
	return (float)(Config.GetDroppackageTable()._Datas[id.ToString()].ItemNumMax);
}
	public float data_droppackage_vlookup_8(int id) {
	return (float)(Config.GetDroppackageTable()._Datas[id.ToString()].Probability);
}
}
